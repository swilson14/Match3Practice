using Match3Practice.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private int _timer;


        public PlayState(StateMachine stateMachine) : base(stateMachine)
        {
            _transitionAlpha = new TransitionAlpha(1f);
            _highlightedTileX = 0;
            _highlightedTileY = 0;
            _highlightedTile = false;
            _canInput = true;
            _score = 0;
            _timer = 60;
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
