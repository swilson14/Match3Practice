using Match3Practice.Input;
using Match3Practice.src;
using Match3Practice.src.graphics;
using Match3Practice.src.states;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Tweening;
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
        private Texture2D _pixel;
        private TextureAtlas _tileAtlas;
        private Dictionary<string, Sprite> _tileSprites = new Dictionary<string, Sprite>();
        private Texture2D _particle;
        private ParticleEffect _sparkleEffect;
        private ParticleEffect _explosionEffect;

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


        private Song _music1;
        private Song _music2;
        private Song _music3;

        

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
                { "start", new StartState(_stateMachine, _tileSprites, _random, soundEffects, _tweener, _pixel, fonts, this, _sparkleEffect, _explosionEffect) },
                { "begin-game", new BeginGameState(_stateMachine, _tileSprites, _random, _pixel, fonts, GraphicsDevice, _sparkleEffect, _explosionEffect) },
                { "play", new PlayState(_stateMachine, soundEffects, this, _pixel, fonts) },
                { "game-over", new GameOverState(_stateMachine, fonts, _pixel) },
            };

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music1);

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
            _particle = Content.Load<Texture2D>("graphics/particle");

            CreateParticleEffects();

            // Populate list of tile sprites from the texture atlas.
            PopulateTileSprites();

            // Audio
            _clockSound = Content.Load<SoundEffect>("sounds/clock");
            _errorSound = Content.Load<SoundEffect>("sounds/error");
            _gameOverSound = Content.Load<SoundEffect>("sounds/game-over");
            _matchSound = Content.Load<SoundEffect>("sounds/match");
            _nextLevelSound = Content.Load<SoundEffect>("sounds/next-level");
            _selectSound = Content.Load<SoundEffect>("sounds/select");
            _music1 = Content.Load<Song>("sounds/music");
            _music2 = Content.Load<Song>("sounds/music2");
            _music3 = Content.Load<Song>("sounds/music3");

            // Fonts
            _smallFont = Content.Load<SpriteFont>("fonts/small");
            _mediumFont = Content.Load<SpriteFont>("fonts/medium");
            _largeFont = Content.Load<SpriteFont>("fonts/large");

        }

        private void CreateParticleEffects()
        {
            CreateSparkleEffect();
            CreateExplosionEffect();
        }

        private void CreateExplosionEffect()
        {
            Vector2 viewportCenter = GraphicsDevice.Viewport.Bounds.Center.ToVector2();

            _explosionEffect = new ParticleEffect("Explosion")
            {
                Position = Vector2.One,

                // Disable auto trigger - we'll trigger manually
                AutoTrigger = false,
            };

            ParticleEmitter emitter = new ParticleEmitter(2000)
            {
                Name = "Explosion Emitter",
                LifeSpan = 1.5f,
                TextureRegion = new MonoGame.Extended.Graphics.Texture2DRegion(_pixel),

                // Change the profile to make particles explode outward
                Profile = Profile.Line(Vector2.UnitX, 8.0f * Globals.TILE_SIZE, LineRadiation.None),

                Parameters = new ParticleReleaseParameters
                {
                    Quantity = new ParticleInt32Parameter(150, 200),

                    // Increase random speed to 100-200
                    Speed = new ParticleFloatParameter(60f, 100f),

                    Color = new ParticleColorParameter(new Vector3(0.0f, 1.0f, 0.6f)),
                    Scale = new ParticleVector2Parameter(new Vector2(3.0f, 5.0f))
                }
            };

            emitter.Modifiers.Add(new LinearGravityModifier
            {
                // Change gravity to move down
                Direction = Vector2.UnitY,

                Strength = 20f
            });

            emitter.Modifiers.Add(new AgeModifier
            {
                Interpolators =
        {
            new OpacityInterpolator
            {
                StartValue = 1.0f,
                EndValue = 0.0f
            },
            new ScaleInterpolator
            {
                StartValue = new Vector2(3.0f, 5.0f),
                EndValue = new Vector2(1.0f, 0.0f)
            }
        }
            });

            // Add drag modifier for air resistance
            emitter.Modifiers.Add(new DragModifier
            {
                Density = 1.0f,
                DragCoefficient = 1.0f
            });

            _explosionEffect.Emitters.Add(emitter);
        }

        private void CreateSparkleEffect()
        {
            Vector2 viewportCenter = GraphicsDevice.Viewport.Bounds.Center.ToVector2();

            _sparkleEffect = new ParticleEffect("sparkle")
            {
                Position = viewportCenter,

                // Disable auto trigger - we'll trigger manually
                AutoTrigger = false,

                AutoTriggerFrequency = 0.1f
            };

            ParticleEmitter emitter = new ParticleEmitter(2000)
            {
                Name = "Sparkle Emitter",

                // Each particle created by this emitter lives for 0.5 seconds
                LifeSpan = 0.2f,
                TextureRegion = new MonoGame.Extended.Graphics.Texture2DRegion(_pixel),

                // Expand the radius of the circle profile to 200
                Profile = Profile.Circle(10, CircleRadiation.Out),

                Parameters = new ParticleReleaseParameters
                {
                    Quantity = new ParticleInt32Parameter(10, 20),

                    // Decrease random speed to 10-40
                    Speed = new ParticleFloatParameter(10.0f, 40.0f),

                    Color = new ParticleColorParameter(
                        new Vector3(252.0f, 1.0f, 0.8f),
                        new Vector3(180.0f, 1.0f, 0.5f)),

                    // Scale
                    Scale = new ParticleVector2Parameter(new Vector2(2.0f, 2.0f))
                }
            };

            // Remove the Linear Gravity Modifier for floating effect
            emitter.Modifiers.Add(new AgeModifier
            {
                Interpolators =
        {
            // Change to Hue interpolator to cycle the hue over time to give the
            // shimmering effect.
            new HueInterpolator
            {
                // Start at red (Hue=0°)
                StartValue = 0.0f,

                // Cycle through all colors (Hue=360°)
                EndValue = 360.0f
            }
        }
            });

            _sparkleEffect.Emitters.Add(emitter);
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

            _backgroundX = _backgroundX - BACKGROUND_SCROLL * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_backgroundX <= -1024 + Globals.VIRTUAL_WIDTH - 4 + 51)
            {
                _backgroundX = 0;
            }

            _keyboard.Update();
            _stateMachine.Update(gameTime, _keyboard);
            _sparkleEffect.Update(gameTime);
            _explosionEffect.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Get the camera's transformation matrix
            Matrix transformMatrix = _camera.GetViewMatrix();

            // Apply the transformation to the sprite batch
            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            // Draw graphics.
            _spriteBatch.Draw(_backgroundTexture, new Vector2(_backgroundX, 0), Color.White);

            _stateMachine.Draw(_spriteBatch);

            _spriteBatch.Draw(_sparkleEffect);

            _spriteBatch.Draw(_explosionEffect);

            _spriteBatch.End();

            // Draw Text.
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
                 
            _stateMachine.DrawText(_spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            _spriteBatch.End();

            // Draw objects that sit in front of the text.
            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            _stateMachine.DrawForeground(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
