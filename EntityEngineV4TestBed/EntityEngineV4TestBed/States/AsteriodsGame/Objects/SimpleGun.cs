using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class SimpleGun : Gun
    {
        public const int MAXBULLETS = 10;
        public const float SPEED = 5f;

        public SimpleGun(Node parent, string name) : base(parent, name)
        {
        }

        public override void Fire()
        {
            if (MAXBULLETS <= Bullets.Count) return;
            var bullet = new Bullet(GetState(), "Bullet" + Bullets.Count);
            bullet.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position + (GetDependency<Body>(DEPENDENCY_BODY).Bounds/2f) -
                                   (bullet.Body.Bounds/2f);
            bullet.Body.Angle = GetDependency<Body>(DEPENDENCY_BODY).Angle +
                                (0.05f*RandomHelper.GetSign()*RandomHelper.NextGaussian(1, 1f));
            bullet.Physics.Thrust(SPEED);
            bullet.DestroyEvent += component => Bullets.Remove(bullet);

            Bullets.Add(bullet);
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;

        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof (Body));
        }
    }
}