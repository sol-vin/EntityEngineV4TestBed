using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;

namespace EntityEngineV4TestBed.States.SuperTownDefence.Objects.Components
{
    public class Targets : Component
    {
        public List<Entity> List = new List<Entity>();

        public Targets(Entity entity, string name)
            : base(entity, name)
        {
        }
    }
}
