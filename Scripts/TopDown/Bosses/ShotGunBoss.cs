using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework.Content;
using AUTO_Matic.Scripts.Effects;

namespace AUTO_Matic.Scripts.TopDown
{
    class ShotGunBoss
    {
        Rectangle bossRect;
        ContentManager content;
        Rectangle turretRect;
        BossHealthBar healthBar;
        public float dmgResistance = 1.75f;

        #region Animations
        enum AnimationStates { Idle, Shoot, Slam}
        AnimationStates animState = AnimationStates.Idle;
        float turretAngle;
        AnimationManager animManagerTurret;
        AnimationManager animManagerBase;
        Texture2D baseTexture;
        Texture2D turretTexture;
        Point FrameSize;//Size of frame
        Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;
        public void ChangeAnimationBase()
        {
            switch (animState)
            {
                case AnimationStates.Idle:
                    baseTexture = content.Load<Texture2D>("TopDown/Animations/ShotgunBossBase");
                    FrameSize = new Point(256, 256);
                    CurrFrame = new Point(1, 0);
                    SheetSize = new Point(1, 1);
                    fpms = 0;
                    break;
                case AnimationStates.Shoot:
                    baseTexture = content.Load<Texture2D>("TopDown/Animations/ShotgunBossBase");
                    FrameSize = new Point(256, 256);
                    CurrFrame = new Point(1, 0);
                    SheetSize = new Point(1, 1);
                    fpms = 0;
                    break;
                case AnimationStates.Slam:
                    baseTexture = content.Load<Texture2D>("TopDown/Animations/ShotgunBossBase");
                    FrameSize = new Point(256, 256);
                    CurrFrame = new Point(1, 0);
                    SheetSize = new Point(3, 1);
                    fpms = 120;
                    break;
            }

            bool isRight = true, isLeft = false, isUp = false, isDown = false;
            if (animManagerBase != null)
            {
                isRight = animManagerBase.isRight;
                isLeft = animManagerBase.isLeft;
                isUp = animManagerBase.isUp;
                isDown = animManagerBase.isDown;
            }

            animManagerBase = new AnimationManager(baseTexture, FrameSize, CurrFrame, SheetSize, fpms, new Vector2(worldRect.X -2, worldRect.Y));

            animManagerBase.isRight = isRight;
            animManagerBase.isLeft = isLeft;
            animManagerBase.isUp = isUp;
            animManagerBase.isDown = isDown;
        }
        public void ChangeAnimationTurret()
        {
            switch (animState)
            {
                case AnimationStates.Idle:
                    turretTexture = content.Load<Texture2D>("TopDown/Animations/ShotgunBossTurret");
                    FrameSize = new Point(256, 256);
                    CurrFrame = new Point(1, 0);
                    SheetSize = new Point(2, 1);
                    fpms = 0;
                    
                    break;
                case AnimationStates.Shoot:
                    turretTexture = content.Load<Texture2D>("TopDown/Animations/ShotgunBossTurret");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(8, 1);
                    fpms = 120;
                    break;
                case AnimationStates.Slam:
                    turretTexture = content.Load<Texture2D>("TopDown/Animations/PlayerShoot");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(4, 1);
                    fpms = 95;
                    break;
            }

            bool isRight = true, isLeft = false, isUp = false, isDown = false;
            if (animManagerTurret != null)
            {
                isRight = animManagerTurret.isRight;
                isLeft = animManagerTurret.isLeft;
                isUp = animManagerTurret.isUp;
                isDown = animManagerTurret.isDown;
            }

            animManagerTurret = new AnimationManager(turretTexture, FrameSize, CurrFrame, SheetSize, fpms, new Vector2(worldRect.Center.X, worldRect.Center.Y));

            animManagerTurret.isRight = isRight;
            animManagerTurret.isLeft = isLeft;
            animManagerTurret.isUp = isUp;
            animManagerTurret.isDown = isDown;
            animManagerTurret.StopLoop();
        }
        #endregion

        #region Shooting
        Texture2D gunTexture;
        public List<Bullet> bullets = new List<Bullet>();
        float angle;
        float bulletSpeed = 4f;
        float bulletMaxX = 10f;
        float bulletMaxY = 10f;
        int spread = 3;
        bool isShootDelay = false;
        float shootDelay = 1.45f;//In seconds
        float iShootDelay;
        bool startShoot = false;
        public float bulletDmg = 1.75f;
        public float bulletTravelDist = 64 * 15;
        Texture2D visionTxture;
        int width;
        int height;
        Rectangle bounds;
        Rectangle tempRect;
        public Rectangle worldRect;

        List<WallTiles> BreakableWalls = new List<WallTiles>();
        #endregion

        Circle slam;
        int growthRate = 2;
        int maxSize = 300;
        bool slamWave = false;
        float health = 20f;
        int currWidthMod = 1;
        public bool moveBack;
        float slamDelay = 1.15f;
        float respawnDelay = 1.75f;
        float iSlamDelay;
        float randomSlamDelay;
        int randSlamMin = 6;
        int randSlamMax = 18;
        List<Rectangle> walls = new List<Rectangle>();
        bool respawn = false;
        float iRespawnDelay = 1.75f;
        List<FloorTiles> floors = new List<FloorTiles>();
        float slamDmg = 2.5f;


        ParticleManager particleManager = new ParticleManager();
        
        public float Health
        {
            get { return health; }
            set
            {
                health = value;
               
                if (health <= 0)
                    health = 0;
                healthBar.ChangeHealth(health);
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
            iRespawnDelay = respawnDelay;

            for(int i = tdMap.WallTiles.Count - 1; i >= 0; i--)
            {
                if(bounds.Intersects(tdMap.WallTiles[i].Rectangle) == false || bounds.Contains(tdMap.WallTiles[i].Rectangle) == false)
                {
                    tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                }
            }
            particleManager.Initialize(content.Load<Texture2D>(@"Textures\white"));
            particleManager.SetParticles(800);
            healthBar = new BossHealthBar(new Rectangle(worldRect.X, worldRect.Y - 286, worldRect.Width, (int)(worldRect.Height /3.5f)), content);
            ChangeAnimationTurret();
            ChangeAnimationBase();
            randomSlamDelay = RandFloat(randSlamMin, randSlamMax);
        }

        public void Update(GameTime gameTime, TDPlayer tdPlayer, TopDownMap tdMap)
        {
          

            if(health > 0)
            {
                if (shootDelay >= 0 && !slamWave)
                {
                    shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                Vector2 targetDir = new Vector2(tdPlayer.rectangle.Center.X, tdPlayer.rectangle.Center.Y) - new Vector2(worldRect.Center.X, worldRect.Center.Y);
                turretAngle = (float)Math.Atan2(targetDir.Y, targetDir.X); //sub by 90 if problems occur
                if (shootDelay <= 0 && !slamWave)
                {
                    
                    bossRect = new Rectangle(((bounds.X + bounds.Width / 2) - width / 2), (((bounds.Y + bounds.Height / 2) - height / 2)), width, height);

                    targetDir = new Vector2(tdPlayer.rectangle.X, tdPlayer.rectangle.Y) - new Vector2(bossRect.Center.X, bossRect.Center.Y);
                    angle = (float)Math.Atan2(targetDir.Y, targetDir.X); //sub by 90 if problems occur

                    ShootShotgun(tdPlayer, turretAngle);

                    #region BrokenAngle
                  //  float angle = (float)Math.Atan2(targetDir.Y, targetDir.X);
                  //  bossRect = new Rectangle(((bounds.X + bounds.Width / 2) - width / 2), (((bounds.Y + bounds.Height / 2) - height / 2)), width, height);

                  //  float bulletSpeedX = (float)Math.Cos((double)angle) * 2;
                  //  float bulletSpeedY = (float)Math.Sin((double)angle) * 2;
                  //  float bulletSpeedX0 = bulletSpeedX / 2;
                  //  float bulletSpeedY0 = bulletSpeedY / 2;


                  //  bullets.Add(new Bullet(new Vector2(worldRect.Center.X, worldRect.Center.Y), bulletSpeedX + bulletSpeedX0, new Vector2(bulletSpeedX, bulletSpeedY),
                  //   content, true, bulletTravelDist, true, bulletSpeedY - bulletSpeedY0));

                  //  bullets.Add(new Bullet(new Vector2(worldRect.Center.X, worldRect.Center.Y), bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY),
                  // content, true, bulletTravelDist, true, bulletSpeedY));

                  //  bullets.Add(new Bullet(new Vector2(worldRect.Center.X, worldRect.Center.Y), bulletSpeedX - bulletSpeedX0, new Vector2(bulletSpeedX, bulletSpeedY),
                  //content, true, bulletTravelDist, true, bulletSpeedY + bulletSpeedX0));
                    #endregion

                    bossRect = tempRect;
                    while (shootDelay < 0.7f)
                    {
                        shootDelay = RandFloat(0, 2);
                    }



                }

                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    bullets[i].Update(gameTime);
                    if (bullets[i].rect.Intersects(tdPlayer.rectangle))
                    {
                        if (!tdPlayer.damaged)
                        {
                            //tdPlayer.damaged = true;
                            tdPlayer.Health -= bulletDmg;
                        }
                       
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
                      (int)(tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2))) <= worldRect.Width / 2 + (64 * 2.5f) && !slamWave)
            {
                slamWave = true;

                //walls.Clear();
                moveBack = false;
            }

            if(!slamWave)
                randomSlamDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (randomSlamDelay <= 0 && !slamWave)
            {
                slamWave = true;
                moveBack = false;

                randomSlamDelay = RandFloat(randSlamMin, randSlamMax);
            }

            if(slamWave)
            {
                slamDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (slamDelay <= 0)
                {
                    float moveOffset = (float)growthRate;
                    slam.Radius += growthRate;
                    slam.SetWidth(currWidthMod);
                    //moveBack = false;
                    for (int i = tdMap.WallTiles.Count - 1; i >= 0; i--)
                    {
                        if (slam.Bounds.Intersects(tdMap.WallTiles[i].Rectangle))
                        {
                            if(walls.Contains(tdMap.WallTiles[i].Rectangle) == false)
                                walls.Add(tdMap.WallTiles[i].Rectangle);
                            floors.Add(new FloorTiles(9, tdMap.WallTiles[i].Rectangle));
                            //tdMap.FloorTiles.Add());
                            tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                           
                        }
                    }
                   
                    if (slam.Radius <= maxSize)
                    {
                        slam.Position = new Vector2(slam.Bounds.X - (moveOffset + currWidthMod), slam.Bounds.Y - moveOffset);
                        //currWidthMod = 1;
                        Circle circle1 = new Circle(slam.Position, slam.Radius);
                        //circle1.Radius += 30;
                        if (slam.Bounds.X < worldRect.X)
                        {
                            particleManager.MakeExplosion(new Rectangle((int)worldRect.X, (int)worldRect.Y, worldRect.Width, worldRect.Height),
                                   new Circle(new Vector2(slam.Position.X + 45/2, slam.Position.Y), (int)(slam.Radius/2)), 45);
                         
                           
                        }
                    }
                    else
                    {
                        slamWave = false;


                        slamDelay = RandFloat(1, 3);
                        //slamDelay = iSlamDelay;
                        slam = new Circle(new Vector2(bossRect.X + bossRect.Width / 2, bossRect.Y + bossRect.Height / 2), 2);

                        respawn = true;
                        //currWidthMod += 1;


                        //articleManager.MakeExplosion(new Rectangle(slam.Bounds.Center.X, slam.Bounds.Center.Y, slam.Bounds.Width, slam.Bounds.Height), circle1, 30);
                    }


                    
                 



                }


            }
            else if (respawn)
            {
                respawnDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (respawnDelay <= 0)
                {
                    respawnDelay = iRespawnDelay;

                    floors.Clear();

                    foreach (Rectangle rect in walls)
                    {
                        //tdMap.FloorTiles.Remove(i);
                        tdMap.WallTiles.Add(new WallTiles(10, rect));
                      
                        //i++;
                    }
                    respawn = false;
                }


            }
            if (slam.Bounds.TouchBottomOf(tdPlayer.rectangle))
            {
                while (tdPlayer.rectangle.Bottom > slam.Bounds.Top)
                {
                    tdPlayer.rectangle.Y -= growthRate;
                    tdPlayer.position.Y -= growthRate;
                }
                if (!moveBack)
                {
                    moveBack = true;

                    if(!tdPlayer.damaged)
                    {
                        //tdPlayer.damaged = true;
                        tdPlayer.Health -= slamDmg;
                    }
                 
                }
            }
            if (slam.Bounds.TouchTopOf(tdPlayer.rectangle))
            {
                while (tdPlayer.rectangle.Top < slam.Bounds.Bottom)
                {
                    tdPlayer.rectangle.Y += growthRate;
                    tdPlayer.position.Y += growthRate;
                }
                if (!moveBack)
                {
                    moveBack = true;

                    if (!tdPlayer.damaged)
                    {
                        //tdPlayer.damaged = true;
                        tdPlayer.Health -= slamDmg;
                    }
                }
            }

            if (slam.Bounds.TouchRightOf(tdPlayer.rectangle))
            {
                while (tdPlayer.rectangle.Right > slam.Bounds.Left)
                {
                    tdPlayer.rectangle.X -= growthRate;
                    tdPlayer.position.X -= growthRate;
                }
                if (!moveBack)
                {
                    moveBack = true;

                    if (!tdPlayer.damaged)
                    {
                        //tdPlayer.damaged = true;
                        tdPlayer.Health -= slamDmg;
                    }
                }
            }
            if (slam.Bounds.TouchLeftOf(tdPlayer.rectangle))
            {
                while (tdPlayer.rectangle.Left < slam.Bounds.Right)
                {
                    tdPlayer.rectangle.X += growthRate;
                    tdPlayer.position.X += growthRate;
                }
                if (!moveBack)
                {
                    moveBack = true;

                    if (!tdPlayer.damaged)
                    {
                        //tdPlayer.damaged = true;
                        tdPlayer.Health -= slamDmg;
                    }
                }
            }
           // moveBack = false;
            particleManager.Update(gameTime, tdPlayer,true);
            if (Health <= 0)
            {
                bossRect = new Rectangle(0, 0, 32, 32);
                worldRect = new Rectangle(0, 0, 32, 32);
                particleManager = new ParticleManager();
                floors.Clear();
            }
            //foreach(Bullet bullet in bullets)
            //{
            //    bullet.Update();
            //}
        
                animManagerTurret.Update(gameTime, new Vector2(worldRect.Center.X, worldRect.Center.Y));

            animManagerBase.Update(gameTime, new Vector2(worldRect.X - 8, worldRect.Y));
            healthBar.Update(new Point(worldRect.X, worldRect.Y - 100));
        }

        private void ShootShotgun(TDPlayer tdPlayer, float angle)
        {


            float bulletUpSpeedX = (float)Math.Cos((double)angle - .15f) * bulletSpeed;
            float bulletUpSpeedY = (float)Math.Sin((double)angle - .15f) * bulletSpeed;

            float bulletSpeedX = (float)Math.Cos((double)angle) * bulletSpeed; //Direct SpeedX
            float bulletSpeedY = (float)Math.Sin((double)angle) * bulletSpeed; //DirectSpeedY

            float bulletDownSpeedX = (float)Math.Cos((double)angle + .15f) * bulletSpeed;
            float bulletDownSpeedY = (float)Math.Sin((double)angle + .15f) * bulletSpeed;

            bullets.Add(new Bullet(new Vector2(worldRect.Center.X, worldRect.Center.Y), bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY),
                content, true, bulletTravelDist, true, bulletSpeedY, angle: angle));
            bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;

            bullets.Add(new Bullet(new Vector2(worldRect.Center.X, worldRect.Center.Y), bulletUpSpeedX, new Vector2(bulletUpSpeedX, bulletUpSpeedY),
             content, true, bulletTravelDist, true, bulletUpSpeedY, angle: angle));
            bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;


            bullets.Add(new Bullet(new Vector2(worldRect.Center.X, worldRect.Center.Y), bulletDownSpeedX, new Vector2(bulletDownSpeedX, bulletDownSpeedY),
             content, true, bulletTravelDist, true, bulletDownSpeedY, angle: angle));
            bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;


            //if (angle < 18 || angle >= 155)//Right
            //{
            //    if (angle < 18)//Right
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width/2 - 15/2, bossRect.Y + height / 2 - 15 / 2), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + height / 2 - 15 / 2), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 7));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + height / 2 - 15 / 2), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 7));
            //    }
            //    else//Left
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + height / 2 - 15 / 2), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + height / 2 - 15 / 2), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 7));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + height / 2 - 15 / 2), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 7));
            //    }


            //}
            //if (angle >= 18 && angle < 45)
            //{
            //    if (tdPlayer.position.Y < bossRect.Y + 64 / 2)//Right up
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 4));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 1.75f));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //    }
            //    else//Right Down
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 6));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2 - 15 / 2, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 2));
            //    }
            //}
            //if (angle >= 45 && angle < 75)
            //{
            //    if (tdPlayer.position.Y < bossRect.Y + 64 / 2)//
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed / 5f, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //    }
            //    else
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed / 5f, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //    }
            //}
            //if (angle >= 75 && angle < 105)
            //{
            //    if (tdPlayer.position.Y < bossRect.Y + 64 / 2)
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + 15), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + 15), -bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //    }
            //    else
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + height - 15), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + height - 15), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X + width / 2, bossRect.Y + height - 15), -bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //    }
            //}
            //if (angle >= 105 && angle < 135)
            //{
            //    if (tdPlayer.position.Y < bossRect.Y + 64 / 2)
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed / 5f, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //    }
            //    else
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed / 5f, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //    }
            //}
            //if (angle >= 135 && angle < 155)
            //{
            //    if (tdPlayer.position.Y < bossRect.Y + 64 / 2)
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 3.75f));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 1.75f));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed));
            //    }
            //    else
            //    {
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 3.75f));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 1.75f));
            //        bullets.Add(new Bullet(new Vector2(bossRect.X, bossRect.Y + height - 15), -bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed));
            //    }
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
            foreach (FloorTiles tile in floors)
            {
                tile.Draw(spriteBatch);
            }
            particleManager.Draw(spriteBatch);
            if (Health > 0)
            {
                bossRect = new Rectangle(((bounds.X + bounds.Width / 2) - width / 2), (((bounds.Y + bounds.Height / 2) - height / 2)), width, height);
                //spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), bossRect, Color.White);
                animManagerBase.Draw(spriteBatch, Color.White);
                animManagerTurret.Draw(spriteBatch, Color.White, turretAngle, new Rectangle(worldRect.X, worldRect.Y, worldRect.Width + 8, worldRect.Height + 8));
                foreach (Bullet bullet in bullets)
                {
                    bullet.Draw(spriteBatch);
                }
            }

            healthBar.Draw(spriteBatch);
           
      
           //spriteBatch.Draw(content.Load<Texture2D>("TopDown/Textures/Player"),slam.Position, slam.Bounds, Color.White * .5f);
           
        }
        //void CreateBulletSpread()
        //{
        //    bullets.Add(new Bullet(Rectangle.))
        //}
    }
   
}
