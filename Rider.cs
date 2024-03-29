﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectTron
{
    class Rider : GameObject
    {
        Keys blockDir; //Prevent user from making 180* turns, max is 90*
        Keys blockKey; //Psuedo method for instantaneous KeyPressed using KeyIsDown and blocking afterwards.
        public Trail currentTrail; //We need to disable collision with last 2 trails
        private Trail oldTrail;
        private Color oColor; //Original color
        private Vector2 oPosition; //Original position (aka spawn position)
        private Vector2 oDirection; //Original direcion
        private static Texture2D[] spriteStorage = new Texture2D[2];
        private float deadTimer;
        public bool dead;
        bool allowInputs;
        public Rider(Texture2D horiontal, Texture2D vertical, int speed, bool allowUserInputs, Vector2 oPosition, Vector2 oDirection, Color oColor)
        {
            spriteStorage[0] = horiontal;
            spriteStorage[1] = vertical;
            ChangeSprite(oDirection);
            this.speed = speed;
            allowInputs = allowUserInputs;
            this.oPosition = oPosition;
            position = oPosition;
            this.oDirection = oDirection;
            direction = oDirection;
            this.oColor = oColor;
            color = oColor;
            NewTrail();
        }
        public override void Update(GameTime gameTime)
        {
            if (!Tron.gameOver)
            {
                if (allowInputs) HandleInput();
                Move(gameTime);
                if (currentTrail == null) NewTrail();
                currentTrail.TrailMove(gameTime);
            }
            if (dead) DeadAnimation(gameTime);
        }
        /// <summary>
        /// Accepts key inputs from local user and calls methods for changes whilst changing variabels.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.Up) && blockDir != Keys.Down && blockKey != Keys.Up)
            {
                direction = new Vector2(0, -1);
                NewTrail(); //Force new trail since were changing direction
                blockDir = Keys.Up;
                blockKey = Keys.Up;
                ChangeSprite(direction);
            }
            else if (kState.IsKeyDown(Keys.Left) && blockDir != Keys.Right && blockKey != Keys.Left)
            {
                direction = new Vector2(-1, 0);
                NewTrail();
                blockDir = Keys.Left;
                blockKey = Keys.Left;
                ChangeSprite(direction);
            }
            else if (kState.IsKeyDown(Keys.Down) && blockDir != Keys.Up && blockKey != Keys.Down)
            {
                direction = new Vector2(0, 1);
                NewTrail();
                blockDir = Keys.Down;
                blockKey = Keys.Down;
                ChangeSprite(direction);
            }
            else if (kState.IsKeyDown(Keys.Right) && blockDir != Keys.Left && blockKey != Keys.Right)
            {
                direction = new Vector2(1, 0);
                NewTrail();
                blockDir = Keys.Right;
                blockKey = Keys.Right;
                ChangeSprite(direction);
            }
            //if (kState.IsKeyUp(blockKey)) blockKey = Keys.G;
        }
        /// <summary>
        /// Changes sprite and sprite mirroring to accomodate movement direction
        /// </summary>
        /// <param name="index"></param>
        private void ChangeSprite(Vector2 index)
        {
            int i = 0;
            if (index.X == 0) i = 1;
            sprite = spriteStorage[i];
            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            if (index.X == 1 || index.Y == 1)
            {
                rotation = (float)((Math.PI / 180) * 180);
            }
            else rotation = 0;
        }
        public override void DirectionChange(Vector2 dir, Vector2 pos)
        {
            position = pos;
            if (dir != direction)
            {
                ChangeSprite(dir);
                direction = dir;
                NewTrail();
            }
        }
        private void Move(GameTime gameTime)
        {
            position += direction * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
        }
        /// <summary>
        /// Simple method called upon collision. Anything related to collision will be here.
        /// </summary>
        /// <param name="Other"></param>
        public override void OnCollision(GameObject Other)
        {
            if (Other != currentTrail)
            {
                if (Other != oldTrail)
                {
                    dead = true;
                    Tron.gameOver = true;
                    Tron.network.SendMsg(new SimpleMsg { }, MessageType.collision, Tron.network.clientEP);
                }
            }
        }
        /// <summary>
        /// Changes sprite color to indicate who died.
        /// </summary>
        /// <param name="gt"></param>
        private void DeadAnimation(GameTime gt)
        {
            if (deadTimer <= 0.25f) //Leeway for instabilities
            {
                color = Color.Red;
                deadTimer = 2f;
            }
            if (deadTimer >= 0.75f && deadTimer <= 1.25f)
            {
                color = oColor;
            }
            deadTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
        }
        /// <summary>
        /// After a player has died the game needs to be reset. All objects will have this called,
        /// wherein they may reset themselves to restart the game.
        /// </summary>
        public override void Reset()
        {
            position = oPosition;
            direction = oDirection;
            ChangeSprite(oDirection);
            blockDir = Keys.G;
            blockKey = Keys.G;
            color = oColor;
            dead = false;
        }
        private void NewTrail()
        {
            oldTrail = currentTrail;
            currentTrail = new Trail(position, oColor, this, direction, speed,Vector2.Zero);
            Tron.newObjects.Add(currentTrail);
        }
        public override void KillRider()
        {
            dead = true;
        }

    }
}
