using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectTron
{
    class Rider : GameObject
    {
        Keys oldKey;
        private Vector2 direction = new Vector2(1, 0);
        private Color oColor; //Original color
        private Vector2 oPosition; //Original position (aka spawn position)
        private Vector2 oDirection; //Original direcion
        private Texture2D[] spriteStorage = new Texture2D[2];
        public int speed;
        private float deadTimer = -1f; //Anything over -1 is used for a timer
        public bool dead { get; set; }
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
        }
        public override void Update(GameTime gameTime)
        {
            if (!Tron.gameOver)
            {
                if (allowInputs) HandleInput();
                Move(gameTime);
            }
            if (dead) DeadAnimation(gameTime);
        }
        private void HandleInput()
        {
            KeyboardState kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.W) && oldKey!=Keys.S)
            {
                direction = new Vector2(0, -1);
                oldKey = Keys.W;
                ChangeSprite(direction);
            }
            else if (kState.IsKeyDown(Keys.A) && oldKey != Keys.D)
            {
                direction = new Vector2(-1, 0);
                oldKey = Keys.A;
                ChangeSprite(direction);
            }
            else if (kState.IsKeyDown(Keys.S) && oldKey != Keys.W)
            {
                direction = new Vector2(0, 1);
                oldKey = Keys.S;
                ChangeSprite(direction);
            }
            else if (kState.IsKeyDown(Keys.D) && oldKey != Keys.A)
            {
                direction = new Vector2(1, 0);
                oldKey = Keys.D;
                ChangeSprite(direction);
            }
        }
        private void ChangeSprite(Vector2 index)
        {
            int i=0;
            if (index.X == 0) i = 1;
            sprite = spriteStorage[i];
            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            if (index.X==1 || index.Y==1)
            {
                rotation = (float)((Math.PI/180)*180);
            }
            else rotation = 0;
        }
        private void Move(GameTime gameTime)
        {
            position += direction * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
        }
        public override void OnCollision(GameObject Other)
        {
            dead = true;
            Tron.gameOver = true;
        }
        private void DeadAnimation(GameTime gt)
        {
            if(deadTimer <= 0.25f) //Leeway for instabilities
            {
                color = Color.Red;
                deadTimer = 2f;
            }
            if(deadTimer>=0.75f && deadTimer <= 1.25f)
            {
                color = oColor;
            }
            deadTimer-=(float)gt.ElapsedGameTime.TotalSeconds;
        }
        public override void Reset()
        {
            position = oPosition;
            direction = oDirection;
            ChangeSprite(oDirection);
            oldKey = Keys.G;
            color = oColor;
            dead = false;
        }
    }
}
