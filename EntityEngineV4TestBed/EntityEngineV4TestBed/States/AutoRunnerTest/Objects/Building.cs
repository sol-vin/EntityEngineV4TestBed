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
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.AutoRunnerTest.Objects
{
    public class Building : Entity
    {
        public Body Body;
        public Physics Physics;
        public ImageRender Image;
        public Collision Collision;

        public Building(EntityState stateref, string name) : base(stateref, name)
        {
            Random rand = new Random(DateTime.Now.Millisecond^DateTime.Now.Second*Id);
            Body = new Body(this, "Body");
            Body.Position = new Vector2(0, 550);
            Body.Bounds = new Vector2(3000, 100);

            Physics = new Physics(this, "Physics", Body);

            Image = new ImageRender(this, "Image", Assets.Pixel, Body);
            Image.Scale = Body.Bounds;
            Image.Color = Color.CornflowerBlue;

            Collision = new Collision(this, "Collision", new AABB(), Body, Physics);
            Collision.GroupMask.AddMask(0);
            Collision.PairMask.AddMask(0);
            Collision.Debug = true;
        }
    }
}
