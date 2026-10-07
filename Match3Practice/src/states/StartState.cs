using Match3Practice.Input;
using Match3Practice.src.graphics;
using Match3Practice.src.States;
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

        private StateMachine _stateMachine;


        public StartState(StateMachine stateMachine, Dictionary<string, Sprite> sprites, Random random,
            Dictionary<string, SoundEffect> soundEffects, Tweener tweener) : base(stateMachine)
        {
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
            _showBoard = new Board(0,0,8,8, sprites, _random);

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
            throw new NotImplementedException();
        }

    }
}
