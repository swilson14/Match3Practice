using Match3Practice.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src.states
{
    public class GameOverState : State
    {
        // State
        private int _score;

        // Fonts
        private SpriteFont _smallFont;
        private SpriteFont _mediumFont;
        private SpriteFont _largeFont;

        // Graphics
        private Texture2D _pixel;

        public GameOverState(StateMachine stateMachine, Dictionary<string, SpriteFont> fonts, Texture2D pixel) : base(stateMachine)
        {
            _mediumFont = fonts["medium"];
            _largeFont = fonts["large"];
            _pixel = pixel;
        }

        public override void Enter(params object[] args)
        {
            _score = (int)args[0];
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard, MouseInfo mouse)
        {
            if (keyboard.WasKeyJustPressed(Keys.Enter))
            {
                _stateMachine.ChangeState("start");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_pixel, new Rectangle(Globals.VIRTUAL_WIDTH / 2 - 64, 64, 128, 136), new Color(56, 56, 56, 234));
        }

        public override void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight)
        {
            string gameText = "GAME";
            string overText = "OVER";
            float gameWidth = _largeFont.MeasureString(gameText).X;
            float overWidth = _largeFont.MeasureString(overText).X;
            Color textColor = new Color(99, 155, 255);

            spriteBatch.DrawString(_largeFont, "GAME", new Vector2((viewportWidth - gameWidth) / 2, 160), textColor);
            spriteBatch.DrawString(_largeFont, "OVER", new Vector2((viewportWidth - overWidth) / 2, 240), textColor);

            string yourScoreText = $"Your Score:";
            float yourScoreWidth = _mediumFont.MeasureString(yourScoreText).X;
            string scoreText = _score.ToString();
            float scoreWidth = _mediumFont.MeasureString(scoreText).X;

            spriteBatch.DrawString(_mediumFont, yourScoreText, new Vector2((viewportWidth - yourScoreWidth) / 2, 340), textColor);
            spriteBatch.DrawString(_mediumFont, scoreText, new Vector2((viewportWidth - scoreWidth) / 2, 380), textColor);

            string pressEnterText = "Press Enter";
            float pressEnterWidth = _mediumFont.MeasureString(pressEnterText).X;

            spriteBatch.DrawString(_mediumFont, pressEnterText, new Vector2((viewportWidth - pressEnterWidth) / 2, 450), textColor);
        }

    }
}
