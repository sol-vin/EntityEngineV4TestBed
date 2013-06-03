using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.SuperTownDefence.Objects.Components
{
    public class Gun : Component
    {
        public float Thrust;
        private float _correctionangle;

        public Gun(Entity e, string name)
            : base(e, name)
        {
        }

        public void Fire(Vector2 position, float angle, Vector2 origin, float scale)
        {
            Bomb b = new Bomb(Parent.StateRef, "Bomb");
            b.Collision.Partners = Parent.GetComponent<Targets>().List;
            b.Body.Position = position + origin * scale - b.ImageRender.Origin * b.ImageRender.Scale.X;
            b.Body.Angle = angle - angle * _correctionangle;
            b.Physics.Thrust(Thrust);
            Parent.StateRef.AddEntity(b);
        }
    }
}
