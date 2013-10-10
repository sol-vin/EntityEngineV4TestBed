using System;
using System.Collections.Generic;
using EntityEngineV4.Collision;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
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

        public override void Create()
        {
            base.Create();

            CollisionHandler colhand = new CollisionHandler(this);
            ControlHandler ch = new ControlHandler(this);

            _collided = new SortedSet<string>();

            _collidedLabel = new Label(ch, "CollidedLabel");
            _collidedLabel.Body.Position = new Vector2(10, 560);
            ch.AddControl(_collidedLabel);

            Random rand = new Random();

            for (int x = 0; x < 3; x++)
            {
                CollisionTestEntity c = new CollisionTestEntity(this, "A" + x);
                if (c.Name == "A0")
                    c.Collision.CollisionDirection.BitmaskChanged += bm => BitmaskChanged();
                c.Collision.GroupMask.AddMask(0);
                c.Collision.PairMask.AddMask(0);
                c.Collision.CollideEvent += collision => _collided.Add(collision.Parent.Name);
                c.Body.Bounds = new Vector2(50 + rand.Next(1, 100), 50 + rand.Next(1, 100));
                c.ImageRender.Scale = c.Body.Bounds;
                c.Collision.Debug = true;
                c.Body.Position = new Vector2(30, 100 * x + 20);
                //AddEntity(c);
            }
            for (int x = 0; x < 3; x++)
            {
                CollisionTestEntity c = new CollisionTestEntity(this, "B" + x);
                c.Collision.GroupMask.AddMask(1);
                c.Collision.PairMask.AddMask(0);
                c.Collision.CollideEvent += collision => _collided.Add(collision.Parent.Name);
                c.Body.Position = new Vector2(510, 80 * x + 20);
                c.Body.Bounds = new Vector2(50 + rand.Next(1, 100), 50 + rand.Next(1, 100));
                c.ImageRender.Scale = c.Body.Bounds;
                c.Color = Color.Orange;
                c.HoverColor = Color.Black;
                c.Collision.Debug = true;
                //AddEntity(c);
            }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (Destroyed) return;
            Bitmask mask = GetEntity<CollisionTestEntity>("A0").Collision.CollisionDirection;
            string text = "Collision Directions (A0): ";

            if (mask.HasMatchingBit(CollisionHandler.LEFT))
                text += "Left ";
            if (mask.HasMatchingBit(CollisionHandler.RIGHT))
                text += "Right ";
            if (mask.HasMatchingBit(CollisionHandler.UP))
                text += "Up ";
            if (mask.HasMatchingBit(CollisionHandler.DOWN))
                text += "Down ";

            _collidedLabel.Text = text;

            _collided.Clear();
        }

        public void BitmaskChanged()
        {
            Console.WriteLine("BITMASK CHANGED!");
        }

        /// <summary>
        /// Simple entity to test collisions
        /// </summary>
        private class CollisionTestEntity : Entity
        {
            public Body Body;

            /// <summary>
            /// Body used to describe TextRender's body
            /// </summary>
            public Body TextBody;

            public Physics Physics;

            public Collision Collision;
            public ImageRender ImageRender;
            public TextRender TextRender;

            public Color Color = Color.Blue;
            public Color HoverColor = Color.Red;

            /// <summary>
            /// Whether or not it has the mouse's attention
            /// </summary>
            private bool _hasFocus;

            /// <summary>
            /// /Creates a new CollisionTestEntity
            /// </summary>
            /// <param name="stateref"></param>
            /// <param name="name"></param>
            public CollisionTestEntity(EntityState stateref, string name)
                : base(stateref, name)
            {
                Body = new Body(this, "Body");
                Body.Bounds = new Vector2(70, 70);

                Physics = new Physics(this, "Physics");
                Physics.Link(Physics.DEPENDENCY_BODY, Body);

                Collision = new Collision(this, "Collision", new AABB(), Body, Physics);

                ImageRender = new ImageRender(this, "Image", Assets.Pixel);
                ImageRender.Link(ImageRender.DEPENDENCY_BODY, Body);
                ImageRender.Scale = new Vector2(70, 70);
                ImageRender.Layer = .5f;

                TextBody = new Body(this, "TextBody");

                TextRender = new TextRender(this, "TextRender");
                TextRender.Link(TextRender.DEPENDENCY_BODY, TextBody);
                TextRender.Color = Color.White;
                TextRender.Font = Assets.Font;
                TextRender.Text = Name;
                TextRender.Layer = 1f;
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
    }
}