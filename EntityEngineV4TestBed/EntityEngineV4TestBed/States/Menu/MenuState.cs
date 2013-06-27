using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4TestBed.States.AutoRunnerTest;
using EntityEngineV4TestBed.States.CameraTest;
using EntityEngineV4TestBed.States.CollisionTest;
using EntityEngineV4TestBed.States.ColorTest;
using EntityEngineV4TestBed.States.FancyParticleTest;
using EntityEngineV4TestBed.States.ParticleTest;
using EntityEngineV4TestBed.States.ResolutionTest;
using EntityEngineV4TestBed.States.SuperTownDefence;
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

        public MenuState(EntityGame eg)
            : base(eg, "MenuState")
        {
            Parent.ShowFPS = true;

            //Service init
            Services.Add(new InputHandler(this));
            Services.Add(new MouseHandler(this));
            var ch = new ControlHandler(this);
            Services.Add(ch);

            _menuStateManager = new MenuStateManager(this, ch);
            _menuStateManager.AddMenuItem("Camera Test State", ShowCameraTestState);
            _menuStateManager.AddMenuItem("Control Test State", ShowControlTestState);
            _menuStateManager.AddMenuItem("Particle Test State", ShowParticleTestState);
            _menuStateManager.AddMenuItem("Fancy Particle Test State", ShowFancyParticleTestState);
            _menuStateManager.AddMenuItem("Collision Test State", ShowCollisionTestState);
            _menuStateManager.AddMenuItem("Resolution Test State", ShowResolutionTestState);
            _menuStateManager.AddMenuItem("Color Test State", ShowColorTestState);
            _menuStateManager.AddMenuItem("Tilemap Test State", ShowTilemapTestState);
            _menuStateManager.AddMenuItem("Super Town Defence", ShowSTDMenuState);
            _menuStateManager.AddMenuItem("Auto Runner", ShowAutoRunnerMenuState);
            AddEntity(_menuStateManager);
        }

        public void ShowParticleTestState()
        {
            var particleTestState = new ParticleTestState(Parent);
            particleTestState.ChangeState += Show;
            particleTestState.Show();
        }

        public void ShowControlTestState()
        {
            var controlTestState = new ControlTestState(Parent);
            controlTestState.ChangeState += Show;
            controlTestState.Show();
        }

        public void ShowSTDMenuState()
        {
            var stdMenuState = new STDMenuState(Parent);
            stdMenuState.ChangeState += Show;
            stdMenuState.Show();
        }

        public void ShowCollisionTestState()
        {
            var collisionTestState = new CollisionTestState(Parent);
            collisionTestState.ChangeState += Show;
            collisionTestState.Show();
        }

        public void ShowResolutionTestState()
        {
            var resolutionTestState = new ResolutionTestState(Parent);
            resolutionTestState.ChangeState += Show;
            resolutionTestState.Show();
        }

        public void ShowColorTestState()
        {
            var colorTestState = new ColorTestState(Parent);
            colorTestState.ChangeState += Show;
            colorTestState.Show();
        }

        public void ShowFancyParticleTestState()
        {
            var fancyParticleTestState = new FancyParticleTestState(Parent);
            fancyParticleTestState.ChangeState += Show;
            fancyParticleTestState.Show();
        }

        public void ShowTilemapTestState()
        {
            var tilemapTestState = new TilemapTestState(Parent);
            tilemapTestState.ChangeState += Show;
            tilemapTestState.Show();
        }

        public void ShowCameraTestState()
        {
            var cameraTestState = new CameraTestState(Parent);
            cameraTestState.ChangeState += Show;
            cameraTestState.Show();
        }

        public void ShowAutoRunnerMenuState()
        {
            var cameraTestState = new AutoRunnerMenuState(Parent);
            cameraTestState.ChangeState += Show;
            cameraTestState.Show();
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
                LinkLabel l = new LinkLabel(StateRef, "MenuItem" + (_lasttabposition.X ^ _lasttabposition.Y));
                l.Body.Position = new Vector2(20, 20 + (_lasttabposition.Y * 30));
                l.Text = label;
                l.Selected += control => changeStateDelegate();

                l.TabPosition = _lasttabposition;
                _lasttabposition.Y++;

                if (l.TabPosition == Point.Zero) l.OnFocusGain(l);
                _controlHandler.AddControl(l);
            }
        }
    }
}