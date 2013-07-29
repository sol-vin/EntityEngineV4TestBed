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
            AddService(new CollisionHandler(this));
            AddService(new MouseHandler(this));
            ControlHandler ch = new ControlHandler(this);
            AddService(ch);

            _collided = new SortedSet<string>();

            _collidedLabel = new Label(ch, "CollidedLabel");
            _collidedLabel.Body.Position = new Vector2(10, 560);
            ch.AddControl(_collidedLabel);
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
            private Body _textBody;
            private Physics _physics;

            public Collision Collision;
            private ImageRender _imageRender;
            private TextRender _textRender;

            public Color Color = Color.Blue;
            public Color HoverColor = Color.Red;

            private Vector2 _clickposition;
            private Vector2 _releaseposition;
            public bool HasFocus;
            private bool _lastImmovable;

            public ResolutionTestEntity(EntityState stateref, string name)
                : base(stateref, name)
            {
                Body = new Body(this, "Body");
                Body.Bounds = new Vector2(70, 70);

                _physics = new Physics(this, "_physics", Body);
                _physics.Drag = .95f;

                Collision = new Collision(this, "Collision", new AABB(), Body, _physics);
                Collision.Mass = 10f;
                Collision.Restitution = .5f;

                _imageRender = new ImageRender(this, "Image", Assets.Pixel, Body);
                _imageRender.Scale = new Vector2(70, 70);
                _imageRender.Layer = .5f;

                _textBody = new Body(this, "_textBody");

                _textRender = new TextRender(this, "_textRender", _textBody);
                _textRender.Color = Color.White;
                _textRender.Font = Assets.Font;
                _textRender.Text = Name;
                _textRender.Layer = 1f;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                if (!HasFocus && Body.BoundingRect.Contains((int)MouseHandler.Cursor.Position.X,
                                                   (int)MouseHandler.Cursor.Position.Y))
                {
                    _imageRender.Color = HoverColor;
                    if (MouseHandler.IsMouseButtonPressed(MouseButton.LeftButton))
                    {
                        _clickposition = MouseHandler.Cursor.Position;
                        HasFocus = true;
                    }
                    if (MouseHandler.IsMouseButtonPressed(MouseButton.RightButton))
                    {
                        HasFocus = true;
                        _lastImmovable = Collision.Immovable;
                    }
                    _textRender.Text = Name + Environment.NewLine +
                                       "X:" + Math.Round(Body.X, 2) + Environment.NewLine +
                                       "Y" + Math.Round(Body.Y, 2);
                }
                else if (!HasFocus)
                {
                    //_clickposition = Vector2.Zero;
                    //_releaseposition = Vector2.Zero;
                    _imageRender.Color = Color;
                }

                if (HasFocus && MouseHandler.IsMouseButtonReleased(MouseButton.LeftButton))
                {
                    HasFocus = false;
                    _releaseposition = MouseHandler.Cursor.Position;

                    //Add delta to force.
                    _physics.AddForce((_clickposition - _releaseposition) / 2f);
                }
                else if (HasFocus && MouseHandler.IsMouseButtonDown(MouseButton.RightButton))
                {
                    Body.Position -= new Vector2(MouseHandler.Delta.X, MouseHandler.Delta.Y);
                    Collision.Immovable = true;
                }
                else if (HasFocus && MouseHandler.IsMouseButtonUp(MouseButton.LeftButton) && MouseHandler.IsMouseButtonUp(MouseButton.RightButton))
                {
                    HasFocus = false;
                    Collision.Immovable = _lastImmovable;
                }

                //Reset our position if it goes off screen.
                if (Body.Right < 0)
                    Body.X = EntityGame.Viewport.Width;
                else if (Body.Left > EntityGame.Viewport.Width)
                    Body.X = 0 - Body.Width;

                if (Body.Bottom < 0)
                    Body.Y = EntityGame.Viewport.Height;
                else if (Body.Top > EntityGame.Viewport.Height)
                    Body.Y = 0 - Body.Bounds.Y;

                _textBody.Position = Body.Position + Vector2.One * 3;
            }
        }
    }
}