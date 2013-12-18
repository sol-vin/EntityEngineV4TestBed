#region Using Statements

using EntityEngineV4.Engine;
using EntityEngineV4TestBed.States.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion Using Statements

namespace EntityEngineV4TestBed
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : EntityGame
    {

        public Game1(Rectangle viewPort)
            : base(viewPort)
        {
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.IsMouseVisible = false;

            (new MenuState()).Show();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Emergency kill key
            if (Keyboard.GetState().IsKeyDown(Keys.Pause))
            {
                Exit();
            }
            base.Update(gameTime);
        }
    }
}