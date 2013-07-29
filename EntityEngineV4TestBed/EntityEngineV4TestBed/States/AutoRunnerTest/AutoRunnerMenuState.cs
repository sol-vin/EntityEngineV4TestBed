using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.AutoRunnerTest
{
    public class AutoRunnerMenuState : TestBedState
    {
        public AutoRunnerMenuState(EntityGame eg) : base(eg, "AutoRunnerMenuState")
        {
            AddService(new MouseHandler(this));
            var ch = new ControlHandler(this);
            AddService(ch);

            LinkLabel l = new LinkLabel(ch, "GoForwardLabel");
            l.Text = "Press Enter To Begin!";
            l.Selected += control => ShowGameState();
            l.Body.Position.X = EntityGame.Viewport.Width/2f - l.Body.Bounds.X/2;
            l.Body.Position.Y = 540;
            l.TabPosition = new Point(0,0);
            l.OnFocusGain(l);
            ch.AddControl(l);

            Label instructions = new Label(ch, "Instructionslbl");
            instructions.Text = "Welcome to AutoRunner!\n" +
                                "Instructions are as follows!\n" +
                                "Press Space to jump\n" +
                                "Press LMB to throw a grenade\n" +
                                "Jump higher by bouncing off your grenade's \nexplosions!\n" +
                                "Have fun!\n" +
                                "Press ESC to come back to the menu and \nbackspace to quit.\n";
            instructions.Body.Position = new Vector2(20,20);
            instructions.TabPosition = new Point(0,1);
            ch.AddControl(instructions);

            AddEntity(new MenuStateManager(this));
        }

        private void ShowGameState()
        {
            var game = new AutoRunnerGameState(GameRef);
            game.ChangeState += Show;
            game.Show();
        }

        private class MenuStateManager : Entity
        {
            private DoubleInput _select;
            private ControlHandler _ch;

            public MenuStateManager(EntityState stateref) : base(stateref, "MenuStateManager")
            {
                _select = new DoubleInput(this, "Select", Keys.Space, Buttons.A, PlayerIndex.One);
                _ch = EntityGame.CurrentState.GetService<ControlHandler>();
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                if (_select.Released())
                {
                    _ch.Select();
                }
            }
        }
    }
}
