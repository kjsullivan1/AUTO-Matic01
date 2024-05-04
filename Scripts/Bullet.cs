using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AUTO_Matic
{
    class Bullet
    {
        Vector2 position;
        Vector2 velocity;
        float moveSpeed;
        Vector2 maxSpeed;
        Texture2D bulletTexture;
        int width = 14;
        int height = 14;
        public Rectangle rect;
        bool shootX = true;

        Vector2 startPos;
        float travelDist;
        public bool delete = false;

        public Bullet(Vector2 pos, float speed, Vector2 maxSpeed, ContentManager content, bool isX, float travelDist)
        {
            position = pos;
            startPos = pos;
            this.travelDist = travelDist;
            moveSpeed = speed;
            this.maxSpeed = maxSpeed;
            bulletTexture = content.Load<Texture2D>("TopDown/Textures/Player");
            rect = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            shootX = isX;
        }

        public void Update()
        {
            if (shootX)
            {
                velocity.X += moveSpeed;
            }
            else
            {
                velocity.Y += moveSpeed;
            }
           
            position += velocity;

            if(Distance(position, startPos) > travelDist)
            {
                delete = true;
            }
            rect = new Rectangle((int)position.X, (int)position.Y, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletTexture, new Rectangle((int)position.X, (int)position.Y, width, height), Color.White);
        }

        public float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }
    }
}
