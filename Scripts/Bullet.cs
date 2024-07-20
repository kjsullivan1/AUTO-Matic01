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
        //float moveSpeed;
        Vector2 maxSpeed;
        Texture2D bulletTexture;
        int width = 14;
        int height = 14;
        public Rectangle rect;
        bool shootX = true;
        bool shootY = false;
        public Vector2 bulletSpeed;
        float angle;

        Vector2 startPos;
        float travelDist;
        public bool delete = false;


        public Bullet(Vector2 pos, float speed, Vector2 maxSpeed, ContentManager content, bool isX, float travelDist,
            bool isY = false, float speedY = 0, float angle = 0, int size = 14)
        {
            position = pos;
            startPos = pos;
            this.travelDist = travelDist;
            this.maxSpeed = maxSpeed;
            bulletTexture = content.Load<Texture2D>("Textures/Button");
            rect = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            shootX = isX;
            shootY = isY;
            bulletSpeed = new Vector2(speed, speedY);
            this.angle = angle;
            width = size;
            height = size;
        }

        public void Update()
        {
            if(shootX && shootY)
            {
                if(angle != 0)
                {
                    velocity += bulletSpeed * angle;
                }
                else
                {
                    velocity += bulletSpeed;
                }
                
            }
            else if (shootX)
            {
                velocity.X += bulletSpeed.X;
            }
            else if(shootY)
            {
                velocity.Y += bulletSpeed.Y;
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
            spriteBatch.Draw(bulletTexture, new Rectangle((int)position.X, (int)position.Y, width, height), Color.FloralWhite);
        }

        public float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }
    }
}
