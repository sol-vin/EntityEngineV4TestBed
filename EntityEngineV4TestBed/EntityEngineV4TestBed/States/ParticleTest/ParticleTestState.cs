using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.ParticleTest
{
    public class ParticleTestState : TestBedState
    {
        public ParticleTestState() : base("ParticleTestState")
        {
            EntityGame.DebugInfo.Color = Color.White;
        }

        public override void Initialize()
        {
            base.Initialize();

            new ParticleNode(this, "Particle1");

            new ParticleNode(this, "Particle2") {Speed = -5, OffsetY = 50};
            new ParticleNode(this, "Particle3") { Speed = -5f, OffsetY = 150 };

            EntityGame.BackgroundColor = Color.Black;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public class ParticleNode : Node
        {
            public Body Body;
            public Physics Physics;
            public ShapeTypes.Rectangle Render;
            private ParticleEmitter _emitter;

            public ParticleNode(Node parent, string name) : base(parent, name)
            {
                Body = new Body(this, "Body");
                Body.Width = 10;
                Body.Height = 10;
                Body.Y = OffsetY;

                Physics = new Physics(this, "Physics");
                Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

                Render = new ShapeTypes.Rectangle(this, "Render", true);
                Render.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);
                Render.Color = Color.OrangeRed;

                _emitter = new ParticleEmitter(this, "Emitter");
                _emitter.LinkDependency(ParticleEmitter.DEPENDENCY_BODY,Body);
            }

            private float _angle;
            public float Speed = 5;
            public float OffsetY = 100;
            public override void Update(GameTime gt)
            {
                base.Update(gt);
                Render.Color = RandomHelper.RandomHue();
                
                _angle += Speed * (float)gt.ElapsedGameTime.TotalSeconds;

                Body.Position = Physics.RotatePoint(
                    new Vector2(EntityGame.Viewport.Width/2f, EntityGame.Viewport.Height/2f),
                    _angle,
                    new Vector2(EntityGame.Viewport.Width / 2f, OffsetY));
                Body.Angle = _angle;

                _emitter.Emit(3);
            }

            public override void Destroy(IComponent sender = null)
            {
                base.Destroy(sender);
                EntityGame.DebugInfo.Color = Color.Black;
            }

            private class ParticleEmitter : Spawner
            {
                public ParticleEmitter(Node parent, string name) : base(parent, name)
                {
                }

                protected override Spawn GenerateNewParticle()
                {
                    var p = GetRoot<State>().GetNextRecycled<TrailParticle>(this, "RecycledParticle") ?? new TrailParticle(this, 3000);
                    p.Body.Position = Physics.RotatePoint(
                    new Vector2(EntityGame.Viewport.Width / 2f, EntityGame.Viewport.Height / 2f),
                    GetDependency<Body>(DEPENDENCY_BODY).Angle,
                    new Vector2(EntityGame.Viewport.Width / 2f, (Parent as ParticleNode).OffsetY + GetDependency<Body>(DEPENDENCY_BODY).Bounds.Y/2));
                    p.Body.Angle = GetDependency<Body>(DEPENDENCY_BODY).Angle - MathHelper.PiOver2 +
                        RandomHelper.GetFloat(-MathHelper.Pi / 2f, MathHelper.Pi / 2f);
                    p.Body.Bounds = new Vector2(RandomHelper.NextInt(2,10));

                    p.Physics.Thrust(RandomHelper.GetFloat() * 100);
                    p.Physics.AngularVelocity = RandomHelper.GetFloat() * 10;

                    

                    return p;
                }
                
                //Dependencies
                public const int DEPENDENCY_BODY = 0;

                public override void CreateDependencyList()
                {
                    base.CreateDependencyList();
                    AddLinkType(DEPENDENCY_BODY, typeof(Body));
                }
            }

            private class TrailParticle : FadeSpawn
            {
                public Body Body;
                public Physics Physics;

                public float ScaleSpeed = .01f;

                public override bool IsObject
                {
                    get { return true; }
                }

                public TrailParticle(Node parent, int ttl) : base(parent, ttl)
                {
                    Body = new Body(this, "Body");
                    Body.Origin = new Vector2(.5f, .5f);

                    Physics = new Physics(this, "Physics");
                    Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

                    Render = new ShapeTypes.Rectangle(this, "Render", true);
                    Render.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);
                    Render.Color = Color.OrangeRed;

                    DeathTimer.LastEvent += () => Destroy();
                }

                public override void Update(GameTime gt)
                {
                    base.Update(gt);

                    HSVColor color = Render.Color.ToHSVColor();
                    color.Action = ColorOutOfBoundsAction.WrapAround;
                    color.H += RandomHelper.GetFloat(-.1f, .1f);
                    Render.Color = color.ToColor();
                }
            }
        }
    }
}
