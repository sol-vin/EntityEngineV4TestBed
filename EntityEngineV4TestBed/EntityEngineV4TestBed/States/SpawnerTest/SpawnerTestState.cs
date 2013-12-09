using System;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Debugging;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.ParticleTest
{
    public class SpawnerTestState : TestBedState
    {
        private Label _screeninfo;

        private Label _strengthText, _strengthValue;
        private LinkLabel _strengthUp, _strengthDown;

        private Label _gravityText, _gravityXValue, _gravityYValue;
        private LinkLabel _gravityXDown, _gravityYDown, _gravityXUp, _gravityYUp;

        private const float STRENGTHSTEP = .1f;
        private const float GRAVITYSTEP = .1f;
        private ParticleTestManager _ptm;

        public static Random Random = new Random();

        public SpawnerTestState()
            : base("SpawnerTestState")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            ControlHandler ch = new ControlHandler(this);

            _ptm = new ParticleTestManager(this);

            var page = new Page(this, "Page");
            page.Show();

            _screeninfo = new Label(page, "ScreenInfo", new Point(0, 0));
            _screeninfo.Body.Position = Vector2.One * 20;

            _strengthText = new Label(page, "StrengthText", new Point(0, 1));
            _strengthText.Text = "Strength:";
            _strengthText.Body.Position = new Vector2(20, 50);

            _strengthDown = new LinkLabel(page, "StrengthDown", new Point(1, 1));
            _strengthDown.Text = "<-";
            _strengthDown.Body.Position = new Vector2(_strengthText.Body.BoundingRect.Right + 5, 50);
            _strengthDown.OnDown += control => _ptm.Spawner.Strength -= STRENGTHSTEP;

            _strengthValue = new Label(page, "StrengthValue", new Point(2, 1));
            _strengthValue.Text = _ptm.Spawner.Strength.ToString();
            _strengthValue.Body.Position = new Vector2(_strengthDown.Body.BoundingRect.Right + 5, 50);

            _strengthUp = new LinkLabel(page, "StrengthUp", new Point(3, 1));
            _strengthUp.Text = "->";
            _strengthUp.Body.Position = new Vector2(_strengthValue.Body.BoundingRect.Right + 5, 50);
            _strengthUp.OnDown += control => _ptm.Spawner.Strength += STRENGTHSTEP;

            _gravityText = new Label(page, "GravityText", new Point(0, 2));
            _gravityText.Text = "Gravity:";
            _gravityText.Body.Position = new Vector2(20, 80);

            _gravityXDown = new LinkLabel(page, "GravityXDown", new Point(1, 2));
            _gravityXDown.Text = "<-";
            _gravityXDown.Body.Position = new Vector2(_gravityText.Body.BoundingRect.Right + 5, 80);
            _gravityXDown.OnDown += control => _ptm.Spawner.Acceleration.X -= GRAVITYSTEP;

            _gravityXValue = new Label(page, "GravityXValue", new Point(2, 2));
            _gravityXValue.Text = "X:" + _ptm.Spawner.Acceleration.X.ToString();
            _gravityXValue.Body.Position = new Vector2(_gravityXDown.Body.BoundingRect.Right + 5, 80);

            _gravityXUp = new LinkLabel(page, "GravityXUp", new Point(3, 2));
            _gravityXUp.Text = "->";
            _gravityXUp.Body.Position = new Vector2(_gravityXValue.Body.BoundingRect.Right + 5, 80);
            _gravityXUp.OnDown += control => _ptm.Spawner.Acceleration.X += GRAVITYSTEP;

            _gravityYDown = new LinkLabel(page, "GravityYDown", new Point(1, 3));
            _gravityYDown.Text = "<-";
            _gravityYDown.Body.Position = new Vector2(_gravityText.Body.BoundingRect.Right + 5, 110);
            _gravityYDown.OnDown += control => _ptm.Spawner.Acceleration.Y -= GRAVITYSTEP;

            _gravityYValue = new Label(page, "GravityYValue", new Point(2, 3));
            _gravityYValue.Text = "Y:" + _ptm.Spawner.Acceleration.Y.ToString();
            _gravityYValue.Body.Position = new Vector2(_gravityYDown.Body.BoundingRect.Right + 5, 110);

            _gravityYUp = new LinkLabel(page, "GravityYUp", new Point(3, 3));
            _gravityYUp.Text = "->";
            _gravityYUp.Body.Position = new Vector2(_gravityYValue.Body.BoundingRect.Right + 5, 110);
            _gravityYUp.OnDown += control => _ptm.Spawner.Acceleration.Y += GRAVITYSTEP;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            _screeninfo.Text = "Active: " + (GetRoot<State>().ActiveObjects) + "\n";

            _strengthValue.Text = Math.Round(_ptm.Spawner.Strength, 1).ToString();
            _strengthUp.Body.Position.X = _strengthValue.Body.BoundingRect.Right + 5;

            _gravityYValue.Text = "Y:" + Math.Round(_ptm.Spawner.Acceleration.Y, 1).ToString();
            _gravityYUp.Body.Position = new Vector2(_gravityYValue.Body.BoundingRect.Right + 5, 110);

            _gravityXValue.Text = "X:" + Math.Round(_ptm.Spawner.Acceleration.X, 1).ToString();
            _gravityXUp.Body.Position = new Vector2(_gravityXValue.Body.BoundingRect.Right + 5, 80);
        }

        public class ParticleTestManager : Node
        {
            private ControlHandler _controlHandler;
            public TestSpawner Spawner;
            private Body _body;

            private GamePadAnalog _moveCursor;
            private GamepadInput _emitButton;

            public ParticleTestManager(State stateref)
                : base(stateref, "ParticleTestManager")
            {
                _controlHandler = stateref.GetService<ControlHandler>();
                _body = new Body(this, "EmitterBody");
                Spawner = new TestSpawner(this, "Spawner");

                _moveCursor = new GamePadAnalog(this, "MoveCursor", Sticks.Left, PlayerIndex.One);
                _emitButton = new GamepadInput(this, "EmitButton", Buttons.B, PlayerIndex.One);
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                MouseService.Cursor.Position = new Vector2(MouseService.Cursor.Position.X + (_moveCursor.Position.X * 5),
                                                           MouseService.Cursor.Position.Y - (_moveCursor.Position.Y * 5));

                if (MouseService.IsMouseButtonPressed(MouseButton.RightButton) || _emitButton.Pressed())
                    EntityGame.Log.Write("Mouse button pressed, dispensing particles", this, Alert.Info);
                if (MouseService.IsMouseButtonDown(MouseButton.RightButton) || _emitButton.Down())
                {
                    Spawner.Emit(30);
                }
                if (MouseService.IsMouseButtonReleased(MouseButton.RightButton) || _emitButton.Pressed())
                    EntityGame.Log.Write("Mouse button released, stopping particles", this, Alert.Info);
            }

            public class TestSpawner : Spawner
            {
                private float _stength = 3;

                public float Strength
                {
                    get { return _stength; }
                    set
                    {
                        _stength = value < 0 ? 0 : value;
                    }
                }

                public Vector2 Acceleration = new Vector2(0, .2f);

                public TestSpawner(Node e, string name)
                    : base(e, name)
                {
                }

                protected override Spawn GenerateNewParticle()
                {
                    var p = GetRoot<State>().GetNextRecycled<TestSpawn>(GetRoot(), "TestSpawnRecycled") ?? new TestSpawn(GetRoot());
                    p.RectRender.Color = ColorMath.HSVtoRGB(new HSVColor((float)Random.NextDouble(), 1, 1, 1));
                    p.Body.Position = new Vector2(MouseService.Cursor.Position.X, MouseService.Cursor.Position.Y);
                    p.Body.Width = Random.Next(3, 10);
                    p.Body.Height = p.Body.Width;
                    p.Body.Angle = (float)Random.NextDouble() * MathHelper.TwoPi;

                    float thrust = (float)Random.NextDouble() * Strength;
                    while (Math.Abs(thrust) < .00001f)
                    {
                        thrust = (float)Random.NextDouble() * Strength;
                    }
                    p.Physics.Thrust(thrust);
                    p.Physics.AngularVelocity = (float)Random.NextDouble() / 3f;
                    p.Physics.Acceleration = Acceleration;

                    //p.RectRender.Scale = Vector2.One * (float)_random.NextDouble() + Vector2.One;
                    p.RectRender.Layer = 0f;
                    return p;
                }

                private class TestSpawn : FadeSpawn
                {
                    public override bool IsObject
                    {
                        get { return true; }
                    }

                    public Body Body;
                    public Physics Physics;
                    public ShapeTypes.Rectangle RectRender;

                    public TestSpawn(Node parent)
                        : base(parent, 2000)
                    {
                        Body = new Body(this, "Body");
                        Body.Origin = new Vector2(.5f, .5f);

                        Physics = new Physics(this, "Physics");
                        Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

                        RectRender = new ShapeTypes.Rectangle(this, "RectRender", RandomHelper.RandomBool());
                        RectRender.LinkDependency(ShapeTypes.Rectangle.DEPENDENCY_BODY, Body);

                        RectRender.Thickness = 1;
                        Render = RectRender;
                        FadeAge = 1000;

                        DeathTimer.LastEvent += Recycle;
                    }

                    public override void Update(GameTime gt)
                    {
                        base.Update(gt);

                        RectRender.X = Body.X;
                        RectRender.Y = Body.Y;
                        RectRender.Width = Body.Width;
                        RectRender.Height = Body.Height;
                        RectRender.Angle = Body.Angle;
                    }

                    public override void Reuse(Node parent, string name)
                    {
                        base.Reuse(parent, name);
                        DeathTimer.Milliseconds = 2000;
                        DeathTimer.LastEvent += Recycle;
                        DeathTimer.Start();
                    }
                }
            }
        }
    }
}