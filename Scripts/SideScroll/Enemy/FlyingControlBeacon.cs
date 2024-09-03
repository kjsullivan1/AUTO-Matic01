using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.SideScroll.Enemy;
using AUTO_Matic.Scripts;
using AUTO_Matic.SideScroll;
using AUTO_Matic;
using AUTO_Matic.Scripts.Effects;

namespace AUTO_Matic.Scripts.SideScroll.Enemy
{
    class FlyingControlBeacon
    {
        public Rectangle rect;

        List<Rectangle> coverArea = new List<Rectangle>();
        float health = 10;
        public float MaxHealth;
        public List<FlyingEnemy> controlledEnemies = new List<FlyingEnemy>();
        public int coverRange = 15;
        //int numEnemies;
        Random rand = new Random();
        ContentManager content;
        ParticleManager particles;
       
        public float Health
        {
            get { return health; }
            set { health = value; }
        }


        public FlyingControlBeacon(Rectangle rect, ContentManager content)
        {
            this.rect = rect;
            this.content = content;
            particles = new ParticleManager();
            MaxHealth = health;
            CreateCoverArea(coverRange, rect.Width);
            
            GatherEnemies();

        }

        private void GatherEnemies()
        {
            int numEnemies = rand.Next(2, 4);
            for (int i = 0; i < numEnemies; i++)
            {

                Vector2 pos = Vector2.One;
                bool unpicked = true;
                while (unpicked)
                {
                    int pickedSpot = rand.Next(coverArea.Count);

                    foreach (GroundTile tile in SideTileMap.GroundTiles)
                    {
                        if (tile.Rectangle.Contains(coverArea[pickedSpot])
                            && SideTileMap.enemySpawns.Contains(new Vector2(tile.Rectangle.X, tile.Rectangle.Y - 64)) == false
                            && tile.Rectangle.Y - 64 > 0)
                        {
                            unpicked = false;
                            pos = new Vector2(tile.Rectangle.X, tile.Rectangle.Y - 128);
                        }
                    }

                    foreach (PlatformTile tile in SideTileMap.PlatformTiles)
                    {
                        if (tile.Rectangle.Contains(coverArea[pickedSpot]) 
                            && SideTileMap.enemySpawns.Contains(new Vector2(tile.Rectangle.X, tile.Rectangle.Y - 64)) == false)
                        {
                            unpicked = false;
                            pos = new Vector2(tile.Rectangle.X, tile.Rectangle.Y - 128);
                        }
                    }



                }

                controlledEnemies.Add(new FlyingEnemy(content, 20, pos, this));
            }
        }

        private void CreateCoverArea(int coverAreaLength, int pixelSize)
        {
            coverArea.Clear();
            Vector2 pos = new Vector2((int)rect.X, (int)(rect.Y));
            #region Full coverArea around enemy

            for (int i = 1; i < coverAreaLength + 1; i++)
            {
                coverArea.Add(new Rectangle((int)pos.X, (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));//Up
                coverArea.Add(new Rectangle((int)pos.X, (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));//Down
            }

            //coverArea.Add(new Rectangle((int)pos.X, (int)pos.Y - (pixelSize * 1), pixelSize, pixelSize));//Up
            for (int i = 1; i < coverAreaLength + 1; i++)//Right
            {
                coverArea.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y, pixelSize, pixelSize));
                for (int j = i + 1; j < coverAreaLength + 1; j++) //Right and Up
                {
                    coverArea.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y - (pixelSize * j), pixelSize, pixelSize));
                }
                for (int k = i + 1; k < coverAreaLength + 1; k++)//Right and down
                {
                    coverArea.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y + (pixelSize * k), pixelSize, pixelSize));
                }
            }

            for (int i = 1; i < coverAreaLength + 1; i++)//Left
            {
                coverArea.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y, pixelSize, pixelSize));
                for (int j = i + 1; j < coverAreaLength + 1; j++) //Left and Up
                {
                    coverArea.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y - (pixelSize * j), pixelSize, pixelSize));
                }
                for (int k = i + 1; k < coverAreaLength + 1; k++)//Left and down
                {
                    coverArea.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y + (pixelSize * k), pixelSize, pixelSize));
                }
            }

            for (int i = 1; i < coverAreaLength + 1; i++) //Up and down
            {
                //coverArea.Add(new Rectangle(pos.X + (int)(rectangle.Width / 3.5f), rect.Top + (rect.Height * 1), rect.Width / 2, pixelSize / 4));
                for (int j = i; j < coverAreaLength + 1; j++) //Left and up 
                {
                    coverArea.Add(new Rectangle((int)pos.X - (pixelSize * j), (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));
                }
                for (int j = i; j < coverAreaLength + 1; j++) //Right and Up
                {
                    coverArea.Add(new Rectangle((int)pos.X + (pixelSize * j), (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));
                }
                //coverArea.Add(new Rectangle((int)pos.X, (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));//Down
                for (int k = i; k < coverAreaLength + 1; k++)//Left and down
                {
                    coverArea.Add(new Rectangle((int)pos.X - (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                }
                for (int k = i; k < coverAreaLength + 1; k++)//Right and down
                {
                    coverArea.Add(new Rectangle((int)pos.X + (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                }

                //if (enemyState != EnemyStates.Jumping && prevState != EnemyStates.Jumping)
                //{
                //    for (int k = i; k < coverAreaLength + 1; k++)//Left and down
                //    {
                //        coverArea.Add(new Rectangle((int)pos.X - (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                //    }
                //    for (int k = i; k < coverAreaLength + 1; k++)//Right and down
                //    {
                //        coverArea.Add(new Rectangle((int)pos.X + (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                //    }
                //}


            }
            //coverArea.Add(new Rectangle(rect.X + (int)(rect.Width/3.5f), rect.Top + (rect.Height * 1), rect.Width/2, pixelSize/4));
            #endregion
        }

        public bool InRange(Rectangle rect)
        {
            bool inRange = false;
            foreach(Rectangle vis in coverArea)
            {
                if(vis.Intersects(rect))
                {
                    inRange = true;
                    break;
                }
            }
            return inRange;
        }

        public void Update(GameTime gameTime, Vector2 gravity, SSPlayer ssPlayer, Rectangle currBounds)
        {
            for (int i = ssPlayer.bullets.Count - 1; i >= 0; i--)
            {
                if (ssPlayer.bullets[i].rect.Intersects(rect))
                {
                    
                    Health -= ssPlayer.bulletDmg;
                    ssPlayer.bullets[i].delete = true;
                }
            }

            for (int i = controlledEnemies.Count - 1; i >= 0; i--)
            {



                for (int j = i - 1; j >= 0; j--)
                {
                    if (controlledEnemies[i].enemyRect.Intersects(controlledEnemies[j].enemyRect))
                    {
                        controlledEnemies[i].moveSpeed = controlledEnemies[j].moveSpeed / 4;
                    }
                    else
                    {
                        controlledEnemies[i].moveSpeed = controlledEnemies[j].moveSpeed;
                    }
                }
                controlledEnemies[i].Update(gameTime, gravity, ssPlayer, SideTileMap.tileMap, currBounds);



                if (controlledEnemies[i].delete)
                {
                    particles.MakeExplosion(controlledEnemies[i].enemyRect, 
                        new Circle(new Vector2(controlledEnemies[i].enemyRect.X - 32, controlledEnemies[i].enemyRect.Y - 32), 48), controlledEnemies[i].enemyRect.Width);
                    controlledEnemies.RemoveAt(i);
                }
               
            }

            if(Health <= 0)
            {
                for(int i = 0; i < controlledEnemies.Count; i++)
                {
                    controlledEnemies[i].health = 0;
                }

                particles.MakeExplosion(rect, new Circle(new Vector2(rect.X - 14, rect.Y - 14), 39), rect.Width);
            }

            particles.Update(gameTime);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i < controlledEnemies.Count; i++)
            {
                controlledEnemies[i].Draw(spriteBatch);
            }

            particles.Draw(spriteBatch);
        }

    }
}
