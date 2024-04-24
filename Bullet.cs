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



        public Bullet(Vector2 pos, float speed, Vector2 maxSpeed, ContentManager content)
        {
            position = pos;
            moveSpeed = speed;
            this.maxSpeed = maxSpeed;
            bulletTexture = content.Load<Texture2D>("Tile4");
        }

        public void Update()
        {
            velocity.X += moveSpeed;
            position += velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletTexture, new Rectangle((int)position.X, (int)position.Y, width, height), Color.White);
        }
    }
}
