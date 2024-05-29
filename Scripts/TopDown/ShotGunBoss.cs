using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework.Content;

namespace AUTO_Matic.Scripts.TopDown
{
    class ShotGunBoss
    {
        Rectangle bossRect;
        ContentManager content;
        #region Animations
        AnimationManager animManager;
        Texture2D texture;
        Point FrameSize;//Size of frame
        Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;
        public void ChangeAnimation()
        {
            //switch (animState)
            //{
            //    case AnimationStates.Idle:
            //        texture = content.Load<Texture2D>("TopDown/Animations/PlayerIdle");
            //        FrameSize = new Point(64, 64);
            //        CurrFrame = new Point(0, 0);
            //        SheetSize = new Point(6, 1);
            //        fpms = 120;
            //        break;
            //    case AnimationStates.Walking:
            //        texture = content.Load<Texture2D>("TopDown/Animations/PlayerWalk");
            //        FrameSize = new Point(64, 64);
            //        CurrFrame = new Point(0, 0);
            //        SheetSize = new Point(8, 1);
            //        fpms = 120;
            //        break;
            //    case AnimationStates.Shooting:
            //        texture = content.Load<Texture2D>("TopDown/Animations/PlayerShoot");
            //        FrameSize = new Point(64, 64);
            //        CurrFrame = new Point(0, 0);
            //        SheetSize = new Point(4, 1);
            //        fpms = 95;
            //        break;
            //}

            //bool isRight = true, isLeft = false, isUp = false, isDown = false;
            //if (animManager != null)
            //{
            //    isRight = animManager.isRight;
            //    isLeft = animManager.isLeft;
            //    isUp = animManager.isUp;
            //    isDown = animManager.isDown;
            //}

            //animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, Position);

            //animManager.isRight = isRight;
            //animManager.isLeft = isLeft;
            //animManager.isUp = isUp;
            //animManager.isDown = isDown;
        }
        #endregion

        #region Shooting
        Texture2D gunTexture;
        public List<Bullet> bullets = new List<Bullet>();
        float bulletSpeed = 2.5f;
        float bulletMaxX = 10f;
        float bulletMaxY = 10f;
        int spread = 3;
        bool isShootDelay = false;
        float shootDelay = 1.45f;//In seconds
        float iShootDelay;
        bool startShoot = false;
        public float bulletDmg = .7f;
        public float bulletTravelDist = 64 * 8;
        Texture2D visionTxture;
        int width;
        int height;
        Rectangle bounds;
        Rectangle tempRect;
        public Rectangle worldRect;

        List<WallTiles> BreakableWalls = new List<WallTiles>();
        #endregion

        Circle slam;
        int growthRate = 6;
        int maxSize = 300;
        bool slamWave = false;
        float health = 18f;
        int currWidthMod = 3;
        public bool moveBack;
        float slamDelay = 2.25f;
        float iSlamDelay;
        public float Health
        {
            get { return health; }
            set
            {
                health = value;
                if (health <= 0)
                    health = 0;
            }
        }

        public ShotGunBoss(Rectangle rect, int width, int height, ContentManager content, List<WallTiles> walls, Rectangle bounds, TopDownMap tdMap)
        {
            bossRect = new Rectangle(((rect.X + rect.Width / 2) - 64 / 2), (((rect.Y + rect.Height / 2) - 64 / 2)), 64, 64);
            this.content = content;
            iShootDelay = shootDelay;
            this.width = width;
            this.height = height;
            this.bounds = rect;
            tempRect = bossRect;
            worldRect = new Rectangle(((bounds.X + bounds.Width / 2) - width / 2), (((bounds.Y + bounds.Height / 2) - height / 2)), width, height);

           BreakableWalls = walls;
           slam = new Circle(new Vector2(bossRect.X + bossRect.Width/2, bossRect.Y+ bossRect.Height/2), 2);
            iSlamDelay = slamDelay;

            for(int i = tdMap.WallTiles.Count - 1; i >= 0; i--)
            {
                if(bounds.Intersects(tdMap.WallTiles[i].Rectangle) == false || bounds.Contains(tdMap.WallTiles[i].Rectangle) == false)
                {
                    tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                }
            }
        }

        public void Update(GameTime gameTime, TDPlayer tdPlayer, TopDownMap tdMap)
        {
          

            if(health > 0)
            {
                if (shootDelay >= 0)
                {
                    shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                Vector2 targetDir = new Vector2(tdPlayer.rectangle.X, tdPlayer.rectangle.Y) - new Vector2(bossRect.X, bossRect.Y);
                if (shootDelay <= 0)
                {
                    float angle = Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(targetDir.Y, targetDir.X))); //sub by 90 if problems occur
                    bossRect = new Rectangle(((bounds.X + bounds.Width / 2) - width / 2), (((bounds.Y + bounds.Height / 2) - height / 2)), width, height);
                    if (angle < 18 || angle >= 155)//Right
                    {
                        if (angle < 18)
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height / 2 - 15 / 2), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height / 2 - 15 / 2), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 7));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height / 2 - 15 / 2), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 7));
                        }
                        else
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height / 2 - 15 / 2), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height / 2 - 15 / 2), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 7));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height / 2 - 15 / 2), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 7));
                        }


                    }
                    if (angle >= 18 && angle < 45)
                    {
                        if (tdPlayer.position.Y < bossRect.Y + 64 / 2)
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 4));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 1.75f));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                        }
                        else
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 6));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 2));
                        }
                    }
                    if (angle >= 45 && angle < 75)
                    {
                        if (tdPlayer.position.Y < bossRect.Y + 64 / 2)
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed / 5f, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                        }
                        else
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed / 5f, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                        }
                    }
                    if (angle >= 75 && angle < 105)
                    {
                        if (tdPlayer.position.Y < bossRect.Y + 64 / 2)
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + 15), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + 15), -bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                        }
                        else
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + height - 15), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + height - 15), -bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                        }
                    }
                    if (angle >= 105 && angle < 135)
                    {
                        if (tdPlayer.position.Y < bossRect.Y + 64 / 2)
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed / 5f, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                        }
                        else
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed / 5f, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                        }
                    }
                    if (angle >= 135 && angle < 155)
                    {
                        if (tdPlayer.position.Y < bossRect.Y + 64 / 2)
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 3.75f));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 1.75f));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
                        }
                        else
                        {
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 3.75f));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 1.75f));
                            bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
                        }
                    }


                    bossRect = tempRect;
                    while (shootDelay < 0.7f)
                    {
                        shootDelay = RandFloat(0, 2);
                    }

                  

                }

                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    bullets[i].Update();
                    if (bullets[i].rect.Intersects(tdPlayer.rectangle))
                    {
                        tdPlayer.Health -= bulletDmg;
                        bullets.RemoveAt(i);
                        break;
                    }
                    if (bullets[i].delete)
                    {
                        bullets.RemoveAt(i);
                        break;
                    }
                    foreach (WallTiles wallTiles in tdMap.WallTiles)
                    {
                        if (bullets[i].rect.Intersects(wallTiles.Rectangle))
                        {
                            bullets.RemoveAt(i);
                            break;
                        }

                    }

                }
            }

            if (Distance(slam.Position, new Vector2((int)(tdPlayer.rectangle.X + tdPlayer.rectangle.Width / 2),
                      (int)(tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2))) <= worldRect.Width / 2 + (64 * 1) && !slamWave)
            {
                slamWave = true;
            }

            if(slamWave)
            {
                slamDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (slamDelay <= 0)
                {
                    float moveOffset = (float)growthRate;
                    slam.Radius += growthRate;
                    slam.SetWidth(currWidthMod);

                    for (int i = tdMap.WallTiles.Count - 1; i >= 0; i--)
                    {
                        if (slam.Bounds.Intersects(tdMap.WallTiles[i].Rectangle))
                        {
                           
                            tdMap.FloorTiles.Add(new FloorTiles(9, tdMap.WallTiles[i].Rectangle));
                            tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                        }
                    }

                    if (slam.Radius >= maxSize)
                    {
                       
                        //currWidthMod = 1;
                        slamWave = false;
                        slamDelay = iSlamDelay;
                        slam = new Circle(new Vector2(bossRect.X + bossRect.Width / 2, bossRect.Y + bossRect.Height / 2), 2);
                    }
                    else
                    {
                        //currWidthMod += 1;
                        slam.Position = new Vector2(slam.Bounds.X - (moveOffset + currWidthMod), slam.Bounds.Y - moveOffset);
                    }

                   
                        
                    
                }
               
               
            }
            if (slam.Bounds.TouchBottomOf(tdPlayer.rectangle))
            {
                while(tdPlayer.rectangle.Bottom > slam.Bounds.Top)
                {
                    tdPlayer.rectangle.Y -= growthRate;
                    tdPlayer.position.Y -= growthRate;
                }
                
            }
            if (slam.Bounds.TouchTopOf(tdPlayer.rectangle))
            {
                while(tdPlayer.rectangle.Top < slam.Bounds.Bottom)
                {
                    tdPlayer.rectangle.Y += growthRate;
                    tdPlayer.position.Y += growthRate;
                }
              
            }

            if (slam.Bounds.TouchRightOf(tdPlayer.rectangle))
            {
                while(tdPlayer.rectangle.Right > slam.Bounds.Left)
                {
                    tdPlayer.rectangle.X -= growthRate;
                    tdPlayer.position.X -= growthRate;
                }
               
            }
            if (slam.Bounds.TouchLeftOf(tdPlayer.rectangle))
            {
                while(tdPlayer.rectangle.Left < slam.Bounds.Right)
                {
                    tdPlayer.rectangle.X += growthRate;
                    tdPlayer.position.X += growthRate;
                }
               
            }
            moveBack = false;
            if (slam.Bounds.Intersects(tdPlayer.rectangle))
            {
                moveBack = true;
            }
            if (Health <= 0)
            {
                bossRect = new Rectangle(0, 0, 32, 32);
                worldRect = new Rectangle(0, 0, 32, 32);
            }
            //foreach(Bullet bullet in bullets)
            //{
            //    bullet.Update();
            //}
        }

        public float RandFloat(int min, int max)
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(min, max).ToString();//number before decimal point
            string afterPoint = r.Next(0, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }
        public float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if(Health > 0)
            {
                bossRect = new Rectangle(((bounds.X + bounds.Width / 2) - width / 2), (((bounds.Y + bounds.Height / 2) - height / 2)), width, height);
                spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), bossRect, Color.White);
                foreach (Bullet bullet in bullets)
                {
                    bullet.Draw(spriteBatch);
                }
            }
           // spriteBatch.Draw(content.Load<Texture2D>("TopDown/Textures/Player"),slam.Position, slam.Bounds, Color.White);
        }
        //void CreateBulletSpread()
        //{
        //    bullets.Add(new Bullet(Rectangle.))
        //}
    }
   
}
