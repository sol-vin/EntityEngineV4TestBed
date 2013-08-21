using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Tiles;
using Microsoft.Xna.Framework;

namespace EntityEngineV4TestBed.States.ColorTest
{
    public class ColorTestState : TestBedState
    {
        private Point _size = new Point(30, 30);
        private Tilemap _tilemap;

        private const float HUEINCREASE = 0.001f;

        public ColorTestState()
            : base("ColorTestState")
        {
        }

        public override void Create()
        {
            base.Create();



            int maxx = EntityGame.Viewport.Width / _size.X + 1;
            int maxy = EntityGame.Viewport.Height / _size.Y + 1;

            _tilemap = new Tilemap(this, "tiles", Assets.Pixel, new Point(maxx, maxy), new Point(1,1));
            _tilemap.Render.Scale = new Vector2(_size.X, _size.Y);
            _tilemap.SetAllTiles(0);

            float huefraction = 1f / (maxx * maxy);

            for (int x = 0; x < maxx; x++)
            {
                for (int y = 0; y < maxy; y++)
                {
                    Tile t = new Tile(0);
                    t.Color = new HSVColor(huefraction*((y*maxx) + x), 1, 1, 1).ToRGBColor();
                    t.Color.Action = ColorOutOfBoundsAction.WrapAround;
                    
                    _tilemap.SetTile(x, y, t);
                }
            }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            Tile[,] tiles = _tilemap.GetTiles();

            foreach (var tile in tiles)
            {
                //Convert color to HSV, increase hue, then put it back
                HSVColor hsv = ColorMath.RGBtoHSV(tile.Color);
                hsv.H += HUEINCREASE;
                tile.Color = ColorMath.HSVtoRGB(hsv);
                tile.Color.Action = ColorOutOfBoundsAction.WrapAround;
            }
        }
    }
}