﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Input.MouseInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.ColorTest
{
    public class ColorTestState : TestBedState
    {
        private Point _size = new Point(30, 30);
        private int sizechange = 5;

        private ColorTestManager _ctm;

        public ColorTestState(EntityGame eg) : base(eg, "ColorTestState")
        {
            Services.Add(new InputHandler(this));
            Services.Add(new MouseHandler(this));

            RepopulateEntities();
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if(_ctm.UpKey.Released()) IncreaseY();
            else if (_ctm.DownKey.Released()) DecreaseY();

            if(_ctm.RightKey.Released()) IncreaseX();
            else if(_ctm.LeftKey.Released()) DecreaseX();
        }

        public void IncreaseX()
        {
            _size.X += sizechange;
            _size.X = (int)MathHelper.Clamp(_size.X, 5, EntityGame.Viewport.Width);
            RepopulateEntities();
        }

        public void DecreaseX()
        {
            _size.X -= sizechange;
            _size.X = (int)MathHelper.Clamp(_size.X, 5, EntityGame.Viewport.Width);
            RepopulateEntities();
        }

        public void IncreaseY()
        {
            _size.Y += sizechange;
            _size.Y = (int)MathHelper.Clamp(_size.Y, 5, EntityGame.Viewport.Height);
            RepopulateEntities();
        }

        public void DecreaseY()
        {
            _size.Y -= sizechange;
            _size.Y = (int)MathHelper.Clamp(_size.Y, 5, EntityGame.Viewport.Height);
            RepopulateEntities();
        }


        public void RepopulateEntities()
        {
            Clear();
            _ctm = new ColorTestManager(this, "ColorTestManager");
            AddEntity(_ctm);

            AddEntity(new TestBedStateManager(this, "TestBedManager"));

            int maxx = EntityGame.Viewport.Width / _size.X + 1;
            int maxy = EntityGame.Viewport.Height / _size.Y + 1;
            float huefraction = 1f / (maxx * maxy);

            for (int x = 0; x < maxx; x++)
            {
                for (int y = 0; y < maxy; y++)
                {
                    ColorTestEntity cte = new ColorTestEntity(this, "ColorTestEntity" + x + "-" + y);
                    cte.Body.Position = new Vector2(x * _size.X, y * _size.Y);
                    cte.Body.Bounds = new Vector2(_size.X, _size.Y);
                    cte.Render.Scale = new Vector2(_size.X, _size.Y);
                    cte.Hue = huefraction * ((y * maxx) + x);
                    AddEntity(cte);
                }
            }
        }

        private class ColorTestManager : Entity
        {
            public DoubleInput UpKey, DownKey, LeftKey, RightKey;

            public ColorTestManager(EntityState stateref, string name) : base(stateref, name)
            {
                UpKey = new DoubleInput(this, "Up", Keys.Up, Buttons.DPadUp, PlayerIndex.One);
                DownKey = new DoubleInput(this, "Down", Keys.Down, Buttons.DPadDown, PlayerIndex.One);
                LeftKey = new DoubleInput(this, "Left", Keys.Left, Buttons.DPadLeft, PlayerIndex.One);
                RightKey = new DoubleInput(this, "Right", Keys.Right, Buttons.DPadRight, PlayerIndex.One);
            }
        }

        private class ColorTestEntity : Entity
        {
            public Body Body;
            public ImageRender Render;
            public Timer ColorIncreaseTimer;

            public float Hue = 0f;
            private const float HUEINCREASE = .01f;
            public ColorTestEntity(EntityState stateref, string name) : base(stateref, name)
            {
                Body = new Body(this, "Body");

                Render = new ImageRender(this, "Render", Assets.Pixel, Body);
                Render.Color = Color.Red;
                
                ColorIncreaseTimer = new Timer(this, "ColorIncreaseTimer");
                ColorIncreaseTimer.Milliseconds = 100;
                ColorIncreaseTimer.LastEvent += IncreaseHue;
                ColorIncreaseTimer.Start();
            }

            private void IncreaseHue()
            {
                Hue += HUEINCREASE;
                if (Hue >= 1)
                    Hue = 0;

                Render.Color = ColorMath.TestHSVtoRGB(Hue, 1, 1, 1);
            }
        }
    }
}