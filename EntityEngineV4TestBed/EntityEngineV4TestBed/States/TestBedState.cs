using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States
{
    public abstract class TestBedState : State
    {
        protected TestBedState(string name)
            : base(name)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            new InputService(this);
            new MouseService(this);
            new TestBedStateManager(this, "TestBedStateManager");

            EntityGame.BackgroundColor = Color.LightGray;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}