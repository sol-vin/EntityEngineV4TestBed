using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.ColorTest
{
    public class ColorTestState : TestBedState
    {
        private Point _size = new Point(30, 30);
        private HSVTilemap _hsv;
        private XYZTilemap _xyz;
        private FadeOutLabel _label;
        private ColorTestManager _ctm;

        private const float HUEINCREASE = 0.001f;
        public ColorTestState()
            : base("ColorTestState")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            int maxx = EntityGame.Viewport.Width / _size.X + 1;
            int maxy = EntityGame.Viewport.Height / _size.Y + 1;

            _hsv = new HSVTilemap(this, "tiles", new Point(maxx, maxy));
            _hsv.Render.Scale = new Vector2(_size.X, _size.Y);

            _xyz = new XYZTilemap(this, "tiles", new Point(maxx, maxy));
            _xyz.Render.Scale = new Vector2(_size.X, _size.Y);
            _xyz.Active = false;
            _xyz.Visible = false;

            _label = new FadeOutLabel(this, "FadeOutLabel");
            _label.Text = "HSV Test";

            _ctm = new ColorTestManager(this,"CTM");
        }

        public void NextTest()
        {
            //find the active test, then make it invisible and not active, enable the next test
            if (_hsv.Active)
            {
                //Move to XYZ
                _hsv.Active = false;
                _hsv.Visible = false;
                _xyz.Active = true;
                _xyz.Visible = true;
                _label.Text = "XYZ Test";
                _label.Reset();
                return;
            }

            if (_xyz.Active)
            {
                //Move to hsv
                _hsv.Active = true;
                _hsv.Visible = true;
                _xyz.Active = false;
                _xyz.Visible = false;
                _label.Text = "HSV Test";
                _label.Reset();
                return;
            }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (_ctm.NextTestKey.Released())
            {
                NextTest();
            }
        }

        private class ColorTestManager : Entity
        {
            public DoubleInput NextTestKey;

            public ColorTestManager(IComponent parent, string name) : base(parent, name)
            {
                NextTestKey =  new DoubleInput(this, "NextTestKey", Keys.Space, Buttons.A, PlayerIndex.One);
            }
        }

        private class HSVTilemap : Tilemap
        {
            public HSVTilemap(IComponent parent, string name, Point size)
                : base(parent, name, Assets.Pixel, size, new Point(1, 1))
            {
                float huefraction = 1f/(size.X*size.Y);

                for (int x = 0; x < size.X; x++)
                {
                    for (int y = 0; y < size.Y; y++)
                    {
                        Tile t = new Tile(0);
                        t.Color = new HSVColor(huefraction*((y*size.X) + x), 1, 1, 1).ToRGBColor();
                        t.Color.Action = ColorOutOfBoundsAction.WrapAround;

                        SetTile(x, y, t);
                    }
                }
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                foreach (var tile in GetTiles())
                {
                    //Convert color to HSV, increase hue, then put it back
                    HSVColor hsv = ColorMath.RGBtoHSV(tile.Color);
                    hsv.H += HUEINCREASE;
                    tile.Color = ColorMath.HSVtoRGB(hsv);
                    tile.Color.Action = ColorOutOfBoundsAction.WrapAround;
                }
            }
        }

        private class XYZTilemap : Tilemap
        {
            public XYZTilemap(IComponent parent, string name, Point size)
                : base(parent, name, Assets.Pixel, size, new Point(1, 1))
            {
                float huefraction = 1f / (size.X * size.Y);

                for (int x = 0; x < size.X; x++)
                {
                    for (int y = 0; y < size.Y; y++)
                    {
                        Tile t = new Tile(0);
                        t.Color = new HSVColor(huefraction * ((y * size.X) + x), 1, 1, 1).ToRGBColor();
                        t.Color.Action = ColorOutOfBoundsAction.WrapAround;

                        SetTile(x, y, t);
                    }
                }
            }
            public override void Update(GameTime gt)
            {
                base.Update(gt);

                foreach (var tile in GetTiles())
                {
                    //Convert color to HSV, increase huek
                    HSVColor hsv = ColorMath.RGBtoHSV(tile.Color);
                    hsv.H += HUEINCREASE;

                    XYZColor xyz = ColorMath.RGBtoXYZ(hsv.ToRGBColor());
                    tile.Color = ColorMath.XYZtoRGB(xyz);
                    tile.Color.Action = ColorOutOfBoundsAction.WrapAround;
                }
            }
        }

        private class FadeOutLabel : Label
        {
            private Timer _beginFadeTimer, _fadeStepTimer;

            public FadeOutLabel(IComponent parent, string name) : base(parent, name)
            {
                _beginFadeTimer = new Timer(this, "BeginFadeTimer");
                _beginFadeTimer.Milliseconds = 2000;
                _beginFadeTimer.LastEvent += StartFade;
                _beginFadeTimer.LastEvent += _beginFadeTimer.Stop;
                _beginFadeTimer.Start();
                
                _fadeStepTimer = new Timer(this, "FadeStepTimer");
                _fadeStepTimer.Milliseconds = 500;
                _fadeStepTimer.LastEvent += _fadeStepTimer.Stop;
            }

            public void StartFade()
            {
                _beginFadeTimer.Stop();
                _fadeStepTimer.Start();
            }

            public void Reset()
            {
                _beginFadeTimer.Reset();
                _fadeStepTimer.Reset();
                _beginFadeTimer.Start();
                Render.Alpha = 1;
            }

            public override void Update(GameTime gt)
            {
                base.Update(gt);

                //Set position
                X = EntityGame.Viewport.Width/2f - Width/2f;
                Y = 30;

                //Set alpha
                Render.Alpha = 1 - _fadeStepTimer.Progress;
            }
        }
    }
}