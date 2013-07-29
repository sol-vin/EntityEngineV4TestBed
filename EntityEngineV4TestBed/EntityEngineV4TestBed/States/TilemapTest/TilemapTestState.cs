using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.TilemapTest
{
    public class TilemapTestState : TestBedState
    {
        Random _rand = new Random();
        private Tilemap _tm;
        public TilemapTestState(EntityGame eg) : base(eg, "TilemapTest")
        {
            _tm = new Tilemap(this, "Tilemap", EntityGame.Game.Content.Load<Texture2D>(@"TilemapTest/tiles"), MakeTiles(30,30), new Point(16,16));
            _tm.Data.Scale = new Vector2(1.5f,1.5f);
            AddEntity(_tm);
            AddEntity( new CameraController(this, "CamCon"));
        }

        public Tile[,] MakeTiles(int sizex, int sizey)
        {
            Tile[,] tiles = new Tile[sizex,sizey];
            for (int x = 0; x < sizex; x++)
            {
                for (int y = 0; y < sizey; y++)
                {
                    tiles[x,y] = new Tile((short)_rand.Next(0, 3));
                }
            }
            return tiles;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
        private class CameraController : Entity
        {
            private DoubleInput _up, _down, _left, _right, _zoomIn, _zoomOut, _rotateLeft, _rotateRight;
            private Camera _camera;

            public CameraController(EntityState stateref, string name)
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

            public override void Destroy(IComponent i = null)
            {
                base.Destroy(i);
                Camera c = new Camera(this, "Camera");
                c.View();
            }
        }

    }
}
