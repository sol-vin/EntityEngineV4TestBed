using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4TestBed.States.ParticleTest;
using EntityEngineV4TestBed.States.SuperTownDefence;
using EntityEngineV4TestBed.States.TestControl;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.Menu
{
    public class MenuState : EntityState
    {
        //Manager
        private MenuStateManager _menuStateManager;

        public MenuState(EntityGame eg) : base(eg, "MenuState")
        {

            //Service init
            Services.Add(new InputHandler(this));
            Services.Add(new MouseHandler(this));
            var ch = new ControlHandler(this);

            LinkLabel l = new LinkLabel(this, "ControlTestStateLink");
            l.Body.Position = new Vector2(20, 20);
            l.Text = "Control Test State";
            //Lambda expression, used to make an "anonymous" method.
            l.Selected += control => ShowControlTestState();
            l.TabPosition = new Point(0,0);
            l.OnFocusGain(l);
            ch.AddControl(l);

            l = new LinkLabel(this, "ParticleTestStateLink");
            l.Body.Position = new Vector2(20,50);
            l.Text = "Particle Test State";
            l.Selected += control => ShowParticleTestState();
            l.TabPosition = new Point(0, 1);
            ch.AddControl(l);

            l = new LinkLabel(this, "STDMenuStateLink");
            l.Body.Position = new Vector2(20, 80);
            l.Text = "Super Town Defence!";
            l.Selected += control => ShowSTDMenuState();
            l.TabPosition = new Point(0, 2);
            ch.AddControl(l);

            Services.Add(ch);

            _menuStateManager = new MenuStateManager(this, ch);
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
    }
}
