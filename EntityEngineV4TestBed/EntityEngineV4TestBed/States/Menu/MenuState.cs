using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4TestBed.States.AsteriodsGame;
using EntityEngineV4TestBed.States.CameraTest;
using EntityEngineV4TestBed.States.CollisionResolution;
using EntityEngineV4TestBed.States.CollisionTest;
using EntityEngineV4TestBed.States.ColorTest;
using EntityEngineV4TestBed.States.FancySpawnerTest;
using EntityEngineV4TestBed.States.GameOfLife;
using EntityEngineV4TestBed.States.ParticleTest;
using EntityEngineV4TestBed.States.PrimitiveTest;
using EntityEngineV4TestBed.States.ResolutionTest;
using EntityEngineV4TestBed.States.SourceRectangleTest;
using EntityEngineV4TestBed.States.TestControl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.Menu
{
    public class MenuState : State
    {
        private delegate void ChangeStateDelegate();

        private Point _lasttabposition = Point.Zero;

        private DoubleInput _upkey, _downkey, _leftkey, _rightkey, _selectkey;

        private Page _page;

        public MenuState()
            : base("MenuState")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            EntityGame.ShowDebugInfo = true;
            EntityGame.BackgroundColor = Color.Gray;

            //Service init
            new InputService(this);
            new MouseService(this);

            _upkey = new DoubleInput(this, "UpKey", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
            _downkey = new DoubleInput(this, "DownKey", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
            _leftkey = new DoubleInput(this, "LeftKey", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
            _rightkey = new DoubleInput(this, "RightKey", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
            _selectkey = new DoubleInput(this, "SelectKey", Keys.Space, Buttons.A, PlayerIndex.One);

            var ch = new ControlHandler(this);
            _page = new Page(this, "Page");
            _page.Show();

            AddMenuItem("Game of Life", () => (new GameOfLifeState()).Show());
            AddMenuItem("Color Game of Life", () => (new ColorGameOfLifeState()).Show());
            AddMenuItem("Asteroids Game", () => (new AsteroidsGame()).Show());
            AddMenuItem("Particle Test State", () => (new ParticleTestState()).Show());
            AddMenuItem("Camera Test State", () => (new CameraTestState()).Show());
            AddMenuItem("Control Test State", () => (new ControlTestState()).Show());
            AddMenuItem("Spawn Test State", () => (new SpawnerTestState()).Show());
            AddMenuItem("Fancy Spawn Test State", () => (new FancySpawnerTestState()).Show());
            AddMenuItem("Collision Test State", () => (new CollisionTestState()).Show());
            AddMenuItem("Collision Tester State", () => (new CollisionResolutionTest()).Show());
            AddMenuItem("Resolution Test State", () => (new ResolutionTestState()).Show());
            AddMenuItem("Color Test State", () => (new ColorTestState()).Show());
            AddMenuItem("Render Test State", () => (new RenderTestState()).Show());
            AddMenuItem("Primitives Test State", () => (new PrimitiveTestState()).Show());
            _page.ProcessControls();
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (_upkey.Released())
                _page.MoveFocusUp();
            else if (_downkey.Released())
                _page.MoveFocusDown();
            else if (_leftkey.Released())
                _page.MoveFocusLeft();
            else if (_rightkey.Released())
                _page.MoveFocusRight();
            if (_selectkey.Released())
                _page.Release();
        }

        private void AddMenuItem(string label, ChangeStateDelegate changeStateDelegate)
        {
            var l = new LinkLabel(_page, "MenuItem" + (_lasttabposition.X ^ _lasttabposition.Y), _lasttabposition);

            l.Text = label;
            l.Body.Position = new Vector2(200, (_lasttabposition.Y * l.Body.Height + 100));
            l.Render.Layer = .2f;
            l.OnReleased += control => changeStateDelegate();
            l.OnReleased += control => Destroy(this);

            _lasttabposition.Y++;

            if (l.TabPosition == Point.Zero) l.OnFocusGain();
        }
    }
}