using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.CameraTest
{
    public class CameraTestState : TestBedState
    {
       
        private CameraTestEntity _cte;
        public CameraTestState(EntityGame eg) : base(eg, "CameraState")
        {
            Services.Add(new InputHandler(this));

            _cte = new CameraTestEntity(this, "CTE");
            AddEntity(_cte);
            
            AddEntity(new CameraEntity(this, "CE"));
        }

        private class CameraTestEntity : Entity
        {
            private DoubleInput _up, _down, _left, _right, _zoomIn, _zoomOut, _rotateLeft, _rotateRight;
            private Camera _camera = new Camera();

            public CameraTestEntity(EntityState stateref, string name) : base(stateref, name)
            {
                _up = new DoubleInput(this, "Up", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
                _down = new DoubleInput(this, "Down", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
                _left = new DoubleInput(this, "Left", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
                _right = new DoubleInput(this, "Right", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
                _zoomIn = new DoubleInput(this, "ZoomIn", Keys.W, Buttons.LeftShoulder, PlayerIndex.One);
                _zoomOut = new DoubleInput(this, "ZoomOut", Keys.S, Buttons.LeftTrigger, PlayerIndex.One);
                _rotateLeft = new DoubleInput(this, "RotateLeft", Keys.A, Buttons.RightShoulder, PlayerIndex.One);
                _rotateRight = new DoubleInput(this, "RotateRight", Keys.D, Buttons.RightTrigger, PlayerIndex.One);
                _camera.View();
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);
                if(_up.Down())
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

            public override void Destroy(IComponent i = null)
            {
                base.Destroy(i);
                Camera c = new Camera();
                c.View();
            }
        }

        private class CameraEntity : Entity
        {
            public Body Body;
            public ImageRender Image;

            public CameraEntity(EntityState stateref, string name) : base(stateref, name)
            {
                Body = new Body(this, "Body", new Vector2(EntityGame.Viewport.Width/2f, EntityGame.Viewport.Height/2f));
                Image = new ImageRender(this, "Image", Assets.Pixel, Body);
                Image.Scale = new Vector2(50, 100);
                Image.Color = Color.Red;
            }
        }
    }
}
