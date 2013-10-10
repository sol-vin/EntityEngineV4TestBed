using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States
{
    public abstract class TestBedState : EntityState
    {
        protected TestBedState(string name)
            : base(name)
        {

        }

        public override void Create()
        {
            base.Create();
            new InputService(this);
            new MouseService(this);
            AddEntity(new TestBedStateManager(this, "TestBedStateManager"));
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}