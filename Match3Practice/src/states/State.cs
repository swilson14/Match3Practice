using Match3Practice.Input;
using Match3Practice.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src.states
{
    public abstract class State
    {
        protected StateMachine _stateMachine;

        public State (StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public virtual void Enter(params object[] args) { }
        public virtual void Exit() { }

        public abstract void Update(GameTime gameTime, KeyboardInfo keyboard);

        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight) { }
    }
}
