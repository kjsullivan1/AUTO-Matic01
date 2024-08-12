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

        List<Rectangle> coverArea;
        float health = 10;
        public List<FlyingEnemy> controlledEnemies;
        public int coverRange = 20;
       
        public float Health
        {
            get { return health; }
            set { health += value; }
        }


        public FlyingControlBeacon(Rectangle rect)
        {
            this.rect = rect;
            CreateCoverArea(coverRange, rect.Width);
            
            GatherEnemies();

        }

        private void GatherEnemies()
        {
            foreach (FlyingEnemy flyingEnemy in SideTileMap.GetFlyingEnemies())
            {
                foreach (Rectangle rectangle in this.coverArea)
                {
                    if (rectangle.Contains(flyingEnemy.enemyRect) && controlledEnemies.Contains(flyingEnemy) == false)
                    {
                        controlledEnemies.Add(flyingEnemy);
                    }
                }
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

        public void Update(SSPlayer ssPlayer)
        {
            for (int i = ssPlayer.bullets.Count - 1; i >= 0; i--)
            {
                if (ssPlayer.bullets[i].rect.Intersects(rect))
                {
                    Health += -ssPlayer.bulletDmg;
                    ssPlayer.bullets.RemoveAt(i);
                }
            }


        }

     
    }
}
