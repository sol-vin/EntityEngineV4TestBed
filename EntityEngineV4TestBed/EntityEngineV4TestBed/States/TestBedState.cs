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
        protected TestBedState(EntityGame eg, string name) : base(eg, name)
        {
            AddEntity(new TestBedStateManager(this, "TestBedStateManager"));
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}
