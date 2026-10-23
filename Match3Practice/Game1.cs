using Match3Practice.Input;
using Match3Practice.src;
using Match3Practice.src.graphics;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;
using System;
using MonoGame.Extended.Tweening;
using Match3Practice.src.states;

namespace Match3Practice
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private BoxingViewportAdapter _viewportAdapter;
        private OrthographicCamera _camera;
        private StateMachine _stateMachine;
        private KeyboardInfo _keyboard;


        // Graphics
        private Texture2D _backgroundTexture;
        private Texture2D _tilesTexture;
        private Texture2D _pixel;
        private TextureAtlas _tileAtlas;
        private Dictionary<string, Sprite> _tileSprites = new Dictionary<string, Sprite>();

        // Fonts
        private SpriteFont _smallFont;
        private SpriteFont _mediumFont;
        private SpriteFont _largeFont;

        // Audio
        private SoundEffect _clockSound;
        private SoundEffect _errorSound;
        private SoundEffect _gameOverSound;
        private SoundEffect _matchSound;
        private SoundEffect _nextLevelSound;
        private SoundEffect _selectSound;


        private Song Music1;
        private Song Music2;
        private Song Music3;

        

        // Other
        private Random _random = new Random();
        private Timer _timer;
        private int[] _intervals;
        private int[] _counters;
        private Board _gameBoard;
        private bool _highlightedTile;
        private int _highlightedTileX;
        private int _highlightedTileY;
        private Tile _selectedTile;
        private Tweener _tweener = new Tweener();

        const float BACKGROUND_SCROLL = 80f;

        private float _backgroundX = 0f;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Window Size
            _graphics.PreferredBackBufferWidth = Globals.WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = Globals.WINDOW_HEIGHT;
            _graphics.ApplyChanges();

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, Globals.VIRTUAL_WIDTH, Globals.VIRTUAL_HEIGHT);
            _camera = new OrthographicCamera(_viewportAdapter);

            _keyboard = new KeyboardInfo();
            _stateMachine = new StateMachine();



            // populate sound effects dictionary
            Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>
            {
                { "clock", _clockSound },
                { "error", _errorSound },
                { "game-over", _gameOverSound },
                { "match", _matchSound },
                { "next-level", _nextLevelSound },
                { "select", _selectSound }
            };

            // Populate fonts dictionary
            Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>
            {
                { "small", _smallFont },
                { "medium", _mediumFont },
                { "large", _largeFont }
            };


            _stateMachine.States = new Dictionary<string, State>
            {
                { "start", new StartState(_stateMachine, _tileSprites, _random, soundEffects, _tweener, _pixel, fonts, this) },
                { "begin-game", new BeginGameState(_stateMachine, _tileSprites, _random, _pixel, fonts, GraphicsDevice) },
                { "game-over", new GameOverState(_stateMachine, fonts, _pixel) },
            };

            _stateMachine.ChangeState("start");
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Graphics
            _backgroundTexture = Content.Load<Texture2D>("graphics/background");
            _tilesTexture = Content.Load<Texture2D>("graphics/match3");
            _tileAtlas = new TextureAtlas(_tilesTexture);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            // Populate list of tile sprites from the texture atlas.
            PopulateTileSprites();
            _gameBoard = new Board(Globals.VIRTUAL_WIDTH-(Globals.TILE_SIZE*Globals.BOARD_WIDTH), 
                Globals.TILE_SIZE+16,Globals.BOARD_WIDTH, Globals.BOARD_HEIGHT, _tileSprites, _random);

            // Audio
            _clockSound = Content.Load<SoundEffect>("sounds/clock");
            _errorSound = Content.Load<SoundEffect>("sounds/error");
            _gameOverSound = Content.Load<SoundEffect>("sounds/game-over");
            _matchSound = Content.Load<SoundEffect>("sounds/match");
            _nextLevelSound = Content.Load<SoundEffect>("sounds/next-level");
            _selectSound = Content.Load<SoundEffect>("sounds/select");
            Music1 = Content.Load<Song>("sounds/music");
            Music2 = Content.Load<Song>("sounds/music2");
            Music3 = Content.Load<Song>("sounds/music3");

            // Fonts
            _smallFont = Content.Load<SpriteFont>("fonts/small");
            _mediumFont = Content.Load<SpriteFont>("fonts/medium");
            _largeFont = Content.Load<SpriteFont>("fonts/large");

        }

        private void PopulateTileSprites()
        {
            int tileWidth = 32;
            int tileHeight = 32;

            float sheetWidth = _tilesTexture.Width / tileWidth;
            float sheetHeight = _tilesTexture.Height / tileHeight;

            int color = 0;
            int variety = 0;

            for (int i = 0; i < sheetHeight; i++)
            {             
                for (int j = 0; j < 6; j++)
                {
                    string tileKey = $"tile-{color}-{variety}";
                    _tileAtlas.AddRegion(tileKey, j * tileWidth, i * tileHeight, tileWidth, tileHeight);
                    _tileSprites.Add(tileKey, _tileAtlas.CreateSprite(tileKey));
                    variety++;
                }
                color++;
                variety = 0;
                for (int j = 6; j < sheetWidth; j++)
                {
                    string tileKey = $"tile-{color}-{variety}";
                    _tileAtlas.AddRegion(tileKey, j * tileWidth, i * tileHeight, tileWidth, tileHeight);
                    _tileSprites.Add(tileKey, _tileAtlas.CreateSprite(tileKey));
                    variety++;
                }
                color++;
                variety = 0;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            //_tweener.Update(gameTime.GetElapsedSeconds());
            //_timer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            _backgroundX = _backgroundX - BACKGROUND_SCROLL * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_backgroundX <= -1024 + Globals.VIRTUAL_WIDTH - 4 + 51)
            {
                _backgroundX = 0;
            }

            _keyboard.Update();
            _stateMachine.Update(gameTime, _keyboard);

            //int selectedTileX = _selectedTile.GridX;
            //int selectedTileY = _selectedTile.GridY;

            //if (_keyboard.WasKeyJustPressed(Keys.Up))
            //{
            //    if (selectedTileY > 0)
            //    {
            //        _selectedTile = _gameBoard.Tiles[selectedTileX, selectedTileY - 1];
            //    }
            //}
            //else if (_keyboard.WasKeyJustPressed(Keys.Down))
            //{
            //    if (selectedTileY < Globals.BOARD_HEIGHT - 1)
            //    {
            //        _selectedTile = _gameBoard.Tiles[selectedTileX, selectedTileY + 1];
            //    }
            //}
            //else if (_keyboard.WasKeyJustPressed(Keys.Left))
            //{
            //    if (selectedTileX > 0)
            //    {
            //        _selectedTile = _gameBoard.Tiles[selectedTileX - 1, selectedTileY];
            //    }
            //}
            //else if (_keyboard.WasKeyJustPressed(Keys.Right))
            //{
            //    if (selectedTileX < Globals.BOARD_WIDTH - 1)
            //    {
            //        _selectedTile = _gameBoard.Tiles[selectedTileX + 1, selectedTileY];
            //    }
            //}

            //if (_keyboard.WasKeyJustPressed(Keys.Enter))
            //{
            //    if (!_highlightedTile)
            //    {
            //        _highlightedTile = true;
            //        _highlightedTileX = _selectedTile.GridX;
            //        _highlightedTileY = _selectedTile.GridY;
            //    }
            //    else
            //    {
            //        Tile tile1 = _selectedTile;
            //        Tile tile2 = _gameBoard.Tiles[_highlightedTileX, _highlightedTileY];

            //        float tempX = tile2.X;
            //        float tempY = tile2.Y;
            //        int tempGridX = tile2.GridX;
            //        int tempGridY = tile2.GridY;

            //        // Swap the positions of the two tiles
            //        Tile tempTile = tile1;
            //        _gameBoard.Tiles[tile1.GridX, tile1.GridY] = tile2;
            //        _gameBoard.Tiles[tile2.GridX, tile2.GridY] = tempTile;

            //        _tweener.TweenTo(target: tile2, expression: tile => tile.X, toValue: tile1.X, duration: 0.2f)
            //            .Easing(EasingFunctions.Linear);
            //        _tweener.TweenTo(target: tile2, expression: tile => tile.Y, toValue: tile1.Y, duration: 0.2f)
            //            .Easing(EasingFunctions.Linear);
            //        _tweener.TweenTo(target: tile1, expression: tile => tile.X, toValue: tempX, duration: 0.2f)
            //            .Easing(EasingFunctions.Linear);
            //        _tweener.TweenTo(target: tile1, expression: tile => tile.Y, toValue: tempY, duration: 0.2f)
            //            .Easing(EasingFunctions.Linear);

            //        tile2.GridX = tile1.GridX;
            //        tile2.GridY = tile1.GridY;

            //        tile1.GridX = tempGridX;
            //        tile1.GridY = tempGridY;

            //        _highlightedTile = false;
            //        _selectedTile = tile2;
            //    }

            //}

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Get the camera's transformation matrix
            Matrix transformMatrix = _camera.GetViewMatrix();

            // Apply the transformation to the sprite batch
            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(_backgroundTexture, new Vector2(_backgroundX, 0), Color.White);

            _stateMachine.Draw(_spriteBatch);

            //_gameBoard.Draw(_spriteBatch);

            //// Draw the selected tile highlight
            //int lineThickness = 2;

            //// Top
            //_spriteBatch.Draw(_pixel, new Rectangle(_gameBoard.X + (int)_selectedTile.X, _gameBoard.Y + (int)_selectedTile.Y, Globals.TILE_SIZE, lineThickness), Color.Yellow);

            //// Bottom
            //_spriteBatch.Draw(_pixel, new Rectangle(_gameBoard.X + (int)_selectedTile.X, _gameBoard.Y + (int)_selectedTile.Y + Globals.TILE_SIZE, Globals.TILE_SIZE, lineThickness), Color.Yellow);

            //// Left 
            //_spriteBatch.Draw(_pixel, new Rectangle(_gameBoard.X + (int)_selectedTile.X, _gameBoard.Y+(int)_selectedTile.Y, lineThickness, Globals.TILE_SIZE), Color.Yellow);

            //// Right
            //_spriteBatch.Draw(_pixel, new Rectangle(_gameBoard.X + (int)_selectedTile.X + Globals.TILE_SIZE, _gameBoard.Y + (int)_selectedTile.Y, lineThickness, Globals.TILE_SIZE), Color.Yellow);


            // Draw graphics
            //_stateMachine.Draw(_spriteBatch);
            _spriteBatch.End();

            // Draw Text.
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
            
            _stateMachine.DrawText(_spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            //for (int i = 0; i < _intervals.Length; i++)
            //{
            //    _spriteBatch.DrawString(_mediumFont, $"Timer {i + 1}: {_counters[i]}", new Vector2(10, 10 + i * 40), Color.White);
            //}
            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            _stateMachine.DrawForeground(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
