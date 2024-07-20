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
    class BombBoss
    {
        public Rectangle bossRect;
        ContentManager content;
        Random rand = new Random();

        enum BossStates { Shoot};

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

        public float angle;
        public List<Bullet> bullets = new List<Bullet>();
        float bulletSpeed = .45f;
        float bulletMaxX = 6f;
        float bulletMaxY = 6f;
        int spread = 3;
        bool isShootDelay = false;
        float shootDelay = 6.5f;//In seconds
        float iShootDelay;
        bool startShoot = false;
        public float bulletDmg = 1.5f;
        public float bulletTravelDist = 64 * 8;
        Texture2D visionTxture;
        int width;
        int height;
        Rectangle bounds;
        Rectangle tempRect;

        WallSide TopWalls;
        WallSide BottomWalls;
        WallSide RightWalls;
        WallSide LeftWalls;
        List<WallTiles> JumpWalls = new List<WallTiles>();
        #endregion

        int sizeMod = 2;
        float health = 20;
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

        #region Constructor
        public BombBoss(Rectangle currBounds, ContentManager content, TopDownMap tdMap, int[,] map, TDPlayer tdPlayer)
        {
            int size = 64 * sizeMod;

            bossRect = new Rectangle(((currBounds.X + currBounds.Width / 2) - size / 2), (((currBounds.Y + currBounds.Height / 2) - size / 2)), size, size);
            this.content = content;
            this.bounds = currBounds;
            iShootDelay = shootDelay;

            SetWalls(tdMap, map);

            bool left = false, top = false, right = false, bottom = false;
            if(MathHelper.Distance(RightWalls.walls[0].Rectangle.X, tdPlayer.rectangle.Center.X) >  
                MathHelper.Distance(LeftWalls.walls[0].Rectangle.Right, tdPlayer.rectangle.Center.X))
            {
                right = true;
            }
            else
            {
                left = true;
            }

            if(MathHelper.Distance(BottomWalls.walls[0].Rectangle.Y, tdPlayer.rectangle.Center.Y) >
                MathHelper.Distance(TopWalls.walls[0].Rectangle.Bottom, tdPlayer.rectangle.Center.Y))
            {
                bottom = true;
            }
            else
            {
                top = true;
            }
           
            if(left && top)
            {
                if(MathHelper.Distance(LeftWalls.walls[0].Rectangle.Right, tdPlayer.rectangle.Center.X) <
                    MathHelper.Distance(TopWalls.walls[0].Rectangle.Bottom, tdPlayer.rectangle.Center.Y))
                {
                    bossRect = new Rectangle(TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.X - bossRect.Width/4,
                        TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.Bottom,
                        bossRect.Width, bossRect.Height);
                }
                else
                {
                    bossRect = new Rectangle(LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Right, 
                        LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Y - bossRect.Height/4,
                        bossRect.Width, bossRect.Height);
                }
            }
            else if(left && bottom)
            {
                if (MathHelper.Distance(LeftWalls.walls[0].Rectangle.Right, tdPlayer.rectangle.Center.X) <
                    MathHelper.Distance(BottomWalls.walls[0].Rectangle.Y, tdPlayer.rectangle.Center.Y))
                {
                    bossRect = new Rectangle(BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.X - bossRect.Width/4,
                   BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.Y - bossRect.Height,
                   bossRect.Width, bossRect.Height);
                }
                else
                {
                    bossRect = new Rectangle(LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Right,
                   LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Y - bossRect.Height/4,
                   bossRect.Width, bossRect.Height);
                }
            }
            else if(bottom && right)
            {
                if (MathHelper.Distance(RightWalls.walls[0].Rectangle.X, tdPlayer.rectangle.Center.X) <
                  MathHelper.Distance(BottomWalls.walls[0].Rectangle.Y, tdPlayer.rectangle.Center.Y))
                {
                    bossRect = new Rectangle(BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.X - bossRect.Width/4,
                   BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.Y - bossRect.Height,
                   bossRect.Width, bossRect.Height);
                }
                else
                {
                    bossRect = new Rectangle(RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.X - bossRect.Width,
                  RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.Y - bossRect.Height/4,
                  bossRect.Width, bossRect.Height);
                }
            }
            else if(right && top)
            {
                if (MathHelper.Distance(RightWalls.walls[0].Rectangle.X, tdPlayer.rectangle.Center.X) <
                 MathHelper.Distance(TopWalls.walls[0].Rectangle.Bottom, tdPlayer.rectangle.Center.Y))
                {
                    bossRect = new Rectangle(TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.X - bossRect.Width/4,
                    TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.Bottom,
                    bossRect.Width, bossRect.Height);
                }
                else
                {
                    bossRect = new Rectangle(RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.X - bossRect.Width,
                 RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.Y - bossRect.Height/4,
                 bossRect.Width, bossRect.Height);
                }
            }
        }
        #endregion
        public void Update(GameTime gameTime, TopDownMap tdMap, TDPlayer tdPlayer)
        {
            Vector2 targetDir = new Vector2(tdPlayer.rectangle.X + tdPlayer.rectangle.Width / 2, tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2) -
                           new Vector2(bossRect.Center.X, bossRect.Center.Y);
            angle = Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(targetDir.Y, targetDir.X)));

            shootDelay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 100;

            if (shootDelay <= 0)
            {
                shootDelay = iShootDelay;
                bulletTravelDist = DistForm(tdPlayer.rectangle.Center.ToVector2(), bossRect.Center.ToVector2());
                if (angle < 16 || angle >= 170)//Right
                {
                    if (angle < 16)
                    {
                        //boss.bulletRects.Add(new Rectangle((int)bossRect.X + (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        //    (int)bossRect.Y /*- (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2)*/, bossRect.Width, bossRect.Height));


                        bullets.Add(new Bullet(new Vector2(bossRect.X + (int)bossRect.X + (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                            (int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4)), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, size: 42));
                    }
                    else//Left
                    {
                        bullets.Add(new Bullet(new Vector2((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                            (int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4)), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, size: 42));
                        //boss.bulletRects.Add(new Rectangle((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        //    (int)bossRect.Y /*- (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2)*/, bossRect.Width, bossRect.Height));

                    }
                }
                if (angle >= 16 && angle < 35)//Right up
                {
                    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                    {
                        bullets.Add(new Bullet(new Vector2((int)bossRect.X + (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        (int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4)),
                          bulletSpeed, new Vector2(bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 4, size: 42));

                        //     boss.bulletRects.Add(new Rectangle((int)bossRect.X + (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        //(int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4), bossRect.Width, bossRect.Height));


                    }
                    else
                    {


                        //      boss.bulletRects.Add(new Rectangle((int)bossRect.X + (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        //(int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4), bossRect.Width, bossRect.Height));

                    }

                    //Compare pos for down
                }
                if (angle >= 35 && angle < 55)//more right up
                {
                    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                    {
                        //     boss.bulletRects.Add(new Rectangle((int)bossRect.X + (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 4),
                        //(int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2), bossRect.Width, bossRect.Height));


                    }
                    else
                    {
                        //      boss.bulletRects.Add(new Rectangle((int)bossRect.X + (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 4),
                        //(int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2), bossRect.Width, bossRect.Height));

                    }
                }

                if (angle >= 55 && angle < 75)
                {

                }
                if (angle >= 75 && angle < 105)//up
                {
                    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                    {
                        //       boss.bulletRects.Add(new Rectangle((int)bossRect.X /*+ (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 4)*/,
                        //(int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2), bossRect.Width, bossRect.Height));

                    }
                    else
                    {
                        //       boss.bulletRects.Add(new Rectangle((int)bossRect.X /*+ (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 4)*/,
                        //(int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2), bossRect.Width, bossRect.Height));


                    }
                }
                if (angle >= 105 && angle < 115)//up left
                {
                    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                    {

                        bullets.Add(new Bullet(new Vector2((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 4),
                                    (int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2)), -bulletSpeed / 4, new Vector2(-bulletMaxX, -bulletMaxY),
                                    content, true, bulletTravelDist, true, -bulletSpeed, size: 42));

                        //        boss.bulletRects.Add(new Rectangle((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 4),
                        //(int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2), bossRect.Width, bossRect.Height));


                    }
                    else
                    {
                        //        boss.bulletRects.Add(new Rectangle((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 4),
                        //(int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2), bossRect.Width, bossRect.Height));
                        bullets.Add(new Bullet(new Vector2((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 4),
                                   (int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 2)), -bulletSpeed / 4, new Vector2(-bulletMaxX, bulletMaxY),
                                   content, true, bulletTravelDist, true, bulletSpeed, size: 42));

                    }
                }
                if (angle >= 115 && angle < 135)
                {
                    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                    {

                        bullets.Add(new Bullet(new Vector2((int)bossRect.X /*- (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2)*/,
                        (int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4)), -bulletSpeed, new Vector2(-bulletMaxX, -bulletMaxY),
                                   content, true, bulletTravelDist, true, -bulletSpeed / 4, size: 42));
                        //        boss.bulletRects.Add(new Rectangle((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        //(int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4), bossRect.Width, bossRect.Height));

                    }
                    else
                    {
                        //         boss.bulletRects.Add(new Rectangle((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        //(int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4), bossRect.Width, bossRect.Height));
                        bullets.Add(new Bullet(new Vector2((int)bossRect.X /*- (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2)*/,
                       (int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4)), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY),
                                  content, true, bulletTravelDist, true, bulletSpeed / 4, size: 42));

                    }
                }
                if (angle >= 135 && angle < 155)
                {
                    if (bossRect.Center.Y > tdPlayer.rectangle.Y)
                    {

                        bullets.Add(new Bullet(new Vector2((int)bossRect.X /*- (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2)*/,
                        (int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4)), -bulletSpeed / 2, new Vector2(-bulletMaxX, -bulletMaxY),
                                   content, true, bulletTravelDist, true, -bulletSpeed / 4, size: 42));
                        //        boss.bulletRects.Add(new Rectangle((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        //(int)bossRect.Y - (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4), bossRect.Width, bossRect.Height));

                        //        Rectangle startRect = boss.bulletRects[0];

                        //        while (boss.bulletRects[boss.bulletRects.Count - 1].X > LeftWalls.walls[0].Rectangle.Right &&
                        //          boss.bulletRects[boss.bulletRects.Count - 1].Top > TopWalls.walls[0].Rectangle.Bottom)
                        //        {
                        //            boss.bulletRects.Add(new Rectangle(startRect.X - bossRect.Width, startRect.Y - bossRect.Height / 4, bossRect.Width, bossRect.Height));
                        //            startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                        //        }
                    }
                    else
                    {

                        bullets.Add(new Bullet(new Vector2((int)bossRect.X /*- (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2)*/,
                       (int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4)), -bulletSpeed / 2, new Vector2(-bulletMaxX, bulletMaxY),
                                  content, true, bulletTravelDist, true, bulletSpeed / 4, size: 42));
                        //         boss.bulletRects.Add(new Rectangle((int)bossRect.X - (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2),
                        //(int)bossRect.Y + (int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4), bossRect.Width, bossRect.Height));

                        //         Rectangle startRect = boss.bulletRects[0];

                        //         while (boss.bulletRects[boss.bulletRects.Count - 1].X > LeftWalls.walls[0].Rectangle.Right &&
                        //           boss.bulletRects[boss.bulletRects.Count - 1].Bottom < BottomWalls.walls[0].Rectangle.Top)
                        //         {
                        //             boss.bulletRects.Add(new Rectangle(startRect.X - bossRect.Width, startRect.Bottom - bossRect.Height / 2, bossRect.Width, bossRect.Height));
                        //             startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                        //         }
                    }
                }

                if (angle >= 155 && angle < 170)
                {
                    if(bossRect.Center.Y > tdPlayer.rectangle.Y)
                    {
                        bullets.Add(new Bullet(new Vector2((int)bossRect.X /*- (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2)*/,
                     (int)bossRect.Y + ((int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4) - 21)), -bulletSpeed, new Vector2(-bulletMaxX, -bulletMaxY),
                                content, true, bulletTravelDist, true, -bulletSpeed / 6, size: 42));
                    }
                    else
                    {
                        bullets.Add(new Bullet(new Vector2((int)bossRect.X /*- (int)(tdPlayer.rectangle.Width / 2 + bossRect.Width / 2)*/,
                     (int)bossRect.Y + ((int)(tdPlayer.rectangle.Height / 2 + bossRect.Height / 4) - 21)), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY),
                                content, true, bulletTravelDist, true, bulletSpeed / 6, size: 42));
                    }

                   
                }
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

        public void Draw(SpriteBatch spriteBatch)
        {

            //foreach (GroundLoc loc in slamLocs)
            //{
            //    for (int i = 0; i < loc.slamTiles.Count; i++)
            //    {
            //        spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile0"), loc.slamTiles[i].Rectangle, Color.White);
            //    }

            //}
            spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), bossRect, Color.White);

            for(int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }

        }

        private void SetWalls(TopDownMap tdMap, int[,] map)
        {
            TopWalls.isUsed = false;
            BottomWalls.isUsed = false;
            RightWalls.isUsed = false;
            LeftWalls.isUsed = false;

            TopWalls.walls = new List<WallTiles>();
            BottomWalls.walls = new List<WallTiles>();
            LeftWalls.walls = new List<WallTiles>();
            RightWalls.walls = new List<WallTiles>();

            for (int i = tdMap.WallTiles.Count - 1; i >= 0; i--)
            {
                if (bounds.Intersects(tdMap.WallTiles[i].Rectangle) == false || bounds.Contains(tdMap.WallTiles[i].Rectangle) == false)
                {
                    tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                }
                else if (tdMap.GetPoint(tdMap.WallTiles[i].mapPoint[0], tdMap.WallTiles[i].mapPoint[1], map) == 10)
                {
                    tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                }
            }

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (y > 0 && x > 0)
                    {
                        //Possibly different list of walls that are in the map, boss reacts differently to them
                        if (tdMap.dMapDims[tdMap.dMapDims.Count - 1][y, x] == 10)
                        {
                            tdMap.dMapDims[tdMap.dMapDims.Count - 1][y, x] = 9;
                        }
                    }
                    if (y == 0 && x > 0 && x < map.GetLength(1) - 1 ||
                        y == map.GetLength(0) - 1 && x > 0 && x < map.GetLength(1) - 1)
                    {
                        if (map[y, x] != 10)
                        {
                            //Rectangle tRect = Rectangle.Empty;
                            foreach (WallTiles wall in tdMap.WallTiles)
                            {
                                if (wall.mapPoint[0] == y && wall.mapPoint[1] == x)
                                {
                                    if (y == 0)
                                    {
                                        TopWalls.walls.Add(wall);
                                    }
                                    else if (y == map.GetLength(0) - 1)
                                    {
                                        BottomWalls.walls.Add(wall);
                                    }
                                    //tRect = wall.Rectangle;
                                    JumpWalls.Add(wall);
                                    break;
                                }
                            }
                            //JumpWalls.Add(new WallTiles(map[y, x], 
                            //    tRect));

                            //JumpWalls.Add(new WallTiles(map[y, x],
                            //   new Rectangle(bounds.X + (x * 64), bounds.Y + (y * 64), 64, 64)));
                        }
                    }
                    if (y > 0 && y < map.GetLength(0) - 1 && x == 0 ||
                         x == map.GetLength(1) - 1 && y > 0 && y < map.GetLength(0) - 1)
                    {
                        if (map[y, x] != 10)
                        {
                            //Rectangle tRect = Rectangle.Empty;
                            foreach (WallTiles wall in tdMap.WallTiles)
                            {
                                if (wall.mapPoint[0] == y && wall.mapPoint[1] == x)
                                {
                                    if (x == 0)
                                    {
                                        LeftWalls.walls.Add(wall);
                                    }
                                    else if (x == map.GetLength(1) - 1)
                                    {
                                        RightWalls.walls.Add(wall);
                                    }

                                    JumpWalls.Add(wall);

                                    break;
                                }
                            }


                            //JumpWalls.Add(new WallTiles(map[y, x],
                            //   new Rectangle(bounds.X + (x * 64), bounds.Y + (y * 64), 64, 64)));
                        }
                    }
                }
            }
        }


        int DistForm(Vector2 pos1, Vector2 pos2)
        {
            int num = (int)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
            return num;

        }
    }

    struct WallSide
    {
        public List<WallTiles> walls;
        public bool isUsed;
    }
}
