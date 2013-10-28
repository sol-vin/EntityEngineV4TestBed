using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public abstract class Gun : Component
    {
        public HashSet<Bullet> Bullets = new HashSet<Bullet>();

        protected Gun(IComponent parent, string name) : base(parent, name)
        {
        }

        public abstract void Fire();
    }
}
