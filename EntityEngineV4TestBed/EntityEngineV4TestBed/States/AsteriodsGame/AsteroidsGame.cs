using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4TestBed.States.AsteriodsGame.Objects;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.AsteriodsGame
{
    public class AsteroidsGame : TestBedState
    {
        public AsteroidsGame() : base("AsteroidsGame")
        {
            
        }



        public override void Initialize()
        {
            base.Initialize();

            //Change bgcolor to black
            EntityGame.BackgroundColor = Color.Black;
            EntityGame.DebugInfo.Render.Color = Color.White;
            new PlayerShip(this, "PlayerShip");
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public override void Destroy(IComponent i = null)
        {
            base.Destroy(i);
            EntityGame.BackgroundColor = Color.White;
            EntityGame.DebugInfo.Render.Color = Color.Black;
        }
    }
}
