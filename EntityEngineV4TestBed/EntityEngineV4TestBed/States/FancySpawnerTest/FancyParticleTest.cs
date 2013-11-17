using System;
using System.Linq;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.FancySpawnerTest
{
    public class FancySpawnerTestState : TestBedState
    {
        public FancySpawnerTestState()
            : base("FancyParticleTest")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            //Init services
            new ControlHandler(this);

            new FancyNode(this, "FE");
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        private class FancyNode : Node
        {
            public FancySpawner FancySpawnerMouse, FancySpawnerAuto;
            private Timer _spawnTimer;
            public Body MouseBody, AutoBody;
            private Random _rand = new Random();

            private DoubleInput _emitkey;

            public FancyNode(Node parent, string name)
                : base(parent, name)
            {
                MouseBody = new Body(this, "MouseBody");
                AutoBody = new Body(this, "AutoBody");
                FancySpawnerMouse = new FancySpawner(this, "FancySpawnerMouse", MouseBody);
                FancySpawnerMouse.Color = Color.Red;

                FancySpawnerAuto = new FancySpawner(this, "FancySpawnerAuto", AutoBody);
                FancySpawnerAuto.Color = Color.Blue;

                _spawnTimer = new Timer(this, "SpwnTimer");
                _spawnTimer.Milliseconds = 25;
                _spawnTimer.LastEvent += () => FancySpawnerAuto.Emit(1);
                _spawnTimer.Start();

                _emitkey = new DoubleInput(this, "emitkey", Keys.Space, Buttons.A, PlayerIndex.One);
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                AutoBody.Position = new Vector2(_rand.Next(40, EntityGame.Viewport.Width - 40), -30);

                if (MouseService.Cursor.Down())
                {
                    MouseBody.Position = new Vector2(MouseService.Cursor.Position.X, MouseService.Cursor.Position.Y);
                    FancySpawnerMouse.Emit(1);
                }
                if (_emitkey.Released()) FancySpawnerAuto.AutoEmit = !FancySpawnerAuto.AutoEmit;
            }
        }

        private class FancySpawner : Spawner
        {
            private Random _rand = new Random();
            private Body _body;
            public Color Color = Color.White;

            public FancySpawner(Node e, string name, Body body)
                : base(e, name)
            {
                _body = body;
            }

            protected override Spawn GenerateNewParticle()
            {
                var p = GetRoot<State>().GetNextRecycled<ExplodingSpawn>(GetRoot(), "RecycledExplode") ?? new ExplodingSpawn(GetRoot(), 60000, Color);
                p.Body.Bounds = new Vector2(5, 5);
                p.Body.Position = _body.Position;

                p.Body.Angle = (float)_rand.NextDouble() * MathHelper.TwoPi;

                float thrust = (float)_rand.NextDouble() * 4f;
                while (Math.Abs(thrust) < .00001f)
                {
                    thrust = (float)_rand.NextDouble() * 4f;
                }
                p.Physics.Thrust(thrust);
                p.Physics.Acceleration = new Vector2(0, .1f);
                //p.RectRender.Scale = new Vector2(0);
                p.RectRender.Color = p.Name == "RecycledExplode" ? Color.Red : Color.Blue;
                return p;
            }

            private class ExplodingSpawn : Spawn
            {
                public override bool IsObject
                {
                    get { return true; }
                }

                public override bool Recyclable
                {
                    get { return true; }
                }

                private int _floor = EntityGame.Viewport.Height - 20;

                public Body Body;
                public Physics Physics;
                public ShapeTypes.Rectangle RectRender;
                public Spawner GibEmit;

                public ExplodingSpawn(Node parent, int ttl, Color color)
                    : base(parent, ttl)
                {
                    Body = new Body(this, "Body");
                    Physics = new Physics(this, "Physics");
                    Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

                    RectRender = new ShapeTypes.Rectangle(this, "RectRender", true);
                    RectRender.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);
                    RectRender.Color = color;

                    GibEmit = new GibSpawner(this, "GibSpawner", color);
                    GibEmit.LinkDependency(GibSpawner.DEPENDENCY_BODY, Body);
                    GibEmit.LinkDependency(GibSpawner.DEPENDENCY_PHYSICS, Physics);
                }

                public override void Update(GameTime gt)
                {
                    base.Update(gt);

                    Physics.FaceVelocity();

                    if (Body.BoundingRect.Bottom > _floor)
                    {
                        //Find penetration depth
                        float depth = Body.Bottom - _floor;

                        //Move out of the floor, add a little extra for safety
                        Body.Position.Y -= depth + .1f;
                        GibEmit.Emit(20);
                        Recycle();
                    }
                }

                private class GibSpawner : Spawner
                {
                    private Random _rand = new Random(DateTime.Now.Millisecond ^ DateTime.Now.Second);
                    private Color _color;

                    public GibSpawner(Node parent, string name, Color color)
                        : base(parent, name)
                    {
                        _color = color;
                    }

                    protected override Spawn GenerateNewParticle()
                    {
                        var p = GetRoot<State>().GetNextRecycled<GibSpawn>(GetRoot(), "RecycledGib") ??  new GibSpawn(GetRoot());
                        p.Body.Bounds = new Vector2(2, 2);
                        p.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position;

                        int sign = _rand.Next(0, 2) == 0 ? -1 : 1;
                        p.Body.Angle = (float)_rand.NextDouble() * MathHelper.PiOver2 * sign;

                        float thrust = ((float)_rand.NextDouble() + 1f) * (GetDependency<Physics>(DEPENDENCY_PHYSICS).Velocity.Y / 4);
                        p.Physics.Thrust(thrust);
                        p.Physics.Acceleration = new Vector2(0, .1f);
                        //p.RectRender.Scale = new Vector2(0);
                        p.RectRender.Color = p.Name == "RecycledGib" ? Color.Red : Color.Blue;
                        return p;
                    }

                    public override void Emit(int amount)
                    {
                        base.Emit(amount);
                    }

                    //Dependencies
                    public const int DEPENDENCY_BODY = 0;
                    public const int DEPENDENCY_PHYSICS = 1;

                    public override void CreateDependencyList()
                    {
                        base.CreateDependencyList();
                        AddLinkType(DEPENDENCY_BODY, typeof(Body));
                        AddLinkType(DEPENDENCY_PHYSICS, typeof(Physics));
                    }
                }

                private class GibSpawn : FadeSpawn
                {
                    public override bool IsObject
                    {
                        get { return true; }
                    }

                    public override bool Recyclable
                    {
                        get { return true; }
                    }

                    private int _floor = EntityGame.Viewport.Height - 20;

                    public Body Body;
                    public Physics Physics;
                    public ShapeTypes.Rectangle RectRender;

                    public GibSpawn(Node parent)
                        : base(parent, 3000)
                    {
                        FadeAge = TimeToLive / 5 * 4;
                        Body = new Body(this, "Body");
                        Physics = new Physics(this, "Physics");
                        Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);


                        RectRender = new ShapeTypes.Rectangle(this, "RectRender", true);
                        RectRender.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);

                        Render = RectRender;

                        DeathTimer.LastEvent += Recycle;
                    }

                    public override void Update(GameTime gt)
                    {
                        base.Update(gt);

                        Physics.FaceVelocity();

                        if (Body.Right < EntityGame.Camera.ScreenSpace.Left || Body.Left > EntityGame.Camera.ScreenSpace.Right)
                            Recycle();

                        if (Body.BoundingRect.Bottom > _floor)
                        {
                            //Find penetration depth
                            float depth = Body.Bottom - _floor;

                            //Move out of the floor, add a little extra for safety
                            Body.Position.Y -= depth + .1f;
                            Physics.Velocity.Y = -Physics.Velocity.Y * .2f; //Add restitution
                        }
                        //Add friction if it's on the ground
                        if (Math.Abs(Physics.Velocity.Y) < .01f && (Body.Bottom < _floor + 1))
                        {
                            Physics.Velocity.X *= .9f;
                            Physics.Velocity.Y = 0;
                        }
                    }

                    public override void Reuse(Node parent, string name)
                    {
                        base.Reuse(parent, name);
                        DeathTimer.Milliseconds = 3000;
                        DeathTimer.LastEvent += Recycle;
                        DeathTimer.Start();
                    }
                }
            }
        }
    }
}