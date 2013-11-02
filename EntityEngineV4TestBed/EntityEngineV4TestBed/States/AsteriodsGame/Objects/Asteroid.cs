using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.AsteriodsGame.Objects
{
    public class Asteroid : BaseEntity
    {
        public ImageRender Render;

        public Asteroid(Node parent, string name) : base(parent, name)
        {
            Body = new Body(this, "Body");
            Body.X = RandomHelper.GetFloat() * EntityGame.Viewport.Right;
            Body.Y = RandomHelper.GetFloat() * EntityGame.Viewport.Bottom;

            Physics = new Physics(this, "Physics");
            Physics.LinkDependency(Physics.DEPENDENCY_BODY, Body);

            Render = new ImageRender(this, "Render");
            Render.LoadTexture(@"AsteroidsGame/circle");
            Render.Scale = new Vector2(0.01f);
            Render.Origin = new Vector2(Render.Texture.Width / 2f, Render.Texture.Height / 2f);
            Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);
            Body.Width = Render.DrawRect.Width;
            Body.Height = Render.DrawRect.Height;
        }
    }
}
