using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProjectTron
{
    public class Tron : Game
    {
        public static int NumberOfPlayers = 1;
        public static GameObject thisRider;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont text;
        static public List<GameObject> gameObjects = new List<GameObject>();
        static public List<GameObject> reverseObjects = new List<GameObject>();
        static public List<GameObject> newObjects = new List<GameObject>();
        static public List<GameObject> removeObjects = new List<GameObject>();
        private Vector2 screen = new Vector2(800, 600); //-----------------------------Change game res here!
        static public Texture2D ct;
        static public bool gameOver;

        public Tron()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = (int)screen.X;
            graphics.PreferredBackBufferHeight = (int)screen.Y;
            graphics.ApplyChanges();
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            text = Content.Load<SpriteFont>("File");

            Texture2D t1 = Content.Load<Texture2D>("RiderHorizontal");
            Texture2D t2 = Content.Load<Texture2D>("RiderVertical");
            ct = Content.Load<Texture2D>("CollisionTexture");

            thisRider = new Rider(t1, t2, 75, true, new Vector2(50, 50), new Vector2(1, 0), Color.Blue);
            gameObjects.Add(thisRider);
            gameObjects.Add(new Rider(t1, t2, 75, false, new Vector2(600, 50), new Vector2(-1, 0), Color.Green));

            //Content.Load<Texture2D>("Rider");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var item in gameObjects)
            {
                item.Update(gameTime);
            }
            foreach (var item in gameObjects)
            {
                foreach (var other in gameObjects)
                {
                    if (item != other) item.CheckCollision(other);
                }
            }
            if (gameOver) GameOverLogic();
            HandleNewObjects();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        private void HandleNewObjects()
        {
            bool change = false;
            foreach (var item in newObjects)
            {
                gameObjects.Add(item);
                change = true;
            }
            foreach (var item in removeObjects)
            {
                gameObjects.Remove(item);
                change = true;
            }
            newObjects.Clear();
            removeObjects.Clear();
            if (change)
            {
                reverseObjects.Clear();
                reverseObjects = new List<GameObject>(gameObjects);

                reverseObjects.Reverse();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            
            foreach (var item in reverseObjects)
            {
                item.Draw(spriteBatch);
            }
            if (gameOver) GameOverText();
            // TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void GameOverText()
        {
            string s = "GameOver!";
            Vector2 c = text.MeasureString(s);
            spriteBatch.DrawString(text, s, new Vector2(screen.X / 2 - c.X / 2, screen.Y / 2), Color.Red, 0, Vector2.Zero, 1f, 0, 0);
            s = "Press 'R' to play again";
            c = text.MeasureString(s);
            spriteBatch.DrawString(text, s, new Vector2(screen.X / 2 - ((c.X / 2) * 0.66f), screen.Y / 2 + 40), Color.Red, 0, Vector2.Zero, 0.66f, 0, 0);
        }
        private void GameOverLogic()
        {
            KeyboardState k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.R))
            {
                foreach (var item in gameObjects)
                {
                    item.Reset();
                }
                gameOver = false;
            }
        }
        private void SendPlayerData()
        {
            var data = new UpdatePlayer()
            {
                otherPlayerDir = Tron.thisRider.GetDir(),
                otherPlayerPos = Tron.thisRider.GetPos()
            };
            Networking.SendMsg(data, MessageType.update);
        }
        private void SendTrailData()
        {
            var data = new UpdateTrail()
            {
                
            };
            Networking.SendMsg(data, MessageType.update);
        }
    }
}
