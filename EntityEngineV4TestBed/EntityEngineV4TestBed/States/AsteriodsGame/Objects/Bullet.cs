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
using EntityEngineV4.Engine.Services;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class Bullet : BaseEntity
    {
        public ImageRender Render;
        public Circle Shape;

        private bool _hasLeftPlayerCircle = false;

        public Bullet(Node parent, string name) : base(parent, name)
        {
            Render = new ImageRender(this, "Render");
            Render.SetTexture(GetState<State>().GetService<AssetCollector>().GetAsset<Texture2D>("bullet"));
            Render.Layer = .1f;
            Render.Scale = new Vector2(.1f);
            Render.Color = Color.White;
            Render.Origin = new Vector2(Render.Texture.Width/2f, Render.Texture.Height/2f);
            Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);

            //Make our collision rectangles the size of the rendered sprite.
            Body.Bounds = Render.Bounds;

            Shape = new Circle(this, "Shape", Body.Width/2);
            Shape.Offset = new Vector2(Body.Width/2, Body.Height/2);
            Shape.Debug = true;
            Shape.LinkDependency(Circle.DEPENDENCY_BODY, Body);

            Collision.GroupMask.AddMask(1);
            Collision.PairMask.AddMask(0);
            Collision.CollideEvent += OnCollide;
            Collision.Immovable = true;
            Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, Shape);
            Shape.LinkDependency(Circle.DEPENDENCY_COLLISION, Collision);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (!Collision.IsColliding) _hasLeftPlayerCircle = true;
        }

        private void OnCollide(Collision collision)
        {
            if (!_hasLeftPlayerCircle) return;

            Destroy();
        }
    }
}
