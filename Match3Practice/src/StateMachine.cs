using Match3Practice.Input;
using Match3Practice.src.states;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src
{
    public class StateMachine
    {     
        private State CurrentState;

        public Dictionary<string, State> States { get; set; }

        public void ChangeState(string state, params object[] args) 
        {
            CurrentState?.Exit();
            CurrentState = States[state];
            CurrentState?.Enter(args);
        }

        public void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            CurrentState.Update(gameTime, keyboard);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentState.Draw(spriteBatch);
        }

        public void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight)
        {
            CurrentState.DrawText(spriteBatch, viewportWidth, viewportHeight);
        }

        public void DrawForeground(SpriteBatch spriteBatch)
        {
            CurrentState.DrawForeground(spriteBatch);
        }

    }
}
