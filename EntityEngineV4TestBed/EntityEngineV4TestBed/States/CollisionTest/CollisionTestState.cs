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
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.CollisionTest
{
    public class CollisionTestState : TestBedState
    {
        private SortedSet<string> _collided;
        private Label _collidedLabel;

        public CollisionTestState(EntityGame eg) : base(eg, "CollisionTestState")
        {
            Services.Add(new InputHandler(this));
            Services.Add(new CollisionHandler(this));
            Services.Add(new MouseHandler(this));

            _collided = new SortedSet<string>();
            
            _collidedLabel = new Label(this, "CollidedLabel");
            _collidedLabel.Body.Position = new Vector2(10, 560);
            AddEntity(_collidedLabel);

            for (int x = 0; x < 3; x++)
            {
                CollisionTestEntity c = new CollisionTestEntity(this, "A" + x);
                c.Collision.GroupMask.AddMask(0);
                c.Collision.CollideEvent += collision => _collided.Add(collision.Parent.Name);
                c.Body.Position = new Vector2(30, 80 * x + 20);
                AddEntity(c);
            }
            for (int x = 0; x < 3; x++)
            {
                CollisionTestEntity c = new CollisionTestEntity(this, "B" + x);
                c.Collision.PairMask.AddMask(0);
                c.Collision.CollideEvent += collision => _collided.Add(collision.Parent.Name);
                c.Body.Position = new Vector2(510, 80 * x + 20);
                c.Color = Color.Orange;
                c.HoverColor = Color.Black;
                AddEntity(c);
            }

        }

        public override void Update(GameTime gt)
        {
            string text = "Currently collided: ";
            foreach (var c in _collided)
            {
                text += c + " ";
            }
            _collidedLabel.Text = text;

            _collided.Clear();
            base.Update(gt);
        }

        private class CollisionTestEntity : Entity
        {
            public Body Body;
            public Body TextBody;
            public Physics Physics;

            public Collision Collision;
            public ImageRender ImageRender;
            public TextRender TextRender;

            public Color Color = Color.Blue;
            public Color HoverColor = Color.Red;
            private bool _hasFocus;

            public CollisionTestEntity(EntityState stateref, string name) : base(stateref, name)
            {
                Body = new Body(this, "Body");
                Body.Bounds = new Vector2(70,70);

                Physics = new Physics(this, "Physics", Body);

                Collision = new Collision(this, "Collision", new AABB(), Body, Physics);

                ImageRender = new ImageRender(this, "Image", Assets.Pixel, Body);
                ImageRender.Scale =  new Vector2(70,70);
                ImageRender.Layer = .5f;

                TextBody = new Body(this, "TextBody");

                TextRender = new TextRender(this, "TextRender", TextBody);
                TextRender.Color = Color.White;
                TextRender.Font = Assets.Font;
                TextRender.Text = Name;
                TextRender.Layer = 1f;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt); 
                //If the cursor is over the object, and the button is down
                if (Body.BoundingRect.Contains((int) MouseHandler.Cursor.Position.X,
                                               (int) MouseHandler.Cursor.Position.Y))
                {
                    ImageRender.Color = HoverColor;
                    if (MouseHandler.IsMouseButtonPressed(MouseButton.LeftButton))
                        _hasFocus = true;
                }
                else
                {
                    ImageRender.Color = Color;
                }

                if (_hasFocus && MouseHandler.IsMouseButtonDown(MouseButton.LeftButton))
                {
                    Body.Position -= new Vector2(MouseHandler.Delta.X, MouseHandler.Delta.Y);
                }
                else if (_hasFocus && MouseHandler.IsMouseButtonUp(MouseButton.LeftButton))
                {
                    _hasFocus = false;
                }

                TextBody.Position = Body.Position + Vector2.One * 10f;
            }
        }
    }
}
