using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.FireworkTest
{
    public class FireworkTestState : TestBedState
    {
        public static Random Random = new Random();
        public const float GRAVITY = .1f;


        public FireworkTestState() : base("FireworkTestState")
        {
        }


        public override void Create()
        {
            base.Create();

            //Entities
            new FireworkSpawner(this);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        //private classes
        private class FireworkSpawner : Entity
        {
            public Body Body;
            public Timer LaunchTimer;
            public FireworkEmitter FireworkEmitter;

            public ShapeTypes.Rectangle Render;

            public readonly Vector2 StartPosition = new Vector2(50, 500);
            public readonly Vector2 StopPosition = new Vector2(550, 500);

            public FireworkSpawner(IComponent parent) : base(parent, "FireworkSpawner")
            {
                Body = new Body(this, "Body", StartPosition);
                Body.Width = 10;
                Body.Height = 15;

                Render = new ShapeTypes.Rectangle(this, "Render", Body, true);
                Render.Color = Color.Black;

                LaunchTimer = new Timer(this, "LaunchTimer");
                LaunchTimer.Milliseconds = 5000;
                LaunchTimer.LastEvent += ShootFirework;
                LaunchTimer.Start();

                FireworkEmitter = new FireworkEmitter(this, "FireworkEmitter");

                ShootFirework();
            }

            private void ShootFirework()
            {
                //Go to a random position
                Body.Position = new Vector2(Random.Next((int)StartPosition.X, (int)StopPosition.X), StartPosition.Y);

                FireworkEmitter.Emit();

                EntityGame.Log.Write("Firework launched", this, Alert.Info);
            }
        }

        private class FireworkEmitter : Emitter
        {
            private Body _body;

            private Random _random = new Random();

            public FireworkEmitter(IComponent parent, string name) : base(parent, name)
            {
                //Get our parent's classes
                _body = (parent as Entity).GetComponent<Body>();
            }

            protected override Particle GenerateNewParticle()
            {
                Firework firework = new Firework(this, new Vector2(), 3000);
                firework.Body.Position = new Vector2(_body.X + _body.Width/2, _body.Y);
                firework.Body.Angle = (float) _random.NextDouble()*MathHelper.PiOver4/8*_random.GetSign();
                firework.Physics.Thrust(_random.Next(7,10));
                firework.Physics.FaceVelocity();

                firework.BodyEmitter = new SparklingEmitter(firework, "BodyEmitter");
                return firework;
            }
        }

        private class Firework : Particle
        {
            public Body Body;
            public Physics Physics;
            public ShapeTypes.Rectangle Render;

            public Emitter BodyEmitter;
            public Emitter ExplosionEmitter;

            public Firework(Emitter e, Vector2 position, int ttl) : base(e, ttl)
            {
                Body = new Body(this, "Body", position);
                Body.Width = 3;
                Body.Height = 3;

                Render = new ShapeTypes.Rectangle(this, "Render", Body, true);
                Render.Color = Color.Red;

                Physics = new Physics(this, "Physics", Body);
                Physics.Acceleration.Y = GRAVITY;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                BodyEmitter.Emit(20);
            }

            public override void AddEntity(Entity c)
            {
                base.AddEntity(c);
            }
        }

        #region Body Type Emitters
        private class SparklingEmitter : Emitter
        {


            public SparklingEmitter(IComponent parent, string name)
                : base(parent, name)
            {
            }

            protected override Particle GenerateNewParticle()
            {
                SparklingParticle sparkling = new SparklingParticle(this, Random.Next(100, 500));
                return sparkling;
            }

            public override void AddEntity(Entity c)
            {
                base.AddEntity(c);
            }

            private class SparklingParticle : Particle
            {
                public Body Body;
                public Physics Physics;
                public ShapeTypes.Rectangle Render;

                private Emitter _miniSparkEmitter;
                private Timer _miniSparkTimer;

                private int _maxLength;
                private float _currentlength
                {
                    get { return _maxLength - _maxLength * _miniSparkTimer.Progress; }
                }

                public SparklingParticle(Emitter e, int ttl)
                    : base(e, ttl)
                {
                    Body = new Body(this, "Body");
                    //Get random length and angle
                    Body.Height = 2;
                    Body.Width = Random.Next(4, 8);
                    _maxLength = (int)Body.Width;

                    Physics = new Physics(this, "Physics", Body);
                    Physics.Acceleration.Y = GRAVITY;
                    //Get random thrust 
                    Physics.Thrust(Random.GetFloat(10, 20));

                    Render = new ShapeTypes.Rectangle(this, "Render", Body, true);
                    Render.Color = Color.RoyalBlue;
                    Body.Width = 1;
                    Body.Height = 1;

                    _miniSparkEmitter = new MiniSparkEmitter(this, "MiniSparkEmitter");

                    _miniSparkTimer = new Timer(this, "MiniSparkTimer");
                    _miniSparkTimer.Milliseconds = 50;
                    _miniSparkTimer.LastEvent += ShootSparks;
                    _miniSparkTimer.Start();
                }

                private void ShootSparks()
                {
                    //if(_random.RandomBool())
                    //    _miniSparkEmitter.Emit(10);
                    Destroy();
                }

                public override void Update(GameTime gt)
                {
                    base.Update(gt);

                    //Face the spark towards velocity
                    Physics.FaceVelocity();

                    Body.Width = _currentlength;
                }

                private class MiniSparkEmitter : Emitter
                {
                    public MiniSparkEmitter(IComponent parent, string name)
                        : base(parent, name)
                    {
                    }

                    private class MiniSpark : Particle
                    {
                        public MiniSpark(Emitter e, int ttl)
                            : base(e, ttl)
                        {
                        }
                    }
                }

            }
        }
        #endregion
    }
}
