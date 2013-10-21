using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework.GamerServices;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class SimpleGun : Gun
    {
        public int BulletCounter { get; private set; }
        public const int MaxBullets = 15;

        public HashSet<Bullet> Bullets = new HashSet<Bullet>(); 
        public SimpleGun(IComponent parent, string name) : base(parent, name)
        {
        }

        public override void Fire()
        {
            if (MaxBullets <= BulletCounter) return;
            Bullet bullet = new Bullet(this, "Bullet" + BulletCounter);
            bullet.Body.Position = GetLink<Body>(DEPENDENCY_BODY).Position + (GetLink<Body>(DEPENDENCY_BODY).Bounds/2f) -
                                   (bullet.Body.Bounds/2f);
            bullet.Body.Angle = GetLink<Body>(DEPENDENCY_BODY).Angle +
                                (0.05f*RandomHelper.GetSign()*RandomHelper.NextGaussian(1, 1f));
            bullet.Physics.AddForce(GetLink<Body>(DEPENDENCY_BODY).Delta);
            bullet.DestroyEvent += component => BulletCounter--;
            bullet.DestroyEvent += component => Bullets.Remove(bullet);
            BulletCounter++;

            Bullets.Add(bullet);
        }

        public const int DEPENDENCY_BODY = 0;
        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }
    }
}
