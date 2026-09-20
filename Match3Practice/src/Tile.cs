using Match3Practice.src.graphics;
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
        public int Color { get; set; }
        public int Variety { get; set; }

        private Dictionary<string, Sprite> _tiles;

        public Tile(int x, int y, int color, int variety, Dictionary<string, Sprite> tiles)
        {
            GridX = x;
            GridY = y;
            X = (GridX - 1) * Globals.TILE_SIZE;
            Y = (GridY - 1) * Globals.TILE_SIZE;
            Color = color;
            Variety = variety;
            _tiles = tiles;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw a shadow

            // Draw the tile
            var tileSprite = _tiles[$"tile-{Color}-{Variety}"];
            //spriteBatch.Draw(tileSprite,);
        }
    }
}
