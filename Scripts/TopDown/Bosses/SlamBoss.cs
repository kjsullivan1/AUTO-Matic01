using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework.Content;

namespace AUTO_Matic.Scripts.TopDown.Bosses
{
    class SlamBoss
    {
        List<GroundLoc> slamLocs = new List<GroundLoc>();
        Rectangle bossRect;

        int slameTimeMin = 5;
        int slamTimeMax = 10;

        float slamDelay = 5;
        float iSlamDelay;

        Random rand = new Random();

        Explosion slam;
        int growthRate = 3;
        bool slamReady = false;

        enum BossState { SetStats, Shoot, Slam}
        BossState state = BossState.SetStats;

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
        float shootDelay = .35f;//In seconds
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
        public float angle;

        List<WallTiles> BreakableWalls = new List<WallTiles>();
        #endregion

        #region Constructor
        public SlamBoss(Rectangle currBounds, ContentManager content, TopDownMap tdMap, int[,] map)
        {
            this.content = content;

            iSlamDelay = slamDelay;
            iShootDelay = shootDelay;
            int sizeMod = 3;
            int width = 64 * sizeMod;
            int height = 64 * sizeMod;
            bossRect = new Rectangle(((currBounds.X + currBounds.Width / 2) - width / 2), (((currBounds.Y + currBounds.Height / 2) - height / 2)), width, height);
            //worldRect = new Rectangle(((rect.X + rect.Width / 2) - size / 2), (((rect.Y + rect.Height / 2) - size / 2)), size, size);
            //Gain the slam locs
            foreach (SlamTiles tile in tdMap.SlamTiles)
            {
                List<SlamTiles> list = new List<SlamTiles>();

                list.Add(tile);

                SlamTiles tTile = tile;
                int i = 1;

                list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Bottom,
                       tTile.Rectangle.Width, tTile.Rectangle.Height)));
                list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0] + 1, tile.mapPoint[1] +1 };

                //while (list.Count < 3)
                //{
                //    list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Y,
                //        tTile.Rectangle.Width, tTile.Rectangle.Height)));

                //    list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0], tile.mapPoint[1] + i };
                //    i++;

                //    //tdMap.dMapDims[tdMap.dMapDims.Count - 1][list[list.Count - 1].mapPoint[0],
                //    //    list[list.Count - 1].mapPoint[1]] = 0;
                //    tTile = list[list.Count - 1];
                //}
                //tTile = tile;
                //tTile.Rectangle = new Rectangle(tTile.Rectangle.X, tTile.Rectangle.Y + tTile.Rectangle.Height,
                //    tTile.Rectangle.Width, tTile.Rectangle.Height);
                //i = 0;
                //while (list.Count < 6)
                //{
                //    list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Y,
                //          tTile.Rectangle.Width, tTile.Rectangle.Height)));
                //    list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0] + 1, tile.mapPoint[1] + i };
                //    i++;

                //    //tdMap.dMapDims[tdMap.dMapDims.Count - 1][list[list.Count - 1].mapPoint[0],
                //    //list[list.Count - 1].mapPoint[1]] = 0;
                //    tTile = list[list.Count - 1];
                //}

                //tTile = tile;
                ////tTile.Rectangle = new Rectangle(tTile.Rectangle.X, tTile.Rectangle.Bottom + (tTile.Rectangle.Height),
                ////    tTile.Rectangle.Width, tTile.Rectangle.Height);
                //i = 0;
                //while (list.Count < 9)
                //{
                //    if (i < 3)
                //    {
                //        list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Bottom + (tTile.Rectangle.Height),
                //         tTile.Rectangle.Width, tTile.Rectangle.Height)));

                //        list[list.Count - 1].mapPoint = new int[] { tile.mapPoint[0] + 2, tile.mapPoint[1] + i };
                //        i++;

                //        //tdMap.dMapDims[tdMap.dMapDims.Count - 1][list[list.Count - 1].mapPoint[0],
                //        //list[list.Count - 1].mapPoint[1]] = 0;
                //        tTile = list[list.Count - 1];
                //    }
                //    else
                //    {
                //        list.Add(new SlamTiles(tdMap.SlamIndexes[0], new Rectangle(tTile.Rectangle.Right, tTile.Rectangle.Bottom,
                //         tTile.Rectangle.Width, tTile.Rectangle.Height)));
                //    }

                //}

                //tdMap.dMapDims[tdMap.dMapDims.Count - 1][tile.mapPoint[0] + 3, tile.mapPoint[1]] = 10;
                //tdMap.dMapDims[tdMap.dMapDims.Count - 1][list[4].mapPoint[0], list[4].mapPoint[1]] = 9;
                //list.RemoveAt(4);
                slamLocs.Add(new GroundLoc(list));

            }

            //int locNum = 4;
            //for(int i = 0; i < slamLocs[locNum].slamTiles.Count; i++)
            //{
            //    tdMap.dMapDims[tdMap.dMapDims.Count - 1][slamLocs[locNum].slamTiles[i].mapPoint[0],
            //        slamLocs[locNum].slamTiles[i].mapPoint[1]] = 9;
            //}
        }
        #endregion

        public void Update(GameTime gameTime, TDPlayer tdPlayer, TopDownMap tdMap)
        {
            switch(state)
            {
                case BossState.SetStats:
                    slamDelay = RandFloat(slameTimeMin, slamTimeMax);
                    state = BossState.Shoot;
                    break;
                case BossState.Shoot:
                    slamDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (slamDelay <= 0)
                    {
                        state = BossState.Slam;
                        //slamDelay = iSlamDelay;
                        break;
                    }

                    FireSemiAuto(tdPlayer);

                    break;
                case BossState.Slam:

                    if(!slamReady)
                    {
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
                        bossRect = new Rectangle(closestLoc.slamTiles[0].Rectangle.X, closestLoc.slamTiles[0].Rectangle.Y,
                                       bossRect.Width, bossRect.Height);
                        slam = new Explosion(new Circle(new Vector2(bossRect.X, bossRect.Y), bossRect.Width / 2), growthRate,
                            (int)(bossRect.Width * 2.5f));
                        slam.rect.SetWidth(3);

                        slamReady = true;
                    }
                    else if(slamReady)
                    {
                        slam.Update(gameTime);

                        if(slam.rect.Bounds.Width >= slam.maxSize)
                        {
                            state = BossState.SetStats;
                            slamReady = false;
                        }
                    }
                   

                    

                  

                   
                    break;
            }

            for(int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update();
                if(bullets[i].delete)
                {
                    bullets.RemoveAt(i);
                }
            }
        }

        private void FireSemiAuto(TDPlayer tdPlayer)
        {
            if (shootDelay <= 0)
            {
                shootDelay = iShootDelay;

                Vector2 targetDir = new Vector2(tdPlayer.rectangle.X + tdPlayer.rectangle.Width / 2, tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2) -
                   new Vector2(bossRect.Center.X, bossRect.Center.Y);
                angle = (float)Math.Atan2(targetDir.Y, targetDir.X);

                Vector2 bossPos = new Vector2(bossRect.Center.X - tdPlayer.rectangle.Width / 2, bossRect.Center.Y - tdPlayer.rectangle.Height / 2);

                float bulletSpeedX = (float)Math.Cos((double)angle) * 2;
                float bulletSpeedY = (float)Math.Sin((double)angle) * 2;

                bullets.Add(new Bullet(bossPos, bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY), content, true, bulletTravelDist, true, bulletSpeedY));

                //if (angle < 16 || angle >= 155)//Right
                //{
                //    if (angle < 16)
                //    {

                //        bullets.Add(new Bullet(new Vector2(bossPos.X + bossRect.Width / 2, bossPos.Y), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY),
                //            content, true, 64 * 25));

                //    }
                //    else//Left
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X - bossRect.Width / 2, bossPos.Y), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY),
                //                content, true, 64 * 25));

                //    }
                //}
                //if (angle >= 16 && angle < 35)//Right up
                //{
                //    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X + bossRect.Width / 2, bossPos.Y - bossRect.Height / 2), bulletSpeed / 4, new Vector2(bulletMaxX, -bulletMaxY),
                //            content, true, 64 * 25, true, -bulletSpeed / 8));
                //    }
                //    else
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X + bossRect.Width / 2, bossPos.Y + bossRect.Height / 2), bulletSpeed / 4, new Vector2(bulletMaxX, bulletMaxY),
                //          content, true, 64 * 25, true, bulletSpeed / 8));


                //    }

                //    //Compare pos for down
                //}
                //if (angle >= 35 && angle < 75)//more right up
                //{
                //    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X + bossRect.Width / 2, bossPos.Y - bossRect.Height / 2), bulletSpeed / 2, new Vector2(bulletMaxX, -bulletMaxY),
                //          content, true, 64 * 25, true, -bulletSpeed / 4));


                //    }
                //    else
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X + bossRect.Width / 2, bossPos.Y + bossRect.Height / 2), bulletSpeed / 2, new Vector2(bulletMaxX, bulletMaxY),
                //          content, true, 64 * 25, true, bulletSpeed / 4));


                //    }
                //}
                //if (angle >= 75 && angle < 105)//up
                //{
                //    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X, bossPos.Y - bossRect.Height / 2), bulletSpeed, new Vector2(bulletMaxX, -bulletMaxY),
                //      content, false, 64 * 25, true, -bulletSpeed));


                //    }
                //    else
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X, bossPos.Y + bossRect.Height / 2), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY),
                //   content, false, 64 * 25, true, bulletSpeed));


                //    }
                //}
                //if (angle >= 105 && angle < 135)//up left
                //{
                //    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X - bossRect.Width / 2, bossPos.Y - bossRect.Height / 2), -bulletSpeed / 4, new Vector2(-bulletMaxX, -bulletMaxY),
                //  content, true, 64 * 25, true, -bulletSpeed / 8));


                //    }
                //    else
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X - bossRect.Width / 2, bossPos.Y + bossRect.Height / 2), -bulletSpeed / 4, new Vector2(-bulletMaxX, bulletMaxY),
                //   content, true, 64 * 25, true, bulletSpeed / 8));


                //    }
                //}
                //if (angle >= 135 && angle < 155)
                //{
                //    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X - bossRect.Width / 2, bossPos.Y - bossRect.Height / 2), -bulletSpeed / 2, new Vector2(-bulletMaxX, -bulletMaxY),
                //      content, true, 64 * 25, true, -bulletSpeed / 4));


                //    }
                //    else
                //    {
                //        bullets.Add(new Bullet(new Vector2(bossPos.X - bossRect.Width / 2, bossPos.Y + bossRect.Height / 2), -bulletSpeed / 2, new Vector2(-bulletMaxX, bulletMaxY),
                //   content, true, 64 * 25, true, bulletSpeed / 4));


                //    }
                //}
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach (GroundLoc loc in slamLocs)
            {
                for (int i = 0; i < loc.slamTiles.Count; i++)
                {
                    spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile0"), loc.slamTiles[i].Rectangle, Color.White);
                }

            }
            spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), bossRect, Color.White);

            for(int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }
    
            if(slamReady)
            {
                slam.Draw(spriteBatch, content);
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
