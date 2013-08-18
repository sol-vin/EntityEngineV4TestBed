using System;
using System.Linq;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.ParticleTest
{
    public class ParticleTestState : TestBedState
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

        public ParticleTestState()
            : base("ParticleTestState")
        {
        }

        public override void Create()
        {
            base.Create();

            ControlHandler ch = new ControlHandler(this);

            _ptm = new ParticleTestManager(this);

            _screeninfo = new Label(ch, "ScreenInfo");
            _screeninfo.Body.Position = Vector2.One * 20;
            _screeninfo.AttachToControlHandler();

            _strengthText = new Label(ch, "StrengthText");
            _strengthText.Text = "Strength:";
            _strengthText.Body.Position = new Vector2(20, 50);
            _strengthText.TabPosition = new Point(0, 1);
            _strengthText.AttachToControlHandler();

            _strengthDown = new LinkLabel(ch, "StrengthDown");
            _strengthDown.Text = "<-";
            _strengthDown.TabPosition = new Point(1, 1);
            _strengthDown.Body.Position = new Vector2(_strengthText.Body.BoundingRect.Right + 5, 50);
            _strengthDown.OnDown += control => _ptm.Emitter.Strength -= STRENGTHSTEP;
            _strengthDown.AttachToControlHandler();

            _strengthValue = new Label(ch, "StrengthValue");
            _strengthValue.Text = _ptm.Emitter.Strength.ToString();
            _strengthValue.Body.Position = new Vector2(_strengthDown.Body.BoundingRect.Right + 5, 50);
            _strengthValue.TabPosition = new Point(2, 1);
            _strengthValue.AttachToControlHandler();

            _strengthUp = new LinkLabel(ch, "StrengthUp");
            _strengthUp.Text = "->";
            _strengthUp.TabPosition = new Point(3, 1);
            _strengthUp.Body.Position = new Vector2(_strengthValue.Body.BoundingRect.Right + 5, 50);
            _strengthUp.OnDown += control => _ptm.Emitter.Strength += STRENGTHSTEP;
           _strengthUp.AttachToControlHandler();

            _gravityText = new Label(ch, "GravityText");
            _gravityText.Text = "Gravity:";
            _gravityText.Body.Position = new Vector2(20, 80);
            _gravityText.TabPosition = new Point(0, 2);
            _gravityText.AttachToControlHandler();

            _gravityXDown = new LinkLabel(ch, "GravityXDown");
            _gravityXDown.Text = "<-";
            _gravityXDown.TabPosition = new Point(1, 2);
            _gravityXDown.Body.Position = new Vector2(_gravityText.Body.BoundingRect.Right + 5, 80);
            _gravityXDown.OnDown += control => _ptm.Emitter.Acceleration.X -= GRAVITYSTEP;
            _gravityXDown.AttachToControlHandler();

            _gravityXValue = new Label(ch, "GravityXValue");
            _gravityXValue.Text = "X:" + _ptm.Emitter.Acceleration.X.ToString();
            _gravityXValue.TabPosition = new Point(2, 2);
            _gravityXValue.Body.Position = new Vector2(_gravityXDown.Body.BoundingRect.Right + 5, 80);
            _gravityXValue.AttachToControlHandler();

            _gravityXUp = new LinkLabel(ch, "GravityXUp");
            _gravityXUp.Text = "->";
            _gravityXUp.TabPosition = new Point(3, 2);
            _gravityXUp.Body.Position = new Vector2(_gravityXValue.Body.BoundingRect.Right + 5, 80);
            _gravityXUp.OnDown += control => _ptm.Emitter.Acceleration.X += GRAVITYSTEP;
            _gravityXUp.AttachToControlHandler();

            _gravityYDown = new LinkLabel(ch, "GravityYDown");
            _gravityYDown.Text = "<-";
            _gravityYDown.TabPosition = new Point(1, 3);
            _gravityYDown.Body.Position = new Vector2(_gravityText.Body.BoundingRect.Right + 5, 110);
            _gravityYDown.OnDown += control => _ptm.Emitter.Acceleration.Y -= GRAVITYSTEP;
            _gravityYDown.AttachToControlHandler();

            _gravityYValue = new Label(ch, "GravityYValue");
            _gravityYValue.Text = "Y:" + _ptm.Emitter.Acceleration.Y.ToString();
            _gravityYValue.TabPosition = new Point(2, 3);
            _gravityYValue.Body.Position = new Vector2(_gravityYDown.Body.BoundingRect.Right + 5, 110);
            _gravityYValue.AttachToControlHandler();

            _gravityYUp = new LinkLabel(ch, "GravityYUp");
            _gravityYUp.Text = "->";
            _gravityYUp.TabPosition = new Point(3, 3);
            _gravityYUp.Body.Position = new Vector2(_gravityYValue.Body.BoundingRect.Right + 5, 110);
            _gravityYUp.OnDown += control => _ptm.Emitter.Acceleration.Y += GRAVITYSTEP;
            _gravityYUp.AttachToControlHandler();
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            _screeninfo.Text = "Active: " + (this.Count()-15) + "\n";

            _strengthValue.Text = Math.Round(_ptm.Emitter.Strength, 1).ToString();
            _strengthUp.Body.Position.X = _strengthValue.Body.BoundingRect.Right + 5;

            _gravityYValue.Text = "Y:" + Math.Round(_ptm.Emitter.Acceleration.Y, 1).ToString();
            _gravityYUp.Body.Position = new Vector2(_gravityYValue.Body.BoundingRect.Right + 5, 110);

            _gravityXValue.Text = "X:" + Math.Round(_ptm.Emitter.Acceleration.X, 1).ToString();
            _gravityXUp.Body.Position = new Vector2(_gravityXValue.Body.BoundingRect.Right + 5, 80);
        }

        public class ParticleTestManager : Entity
        {
            private ControlHandler _controlHandler;
            public TestEmitter Emitter;
            private Body _body;

            private GamePadAnalog _moveCursor;
            private GamepadInput _emitButton;

            private DoubleInput _upkey, _downkey, _leftkey, _rightkey, _selectkey;

            public ParticleTestManager(EntityState stateref)
                : base(stateref, "ParticleTestManager")
            {
                _controlHandler = stateref.GetService<ControlHandler>();
                _body = new Body(this, "EmitterBody");
                Emitter = new TestEmitter(this, "Emitter");

                _moveCursor = new GamePadAnalog(this, "MoveCursor", Sticks.Left, PlayerIndex.One);
                _emitButton = new GamepadInput(this, "EmitButton", Buttons.B, PlayerIndex.One);
                _upkey = new DoubleInput(this, "UpKey", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
                _downkey = new DoubleInput(this, "DownKey", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
                _leftkey = new DoubleInput(this, "LeftKey", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
                _rightkey = new DoubleInput(this, "RightKey", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
                _selectkey = new DoubleInput(this, "SelectKey", Keys.Space, Buttons.A, PlayerIndex.One);
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                MouseHandler.Cursor.Position = new Vector2(MouseHandler.Cursor.Position.X + (_moveCursor.Position.X*5),
                                                           MouseHandler.Cursor.Position.Y - (_moveCursor.Position.Y*5));

                if(MouseHandler.IsMouseButtonPressed(MouseButton.RightButton) || _emitButton.Pressed())
                    EntityGame.Log.Write("Mouse button pressed, dispensing particles", this, Alert.Info);
                if (MouseHandler.IsMouseButtonDown(MouseButton.RightButton) || _emitButton.Down())
                {

                    Emitter.Emit(30);
                }
                if (MouseHandler.IsMouseButtonReleased(MouseButton.RightButton) || _emitButton.Pressed())
                    EntityGame.Log.Write("Mouse button released, stopping particles", this, Alert.Info);
                if (_upkey.Released())
                    _controlHandler.UpControl();
                else if (_downkey.Released())
                    _controlHandler.DownControl();
                else if (_leftkey.Released())
                    _controlHandler.LeftControl();
                else if (_rightkey.Released())
                    _controlHandler.RightControl();
                if (_selectkey.Released())
                    _controlHandler.Release();
            }

            public class TestEmitter : Emitter
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

                public TestEmitter(Entity e, string name)
                    : base(e, name)
                {
                }

                protected override Particle GenerateNewParticle()
                {
                    var p = new TestParticle(this);
                    p.RectRender.Color = ColorMath.HSVtoRGB(new HSVColor((float)Random.NextDouble(), 1, 1, 1));
                    p.Body.Position = new Vector2(MouseHandler.Cursor.Position.X, MouseHandler.Cursor.Position.Y);
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

                private class TestParticle : FadeParticle
                {
                    public Body Body;
                    public Physics Physics;
                    public ShapeTypes.Rectangle RectRender;

                    public TestParticle(Emitter e)
                        : base(e, 3000)
                    {
                        Body = new Body(this, "Body");
                        Physics = new Physics(this, "Physics", Body);
                        RectRender = new ShapeTypes.Rectangle(this, "RectRender", Body, Random.RandomBool());
                        RectRender.Origin = new Vector2(.5f,.5f);
                        RectRender.Thickness = 1;
                        Render = RectRender;
                        FadeAge = 1000;
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
                }
            }
        }
    }
}