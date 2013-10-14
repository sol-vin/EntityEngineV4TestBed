using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework.GamerServices;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class SimpleGun : Gun
    {
        public int BulletCounter { get; private set; }
        
        public SimpleGun(IComponent parent, string name) : base(parent, name)
        {
        }

        public override void Fire()
        {
            Bullet bullet = new Bullet(this, "Bullet" + BulletCounter);
            bullet.Body.Position = GetLink<Body>(DEPENDENCY_BODY).Position + (GetLink<Body>(DEPENDENCY_BODY).Bounds/2) - (bullet.Body.Bounds/2);
            bullet.Body.Angle = GetLink<Body>(DEPENDENCY_BODY).Angle;
            bullet.Initialize();
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }
    }
}
