using EntityEngineV4.Collision;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace EntityEngineV4Test
{
    [TestClass]
    public class CollisionTest
    {
        [TestMethod]
        public void CollideAABBvsAABB1()
        {
            Game g = new Game();
            GraphicsDeviceManager gdm = new GraphicsDeviceManager(g);
            gdm.ApplyChanges();

            EntityGame.MakeGame(g, null);

            //Create a teststate
            EntityState es = new EntityState("State");
            es.Show();

            //Invoke the collision handler
            CollisionHandler ch = new CollisionHandler(es);
            es.AddService(ch);

            //create our testentity and components
            Entity e = new Entity(es, "Entity");
            Collision a, b;

            Body abody = new Body(e, "BodyA");
            abody.Position = new Vector2(50, 50);
            abody.Bounds = new Vector2(300, 100);

            a = new Collision(e, "CollisionA", new AABB());
            a.Link(Collision.DEPENDENCY_BODY, abody);
            a.PairMask.AddMask(0);
            a.GroupMask.AddMask(0);

            Body bbody = new Body(e, "BodyB");
            bbody.Position = new Vector2(100, 90);
            bbody.Bounds = new Vector2(100, 100);
            b = new Collision(e, "CollisionB", new AABB());
            b.Link(Collision.DEPENDENCY_BODY, bbody);
            b.GroupMask.AddMask(0);
            b.PairMask.AddMask(0);

            Manifold m = CollisionHandler.AABBvsAABB((AABB)a.Shape, (AABB)b.Shape);

            bool aCollisionDirection = m.A.CollisionDirection.HasMatchingBit(CollisionHandler.DOWN);
            bool bCollisionDirection = m.B.CollisionDirection.HasMatchingBit(CollisionHandler.UP);

            //Start testing and ensuring that our values are as expected
            if (!(m.AreColliding && aCollisionDirection && bCollisionDirection))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CollideAABBvsAABB2()
        {
            //Create a teststate
            EntityState es = new EntityState("State");
            es.Show();
            //Invoke the collision handler
            CollisionHandler ch = new CollisionHandler(es);
            es.AddService(ch);

            //create our testentity and components
            Entity e = new Entity(es, "Entity");
            Collision a, b;

            Body abody = new Body(e, "BodyA");
            abody.Position = new Vector2(50, 50);
            abody.Bounds = new Vector2(300, 100);

            a = new Collision(e, "CollisionA", new AABB());
            a.Link(Collision.DEPENDENCY_BODY, abody);

            a.PairMask.AddMask(0);
            a.GroupMask.AddMask(0);

            Body bbody = new Body(e, "BodyB");
            bbody.Position = new Vector2(10, 50);
            bbody.Bounds = new Vector2(50, 100);
            b = new Collision(e, "CollisionB", new AABB());
            b.Link(Collision.DEPENDENCY_BODY, bbody);

            b.GroupMask.AddMask(0);
            b.PairMask.AddMask(0);

            Manifold m = CollisionHandler.AABBvsAABB((AABB)a.Shape, (AABB)b.Shape);

            bool aCollisionDirection = m.A.CollisionDirection.HasMatchingBit(CollisionHandler.LEFT);
            bool bCollisionDirection = m.B.CollisionDirection.HasMatchingBit(CollisionHandler.RIGHT);

            //Start testing and ensuring that our values are as expected
            if (!(m.AreColliding && aCollisionDirection && bCollisionDirection))
            {
                Assert.Fail();
            }
        }
    }
}