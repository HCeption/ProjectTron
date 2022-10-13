using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectTron
{
    class Trail : GameObject
    {
        Vector2 rectSize = new Vector2(20, 20);
        private Rectangle rect;
        private Rider rider;
        float newSize;
        public Trail(Vector2 pos, Color c, Rider rider, Vector2 direction, int speed, Vector2 r)
        {
            this.speed = speed;
            rider.currentTrail = this;
            this.direction = direction;
            this.rider = rider;
            rect = new Rectangle((int)pos.X, (int)pos.Y, (int)rectSize.X, (int)rectSize.Y);
            position = pos;
            base.sprite = Tron.ct;
            color = c;
            origin = new Vector2(rectSize.X / 2, rectSize.Y / 2);
            newSize = rectSize.X;
            if (r != Vector2.Zero)
            {
                rect.Width = (int)r.X;
                rect.Height = (int)r.Y;
            }
        }
        public override Rectangle Collision
        {
            get
            {
                return new Rectangle(
               (int)(position.X - origin.X),
               (int)(position.Y - origin.Y),
               rect.Width,
               rect.Height
               );
            }
        }
        public override void Update(GameTime gameTime)
        {

        }
        public override void OnCollision(GameObject other)
        {

        }
        public void TrailMove(GameTime gameTime)
        {
            newSize += (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
            float move = (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
            if (direction == new Vector2(1, 0)) //Going right
            {
                rect.Width = (int)newSize;
            }
            else if (direction == new Vector2(-1, 0)) //Going left
            {
                position.X -= move;
                rect.Width = (int)newSize;
            }
            else if (direction == new Vector2(0, 1)) //Going down
            {
                rect.Height = (int)newSize;
            }
            else //Going up
            {
                position.Y -= move;
                rect.Height = (int)newSize;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, rect, color, rotation, origin, 1, SpriteEffects.None, 0);
            base.DrawCollisionBox(spriteBatch);
        }
        public override void Reset()
        {
            rider.currentTrail = null;
            Tron.removeObjects.Add(this);
        }
    }
}
