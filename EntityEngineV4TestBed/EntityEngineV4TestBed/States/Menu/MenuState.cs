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
using EntityEngineV4TestBed.States.PrimitiveTest;
using EntityEngineV4TestBed.States.ResolutionTest;
using EntityEngineV4TestBed.States.SourceRectangleTest;
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
            EntityGame.ShowFPS = true;

            //Service init
            AddService(new InputHandler(this));
            AddService(new MouseHandler(this));
            var ch = new ControlHandler(this);
            AddService(ch);

            _menuStateManager = new MenuStateManager(this, ch);
            _menuStateManager.AddMenuItem("Camera Test State", ShowCameraTestState);
            _menuStateManager.AddMenuItem("Control Test State", ShowControlTestState);
            _menuStateManager.AddMenuItem("Particle Test State", ShowParticleTestState);
            _menuStateManager.AddMenuItem("Fancy Particle Test State", ShowFancyParticleTestState);
            _menuStateManager.AddMenuItem("Collision Test State", ShowCollisionTestState);
            _menuStateManager.AddMenuItem("Resolution Test State", ShowResolutionTestState);
            _menuStateManager.AddMenuItem("Color Test State", ShowColorTestState);
            _menuStateManager.AddMenuItem("Tilemap Test State", ShowTilemapTestState);
            _menuStateManager.AddMenuItem("Render Test State", ShowRenderTestState);
            _menuStateManager.AddMenuItem("Primitives Test State", ShowPrimitiveTestState);
            AddEntity(_menuStateManager);
        }

        public void ShowParticleTestState()
        {
            var particleTestState = new ParticleTestState(Parent as EntityGame);
            particleTestState.ChangeState += Show;
            particleTestState.Show();
        }

        public void ShowControlTestState()
        {
            var controlTestState = new ControlTestState(Parent as EntityGame);
            controlTestState.ChangeState += Show;
            controlTestState.Show();
        }

        public void ShowCollisionTestState()
        {
            var collisionTestState = new CollisionTestState(Parent as EntityGame);
            collisionTestState.ChangeState += Show;
            collisionTestState.Show();
        }

        public void ShowResolutionTestState()
        {
            var resolutionTestState = new ResolutionTestState(Parent as EntityGame);
            resolutionTestState.ChangeState += Show;
            resolutionTestState.Show();
        }

        public void ShowColorTestState()
        {
            var colorTestState = new ColorTestState(Parent as EntityGame);
            colorTestState.ChangeState += Show;
            colorTestState.Show();
        }

        public void ShowFancyParticleTestState()
        {
            var fancyParticleTestState = new FancyParticleTestState(Parent as EntityGame);
            fancyParticleTestState.ChangeState += Show;
            fancyParticleTestState.Show();
        }

        public void ShowTilemapTestState()
        {
            var tilemapTestState = new TilemapTestState(Parent as EntityGame);
            tilemapTestState.ChangeState += Show;
            tilemapTestState.Show();
        }

        public void ShowCameraTestState()
        {
            var cameraTestState = new CameraTestState(Parent as EntityGame);
            cameraTestState.ChangeState += Show;
            cameraTestState.Show();
        }

        public void ShowRenderTestState()
        {
            var sourceTestState = new RenderTestState(Parent as EntityGame);
            sourceTestState.ChangeState += Show;
            sourceTestState.Show();
        }

        public void ShowPrimitiveTestState()
        {
            var sourceTestState = new PrimitiveTestState(Parent as EntityGame);
            sourceTestState.ChangeState += Show;
            sourceTestState.Show();
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
                LinkLabel l = new LinkLabel(_controlHandler, "MenuItem" + (_lasttabposition.X ^ _lasttabposition.Y));
                
                l.Text = label;
                l.Body.Position = new Vector2(20, (_lasttabposition.Y * l.Body.Height + 5));

                l.Selected += control => changeStateDelegate();

                l.TabPosition = _lasttabposition;
                _lasttabposition.Y++;

                if (l.TabPosition == Point.Zero) l.OnFocusGain(l);
                _controlHandler.AddControl(l);
            }
        }
    }
}