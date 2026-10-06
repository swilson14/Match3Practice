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
        public float X { get; set; }
        public float Y { get; set; }
        public int TileColor { get; set; }
        public int Variety { get; set; }

        private Dictionary<string, Sprite> _tileSprites;

        public Tile(int x, int y, int color, int variety, Dictionary<string, Sprite> tileSprites)
        {
            GridX = x;
            GridY = y;
            X = (GridX - 1) * Globals.TILE_SIZE;
            Y = (GridY - 1) * Globals.TILE_SIZE;
            TileColor = color;
            Variety = variety;
            _tileSprites = tileSprites;
        }

        public void Draw(SpriteBatch spriteBatch, int boardX, int boardY)
        {
            var tileSprite = _tileSprites[$"tile-{TileColor}-{Variety}"];
            // Draw a shadow
            //tileSprite.Color = Color.White * 0.3f; 

            // Draw the tile
            tileSprite.Draw(spriteBatch, new Vector2(boardX + X, boardY + Y));
        }
    }
}
