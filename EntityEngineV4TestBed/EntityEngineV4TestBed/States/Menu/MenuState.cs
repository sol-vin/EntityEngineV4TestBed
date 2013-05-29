using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4TestBed.States.Test;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.Menu
{
    public class MenuState : EntityState
    {
        //Manager
        private MenuStateManager _menuStateManager;
        //States
        private ControlTestState _controlTestState;

        public MenuState(EntityGame eg) : base(eg, "MenuState")
        {
            //State initialization
            _controlTestState = new ControlTestState(Parent);
            _controlTestState.ChangeState += Show;

            //Service init
            Services.Add(new InputHandler(this));
            Services.Add(new MouseHandler(this, true));
            var ch = new ControlHandler(this);

            LinkLabel l = new LinkLabel(this, "ControlTestStateLink");
            l.Body.Position = new Vector2(20,20);
            l.Text = "Control Test State";
            l.Selected += _controlTestState.Show;
            l.TabPosition = new Point(0,0);
            l.OnFocusGain(l);
            ch.AddControl(l);

            l = new LinkLabel(this, "ControlTestStateLink2");
            l.Body.Position = new Vector2(20,50);
            l.Text = "COMING SOON!";
            //l.Selected += _controlTestState.Show;
            l.TabPosition = new Point(0, 1);
            ch.AddControl(l);

            Services.Add(ch);

            _menuStateManager = new MenuStateManager(this, ch);
            AddEntity(_menuStateManager);
        }
    }
}
