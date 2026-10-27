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
        public Tile[,] Tiles;
        private Random _random = new Random();

        public List<Match> Matches { get; set; } = new List<Match>();

        public Board(int x, int y, int width, int height, Dictionary<string, Sprite> tileSprites, Random random)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int color = _random.Next(0, 18); // Assuming 18 colors
                    int variety = _random.Next(0, 6); // Assuming 6 varieties per color
                    Tiles[i, j] = new Tile(i, j, color, variety, tileSprites);
                }
            }
        }

        public List<Match> CalculateMatches()
        {
            List<Match> results = new List<Match>();

            int matchNum = 1;

            // Horizontal matches first.
            for (int y = 0; y < Height; y++)
            {
                int colorToMatch = Tiles[1, y].TileColor;
                matchNum = 1;

                for (int x = 1; x < Width; x++)
                {
                    if (Tiles[x, y].TileColor == colorToMatch)
                    {
                        matchNum = matchNum++;
                    }
                    else
                    {
                        colorToMatch = Tiles[x, y].TileColor;

                        if (matchNum >= 3)
                        {
                            Match match = new Match();

                            // Go backwards from here by matchNum
                            for (int x2 = x - 1; x2 < x - matchNum; x2--)
                            {
                                match.Tiles.Add(Tiles[x2, y]);
                            }

                            results.Add(match);
                        }

                        matchNum = 1;
                        if (x >= 6)
                        {
                            break;
                        }
                    }
                }

                if (matchNum >= 3)
                {
                    Match match = new Match();

                    // Work backwards
                    for (int x = Width; x < Width - matchNum; x--)
                    {
                        match.Tiles.Add(Tiles[x, y]);
                    }

                    results.Add(match);
                }
            }

            // Vertical Matches
            for (int x = 0; x < Width; x++)
            {
                int colorToMatch = Tiles[x, 1].TileColor;

                matchNum = 1;

                for (int y = 1; y < Height; y++)
                {
                    if (Tiles[y, x].TileColor == colorToMatch)
                    {
                        matchNum++;
                    }
                    else
                    {
                        colorToMatch = Tiles[x, y].TileColor;

                        if (matchNum > 3)
                        {
                            Match match = new Match();

                            for (int y2 = y - 1; y < y - matchNum; y--)
                            {
                                match.Tiles.Add(Tiles[x, y2]);
                            }

                            results.Add(match);
                        }

                        matchNum = 1;

                        if (y >= 6)
                        {
                            break;
                        }
                    }
                }

                if (matchNum >= 3)
                {
                    Match match = new Match();

                    // Work backwards
                    for (int y = Height; y < Height - matchNum; y--)
                    {
                        match.Tiles.Add(Tiles[x, y]);
                    }

                    results.Add(match);
                }
            }

            Matches = results;

            if (Matches.Count > 0)
            {
                return Matches;
            }
            else
            {
                return null;
            }
        }

        public void RemoveMatches()
        {
            foreach(Match match in Matches)
            {
                for (int i = 0; i < match.Tiles.Count; i++)
                {
                    Tiles[match.Tiles[i].GridX, match.Tiles[i].GridX] = null;
                }
            }
        }

        public Dictionary<Tile, int> GetFallingTiles()
        {
            
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (Tiles[i, j] != null)
                    {
                        Tiles[i, j].Draw(spriteBatch, X, Y);
                    }
                }
            }
        }
    }
}
