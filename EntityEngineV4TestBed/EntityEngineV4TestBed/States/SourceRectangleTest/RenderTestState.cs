using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.SourceRectangleTest
{
    public class RenderTestState : TestBedState
    {
        //TODO: Finish writing test
        private Entity _animation;
        public RenderTestState(EntityGame eg) : base(eg, "RenderTestState")
        {
            Services.Add(new InputHandler(this));


            _animation = new Entity(this,  "Animation");

            Body body = new Body(_animation, "Body");
            body.Position = new Vector2(50,50);
            SourceAnimation source = new SourceAnimation(_animation, "Source", EntityGame.Game.Content.Load<Texture2D>(@"SourceAnimationTest/scott"), new Vector2(36,59), 8, body );
            source.Scale = Vector2.One;
            source.ReadXml(@"States\SourceRectangleTest\sourcerectangles.xml");
            source.Start();
            AddEntity(_animation);
        }
    }
}
