using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;

namespace EntityEngineV4TestBed.States.TowerDefence
{
    public class TowerDefenceMenu : State
    {
        //GUI
        private Page ManuContainer;
        private LinkLabel _startLink;
        private LinkLabel _optionsLink;
        private LinkLabel _exitLink;
        private Label _titleText;
        
        public TowerDefenceMenu() : base("TowerDefenceMenu")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
