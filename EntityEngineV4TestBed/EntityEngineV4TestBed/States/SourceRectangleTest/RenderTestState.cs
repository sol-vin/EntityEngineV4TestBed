using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.SourceRectangleTest
{
    public class RenderTestState : TestBedState
    {
        //TODO: Finish writing test

        public RenderTestState()
            : base("RenderTestState")
        {
           
        }

        public override void Create()
        {
            base.Create();
            new AnimationTestEntity(this, "Animation");
        }

        private class AnimationTestEntity : Entity
        {
            public Body Body;
            public SourceAnimation StandingAnim;

            public AnimationTestEntity(EntityState stateref, string name)
                : base(stateref, name)
            {
                Body = new Body(this, "Body");
                Body.Position = new Vector2(100, 400);
                StandingAnim = new SourceAnimation(this, "StandingAnim", EntityGame.Game.Content.Load<Texture2D>(@"SourceAnimationTest/scott"), new Vector2(36, 59), 8);
                StandingAnim.Link(SourceAnimation.DEPENDENCY_BODY, Body);
                StandingAnim.Scale = Vector2.One;
                StandingAnim.ReadXml(@"States\SourceRectangleTest\standing.xml");
                StandingAnim.Start();
                StandingAnim.Debug = true;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                Body.Angle += .05f;
            }
        }
    }
}