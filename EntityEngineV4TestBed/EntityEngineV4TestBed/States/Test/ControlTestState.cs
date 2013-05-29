using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.Test
{
    public class ControlTestState : TestBedState
    {
        private TestControlManager _testControlManager;

        public ControlTestState(EntityGame parent)
            : base(parent, "ControlTestState")
        {
            Initialize();
        }

        public void Initialize()
        {
            //Add our services
            Services.Add(new InputHandler(this));
            Services.Add(new MouseHandler(this, true));
            var ch = new ControlHandler(this);
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    var tc = new TestControl(this, "TC-X" + x + "Y" + y)
                        {
                            Body = { Position = new Vector2(x * 200 + 50, y * 30 + 50) },
                            TabPosition = new Point(x, y)
                        };
                    //Set the default control.
                    if (x == 0 && y == 0)
                        tc.OnFocusGain(tc);
                    ch.AddControl(tc);
                }
            Services.Add(ch);

            _testControlManager = new TestControlManager(this, ch);
            AddEntity(_testControlManager);
        }

        public override void Update(GameTime gt)
        {
            if (_testControlManager.Reset)
            {
                Reset();
                Initialize();
            }
            base.Update(gt);
        }
    }
}