using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed
{
    public class TestStateManager : Entity
    {
        private DoubleInput _resetkey;

        private bool _reset;

        public bool Reset
        {
            get { return _reset; }
        }

        public TestStateManager(EntityState stateref)
            : base(stateref, stateref, "TestStateManager")
        {
            _resetkey = new DoubleInput(this, "ResetKey", Keys.Escape, Buttons.Start, PlayerIndex.One);
        }

        public override void Update(GameTime gt)
        {
            _reset = _resetkey.Pressed();
            base.Update(gt);
        }
    }
}