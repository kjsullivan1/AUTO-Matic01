using AUTO_Matic.Scripts.Effects;
using AUTO_Matic.SideScroll;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic.Scripts
{
    class Bomb
    {
        public Circle circle;
        Vector2 velocity;
        float terminalVel = 20;
        float maxSpeedX = 10;
        bool slow = false;
        float moveSpeed;
        Vector2 position;
        ParticleManager particles;
        ContentManager content;
        public bool delete = false;
        public Bomb(Circle circle, float throwSpeedX, float launchSpeed, ContentManager content)
        {
            this.circle = circle;
            moveSpeed = throwSpeedX;
            velocity.Y = launchSpeed;
            position = new Vector2(circle.Bounds.X, circle.Bounds.Y);
            particles = new ParticleManager();
            particles.Initialize(content.Load<Texture2D>("Textures/white"));
            if(throwSpeedX < 0)
            {
                maxSpeedX = -maxSpeedX;
            }
            this.content = content;
        }

        public void Update(GameTime gameTime, Vector2 gravity, List<SSEnemy> enemies)
        {
            if (!slow)
                velocity += new Vector2(moveSpeed, gravity.Y);
            else if (velocity.X < 0 && slow)
            {
                velocity += new Vector2(.175f, gravity.Y);
                if (velocity.X >= 0)
                    velocity.X = 0;
            }
            else if (velocity.X > 0 && slow)
            {
                velocity += new Vector2(-.175f, gravity.Y);
                if (velocity.X <= 0)
                    velocity.X = 0;
            }
            else
            {
                velocity.Y += gravity.Y;
            }
               

            if (velocity.X > maxSpeedX && velocity.X > 0)
            {
                velocity.X = maxSpeedX;
                slow = true;
            }    
            else if (velocity.X < maxSpeedX && velocity.X < 0)
            {
                velocity.X = maxSpeedX;
                slow = true;
            }
               

            if (velocity.Y > terminalVel)
                velocity.Y = terminalVel;

            position += velocity;

            circle = new Circle(position, circle.Radius);

            SideScrollCollision(enemies);

        }

        private void SideScrollCollision(List<SSEnemy> enemies)
        {
            foreach (WallTile tile in SideTileMap.WallTiles)
            {
                if (circle.Intersects(tile.Rectangle))
                    delete = true;

            }
            foreach (GroundTile tile2 in SideTileMap.GroundTiles)
            {
                if (circle.Intersects(tile2.Rectangle))
                    delete = true;
            }
            foreach (TopDoorTile topDoorTile in SideTileMap.TopDoorTiles)
            {
                if (circle.Intersects(topDoorTile.Rectangle))
                    delete = true;
            }
            foreach (BottomDoorTile bottom in SideTileMap.BottomDoorTiles)
            {
                if (circle.Intersects(bottom.Rectangle))
                    delete = true;
            }

            foreach (PlatformTile tile1 in SideTileMap.PlatformTiles)
            {
                if (circle.Intersects(tile1.Rectangle))
                    delete = true;
            }
            foreach (SSEnemy enemy in enemies)
            {
                if (circle.Intersects(enemy.enemyRect))
                    delete = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(content.Load<Texture2D>("Textures/white"), circle.Bounds, Color.White);
        }
    }

   
}
