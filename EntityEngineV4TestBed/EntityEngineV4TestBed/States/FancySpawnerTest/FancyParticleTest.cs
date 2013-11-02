using System;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering.Primitives;

using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.FancyParticleTest
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

            new FancyEntity(this, "FE");

            //GUI
            var countlabel = new Label(this, "countlabel");
            countlabel.TabPosition = new Point(0,0);
            countlabel.X = 0;
            countlabel.Y = EntityGame.Viewport.Height - countlabel.Height;
            NodeAdded += e => countlabel.Text = "Active: " + (ActiveNodes-5);
            NodeRemoved += e => countlabel.Text = "Active: " + (ActiveNodes-5); 
            countlabel.AttachToControlHandler();
        }

        private class FancyEntity : Entity
        {
            public FancySpawner FancySpawnerMouse, FancySpawnerAuto;
            public Body MouseBody, AutoBody;
            private Random _rand = new Random();

            private DoubleInput _emitkey;

            public FancyEntity(State stateref, string name)
                : base(stateref, name)
            {
                MouseBody = new Body(this, "MouseBody");
                AutoBody = new Body(this, "AutoBody");
                FancySpawnerMouse = new FancySpawner(this, "FancySpawnerMouse", MouseBody);
                FancySpawnerMouse.Color = Color.Red;

                FancySpawnerAuto = new FancySpawner(this, "FancySpawnerAuto", AutoBody);
                FancySpawnerAuto.Color = Color.Blue;
                FancySpawnerAuto.AutoEmit = true;
                FancySpawnerAuto.AutoEmitAmount = 1;

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

            public FancySpawner(Entity e, string name, Body body)
                : base(e, name)
            {
                _body = body;
            }

            protected override Spawn GenerateNewParticle()
            {
                var p = new ExplodingSpawn(this, 60000, Color);
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
                return p;
            }

            private class ExplodingSpawn : Spawn
            {
                private int _floor = EntityGame.Viewport.Height - 20;

                public Body Body;
                public Physics Physics;
                public ShapeTypes.Rectangle RectRender;
                public Spawner GibEmit;

                public ExplodingSpawn(Spawner e, int ttl, Color color)
                    : base(e, ttl)
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
                        Destroy();
                    }
                }

                private class GibSpawner : Spawner
                {
                    private Random _rand = new Random(DateTime.Now.Millisecond ^ DateTime.Now.Second);
                    private Color _color;

                    public GibSpawner(Entity parent, string name, Color color)
                        : base(parent, name)
                    {
                        _color = color;
                    }

                    protected override Spawn GenerateNewParticle()
                    {
                        var p = new GibSpawn(this);
                        p.Body.Bounds = new Vector2(2, 2);
                        p.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position;

                        int sign = _rand.Next(0, 2) == 0 ? -1 : 1;
                        p.Body.Angle = (float)_rand.NextDouble() * MathHelper.PiOver2 * sign;

                        float thrust = ((float)_rand.NextDouble() + 1f) * (GetDependency<Physics>(DEPENDENCY_PHYSICS).Velocity.Y / 4);
                        p.Physics.Thrust(thrust);
                        p.Physics.Acceleration = new Vector2(0, .1f);
                        //p.RectRender.Scale = new Vector2(0);
                        p.RectRender.Color = _color;
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
                    private int _floor = EntityGame.Viewport.Height - 20;

                    public Body Body;
                    public Physics Physics;
                    public ShapeTypes.Rectangle RectRender;

                    public GibSpawn(Spawner e)
                        : base(e, 3000)
                    {
                        FadeAge = TimeToLive / 5 * 4;
                        Body = new Body(this, "Body");
                        Physics = new Physics(this, "Physics");
                        Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);


                        RectRender = new ShapeTypes.Rectangle(this, "RectRender", true);
                        RectRender.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);

                        Render = RectRender;
                    }

                    public override void Update(GameTime gt)
                    {
                        base.Update(gt);

                        Physics.FaceVelocity();

                        if (Body.Right < EntityGame.Camera.ScreenSpace.Left || Body.Left > EntityGame.Camera.ScreenSpace.Right)
                            Destroy();

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
                }
            }
        }
    }
}