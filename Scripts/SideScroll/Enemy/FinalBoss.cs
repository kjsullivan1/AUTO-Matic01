using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework.Content;
using AUTO_Matic.SideScroll;

namespace AUTO_Matic.Scripts.SideScroll.Enemy
{
    class FinalBoss
    {
        public Rectangle bossRect;

        List<Rectangle> WallRects;
        ContentManager content;
        Random random = new Random();

        List<ShootingLocs> shootLocs = new List<ShootingLocs>();
        List<Vector2> goToLocs = new List<Vector2>(); //locations the shootLocs will goTo
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

        bool clockWise = true;
        int rotateSpeed = 3;
        //public float angle;
        public List<Bullet> bullets = new List<Bullet>();
        float bulletSpeed = 5f;
        float bulletMaxX = 20f;
        float bulletMaxY = 20f;
        int spread = 3;
        bool isShootDelay = false;
        float shootDelay = 2.5f;//In seconds
        float iShootDelay;
        bool startShoot = false;
        public float bulletDmg = 1.5f;
        public float bulletTravelDist = 64 * 8;
        Texture2D visionTxture;
        int width;
        int height;
        Rectangle bounds;
        Rectangle tempRect;

        //PossibleJumpSide TopWalls;
        //PossibleJumpSide BottomWalls;
        //PossibleJumpSide RightWalls;
        //PossibleJumpSide LeftWalls;
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
        public FinalBoss(Vector2 pos, ContentManager content)
        {
            bossRect = new Rectangle(pos.ToPoint(), new Point(64 * sizeMod, 64 * sizeMod));

            this.content = content;

            int mod = 48;

            shootLocs.Add(new ShootingLocs(new Rectangle(bossRect.Right, bossRect.Y - (bossRect.Height - mod),
                                bossRect.Width - mod, bossRect.Height - mod)));
            shootLocs[0].dir = new Vector2(1, 1); //TopRight
            goToLocs.Add(new Vector2(bossRect.Right, bossRect.Y - (bossRect.Height - mod)));

            shootLocs.Add(new ShootingLocs(new Rectangle(bossRect.X - (bossRect.Width - mod), bossRect.Y - (bossRect.Height - mod),
                    bossRect.Width - mod, bossRect.Height - mod)));
            shootLocs[1].dir = new Vector2(-1, 1);//TopLeft
            goToLocs.Add(new Vector2(bossRect.X - (bossRect.Width - mod), bossRect.Y - (bossRect.Height - mod)));

            shootLocs.Add(new ShootingLocs(new Rectangle(bossRect.Right, bossRect.Bottom /*- (bossRect.Height - mod)*/,
                    bossRect.Width - mod, bossRect.Height - mod)));
            shootLocs[2].dir = new Vector2(1, -1);//BottomRight
            goToLocs.Add(new Vector2(bossRect.Right, bossRect.Bottom));

            shootLocs.Add(new ShootingLocs(new Rectangle(bossRect.X - (bossRect.Width - mod), bossRect.Bottom /*- (bossRect.Height - mod)*/,
                   bossRect.Width - mod, bossRect.Height - mod)));
            shootLocs[3].dir = new Vector2(-1, -1);//BottomLeft
            goToLocs.Add(new Vector2(bossRect.X - (bossRect.Width - mod), bossRect.Bottom));
        }
        #endregion
        
        public void Update(GameTime gameTime, SSPlayer ssPlayer)
        {
            foreach(ShootingLocs loc in shootLocs)
            {
                Vector2 locPos = new Vector2(loc.rect.X, loc.rect.Y);
                if(loc.dir == new Vector2(1,1))
                {
                    if(clockWise)
                    {
                        //if the y first and use else statements to guarentee y movement first 
                        if(locPos.Y < goToLocs[2].Y)
                        {
                            loc.rect.Y += rotateSpeed;
                            if (loc.rect.Y > goToLocs[2].Y)
                            {
                                loc.rect.Y = (int)goToLocs[2].Y;
                                loc.dir = new Vector2(1, -1);
                            }
                        }
                        else if (loc.rect.Y >= goToLocs[2].Y)
                        {
                            loc.rect.Y = (int)goToLocs[2].Y;
                            loc.dir = new Vector2(1, -1);
                        }

                    }
                    else
                    {
                        if(locPos.X > goToLocs[1].X)
                        {
                            loc.rect.X -= rotateSpeed;

                            if(loc.rect.X < goToLocs[1].X)
                            {
                                loc.rect.X = (int)goToLocs[1].X;
                                loc.dir = new Vector2(-1, 1);
                            }
                        }
                        else if (loc.rect.X <= goToLocs[1].X)
                        {
                            loc.rect.X = (int)goToLocs[1].X;
                            loc.dir = new Vector2(-1, 1);
                        }
                    }
                }
                else if(loc.dir == new Vector2(-1,1))
                {
                    if(clockWise)
                    {
                        if(locPos.X < goToLocs[0].X)
                        {
                            loc.rect.X += rotateSpeed;

                            if(loc.rect.X > goToLocs[0].X)
                            {
                                loc.rect.X = (int)goToLocs[0].X;
                                loc.dir = new Vector2(1, 1);
                            }
                        }
                        else if (loc.rect.X >= goToLocs[0].X)
                        {
                            loc.rect.X = (int)goToLocs[0].X;
                            loc.dir = new Vector2(1, 1);
                        }
                    }
                    else
                    {
                        if(locPos.Y < goToLocs[3].Y)
                        {
                            loc.rect.Y += rotateSpeed;

                            if(loc.rect.Y > goToLocs[3].Y)
                            {
                                loc.rect.Y = (int)goToLocs[3].Y;
                                loc.dir = new Vector2(-1, -1);
                            }
                        }
                        else if(loc.rect.Y >= goToLocs[3].Y)
                        {
                            loc.rect.Y = (int)goToLocs[3].Y;
                            loc.dir = new Vector2(-1, -1);
                        }
                    }
                }
                else if (loc.dir == new Vector2(-1, -1))
                {
                    if (clockWise)
                    {
                        if (locPos.Y > goToLocs[1].Y)
                        {
                            loc.rect.Y -= rotateSpeed;

                            if (loc.rect.Y < goToLocs[1].Y)
                            {
                                loc.rect.Y = (int)goToLocs[1].Y;
                                loc.dir = new Vector2(-1, 1);
                            }
                        }
                        else if (loc.rect.Y <= goToLocs[1].Y)
                        {
                            loc.rect.Y = (int)goToLocs[1].Y;
                            loc.dir = new Vector2(-1, 1);
                        }
                    }
                    else
                    {
             

                        if (locPos.X < goToLocs[2].X)
                        {
                            loc.rect.X += rotateSpeed;

                            if (loc.rect.X > goToLocs[2].X)
                            {
                                loc.rect.X = (int)goToLocs[2].X;
                                loc.dir = new Vector2(1, -1);
                            }
                        }
                        else if (loc.rect.X >= goToLocs[2].X)
                        {
                            loc.rect.X = (int)goToLocs[2].X;
                            loc.dir = new Vector2(1, -1);
                        }
                    }
                }
                else if (loc.dir == new Vector2(1, -1))
                {
                    if (clockWise)
                    {
                        if (locPos.X > goToLocs[3].X)
                        {
                            loc.rect.X -= rotateSpeed;

                            if (loc.rect.X < goToLocs[3].X)
                            {
                                loc.rect.X = (int)goToLocs[3].X;
                                loc.dir = new Vector2(-1, -1);
                            }
                        }
                        else if (loc.rect.X <= goToLocs[3].X)
                        {
                            loc.rect.X = (int)goToLocs[3].X;
                            loc.dir = new Vector2(-1, -1);
                        }

                    }
                    else
                    {
                
                        if (locPos.Y > goToLocs[0].Y)
                        {
                            loc.rect.Y -= rotateSpeed;

                            if (loc.rect.Y < goToLocs[0].Y)
                            {
                                loc.rect.Y = (int)goToLocs[0].Y;
                                loc.dir = new Vector2(1, 1);
                            }
                        }
                        else if (loc.rect.Y <= goToLocs[0].Y)
                        {
                            loc.rect.Y = (int)goToLocs[0].Y;
                            loc.dir = new Vector2(1, 1);
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), bossRect, Color.White);

            foreach(ShootingLocs loc in shootLocs)
            {
                spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), loc.rect, Color.White);
            }
        }
    }

    class ShootingLocs
    {
        public Rectangle rect;
        public string type;
        public Vector2 dir;

        public ShootingLocs(Rectangle rect)
        {
            this.rect = rect;
            dir = Vector2.Zero;
        }

    }
}
