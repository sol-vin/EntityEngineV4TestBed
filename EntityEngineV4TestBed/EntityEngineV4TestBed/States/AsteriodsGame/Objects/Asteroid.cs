using EntityEngineV4.CollisionEngine;
using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Services;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class Asteroid : BaseNode
    {
        public ImageRender Render;
        public Circle Shape;
        public Health Health;
        private AsteroidGhoster _ghoster;

        public Asteroid(Node parent, string name)
            : base(parent, name)
        {
            Body.X = RandomHelper.GetFloat() * EntityGame.Viewport.Right;
            Body.Y = RandomHelper.GetFloat() * EntityGame.Viewport.Bottom;
            Body.Angle = MathHelper.TwoPi * RandomHelper.GetFloat();

            Physics.Thrust(RandomHelper.GetFloat(30, 60));
            Physics.Restitution = 1.5f;

            Render = new ImageRender(this, "Render");
            Render.SetTexture(GetRoot<State>().GetService<AssetCollector>().GetAsset<Texture2D>("circle"));
            Render.Scale = new Vector2(RandomHelper.GetFloat(.25f, .5f));
            Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);

            Body.Width = Render.DrawRect.Width;
            Body.Height = Render.DrawRect.Height;
            Body.Origin = new Vector2(Render.Texture.Width / 2f, Render.Texture.Height / 2f);

            Physics.Mass = Render.Scale.X;

            Health = new Health(this, "Health", 3);
            Health.DiedEvent += entity => Destroy(this);

            Shape = new Circle(this, "Circle", Body.Width / 2);
            Shape.Offset = new Vector2(Body.Width / 2, Body.Height / 2);
            Shape.Debug = true;
            Shape.LinkDependency(Circle.DEPENDENCY_BODY, Body);

            Collision.Pair.AddMask(0);
            Collision.Pair.AddMask(1);
            Collision.Group.AddMask(2);
            Collision.Pair.AddMask(2);
            Collision.ResolutionGroup.AddMask(2);
            Collision.CollideEvent += OnCollide;
            Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Shape);
            Shape.LinkDependency(Circle.DEPENDENCY_COLLISION, Collision);

            _ghoster = new AsteroidGhoster(this, "Ghoster");
            _ghoster.LinkDependency(AsteroidGhoster.DEPENDENCY_BODY, Body);
            _ghoster.LinkDependency(AsteroidGhoster.DEPENDENCY_RENDER, Render);
            _ghoster.LinkDependency(AsteroidGhoster.DEPENDENCY_COLLISION, Collision);
            _ghoster.Initialize();
            _ghoster.SetOnCollide(OnCollide);
        }

        public void OnCollide(Manifold m)
        {
            //Find which collider is another node.
            Node otherNode; //TODO:ASTEROIDS COLLIDE WITH THEIR OWN GHOSTS!

            if (m.A == Collision || m.B == Collision) //If we are one of the collision pairings
            {
                otherNode = m.A != Collision ? m.A : m.B; //Find which one is the other node.
            }
            else
            {
                //TODO: Asteroids do not resolve with ghosts.
                //Oncollide was called from a ghost, find which one isn't the ghost
                otherNode = m.A.Parent.GetType() == typeof(AsteroidGhost) ? m.A : m.B;
                Asteroid a = (otherNode.Parent as AsteroidGhost).GetOriginal();
                //Use a special resolver to resove the collision
                Manifold specialresolver = new Manifold(Collision, a.Collision);
                CollisionHandler.ResolveCollision(specialresolver);
                CollisionHandler.PositionalCorrection(specialresolver);
            }

            if (otherNode.Parent.GetType() == typeof(Bullet) || otherNode.Parent.GetType() == typeof(PlayerShip))
            {
                Health.Hurt(1);
            }
        }

        private class AsteroidGhoster : Component
        {
            private AsteroidGhost _xGhost, _yGhost, _cornerGhost;

            public AsteroidGhoster(Node parent, string name)
                : base(parent, name)
            {
            }

            public override void Initialize()
            {
                base.Initialize();

                _xGhost = new AsteroidGhost(this, "XGhost");
                _xGhost.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position;
                _xGhost.Render.Scale = GetDependency<Render>(DEPENDENCY_RENDER).Scale; //Scale is set dymanically by asteroid
                _xGhost.Collision.Exclusions.Add(GetDependency<Collision>(DEPENDENCY_COLLISION)); //Add an exclusion to prevent the ghost from colliding with its original.
                _xGhost.Initialize();

                _yGhost = new AsteroidGhost(this, "YGhost");
                _yGhost.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position;
                _yGhost.Render.Scale = GetDependency<Render>(DEPENDENCY_RENDER).Scale;
                _yGhost.Collision.Exclusions.Add(GetDependency<Collision>(DEPENDENCY_COLLISION));
                _yGhost.Initialize();

                //_cornerGhost = new AsteroidGhost(this, "CornerGhost");
                //_cornerGhost.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position;
                //_cornerGhost.Render.Scale = GetDependency<Render>(DEPENDENCY_RENDER).Scale;
                //_cornerGhost.Collision.Exclusions.Add(GetDependency<Collision>(DEPENDENCY_COLLISION));
                //_cornerGhost.Initialize();
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                UpdateGhosts();
            }

            public void UpdateGhosts()
            {
                _xGhost.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position;
                _yGhost.Body.Position = GetDependency<Body>(DEPENDENCY_BODY).Position;

                //First ensure the real asteroid hasn't gone completely off screen
                if (!GetDependency<Body>(DEPENDENCY_BODY).BoundingRect.Intersects(EntityGame.Viewport))
                {
                    //It did, reset it to the other side.
                    if (GetDependency<Body>(DEPENDENCY_BODY).Right < 0)
                        GetDependency<Body>(DEPENDENCY_BODY).X += EntityGame.Viewport.Width;
                    else if (GetDependency<Body>(DEPENDENCY_BODY).Left > EntityGame.Viewport.Width)
                        GetDependency<Body>(DEPENDENCY_BODY).X -= EntityGame.Viewport.Width;

                    //It did, reset it to the other side.
                    if (GetDependency<Body>(DEPENDENCY_BODY).Top < 0)
                        GetDependency<Body>(DEPENDENCY_BODY).Y += EntityGame.Viewport.Height;
                    else if (GetDependency<Body>(DEPENDENCY_BODY).Bottom > EntityGame.Viewport.Height)
                        GetDependency<Body>(DEPENDENCY_BODY).Y -= EntityGame.Viewport.Height;
                }
                else //If we are colliding, ghost that shit.
                {
                    //Check if original is inside the viewport
                    Rectangle box = GetDependency<Body>(DEPENDENCY_BODY).BoundingRect;
                    if (box.Left > 0 && box.Right < EntityGame.Viewport.Width && box.Top > 0 &&
                        box.Bottom < EntityGame.Viewport.Height)
                    {
                        //It is, deactivate ghosts
                        _xGhost.Active = false;
                        _yGhost.Active = false;
                        _xGhost.Visible = false;
                        _yGhost.Visible = false;
                    }
                    else
                    {
                        //It isnt, activate ghosts
                        _xGhost.Active = true;
                        _yGhost.Active = true;
                        _xGhost.Visible = true;
                        _yGhost.Visible = true;

                        //Orient XGhost
                        if (GetDependency<Body>(DEPENDENCY_BODY).Left < 0)
                        //If the left side of the real asteroids out of bounds to the left
                        {
                            _xGhost.Body.Position.X = GetDependency<Body>(DEPENDENCY_BODY).X + EntityGame.Viewport.Width;
                            //Move ghost to opposite side.
                        }
                        else if (GetDependency<Body>(DEPENDENCY_BODY).Right > EntityGame.Viewport.Width)
                        //If the right side is out of bounds to the right
                        {
                            _xGhost.Body.Position.X = GetDependency<Body>(DEPENDENCY_BODY).X - EntityGame.Viewport.Width;
                            //Move ghost to opposite side
                        }

                        //Orient YGhost
                        if (GetDependency<Body>(DEPENDENCY_BODY).Top < 0)
                        //If the top side of the real asteroids out of bounds upwards
                        {
                            _yGhost.Body.Position.Y = GetDependency<Body>(DEPENDENCY_BODY).Y
                                                      + EntityGame.Viewport.Height; //Move ghost to opposite side.
                        }
                        else if (GetDependency<Body>(DEPENDENCY_BODY).Bottom > EntityGame.Viewport.Height)
                        //If the bottom is out of bounds downwards
                        {
                            _yGhost.Body.Position.Y = GetDependency<Body>(DEPENDENCY_BODY).Y -
                                                      EntityGame.Viewport.Height; //Move ghost to opposite side
                        }
                    }
                }
            }

            public Asteroid GetOriginal()
            {
                return (Asteroid)GetDependency<Body>(DEPENDENCY_BODY).Parent;
            }

            public void SetOnCollide(Collision.CollisionEventHandler method)
            {
                _xGhost.Collision.CollideEvent += method;
                _yGhost.Collision.CollideEvent += method;
                //_cornerGhost.Collision.CollideEvent += method;
            }

            //Dependencies
            public const int DEPENDENCY_BODY = 0;

            public const int DEPENDENCY_RENDER = 1;
            public const int DEPENDENCY_COLLISION = 2;

            public override void CreateDependencyList()
            {
                base.CreateDependencyList();
                AddLinkType(DEPENDENCY_BODY, typeof(Body));
                AddLinkType(DEPENDENCY_RENDER, typeof(Render));
                AddLinkType(DEPENDENCY_COLLISION, typeof(Collision));
            }
        }

        private class AsteroidGhost : Node
        {
            public Body Body;
            public Circle Shape;
            public ImageRender Render;
            public Collision Collision;

            public AsteroidGhost(Node parent, string name)
                : base(parent, name)
            {
                Body = new Body(this, "Body");

                Render = new ImageRender(this, "Render");
                Render.SetTexture(GetRoot<State>().GetService<AssetCollector>().GetAsset<Texture2D>("circle"));
                Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);

                Collision = new Collision(this, "Collision");
                Collision.Pair.AddMask(0);
                Collision.Pair.AddMask(1);
                Collision.Group.AddMask(2);
                Collision.Pair.AddMask(2);
            }

            public override void Initialize()
            {
                base.Initialize();
                Body.Width = Render.DrawRect.Width;
                Body.Height = Render.DrawRect.Height;
                Body.Origin = new Vector2(Render.Texture.Width / 2f, Render.Texture.Height / 2f);

                Shape = new Circle(this, "Circle", Body.Width / 2);
                Shape.Offset = new Vector2(Body.Width / 2, Body.Height / 2);
                Shape.Debug = true;
                Shape.LinkDependency(Circle.DEPENDENCY_BODY, Body);

                Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Shape);
                Shape.LinkDependency(Circle.DEPENDENCY_COLLISION, Collision);
            }

            public Asteroid GetOriginal()
            {
                return (Asteroid)Parent.Parent;
            }
        }
    }
}