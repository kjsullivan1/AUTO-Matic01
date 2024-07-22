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
    class LaserBoss
    {
        public Rectangle worldRect;

        public List<BossRect> bossRects = new List<BossRect>();
        ContentManager content;
        Random rand = new Random();
        //Rectangle bounds;

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

        PossibleJumpSide TopWalls;
        PossibleJumpSide BottomWalls;
        PossibleJumpSide RightWalls;
        PossibleJumpSide LeftWalls;
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
        public LaserBoss(Rectangle rect, ContentManager content, TopDownMap tdMap, int[,] map)
        {
            int size = 64 * sizeMod;
           
            worldRect = new Rectangle(((rect.X + rect.Width / 2) - size / 2), (((rect.Y + rect.Height / 2) - size / 2)), size, size);
            this.content = content;
            this.bounds = rect;
            iShootDelay = shootDelay;
          
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

            for(int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                   if(y > 0 && x > 0)
                    {
                       //Possibly different list of walls that are in the map, boss reacts differently to them
                        if(tdMap.dMapDims[tdMap.dMapDims.Count-1][y,x] == 10)
                        {
                            tdMap.dMapDims[tdMap.dMapDims.Count - 1][y, x] = 9;
                        }
                    }
                    if(y == 0 && x > 0 && x < map.GetLength(1) - 1 || 
                        y == map.GetLength(0) - 1 && x > 0 && x < map.GetLength(1) - 1)
                    {
                        if(map[y,x] != 10)
                        {
                            //Rectangle tRect = Rectangle.Empty;
                            foreach(WallTiles wall in tdMap.WallTiles)
                            {
                                if(wall.mapPoint[0] == y && wall.mapPoint[1] == x)
                                {
                                    if(y == 0)
                                    {
                                        TopWalls.walls.Add(wall);
                                    }
                                    else if(y == map.GetLength(0) - 1)
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
                   if(y > 0 && y < map.GetLength(0) - 1 && x == 0 ||
                        x == map.GetLength(1) - 1 && y > 0 && y < map.GetLength(0) - 1)
                    {
                        if (map[y, x] != 10)
                        {
                            //Rectangle tRect = Rectangle.Empty;
                            foreach (WallTiles wall in tdMap.WallTiles)
                            {
                                if (wall.mapPoint[0] == y && wall.mapPoint[1] == x)
                                {
                                    if(x == 0)
                                    {
                                        LeftWalls.walls.Add(wall);
                                    }
                                    else if(x == map.GetLength(1) - 1)
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
            for (int i = 0; i < 3; i++)
            {
                while(true)
                {
                    int num = rand.Next(0, 4);
                    if (!TopWalls.isUsed && num == 0)
                    {
                        bossRects.Add(new BossRect(TopWalls, worldRect, "top"));
                        TopWalls.isUsed = true;
                        break;
                    }
                    else if (!RightWalls.isUsed && num == 1)
                    {
                        bossRects.Add(new BossRect(RightWalls, worldRect, "right"));
                        RightWalls.isUsed = true;
                        break;
                    }
                    else if (!BottomWalls.isUsed && num == 2)
                    {
                        bossRects.Add(new BossRect(BottomWalls, worldRect, "bottom"));
                        BottomWalls.isUsed = true;
                        break;
                    }
                    else if (!LeftWalls.isUsed && num == 3)
                    {
                        bossRects.Add(new BossRect(LeftWalls, worldRect, "left"));
                        LeftWalls.isUsed = true;
                        break;
                    }
                    else
                    {
                        bossRects.Add(new BossRect(RightWalls, worldRect, "right"));
                        RightWalls.isUsed = true;
                        break;
                    }
                }
                
               
                
            }

        }
        #endregion

        public void Update(GameTime gameTime, TDPlayer tdPlayer, TopDownMap tdMap)
        {
            foreach(BossRect boss in bossRects)
            {
                switch(boss.state)
                {
                    #region Idle
                    case BossRect.BossState.Idle:
                        switch(boss.side)
                        {
                            case "top":
                                for(int i = 0; i < boss.jumpSide.walls.Count - 1; i++)
                                {
                                    if(boss.jumpSide.walls[i].mapPoint[1] == 
                                        tdMap.dMapDims[tdMap.dMapDims.Count-1].GetLength(1)/2)
                                    {
                                        boss.destinationRect = boss.jumpSide.walls[i].Rectangle;
                                        boss.destinationRect.X -= boss.rect.Width / (2 * sizeMod);
                                    }
                                }
                                break;
                            case "bottom":
                                for (int i = 0; i < boss.jumpSide.walls.Count - 1; i++)
                                {
                                    if (boss.jumpSide.walls[i].mapPoint[1] ==
                                       tdMap.dMapDims[tdMap.dMapDims.Count - 1].GetLength(1) / 2)
                                    {
                                        boss.destinationRect = boss.jumpSide.walls[i].Rectangle;
                                        boss.destinationRect.X -= boss.rect.Width / (2 * sizeMod);
                                    }
                                }
                                break;
                            case "right":
                                for (int i = 0; i < boss.jumpSide.walls.Count - 1; i++)
                                {
                                    if (boss.jumpSide.walls[i].mapPoint[0] ==
                                        tdMap.dMapDims[tdMap.dMapDims.Count - 1].GetLength(0) / 2)
                                    {
                                        boss.destinationRect = boss.jumpSide.walls[i].Rectangle;
                                        boss.destinationRect.Y -= boss.rect.Height / (2 * sizeMod);
                                    }
                                }
                                break;
                            case "left":
                                for (int i = 0; i < boss.jumpSide.walls.Count - 1; i++)
                                {
                                    if (boss.jumpSide.walls[i].mapPoint[0] ==
                                         tdMap.dMapDims[tdMap.dMapDims.Count - 1].GetLength(0) / 2)
                                    {
                                        boss.destinationRect = boss.jumpSide.walls[i].Rectangle;
                                        boss.destinationRect.Y -= boss.rect.Height / (2 * sizeMod);
                                    }
                                }
                                break;
                        }
                        boss.state = BossRect.BossState.EnterWall;
                        break;
                    #endregion
                    #region EnterWall
                    case BossRect.BossState.EnterWall:
                        boss.rect = new Rectangle((int)(boss.rect.X + boss.velocity.X), (int)(boss.rect.Y + boss.velocity.Y),
                            boss.rect.Width, boss.rect.Height);
                        int count = 0;

                        count = MoveToWall(boss, count);

                        if (count >= 1 && boss.prevState != BossRect.BossState.Attack)
                        {
                            boss.state = BossRect.BossState.InWall;
                        }
                        if(boss.prevState == BossRect.BossState.Attack)
                        {
                            if(boss.side == "top")
                            {
                                if(boss.rect.Center.Y <= TopWalls.walls[0].Rectangle.Bottom)
                                {
                                    //boss.rect.Y = TopWalls.walls[0].Rectangle.Bottom + boss.rect.Height/4;
                                    boss.state = BossRect.BossState.InWall;
                                }
                            }
                            if (boss.side == "bottom")
                            {
                                if (boss.rect.Center.Y >= BottomWalls.walls[0].Rectangle.Top)
                                {
                                    //boss.rect.Y = BottomWalls.walls[0].Rectangle.Top - boss.rect.Height / 4;
                                    boss.state = BossRect.BossState.InWall;
                                }
                            }
                            if (boss.side == "right")
                            {
                                if (boss.rect.Center.X >= RightWalls.walls[0].Rectangle.Left)
                                {
                                    //boss.rect.X = RightWalls.walls[0].Rectangle.Left - boss.rect.Width / 4;
                                    boss.state = BossRect.BossState.InWall;
                                }
                            }
                            if (boss.side == "left")
                            {
                                if (boss.rect.Center.X <= LeftWalls.walls[0].Rectangle.Right)
                                {
                                    //boss.rect.X = LeftWalls.walls[0].Rectangle.Right + boss.rect.Width / 4;
                                    boss.state = BossRect.BossState.InWall;
                                }
                            }

                        }
                        break;
                    #endregion
                    #region InWall
                    case BossRect.BossState.InWall:
                        boss.rect = new Rectangle((int)(boss.rect.X + boss.velocity.X), (int)(boss.rect.Y + boss.velocity.Y),
                           boss.rect.Width, boss.rect.Height);
                        Vector2 targetPos = new Vector2(tdPlayer.rectangle.X + tdPlayer.rectangle.Width / 2,
                            tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2);
                        Vector2 bossPos = new Vector2((boss.rect.X + boss.rect.Width / 2) - tdPlayer.rectangle.Width / 2,
                            (boss.rect.Y + boss.rect.Height / 2) - tdPlayer.rectangle.Height / 2);

                        MoveInWall(tdPlayer, boss, bossPos);

                        shootDelay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds/100;
                        if(shootDelay <= 0)
                        {
                            shootDelay = RandFloat(1,6);
                            boss.jumpForce = RandFloat(6, 12);
                            
                            boss.state = BossRect.BossState.Attack;
                            
                        }

                        break;
                    #endregion
                    #region Attack
                    case BossRect.BossState.Attack:
                        boss.rect = new Rectangle((int)(boss.rect.X + boss.velocity.X), (int)(boss.rect.Y + boss.velocity.Y),
                                      boss.rect.Width, boss.rect.Height);
                        targetPos = new Vector2(tdPlayer.rectangle.X + tdPlayer.rectangle.Width / 2,
                            tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2);

                        Vector2 targetDir = new Vector2(tdPlayer.rectangle.X + tdPlayer.rectangle.Width/2, tdPlayer.rectangle.Y + tdPlayer.rectangle.Height/2) -
                            new Vector2(boss.rect.X + boss.rect.Width/2, boss.rect.Y + boss.rect.Height/2);
                        boss.lingerTime = RandFloat(1, 3);
                        switch (boss.side)
                        {
                            case "top":
                                if(boss.jumpForce != 0)
                                {
                                    boss.velocity.Y = boss.jumpForce;
                                    //boss.velocity.X = 0;
                                    boss.jumpForce = 0;
                                    boss.hasWall = false;
                                    boss.moveDir = boss.velocity;
                                    TopWalls.isUsed = false;
                                }
                                else
                                {
                                    boss.angle = Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(targetDir.Y, targetDir.X)));
                                    if (boss.rect.Y + boss.velocity.Y < targetPos.Y)
                                    {
                                       foreach(WallTiles wall in RightWalls.walls)
                                        {
                                            if(boss.rect.Intersects(wall.Rectangle))
                                            {
                                                boss.prevState = boss.state;
                                                boss.state = BossRect.BossState.Fire;
                                                boss.velocity = Vector2.Zero;

                                                if(RightWalls.isUsed == false)
                                                {
                                                    boss.hasWall = true;
                                                    boss.side = "right";
                                                    boss.jumpSide = RightWalls;
                                                    RightWalls.isUsed = true;
                                                }
                                               
                                                break;
                                            }
                                        }

                                       foreach(WallTiles wall in LeftWalls.walls)
                                        {
                                            if(boss.rect.Intersects(wall.Rectangle))
                                            {
                                                boss.prevState = boss.state;
                                                boss.state = BossRect.BossState.Fire;
                                                boss.velocity = Vector2.Zero;

                                                if(LeftWalls.isUsed == false)
                                                {
                                                    boss.side = "left";
                                                    boss.hasWall = true;
                                                    boss.jumpSide = LeftWalls;
                                                    LeftWalls.isUsed = true;
                                                }
                                              
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        boss.prevState = boss.state;
                                        boss.state = BossRect.BossState.Fire;
                                        boss.velocity = Vector2.Zero;
                                        boss.bulletRects.Clear();
                                    }
                                }
                                break;
                            case "bottom":
                                if(boss.jumpForce != 0)
                                {
                                    boss.velocity.Y = -boss.jumpForce;
                                    //boss.velocity.X = 0;
                                    boss.jumpForce = 0;
                                    boss.hasWall = false;
                                    boss.moveDir = boss.velocity;
                                    BottomWalls.isUsed = false;
                                }
                                else
                                {
                                    boss.angle = Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(targetDir.Y, targetDir.X)));
                                    if (boss.rect.Y + boss.velocity.Y > targetPos.Y)
                                    {
                                        foreach (WallTiles wall in RightWalls.walls)
                                        {
                                            if (boss.rect.Intersects(wall.Rectangle))
                                            {
                                                boss.prevState = boss.state;
                                                boss.state = BossRect.BossState.Fire;
                                                boss.velocity = Vector2.Zero;

                                                if(RightWalls.isUsed == false)
                                                {
                                                    boss.hasWall = true;
                                                    boss.side = "right";
                                                    boss.jumpSide = RightWalls;
                                                    RightWalls.isUsed = true;
                                                }
                                               
                                                break;
                                            }
                                        }

                                        foreach (WallTiles wall in LeftWalls.walls)
                                        {
                                            if (boss.rect.Intersects(wall.Rectangle))
                                            {
                                                boss.prevState = boss.state;
                                                boss.state = BossRect.BossState.Fire;
                                                boss.velocity = Vector2.Zero;

                                                if(LeftWalls.isUsed == false)
                                                {
                                                    boss.side = "left";
                                                    boss.hasWall = true;
                                                    boss.jumpSide = LeftWalls;
                                                    LeftWalls.isUsed = true;
                                                }
                                            
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        boss.prevState = boss.state;
                                        boss.state = BossRect.BossState.Fire;
                                        boss.velocity = Vector2.Zero;
                                        boss.bulletRects.Clear();
                                    }
                                }
                                break;
                            case "right":
                                if(boss.jumpForce !=0)
                                {
                                    boss.velocity.X = -boss.jumpForce;
                                    //boss.velocity.Y = 0;
                                    boss.jumpForce = 0;
                                    boss.hasWall = false;
                                    boss.moveDir = boss.velocity;
                                    RightWalls.isUsed = false;
                                }
                                else
                                {
                                    boss.angle = Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(targetDir.Y, targetDir.X)));
                                    if (boss.rect.X + boss.velocity.X > targetPos.X)
                                    {
                                        foreach (WallTiles wall in TopWalls.walls)
                                        {
                                            if (boss.rect.Intersects(wall.Rectangle))
                                            {
                                                boss.prevState = boss.state;
                                                boss.state = BossRect.BossState.Fire;
                                                boss.velocity = Vector2.Zero;

                                                if(TopWalls.isUsed == false)
                                                {
                                                    boss.hasWall = true;
                                                    boss.side = "top";
                                                    boss.jumpSide = TopWalls;
                                                    TopWalls.isUsed = true;
                                                }
                                              
                                                break;
                                            }
                                        }

                                        foreach (WallTiles wall in BottomWalls.walls)
                                        {
                                            if (boss.rect.Intersects(wall.Rectangle))
                                            {
                                                boss.prevState = boss.state;
                                                boss.state = BossRect.BossState.Fire;
                                                boss.velocity = Vector2.Zero;

                                                if(BottomWalls.isUsed == false)
                                                {
                                                    boss.side = "bottom";
                                                    boss.hasWall = true;
                                                    boss.jumpSide = BottomWalls;
                                                    BottomWalls.isUsed = true;
                                                }
                                               
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        boss.prevState = boss.state;
                                        boss.state = BossRect.BossState.Fire;
                                        boss.velocity = Vector2.Zero;
                                        boss.bulletRects.Clear();
                                    }
                                }
                                break;
                            case "left":
                                if(boss.jumpForce != 0)
                                {
                                    boss.velocity.X = boss.jumpForce;
                                   // boss.velocity.Y = 0;
                                    boss.jumpForce = 0;
                                    boss.hasWall = false;
                                    boss.moveDir = boss.velocity;
                                    LeftWalls.isUsed = false;
                                }
                                else
                                {
                                    boss.angle = Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(targetDir.Y, targetDir.X)));
                                    if (boss.rect.X + boss.velocity.X < targetPos.X)
                                    {
                                        foreach (WallTiles wall in TopWalls.walls)
                                        {
                                            if (boss.rect.Intersects(wall.Rectangle))
                                            {
                                                boss.prevState = boss.state;
                                                boss.state = BossRect.BossState.Fire;
                                                boss.velocity = Vector2.Zero;

                                                if(TopWalls.isUsed == false)
                                                {
                                                    boss.hasWall = true;
                                                    boss.side = "top";
                                                    boss.jumpSide = TopWalls;
                                                    TopWalls.isUsed = true;
                                                }
                                                
                                                break;
                                            }
                                        }

                                        foreach (WallTiles wall in BottomWalls.walls)
                                        {
                                            if (boss.rect.Intersects(wall.Rectangle))
                                            {
                                                boss.prevState = boss.state;
                                                boss.state = BossRect.BossState.Fire;
                                                boss.velocity = Vector2.Zero;

                                                if(BottomWalls.isUsed == false)
                                                {
                                                    boss.side = "bottom";
                                                    boss.hasWall = true;
                                                    boss.jumpSide = BottomWalls;
                                                    BottomWalls.isUsed = true;
                                                }
                                              
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        boss.prevState = boss.state;
                                        boss.state = BossRect.BossState.Fire;
                                        boss.velocity = Vector2.Zero;
                                        boss.bulletRects.Clear();
                                    }
                                }
                                break;
                        }
                        break;
                    #endregion
                    #region Fire
                    case BossRect.BossState.Fire:

                        targetDir = new Vector2(tdPlayer.rectangle.X + tdPlayer.rectangle.Width / 2, tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2) -
                            new Vector2(boss.rect.X + boss.rect.Width / 2, boss.rect.Y + boss.rect.Height / 2);
                        boss.angle = Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(targetDir.Y, targetDir.X)));
                        bossPos = new Vector2((boss.rect.X + boss.rect.Width / 2) - tdPlayer.rectangle.Width / 2,
                           (boss.rect.Y + boss.rect.Height / 2) - tdPlayer.rectangle.Height / 2);
                        //boss.bulletRects.Clear();
                        if(boss.bulletRects.Count == 0)
                        {
                            SetRay(boss.angle, tdPlayer, boss, bossPos);
                        }
                  

                        boss.lingerTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if(boss.lingerTime <= 0)
                        {
                            boss.bulletRects.Clear();
                            if(boss.hasWall == false)
                            {
                                if(boss.moveDir.X >= 0 && boss.moveDir.Y >=0)
                                {
                                    if (MathHelper.Distance(boss.rect.Bottom, BottomWalls.walls[0].Rectangle.Top) <
                                        MathHelper.Distance(boss.rect.Right, RightWalls.walls[0].Rectangle.Left) && BottomWalls.isUsed == false)
                                    {
                                        boss.jumpSide = BottomWalls;
                                        boss.side = "bottom";
                                        boss.hasWall = true;
                                        BottomWalls.isUsed = true;
                                    }
                                    else if (RightWalls.isUsed == false)
                                    {
                                        boss.jumpSide = RightWalls;
                                        boss.side = "right";
                                        boss.hasWall = true;
                                        RightWalls.isUsed = true;
                                    }
                                }
                                if(boss.moveDir.X <= 0 && boss.moveDir.Y <= 0)
                                {
                                    if (MathHelper.Distance(boss.rect.Top, TopWalls.walls[0].Rectangle.Bottom) <
                                        MathHelper.Distance(boss.rect.Left, LeftWalls.walls[0].Rectangle.Right) && TopWalls.isUsed == false)
                                    {
                                        boss.jumpSide = TopWalls;
                                        boss.side = "top";
                                        boss.hasWall = true;
                                        TopWalls.isUsed = false;
                                    }
                                    else if(LeftWalls.isUsed == false)
                                    {
                                        boss.jumpSide = LeftWalls;
                                        boss.side = "left";
                                        boss.hasWall = true;
                                        LeftWalls.isUsed = true;
                                    }
                                }
                                if(boss.moveDir.X >= 0 && boss.moveDir.Y <= 0)
                                {
                                    if (MathHelper.Distance(boss.rect.Top, TopWalls.walls[0].Rectangle.Bottom) <
                                       MathHelper.Distance(boss.rect.Right, RightWalls.walls[0].Rectangle.Left) && TopWalls.isUsed == false)
                                    {
                                        boss.jumpSide = TopWalls;
                                        boss.side = "top";
                                        boss.hasWall = true;
                                        TopWalls.isUsed = false;
                                    }
                                    else if(RightWalls.isUsed == false)
                                    {
                                        boss.jumpSide = RightWalls;
                                        boss.side = "right";
                                        boss.hasWall = true;
                                        RightWalls.isUsed = true;
                                    }
                                }
                                if(boss.moveDir.X <= 0 && boss.moveDir.Y >= 0)
                                {
                                    if (MathHelper.Distance(boss.rect.Bottom, BottomWalls.walls[0].Rectangle.Top) <
                                       MathHelper.Distance(boss.rect.Left, LeftWalls.walls[0].Rectangle.Right) && BottomWalls.isUsed)
                                    {
                                        boss.jumpSide = BottomWalls;
                                        boss.side = "bottom";
                                        boss.hasWall = true;
                                        BottomWalls.isUsed = true;
                                    }
                                    else if(LeftWalls.isUsed == false)
                                    {
                                        boss.jumpSide = LeftWalls;
                                        boss.side = "left";
                                        boss.hasWall = true;
                                        LeftWalls.isUsed = true;
                                    }
                                }
                            }
                            boss.state = BossRect.BossState.Idle;
                        }
                       
                        break;
                        #endregion
                }
            }
        }
        private static void MoveInWall(TDPlayer tdPlayer, BossRect boss, Vector2 bossPos)
        {
            switch (boss.side)
            {
                case "top":
                    boss.velocity.Y = 0;
                    if (bossPos.X < tdPlayer.rectangle.X)
                    {
                        if (bossPos.X + boss.velocity.X < tdPlayer.rectangle.X)
                        {
                            if (boss.velocity.X < 0)
                            {
                                boss.velocity.X = -boss.velocity.X;
                                boss.velocity.X += boss.moveSpeed;
                                if (boss.velocity.X > boss.maxSpeed / 2)
                                {
                                    boss.velocity.X = boss.maxSpeed / 2;
                                }
                            }
                            else
                            {
                                boss.velocity.X += boss.moveSpeed;
                                if (boss.velocity.X > boss.maxSpeed / 2)
                                {
                                    boss.velocity.X = boss.maxSpeed / 2;
                                }

                            }
                        }
                        else
                        {
                            boss.velocity.X = 0;
                        }


                    }

                    if (bossPos.X > tdPlayer.rectangle.X)
                    {
                        if (bossPos.X + boss.velocity.X > tdPlayer.rectangle.X)
                        {
                            if (boss.velocity.X > 0)
                            {
                                boss.velocity.X = -boss.velocity.X;
                                boss.velocity.X += -boss.moveSpeed;
                                if (boss.velocity.X < -boss.maxSpeed / 2)
                                {
                                    boss.velocity.X = -boss.maxSpeed / 2;
                                }
                            }
                            else
                            {
                                boss.velocity.X += -boss.moveSpeed;
                                if (boss.velocity.X < -boss.maxSpeed / 2)
                                {
                                    boss.velocity.X = -boss.maxSpeed / 2;
                                }
                            }
                        }
                        else
                        {
                            boss.velocity.X = 0;
                        }
                    }
                    break;
                case "bottom":
                    boss.velocity.Y = 0;
                    if (bossPos.X < tdPlayer.rectangle.X)
                    {
                        if (bossPos.X + boss.velocity.X < tdPlayer.rectangle.X)
                        {
                            if (boss.velocity.X < 0)
                            {
                                boss.velocity.X = -boss.velocity.X;
                                boss.velocity.X += boss.moveSpeed;
                                if (boss.velocity.X > boss.maxSpeed / 2)
                                {
                                    boss.velocity.X = boss.maxSpeed / 2;
                                }
                            }
                            else
                            {
                                boss.velocity.X += boss.moveSpeed;
                                if (boss.velocity.X > boss.maxSpeed / 2)
                                {
                                    boss.velocity.X = boss.maxSpeed / 2;
                                }

                            }
                        }
                        else
                        {
                            boss.velocity.X = 0;
                        }


                    }

                    if (bossPos.X > tdPlayer.rectangle.X)
                    {
                        if (bossPos.X + boss.velocity.X > tdPlayer.rectangle.X)
                        {
                            if (boss.velocity.X > 0)
                            {
                                boss.velocity.X = -boss.velocity.X;
                                boss.velocity.X += -boss.moveSpeed;
                                if (boss.velocity.X < -boss.maxSpeed / 2)
                                {
                                    boss.velocity.X = -boss.maxSpeed / 2;
                                }
                            }
                            else
                            {
                                boss.velocity.X += -boss.moveSpeed;
                                if (boss.velocity.X < -boss.maxSpeed / 2)
                                {
                                    boss.velocity.X = -boss.maxSpeed / 2;
                                }
                            }
                        }
                        else
                        {
                            boss.velocity.X = 0;
                        }
                    }
                    break;
                case "right":
                    boss.velocity.X = 0;

                    if (bossPos.Y < tdPlayer.rectangle.Y)
                    {
                        if (bossPos.Y + boss.velocity.Y < tdPlayer.rectangle.Y)
                        {
                            if (boss.velocity.Y < 0)
                            {
                                boss.velocity.Y = -boss.velocity.Y;
                                boss.velocity.Y += boss.moveSpeed;
                                if (boss.velocity.Y > boss.maxSpeed / 2)
                                {
                                    boss.velocity.Y = boss.maxSpeed / 2;
                                }
                            }
                            else
                            {
                                boss.velocity.Y += boss.moveSpeed;
                                if (boss.velocity.Y > boss.maxSpeed / 2)
                                {
                                    boss.velocity.Y = boss.maxSpeed / 2;
                                }

                            }
                        }
                        else
                        {
                            boss.velocity.Y = 0;
                        }


                    }

                    if (bossPos.Y > tdPlayer.rectangle.Y)
                    {
                        if (bossPos.Y + boss.velocity.Y > tdPlayer.rectangle.Y)
                        {
                            if (boss.velocity.Y > 0)
                            {
                                boss.velocity.Y = -boss.velocity.Y;
                                boss.velocity.Y += -boss.moveSpeed;
                                if (boss.velocity.Y < -boss.maxSpeed / 2)
                                {
                                    boss.velocity.Y = -boss.maxSpeed / 2;
                                }
                            }
                            else
                            {
                                boss.velocity.Y += -boss.moveSpeed;
                                if (boss.velocity.Y < -boss.maxSpeed / 2)
                                {
                                    boss.velocity.Y = -boss.maxSpeed / 2;
                                }
                            }
                        }
                        else
                        {
                            boss.velocity.Y = 0;
                        }
                    }
                    break;
                case "left":
                    boss.velocity.X = 0;
                    if (bossPos.Y < tdPlayer.rectangle.Y)
                    {
                        if (bossPos.Y + boss.velocity.Y < tdPlayer.rectangle.Y)
                        {
                            if (boss.velocity.Y < 0)
                            {
                                boss.velocity.Y = -boss.velocity.Y;
                                boss.velocity.Y += boss.moveSpeed;
                                if (boss.velocity.Y > boss.maxSpeed / 2)
                                {
                                    boss.velocity.Y = boss.maxSpeed / 2;
                                }
                            }
                            else
                            {
                                boss.velocity.Y += boss.moveSpeed;
                                if (boss.velocity.Y > boss.maxSpeed / 2)
                                {
                                    boss.velocity.Y = boss.maxSpeed / 2;
                                }

                            }
                        }
                        else
                        {
                            boss.velocity.Y = 0;
                        }


                    }

                    if (bossPos.Y > tdPlayer.rectangle.Y)
                    {
                        if (bossPos.Y + boss.velocity.Y > tdPlayer.rectangle.Y)
                        {
                            if (boss.velocity.Y > 0)
                            {
                                boss.velocity.Y = -boss.velocity.Y;
                                boss.velocity.Y += -boss.moveSpeed;
                                if (boss.velocity.Y < -boss.maxSpeed / 2)
                                {
                                    boss.velocity.Y = -boss.maxSpeed / 2;
                                }
                            }
                            else
                            {
                                boss.velocity.Y += -boss.moveSpeed;
                                if (boss.velocity.Y < -boss.maxSpeed / 2)
                                {
                                    boss.velocity.Y = -boss.maxSpeed / 2;
                                }
                            }
                        }
                        else
                        {
                            boss.velocity.Y = 0;
                        }
                    }
                    break;
            }
        }
        private static int MoveToWall(BossRect boss, int count)
        {
            if (boss.rect.X < boss.destinationRect.X)
            {
                if (boss.rect.X + boss.velocity.X < boss.destinationRect.X)
                {
                    if (boss.velocity.X < 0)
                    {
                        boss.velocity.X = -boss.velocity.X;
                        boss.velocity.X += boss.moveSpeed;
                        if (boss.velocity.X > boss.maxSpeed)
                        {
                            boss.velocity.X = boss.maxSpeed;
                        }
                    }
                    else
                    {
                        boss.velocity.X += boss.moveSpeed;
                        if (boss.velocity.X > boss.maxSpeed)
                        {
                            boss.velocity.X = boss.maxSpeed;
                        }

                    }

                }
                else
                {

                    boss.rect.X = boss.destinationRect.X - boss.rect.Width / 2;
                    boss.velocity.X = 0;
                    count++;
                }
            }
            if (boss.rect.X > boss.destinationRect.X)
            {
                if (boss.rect.X + boss.velocity.X > boss.destinationRect.X)
                {
                    if (boss.velocity.X > 0)
                    {
                        boss.velocity.X = -boss.velocity.X;
                        boss.velocity.X += -boss.moveSpeed;
                        if (boss.velocity.X < -boss.maxSpeed)
                        {
                            boss.velocity.X = -boss.maxSpeed;
                        }
                    }
                    else
                    {
                        //boss.velocity.X = -boss.velocity.X;
                        boss.velocity.X += -boss.moveSpeed;
                        if (boss.velocity.X < -boss.maxSpeed)
                        {
                            boss.velocity.X = -boss.maxSpeed;
                        }
                    }

                }
                else
                {
                    boss.rect.X = boss.destinationRect.X;//- boss.rect.Width/2;
                    boss.velocity.X = 0;
                    count++;
                }
            }
            //else if(boss.prevState == BossRect.BossState.Attack)
            //{
            //    count++;
            //    boss.prevState = BossRect.BossState.Idle;
            //}

            if (boss.rect.Y < boss.destinationRect.Y)
            {
                if (boss.rect.Y + boss.velocity.Y < boss.destinationRect.Y)
                {
                    //While loop the if statement changing the speed until it gets to less than 1 making it a slowdown
                    if (boss.velocity.Y < 0)
                    {
                        boss.velocity.Y = -boss.velocity.Y;
                        boss.velocity.Y += boss.moveSpeed;
                        if (boss.velocity.Y > boss.maxSpeed)
                        {
                            boss.velocity.Y = boss.maxSpeed;
                        }
                    }
                    else
                    {
                        boss.velocity.Y += boss.moveSpeed;
                        if (boss.velocity.Y > boss.maxSpeed)
                        {
                            boss.velocity.Y = boss.maxSpeed;
                        }
                    }
                }
                else
                {
                    boss.rect.Y = boss.destinationRect.Y - boss.rect.Height / 2;
                    boss.velocity.Y = 0;
                    count++;
                }
            }
            if (boss.rect.Y > boss.destinationRect.Y)
            {
                if (boss.rect.Y + boss.velocity.Y > boss.destinationRect.Y)
                {
                    if (boss.velocity.Y > 0)
                    {
                        boss.velocity.Y = -boss.velocity.Y;
                        boss.velocity.Y += -boss.moveSpeed;
                        if (boss.velocity.Y < -boss.maxSpeed)
                        {
                            boss.velocity.Y = -boss.maxSpeed;
                        }
                    }
                    else
                    {
                        boss.velocity.Y += -boss.moveSpeed;
                        if (boss.velocity.Y < -boss.maxSpeed)
                        {
                            boss.velocity.Y = -boss.maxSpeed;
                        }
                    }
                }
                else
                {
                    boss.rect.Y = boss.destinationRect.Y;//- boss.rect.Height/2;
                    boss.velocity.Y = 0;
                    count++;
                }
            }
            else if(count == 1)
            {
                count++;
            }

            return count;
        }

        

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Health > 0)
            {
                //worldRect = new Rectangle(((bounds.X + bounds.Width / 2) - worldRect.Width / 2), (((bounds.Y + bounds.Height / 2) -  / 2)), width, height);
                //spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), worldRect, Color.White);
                int i = 0;
                foreach(BossRect boss in bossRects)
                {
                    if(i == 0)
                    {
                        spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), boss.rect, Color.White);
                        foreach (Rectangle rect in boss.bulletRects)
                        {
                            spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), rect, Color.White);
                        }
                        i++;
                    }
                    else if(i ==1)
                    {
                        spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), boss.rect, Color.Black);
                        foreach (Rectangle rect in boss.bulletRects)
                        {
                            spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), rect, Color.White);
                        }
                        i++;
                    }
                    else if(i == 2)
                    {
                        spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), boss.rect, Color.CornflowerBlue);
                        foreach (Rectangle rect in boss.bulletRects)
                        {
                            spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), rect, Color.White);
                        }
                        i++;
                    }
                    else if(i == 3)
                    {
                        spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), boss.rect, Color.DarkGreen);
                        foreach (Rectangle rect in boss.bulletRects)
                        {
                            spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), rect, Color.White);
                        }
                        i++;
                    }
                    else
                    {
                        spriteBatch.Draw(content.Load<Texture2D>("TopDown/MapTiles/Tile11"), boss.rect, Color.DarkGreen);
                        foreach (Rectangle rect in boss.bulletRects)
                        {
                            spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), rect, Color.White);
                        }
                        i++;
                    }
                  
                    
                }
                foreach (Bullet bullet in bullets)
                {
                    bullet.Draw(spriteBatch);
                }
               
            }

            //foreach (FloorTiles tile in floors)
            //{
            //    tile.Draw(spriteBatch);
            //}
            //spriteBatch.Draw(content.Load<Texture2D>("TopDown/Textures/Player"), slam.Position, slam.Bounds, Color.White * .5f);
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

        void SetRay(float angle, TDPlayer playerRect, BossRect boss, Vector2 bossPos)
        {
            int size = 64;
            if (angle < 16 || angle >= 155)//Right
            {
                if (angle < 16)
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X + (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 2),
                        (int)bossPos.Y /*- (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 2)*/, size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].Right < RightWalls.walls[0].Rectangle.X)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.Right, startRect.Y, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
                else//Left
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X - (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 2),
                        (int)bossPos.Y /*- (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 2)*/, size, size));
                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].X > LeftWalls.walls[0].Rectangle.X)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.X - size, startRect.Y, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
            }
            if (angle >= 16 && angle < 35)//Right up
            {
                if (boss.rect.Center.Y > playerRect.rectangle.Y)
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X + (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 2),
               (int)bossPos.Y - (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 4), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].Right < RightWalls.walls[0].Rectangle.X &&
                        boss.bulletRects[boss.bulletRects.Count - 1].Top > TopWalls.walls[0].Rectangle.Top)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.Right, startRect.Y - size / 4, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
                else
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X + (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 2),
              (int)bossPos.Y + (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 4), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].Right < RightWalls.walls[0].Rectangle.X &&
                      boss.bulletRects[boss.bulletRects.Count - 1].Bottom < BottomWalls.walls[0].Rectangle.Top)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.Right, startRect.Bottom - size / 2, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }

                //Compare pos for down
            }
            if (angle >= 35 && angle < 75)//more right up
            {
                if (boss.rect.Center.Y > playerRect.rectangle.Y)
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X + (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 4),
               (int)bossPos.Y - (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 2), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].Right < RightWalls.walls[0].Rectangle.X &&
                      boss.bulletRects[boss.bulletRects.Count - 1].Top > TopWalls.walls[0].Rectangle.Top)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.Right, startRect.Y - size, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
                else
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X + (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 4),
              (int)bossPos.Y + (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 2), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].Right < RightWalls.walls[0].Rectangle.X &&
                      boss.bulletRects[boss.bulletRects.Count - 1].Bottom < BottomWalls.walls[0].Rectangle.Top)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.Right, startRect.Bottom, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
            }
            if (angle >= 75 && angle < 105)//up
            {
                if (boss.rect.Center.Y > playerRect.rectangle.Y)
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X /*+ (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 4)*/,
             (int)bossPos.Y - (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 2), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (/*boss.bulletRects[boss.bulletRects.Count - 1].Right < RightWalls.walls[0].Rectangle.X &&*/
                      boss.bulletRects[boss.bulletRects.Count - 1].Top > TopWalls.walls[0].Rectangle.Top)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.X, startRect.Y - size, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
                else
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X /*+ (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 4)*/,
             (int)bossPos.Y + (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 2), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (/*boss.bulletRects[boss.bulletRects.Count - 1].Right < RightWalls.walls[0].Rectangle.X &&*/
                      boss.bulletRects[boss.bulletRects.Count - 1].Bottom < BottomWalls.walls[0].Rectangle.Bottom)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.X, startRect.Bottom, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
            }
            if (angle >= 105 && angle < 135)//up left
            {
                if (boss.rect.Center.Y > playerRect.rectangle.Y)
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X - (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 4),
            (int)bossPos.Y - (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 2), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].X > LeftWalls.walls[0].Rectangle.Right &&
                      boss.bulletRects[boss.bulletRects.Count - 1].Top > TopWalls.walls[0].Rectangle.Bottom)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.X - size, startRect.Y - size, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
                else
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X - (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 4),
            (int)bossPos.Y + (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 2), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].X > LeftWalls.walls[0].Rectangle.Right &&
                      boss.bulletRects[boss.bulletRects.Count - 1].Bottom < BottomWalls.walls[0].Rectangle.Top)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.X - size, startRect.Bottom, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
            }
            if (angle >= 135 && angle < 155)
            {
                if (boss.rect.Center.Y > playerRect.rectangle.Y)
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X - (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 2),
            (int)bossPos.Y - (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 4), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].X > LeftWalls.walls[0].Rectangle.Right &&
                      boss.bulletRects[boss.bulletRects.Count - 1].Top > TopWalls.walls[0].Rectangle.Bottom)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.X - size, startRect.Y - size / 4, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
                else
                {
                    boss.bulletRects.Add(new Rectangle((int)bossPos.X - (int)(playerRect.rectangle.Width / 2 + boss.rect.Width / 2),
           (int)bossPos.Y + (int)(playerRect.rectangle.Height / 2 + boss.rect.Height / 4), size, size));

                    Rectangle startRect = boss.bulletRects[0];

                    while (boss.bulletRects[boss.bulletRects.Count - 1].X > LeftWalls.walls[0].Rectangle.Right &&
                      boss.bulletRects[boss.bulletRects.Count - 1].Bottom < BottomWalls.walls[0].Rectangle.Top)
                    {
                        boss.bulletRects.Add(new Rectangle(startRect.X - size, startRect.Bottom - size / 2, size, size));
                        startRect = boss.bulletRects[boss.bulletRects.Count - 1];
                    }
                }
            }



        }
    }

    struct PossibleJumpSide
    {
        public List<WallTiles> walls;
        public bool isUsed;
    }

    class BossRect
    {
        public bool hasWall;
        public PossibleJumpSide jumpSide;
        public float jumpForce;
        public Rectangle rect;
        public string side;
        public Rectangle destinationRect;
        public float moveSpeed = .75f;
        public float maxSpeed = 8;
        public float angle;
        public float angleOfLine;
        //public Rectangle bulletRect;
        public List<Rectangle> bulletRects;
        public float lingerTime;

        public Vector2 velocity;
        public Vector2 moveDir;
        public enum BossState { Idle, EnterWall,InWall, Attack, Fire}
        public BossState state = BossState.Idle;
        public BossState prevState = BossState.Idle;

        public BossRect(PossibleJumpSide side, Rectangle rect, string s)
        {
            hasWall = false;
            jumpSide = side;
            this.rect = rect;
           this.side = s;
            bulletRects = new List<Rectangle>();
        }
    }

}
