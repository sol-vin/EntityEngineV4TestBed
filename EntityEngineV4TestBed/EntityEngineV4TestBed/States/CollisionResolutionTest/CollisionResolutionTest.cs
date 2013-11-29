using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.CollisionEngine;
using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Components.Rendering.Primitives;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
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
        private Label _titleLabel, _descriptionLabel;

        public CollisionResolutionTest() : base("CollisionResolutionTest")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _currentTest = new CircleTest1(this, "CircleTest1");
            _currentTest.Initialize();

            _beginTest = new DoubleInput(this, "BeginTest", Keys.Enter, Buttons.Start, PlayerIndex.One);
            _nextTest = new DoubleInput(this, "NextTest", Keys.Right, Buttons.RightShoulder, PlayerIndex.One);
            _lastTest = new DoubleInput(this, "LastTest", Keys.Left, Buttons.LeftShoulder, PlayerIndex.One);

            Page p =new Page(this, "Page");
            p.Show();

            _titleLabel = new Label(p, "TitleLabel", new Point(0,0));
            _titleLabel.Text = _currentTest.Title;
            _titleLabel.Body.Position = new Vector2(2,2);

            _descriptionLabel =new Label(p, "DescriptionLabel", new Point(0,1));
            _descriptionLabel.Text = _currentTest.Description;
            _descriptionLabel.Body.Position = new Vector2(2, EntityGame.Viewport.Height - _descriptionLabel.Render.Bounds.Y - 2);

            //Init tests
            _tests.Add(_currentTest);
            _tests.Add(new CircleTest2(this, "CircleTest2"));
            _tests.Add(new CircleTest3(this, "CircleTest3"));
            _tests.Add(new CircleTest4(this, "CircleTest4"));
            _tests.Add(new CircleTest5(this, "CirlceTest5"));
            _tests.Add(new CircleTest6(this, "CirlceTest6"));
            _tests.Add(new CircleTest7(this, "CirlceTest7"));
            _tests.Add(new CircleTest8(this, "CirlceTest8"));
            _tests.Add(new CircleTest9(this, "CirlceTest9"));
            _tests.Add(new CircleTest10(this, "CirlceTest10"));



            _tests.Add(new AABBTest1(this, "AABBTest1"));
            _tests.Add(new AABBTest2(this, "AABBTest2"));
            _tests.Add(new AABBTest3(this, "AABBTest3"));
            _tests.Add(new AABBTest4(this, "AABBTest4"));

        }

        public void LastTest()
        {
            _currentTest.DestroyChildren();
            _currentTest.Reset();

            --_currentTestNumber;
            if (_currentTestNumber < 0) _currentTestNumber = _tests.Count - 1;
            _currentTest = _tests[_currentTestNumber];
            _currentTest.Initialize();
        }

        public void NextTest()
        {
            _currentTest.DestroyChildren();
            _currentTest.Reset();

            ++_currentTestNumber;
            if (_currentTestNumber > _tests.Count-1) _currentTestNumber = 0;
            _currentTest = _tests[_currentTestNumber];
            _currentTest.Initialize();
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            _titleLabel.Text = _currentTest.Title;

            _descriptionLabel.Text = _currentTest.Description;
            
            if (_beginTest.Released())
                _currentTest.Begin();

            if(_nextTest.Released())
                NextTest();
            else if(_lastTest.Released())
                LastTest();
        }
    }

    public class Test : Node
    {
        public string Title;
        public string Description;
        public Test(Node parent, string name) : base(parent, name)
        {
            Active = false;
            Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            Active = false;
            Visible = true;
        }

        public virtual void Begin()
        {
            Active = true;
        }

        public override void Reset()
        {
            base.Reset();
            Active = false;
            Visible = false;
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
                Collision.Group.AddMask(0);
                Collision.Pair.AddMask(0);
                Collision.ResolutionGroup.AddMask(0);
                Collision.LinkDependency(Collision.DEPENDENCY_PHYSICS, Physics);
            }
        }

        public class AABBTester : TestEntity
        {
            public AABB AABB;
            public ShapeTypes.Rectangle Render;

            public AABBTester(Node parent, string name, float width=20, float height=20)
                : base(parent, name)
            {
                AABB = new AABB(this, "AABB");
                AABB.LinkDependency(AABB.DEPENDENCY_BODY, Body);
                AABB.LinkDependency(AABB.DEPENDENCY_COLLISION, Collision);

                Body.Width = width;
                Body.Height = height;

                Render = new ShapeTypes.Rectangle(this, "Render", true);
                Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);

                Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, AABB);
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
                
                Body.Origin = new Vector2(Render.Texture.Width/2f, Render.Texture.Height/2f);
                Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Circle);
            }
        }
    }

    public class CircleTestMaker : Test
    {
        public CircleTester A, B;
        public Page ControlContainer;

        public CircleTestMaker(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test Maker";
            Description = @"Use the controls to change the test";
        }

        public override void Initialize()
        {
            base.Initialize();

            ControlContainer = new Page(this, "ControlContainer");
            ControlContainer.Show();

            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Physics.Restitution = 1.5f;
            A.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - A.Body.Width / 2, 20);
            A.Physics.Acceleration.Y = .1f;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 1.5f;
            B.Collision.Immovable = true;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - B.Body.Width / 2,
                EntityGame.Viewport.Height - 20 - B.Body.Height);
        }
    }

    public class CircleTest1 : Test
    {
        public CircleTester A, B;

        public CircleTest1(Node parent, string name) : base(parent, name)
        {
            Title = "Circle Test 1";
            Description = @"Smashes two circles together on the X axis, they are exactly aligned";
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height/2f - A.Body.Height/2f);
            A.Physics.Velocity.X = 200;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width,
                EntityGame.Viewport.Height/2f - B.Body.Height/2f);
            B.Physics.Velocity.X = -200;
        }
    }

    public class CircleTest2 : Test
    {
        public CircleTester A, B;

        public CircleTest2(Node parent, string name) : base(parent, name)
        {
            Title = "Circle Test 2";
            Description = @"Smashes two circles together on the X axis, they are misaligned";
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height/2f - A.Body.Height/2f - 5);
            A.Physics.Velocity.X = 200;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width,
                EntityGame.Viewport.Height/2f - B.Body.Height/2f + 5);
            B.Physics.Velocity.X = -200;
        }
    }

    public class CircleTest3 : Test
    {
        public CircleTester A, B;

        public CircleTest3(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test 3";
            Description = @"Smashes two circles together on the X axis, one is stationary";
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height / 2f - A.Body.Height / 2f);
            A.Physics.Velocity.X = 200;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 100 - B.Body.Width,
                EntityGame.Viewport.Height / 2f - B.Body.Height / 2f);
            //B.Physics.Velocity.X = -1;
        }
    }

    public class CircleTest4 : Test
    {
        public CircleTester A, B;

        public CircleTest4(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test 4";
            Description = @"Smashes two circles together on the X axis, one is stationary and misaligned";
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height / 2f - A.Body.Height / 2f);
            A.Physics.Velocity.X = 200;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 100 - B.Body.Width,
                EntityGame.Viewport.Height / 2f - B.Body.Height / 2f - 5);
            //B.Physics.Velocity.X = -1;
        }
    }

    public class CircleTest5 : Test
    {
        public CircleTester A, B;

        public CircleTest5(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test 5";
            Description = @"Collision on Y axis, B is immovable, A has gravity.";
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Physics.Restitution = 2f;

            A.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - A.Body.Width/2, 20);
            A.Physics.Acceleration.Y = 25f;
            
            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 2f;
            B.Collision.Immovable = true;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - B.Body.Width / 2,
                EntityGame.Viewport.Height - 20 - B.Body.Height);
            //B.Physics.Velocity.X = -1;
        }
    }

    public class CircleTest6 : Test
    {
        public CircleTester A, B;

        public CircleTest6(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test 6";
            Description = @"Collision on Y axis, B is immovable, A has gravity and is misaligned.";
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Physics.Restitution = 2f;

            A.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - A.Body.Width / 2 - 5, 20);
            A.Physics.Acceleration.Y = 25f;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 2f;
            B.Collision.Immovable = true;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - B.Body.Width / 2,
                EntityGame.Viewport.Height - 20 - B.Body.Height);
            //B.Physics.Velocity.X = -1;
        }
    }

    public class CircleTest7 : Test
    {
        public CircleTester B;
        public Timer EmitTimer;

        public CircleTest7(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test 7";
            Description = @"Collision speed test";
        }

        public override void Initialize()
        {
            base.Initialize();

            EmitTimer = new Timer(this, "EmitTimer");
            EmitTimer.Milliseconds = 250;
            EmitTimer.LastEvent += EmitParticle;

            B = new CircleTester(this, "B", 60);
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 2f;
            B.Collision.Immovable = true;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - B.Body.Width/2,
                EntityGame.Viewport.Height - 20 - B.Body.Height);
            B.Debug = true;
            //B.Physics.Velocity.X = -1;
        }

        public override void Begin()
        {
            base.Begin();
            EmitTimer.Start();
        }

        public void EmitParticle()
        {
            var c = new CircleParticle(this, "CircleParticle", RandomHelper.GetFloat(3, 10));
            c.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - c.Body.Width/2f, -20);
            c.Body.Angle = RandomHelper.GetFloat(0, MathHelper.TwoPi);
            c.Physics.Thrust(10f);
            c.Physics.Restitution = RandomHelper.GetFloat(1f, 2f);
            c.Render.Color = Color.Red;
            //c.Circle.Debug = true;
        }

        private class CircleParticle : CircleTester
        {
            public CircleParticle(Node parent, string name, float radius) : base(parent, name, radius)
            {
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                if (Body.Position.Y > EntityGame.Viewport.Height)
                    Destroy(this);
            }

            public override void Initialize()
            {
                base.Initialize();
                //Keep them from colliding with each other
                Collision.Group.RemoveMask(0);

                //Gravity
                Physics.Acceleration.Y = 20f;
            }
        }
    }

    public class CircleTest8 : Test
    {
        public CircleTester A, B, C;

        public CircleTest8(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test 8";
            Description = @"Collision on Y axis, B is immovable, A has gravity.";
        }

        public override void Initialize()
        {
            base.Initialize();

            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Physics.Restitution = 1.5f;
            A.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - A.Body.Width/2, 20);
            A.Physics.Acceleration.Y = 10f;

            C = new CircleTester(this, "C", 30);
            C.Render.Color = Color.Red;
            C.Physics.Restitution = 1.5f;
            C.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - A.Body.Width/2, 200);
            C.Physics.Acceleration.Y = 10f;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 1.5f;
            B.Collision.Immovable = true;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - B.Body.Width/2,
                EntityGame.Viewport.Height - 20 - B.Body.Height);
            //B.Physics.Velocity.X = -1;
        }

    }

    public class CircleTest9 : Test
    {
        public CircleTester A, B;

        public CircleTest9(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test 9";
            Description = @"Smashes two circles together on the X axis, Acceleration";
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height / 2f - A.Body.Height / 2f);
            A.Physics.Acceleration.X = 3;
            A.Physics.Restitution = .8f;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width,
                EntityGame.Viewport.Height / 2f - B.Body.Height / 2f);
            B.Physics.Acceleration.X = -3f;
            B.Physics.Restitution = .8f;

        }    
    }

    public class CircleTest10 : Test
    {
        public CircleTester A, B;
        private CircleTester C;
        private CircleTester D;

        public CircleTest10(Node parent, string name)
            : base(parent, name)
        {
            Title = "Circle Test 10";
            Description = @"Smashes four circles together on the X axis, Acceleration";
        }

        public override void Initialize()
        {
            base.Initialize();
            A = new CircleTester(this, "A", 30);
            A.Render.Color = Color.Red;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height/2f - A.Body.Height/2f);
            A.Physics.Acceleration.X = 3;
            A.Physics.Restitution = .8f;

            B = new CircleTester(this, "B", 30);
            B.Render.Color = Color.Blue;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width,
                EntityGame.Viewport.Height/2f - B.Body.Height/2f);
            B.Physics.Acceleration.X = -3f;
            B.Physics.Restitution = .8f;

            C = new CircleTester(this, "C", 30);
            C.Render.Color = Color.Red;
            C.Body.Position = new Vector2(100, EntityGame.Viewport.Height / 2f - C.Body.Height / 2f);
            C.Physics.Acceleration.X = 3;
            C.Physics.Restitution = .8f;

            D = new CircleTester(this, "D", 30);
            D.Render.Color = Color.Blue;
            D.Body.Position = new Vector2(EntityGame.Viewport.Width - 100 - D.Body.Width,
                EntityGame.Viewport.Height / 2f - D.Body.Height / 2f);
            D.Physics.Acceleration.X = -3f;
            D.Physics.Restitution = .8f;

        }

    }

    public class AABBTest1 : Test
    {
        public AABBTester A, B;

        public AABBTest1(Node parent, string name)
            : base(parent, name)
        {
            Title = "AABB Test 1";
            Description = @"Collision on X axis, perfect alignment, velocity.";
        }

        public override void Initialize()
        {
            base.Initialize();

            A = new AABBTester(this, "A");
            A.Render.Color = Color.Red;
            A.Physics.Restitution = 1.5f;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height/2f - A.Body.Height/2f);
            A.Physics.Velocity.X = 200;

            B = new AABBTester(this, "B");
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 1.5f;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width, EntityGame.Viewport.Height / 2f - B.Body.Height / 2f);
            B.Physics.Velocity.X = -200;
        }
    }

    public class AABBTest2 : Test
    {
        public AABBTester A, B;

        public AABBTest2(Node parent, string name)
            : base(parent, name)
        {
            Title = "AABB Test 2";
            Description = @"Collision on X axis, imperfect alignment, velocity.";
        }

        public override void Initialize()
        {
            base.Initialize();

            A = new AABBTester(this, "A");
            A.Render.Color = Color.Red;
            A.Physics.Restitution = 1.5f;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height / 2f - A.Body.Height / 2f - 10);
            A.Physics.Velocity.X = 200;

            B = new AABBTester(this, "B");
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 1.5f;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width, EntityGame.Viewport.Height / 2f - B.Body.Height / 2f);
            B.Physics.Velocity.X = -200;
        }
    }

    public class AABBTest3 : Test
    {
        public AABBTester A, B;

        public AABBTest3(Node parent, string name)
            : base(parent, name)
        {
            Title = "AABB Test 3";
            Description = @"Collision on X axis, perfect alignment, acceleration.";
        }

        public override void Initialize()
        {
            base.Initialize();

            A = new AABBTester(this, "A");
            A.Render.Color = Color.Red;
            A.Physics.Restitution = 0.5f;
            A.Body.Position = new Vector2(20, EntityGame.Viewport.Height/2f - A.Body.Height/2f);
            A.Physics.Acceleration.X = 10f;

            B = new AABBTester(this, "B");
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 0.5f;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width - 20 - B.Body.Width,
                EntityGame.Viewport.Height/2f - B.Body.Height/2f);
            B.Physics.Acceleration.X = -10f;
        }
    }
    public class AABBTest4 : Test
    {
        public AABBTester A, B;

        public AABBTest4(Node parent, string name)
            : base(parent, name)
        {
            Title = "AABB Test 4";
            Description = @"Collision on Y axis, perfect alignment, gravity.";
        }

        public override void Initialize()
        {
            base.Initialize();

            A = new AABBTester(this, "A");
            A.Render.Color = Color.Red;
            A.Physics.Restitution = 0.5f;
            A.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - A.Body.Width / 2f,
                20);
            A.Physics.Acceleration.Y = 10f;

            B = new AABBTester(this, "B");
            B.Render.Color = Color.Blue;
            B.Physics.Restitution = 0.5f;
            B.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - B.Body.Width / 2f,
                EntityGame.Viewport.Height  - 20 - B.Body.Height);
            B.Collision.Immovable = true;
        }
    }
}