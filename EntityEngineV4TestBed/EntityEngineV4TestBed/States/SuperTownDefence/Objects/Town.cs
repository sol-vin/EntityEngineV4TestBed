using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Collision;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;

namespace EntityEngineV4TestBed.States.SuperTownDefence.Objects
{
    public class Town : Entity
    {
        public Body Body;
        public TileRender TileRender;
        public BasicCollision Collision;

        public Town(EntityState stateref, string name) : base(stateref, name)
        {
        }
    }
}
