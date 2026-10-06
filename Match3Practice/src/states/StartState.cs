using Match3Practice.Input;
using Match3Practice.src.graphics;
using Match3Practice.src.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private Timer _colorTimer;

        private int _transitionAlpha = 0;
        private bool _pauseInput = false;

        private Board _showBoard;

        private Random _random;

        public StartState(StateMachine stateMachine, Dictionary<string, Sprite> sprites, Random random) : base(stateMachine)
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
                for (int i = 5; i >= 0; i--)
                {
                    _colors[i] = _colors[i - 1];
                }
            });

            _random = random;

            // Generate board of tiles just for show.
            _showBoard = new Board(0,0,8,8, sprites, _random);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            throw new NotImplementedException();
        }
    }
}
