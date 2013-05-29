using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using EntityEngineV4TestBed.States.Menu;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States
{
    public abstract class TestBedState : EntityState
    {
        private TestBedStateManager _tbsm;

        protected TestBedState(EntityGame eg, string name) : base(eg, name)
        {
            _tbsm = new TestBedStateManager(this, "TestBedStateManager");
            AddEntity(_tbsm);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if(_tbsm.GoBack)
                ChangeToState("MenuState");
        }
    }
}
