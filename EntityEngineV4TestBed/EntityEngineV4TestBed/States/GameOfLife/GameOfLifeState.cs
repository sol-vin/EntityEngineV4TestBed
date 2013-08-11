using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using EntityEngineV4.GUI;
using EntityEngineV4.Input.MouseInput;
using EntityEngineV4.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.GameOfLife
{
    public class GameOfLifeState : TestBedState
    {
        public Tilemap Cells;

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
        public bool WrapEdges = false; //TODO: Fix cell wrapping!

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

            Cells = new Tilemap(this, "Cells", EntityGame.Game.Content.Load<Texture2D>(@"GameOfLife\tiles"), new Point(30,30),new Point(16,16));
            Cells.SetAllTiles(DEAD);
            //Position Tilemap to center
            Cells.Body.Position = new Vector2(EntityGame.Viewport.Width/2f - Cells.Body.Width/2f, 10);

            Cells.TileSelected += OnTileSelected;

            _tiles = Cells.GetTiles();

            //GUI
            LinkLabel startLink = new LinkLabel(this, "StartLink");
            startLink.Body.Position = new Vector2(50, 500);
            startLink.TabPosition = new Point(0,0);
            startLink.OnFocusGain(startLink);
            startLink.Text = "Start";
            startLink.Selected += control => _manager.Start();
            startLink.AttachToControlHandler();

            LinkLabel stopLink = new LinkLabel(this, "StopLink");
            stopLink.Body.Position = new Vector2(50, startLink.Body.Bottom);
            stopLink.TabPosition = new Point(0, 1);
            stopLink.Text = "Stop";
            stopLink.Selected += control => _manager.Stop();
            stopLink.AttachToControlHandler();

            LinkLabel resetLink = new LinkLabel(this, "ResetLink");
            resetLink.Body.Position = new Vector2(50, stopLink.Body.Bottom);
            resetLink.TabPosition = new Point(0, 2);
            resetLink.Text = "Reset";
            resetLink.Selected += control => Cells.Render.SetAllTiles(DEAD);
            resetLink.AttachToControlHandler();
        }

        private void OnTileSelected(Tile tile)
        {
            if(!_manager.RunningSimulation)
                tile.Index = tile.Index != DEAD ? DEAD : ALIVE;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (Destroyed) return;

            MouseHandler.Cursor.Render.Color = Color.Purple;
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
                        testx += _tiles.GetUpperBound(0);
                    }

                    if (testy < 0)
                    {
                        testy += _tiles.GetUpperBound(1);
                    }
                }
                else if ((testx < 0 || testy < 0) && !WrapEdges)
                {
                    continue;
                }

                if ((testx >= _tiles.GetUpperBound(0) || testy >= _tiles.GetUpperBound(1)) && WrapEdges)
                {
                    //Wrap the edges around
                    if (testx >= _tiles.GetUpperBound(0))
                    {
                        testx -= _tiles.GetUpperBound(0);
                    }

                    if (testy >= _tiles.GetUpperBound(1))
                    {
                        testy -= _tiles.GetUpperBound(1);
                    }
                }
                else if ((testx >= _tiles.GetUpperBound(0) || testy >= _tiles.GetUpperBound(1)) && !WrapEdges)
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
            _tiles = Cells.GetTiles();

            for(int x = 0; x <= _tiles.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= _tiles.GetUpperBound(0); y++)
                {
                    short index = _tiles[x, y].Index;
                    int neighborcount = GetNeighborsCount(x, y);

                    //Toggle cell based on Life conditions
                    if (index == ALIVE)
                    {
                        if (neighborcount < 2) Cells.SetTile(x,y,DEAD);
                        else if (neighborcount > 3) Cells.SetTile(x,y,DEAD);
                    }
                    else
                    {
                        if (neighborcount == 3) 
                            Cells.SetTile(x,y,ALIVE);
                    }

                    //Check to see if the cell is not alive or dead, 
                    //subtract its index if not to create the
                    //death map
                    //if (index < ALIVE || index > DEAD)
                    //    Cells.SetTile(x,y,--index);
                }
            }
        }

        private class GameOfLifeManager : Entity
        {
            public Timer UpdateTimer;

            public bool RunningSimulation { get { return UpdateTimer.Alive; } }

            public GameOfLifeManager(IComponent parent, string name) : base(parent, name)
            {
                UpdateTimer = new Timer(this, "UpdateTimer");
                UpdateTimer.Milliseconds = 1000;
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
