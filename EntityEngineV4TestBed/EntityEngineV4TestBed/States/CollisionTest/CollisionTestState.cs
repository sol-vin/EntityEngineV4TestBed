using System;
using System.Collections.Generic;
using EntityEngineV4.CollisionEngine;
using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.CollisionTest
{
    public class CollisionTestState : TestBedState
    {
        private SortedSet<string> _collided;
        private Label _collidedLabel;

        public CollisionTestState()
            : base("CollisionTestState")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            ControlHandler ch = new ControlHandler(this);

            _collided = new SortedSet<string>();

            Page p = new Page(this, "Page");
            p.Show();

            _collidedLabel = new Label(p, "CollidedLabel" , new Point(0,0));
            _collidedLabel.Body.Position = new Vector2(10, 500);

            Random rand = new Random();

            for (int x = 0; x < 3; x++)
            {
                AabbNode c = new AabbNode(this, "A" + x);
                c.Collision.Group.AddMask(0);
                c.Collision.Pair.AddMask(0);
                c.Collision.CollideEvent += manifold => _collided.Add(manifold.A != c.Collision ? manifold.A.Parent.Name : manifold.B.Parent.Name);
                c.Collision.Debug = true;
                c.Body.Position = new Vector2(30, 100 * x + 20);
                c.Shape.Debug = true;
            }
            for (int x = 0; x < 3; x++)
            {
                AabbNode c = new AabbNode(this, "B" + x);
                c.Collision.Group.AddMask(1);
                c.Collision.Pair.AddMask(0);
                c.Collision.CollideEvent += manifold => _collided.Add(manifold.A != c.Collision ? manifold.A.Parent.Name : manifold.B.Parent.Name);
                c.Body.Position = new Vector2(510, 80 * x + 20);
                c.Color = Color.Orange;
                c.HoverColor = Color.Black;
                c.Collision.Debug = true;
            }

            for (int x = 0; x < 3; x++)
            {
                CircleNode c = new CircleNode(this, "C" + x);
                c.Collision.Group.AddMask(2);
                c.Collision.Pair.AddMask(2);
                c.Collision.CollideEvent += manifold => _collided.Add(manifold.A != c.Collision ? manifold.A.Parent.Name : manifold.B.Parent.Name); ;
                c.Body.Position = new Vector2(80 * x + 20, 300);
                c.Color = Color.LightBlue;
                c.HoverColor = Color.DarkRed;
                c.Shape.Debug = true;
            }

            
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            string text = "";
            text += "Colliding: ";
            CollisionHandler c = GetService<CollisionHandler>();
            IEnumerable<Collision> list = c.GetColliding();
            foreach (var collider in GetService<CollisionHandler>().GetColliding())
            {
                text += collider.Parent.Name + " ";
            }

            text += '\n';
            text += "C0 -> C1: ";

            Vector2 delta = GetRoot().GetChild<CircleNode>("C0").Body.Position - GetRoot().GetChild<CircleNode>("C1").Body.Position;
            text += delta.Length();

            _collidedLabel.Text = text;
            _collided.Clear();
        }

        private class CollisionTestNode : Node
        {
             public Body Body;

            /// <summary>
            /// Body used to describe Render's body
            /// </summary>
            public Body TextBody;

            public Physics Physics;

            public Collision Collision;
            public ImageRender ImageRender;
            public TextRender TextRender;
            public Shape Shape;

            public Color Color = Color.Blue;
            public Color HoverColor = Color.Red;

            /// <summary>
            /// Whether or not it has the mouse's attention
            /// </summary>
            private bool _hasFocus;

            /// <summary>
            /// /Creates a new AABBEntity
            /// </summary>
            /// <param name="stateref"></param>
            /// <param name="name"></param>
            public CollisionTestNode(State stateref, string name)
                : base(stateref, name)
            {
                Body = new Body(this, "Body");

                Physics = new Physics(this, "Physics");
                Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

                Collision = new Collision(this, "Collision");
                Collision.LinkDependency(Collision.DEPENDENCY_PHYSICS, Physics);

                ImageRender = new ImageRender(this, "Image", Assets.Pixel);
                ImageRender.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);

                TextBody = new Body(this, "TextBody");

                TextRender = new TextRender(this, "Render");
                TextRender.LinkDependency(TextRender.DEPENDENCY_BODY, TextBody);

            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                //If the cursor is over the object, and the button is down
                if (Body.BoundingRect.Contains((int)MouseService.Cursor.Position.X,
                                               (int)MouseService.Cursor.Position.Y))
                {
                    ImageRender.Color = HoverColor;
                    if (MouseService.IsMouseButtonPressed(MouseButton.LeftButton))
                        _hasFocus = true;
                }
                else
                {
                    ImageRender.Color = Color;
                }

                if (_hasFocus && MouseService.IsMouseButtonDown(MouseButton.LeftButton))
                {
                    Body.Position -= new Vector2(MouseService.Delta.X, MouseService.Delta.Y);
                }
                else if (_hasFocus && MouseService.IsMouseButtonUp(MouseButton.LeftButton))
                {
                    _hasFocus = false;
                }

                TextBody.Position = Body.Position + Vector2.One * 10f;
            }
        }

        /// <summary>
        /// Simple entity to test collisions
        /// </summary>
        private class AabbNode : CollisionTestNode
        {
            public AabbNode(State stateref, string name) : base(stateref, name)
            {
                Body.Bounds = new Vector2(50 + RandomHelper.GetFloat(0, 30), 50 + RandomHelper.GetFloat(0, 30));

                Shape = new AABB(this, "AABB");
                Shape.LinkDependency(AABB.DEPENDENCY_BODY, Body);
                Shape.LinkDependency(AABB.DEPENDENCY_COLLISION, Collision);

                Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Shape);

               
                ImageRender.Scale = new Vector2(Body.Width, Body.Height);
                ImageRender.Layer = .5f;

                
                TextRender.Color = Color.White;
                TextRender.Font = Assets.Font;
                TextRender.Text = Name;
                TextRender.Layer = 1f;
            }
        }

        private class CircleNode : CollisionTestNode
        {
            public CircleNode(State stateref, string name)
                : base(stateref, name)
            {
                Shape = new Circle(this, "Circle", 30);
                Shape.Offset = new Vector2((Shape as Circle).Radius);
                Shape.LinkDependency(Circle.DEPENDENCY_BODY, Body);
                Shape.LinkDependency(Circle.DEPENDENCY_COLLISION, Collision);

                Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Shape);

                Body.Bounds = new Vector2((Shape as Circle).Diameter);

                ImageRender.Scale = new Vector2(Body.Width, Body.Height);
                ImageRender.Layer = .5f;

                TextRender.Color = Color.White;
                TextRender.Font = Assets.Font;
                TextRender.Text = Name;
                TextRender.Layer = 1f;
            }
        }
    }
}