using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src
{
    public class Tile
    {
        public int GridX { get; set; }
        public int GridY { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Tile(int x, int y, int color, int variety)
        {
            GridX = x;
            GridY = y;
            X = (GridX - 1) * Globals.TILE_SIZE;
            Y = (GridY - 1) * Globals.TILE_SIZE;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
