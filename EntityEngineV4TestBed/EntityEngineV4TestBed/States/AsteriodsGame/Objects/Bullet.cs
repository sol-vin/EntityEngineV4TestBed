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
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class Bullet : BaseEntity
    {
        public const float SPEED = 5f;
        public ImageRender Render;
        public Circle Shape;

        private bool _hasLeftPlayerCircle = false;

        public Bullet(IComponent parent, string name) : base(parent, name)
        {
            Render = new ImageRender(this, "Render");
            Render.Texture = Assets.Circle;
            Render.Layer = .1f;
            Render.Scale = new Vector2(.06f);
            Render.Color = Color.White;
            Render.Origin = new Vector2(Render.Texture.Width/2f, Render.Texture.Height/2f);
            Render.Link(ImageRender.DEPENDENCY_BODY, Body);

            //Make our collision rectangles the size of the rendered sprite.
            Body.Bounds = Render.Bounds;

            Shape = new Circle(this, "Shape", Body.Width/2);
            Shape.Offset = new Vector2(Body.Width/2, Body.Height/2);
            Shape.Debug = true;
            Shape.Link(Circle.DEPENDENCY_BODY, Body);

            Collision.GroupMask.AddMask(1);
            Collision.PairMask.AddMask(0);
            Collision.CollideEvent += OnCollide;
            Collision.Immovable = true;
            Collision.Link(Collision.DEPENDENCY_SHAPE, Shape);
            Shape.Link(Circle.DEPENDENCY_COLLISION, Collision);
        }

        public override void Initialize()
        {
            base.Initialize();
            Physics.Thrust(SPEED);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (!Collision.IsColliding) _hasLeftPlayerCircle = true;
        }

        public void OnCollide(Collision collision)
        {
            if (!_hasLeftPlayerCircle) return;

            Destroy();
        }
    }
}
