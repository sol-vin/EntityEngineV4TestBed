using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.SourceRectangleTest
{
    public class RenderTestState : TestBedState
    {
        //TODO: Finish writing test
        private AnimationTestEntity _animation;

        public RenderTestState(EntityGame eg) : base(eg, "RenderTestState")
        {
            _animation = new AnimationTestEntity(this, "Animation");
            AddEntity(_animation);

        }

        private class AnimationTestEntity : Entity
        {
            public Body Body;
            public SourceAnimation StandingAnim;

            public AnimationTestEntity(EntityState stateref, string name) : base(stateref, name)
            {
                Body = new Body(this, "Body");
                Body.Position = new Vector2(100,400);
                StandingAnim = new SourceAnimation(this, "StandingAnim", EntityGame.Game.Content.Load<Texture2D>(@"SourceAnimationTest/scott"), new Vector2(36, 59), 8, Body);
                StandingAnim.Scale = Vector2.One;
                StandingAnim.ReadXml(@"States\SourceRectangleTest\standing.xml");
                StandingAnim.Start();
            }
        }
    }
}
