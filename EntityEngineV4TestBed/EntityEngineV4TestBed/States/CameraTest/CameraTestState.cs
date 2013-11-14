using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.CameraTest
{
    public class CameraTestState : TestBedState
    {
        private CameraTestNode _cte;

        public CameraTestState()
            : base("CameraState")
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            _cte = new CameraTestNode(this, "CTE");

            new CameraNode(this, "CE");

            //add a label to track screen space
            var ch = new ControlHandler(this);

            //TODO: Add labels to find camera screen space and values
            var label = new Label(ch, "CamLabel");

        }

        private class CameraTestNode : Node
        {
            private DoubleInput _up, _down, _left, _right, _zoomIn, _zoomOut, _rotateLeft, _rotateRight;
            private Camera _camera;

            public CameraTestNode(State stateref, string name)
                : base(stateref, name)
            {
                _up = new DoubleInput(this, "Up", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
                _down = new DoubleInput(this, "Down", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
                _left = new DoubleInput(this, "Left", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
                _right = new DoubleInput(this, "Right", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
                _zoomIn = new DoubleInput(this, "ZoomIn", Keys.W, Buttons.LeftShoulder, PlayerIndex.One);
                _zoomOut = new DoubleInput(this, "ZoomOut", Keys.S, Buttons.LeftTrigger, PlayerIndex.One);
                _rotateLeft = new DoubleInput(this, "RotateLeft", Keys.A, Buttons.RightShoulder, PlayerIndex.One);
                _rotateRight = new DoubleInput(this, "RotateRight", Keys.D, Buttons.RightTrigger, PlayerIndex.One);

                _camera = new Camera(this, "Camera");
                _camera.View();
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                if (_up.Down())
                    _camera.Position.Y -= 1f;
                else if (_down.Down())
                    _camera.Position.Y += 1f;
                if (_left.Down())
                    _camera.Position.X -= 1f;
                else if (_right.Down())
                    _camera.Position.X += 1f;
                if (_zoomIn.Down())
                    _camera.Zoom += .1f;
                else if (_zoomOut.Down())
                    _camera.Zoom -= .1f;
            }

            public override void Destroy(IComponent sender = null)
            {
                base.Destroy(sender);
                Camera c = new Camera(this, "DefaultCamera");
                c.View();
            }
        }

        private class CameraNode : Node
        {
            public Body Body;
            public ImageRender Image;

            public CameraNode(State stateref, string name)
                : base(stateref, name)
            {
                Body = new Body(this, "Body", new Vector2(EntityGame.Viewport.Width / 2f, EntityGame.Viewport.Height / 2f));
                Image = new ImageRender(this, "Image", Assets.Pixel);
                Image.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);
                Image.Scale = new Vector2(50, 100);
                Image.Color = Color.Red;
            }
        }
    }
}