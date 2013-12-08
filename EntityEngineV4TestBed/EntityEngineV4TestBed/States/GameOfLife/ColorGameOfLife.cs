using System;
using System.Collections.Generic;
using System.Linq;

using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input;
using EntityEngineV4.Tiles;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4TestBed.States.GameOfLife
{
    public class ColorGameOfLifeState : TestBedState
    {
        public Tilemap Cells;
        private Label _millisecondsText;
        public const short ALIVE = 4;
        public const short DEAD = 0;

        //List of possible neighbor points
        private List<Point> _neighbors = new List<Point>()
            {
                new Point(-1,-1), new Point(0,-1), new Point(1,-1),
                new Point(-1, 0)                 , new Point(1,0),
                new Point(-1,1), new Point(0, 1), new Point(1, 1)
            };

        //Defines whether or not to wrap the edges around when checking on neighbors
        public bool WrapEdges = true;

        private Tile[,] _tiles;

        private Color _currentColor = Color.Red;

        private ColorGameOfLifeManager _manager;

        public ColorGameOfLifeState()
            : base("GameOfLifeState")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            new ControlHandler(this);
            _page = new Page(this, "GUIPage");

            _manager = new ColorGameOfLifeManager(this, "Manager");
            _manager.UpdateTimer.LastEvent += CheckAllCells;

            //Cells = new Tilemap(this, "Cells", EntityGame.Game.Content.Load<Texture2D>(@"GameOfLife\tiles"), new Point(30,30),new Point(16,16));

            Cells = new Tilemap(this, "Cells", EntityGame.Self.Content.Load<Texture2D>(@"GameOfLife\tilesSmall"),
                                new Point(30, 30), new Point(1, 1));
            Cells.Render.Scale = new Vector2(16, 16);
            Cells.SetAllTiles(new Tile(DEAD) { Color = Color.Red.ToRGBColor() });
            //Position Tilemap to center
            Cells.Body.Position = new Vector2(EntityGame.Viewport.Width / 2f - Cells.Width / 2f * Cells.Render.Scale.X, 10);

            Cells.TileSelected += OnTileSelected;
            _tiles = Cells.CloneTiles();

            //GUI
            _page.Show();

            TextButton startTextButton = new TextButton(_page, "StartButton", new Point(0, 0), new Vector2(Cells.Body.X, 500), Color.White.ToRGBColor());
            startTextButton.OnFocusGain();
            startTextButton.Text = "Start";
            startTextButton.MakeDefault();
            startTextButton.OnReleased += control => _manager.Start();

            TextButton stopTextButton = new TextButton(_page, "StopLink", new Point(0, 1), new Vector2(Cells.Body.X, startTextButton.Body.Bottom), Color.White.ToRGBColor());
            stopTextButton.Text = "Stop";
            stopTextButton.OnReleased += control => _manager.Stop();
            stopTextButton.MakeDefault();

            TextButton resetTextButton = new TextButton(_page, "ResetLink",  new Point(0, 2),new Vector2(Cells.Body.X, stopTextButton.Body.Bottom), Color.White.ToRGBColor());
            resetTextButton.Text = "Reset";
            resetTextButton.OnReleased += control => ResetCells();
            resetTextButton.MakeDefault();

            LinkLabel downMillisecondsLink = new LinkLabel(_page, "downMillisecondsLink", new Point(1, 0));
            downMillisecondsLink.Body.Position = new Vector2(Cells.Body.X + 100, startTextButton.Body.Bottom);
            downMillisecondsLink.Text = "<-";
            downMillisecondsLink.OnDown += control => _manager.UpdateTimer.Milliseconds -= 50;

            _millisecondsText = new Label(_page, "millisecondsText", new Point(2, 0));
            _millisecondsText.Body.Position = new Vector2(downMillisecondsLink.Body.Right + 2, startTextButton.Body.Bottom);
            _millisecondsText.Text = _manager.UpdateTimer.Milliseconds.ToString();

            LinkLabel upMillisecondsLink = new LinkLabel(_page, "upMillisecondsLink", new Point(3, 0));
            upMillisecondsLink.Body.Position = new Vector2(_millisecondsText.Body.Right + 25, startTextButton.Body.Bottom);
            upMillisecondsLink.Text = "->";
            upMillisecondsLink.OnDown += control => _manager.UpdateTimer.Milliseconds += 50;

            MakeNextColorButton(Color.Red.ToRGBColor());
            MakeNextColorButton(Color.Orange.ToRGBColor());
            MakeNextColorButton(Color.Yellow.ToRGBColor());
            MakeNextColorButton(Color.Green.ToRGBColor());
            MakeNextColorButton(Color.LightBlue.ToRGBColor());
            MakeNextColorButton(Color.Blue.ToRGBColor());
            MakeNextColorButton(Color.MediumPurple.ToRGBColor());
        }


        private int _lastX = 160;
        private Point _lastTab = new Point(1,1);
        private Page _page;

        public void MakeNextColorButton(RGBColor color)
        {
            //Change the color to the base hue value
            HSVColor hsv = ColorMath.RGBtoHSV(color);
            //make the color the purest possible
            hsv.S = 1;
            hsv.V = 1;
            RGBColor rgb = hsv.ToRGBColor();

            Button button = new Button(_page, "ColorButton" + _lastX.ToString(), _lastTab, new Vector2(_lastX, 539), new Vector2(20,20), rgb);
            button.OnReleased += control => _currentColor = rgb;
            button.OnDown += c => button.RGBColor = Color.White.ToRGBColor();
            button.FocusLost += c => button.RGBColor = rgb;
            button.FocusGain += c => button.RGBColor = rgb;

            _lastX += 25;
            _lastTab.X++;
        }

        public void ResetCells()
        {
            _manager.Stop();
            Cells.Render.SetAllTiles(new Tile(DEAD) { Color = Color.White.ToRGBColor() });
        }


        private void OnTileSelected(Tile tile)
        {
            if (!_manager.RunningSimulation)
            {
                tile.Index = tile.Index != DEAD ? DEAD : ALIVE;
                tile.Color = _currentColor.ToRGBColor();
            }
        }

        public override void Update(GameTime gt)
        {
            if (_manager.UpdateTimer.Milliseconds == 0) _manager.UpdateTimer.Milliseconds = 50;

            base.Update(gt);
            if (Destroyed) return;

            MouseService.Cursor.Render.Color = Color.PaleVioletRed;
            _millisecondsText.Text = _manager.UpdateTimer.Milliseconds.ToString();
            if (_manager.DrawButton.Down() || _manager.DrawMouseButton.Down())
            {
                Tile t = Cells.GetTileByPosition(MouseService.Cursor.Position);
                if (t != null)
                {
                    t.Index = ALIVE;
                    t.Color = _currentColor.ToRGBColor();
                }
            }

            if (_manager.EraseButton.Down() || _manager.EraseMouseButton.Down())
            {
                Tile t = Cells.GetTileByPosition(MouseService.Cursor.Position);
                if (t != null)
                {
                    t.Index = DEAD;
                    t.Color = Color.White.ToRGBColor();
                }
            }
        }

        public int GetNeighborsCount(int x, int y)
        {
            int answer = 0;

            foreach (var neighbor in _neighbors)
            {
                int testx = x + neighbor.X;
                int testy = y + neighbor.Y;

                if ((testx < 0 || testy < 0) && WrapEdges)
                {
                    //Wrap the edges around
                    if (testx < 0)
                    {
                        testx += _tiles.GetUpperBound(0) + 1;
                    }

                    if (testy < 0)
                    {
                        testy += _tiles.GetUpperBound(1) + 1;
                    }
                }
                else if ((testx < 0 || testy < 0) && !WrapEdges)
                {
                    continue;
                }

                if ((testx > _tiles.GetUpperBound(0) || testy > _tiles.GetUpperBound(1)) && WrapEdges)
                {
                    //Wrap the edges around
                    if (testx > _tiles.GetUpperBound(0))
                    {
                        testx -= _tiles.GetUpperBound(0) + 1;
                    }

                    if (testy > _tiles.GetUpperBound(1))
                    {
                        testy -= _tiles.GetUpperBound(1) + 1;
                    }
                }
                else if ((testx > _tiles.GetUpperBound(0) || testy > _tiles.GetUpperBound(1)) && !WrapEdges)
                {
                    continue;
                }

                if (_tiles[testx, testy].Index == ALIVE)
                {
                    answer++;
                }

            }

            return answer;
        }

        public List<Color> GetNeighborColors(int x, int y)
        {
            var answer = new List<Color>();

            foreach (var neighbor in _neighbors)
            {
                int testx = x + neighbor.X;
                int testy = y + neighbor.Y;

                if ((testx < 0 || testy < 0) && WrapEdges)
                {
                    //Wrap the edges around
                    if (testx < 0)
                    {
                        testx += _tiles.GetUpperBound(0) + 1;
                    }

                    if (testy < 0)
                    {
                        testy += _tiles.GetUpperBound(1) + 1;
                    }
                }
                else if ((testx < 0 || testy < 0) && !WrapEdges)
                {
                    continue;
                }

                if ((testx > _tiles.GetUpperBound(0) || testy > _tiles.GetUpperBound(1)) && WrapEdges)
                {
                    //Wrap the edges around
                    if (testx > _tiles.GetUpperBound(0))
                    {
                        testx -= _tiles.GetUpperBound(0) + 1;
                    }

                    if (testy > _tiles.GetUpperBound(1))
                    {
                        testy -= _tiles.GetUpperBound(1) + 1;
                    }
                }
                else if ((testx > _tiles.GetUpperBound(0) || testy > _tiles.GetUpperBound(1)) && !WrapEdges)
                {
                    continue;
                }

                if (_tiles[testx, testy].Index == ALIVE)
                {
                    answer.Add(_tiles[testx, testy].Color);
                }

            }

            return answer;
        }

        public void CheckAllCells()
        {
            //Copy the tiles so we can change the Cell's tiles without screwing up the neighbor detection
            _tiles = Cells.CloneTiles();
            List<Color> colorsToMix = new List<Color>();

            for (int x = 0; x <= _tiles.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= _tiles.GetUpperBound(0); y++)
                {
                    colorsToMix.Clear();

                    Tile tile = _tiles[x, y];
                    short index = tile.Index;
                    int neighborcount = GetNeighborsCount(x, y);

                    //Toggle cell based on Life conditions
                    if (index == ALIVE)
                    {
                        if (neighborcount < 2) Cells.SetTile(x, y, --index);
                        else if (neighborcount > 3) Cells.SetTile(x, y, --index);
                    }
                    //Check to see if the cell is not alive or dead, 
                    //subtract its index if not to create the
                    //death map
                    else
                    {
                        if (index < ALIVE || index > DEAD)
                        {
                            index--;
                            if (index < DEAD) index = DEAD;
                            Cells.SetTile(x, y, index);
                        }

                        //Turn it on if it's alive
                        if (neighborcount == 3)
                        {
                            Cells.SetTile(x, y, new Tile(ALIVE) { Color = new RGBColor(0,0,0,1)});
                        }
                            
                    }

                    if(Cells.GetTile(x,y).Index == ALIVE)
                    {
                        //Now we get neighbor colors, mix them by average hue, output a new color.
                        colorsToMix = colorsToMix.Concat(GetNeighborColors(x, y)).ToList();

                        float newhue = 0f;

                        //find lowest and highest bands
                        float minhue = float.MaxValue;
                        float maxhue = 0f;
                        foreach (var color in colorsToMix)
                        {
                            float colorhue = color.ToHSVColor().H;
                            minhue = Math.Min(minhue, colorhue);
                            maxhue = Math.Max(maxhue, colorhue);
                        }

                        //Check if the tile was dead, and now is alive
                        if (Cells.GetTile(x, y).Index == ALIVE && _tiles[x,y].Index != ALIVE)
                        {
                            foreach (var color in colorsToMix)
                            {
                                float colorhue = color.ToHSVColor().H;

                                newhue = colorhue - minhue;
                                //now that we have that, we will subtract each color from our min, average it, and get our new color. 

                                newhue /= colorsToMix.Count();
                                newhue += minhue;
                            }
                            Tile newtile = new Tile(ALIVE);
                            newtile.Color = ColorMath.HSVtoRGB(new HSVColor(newhue, 1, 1, 1, ColorOutOfBoundsAction.WrapAround));
                            Cells.SetTile(x, y, newtile);
                        }
                        else if(Cells.GetTile(x,y).Index == ALIVE)
                        {
                            //Get our tile's color
                            HSVColor tilecolor = ColorMath.RGBtoHSV(_tiles[x, y].Color);
                            foreach (var color in colorsToMix)
                            {
                                float colorhue = color.ToHSVColor().H;

                                //find out which direction is the closest route to the color.
                                float route = (Math.Abs(colorhue - tilecolor.H) < Math.Abs(1 - colorhue - tilecolor.H))
                                                  ? colorhue - tilecolor.H
                                                  : 1 - colorhue - tilecolor.H;
                                newhue += route;
                            }

                            //TODO: Update color blending with Cie-LCH, for which you need to implment XYZ and Cie-Lab
                            //now that we have that, we will subtract each color from our min, average it, and get our new color. 

                            newhue /= colorsToMix.Count();
                            newhue += tilecolor.H;

                            Tile newtile = new Tile(ALIVE);
                            newtile.Color = ColorMath.HSVtoRGB(new HSVColor(newhue, 1, 1, 1));
                            newtile.Color.Action = ColorOutOfBoundsAction.WrapAround;
                            Cells.SetTile(x, y, newtile);

                        }

                    }
                }
            }
        }

        private class ColorGameOfLifeManager : Node
        {
            public Timer UpdateTimer;
            public GamepadInput DrawButton;
            public MouseInput DrawMouseButton;

            public GamepadInput EraseButton;
            public MouseInput EraseMouseButton;

            public bool RunningSimulation { get { return UpdateTimer.Alive; } }

            public ColorGameOfLifeManager(Node parent, string name)
                : base(parent, name)
            {
                UpdateTimer = new Timer(this, "UpdateTimer");
                UpdateTimer.Milliseconds = 250;

                DrawButton = new GamepadInput(this, "DrawButton", Buttons.B, PlayerIndex.One);
                DrawMouseButton = new MouseInput(this, "DrawMousebUtton", MouseButton.RightButton);

                EraseButton = new GamepadInput(this, "EraseButton", Buttons.X, PlayerIndex.One);
                EraseMouseButton = new MouseInput(this, "EraseMousebUtton", MouseButton.MiddleButton);
            }

            public void Start()
            {
                UpdateTimer.Start();
            }

            public void Stop()
            {
                UpdateTimer.Stop();
            }
        }
    }
}

