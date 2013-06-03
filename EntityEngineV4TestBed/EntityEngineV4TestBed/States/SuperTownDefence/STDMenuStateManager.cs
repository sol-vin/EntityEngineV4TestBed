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
    public class STDMenuStateManager : Entity
    {
        private DoubleInput _startkey;
        public STDMenuStateManager(EntityState stateref) : base(stateref, "STDMenuStateManager")
        {
            
            _startkey = new DoubleInput(this, "StartKey", Keys.Enter, Buttons.Start, PlayerIndex.One);

        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if(_startkey.Released())
                StateRef.ChangeToState("STDGameState");
        }
    }
}
