using Match3Practice.Input;
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
    public class PlayState : State
    {
        // Gameplay / State
        private TransitionAlpha _transitionAlpha;
        private bool _rectHighlighted;
        private bool _canInput;
        private int _boardHighlightX;
        private int _boardHighlightY;
        private Tile _highlightedTile;
        private int _score;
        private int _timerCountdown;
        private Timer _timer;
        private int _level;
        private Board _board;
        private float _scoreGoal;

        private Tweener _tweener;

        // Audio
        private SoundEffect _clockSound;
        private SoundEffect _gameOverSound;
        private SoundEffect _nextLevelSound;
        private SoundEffect _playSound;
        private SoundEffect _errorSound;

        // Game / System
        private Game _game;


        public PlayState(StateMachine stateMachine, Dictionary<string, SoundEffect> soundEffects, Game game) : base(stateMachine)
        {
            _transitionAlpha = new TransitionAlpha(1f);
            _clockSound = soundEffects["clock"];
            _gameOverSound = soundEffects["game-over"];
            _nextLevelSound = soundEffects["next-level"];
            _errorSound = soundEffects["error"];
            _boardHighlightX = 0;
            _boardHighlightY = 0;
            _rectHighlighted = false;
            _canInput = true;
            _score = 0;
            _timerCountdown = 60;

            _timer = new Timer();
            _tweener = new Tweener();

            _timer.Every(0.5f, () =>
            {
                // Toggle the highlighted tile on and off every half second
                _rectHighlighted = !_rectHighlighted;
            });

            _timer.Every(1f, () =>
            {
                _timerCountdown--;
                if (_timerCountdown <= 5)
                {
                    _clockSound.Play();
                }
            });
            _game = game;
        }

        public override void Enter(params object[] args)
        {
            _level = (int)args[0];
            _board = (Board)args[1];
            _score = (int)args[2];
            _scoreGoal = _level * 1.25f * 1000;
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                _game.Exit();
            }

            if (_timerCountdown <= 0)
            {
                // Game over, transition to the game-over state with the final score.
                _timer.Clear();
                _gameOverSound.Play();
                _stateMachine.ChangeState("game-over", _score);
            }

            if (_score > _scoreGoal)
            {
                _timer.Clear();
                _nextLevelSound.Play();
                _stateMachine.ChangeState("begin-game", _level + 1, _score);
            }

            if (_canInput)
            {

                if (keyboard.WasKeyJustPressed(Keys.Up))
                {
                    _boardHighlightY = Math.Max(0, _boardHighlightY - 1);
                }
                else if (keyboard.WasKeyJustPressed(Keys.Down))
                {
                    _boardHighlightY = Math.Min(Globals.BOARD_HEIGHT - 1, _boardHighlightY + 1);
                }
                else if (keyboard.WasKeyJustPressed(Keys.Left))
                {
                    _boardHighlightX = Math.Max(0, _boardHighlightX - 1);
                }
                else if (keyboard.WasKeyJustPressed(Keys.Right))
                {
                    _boardHighlightX = Math.Min(Globals.BOARD_WIDTH - 1, _boardHighlightX + 1);
                }
            }

            if (keyboard.WasKeyJustPressed(Keys.Enter))
            {
                _tweener.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                _timer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                int x = _boardHighlightX;
                int y = _boardHighlightY;

                if (_highlightedTile == null)
                {
                    _highlightedTile = _board.Tiles[y, x];
                }
                else if (_highlightedTile == _board.Tiles[y, x])
                {
                    _highlightedTile = null;
                }
                else if (Math.Abs(_highlightedTile.GridX - x) + Math.Abs(_highlightedTile.GridY - y) > 1)
                {
                    _errorSound.Play();
                    _highlightedTile = null;
                }
                else
                {
                    int tempX = _highlightedTile.GridX;
                    int tempY = _highlightedTile.GridY;

                    // Swap the positions of the two tiles
                    Tile newTile = _board.Tiles[y, x];
                    
                    _highlightedTile.GridX = newTile.GridX;
                    _highlightedTile.GridY = newTile.GridY;

                    newTile.GridX = tempX;
                    newTile.GridY = tempY;

                    _board.Tiles[_highlightedTile.GridY, _highlightedTile.GridX] = _highlightedTile;
                    _board.Tiles[newTile.GridY, newTile.GridX] = newTile;

                    _tweener.TweenTo(target: _highlightedTile, expression: tile => tile.X, toValue: newTile.X, duration: 0.1f)
                        .Easing(EasingFunctions.Linear);
                    _tweener.TweenTo(target: _highlightedTile, expression: tile => tile.Y, toValue: newTile.Y, duration: 0.1f)
                        .Easing(EasingFunctions.Linear);
                    _tweener.TweenTo(target: newTile, expression: tile => tile.X, toValue: _highlightedTile.X, duration: 0.1f)
                        .Easing(EasingFunctions.Linear);
                    _tweener.TweenTo(target: newTile, expression: tile => tile.Y, toValue: _highlightedTile.Y, duration: 0.1f)
                        .Easing(EasingFunctions.Linear).OnEnd(_ =>
                        {
                            // Calculate matches.
                        });
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _board.Draw(spriteBatch);
        }
    }
}
