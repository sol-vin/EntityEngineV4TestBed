using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public abstract class Gun : Component
    {
        protected Gun(Node parent, string name) : base(parent, name)
        {
        }

        public abstract void Fire();
    }
}
