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
using System;
using System.Collections.Generic;

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
        private TextureAtlas _tileAtlas;
        private List<Sprite> _tileSprites = new List<Sprite>();

        private Song Music1;
        private Song Music2;
        private Song Music3;

        // Other
        private Random _random = new Random();
        private int[,] _board;
        private Timer _timer;
        private int[] _intervals;
        private int[] _counters;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _timer = new Timer();

            // Timer example
            _intervals = new int[] { 1, 2, 4, 3, 2, 8 };
            _counters = new int[] { 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < _intervals.Length; i++)
            {
                int index = i;
                _timer.Every(_intervals[index], () =>
                { 
                    _counters[index]++;
                });
            }

            // Window Size
            _graphics.PreferredBackBufferWidth = Globals.WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = Globals.WINDOW_HEIGHT;
            _graphics.ApplyChanges();

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, Globals.VIRTUAL_WIDTH, Globals.VIRTUAL_HEIGHT);
            _camera = new OrthographicCamera(_viewportAdapter);

            _keyboard = new KeyboardInfo();

            _stateMachine = new StateMachine();
            // Add states and then change to the initial state

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Graphics
            _backgroundTexture = Content.Load<Texture2D>("graphics/background");
            _tilesTexture = Content.Load<Texture2D>("graphics/match3");
            _tileAtlas = new TextureAtlas(_tilesTexture);

            // Populate list of tile sprites from the texture atlas.
            PopulateTileSprites();
            RefreshBoard();

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

        private void RefreshBoard()
        {
            _board = new int[Globals.VIRTUAL_HEIGHT / Globals.TILE_SIZE, Globals.VIRTUAL_WIDTH / Globals.TILE_SIZE];

            int boardHeight = Globals.VIRTUAL_HEIGHT / Globals.TILE_SIZE;
            int boardWidth = Globals.VIRTUAL_WIDTH / Globals.TILE_SIZE;

            for (int i = 0; i < boardHeight; i++)
            {
                for (int j = 0; j < boardWidth; j++)
                {
                    _board[i, j] = _random.Next(0, _tileSprites.Count - 1);  
                }
            }
        }

        private void PopulateTileSprites()
        {
            int tileWidth = 32;
            int tileHeight = 32;

            float sheetWidth = _tilesTexture.Width / tileWidth;
            float sheetHeight = _tilesTexture.Height / tileHeight;

            int count = 1;

            for (int i = 0; i < sheetHeight; i++)
            {
                for (int j = 0; j < sheetWidth; j++)
                {
                    _tileAtlas.AddRegion($"tile-{count}", j * tileWidth, i * tileHeight, tileWidth, tileHeight);
                    _tileSprites.Add(_tileAtlas.CreateSprite($"tile-{count}"));
                    count++;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _timer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            _keyboard.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _keyboard.WasKeyJustPressed(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (_keyboard.WasKeyJustPressed(Keys.Space))
            {
                RefreshBoard();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Get the camera's transformation matrix
            Matrix transformMatrix = _camera.GetViewMatrix();

            // Apply the transformation to the sprite batch
            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            int boardHeight = Globals.VIRTUAL_HEIGHT / Globals.TILE_SIZE;
            int boardWidth = Globals.VIRTUAL_WIDTH / Globals.TILE_SIZE;

            //for (int i = 0; i < boardHeight; i++)
            //{
            //    for (int j = 0; j < boardWidth; j++)
            //    {
            //        _tileSprites[_board[i, j]].Draw(_spriteBatch, new Vector2(j * Globals.TILE_SIZE, i * Globals.TILE_SIZE));
            //    }
            //}

            // Draw graphics
            //_stateMachine.Draw(_spriteBatch);
            _spriteBatch.End();

            // Draw Text.
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);

            for (int i = 0; i < _intervals.Length; i++)
            {
                _spriteBatch.DrawString(_mediumFont, $"Timer {i + 1}: {_counters[i]}", new Vector2(10, 10 + i * 40), Color.White);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
