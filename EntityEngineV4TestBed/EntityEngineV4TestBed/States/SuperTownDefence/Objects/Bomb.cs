using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Collision;
using EntityEngineV4.Components.Render;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using EntityEngineV4.Object;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.SuperTownDefence.Objects
{
    public sealed class Bomb : Entity
    {
        //Components
        public Body Body;
        public Physics Physics;
        public BasicCollision Collision;
        public ImageRender ImageRender;

        private Animation _explodeanim;
        private Sound _explodesound;

        private ExplosionEmitter _explosionemitter;
        private SmokeEmitter _smokeemitter;

        //Data
        public bool IsExploding { get; private set; }

        private float _gravity = .1f;
        public float Damage = 1;

        public Bomb(EntityState es, string name)
            : base(es, name)
        {
            string path = es.Name + "->" + "Bomb";

            Name = Name + Id;

            Body = new Body(this, "Body");
            Body.Bounds = new Vector2(5,5);

            Physics = new Physics(this, "Physics", Body);

            Collision = new BasicCollision(this, "Collision", Body);
            Collision.CollideEvent += CollisionHandler;

            ImageRender = new ImageRender(this, "ImageRender", Body);
            ImageRender.LoadTexture(@"SuperTownDefence/game/bomb");

            _explodeanim = new Animation(this, "ExplodeAnim", Body);
            _explodeanim.LastFrameEvent += () => Destroy();

            _explodesound = new Sound(this, "ExplodeSound");

            _explosionemitter = new ExplosionEmitter(this, Body);

            _smokeemitter = new SmokeEmitter(this, Body);

            //TODO: Hook up Collision.CollideEvent to a handler
            _explodeanim.Origin = new Vector2(_explodeanim.TileSize.X / 2.0f, _explodeanim.TileSize.Y / 2.0f);
            ImageRender.Origin = new Vector2(ImageRender.Texture.Width / 2f, ImageRender.Texture.Height / 2f);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (!IsExploding)
            {
                Physics.Velocity.Y += _gravity;
                Physics.FaceVelocity();
                _smokeemitter.Emit(1);
                if (Body.Position.Y > 510)
                    IsExploding = true;
            }
            if (IsExploding)
            {
                Physics.Velocity = Vector2.Zero;
                if (!_explodeanim.Active)
                {
                    ImageRender.Active = false;
                    _explodeanim.Active = true;
                    _explodeanim.Start();
                    Body.Position -= _explodeanim.Origin * _explodeanim.Scale;
                    Body.Bounds.X = (int)_explodeanim.TileSize.X;
                    Body.Bounds.Y = (int)_explodeanim.TileSize.Y;
                    _explosionemitter.Emit(20);
                    _explodesound.Play();
                }
            }
        }

        public void CollisionHandler(Entity e)
        {
            e.GetComponent<Health>().Hurt((int)Damage);
        }

        class ExplosionEmitter : Emitter
        {
            Random _rand = new Random(DateTime.Now.Millisecond);
            public ExplosionEmitter(Entity e, Body body)
                : base(e, "ExplosionEmitter", body)
            {

            }

            protected override Particle GenerateNewParticle()
            {
                var index = _rand.Next(0, 3);
                var ttl = _rand.Next(50, 80);

                var p = new ExplosionParticle(Parent.StateRef, Body.Position + (Body.Bounds / 2f), ttl, this);
                p.TileRender.Index = index;
                p.Body.Angle = (float)_rand.NextDouble() - .5f * 1.5f;
                p.Physics.Thrust(((float)_rand.NextDouble() + 1f) * 2.5f);
                p.TileRender.Layer = .5f;
                return p;
            }
        }

        class ExplosionParticle : FadeParticle
        {
            public ExplosionParticle(EntityState stateref, Vector2 position, int ttl, Emitter e) : base(stateref, stateref, position, ttl-10, ttl, e)
            {
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                Physics.Velocity.Y += .1f;
            }
        }

        private class SmokeEmitter : Emitter
        {
            private Random _rand = new Random(DateTime.Now.Millisecond);
            private List<Color> Colors;

            public SmokeEmitter(Entity e, Body body)
                : base(e, "SmokeEmitter", body)
            {
                Colors = new List<Color>();
                Colors.Add(Color.Gray);
                Colors.Add(Color.DarkGray);
                Colors.Add(Color.LightGray);
                Colors.Add(Color.LightSlateGray);
                Colors.Add(Color.SlateGray);
            }

            protected override Particle GenerateNewParticle()
            {
                int index = _rand.Next(0, 3);
                int ttl = _rand.Next(40, 80);
                //Rotate the point based on the center of the sprite
                // p = unrotated point, o = rotation origin
                //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
                //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy

                var origin =  Body.Position + Body.Bounds/2f;

                //var unrotatedposition = new Vector2(
                // Entity.GetComponent<Render>().DrawRect.X + (Entity.GetComponent<Render>().DrawRect.Width/2f) * Entity.GetComponent<Render>().Scale.X,
                // Entity.GetComponent<Render>().DrawRect.Bottom);

                var unrotatedposition = new Vector2(Body.Position.X + Body.Bounds.X/2f,
                                                    Body.Position.Y + Body.Bounds.Y*.75f);

                var angle = Body.Angle;

                var position = new Vector2(
                    (float)
                    (Math.Cos(angle) * (unrotatedposition.X - origin.X) - Math.Sin(angle) * (unrotatedposition.Y - origin.Y) +
                     origin.X),
                    (float)
                    (Math.Sin(angle) * (unrotatedposition.X - origin.X) + Math.Cos(angle) * (unrotatedposition.Y - origin.Y) +
                     origin.Y)
                    );

                FadeParticle p = new FadeParticle(Parent.StateRef, Parent.StateRef, ttl, this);
                p.TileRender.Index = index;
                p.Body.Position = position;
                p.Body.Angle = (float)_rand.NextDouble() / 2 - .25f;
                p.Physics.Thrust((float)_rand.NextDouble() + .1f);
                p.TileRender.Layer = .5f;
                p.TileRender.Color = Colors[_rand.Next(0, Colors.Count)];
                return p;
            }
        }
    }
}
