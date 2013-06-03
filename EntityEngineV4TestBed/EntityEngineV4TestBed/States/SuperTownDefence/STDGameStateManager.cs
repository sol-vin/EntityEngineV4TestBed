using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.SuperTownDefence
{
    public class STDGameStateManager : Entity
    {
        private DoubleInput _menukey;
        public STDGameStateManager(EntityState stateref) : base(stateref, "STDGameStateManager")
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
