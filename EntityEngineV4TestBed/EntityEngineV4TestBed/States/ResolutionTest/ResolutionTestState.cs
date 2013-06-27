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
using EntityEngineV4.Input.MouseInput;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.ResolutionTest
{
    public class ResolutionTestState : TestBedState
    {
        private SortedSet<string> _collided;
        private Label _collidedLabel;

        public ResolutionTestState(EntityGame eg)
            : base(eg, "ResolutionTestState")
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
                ResolutionTestEntity c = new ResolutionTestEntity(this, "A" + x);
                c.Collision.GroupMask.AddMask(0);
                c.Collision.PairMask.AddMask(0);
                c.Collision.PairMask.AddMask(2);
                c.Collision.ResolutionGroupMask.AddMask(0);
                c.Collision.ResolutionGroupMask.AddMask(1);
                c.Collision.ResolutionGroupMask.AddMask(2);
                c.Collision.CollideEvent += collision => _collided.Add(collision.Parent.Name);
                c.Collision.Debug = true;
                c.Body.Position = new Vector2(30, 80 * x + 20);
                AddEntity(c);
            }
            for (int x = 0; x < 3; x++)
            {
                ResolutionTestEntity c = new ResolutionTestEntity(this, "B" + x);
                c.Collision.GroupMask.AddMask(1);
                c.Collision.PairMask.AddMask(0);
                c.Collision.PairMask.AddMask(2);
                c.Collision.ResolutionGroupMask.AddMask(0);
                c.Collision.CollideEvent += collision => _collided.Add(collision.Parent.Name);
                c.Collision.Debug = true;
                c.Body.Position = new Vector2(510, 80 * x + 20);
                c.Color = Color.Orange;
                c.HoverColor = Color.Violet;
                AddEntity(c);
            }

            for (int x = 0; x < 3; x++)
            {
                ResolutionTestEntity c = new ResolutionTestEntity(this, "C" + x);
                c.Collision.GroupMask.AddMask(2);
                c.Collision.PairMask.AddMask(0);
                c.Collision.PairMask.AddMask(1);
                c.Collision.ResolutionGroupMask.AddMask(0);
                c.Collision.ResolutionGroupMask.AddMask(1);
                c.Collision.CollideEvent += collision => _collided.Add(collision.Parent.Name);
                c.Collision.Immovable = true;
                c.Collision.Debug = true;
                c.Body.Position = new Vector2(50+x*70,450);
                c.Color = Color.Green;
                c.HoverColor = Color.DarkBlue;
                
                AddEntity(c);
            }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (Destroyed) return;

            string debug = GetEntity<ResolutionTestEntity>("A0").Body.Position.ToString(); 
            _collidedLabel.Text = "A0: " + debug;
        }

        private class ResolutionTestEntity : Entity
        {
            public Body Body;
            public Body TextBody;
            public Physics Physics;

            public Collision Collision;
            public ImageRender ImageRender;
            public TextRender TextRender;

            public Color Color = Color.Blue;
            public Color HoverColor = Color.Red;

            private Vector2 _clickposition;
            private Vector2 _releaseposition;
            private bool _hasFocus;
            private bool _lastImmovable;

            public ResolutionTestEntity(EntityState stateref, string name)
                : base(stateref, name)
            {
                Body = new Body(this, "Body");
                Body.Bounds = new Vector2(70, 70);

                Physics = new Physics(this, "Physics", Body);
                Physics.Drag = .95f;

                Collision = new Collision(this, "Collision", new AABB(), Body, Physics);
                Collision.Mass = 10f;
                Collision.Restitution = .5f;

                ImageRender = new ImageRender(this, "Image", Assets.Pixel, Body);
                ImageRender.Scale = new Vector2(70, 70);
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

                if (!_hasFocus && Body.BoundingRect.Contains((int)MouseHandler.Cursor.Position.X,
                                                   (int)MouseHandler.Cursor.Position.Y))
                {
                    ImageRender.Color = HoverColor;
                    if (MouseHandler.IsMouseButtonPressed(MouseButton.LeftButton))
                    {
                        _clickposition = MouseHandler.Cursor.Position;
                        _hasFocus = true;
                    }
                    if (MouseHandler.IsMouseButtonPressed(MouseButton.RightButton))
                    {
                        _hasFocus = true;
                        _lastImmovable = Collision.Immovable;
                    }
                }
                else if (!_hasFocus)
                {
                    //_clickposition = Vector2.Zero;
                    //_releaseposition = Vector2.Zero;
                    ImageRender.Color = Color;
                }

                if (_hasFocus && MouseHandler.IsMouseButtonReleased(MouseButton.LeftButton))
                {
                    _hasFocus = false;
                    _releaseposition = MouseHandler.Cursor.Position;

                    //Add delta to force.
                    Physics.AddForce((_clickposition - _releaseposition) / 2f);
                }
                else if (_hasFocus && MouseHandler.IsMouseButtonDown(MouseButton.RightButton))
                {
                    Body.Position -= new Vector2(MouseHandler.Delta.X, MouseHandler.Delta.Y);
                    Collision.Immovable = true;
                }
                else if (_hasFocus && MouseHandler.IsMouseButtonUp(MouseButton.LeftButton) && MouseHandler.IsMouseButtonUp(MouseButton.RightButton))
                {
                    _hasFocus = false;
                    Collision.Immovable = _lastImmovable;
                }

                //Reset our position if it goes off screen.
                if (Body.BoundingRect.Right < 0)
                    Body.Position.X = EntityGame.Viewport.Width;
                else if (Body.Position.X > EntityGame.Viewport.Width)
                    Body.Position.X = 0 - Body.Bounds.X;

                if (Body.BoundingRect.Bottom < 0)
                    Body.Position.Y = EntityGame.Viewport.Height;
                else if (Body.Position.Y > EntityGame.Viewport.Height)
                    Body.Position.Y = 0 - Body.Bounds.Y;

                TextBody.Position = Body.Position + Vector2.One * 10f;
            }
        }
    }
}