using EntityEngineV4.Collision;
using EntityEngineV4.Engine;
using EntityEngineV4TestBed.States.AsteriodsGame.Objects;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.AsteriodsGame
{
    public class AsteroidsGame : TestBedState
    {
        private PlayerShip _player;

        public AsteroidsGame() : base("AsteroidsGame")
        {
        }


        public override void Initialize()
        {
            base.Initialize();
            new CollisionHandler(this);
            //Change bgcolor to black
            EntityGame.BackgroundColor = Color.Black;
            EntityGame.DebugInfo.Render.Color = Color.White;
            _player = new PlayerShip(this, "PlayerShip");
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