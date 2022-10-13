﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace ProjectTron
{
    public class Tron : Game
    {
        public static GameObject thisRider;
        public static GameObject otherRider;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont text;
        private Texture2D[] s = new Texture2D[2];
        static public List<GameObject> gameObjects = new List<GameObject>();
        static public List<GameObject> reverseObjects = new List<GameObject>();
        static public List<GameObject> newObjects = new List<GameObject>();
        static public List<GameObject> removeObjects = new List<GameObject>();
        private Vector2 screen = new Vector2(800, 600); //-----------------------------Change game res here!
        static public Texture2D ct;
        static public bool gameOver;
        private bool setup;
        public static bool gameStart;
        public Network network;

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

            s[0] = Content.Load<Texture2D>("RiderHorizontal");
            s[1] = Content.Load<Texture2D>("RiderVertical");
            ct = Content.Load<Texture2D>("CollisionTexture");



            //Content.Load<Texture2D>("Rider");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (setup && gameStart)
            {
                thisRider.Update(gameTime);
                otherRider.Update(gameTime);
                foreach (var item in gameObjects)
                {
                    item.Update(gameTime);
                }
                foreach (var other in gameObjects)
                {
                    if (thisRider != other) thisRider.CheckCollision(other);
                    if (otherRider != other) otherRider.CheckCollision(other);
                }
                if (gameOver) GameOverLogic();
                HandleNewObjects(null, false);

                network.Update();
            }
            else
            {
                StartMenu();
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        public static void HandleNewObjects(List<GameObject> list, bool fullChange)
        {
            //bool change = false;

            foreach (var item in newObjects)
            {
                gameObjects.Add(item);
                //change = true;
            }
            foreach (var item in removeObjects)
            {
                gameObjects.Remove(item);
                //change = true;
            }
            newObjects.Clear();
            removeObjects.Clear();

            //if (change)
            //{
            //    reverseObjects.Clear();
            //    reverseObjects = new List<GameObject>(gameObjects);

            //    reverseObjects.Reverse();
            //}

            if (fullChange)
            {
                gameObjects = new List<GameObject>(list); //list only contains Trails
                //gameObjects.Insert(0, thisRider); //Need to re-add riders seperately
                //gameObjects.Insert(0, otherRider);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            if (setup)
            {
                foreach (var item in gameObjects)
                {
                    item.Draw(spriteBatch);
                }
                thisRider.Draw(spriteBatch);
                otherRider.Draw(spriteBatch);
            }
            else
            {
                string select = "Press 'H' to Host and 'J' to Join";
                Vector2 c = text.MeasureString(select);
                spriteBatch.DrawString(text, select, new Vector2(screen.X / 2 - c.X / 2, screen.Y / 2), Color.Red, 0, Vector2.Zero, 1f, 0, 0);
            }
            if (gameOver) GameOverText();
            // TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void StartMenu()
        {
            KeyboardState k = Keyboard.GetState();



            if (k.IsKeyDown(Keys.H))
            {
                setup = true;

                //Networking.Receiver();   udkommenteret da man ikke kan starte spillet uden
                thisRider = new Rider(s[0], s[1], 75, true, new Vector2(50, 50), new Vector2(1, 0), Color.Blue);
                otherRider = new Rider(s[0], s[1], 75, false, new Vector2(600, 50), new Vector2(-1, 0), Color.Green);
                network = new Network(true);
            }
            if (k.IsKeyDown(Keys.J))
            {
                setup = true;

                //Networking.SendMsg();    mangler noget i ()
                otherRider = new Rider(s[0], s[1], 75, false, new Vector2(50, 50), new Vector2(1, 0), Color.Blue);
                thisRider = new Rider(s[0], s[1], 75, true, new Vector2(600, 50), new Vector2(-1, 0), Color.Green);
                network = new Network(false);
            }
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
    }
}
