using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.Scripts;
using AUTO_Matic.Scripts.TopDown;

namespace AUTO_Matic.TopDown
{
    class TDPlayer
    {
        ContentManager content;
        enum AnimationStates {Idle ,Walking, Shooting, Death}
        AnimationStates animState = AnimationStates.Walking;

        public enum PlayerState {Movement, Shooting, Death, Hit, Dash}
        public PlayerState playerState = PlayerState.Movement;

        Vector2 controllerMoveDir;
        public Vector2 velocity;
        GamePadButtons currButtons;
        GamePadButtons prevButtons;
        public int bossRoom = 1;
        bool lockDir = false;
        Vector2 startPos;

        float meleeDmg = 2.25f;
        bool melee = false;
        float meleeDelay = .75f;
        float iMeleeDelay;
        
        Rectangle MeleeHitbox
        {
            get
            {
                int meleeHeight = rectangle.Height + (64 - rectangle.Height);//makes = to 64 or one Tile length
                int widthMod = 2;
                Rectangle rect = new Rectangle();
                switch(shootDir)
                {
                    case "up":
                        rect = new Rectangle(rectangle.X - Math.Abs((rectangle.Width - meleeHeight)/3), rectangle.Y - (meleeHeight / widthMod), meleeHeight, meleeHeight / widthMod);
                        break;
                    case "down":
                        rect = new Rectangle(rectangle.X - Math.Abs((rectangle.Width - meleeHeight)/3), rectangle.Y + Math.Abs((rectangle.Height)), meleeHeight, meleeHeight/widthMod);
                        break;
                    case "left":
                        rect = new Rectangle(rectangle.X - ((meleeHeight / widthMod) - 10), rectangle.Y - Math.Abs((rectangle.Height - meleeHeight)/3), meleeHeight / widthMod, meleeHeight);
                        break;
                    case "right":
                        rect = new Rectangle(rectangle.Right, rectangle.Y - Math.Abs((rectangle.Height - meleeHeight)/3), meleeHeight/widthMod, meleeHeight);
                        break;
                        
                }
                return rect;
            }
        }

        #region Animations
        AnimationManager animManager;
        Texture2D texture;
        Point FrameSize;//Size of frame
        Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;
        
        public void ChangeAnimation()
        {
            switch (animState)
            {
                case AnimationStates.Idle:
                    texture = content.Load<Texture2D>("TopDown/Animations/PlayerIdle");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(6, 1);
                    fpms = 120;
                    break;
                case AnimationStates.Walking:

                    if(shootDir == "up")
                    {
                        texture = content.Load<Texture2D>("TopDown/Animations/PilotUpWalk");
                    }
                    else if(shootDir == "down")
                    {
                        texture = content.Load<Texture2D>("TopDown/Animations/PilotDownWalk");
                    }
                    else if(shootDir == "right")
                    {
                        texture = content.Load<Texture2D>("TopDown/Animations/PilotRightWalk");
                    }
                    else if(shootDir == "left")
                    {
                        texture = content.Load<Texture2D>("TopDown/Animations/PilotLeftWalk");
                    }
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(3, 1);
                    fpms = 120;
                    break;
                case AnimationStates.Shooting:
                    texture = content.Load<Texture2D>("TopDown/Animations/PlayerShoot");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(4, 1);
                    fpms = 95;
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

            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, new Vector2(rectangle.X, rectangle.Y));

            animManager.isRight = isRight;
            animManager.isLeft = isLeft;
            animManager.isUp = isUp;
            animManager.isDown = isDown;
        }
        #endregion

        #region Fields
        public Vector2 position;
        public Rectangle rectangle;
        float moveSpeed = 5.25f;
        public bool changeLevel = false;
        public bool isColliding = false;
        KeyboardState kb;
        KeyboardState prevKb;
        Game1 game;
        public int upperBound;
        public int lowerBound;
        int pixelSize = 32;
        TopDownMap map;
        Rectangle bounds;
        public string shootDir = "right";
        float health = 5;
        public bool damaged = false;
        public float Health
        {
            get { return health; }
            set {
                damaged = true;
                health = value;
                if(health <= 0)
                {
                    
                    
                    health = 0;
                }

                if(health >= 5)
                {
                    health = 5;
                }
            }
        }
        #endregion

        #region Constructor
        public TDPlayer(Game1 game, int pixelSize, int levelInX, int levelInY)
        {
            this.game = game;
            this.pixelSize = pixelSize - 12;
            this.levelInX = levelInX;
            this.levelInY = levelInY;
            //this.bounds = bounds;

            DiagLevels.dLevels = new List<int[,]>();
            DiagLevels.Points = new List<Vector2>();
            DiagLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
            DiagLevels.diagIndex = 0;

            PosXLevels.xLevels = new List<int[,]>();
            PosXLevels.Points = new List<Vector2>();
            PosXLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));

            PosYLevels.yLevels = new List<int[,]>();
            PosYLevels.Points = new List<Vector2>();
            PosYLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
            iMeleeDelay = meleeDelay;
        }

        #endregion

        #region Map Tracker
        public struct diagLevels
        {
            public List<int[,]> dLevels;
            public int diagIndex;
            public List<Vector2> Points;
        }
        public struct posXLevels
        {
            public List<int[,]> xLevels;
            public int xIndex;
            public List<Vector2> Points;
        }
        public struct posYLevels
        {
            public List<int[,]> yLevels;
            public int yIndex;
            public List<Vector2> Points;
        }

        public diagLevels DiagLevels;
        public posXLevels PosXLevels;
        public posYLevels PosYLevels;

        List<int[,]> WallCords = new List<int[,]>();
        int levelCount = 0;
        public int levelInX = 1;
        public int levelInY = 1;
        public List<Vector2> BoundIndexs = new List<Vector2>();
        #endregion

        #region Shooting
        Texture2D gunTexture;
        public List<Bullet> bullets = new List<Bullet>();
        MouseState prevMs;
        float bulletSpeed = 2f;
        float bulletMaxX = 10f;
        float bulletMaxY = 10f;
        bool isShootDelay = false;
        float shootDelay = .3f;//In seconds
        float iShootDelay;
        bool startShoot = false;
        public float bulletDmg = 1.2f;
        public float bulletTravelDist = 64 * 4;
        #endregion


        float dashDistance = 64 * 2.75f;
        float dashSpeed = 12f;

        public void Load(ContentManager Content, Rectangle bounds)
        {
            texture = Content.Load<Texture2D>("TopDown/Textures/Player");
            upperBound = 0 + (bounds.Height * -(levelInY - 1));
            lowerBound = bounds.Height + (bounds.Height * -(levelInY - 1));
            this.bounds = bounds;
            this.content = Content;
            ChangeAnimation();
            iShootDelay = shootDelay;
            shootDelay = 0;
        }

        public void GenerateMap(bool xLevel, bool yLevel, bool dLevel)
        {


            levelCount++;

            //for (int j = 0; j < maxObstacles; j++)
            //{
            //    int tileNum;
            //    int temp = 0;
            //    rndRow = rnd.Next(1, map.rows[levelCount - 1] - 1);// gets a random row
            //    rndCol = rnd.Next(1, map.cols[levelCount - 1] - 1);//random col
            //    tileNum = rnd.Next(0, 3); // 1/3 chance of being a wall (set of 5) or a skull or nothing


            //    if (tileNum == 2)//Wall block. Attempts to reach 7 block shape, but doesnt if there is something there
            //    {
            //        int directionX;
            //        int directionY;
            //        while (true)
            //        {
            //            directionX = rnd.Next(-1, 2); //sets direction of the wall
            //            directionY = rnd.Next(-1, 2); //sets direction of wall
            //            if (directionX != 0 || directionY != 0)
            //                break;
            //        }
            //        while (temp < 5)
            //        {

            //            if (dimensions[rndRow, rndCol] == 0) //if there is nothing assigned 
            //            {
            //                dimensions[rndRow, rndCol] = 2;
            //                wallCoords.Add(new int[rndRow, rndCol]);
            //            }

            //            if (directionX == -1 && rndRow == 0)
            //            {
            //                directionX = rnd.Next(0, 2);
            //            }
            //            else if (directionX == 1 && rndRow == map.rows[levelCount - 1] - 1)
            //            {

            //                directionX = rnd.Next(-1, 1);



            //            }
            //            else
            //            {
            //                rndRow += directionX;
            //            }

            //            if (directionY == -1 && rndCol == 0)
            //            {

            //                directionY = rnd.Next(0, 2);
            //            }
            //            else if (directionY == 1 && rndCol == map.cols[levelCount - 1] - 1)
            //            {

            //                directionY = rnd.Next(-1, 1);



            //            }
            //            else
            //            {
            //                rndCol += directionY;
            //            }


            //            temp++;
            //        }
            //        temp = 0;
            //    }
            //    else if (tileNum == 1)
            //    {
            //        if (dimensions[rndRow, rndCol] == 0)
            //        {
            //            dimensions[rndRow, rndCol] = 3;
            //        }
            //    }


            //}

            //for (int j = 0; j < map.rows[levelCount]; j++)
            //{
            //    for (int k = 0; k < map.cols[levelCount - 1]; k++)
            //    {
            //        if (dimensions[j, k] == 0)
            //        {
            //            dimensions[j, k] = 1;

            //        }
            //    }
            //}

            //if (xLevel)
            //    PosXLevel.xLevels.Add(dimensions);
            //if (yLevel)
            //{
            //    PosYLevel.yLevels.Add(dimensions);
            //}
            //if (dLevel)
            //{
            //    DiagLevels.dLevels.Add(dimensions);
            //}
            //dimensions = new int[row, col];


            //map.Refresh(PosXLevel.xLevels, PosYLevel.yLevels, DiagLevels.dLevels, pixelBits, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, PosXLevel.Points, PosYLevel.Points, DiagLevels.Points);
            ////map.Refresh(currentLevels, pixelBits, Window.ClientBounds.Width + (Window.ClientBounds.Width * (levelInX - 1)), graphics.PreferredBackBufferHeight + (graphics.PreferredBackBufferHeight * (levelInY - 1)), levelInX, levelInY);


        }

        public void Update(GameTime gameTime, TopDownMap map, ShotGunBoss boss, List<TDEnemy> enemies)
        {

            controllerMoveDir = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
            currButtons = GamePad.GetState(PlayerIndex.One).Buttons;
            this.map = map;
            kb = Keyboard.GetState();
            rectangle = new Rectangle((int)position.X, (int)position.Y, pixelSize, pixelSize);

            meleeDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch(playerState)
            {
                case PlayerState.Movement:
                    Input(enemies, gameTime);
                    if (levelInX >= 1 && levelInY >= 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (boss != null)
                            {
                                if (rectangle.TouchLeftOf(boss.worldRect))
                                {
                                    while (rectangle.Right > boss.worldRect.Left)
                                    {
                                        rectangle.X -= 1;
                                        position.X -= 1;
                                    }
                                }
                                if (rectangle.TouchRightOf(boss.worldRect))
                                {
                                    while (rectangle.Left < boss.worldRect.Right)
                                    {
                                        rectangle.X += 1;
                                        position.X += 1;
                                    }
                                }
                                if(rectangle.TouchBottomOf(boss.worldRect))
                                {
                                    while(rectangle.Top < boss.worldRect.Bottom)
                                    {
                                        rectangle.Y += 1;
                                        position.Y += 1;
                                    }
                                }
                                if(rectangle.TouchTopOf(boss.worldRect))
                                {
                                    while(rectangle.Bottom > boss.worldRect.Top)
                                    {
                                        rectangle.Y -= 1;
                                        position.Y -= 1;
                                    }
                                }
                            }
                               
                            if (changeLevel)
                                break;
                        }
                    }
                    else if (levelInY > 1 && levelInX == 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (boss != null)
                            {
                                if (rectangle.TouchLeftOf(boss.worldRect))
                                {
                                    while (rectangle.Right > boss.worldRect.Left)
                                    {
                                        rectangle.X -= 1;
                                        position.X -= 1;
                                    }
                                }
                                if (rectangle.TouchRightOf(boss.worldRect))
                                {
                                    while (rectangle.Left < boss.worldRect.Right)
                                    {
                                        rectangle.X += 1;
                                        position.X += 1;
                                    }
                                }
                                if (rectangle.TouchBottomOf(boss.worldRect))
                                {
                                    while (rectangle.Top < boss.worldRect.Bottom)
                                    {
                                        rectangle.Y += 1;
                                        position.Y += 1;
                                    }
                                }
                                if (rectangle.TouchTopOf(boss.worldRect))
                                {
                                    while (rectangle.Bottom > boss.worldRect.Top)
                                    {
                                        rectangle.Y -= 1;
                                        position.Y -= 1;
                                    }
                                }
                            }
                            if (changeLevel)
                                break;
                        }
                    }
                    else if (levelInY > 1 && levelInX > 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (boss != null)
                            {
                                if (rectangle.TouchLeftOf(boss.worldRect))
                                {
                                    while (rectangle.Right > boss.worldRect.Left)
                                    {
                                        rectangle.X -= 1;
                                        position.X -= 1;
                                    }
                                }
                                if (rectangle.TouchRightOf(boss.worldRect))
                                {
                                    while (rectangle.Left < boss.worldRect.Right)
                                    {
                                        rectangle.X += 1;
                                        position.X += 1;
                                    }
                                }
                                if (rectangle.TouchBottomOf(boss.worldRect))
                                {
                                    while (rectangle.Top < boss.worldRect.Bottom)
                                    {
                                        rectangle.Y += 1;
                                        position.Y += 1;
                                    }
                                }
                                if (rectangle.TouchTopOf(boss.worldRect))
                                {
                                    while (rectangle.Bottom > boss.worldRect.Top)
                                    {
                                        rectangle.Y -= 1;
                                        position.Y -= 1;
                                    }
                                }
                            }
                            if (changeLevel)
                                break;
                        }
                    }
                    break;
                case PlayerState.Dash:
                    switch(shootDir)
                    {
                        case "up":
                            if(MathHelper.Distance(startPos.Y, position.Y) < dashDistance)
                            {
                                position.Y -= dashSpeed;
                            }
                            else
                            {
                                playerState = PlayerState.Movement;
                            }
                            break;
                        case "down":
                            if (MathHelper.Distance(startPos.Y, position.Y) < dashDistance)
                            {
                                position.Y += dashSpeed;
                            }
                            else
                            {
                                playerState = PlayerState.Movement;
                            }
                            break;
                        case "right":
                            if (MathHelper.Distance(startPos.X, position.X) < dashDistance)
                            {
                                position.X += dashSpeed;
                            }
                            else
                            {
                                playerState = PlayerState.Movement;
                            }
                            break;
                        case "left":
                            if (MathHelper.Distance(startPos.X, position.X) < dashDistance)
                            {
                                position.X -= dashSpeed;
                            }
                            else
                            {
                                playerState = PlayerState.Movement;
                            }
                            break;


                    }
                    isColliding = false;
                    #region Collisions
                    if (levelInX >= 1 && levelInY >= 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (boss != null)
                            {
                                if (rectangle.TouchLeftOf(boss.worldRect))
                                {
                                    while (rectangle.Right > boss.worldRect.Left)
                                    {
                                        rectangle.X -= 1;
                                        position.X -= 1;
                                    }
                                }
                                if (rectangle.TouchRightOf(boss.worldRect))
                                {
                                    while (rectangle.Left < boss.worldRect.Right)
                                    {
                                        rectangle.X += 1;
                                        position.X += 1;
                                    }
                                }
                                if (rectangle.TouchBottomOf(boss.worldRect))
                                {
                                    while (rectangle.Top < boss.worldRect.Bottom)
                                    {
                                        rectangle.Y += 1;
                                        position.Y += 1;
                                    }
                                }
                                if (rectangle.TouchTopOf(boss.worldRect))
                                {
                                    while (rectangle.Bottom > boss.worldRect.Top)
                                    {
                                        rectangle.Y -= 1;
                                        position.Y -= 1;
                                    }
                                }
                            }

                            if (changeLevel)
                                break;
                        }
                    }
                    else if (levelInY > 1 && levelInX == 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (boss != null)
                            {
                                if (rectangle.TouchLeftOf(boss.worldRect))
                                {
                                    while (rectangle.Right > boss.worldRect.Left)
                                    {
                                        rectangle.X -= 1;
                                        position.X -= 1;
                                    }
                                }
                                if (rectangle.TouchRightOf(boss.worldRect))
                                {
                                    while (rectangle.Left < boss.worldRect.Right)
                                    {
                                        rectangle.X += 1;
                                        position.X += 1;
                                    }
                                }
                                if (rectangle.TouchBottomOf(boss.worldRect))
                                {
                                    while (rectangle.Top < boss.worldRect.Bottom)
                                    {
                                        rectangle.Y += 1;
                                        position.Y += 1;
                                    }
                                }
                                if (rectangle.TouchTopOf(boss.worldRect))
                                {
                                    while (rectangle.Bottom > boss.worldRect.Top)
                                    {
                                        rectangle.Y -= 1;
                                        position.Y -= 1;
                                    }
                                }
                            }
                            if (changeLevel)
                                break;
                        }
                    }
                    else if (levelInY > 1 && levelInX > 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (boss != null)
                            {
                                if (rectangle.TouchLeftOf(boss.worldRect))
                                {
                                    while (rectangle.Right > boss.worldRect.Left)
                                    {
                                        rectangle.X -= 1;
                                        position.X -= 1;
                                    }
                                }
                                if (rectangle.TouchRightOf(boss.worldRect))
                                {
                                    while (rectangle.Left < boss.worldRect.Right)
                                    {
                                        rectangle.X += 1;
                                        position.X += 1;
                                    }
                                }
                                if (rectangle.TouchBottomOf(boss.worldRect))
                                {
                                    while (rectangle.Top < boss.worldRect.Bottom)
                                    {
                                        rectangle.Y += 1;
                                        position.Y += 1;
                                    }
                                }
                                if (rectangle.TouchTopOf(boss.worldRect))
                                {
                                    while (rectangle.Bottom > boss.worldRect.Top)
                                    {
                                        rectangle.Y -= 1;
                                        position.Y -= 1;
                                    }
                                }
                            }
                            if (changeLevel)
                                break;
                        }
                    }
                    #endregion
                    if(isColliding)
                    {
                        playerState = PlayerState.Movement;
                    }
                    break;
            }

            if (bullets.Count != 0)
            {
                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    bullets[i].Update();
                    if (bullets[i].delete)
                    {
                        bullets.RemoveAt(i);
                        break;
                    }
                    //foreach (SSEnemy enemy in enemies)
                    //{
                    //    if (bullets[i].rect.TouchBottomOf(enemy.enemyRect) || bullets[i].rect.TouchTopOf(enemy.enemyRect)
                    //    || bullets[i].rect.TouchLeftOf(enemy.enemyRect) || bullets[i].rect.TouchRightOf(enemy.enemyRect))
                    //    {
                    //        enemy.Health -= bulletDmg;
                    //        bullets.RemoveAt(i);
                    //        break;
                    //    }
                    //}

                }
            }
            animManager.Update(gameTime, new Vector2(rectangle.X, rectangle.Y - (64 - rectangle.Height)));
        }

        private void Input(List<TDEnemy> enemies, GameTime gameTime)
        {
            if(kb.IsKeyDown(Keys.LeftShift) && prevKb.IsKeyDown(Keys.LeftShift))
            {
                lockDir = true;//!lockDir
            }
            else
            {
                lockDir = false;
            }
            //Else ifs for cardinal
            if (kb.IsKeyDown(Keys.D) || controllerMoveDir.X > 0 /*&& controllerMoveDir.Y > -.9 && controllerMoveDir.Y < .9*/)
            {
                velocity.X += moveSpeed;
                if(!lockDir && shootDir != "right")
                {
                    shootDir = "right";

                    ChangeAnimation();
                }
                    
            }
            if (kb.IsKeyDown(Keys.A) || controllerMoveDir.X  < 0/* && controllerMoveDir.Y > -.9 && controllerMoveDir.Y < .9*/)
            {
                velocity.X += -moveSpeed;
                if (!lockDir && shootDir != "left")
                {
                    shootDir = "left";
                    ChangeAnimation();
                }
                   
            }
            if (kb.IsKeyDown(Keys.W) ||/* controllerMoveDir.X < .6 &&*/ controllerMoveDir.Y > 0 /*&& controllerMoveDir.X > -.6*/)
            {
                velocity.Y += -moveSpeed;
                if (!lockDir && shootDir != "up")
                {
                    shootDir = "up";
                    ChangeAnimation();
                }
                  
            }
            if (kb.IsKeyDown(Keys.S) ||/* controllerMoveDir.X < .6 &&*/ controllerMoveDir.Y < 0 /*&& controllerMoveDir.X > -.6*/ )
            {
                velocity.Y += moveSpeed;
                if (!lockDir && shootDir != "down")
                {
                    shootDir = "down";
                    ChangeAnimation();
                }
                    
            }

            if(kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D))
            {
                velocity.X = 0;
            }
            if(kb.IsKeyUp(Keys.S) && kb.IsKeyUp(Keys.W))
            {
                velocity.Y = 0;
            }
            shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.Enter) && prevKb.IsKeyUp(Keys.Enter) || currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released)
            {
               

                if(shootDelay <= 0)
                {
                    switch (shootDir)
                    {
                        case "up":
                            bullets.Add(new Bullet(new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Top), -bulletSpeed, new Vector2(bulletMaxX, -bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed));
                            break;
                        case "down":
                            bullets.Add(new Bullet(new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Top), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed));
                            break;
                        case "left":
                            bullets.Add(new Bullet(new Vector2(rectangle.Left, rectangle.Y + (rectangle.Height / 2)), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                            break;
                        case "right":
                            bullets.Add(new Bullet(new Vector2(rectangle.Right, rectangle.Y + (rectangle.Height / 2)), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                            break;
                    }

                    shootDelay = iShootDelay;
                }
               
                
            }

            if(kb.IsKeyDown(Keys.LeftShift) && prevKb.IsKeyUp(Keys.LeftShift) || currButtons.B == ButtonState.Pressed && prevButtons.B == ButtonState.Released)
            {
                playerState = PlayerState.Dash;
                startPos = position;
            }

            if(kb.IsKeyDown(Keys.F) && prevKb.IsKeyUp(Keys.F) && meleeDelay <= 0 || currButtons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released && meleeDelay <= 0)
            {
                meleeDelay = iMeleeDelay;
                foreach(TDEnemy enemy in enemies)
                {
                    if(MeleeHitbox.Intersects(enemy.Rectangle))
                    {
                        enemy.Health -= meleeDmg;
                    }
                }
            }
            prevKb = kb;
            prevButtons = currButtons;

            if(velocity.X >= moveSpeed)
            {
                velocity.X = moveSpeed;
            }
            else if(velocity.X <= -moveSpeed)
            {
                velocity.X = -moveSpeed;
            }
            if(velocity.Y >= moveSpeed)
            {
                velocity.Y = moveSpeed;
            }
            else if(velocity.Y <= -moveSpeed)
            {
                velocity.Y = -moveSpeed;
            }

            position += velocity;
            if(controllerMoveDir != Vector2.Zero)
            {

            }
        }

        public void Collision(Rectangle newRect, int xOffset, int yOffset, Rectangle bounds)
        {

            if(rectangle.TouchTopOf(newRect))
            {
                while(rectangle.Bottom > newRect.Top)
                {
                    position.Y -= 1;

                    rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                }
                isColliding = true;
                position.Y -= moveSpeed;
            }
            if(rectangle.TouchBottomOf(newRect))
            {
                while(rectangle.Top < newRect.Bottom)
                {
                    position.Y += 1;
                    rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                }
                position.Y += moveSpeed;
                isColliding = true;
            }
            if(rectangle.TouchLeftOf(newRect))
            {
                while(rectangle.Right > newRect.Left)
                {
                    position.X -= 1;
                    rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                }
                position.X -= moveSpeed;
                isColliding = true;
            }
            if(rectangle.TouchRightOf(newRect))
            {
                while(rectangle.Left < newRect.Right)
                {
                    position.X += 1;
                    rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                }
                position.X += moveSpeed;
                isColliding = true;
            }
          
            //Border collisions
            if ((position.X + (rectangle.Width / 8f) < (xOffset - (xOffset / (levelInX)))))
                CheckBorderCollisionLeft(xOffset, newRect, xOffset - (xOffset / (levelInX)) + pixelSize);
            else if ((position.X + (rectangle.Width / 12f)) > xOffset - rectangle.Width)
                CheckBorderCollisionRight(xOffset, newRect, xOffset - rectangle.Width);
            else if ((position.Y - (rectangle.Height / 12f) < (upperBound)))
            {
                CheckBorderCollisionTop(bounds.Height, newRect, upperBound);
            }

            if (DiagLevels.Points.Count > 0 && levelInX >= 2 && levelInY > 1)
            {
                if ((position.Y + (rectangle.Height / 1.2f)) > lowerBound)
                {
                    CheckBorderCollisionBottom(bounds.Height, newRect, lowerBound/*(yOffset - rectangle.Height) + ((yOffset - rectangle.Height) * -(levelInY - 1))*/);
                }//else do negative 
            }
            else if (PosYLevels.yIndex - 1 >= 0 && (position.Y + (rectangle.Height / 1.2f)) > lowerBound)
                CheckBorderCollisionBottom(bounds.Height, newRect, (yOffset - rectangle.Height) + ((yOffset - rectangle.Height) * -(levelInY - 1)));
            //if (PosYLevels.yIndex - 1 >= 0 && (position.Y + (rectangle.Height / 1.2f)) > lowerBound)
            //    CheckBorderCollisionBottom(bounds.Height, newRect, (yOffset - rectangle.Height) + ((yOffset - rectangle.Height) * -(levelInY - 1)));
        }

        void CheckBorderCollisionRight(int xOffset, Rectangle newRect, int border) // No Y because in max right
        {


            position.X += pixelSize;
            if (levelInY == 1)
            {
                if (CanMove(newRect))
                {
                    levelInX++;
                    if (PosXLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        PosXLevels.xIndex = PosXLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                    }
                    else
                    {
                        PosXLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        PosXLevels.xIndex = PosXLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(true, false, false, true);
                            position.X += 64;
                        }
                        else
                        {
                            game.GenerateNewMap(true, false, false, false);
                        }
                        
                    }

                    changeLevel = true;
                }
            }
            else if (levelInY > 1)
            {
                if (CanMove(newRect))
                {
                    levelInX++;


                    if (DiagLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                    }
                    else
                    {

                        DiagLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(false, false, true, true);
                            position.X += 64;
                        }
                        else
                        {
                            game.GenerateNewMap(false, false, true, false);
                        }
                       
                    }

                    changeLevel = true;
                }
            }
            else
            {
                position.X = border;

            }
            //switch(levelInIndex)
            //{
            //    case 0:
            //        if(CanMove(newRect))
            //        {
            //            levelInX++;
            //            changeLevel = true;

            //        }
            //        else
            //        {
            //            position.X = border;
            //        }
            //        break;
            //    case 2:
            //        //position.X = xOffset - rectangle.Width;  //right collision
            //        if(CanMove(newRect))
            //        {
            //            levelInX = 3;
            //            changeLevel = true;

            //        }
            //        else
            //        {
            //            position.X = border;
            //        }
            //        break;
            //    case 3:
            //        position.X = xOffset - rectangle.Width;
            //        break;
            //}
        }
        void CheckBorderCollisionLeft(int xOffset, Rectangle rect, int border)
        {
            position.X -= pixelSize;


            if (levelInY == 1)
            {
                if (position.X >= 0)
                {

                    levelInX--;
                    if (PosXLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        PosXLevels.xIndex = PosXLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                    }
                    else
                    {
                        PosXLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        PosXLevels.xIndex = PosXLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(true, false, false, true);
                            position.X -= 64;
                        }
                        else
                        {
                            game.GenerateNewMap(true, false, false, false);
                        }
                       
                    }

                    changeLevel = true;
                }
                else
                {
                    position.X = border;

                }
            }
            else if (levelInY > 1 && levelInX == 2)
            {
                if (CanMove(rect))
                {
                    levelInX--;


                    if (PosXLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        PosYLevels.yIndex = PosYLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                    }
                    else
                    {

                        PosYLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        PosYLevels.yIndex = PosYLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(false, true, false, true);
                            position.X -= 64;
                        }
                        else
                        {
                            game.GenerateNewMap(false, true, false, false);
                        }
                       
                    }

                    changeLevel = true;
                }
            }
            else if (levelInY > 1 && levelInX > 2)
            {
                if (CanMove(rect))
                {
                    levelInX--;


                    if (DiagLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                    }
                    else if (levelInX > 1)
                    {

                        DiagLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(false, false, true, true);
                            position.X -= 64;
                        }
                        else
                        {
                            game.GenerateNewMap(false, false, true, false);
                        }
                       
                    }

                    changeLevel = true;
                }
            }
            else
            {
                position.X = border;
            }




        }

        void CheckBorderCollisionTop(int bounds, Rectangle rect, int border) // missing check for x because to max top
        {
            position.Y -= pixelSize;

            if (levelInX == 1)
            {
                if (CanMove(rect))
                {
                    levelInY++;
                    if (PosYLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        PosYLevels.yIndex = PosYLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                    }
                    else
                    {
                        PosYLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        PosYLevels.yIndex = PosYLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if (game.levelCount >= 0)
                        {
                            game.GenerateNewMap(false, true, false, true);
                            position.Y -= 64;
                        }
                        else
                        {
                            game.GenerateNewMap(false, true, false, false);
                        }
                       
                    }

                    changeLevel = true;
                    upperBound += -bounds;
                    lowerBound += -bounds;
                }
                else
                {
                    position.Y = border;
                }
            }
            else if (levelInX > 1)
            {
                if (CanMove(rect))
                {
                    levelInY++;


                    if (DiagLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                        upperBound += -bounds;
                        lowerBound += -bounds;
                    }
                    else
                    {

                        DiagLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(false, false, true, true);
                            position.Y -= 64;
                        }
                        else
                        {
                            game.GenerateNewMap(false, false, true, false);
                        }
                      
                        upperBound += -bounds;
                        lowerBound += -bounds;
                    }

                    changeLevel = true;
                }
            }

        }

        void CheckBorderCollisionBottom(int bounds, Rectangle rect, int border)
        {

            position.Y += pixelSize;
            if (levelInX == 1 && position.Y < bounds)
            {
                if (CanMove(rect))
                {




                    levelInY--;
                    if (PosYLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        PosYLevels.yIndex = PosYLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                    }
                    else
                    {
                        PosYLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        PosYLevels.yIndex = PosYLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(false, true, false, true);
                            position.Y += 64;
                        }
                        else
                        {
                            game.GenerateNewMap(false, true, false, false);
                        }
                        
                    }

                    changeLevel = true;
                    upperBound += bounds;
                    lowerBound += bounds;




                }
                else
                {
                    position.Y = border;
                }
            }
            else if (levelInX > 1 && levelInY == 2 && changeLevel != true)
            {
                if (CanMove(rect))
                {
                    levelInY--;


                    if (PosXLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        PosXLevels.xIndex = PosXLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                        upperBound += bounds;
                        lowerBound += bounds;
                    }
                    else
                    {

                        PosXLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        PosXLevels.xIndex = PosXLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(true, false, false, true);
                            position.Y += 64;
                        }
                        else
                        {
                            game.GenerateNewMap(true, false, false, false);
                        }
                       
                        upperBound += bounds;
                        lowerBound += bounds;
                    }

                    changeLevel = true;
                }
            }
            else if (levelInX > 1 && levelInY > 2 && changeLevel != true)
            {
                if (CanMove(rect))
                {
                    levelInY--;


                    if (DiagLevels.Points.Contains(new Vector2(levelInX - 1, levelInY - 1)))
                    {
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));
                        upperBound += bounds;
                        lowerBound += bounds;
                    }
                    else
                    {

                        DiagLevels.Points.Add(new Vector2(levelInX - 1, levelInY - 1));
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(false, false, true, true);
                            position.Y += 64;
                        }
                        else
                        {
                            game.GenerateNewMap(false, false, true, false);
                        }
                       
                        upperBound += bounds;
                        lowerBound += bounds;
                    }

                    changeLevel = true;
                }
            }
            else
            {
                position.Y = border - (pixelSize / 2);
            }


        }

        bool CanMove(Rectangle newRect)
        {
            if (rectangle.TouchTopOf(newRect))
            {
                return false;
            }
            else if (rectangle.TouchLeftOf(newRect))
            {
                return false;
            }
            else if (rectangle.TouchRightOf(newRect))
            {
                return false;
            }
            else if (rectangle.TouchBottomOf(newRect))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //if (damaged)
            //{
            //    if (redCount <= whiteCount || redCount == 0 && whiteCount == 0)
            //    {
            //        animManager.Draw(spriteBatch, Color.Red);
            //        redCount++;
            //    }
            //    if (whiteCount < redCount)
            //    {
            //        animManager.Draw(spriteBatch, Color.White * .5f);
            //        whiteCount++;
            //    }
            //    if (whiteCount == whiteFrames)
            //    {
            //        damaged = false;
            //        whiteCount = 0;
            //        redCount = 0;
            //    }
            //}
            //else
            //{
            //    animManager.Draw(spriteBatch, Color.White);
            //}

            spriteBatch.Draw(content.Load<Texture2D>("TopDown/Textures/Player"), MeleeHitbox, Color.White);
            //spriteBatch.Draw(texture, rectangle, Color.White);
            animManager.Draw(spriteBatch, Color.White);

            foreach(Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

    }
}
