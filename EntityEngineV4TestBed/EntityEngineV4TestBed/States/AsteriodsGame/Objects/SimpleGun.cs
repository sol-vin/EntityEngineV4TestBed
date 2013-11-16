using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class SimpleGun : Gun
    {
        public const float SPEED = 5f;

        public SimpleGun(Node parent, string name) : base(parent, name)
        {
        }

        public override void Fire()
        {
            var bullet = GetRoot<State>().GetNextRecycled<Bullet>(this, "BulletRecycled") ?? new Bullet(this, "Bullet" + Count);
            bullet.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position + (GetDependency<Body>(DEPENDENCY_BODY).Bounds/2f) -
                                   (bullet.Body.Bounds/2f);
            bullet.Body.Angle = GetDependency<Body>(DEPENDENCY_BODY).Angle +
                                (0.05f*RandomHelper.GetSign()*RandomHelper.NextGaussian(1, 1f));
            bullet.Physics.Thrust(SPEED + GetDependency<Physics>(DEPENDENCY_PHYSICS).Velocity.Length());
            bullet.DestroyEvent += component => Remove(bullet);
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
        public const int DEPENDENCY_PHYSICS = 1;
        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof (Body));
            AddLinkType(DEPENDENCY_PHYSICS, typeof(Physics));
        }
    }
}