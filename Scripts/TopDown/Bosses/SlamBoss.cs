﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework.Content;
using AUTO_Matic.Scripts.Effects;

namespace AUTO_Matic.Scripts.TopDown.Bosses
{
    class SlamBoss
    {
        List<GroundLoc> slamLocs = new List<GroundLoc>();
        public Rectangle bossRect;

        int slameTimeMin = 5;
        int slamTimeMax = 10;

        int airTimeMin = 5;
        int airTimeMax = 10;
        float airTimeDelay;

        float slamDelay = 5;
        float iSlamDelay;

        public float health = 20;

        Random rand = new Random();
        ParticleManager particles;

        Explosion slam;
        int growthRate = 3;
        bool slamReady = false;
        bool moveBack = false;
        float slamDmg = 1.5f;

        BossHealthBar healthBar;
        float dmgResistance = 2.45f;

        enum BossState { SetStats, Shoot, Slam}
        BossState state = BossState.SetStats;

        ContentManager content;
        List<WallTiles> walls = new List<WallTiles>();
        List<FloorTiles> floors = new List<FloorTiles>();

        #region Animations
        enum AnimationStates { Idle, Slam, Shoot}
        AnimationStates animState = AnimationStates.Idle;
        AnimationManager animManager;
        Texture2D texture;
        Point FrameSize;//Size of frame
        Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;

        GroundLoc closestLoc;
        public void ChangeAnimation()
        {
            switch (animState)
            {
                case AnimationStates.Idle:
                    texture = content.Load<Texture2D>("TopDown/Animations/SlimeBurstBoss");
                    FrameSize = new Point(192, 192);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(1, 1);
                    fpms = 120;
                    break;
                case AnimationStates.Slam:
                    texture = content.Load<Texture2D>("TopDown/Animations/PlayerWalk");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(8, 1);
                    fpms = 120;
                    break;
                case AnimationStates.Shoot:
                    texture = content.Load<Texture2D>("TopDown/Animations/SlimeBurstBoss");
                    FrameSize = new Point(192, 192);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(11, 1);
                    fpms = 120;
                    break;
            }

            bool isRight = true, isLeft = false, isUp = false, isDown = false;
            if (animManager != null)
            {
                isRight = animManager.isRight;
                isLeft = animManager.isLeft;
                isUp = animManager.isUp;
                isDown = animManager.isDown;
            }

            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, new Vector2(bossRect.X, bossRect.Y));

            animManager.isRight = isRight;
            animManager.isLeft = isLeft;
            animManager.isUp = isUp;
            animManager.isDown = isDown;
        }
        #endregion

        #region Shooting
        public List<Bullet> bullets = new List<Bullet>();
        float shootDelay = .9f;//In seconds
        float iShootDelay;
        public float bulletDmg = .7f;
        public float bulletTravelDist = 64 * 8;
        public Rectangle worldRect;
        public float angle;

        List<WallTiles> BreakableWalls = new List<WallTiles>();
        List<WallTiles> BrokenWalls = new List<WallTiles>();
        bool respawnWalls = false;
        float respawnDelay;
        #endregion

        #region Constructor
        public SlamBoss(Rectangle currBounds, ContentManager content, TopDownMap tdMap, int[,] map, List<WallTiles> walls)
        {
            this.content = content;
            BreakableWalls = walls;
            iSlamDelay = slamDelay;
            iShootDelay = shootDelay;
            particles = new ParticleManager();
            particles.Initialize(content.Load<Texture2D>("Textures/WhiteCircle"));
            int sizeMod = 3;
            int width = 64 * sizeMod;
            int height = 64 * sizeMod;
            bossRect = new Rectangle(((currBounds.X + currBounds.Width / 2) - width / 2), (((currBounds.Y + currBounds.Height / 2) - height / 2)), width, height);
            healthBar = new BossHealthBar(new Rectangle(bossRect.X, bossRect.Y - 286, bossRect.Width, (int)(bossRect.Height / 3.5f)), content);
            //Gain the slam locs
            foreach (SlamTiles tile in tdMap.SlamTiles)
            {
                List<SlamTiles> list = new List<SlamTiles>();

                list.Add(tile);

                SlamTiles tTile = tile;

                list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Bottom,
                       tTile.Rectangle.Width, tTile.Rectangle.Height)));
                list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0] + 1, tile.mapPoint[1] +1 };
                slamLocs.Add(new GroundLoc(list));

            }

            ChangeAnimation();

        }
        #endregion

        public void Update(GameTime gameTime, TDPlayer tdPlayer, TopDownMap tdMap, Rectangle currBounds)
        {
            if(animState == AnimationStates.Shoot && animManager.GetCurrFrame().X == animManager.GetSheetSize().X)
            {
                animState = AnimationStates.Idle;
                ChangeAnimation();
            }

            switch(state)
            {
                case BossState.SetStats:
                    slamDelay = RandFloat(slameTimeMin, slamTimeMax);
                    state = BossState.Shoot;
                   
                    respawnDelay = slamDelay / 1.5f;
                    airTimeDelay = RandFloat(airTimeMin, airTimeMax);
                    break;
                case BossState.Shoot:
                    slamDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (slamDelay <= 0)
                    {
                        if(bossRect.Bottom > currBounds.Y)
                        {
                            bossRect.Y -= 10;
                        }
                        else
                        {
                            state = BossState.Slam;
                            bossRect.X = 0;
                        }
                        break;
                    }

                    FireSemiAuto(tdPlayer);

                    break;
                case BossState.Slam:

                    if(!slamReady && airTimeDelay > 0)
                    {
                        airTimeDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        Point centerPlayer = tdPlayer.rectangle.Center;
                        Rectangle centerRec = slamLocs[0].slamTiles[slamLocs[0].slamTiles.Count / 2].Rectangle;
                        Rectangle centerRec2 = slamLocs[1].slamTiles[slamLocs[1].slamTiles.Count / 2].Rectangle;
                        Rectangle centerRec3 = slamLocs[2].slamTiles[slamLocs[2].slamTiles.Count / 2].Rectangle;
                        Rectangle centerRec4 = slamLocs[3].slamTiles[slamLocs[3].slamTiles.Count / 2].Rectangle;
                        Rectangle centerRec5 = slamLocs[4].slamTiles[slamLocs[4].slamTiles.Count / 2].Rectangle;

                        GroundLoc closestLoc = slamLocs[0];
                        closestLoc.index = 0;

                        for (int i = 0; i < 1; i++) //This is weird cause the intial testing was weird IE stupid
                        {
                            centerRec = slamLocs[i].slamTiles[1].Rectangle;
                            for (int j = i + 1; j < slamLocs.Count; j++)
                            {
                                Rectangle centerRec1 = slamLocs[j].slamTiles[1].Rectangle;

                                if (DistForm(centerPlayer.ToVector2(), centerRec.Center.ToVector2()) <
                                    DistForm(centerPlayer.ToVector2(), centerRec1.Center.ToVector2()))
                                {
                                    //closestLoc = slamLocs[i];
                                    //closestLoc.index = i;
                                }
                                else
                                {
                                    centerRec = centerRec1;
                                    closestLoc = slamLocs[j];
                                    closestLoc.index = j;
                                    


                                    int number = closestLoc.index;
                                   
                                }

                            }
                        }
                        this.closestLoc = closestLoc;
                        if(airTimeDelay <=0)
                        {
                            bossRect = new Rectangle(closestLoc.slamTiles[0].Rectangle.X, closestLoc.slamTiles[0].Rectangle.Y,
                                       bossRect.Width, bossRect.Height);
                            slam = new Explosion(new Circle(new Vector2(bossRect.X, bossRect.Y), bossRect.Width / 2), growthRate,
                                (int)(bossRect.Width * 2.5f));
                            slam.rect.SetWidth(3);

                            slamReady = true;

                            particles.MakeSplash(bossRect, new Circle(new Vector2(slam.rect.Position.X + 20 / 2, slam.rect.Position.Y + 20/2), slam.rect.Radius / 4), 20);
                            BrokenWalls.Clear();
                        }
                    }
                    else if(slamReady && airTimeDelay <= 0)
                    {

                        slam.Update(gameTime);


                        for (int i = tdMap.WallTiles.Count - 1; i >= 0; i--)
                        {
                            if (slam.rect.Bounds.Intersects(tdMap.WallTiles[i].Rectangle))
                            {
                                if (walls.Contains(tdMap.WallTiles[i]) == false 
                                    && tdMap.dMapDims[tdMap.dMapDims.Count - 1][tdMap.WallTiles[i].mapPoint[0], tdMap.WallTiles[i].mapPoint[1]] == 10)
                                    walls.Add(tdMap.WallTiles[i]);
                               
                                if(tdMap.dMapDims[tdMap.dMapDims.Count - 1][tdMap.WallTiles[i].mapPoint[0], tdMap.WallTiles[i].mapPoint[1]] == 10)
                                {
                                    floors.Add(new FloorTiles(27, tdMap.WallTiles[i].Rectangle));
                                    tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                                }
                        

                            }
                        }

                        if (slam.rect.Bounds.Width >= slam.maxSize)
                        {
                            state = BossState.SetStats;
                            slamReady = false;
                            respawnWalls = true;

                        }
                        if (slam.rect.Bounds.TouchBottomOf(tdPlayer.rectangle))
                        {
                            //while (tdPlayer.rectangle.Bottom > slam.rect.Bounds.Top)
                            //{
                            //    tdPlayer.rectangle.Y -= growthRate;
                            //    tdPlayer.position.Y -= growthRate;
                            //}
                            if (!moveBack)
                            {
                                //moveBack = true;

                                if (!tdPlayer.damaged)
                                {
                                    tdPlayer.Health -= slamDmg;
                                }

                            }
                        }
                        if (slam.rect.Bounds.TouchTopOf(tdPlayer.rectangle))
                        {
                            //while (tdPlayer.rectangle.Top < slam.rect.Bounds.Bottom)
                            //{
                            //    tdPlayer.rectangle.Y += growthRate;
                            //    tdPlayer.position.Y += growthRate;
                            //}
                            if (!moveBack)
                            {
                                //moveBack = true;

                                if (!tdPlayer.damaged)
                                {
                                    tdPlayer.Health -= slamDmg;
                                }
                            }
                        }

                        if (slam.rect.Bounds.TouchRightOf(tdPlayer.rectangle))
                        {
                            //while (tdPlayer.rectangle.Right > slam.rect.Bounds.Left)
                            //{
                            //    tdPlayer.rectangle.X -= growthRate;
                            //    tdPlayer.position.X -= growthRate;
                            //}
                            if (!moveBack)
                            {
                               // moveBack = true;

                                if (!tdPlayer.damaged)
                                {
                                    tdPlayer.Health -= slamDmg;
                                }
                            }
                        }
                        if (slam.rect.Bounds.TouchLeftOf(tdPlayer.rectangle))
                        {
                            //while (tdPlayer.rectangle.Left < slam.rect.Bounds.Right)
                            //{
                            //    tdPlayer.rectangle.X += growthRate;
                            //    tdPlayer.position.X += growthRate;
                            //}
                            if (!moveBack)
                            {
                                //moveBack = true;

                                if (!tdPlayer.damaged)
                                {
                                    tdPlayer.Health -= slamDmg;
                                }
                            }
                        }
                    }
                    break;
            }

            for(int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(gameTime);

                if(bullets[i].rect.Intersects(tdPlayer.rectangle))
                {
                    if(!tdPlayer.damaged)
                    {
                        tdPlayer.Health -= bulletDmg;
                    }
                    bullets[i].delete = true;
                }

                foreach(WallTiles tile in tdMap.WallTiles)
                {
                    if(bullets[i].rect.Intersects(tile.Rectangle))
                    {
                        bullets[i].delete = true;
                    }
                }

                if(bullets[i].delete)
                {
                    bullets.RemoveAt(i);
                }
            }

            for(int i = tdPlayer.bullets.Count - 1; i >=0; i--)
            {
                if (tdPlayer.bullets[i].rect.Intersects(bossRect))
                {
                    health -= tdPlayer.bulletDmg / dmgResistance;
                    healthBar.RecieveDamage(tdPlayer.bulletDmg /dmgResistance);
                    tdPlayer.bullets[i].delete = true;
                }
                   
            }

            if(respawnWalls)
            {
                respawnDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(respawnDelay <= 0)
                {
                    floors.Clear();

                    foreach (WallTiles rect in walls)
                    {
                        tdMap.WallTiles.Add(rect);
                    }
                    respawnWalls = false;
                }
            }

            particles.Update(gameTime, tdPlayer, true);
            animManager.Update(gameTime, new Vector2(bossRect.X, bossRect.Y));
            healthBar.Update(new Point(bossRect.X, bossRect.Y - 32));
        }

        private void FireSemiAuto(TDPlayer tdPlayer)
        {
            if (shootDelay <= 0)
            {
                animState = AnimationStates.Shoot;
                ChangeAnimation();

                shootDelay = iShootDelay;

                Vector2 targetDir = new Vector2(tdPlayer.rectangle.X + tdPlayer.rectangle.Width / 2, tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2) -
                   new Vector2(bossRect.Center.X, bossRect.Center.Y);
                angle = (float)Math.Atan2(targetDir.Y, targetDir.X);

                Vector2 bossPos = new Vector2(bossRect.Center.X, bossRect.Center.Y);

                float bulletSpeedX = (float)Math.Cos((double)angle) * 2;
                float bulletSpeedY = (float)Math.Sin((double)angle) * 2;

                int i = 1;
                int xOffset = 0;
                int yOffset = 0;
                if(bossRect.Center.Y < tdPlayer.rectangle.Center.Y)
                {
                    xOffset = bossRect.Width / 5 * i;
                }
                else if(bossRect.Center.Y > tdPlayer.rectangle.Center.Y)
                {
                    xOffset = bossRect.Width / 5 * -i;
                }
         
                #region BurstShot
                bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY),
                    content, true, bulletTravelDist, true, bulletSpeedY, angle: angle));
                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;
                bullets[bullets.Count - 1].animOffset = new Vector2(xOffset, yOffset);


                bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletSpeedX * 1.5f, new Vector2(bulletSpeedX, bulletSpeedY),
               content, true, bulletTravelDist, true, bulletSpeedY * 1.5f, angle: angle));
                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;
                bullets[bullets.Count - 1].animOffset = new Vector2(xOffset, yOffset);

                bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletSpeedX / 1.5f, new Vector2(bulletSpeedX, bulletSpeedY),
              content, true, bulletTravelDist, true, bulletSpeedY / 1.5f, angle: angle));
                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;
                bullets[bullets.Count - 1].animOffset = new Vector2(xOffset, yOffset);

                #endregion
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach (FloorTiles tile in floors)
            {
                tile.Draw(spriteBatch);
            }
            if(state == BossState.Slam && closestLoc != null && airTimeDelay < 1f && !slamReady)
            {
                for(int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw(content.Load<Texture2D>("Textures/white"), new Rectangle(closestLoc.slamTiles[0].Rectangle.X + (64 * i), closestLoc.slamTiles[0].Rectangle.Y,
                        closestLoc.slamTiles[0].Rectangle.Width, closestLoc.slamTiles[0].Rectangle.Height), Color.Black * .5f);
                    spriteBatch.Draw(content.Load<Texture2D>("Textures/white"), new Rectangle(closestLoc.slamTiles[0].Rectangle.X + (64 * i), closestLoc.slamTiles[0].Rectangle.Y + 64,
                        closestLoc.slamTiles[0].Rectangle.Width, closestLoc.slamTiles[0].Rectangle.Height), Color.Black * .5f);
                    spriteBatch.Draw(content.Load<Texture2D>("Textures/white"), new Rectangle(closestLoc.slamTiles[0].Rectangle.X + (64 * i), closestLoc.slamTiles[0].Rectangle.Y + 128,
                        closestLoc.slamTiles[0].Rectangle.Width, closestLoc.slamTiles[0].Rectangle.Height), Color.Black * .5f);
                }
            }
           
            //if (slamReady)
            //{
            //    spriteBatch.Draw(content.Load<Texture2D>("Textures/white"), slam.rect.Bounds, Color.White * .25f);
            //}

            particles.Draw(spriteBatch);
            if(state != BossState.Slam && slamDelay > 0)
                healthBar.Draw(spriteBatch);
            //foreach (GroundLoc loc in slamLocs)
            //{
            //    for (int i = 0; i < loc.slamTiles.Count; i++)
            //    {
            //        spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile0"), loc.slamTiles[i].Rectangle, Color.White);
            //    }

            //}
            //spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), bossRect, Color.White);
            
            animManager.Draw(spriteBatch, Color.White);

            for(int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }
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

        int DistForm(Vector2 pos1, Vector2 pos2)
        {
            int num = (int)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
            return num;

        }
    }

    class GroundLoc
    {
        public List<SlamTiles> slamTiles;
        public int index;
       
        public GroundLoc(List<SlamTiles> tiles)
        {
            slamTiles = tiles;
        }

    }
   
}
