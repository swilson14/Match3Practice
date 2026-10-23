using Match3Practice.Input;
using Match3Practice.src.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src.states
{
    public class BeginGameState : State
    {

        private TransitionAlpha _transitionAlpha;

        private Board _board;

        public float LevelLabelY;

        private int _level;

        private Tweener _tweener;

        private Texture2D _pixel;

        private SpriteFont _largeFont;

        GraphicsDevice _graphicsDevice;


        public BeginGameState(StateMachine stateMachine, Dictionary<string, Sprite> tileSprites,
            Random random, Texture2D pixel, Dictionary<string, SpriteFont> fonts, GraphicsDevice graphicsDevice) : base(stateMachine)
        {
            _transitionAlpha = new TransitionAlpha();
            _transitionAlpha.Alpha = 1f;
            _board = new Board(Globals.VIRTUAL_WIDTH - 272, 16, 8, 8, tileSprites, random);
            LevelLabelY = -160;
            _tweener = new Tweener();
            _pixel = pixel;
            _largeFont = fonts["large"];
            _stateMachine = stateMachine;
            _graphicsDevice = graphicsDevice;
        }

        public override void Enter(params object[] args)
        {
            _level = (int)args[0];

            _tweener.TweenTo(
                target: _transitionAlpha,
                expression: alpha => alpha.Alpha,
                toValue: 0f,
                duration: 1f
            )
            .Easing(EasingFunctions.Linear)
            .OnEnd(_ =>
            {
                _tweener.TweenTo(
                    target: this,
                    expression: state => state.LevelLabelY,
                    toValue: (_graphicsDevice.Viewport.Height / 2 - 24),
                    duration: 0.25f
                )
                .Easing(EasingFunctions.Linear)
                .OnEnd(_ =>
                {
                    _tweener.TweenTo(
                        target: this,
                        expression: state => state.LevelLabelY,
                        toValue: (_graphicsDevice.Viewport.Height + 75),
                        duration: 0.25f,
                        delay: 1f
                    )
                    .Easing(EasingFunctions.Linear)
                    .OnEnd(_ =>
                    {
                        _stateMachine.ChangeState("play", _level, _board);
                    });
                });
            });
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            _tweener.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _board.Draw(spriteBatch);
            
        }

        public override void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight)
        {
            string levelText = $"Level {_level}";
            float levelTextWidth = _largeFont.MeasureString(levelText).X;
            spriteBatch.Draw(_pixel, new Rectangle(0, (int)LevelLabelY - 24, viewportWidth, 120), new Color(95, 205, 228, 200));
            spriteBatch.DrawString(_largeFont, levelText, new Vector2((viewportWidth - levelTextWidth) / 2, LevelLabelY), Color.White);
        }
        
        public override void DrawForeground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_pixel, new Rectangle(0,0, Globals.VIRTUAL_WIDTH, Globals.VIRTUAL_HEIGHT), Color.White * _transitionAlpha.Alpha);
        }

    }
}
