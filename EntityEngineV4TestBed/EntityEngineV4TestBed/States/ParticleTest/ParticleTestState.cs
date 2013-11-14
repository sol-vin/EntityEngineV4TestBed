using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering.Primitives;
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
            EntityGame.DebugInfo.Render.Color = Color.White;
        }

        public override void Initialize()
        {
            base.Initialize();

            new ParticleNode(this, "Particle");

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
                Body.X = -100;
                Body.Y = -100;

                Physics = new Physics(this, "Physics");
                Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

                Render = new ShapeTypes.Rectangle(this, "Render", true);
                Render.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);
                Render.Color = Color.OrangeRed;

                _emitter = new ParticleEmitter(this, "Emitter");
                _emitter.LinkDependency(ParticleEmitter.DEPENDENCY_BODY,Body);
            }

            private float _angle;
            private const float _AMPLITUDE = 10;
            private const float _SPEED = .01f;
            public const float YOFFSET = 100;
            public override void Update(GameTime gt)
            {
                base.Update(gt);

                
                _angle += _SPEED;

                Body.Position = MathTools.Physics.RotatePoint(
                    new Vector2(EntityGame.Viewport.Width/2, EntityGame.Viewport.Height/2),
                    _angle,
                    new Vector2(EntityGame.Viewport.Width / 2, YOFFSET));
                Body.Angle = _angle;

                _emitter.Emit(3);
            }

            public override void Destroy(IComponent sender = null)
            {
                base.Destroy(sender);
                EntityGame.DebugInfo.Render.Color = Color.Black;
            }

            private class ParticleEmitter : Spawner
            {
                private Random _random = new Random();
                public ParticleEmitter(Node parent, string name) : base(parent, name)
                {
                }

                protected override Spawn GenerateNewParticle()
                {
                    TrailParticle p = new TrailParticle(this, 1000);
                    p.Body.Position = MathTools.Physics.RotatePoint(
                    new Vector2(EntityGame.Viewport.Width / 2, EntityGame.Viewport.Height / 2),
                    GetDependency<Body>(DEPENDENCY_BODY).Angle,
                    new Vector2(EntityGame.Viewport.Width / 2, YOFFSET+GetDependency<Body>(DEPENDENCY_BODY).Bounds.Y/2));
                    p.Body.Angle = GetDependency<Body>(DEPENDENCY_BODY).Angle + MathHelper.Pi - 
                        (MathHelper.Pi/16 * ((_random.Next(0,2) == 0) ? 1 : -1) 
                        * (float)_random.NextDouble());
                    p.Body.Bounds = new Vector2(_random.Next(2,10));

                    p.Physics.Thrust((float)_random.NextDouble() * 2);
                    p.Physics.AngularVelocity = (float) _random.NextDouble()/2f;
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

                public float ScaleSpeed = .05f;
                
                public TrailParticle(Spawner parent, int ttl) : base(parent, ttl)
                {
                    Body = new Body(this, "Body");

                    Physics = new Physics(this, "Physics");
                    Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

                    Render = new ShapeTypes.Rectangle(this, "Render", true);
                    Render.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);
                    Render.Color = Color.OrangeRed;
                    Render.Origin = new Vector2(.5f, .5f);
                }
            }
        }
    }
}
