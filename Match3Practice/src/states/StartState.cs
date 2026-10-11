using Match3Practice.Input;
using Match3Practice.src.graphics;
using Match3Practice.src.states;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Match3Practice.src.states
{
    public class StartState : State
    {
        private int _currentMenuItem = 0;
        private List<Color> _colors;
        private Dictionary<string, float> _letterTable;
        private Timer _colorTimer = new Timer();

        private TransitionAlpha _transitionAlpha = new TransitionAlpha();
        private bool _pauseInput = false;

        private Board _showBoard;

        private Random _random;

        private SoundEffect _selectSound;

        private Tweener _tweener;

        private Texture2D _pixel;

        private SpriteFont _smallFont;
        private SpriteFont _mediumFont;
        private SpriteFont _largeFont;

        public StartState(StateMachine stateMachine, Dictionary<string, Sprite> sprites, Random random,
            Dictionary<string, SoundEffect> soundEffects, Tweener tweener, Texture2D pixel, Dictionary<string, SpriteFont> fonts) : base(stateMachine)
        {

            _pixel = pixel;
            _largeFont = fonts["large"];    

            _colors = new List<Color>
            {
              new Color(217, 87, 99),
              new Color(95, 205, 228),
              new Color(251, 242, 54),
              new Color(118, 66, 138),
              new Color(153, 229, 80),
              new Color(223, 113, 38),
            };

            _letterTable = new Dictionary<string, float>
            {
                { "M", -108f },
                { "A", -64f },
                { "T", -28f },
                { "C", 2f },
                { "H", 40f },
                { "3", 112f },
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
                8,8, sprites, _random);

            _transitionAlpha.Alpha = 0f;
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            _colorTimer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            _tweener.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.WasKeyJustPressed(Keys.Escape))
            {
                Exit();
            }

            if (!_pauseInput)
            {
                if (keyboard.WasKeyJustPressed(Keys.Up) || keyboard.WasKeyJustPressed(Keys.Down))
                {
                    _currentMenuItem = _currentMenuItem == 0 ? 1 : 0;
                    // play select sound
                    _selectSound.Play();
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
                        Exit();
                    }
                }
            }
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
            //DrawOptionsText();
        }

        private void DrawMatch3Text(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight, int y)
        {
            string match3Text = "MATCH 3";
            // Draw Match 3 Shadows
            DrawTextShadow(spriteBatch, viewportWidth, viewportHeight, match3Text, viewportHeight / 2 + y);

            // Print Match 3 Letters with color transition

        }

        private void DrawOptionsText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight, int y)
        {

        }

        private void DrawTextShadow(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight, string text, int y)
        {
            float textWidth = _largeFont.MeasureString(text).X;
            spriteBatch.DrawString(_largeFont, text, new Vector2(2 + (viewportWidth - textWidth) / 2, y + 1), new Color(34, 32, 52));
            spriteBatch.DrawString(_largeFont, text, new Vector2(1 + (viewportWidth - textWidth) / 2, y + 1), new Color(34, 32, 52));
            spriteBatch.DrawString(_largeFont, text, new Vector2((viewportWidth - textWidth) / 2, y + 1), new Color(34, 32, 52));
            spriteBatch.DrawString(_largeFont, text, new Vector2(1 + (viewportWidth - textWidth) / 2, y + 2), new Color(34, 32, 52));
        }



    }
}
