using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectTron
{
    public class GameObject
    {
        protected Texture2D sprite;
        protected int speed;
        protected Vector2 direction;
        protected float rotation;
        protected Vector2 origin;
        protected Color color = Color.White;
        protected Vector2 position;
        public virtual Rectangle Collision
        {
            get
            {
                return new Rectangle(
               (int)(position.X - origin.X),
               (int)(position.Y - origin.Y),
               sprite.Width,
               sprite.Height
               );
            }
        }
        public virtual void Update(GameTime gameTime){}
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, color, rotation, origin, 1, SpriteEffects.None, 0);
            DrawCollisionBox(spriteBatch);
        }
        public virtual void OnCollision(GameObject other){}
        public void CheckCollision(GameObject other)
        {
            if (Collision.Intersects(other.Collision))
            {
                OnCollision(other);
            }
        }
        public void DrawCollisionBox(SpriteBatch spriteBatch)
        {


            Rectangle topLine = new Rectangle(Collision.X, Collision.Y, Collision.Width, 1);
            Rectangle bottomLine = new Rectangle(Collision.X, Collision.Y + Collision.Height, Collision.Width, 1);
            Rectangle rightLine = new Rectangle(Collision.X + Collision.Width, Collision.Y, 1, Collision.Height);
            Rectangle leftLine = new Rectangle(Collision.X, Collision.Y, 1, Collision.Height);

            spriteBatch.Draw(Tron.ct, topLine, Color.Red);
            spriteBatch.Draw(Tron.ct, bottomLine, Color.Red);
            spriteBatch.Draw(Tron.ct, rightLine, Color.Red);
            spriteBatch.Draw(Tron.ct, leftLine, Color.Red);
        }
        public virtual void Reset()
        {

        }
    }
}
