using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.TestControl
{
    public class TestControlManager : Node
    {
        public ControlHandler ControlHandler;

        private DoubleInput _upkey, _downkey, _leftkey, _rightkey, _selectkey;

        public TestControlManager(State stateref, ControlHandler controlHandler)
            : base(stateref, "TestControlManager")
        {
            ControlHandler = controlHandler;
            _upkey = new DoubleInput(this, "UpKey", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
            _downkey = new DoubleInput(this, "DownKey", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
            _leftkey = new DoubleInput(this, "LeftKey", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
            _rightkey = new DoubleInput(this, "RightKey", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
            _selectkey = new DoubleInput(this, "SelectKey", Keys.Enter, Buttons.A, PlayerIndex.One);
        }

        public override void Update(GameTime gt)
        {
            if (_upkey.Released())
                ControlHandler.UpControl();
            else if (_downkey.Released())
                ControlHandler.DownControl();
            else if (_leftkey.Released())
                ControlHandler.LeftControl();
            else if (_rightkey.Released())
                ControlHandler.RightControl();
            if (_selectkey.Released())
                ControlHandler.Release();

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}