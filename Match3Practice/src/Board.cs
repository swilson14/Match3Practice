using Match3Practice.src.graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using MonoGame.Extended.Particles;
using Microsoft.Xna.Framework;

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
        private Random _random;

        private Dictionary<string, Sprite> _tileSprites;

        public List<Match> Matches { get; set; } = new List<Match>();

        private int _level;
        public ParticleEffect SparkleEffect {get; set; }
        private ParticleEffect _explosionEffect;

        public Board(int x, int y, int width, int height, Dictionary<string, Sprite> tileSprites, Random random, int level,
            ParticleEffect sparkleEffect, ParticleEffect explosionEffect)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            _tileSprites = tileSprites;
            _random = random;
            _level = level;
            SparkleEffect = sparkleEffect;
            _explosionEffect = explosionEffect;

            InitializeTiles(Width, Height);
        }

        private void InitializeTiles(int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int color = _random.Next(18); // Assuming 18 colors
                    int variety = _random.Next(6); // Assuming 6 varieties per color

                    if (variety >= _level)
                    {
                        // level 1 only has basic blocks, with more patterns added per level.
                        variety = _level - 1;
                    }

                    bool isShiny = false;

                    if (_random.Next(10) < 2 && GetShinyTiles().Count < 1)
                    {
                        isShiny = true;
                    }

                    Tiles[i, j] = new Tile(i, j, color, variety, _tileSprites, SparkleEffect, X, Y, isShiny);
                }
            }

            while (CalculateMatches() != null)
            {
                InitializeTiles(width, height);
            }
        }

        public List<Match> CalculateMatches()
        {
            List<Match> results = new List<Match>();

            int matchNum = 1;

            // Horizontal matches first.
            for (int y = 0; y < Height; y++)
            {
                int colorToMatch = Tiles[0, y].TileColor;
                matchNum = 1;

                for (int x = 1; x < Width; x++)
                {
                    if (Tiles[x, y].TileColor == colorToMatch)
                    {
                        matchNum++;
                    }
                    else
                    {
                        colorToMatch = Tiles[x, y].TileColor;

                        if (matchNum >= 3)
                        {
                            Match match = new Match();

                            // Go backwards from here by matchNum
                            for (int x2 = x-1; x2 > x-1 - matchNum; x2--)
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
                    for (int x = Width-1; x > Width-1 - matchNum; x--)
                    {                    
                        match.Tiles.Add(Tiles[x, y]);
                    }

                    results.Add(match);
                }
            }

            // Vertical Matches
            for (int x = 0; x < Width; x++)
            {                
                int colorToMatch = Tiles[x, 0].TileColor;

                matchNum = 1;

                for (int y = 1; y < Height; y++)
                {
                    if (Tiles[x, y].TileColor == colorToMatch)
                    {
                        matchNum++;
                    }
                    else
                    {
                        colorToMatch = Tiles[x, y].TileColor;

                        if (matchNum >= 3)
                        {
                            Match match = new Match();

                            for (int y2 = y-1; y2 > y-1 - matchNum; y2--)
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
                    for (int y = Height-1; y > Height-1 - matchNum; y--)
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

                    Tile currentTile = Tiles[match.Tiles[i].GridX, match.Tiles[i].GridY];

                    if (currentTile == null)
                    {
                        continue;
                    }

                    if (currentTile.IsShiny)
                    {
                        // Destroy all tiles in the same row and trigger a particle effect.
                        for (int x = 0; x < Width; x++)
                        {
                            Tiles[x, currentTile.GridY] = null;
                        }
                        _explosionEffect.Position = new Vector2(X + Globals.TILE_SIZE*4, Y + currentTile.Y);
                        _explosionEffect.Trigger();
                    }
                    else
                    {
                        Tiles[match.Tiles[i].GridX, match.Tiles[i].GridY] = null;
                    }
                }
            }
        }

        public Dictionary<Tile, int> GetFallingTiles()
        {
            Dictionary<Tile, int> results = new Dictionary<Tile, int>();

            for (int x = 0; x < Width; x++)
            {
                bool space = false;
                int spaceY = 0;

                int y = Height-1;
                while (y >= 0)
                {
                    Tile currentTile = Tiles[x, y];

                    if (space)
                    {
                        if (currentTile != null)
                        {
                            Tiles[x, spaceY] = currentTile;
                            currentTile.GridY = spaceY;

                            Tiles[x, y] = null;

                            results.Add(currentTile, currentTile.GridY * Globals.TILE_SIZE);

                            space = false;
                            y = spaceY;
                            spaceY = 0;
                        }
                    }
                    else if (currentTile == null)
                    {
                        space = true;

                        if (spaceY == 0)
                        {
                            spaceY = y;
                        }
                    }
                    y--;
                }
            }
            // Replace tiles
            for (int x = 0; x < Width; x++)
            {
                for (int y = Height-1; y >= 0; y--)
                {
                    Tile currentTile = Tiles[x, y];

                    if (currentTile == null)
                    {

                        int color = _random.Next(18);
                        int variety = _random.Next(6);

                        if (variety >= _level)
                        {
                            // level 1 only has basic blocks, with more patterns added per level.
                            variety = _level - 1;
                        }

                        bool isShiny = false;

                        if (_random.NextDouble() < 0.1 && GetShinyTiles().Count < 1)
                        {
                            isShiny = true;
                        }

                        Tile newTile = new Tile(x, y, color, variety, _tileSprites, SparkleEffect, X, Y, isShiny);

                        newTile.Y = -32;
                        Tiles[x, y] = newTile;

                        results.Add(newTile, newTile.GridY * Globals.TILE_SIZE);
                    }
                }
            }
            return results;
        }

        public List<Tile> GetShinyTiles()
        {
            List<Tile> shinyTiles = new List<Tile>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile currentTile = Tiles[x, y];
                    if (currentTile != null && currentTile.IsShiny)
                    {
                        shinyTiles.Add(currentTile);
                    }
                }
            }
            return shinyTiles;
        }

        public void Update(float deltaTime)
        {
            if (GetShinyTiles().Count <= 0)
            {
                SparkleEffect.AutoTrigger = false;
            }

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (Tiles[i, j] != null)
                    {
                        Tiles[i, j].Update(deltaTime);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (Tiles[i, j] != null)
                    {
                        Tiles[i, j].Draw(spriteBatch);
                    }
                }
            }
        }

        public bool HasPossibleMatches()
        {
            bool result = false;

            return result;
        }
    }
}
