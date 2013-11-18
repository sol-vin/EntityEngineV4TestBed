using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Collision;
using EntityEngineV4.Collision.Shapes;
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

            Physics.Velocity = new Vector2(RandomHelper.GetFloat(-.5f, .5f), RandomHelper.GetFloat(-.5f, .5f));

            Render = new ImageRender(this, "Render");
            Render.SetTexture(GetRoot<State>().GetService<AssetCollector>().GetAsset<Texture2D>("circle"));
            Render.Scale = new Vector2(RandomHelper.GetFloat(.25f, .5f));
            Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);

            Body.Width = Render.DrawRect.Width;
            Body.Height = Render.DrawRect.Height;
            Body.Origin = new Vector2(Render.Texture.Width / 2f, Render.Texture.Height / 2f);

            Health = new Health(this, "Health", 3);
            Health.DiedEvent += entity => Destroy(this);

            Shape = new Circle(this, "Circle", Body.Width/2);
            Shape.Offset = new Vector2(Body.Width / 2, Body.Height / 2);
            Shape.Debug = true;
            Shape.LinkDependency(Circle.DEPENDENCY_BODY, Body);

            Collision.GroupMask.AddMask(2);
            Collision.PairMask.AddMask(0);
            Collision.PairMask.AddMask(1);
            Collision.CollideEvent += collision => Health.Hurt(1);
            Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Shape);
            Shape.LinkDependency(Circle.DEPENDENCY_COLLISION, Collision);
        }
    }
}
