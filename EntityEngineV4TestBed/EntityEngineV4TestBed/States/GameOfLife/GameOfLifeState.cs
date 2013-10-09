using System;
using System.Collections.Generic;
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
    public class GameOfLifeState : TestBedState
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
        public bool WrapEdges = true; //TODO: Fix cell wrapping!

        private Tile[,] _tiles;

        private GameOfLifeManager _manager;

        public GameOfLifeState() : base ("GameOfLifeState")
        {
        }

        public override void Create()
        {
            base.Create();

            new ControlHandler(this);

            _manager = new GameOfLifeManager(this, "Manager");
            _manager.UpdateTimer.LastEvent += CheckAllCells;

            //Cells = new Tilemap(this, "Cells", EntityGame.Game.Content.Load<Texture2D>(@"GameOfLife\tiles"), new Point(30,30),new Point(16,16));

            Cells = new Tilemap(this, "Cells", EntityGame.Game.Content.Load<Texture2D>(@"GameOfLife\tilesSmall"),
                                new Point(30, 30), new Point(1, 1));
            Cells.Render.Scale = new Vector2(16,16);
            Cells.SetAllTiles(new Tile(DEAD) {Color = Color.Red.ToRGBColor()});
            //Position Tilemap to center
            Cells.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - Cells.Width/2f * Cells.Render.Scale.X, 10);

            Cells.TileSelected += OnTileSelected;
            _tiles = Cells.CloneTiles();

            //GUI
            LinkLabel startLink = new LinkLabel(this, "StartLink");
            startLink.Body.Position = new Vector2(Cells.Body.X, 500);
            startLink.TabPosition = new Point(0,0);
            startLink.OnFocusGain(startLink);
            startLink.Text = "Start";
            startLink.OnReleased += control => _manager.Start();
            startLink.AttachToControlHandler();

            LinkLabel stopLink = new LinkLabel(this, "StopLink");
            stopLink.Body.Position = new Vector2(Cells.Body.X, startLink.Body.Bottom);
            stopLink.TabPosition = new Point(0, 1);
            stopLink.Text = "Stop";
            stopLink.OnReleased += control => _manager.Stop();
            stopLink.AttachToControlHandler();

            LinkLabel resetLink = new LinkLabel(this, "ResetLink");
            resetLink.Body.Position = new Vector2(Cells.Body.X, stopLink.Body.Bottom);
            resetLink.TabPosition = new Point(0, 2);
            resetLink.Text = "Reset";
            resetLink.OnReleased += control => ResetCells();
            resetLink.AttachToControlHandler();

            LinkLabel downMillisecondsLink = new LinkLabel(this, "downMillisecondsLink");
            downMillisecondsLink.Body.Position = new Vector2(Cells.Body.X + 100, startLink.Body.Bottom);
            downMillisecondsLink.TabPosition = new Point(1, 0);
            downMillisecondsLink.Text = "<-";
            downMillisecondsLink.OnDown += control => _manager.UpdateTimer.Milliseconds -= 50;
            downMillisecondsLink.AttachToControlHandler();

            _millisecondsText = new Label(this, "millisecondsText");
            _millisecondsText.Body.Position = new Vector2(downMillisecondsLink.Body.Right + 2, startLink.Body.Bottom);
            _millisecondsText.TabPosition = new Point(2, 0);
            _millisecondsText.Text = _manager.UpdateTimer.Milliseconds.ToString();
            _millisecondsText.AttachToControlHandler();

            LinkLabel upMillisecondsLink = new LinkLabel(this, "upMillisecondsLink");
            upMillisecondsLink.Body.Position = new Vector2(_millisecondsText.Body.Right + 25, startLink.Body.Bottom);
            upMillisecondsLink.TabPosition = new Point(3, 0);
            upMillisecondsLink.Text = "->";
            upMillisecondsLink.OnDown += control => _manager.UpdateTimer.Milliseconds += 50;
            upMillisecondsLink.AttachToControlHandler();
        }

        public void ResetCells()
        {
            _manager.Stop();
            Cells.Render.SetAllTiles(new Tile(DEAD) {Color = Color.Red.ToRGBColor()});
        }


        private void OnTileSelected(Tile tile)
        {
            if(!_manager.RunningSimulation)
                tile.Index = tile.Index != DEAD ? DEAD : ALIVE;
        }

        public override void Update(GameTime gt)
        {
            if (_manager.UpdateTimer.Milliseconds == 0) _manager.UpdateTimer.Milliseconds = 50;

            base.Update(gt);
            if (Destroyed) return;

            MouseHandler.Cursor.Render.Color = Color.PaleVioletRed;
            _millisecondsText.Text = _manager.UpdateTimer.Milliseconds.ToString();
            if (_manager.DrawButton.Down() || _manager.DrawMouseButton.Down())
            {
                Tile t = Cells.GetTileByPosition(MouseHandler.Cursor.Position);
                if (t != null)
                {
                    t.Index = ALIVE;
                }
            }

            if (_manager.EraseButton.Down() || _manager.EraseMouseButton.Down())
            {
                Tile t = Cells.GetTileByPosition(MouseHandler.Cursor.Position);
                if (t != null)
                {
                    t.Index = DEAD;
                }
            }
        }

        public int GetNeighborsCount(int x, int y)
        {
            int answer = 0;
            if(x==29 || y == 29)
                Console.WriteLine("YEAH");
            foreach (var neighbor in _neighbors)
            {
                int testx = x + neighbor.X;
                int testy = y + neighbor.Y;

                if ((testx < 0 || testy < 0) && WrapEdges)
                {
                    //Wrap the edges around
                    if (testx < 0)
                    {
                        testx += _tiles.GetUpperBound(0)+1;
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
                        testx -= _tiles.GetUpperBound(0)+1;
                    }

                    if (testy > _tiles.GetUpperBound(1))
                    {
                        testy -= _tiles.GetUpperBound(1)+1;
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

        public void CheckAllCells()
        {
            //Copy the tiles so we can change the Cell's tiles without screwing up the neighbor detection
            _tiles = Cells.CloneTiles();

            for(int x = 0; x <= _tiles.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= _tiles.GetUpperBound(0); y++)
                {
                    short index = _tiles[x, y].Index;
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
                            Cells.SetTile(x,y, index);
                        }

                        //Turn it on if it's alive
                        if (neighborcount == 3)
                            Cells.SetTile(x, y, ALIVE);
                    }
                }
            }
        }

        private class GameOfLifeManager : Entity
        {
            public Timer UpdateTimer;
            public GamepadInput DrawButton;
            public MouseInput DrawMouseButton;

            public GamepadInput EraseButton;
            public MouseInput EraseMouseButton;

            public bool RunningSimulation { get { return UpdateTimer.Alive; } }

            public GameOfLifeManager(IComponent parent, string name) : base(parent, name)
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
