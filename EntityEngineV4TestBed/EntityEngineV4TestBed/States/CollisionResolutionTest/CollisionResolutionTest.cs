using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Collision;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.CollisionResolution
{
    public class CollisionResolutionTest : TestBedState
    {
        private Test _currentTest;
        private int _currentTestNumber;
        private DoubleInput _nextTest, _lastTest, _beginTest;
        private List<Test> _tests = new List<Test>();


        public CollisionResolutionTest() : base("CollisionResolutionTest")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            new CollisionHandler(this);

            _currentTest = new CircleTest2(this, "CircleTest2");

            _beginTest = new DoubleInput(this, "BeginTest", Keys.Enter, Buttons.Start, PlayerIndex.One);
        }

        public void LastTest()
        {
            _currentTest.Destroy();
            _currentTest.Reset();

            _currentTest = _tests[--_currentTestNumber];
            _currentTest.Initialize();
        }

        public void NextTest()
        {
            _currentTest.Destroy();
            _currentTest.Reset();

            _currentTest = _tests[++_currentTestNumber];
            _currentTest.Initialize();
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (_beginTest.Released())
                _currentTest.Begin();
        }
    }

    public class Test : Node
    {
        public Test(Node parent, string name) : base(parent, name)
        {
            Active = false;
        }

        public virtual void Begin()
        {
            Active = true;
        }

        public class TestEntity : Node
        {
            public Body Body;
            public Physics Physics;
            public Collision Collision;

            public TestEntity(Node parent, string name) : base(parent, name)
            {
                Body = new Body(this, "Body");

                Physics = new Physics(this, "Physics");
                Physics.Mass = 10;
                Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

                Collision = new Collision(this, "Collision");
                Collision.GroupMask.AddMask(0);
                Collision.PairMask.AddMask(0);
                Collision.ResolutionGroupMask.AddMask(0);
                Collision.LinkDependency(Collision.DEPENDENCY_PHYSICS, Physics);
            }
        }

        public class CircleTester : TestEntity
        {
            public Circle Circle;
            public ImageRender Render;

            public CircleTester(Node parent, string name, float radius) : base(parent, name)
            {
                Circle = new Circle(this, "Circle", radius);
                Circle.Offset = new Vector2(radius);
                Circle.LinkDependency(Circle.DEPENDENCY_BODY, Body);
                Circle.LinkDependency(Circle.DEPENDENCY_COLLISION, Collision);

                Body.Bounds = new Vector2(Circle.Diameter);

                Render = new ImageRender(this, "Render", Assets.Circle);
                Render.Scale = new Vector2(Circle.Diameter/Assets.Circle.Width);
                Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);

                Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Circle);
            }
        }
    }

    public class CircleTest1 : Test
    {
        public CircleTester A, B;

        public CircleTest1(Node parent, string name) : base(parent, name)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height/2f - A.Body.Height/2f);
            A.Physics.Velocity.X = 1;

            B = new CircleTester(this, "B", 30);
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width,
                EntityGame.Viewport.Height/2f - B.Body.Height/2f);
            B.Physics.Velocity.X = -1;
        }
    }


    public class CircleTest2 : Test
    {
        public CircleTester A, B;

        public CircleTest2(Node parent, string name) : base(parent, name)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height/2f - A.Body.Height/2f - 5);
            A.Physics.Velocity.X = 1;

            B = new CircleTester(this, "B", 30);
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width,
                EntityGame.Viewport.Height/2f - B.Body.Height/2f + 5);
            B.Physics.Velocity.X = -1;
        }
    }
}