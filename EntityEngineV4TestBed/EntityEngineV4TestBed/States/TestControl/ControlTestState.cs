using EntityEngineV4.GUI;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.TestControl
{
    public class ControlTestState : TestBedState
    {
        private TestControlManager _testControlManager;
        private Label _actionLabel;
        private string _actionLabelText = "ActionLabelText";

        //TODO: Write this to test more controls!
        public ControlTestState()
            : base("ControlTestState")
        {
        }

        public override void Create()
        {
            base.Create();
            //Add our services
            var controlHandler = new ControlHandler(this);
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    var testControl = new LinkLabel(controlHandler, "TC-X" + x + "Y" + y)
                        {
                            Body = { Position = new Vector2(x * 135 + 40, y * 30 + 50) },
                            TabPosition = new Point(x, y)
                        };
                    testControl.OnReleased += OnSelected;
                    //Set the default control.
                    if (x == 0 && y == 0)
                        testControl.OnFocusGain(testControl);
                    controlHandler.AddControl(testControl);
                }

            _actionLabel = new Label(controlHandler, "TestContolLabel");
            _actionLabel.Text = "TestControlLabel";
            _actionLabel.TabPosition = new Point(5, 2);
            _actionLabel.Body.Position = new Vector2(50, 400);
            controlHandler.AddControl(_actionLabel);

            _testControlManager = new TestControlManager(this, controlHandler);
            AddEntity(_testControlManager);
        }

        public override void Update(GameTime gt)
        {
            _actionLabel.Text = _actionLabelText;

            base.Update(gt);
        }

        public void OnSelected(Control c)
        {
            _actionLabelText = "Control " + c.Name + " was clicked!";
        }
    }
}