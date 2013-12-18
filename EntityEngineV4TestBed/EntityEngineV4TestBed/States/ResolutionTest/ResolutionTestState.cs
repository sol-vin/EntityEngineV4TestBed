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
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.ResolutionTest
{
    public class ResolutionTestState : TestBedState
    {
        private SortedSet<string> _collided;
        private Label _collidedLabel;

        public ResolutionTestState()
            : base("ResolutionTestState")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            new ControlHandler(this);

            _collided = new SortedSet<string>();
            Page p = new Page(this, "Page");
            p.Show();
            _collidedLabel = new Label(p, "CollidedLabel", new Point(0, 0));
            _collidedLabel.Body.Position = new Vector2(10, 560);
            for (int x = 0; x < 3; x++)
            {
                ResolutionTestNode c = new ResolutionTestNode(this, "A" + x);
                c.Collision.Group.AddMask(0);
                c.Collision.Pair.AddMask(0);
                c.Collision.Pair.AddMask(2);
                c.Collision.ResolutionGroup.AddMask(0);
                c.Collision.ResolutionGroup.AddMask(1);
                c.Collision.ResolutionGroup.AddMask(2);
                c.Collision.CollideEvent +=
                    manifold =>
                        _collided.Add(manifold.A != c.Collision ? manifold.A.Parent.Name : manifold.B.Parent.Name);
                c.Collision.Debug = true;
                c.Body.Position = new Vector2(30, 80 * x + 20);
            }
            for (int x = 0; x < 3; x++)
            {
                ResolutionTestNode c = new ResolutionTestNode(this, "B" + x);
                c.Collision.Group.AddMask(1);
                c.Collision.Pair.AddMask(0);
                c.Collision.Pair.AddMask(2);
                c.Collision.ResolutionGroup.AddMask(0);
                c.Collision.CollideEvent += manifold => _collided.Add(manifold.A != c.Collision ? manifold.A.Parent.Name : manifold.B.Parent.Name);
                c.Collision.Debug = true;
                c.Body.Position = new Vector2(510, 80 * x + 20);
                c.Color = Color.Orange;
                c.HoverColor = Color.Violet;
            }

            for (int x = 0; x < 3; x++)
            {
                ResolutionTestNode c = new ResolutionTestNode(this, "C" + x);
                c.Collision.Group.AddMask(2);
                c.Collision.Pair.AddMask(0);
                c.Collision.Pair.AddMask(1);
                c.Collision.ResolutionGroup.AddMask(0);
                c.Collision.ResolutionGroup.AddMask(1);
                c.Collision.CollideEvent += manifold => _collided.Add(manifold.A != c.Collision ? manifold.A.Parent.Name : manifold.B.Parent.Name);
                c.Collision.Immovable = true;
                c.Collision.Debug = true;
                c.Body.Position = new Vector2(50 + x * 70, 450);
                c.Color = Color.Green;
                c.HoverColor = Color.DarkBlue;
            }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (Destroyed) return;

            string debug = GetRoot().GetChild<ResolutionTestNode>("A0").Collision.IsColliding.ToString();
            _collidedLabel.Text = "A0: " + debug;
        }

        private class ResolutionTestNode : Node
        {
            public Body Body;
            private Body _textBody;
            private Physics _physics;

            public Collision Collision;
            public AABB Shape;
            private ImageRender _imageRender;
            private TextRender _textRender;

            public Color Color = Color.Blue;
            public Color HoverColor = Color.Red;

            private Vector2 _clickposition;
            private Vector2 _releaseposition;
            public bool HasFocus;
            private bool _lastImmovable;

            public ResolutionTestNode(State stateref, string name)
                : base(stateref, name)
            {
                Body = new Body(this, "Body");
                Body.Bounds = new Vector2(70, 70);

                _physics = new Physics(this, "_physics");
                _physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);
                _physics.Mass = 10f;
                _physics.Restitution = 1f;
                _physics.Drag = .95f;

                Shape = new AABB(this, "AABB");
                Shape.LinkDependency(AABB.DEPENDENCY_BODY, Body);

                Collision = new Collision(this, "Collision");
                Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Shape);
                Collision.LinkDependency(Collision.DEPENDENCY_PHYSICS, _physics);
                Collision.Initialize();

                _imageRender = new ImageRender(this, "Image", Assets.Pixel);
                _imageRender.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);
                _imageRender.Scale = new Vector2(70, 70);
                _imageRender.Layer = .5f;

                _textBody = new Body(this, "_textBody");

                _textRender = new TextRender(this, "_textRender");
                _textRender.LinkDependency(TextRender.DEPENDENCY_BODY, _textBody);
                _textRender.Color = Color.White;
                _textRender.Font = Assets.Font;
                _textRender.Text = Name;
                _textRender.Layer = 1f;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                if (!HasFocus && Body.BoundingRect.Contains((int)MouseService.Cursor.Position.X,
                                                   (int)MouseService.Cursor.Position.Y))
                {
                    _imageRender.Color = HoverColor;
                    if (MouseService.IsMouseButtonPressed(MouseButton.LeftButton))
                    {
                        _clickposition = MouseService.Cursor.Position;
                        HasFocus = true;
                    }
                    if (MouseService.IsMouseButtonPressed(MouseButton.RightButton))
                    {
                        HasFocus = true;
                        _lastImmovable = Collision.Immovable;
                    }
                }
                else if (!HasFocus)
                {
                    //_clickposition = Vector2.Zero;
                    //_releaseposition = Vector2.Zero;
                    _imageRender.Color = Color;
                }

                if (HasFocus && MouseService.IsMouseButtonReleased(MouseButton.LeftButton))
                {
                    HasFocus = false;
                    _releaseposition = MouseService.Cursor.Position;

                    //Add delta to force.
                    _physics.AddForce((_clickposition - _releaseposition) / 2f);
                }
                else if (HasFocus && MouseService.IsMouseButtonDown(MouseButton.RightButton))
                {
                    Body.Position -= new Vector2(MouseService.Delta.X, MouseService.Delta.Y);
                    Collision.Immovable = true;
                }
                else if (HasFocus && MouseService.IsMouseButtonUp(MouseButton.LeftButton) && MouseService.IsMouseButtonUp(MouseButton.RightButton))
                {
                    HasFocus = false;
                    Collision.Immovable = _lastImmovable;
                }

                //ResetTimer our position if it goes off screen.
                if (Body.Right < 0)
                    Body.X = EntityGame.Viewport.Width;
                else if (Body.Left > EntityGame.Viewport.Width)
                    Body.X = 0 - Body.Width;

                if (Body.Bottom < 0)
                    Body.Y = EntityGame.Viewport.Height;
                else if (Body.Top > EntityGame.Viewport.Height)
                    Body.Y = 0 - Body.Bounds.Y;

                _textRender.Text = Name + Environment.NewLine +
                                       "X:" + Math.Round(Body.X, 2) + Environment.NewLine +
                                       "Y" + Math.Round(Body.Y, 2);
                _textBody.Position = Body.Position + Vector2.One * 3;
            }
        }
    }
}