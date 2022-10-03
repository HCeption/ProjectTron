using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ProjectTron
{
    public class Tron : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public List<GameObject> gameObjects = new List<GameObject>();
        static public Texture2D ct;

        public Tron()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameObjects.Add(new Rider(Content.Load<Texture2D>("RiderHorizontal"), Content.Load<Texture2D>("RiderVertical"), 75,true));
            gameObjects.Add(new Rider(Content.Load<Texture2D>("RiderHorizontal"), Content.Load<Texture2D>("RiderVertical"), 75, false));
            ct = Content.Load<Texture2D>("CollisionTexture");
            //Content.Load<Texture2D>("Rider");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            foreach(var item in gameObjects)
            {
                item.Update(gameTime);
            }
            foreach (var item in gameObjects)
            {
                foreach (var other in gameObjects)
                {
                    if(item!=other)item.CheckCollision(other);
                }
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            foreach (var item in gameObjects)
            {
                item.Draw(spriteBatch);
            }
            // TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
