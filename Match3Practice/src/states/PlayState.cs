using Match3Practice.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src.states
{
    public class PlayState : State
    {
        private TransitionAlpha _transitionAlpha;
        private bool _highlightedTile;
        private bool _canInput;
        private int _highlightedTileX;
        private int _highlightedTileY;
        private Tile _selectedTile;
        private int _score;
        private int _timerCountdown;
        private Timer _timer;

        private int _level;

        private Board _board;

        private SoundEffect _clockSound;

        private SoundEffect _gameOverSound;

        private float _scoreGoal;

        private Game _game;


        public PlayState(StateMachine stateMachine, Dictionary<string, SoundEffect> soundEffects, Game game) : base(stateMachine)
        {
            _transitionAlpha = new TransitionAlpha(1f);
            _clockSound = soundEffects["clock"];
            _gameOverSound = soundEffects["game-over"];
            _highlightedTileX = 0;
            _highlightedTileY = 0;
            _highlightedTile = false;
            _canInput = true;
            _score = 0;
            _timerCountdown = 60;

            _timer = new Timer();

            _timer.Every(0.5f, () =>
            {
                // Toggle the highlighted tile on and off every half second
                _highlightedTile = !_highlightedTile;
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

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
