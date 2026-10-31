using Match3Practice.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
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
        private SoundEffect _selectSound;
        private SoundEffect _errorSound;
        private SoundEffect _matchSound;

        // Fonts
        private SpriteFont _mediumFont;

        // Game / System
        private Game _game;

        // Graphics
        private Texture2D _pixel;


        public PlayState(StateMachine stateMachine, Dictionary<string, SoundEffect> soundEffects, Game game,
            Texture2D pixel, Dictionary<string,SpriteFont> fonts) : base(stateMachine)
        {
            _transitionAlpha = new TransitionAlpha(1f);
            _clockSound = soundEffects["clock"];
            _gameOverSound = soundEffects["game-over"];
            _nextLevelSound = soundEffects["next-level"];
            _errorSound = soundEffects["error"];
            _selectSound = soundEffects["select"];
            _matchSound = soundEffects["match"];
            _boardHighlightX = 0;
            _boardHighlightY = 0;
            _rectHighlighted = false;
            _canInput = true;
            _score = 0;
            _timerCountdown = 60;
            _pixel = pixel;
            _mediumFont = fonts["medium"];

            _timer = new Timer();
            _tweener = new Tweener();
            _game = game;
        }

        public override void Enter(params object[] args)
        {
            _level = (int)args[0];
            _board = (Board)args[1];
            _score = (int)args[2];
            _scoreGoal = _level * 1.25f * 1000;

            // reset timer countdown
            _timerCountdown = 60;

            // Countdown
            _timer.Every(1f, () =>
            {
                _timerCountdown--;
                if (_timerCountdown <= 5)
                {
                    _clockSound.Play();
                }
            });

            _timer.Every(0.5f, () =>
            {
                // Toggle the highlighted tile on and off every half second
                _rectHighlighted = !_rectHighlighted;
            });

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
                    _selectSound.Play();
                }
                else if (keyboard.WasKeyJustPressed(Keys.Down))
                {
                    _boardHighlightY = Math.Min(Globals.BOARD_HEIGHT - 1, _boardHighlightY + 1);
                    _selectSound.Play();
                }
                else if (keyboard.WasKeyJustPressed(Keys.Left))
                {
                    _boardHighlightX = Math.Max(0, _boardHighlightX - 1);
                    _selectSound.Play();
                }
                else if (keyboard.WasKeyJustPressed(Keys.Right))
                {
                    _boardHighlightX = Math.Min(Globals.BOARD_WIDTH - 1, _boardHighlightX + 1);
                    _selectSound.Play();
                }
            }

            if (keyboard.WasKeyJustPressed(Keys.Enter))
            {
                int x = _boardHighlightX;
                int y = _boardHighlightY;

                if (_highlightedTile == null)
                {
                    _highlightedTile = _board.Tiles[x, y];
                }
                else if (_highlightedTile == _board.Tiles[x, y])
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
                    Tile newTile = _board.Tiles[x, y];
                    
                    _highlightedTile.GridX = newTile.GridX;
                    _highlightedTile.GridY = newTile.GridY;

                    newTile.GridX = tempX;
                    newTile.GridY = tempY;

                    _board.Tiles[_highlightedTile.GridX, _highlightedTile.GridY] = _highlightedTile;
                    _board.Tiles[newTile.GridX, newTile.GridY] = newTile;

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
                            CalculateMatches();
                            
                        });
                }
            }
            _tweener.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            _timer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _board.Draw(spriteBatch);

            if (_highlightedTile != null)
            {
                spriteBatch.Draw(_pixel, new Rectangle(_highlightedTile.GridX * Globals.TILE_SIZE + (Globals.VIRTUAL_WIDTH - 272),
                    _highlightedTile.GridY * Globals.TILE_SIZE + 16, Globals.TILE_SIZE, Globals.TILE_SIZE),
                    Color.White * ((float)96/255));
            }

            // Cursor rect
            Color cursorColor = new Color();
            if (_rectHighlighted)
            {
                cursorColor = new Color(217, 87, 99);
            }
            else
            {
                cursorColor = new Color(172, 50, 50);
            }

            // Draw the selected tile highlight
            int lineThickness = 2;

            int offsetX = _boardHighlightX * Globals.TILE_SIZE;
            int offsetY = _boardHighlightY * Globals.TILE_SIZE;

            // Top
            spriteBatch.Draw(_pixel, new Rectangle(_board.X + offsetX, _board.Y + offsetY, Globals.TILE_SIZE, lineThickness), cursorColor);

            // Bottom
            spriteBatch.Draw(_pixel, new Rectangle(_board.X + offsetX, _board.Y + offsetY + Globals.TILE_SIZE - lineThickness, Globals.TILE_SIZE, lineThickness), cursorColor);

            // Left 
            spriteBatch.Draw(_pixel, new Rectangle(_board.X + offsetX, _board.Y + offsetY, lineThickness, Globals.TILE_SIZE), cursorColor);

            // Right
            spriteBatch.Draw(_pixel, new Rectangle(_board.X + offsetX + Globals.TILE_SIZE - lineThickness, _board.Y + offsetY, lineThickness, Globals.TILE_SIZE), cursorColor);

            Color guiColor = Color.FromNonPremultiplied(56, 56, 56, 234);

            spriteBatch.Draw(_pixel, new Rectangle(16, 16, 186, 116), guiColor);
        }

        public override void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight)
        {
            string levelText = $"Level: {_level}";
            float levelTextWidth = _mediumFont.MeasureString(levelText).X;

            string scoreText = $"Score: {_score}";
            float scoreTextWidth = _mediumFont.MeasureString(scoreText).X;

            string goalText = $"Goal: {_scoreGoal}";
            float goalTextWidth = _mediumFont.MeasureString(goalText).X;

            string timerText = $"Timer: {_timerCountdown}";
            float timerTextWidth = _mediumFont.MeasureString(timerText).X;

            Color textColor = new Color(99, 155, 255);
            spriteBatch.DrawString(_mediumFont, levelText, new Vector2((550 - levelTextWidth) / 2, 60), textColor);
            spriteBatch.DrawString(_mediumFont, scoreText, new Vector2((550 - scoreTextWidth) / 2, 130), textColor);
            spriteBatch.DrawString(_mediumFont, goalText, new Vector2((550 - goalTextWidth) / 2, 200), textColor);
            spriteBatch.DrawString(_mediumFont, timerText, new Vector2((550 - timerTextWidth) / 2, 270), textColor);
        }

        public void CalculateMatches()
        {
            _highlightedTile = null;
            _canInput = false;

            List<Match> matches = _board.CalculateMatches();
         
            if (matches != null)
            {
                _matchSound.Play();

                foreach (Match match in matches)
                {
                    foreach(Tile tile in match.Tiles)
                    {
                        _score += 25 * tile.Variety + 50;
                    }

                    _timerCountdown += match.Tiles.Count;
                }

                _board.RemoveMatches();

                // Dictionary of Tiles + Tween Destinations
                Dictionary<Tile, int> tilesToFall = _board.GetFallingTiles();

                // Tween tiles to fall.
                foreach (Tile tile in tilesToFall.Keys)
                {
                    int destY = tilesToFall[tile];

                    _tweener.TweenTo(tile, t => t.Y, destY, 0.25f).OnEnd(_ =>
                        // recursively call, in case there are more matches
                        CalculateMatches()
                    );
                }
            }
            else
            {
                _canInput = true;
            }

        }
    }
}
