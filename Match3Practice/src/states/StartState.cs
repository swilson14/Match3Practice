using Match3Practice.Input;
using Match3Practice.src.graphics;
using Match3Practice.src.states;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src.states
{
    public class StartState : State
    {
        const int START_BUTTON_X = 579;
        const int START_BUTTON_Y = 390;
        const int QUIT_BUTTON_X = 530;
        const int QUIT_BUTTON_Y = 456;
        const int START_BUTTON_WIDTH = 123;
        const int QUIT_BUTTON_WIDTH = 220;
        const int MENU_HEIGHT = 43;


        // Menu / Gameplay
        private int _currentMenuItem = 0;
        private List<Color> _colors;
        private List<Tuple<string, float>> _letterTable;
        private Timer _colorTimer = new Timer();
        private TransitionAlpha _transitionAlpha = new TransitionAlpha(0f);
        private bool _pauseInput = false;
        private Board _showBoard;
        private Random _random;
        private Tweener _tweener;
        private Game _game;


        // Audio
        private SoundEffect _selectSound;

        // Graphics
        private Texture2D _pixel;
        private ParticleEffect _sparkleEffect;
        private ParticleEffect _explosionEffect;

        //Fonts
        private SpriteFont _smallFont;
        private SpriteFont _mediumFont;
        private SpriteFont _largeFont;

        private MouseInfo _mouse;


        public StartState(StateMachine stateMachine, Dictionary<string, Sprite> sprites, Random random,
            Dictionary<string, SoundEffect> soundEffects, Tweener tweener, Texture2D pixel,
            Dictionary<string, SpriteFont> fonts, Game game, ParticleEffect sparkleEffect, ParticleEffect explosionEffect) : base(stateMachine)
        {

            _pixel = pixel;
            _sparkleEffect = sparkleEffect;
            _explosionEffect = explosionEffect;
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];

            _colors = new List<Color>
            {
              new Color(217, 87, 99),
              new Color(95, 205, 228),
              new Color(251, 242, 54),
              new Color(118, 66, 138),
              new Color(153, 229, 80),
              new Color(223, 113, 38),
            };

            _letterTable = new List<Tuple<string, float>>
            {
                { new Tuple<string, float>("M", -113f) },
                { new Tuple<string, float>("A", -66f) },
                { new Tuple<string, float>("T", -28f) },
                { new Tuple<string, float>("C", 3f) },
                { new Tuple<string, float>("H", 42f) },
                { new Tuple<string, float>("3", 118f) },
            };

            _colorTimer.Every(0.075f, () =>
            {
                var lastColor = _colors[5];

                for (int i = 5; i > 0; i--)
                {
                    _colors[i] = _colors[i - 1];
                }

                _colors[0] = lastColor;
            });

            _random = random;

            _selectSound = soundEffects["select"];

            _tweener = tweener;

            _stateMachine = stateMachine;

            // Generate board of tiles just for show.
            _showBoard = new Board((Globals.VIRTUAL_WIDTH - 8 * Globals.TILE_SIZE) / 2, (Globals.VIRTUAL_HEIGHT - 8 * Globals.TILE_SIZE) / 2,
                8, 8, sprites, _random, 7, _sparkleEffect, _explosionEffect);

            _transitionAlpha.Alpha = 0f;

            _game = game;
        }

        public override void Enter(params object[] args)
        {
            // Reset Transition Alpha.
            _transitionAlpha.Alpha = 0f;
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard, MouseInfo mouse)
        {
            _mouse = mouse;

            _colorTimer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            _tweener.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                _game.Exit();
            }

            if (!_pauseInput)
            {
                if (keyboard.WasKeyJustPressed(Keys.Up) || keyboard.WasKeyJustPressed(Keys.Down))
                {
                    _currentMenuItem = _currentMenuItem == 0 ? 1 : 0;
                    // play select sound
                    _selectSound.Play();
                }

                // hover mouse on menu items
                if (MouseOver(mouse.Position, new Rectangle(START_BUTTON_X, START_BUTTON_Y, START_BUTTON_WIDTH, MENU_HEIGHT)))
                {
                    _currentMenuItem = 0;
                }

                if (MouseOver(mouse.Position, new Rectangle(QUIT_BUTTON_X, QUIT_BUTTON_Y, QUIT_BUTTON_WIDTH, MENU_HEIGHT)))
                {
                    _currentMenuItem = 1;
                }

                if (mouse.WasButtonJustPressed(MouseButton.Left))
                {
                    if (MouseOver(mouse.Position, new Rectangle(START_BUTTON_X, START_BUTTON_Y, 123, 43)))
                    {
                        _tweener.TweenTo(target: _transitionAlpha, expression: Alpha => Alpha.Alpha, toValue: 1f, duration: 1f)
                            .Easing(EasingFunctions.Linear).OnEnd(_ =>
                            {
                                _colorTimer.Clear();
                                _stateMachine.ChangeState("begin-game", 1);
                            });
                    }
                    else if (MouseOver(mouse.Position, new Rectangle(QUIT_BUTTON_X, QUIT_BUTTON_Y, 220, 43)))
                    {
                        _game.Exit();
                    }
                }


                if (keyboard.WasKeyJustPressed(Keys.Enter))
                {
                    if (_currentMenuItem == 0)
                    {
                        _tweener.TweenTo(target: _transitionAlpha, expression: Alpha => Alpha.Alpha, toValue: 1f, duration: 1f)
                            .Easing(EasingFunctions.Linear).OnEnd(_ =>
                            {
                                _colorTimer.Clear();
                                _stateMachine.ChangeState("begin-game", 1);
                            });
                        
                    }
                    else
                    {
                        _game.Exit();
                    }
                }
            }
        }

        private bool MouseOver(Point mousePosition, Rectangle bounds)
        {
            return bounds.Contains(mousePosition);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _showBoard.Draw(spriteBatch);

            // Keep background and board dark with a semi-transparent rectangle
            spriteBatch.Draw(_pixel, new Rectangle(0, 0, Globals.VIRTUAL_WIDTH, Globals.VIRTUAL_HEIGHT), Color.Black * 0.5f);

            // Draw semi transparent background rectangle
            spriteBatch.Draw(_pixel, new Rectangle(Globals.VIRTUAL_WIDTH / 2 - 76, Globals.VIRTUAL_HEIGHT / 2 - 71, 150, 58), Color.White * 0.5f);

            // Draw semi transparent background rectangle
            spriteBatch.Draw(_pixel, new Rectangle(Globals.VIRTUAL_WIDTH / 2 - 76, Globals.VIRTUAL_HEIGHT / 2 + 1, 150, 58), Color.White * 0.5f);

            // Draw transition rectangle
            spriteBatch.Draw(_pixel, new Rectangle(0, 0, Globals.VIRTUAL_WIDTH, Globals.VIRTUAL_HEIGHT), Color.White * _transitionAlpha.Alpha);
        }

        public override void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight)
        {
            DrawMatch3Text(spriteBatch, viewportWidth, viewportHeight, -150);
            DrawOptionsText(spriteBatch, viewportWidth, viewportHeight, 30);
        }

        private void DrawMatch3Text(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight, int y)
        {
            string match3Text = "MATCH 3";

            // Draw Match 3 Shadows
            DrawTextShadow(spriteBatch, viewportWidth, viewportHeight, match3Text, viewportHeight / 2 + y, _largeFont);

            // Print Match 3 Letters with color transition
            for (int i = 0; i < _letterTable.Count; i++)
            {
                string letter = _letterTable[i].Item1;
                float textWidth = _largeFont.MeasureString(letter).X;
                float letterX = ((_letterTable[i].Item2 * 2.5f) + viewportWidth - textWidth) / 2;

                spriteBatch.DrawString(_largeFont, letter, new Vector2(letterX, viewportHeight / 2 + y), _colors[i]);
            }
        }

        private void DrawOptionsText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight, int y)
        {
            DrawTextShadow(spriteBatch, viewportWidth, viewportHeight, "START", viewportHeight / 2 + y, _mediumFont);
            float startTextHeight = _mediumFont.MeasureString("START").Y;
            float startTextWidth = _mediumFont.MeasureString("START").X;

            Vector2 startPosition = new Vector2((viewportWidth - startTextWidth) / 2, viewportHeight / 2 + y);

            if (_currentMenuItem == 0)
            {
                spriteBatch.DrawString(_mediumFont, "START", startPosition, new Color(99, 155, 255));
            }
            else
            {
                spriteBatch.DrawString(_mediumFont, "START", startPosition, new Color(48, 96, 130));
            }

            DrawTextShadow(spriteBatch, viewportWidth, viewportHeight, "Quit Game", viewportHeight / 2 + y + 66, _mediumFont);


            float quitTextHeight = _mediumFont.MeasureString("Quit Game").Y;
            float quitTextWidth = _mediumFont.MeasureString("Quit Game").X;
            Vector2 quitPosition = new Vector2((viewportWidth - quitTextWidth) / 2, viewportHeight / 2 + y + 66);


            if (_currentMenuItem == 1)
            {
                spriteBatch.DrawString(_mediumFont, "Quit Game", quitPosition, new Color(99, 155, 255));
            }
            else
            {
                spriteBatch.DrawString(_mediumFont, "Quit Game", quitPosition, new Color(48, 96, 130));
            }

            spriteBatch.DrawString(_mediumFont, _mouse.Position.ToString(), new Vector2(10, 10), Color.White);
        }

        private void DrawTextShadow(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight, string text, int y, SpriteFont font)
        {
            float textWidth = font.MeasureString(text).X;
            spriteBatch.DrawString(font, text, new Vector2(2 + (viewportWidth - textWidth) / 2, y + 1), new Color(34, 32, 52));
            spriteBatch.DrawString(font, text, new Vector2(1 + (viewportWidth - textWidth) / 2, y + 1), new Color(34, 32, 52));
            spriteBatch.DrawString(font, text, new Vector2((viewportWidth - textWidth) / 2, y + 1), new Color(34, 32, 52));
            spriteBatch.DrawString(font, text, new Vector2(1 + (viewportWidth - textWidth) / 2, y + 2), new Color(34, 32, 52));
        }
    }
}
