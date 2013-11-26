using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.CollisionEngine;
using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Services;
using EntityEngineV4.GUI;
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

        public Asteroid(Node parent, string name) : base(parent, name)
        {
            Body.X = RandomHelper.GetFloat() * EntityGame.Viewport.Right;
            Body.Y = RandomHelper.GetFloat() * EntityGame.Viewport.Bottom;
            Body.Angle = MathHelper.TwoPi*RandomHelper.GetFloat();

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

            Shape = new Circle(this, "Circle", Body.Width/2);
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
        }

        public void OnCollide(Manifold m)
        {
            Node otherNode = m.A != Collision ? m.A : m.B;
            if (otherNode.Parent.GetType() == typeof(Bullet) || otherNode.Parent.GetType() == typeof(PlayerShip))
            {
                Health.Hurt(1);
            }
        }
    }
}
