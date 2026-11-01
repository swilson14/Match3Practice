using Match3Practice.src.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Data;
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

        public bool IsShiny { get; set; } = false;

        private Dictionary<string, Sprite> _tileSprites;
        private ParticleEffect _sparkleEffect;

        private int _boardX;
        private int _boardY;


        public Tile(int x, int y, int color, int variety, Dictionary<string, Sprite> tileSprites, ParticleEffect sparkleEffect,
            int boardX, int boardY,bool isShiny = false)
        {
            GridX = x;
            GridY = y;
            X = GridX * Globals.TILE_SIZE;
            Y = GridY * Globals.TILE_SIZE;
            TileColor = color;
            Variety = variety;
            _tileSprites = tileSprites;
            _sparkleEffect = sparkleEffect;
            IsShiny = isShiny;

            _boardX = boardX;
            _boardY = boardY;

            if (isShiny)
            {
                _sparkleEffect.AutoTrigger = true;
                _sparkleEffect.Position = new Vector2(_boardX+X+Globals.TILE_SIZE/2, _boardY+Y+Globals.TILE_SIZE/2);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var tileSprite = _tileSprites[$"tile-{TileColor}-{Variety}"];

            // Draw a shadow
            //tileSprite.Color = Color.White * 0.3f; 

            // Draw the tile
            tileSprite.Draw(spriteBatch, new Vector2(_boardX + X, _boardY + Y));
        }
    }
}
