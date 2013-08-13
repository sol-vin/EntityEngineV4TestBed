using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4TestBed.States.CameraTest;
using EntityEngineV4TestBed.States.CollisionTest;
using EntityEngineV4TestBed.States.ColorTest;
using EntityEngineV4TestBed.States.FancyParticleTest;
using EntityEngineV4TestBed.States.FireworkTest;
using EntityEngineV4TestBed.States.GameOfLife;
using EntityEngineV4TestBed.States.ParticleTest;
using EntityEngineV4TestBed.States.PrimitiveTest;
using EntityEngineV4TestBed.States.ResolutionTest;
using EntityEngineV4TestBed.States.SourceRectangleTest;
using EntityEngineV4TestBed.States.TestControl;
using EntityEngineV4TestBed.States.TilemapTest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.Menu
{
    public class MenuState : EntityState
    {
        private delegate void ChangeStateDelegate();

        //Manager
        private MenuStateManager _menuStateManager;

        public MenuState()
            : base("MenuState")
        {
        }

        public override void Create()
        {
            base.Create();

            EntityGame.ShowFPS = true;

            //Service init
            new InputHandler(this);
            new MouseHandler(this);

            var ch = new ControlHandler(this);

            _menuStateManager = new MenuStateManager(this, ch);
            _menuStateManager.AddMenuItem("Game of Life", () => EntityGame.SwitchState(new GameOfLifeState()));
            _menuStateManager.AddMenuItem("Color Game of Life", () => EntityGame.SwitchState(new ColorGameOfLifeState()));

            _menuStateManager.AddMenuItem("Camera Test State", () => EntityGame.SwitchState(new CameraTestState()));
            _menuStateManager.AddMenuItem("Control Test State", () => EntityGame.SwitchState(new ControlTestState()));
            _menuStateManager.AddMenuItem("Particle Test State", () => EntityGame.SwitchState(new ParticleTestState()));
            _menuStateManager.AddMenuItem("Fancy Particle Test State", () => EntityGame.SwitchState(new FancyParticleTestState()));
            _menuStateManager.AddMenuItem("Firework Test State", () => EntityGame.SwitchState(new FireworkTestState()));
            _menuStateManager.AddMenuItem("Collision Test State", () => EntityGame.SwitchState(new CollisionTestState()));
            _menuStateManager.AddMenuItem("Resolution Test State", () => EntityGame.SwitchState(new ResolutionTestState()));
            _menuStateManager.AddMenuItem("Color Test State", () => EntityGame.SwitchState(new ColorTestState()));
            _menuStateManager.AddMenuItem("Tilemap Test State", () => EntityGame.SwitchState(new TilemapTestState()));
            _menuStateManager.AddMenuItem("Render Test State", () => EntityGame.SwitchState(new RenderTestState()));
            _menuStateManager.AddMenuItem("Primitives Test State", () => EntityGame.SwitchState(new PrimitiveTestState()));
            AddEntity(_menuStateManager);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        private class MenuStateManager : Entity
        {
            private ControlHandler _controlHandler;

            private Point _lasttabposition = Point.Zero;

            private DoubleInput _upkey, _downkey, _leftkey, _rightkey, _selectkey;

            public MenuStateManager(EntityState stateref, ControlHandler controlHandler)
                : base(stateref, "MenuStateManager")
            {
                _controlHandler = controlHandler;
                _upkey = new DoubleInput(this, "UpKey", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
                _downkey = new DoubleInput(this, "DownKey", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
                _leftkey = new DoubleInput(this, "LeftKey", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
                _rightkey = new DoubleInput(this, "RightKey", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
                _selectkey = new DoubleInput(this, "SelectKey", Keys.Space, Buttons.A, PlayerIndex.One);
            }

            public override void Update(GameTime gt)
            {
                if (_upkey.Released())
                    _controlHandler.UpControl();
                else if (_downkey.Released())
                    _controlHandler.DownControl();
                else if (_leftkey.Released())
                    _controlHandler.LeftControl();
                else if (_rightkey.Released())
                    _controlHandler.RightControl();
                if (_selectkey.Released())
                    _controlHandler.Select();

                base.Update(gt);
            }

            public void AddMenuItem(string label, ChangeStateDelegate changeStateDelegate)
            {
                LinkLabel l = new LinkLabel(Parent, "MenuItem" + (_lasttabposition.X ^ _lasttabposition.Y));

                l.Text = label;
                l.Body.Position = new Vector2(20, (_lasttabposition.Y * l.Body.Height + 5));
                l.TextRender.Layer = .2f;
                l.Selected += control => changeStateDelegate();

                l.TabPosition = _lasttabposition;
                _lasttabposition.Y++;

                if (l.TabPosition == Point.Zero) l.OnFocusGain(l);
                l.AttachToControlHandler();
            }
        }
    }
}