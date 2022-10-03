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
        private Texture2D[] spriteStorage = new Texture2D[2];
        private int speed;
        bool allowInputs;
        public Rider(Texture2D horiontal, Texture2D vertical, int speed, bool allowUserInputs)
        {
            spriteStorage[0] = horiontal;
            spriteStorage[1] = vertical;
            ChangeSprite(0,true);
            this.speed = speed;
            allowInputs = allowUserInputs;
            if (!allowInputs) { direction = new Vector2(0, 0); position = new Vector2(200, 200); }
        }
        public override void Update(GameTime gameTime)
        {
            if(allowInputs)HandleInput();
            Move(gameTime);
            color = Color.White;
        }
        private void HandleInput()
        {
            KeyboardState kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.W) && oldKey!=Keys.S)
            {
                direction = new Vector2(0, -1);
                oldKey = Keys.W;
                ChangeSprite(1,false);
            }
            else if (kState.IsKeyDown(Keys.A) && oldKey != Keys.D)
            {
                direction = new Vector2(-1, 0);
                oldKey = Keys.A;
                ChangeSprite(0,false);
            }
            else if (kState.IsKeyDown(Keys.S) && oldKey != Keys.W)
            {
                direction = new Vector2(0, 1);
                oldKey = Keys.S;
                ChangeSprite(1,true);
            }
            else if (kState.IsKeyDown(Keys.D) && oldKey != Keys.A)
            {
                direction = new Vector2(1, 0);
                oldKey = Keys.D;
                ChangeSprite(0,true);
            }
        }
        private void ChangeSprite(int index, bool needsFlipping)
        {
            sprite = spriteStorage[index];
            origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            if (needsFlipping)
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
            color = Color.Red;
        }
    }
}
