using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4TestBed.States.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States
{
    public abstract class TestBedState : State
    {
        private DoubleInput _backkey;

        protected TestBedState(string name)
            : base(name)
        {
            _backkey = new DoubleInput(this, "BackKey", Keys.Back, Buttons.Back, PlayerIndex.One);
        }

        public override void Initialize()
        {
            base.Initialize();
            new MouseService(this);

            EntityGame.BackgroundColor = Color.LightGray;
        }

        public override void PostUpdate()
        {
            base.PostUpdate();

            if (_backkey.Released())
            {
                (new MenuState()).Show(); 
            }
        }
    }
}