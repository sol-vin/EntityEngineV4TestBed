using EntityEngineV4.Engine;
using EntityEngineV4.Input;

namespace EntityEngineV4TestBed.States.SuperTownDefence
{
    public class STDGameState : EntityState
    {
        public STDGameState(EntityGame eg) : base(eg, "STDGameState")
        {
            Services.Add(new InputHandler(this));
            AddEntity(new STDGameStateManager(this));
        }
    }
}
