using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4TestBed.States.TilemapTest
{
    public class TilemapTestState : TestBedState
    {
        Random _rand = new Random();
        private Tilemap _tm;
        public TilemapTestState(EntityGame eg) : base(eg, "TilemapTest")
        {
            Services.Add(new InputHandler(this));
            
            _tm = new Tilemap(this, this, "Tilemap", EntityGame.Game.Content.Load<Texture2D>(@"TilemapTest/tiles"), MakeTiles(30,30), new Point(16,16));
            _tm.Data.Scale = new Vector2(1.5f,1.5f);
            AddEntity(_tm);
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
    }
}
