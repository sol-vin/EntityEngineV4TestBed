using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class BaseEntity : Entity
    {
        public Body Body;
        public Physics Physics;

        public BaseEntity(IComponent parent, string name) : base(parent, name)
        {
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            UpdateOutOfBounds();
        }

        private void UpdateOutOfBounds()
        {
            if (Body.Bottom < 0) Body.Y = EntityGame.Viewport.Height;
            else if (Body.Top > EntityGame.Viewport.Height) Body.Y = -Body.Height;
            if (Body.Right < 0) Body.X = EntityGame.Viewport.Width;
            else if (Body.Left > EntityGame.Viewport.Width) Body.X = -Body.Width;
        }
    }
}
