using Match3Practice.src;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Match3Practice
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private BoxingViewportAdapter _viewportAdapter;
        private OrthographicCamera _camera;
        private StateMachine _stateMachine;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Window Size
            _graphics.PreferredBackBufferWidth = Globals.WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = Globals.WINDOW_HEIGHT;
            _graphics.ApplyChanges();

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, Globals.VIRTUAL_WIDTH, Globals.VIRTUAL_HEIGHT);
            _camera = new OrthographicCamera(_viewportAdapter);

            _stateMachine = new StateMachine();
            // Add states and then change to the initial state

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Get the camera's transformation matrix
            Matrix transformMatrix = _camera.GetViewMatrix();

            // Apply the transformation to the sprite batch
            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            // Draw graphics
            _stateMachine.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}
