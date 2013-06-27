using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.SuperTownDefence
{
    public class STDGameState : EntityState
    {
        public STDGameState(EntityGame eg)
            : base(eg, "STDGameState")
        {
            Services.Add(new InputHandler(this));
            AddEntity(new STDGameStateManager(this));
        }

        private class STDGameStateManager : Entity
        {
            private DoubleInput _menukey;

            public STDGameStateManager(EntityState stateref)
                : base(stateref, "STDGameStateManager")
            {
                _menukey = new DoubleInput(this, "StartKey", Keys.Back, Buttons.Back, PlayerIndex.One);
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                if (_menukey.Released())
                    StateRef.ChangeToState("STDMenuState");
            }
        }
    }
}