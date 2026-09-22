using Match3Practice.src.graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Match3Practice.src
{
    /// <summary>
    /// Board class
    /// </summary>
    public class Board
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        private Tile[,] _tiles;
        private Random _random = new Random();
        public Board(int x, int y, int width, int height, Dictionary<string, Sprite> tileSprites, Random random)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            _tiles = new Tile[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int color = _random.Next(0, 18); // Assuming 18 colors
                    int variety = _random.Next(0, 6); // Assuming 6 varieties per color
                    _tiles[i, j] = new Tile(i, j, color, variety, tileSprites);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _tiles[i, j].Draw(spriteBatch, X, Y);
                }
            }
        }
    }
}
