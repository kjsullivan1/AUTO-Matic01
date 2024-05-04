using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic.TopDown
{
    class TDPlayer
    {
        ContentManager content;
        enum AnimationStates {Idle ,Walking, Shooting, Death}
        AnimationStates animState = AnimationStates.Idle;

        public enum PlayerState {Movement, Shooting, Death, Hit}
        public PlayerState playerState = PlayerState.Movement;

        public int bossRoom = 3;

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

        #region Fields
        public Vector2 position;
        public Rectangle rectangle;
        float moveSpeed = 6f;
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
        string shootDir = "";
        float health = 1.5f;
        public float Health
        {
            get { return health; }
            set { health = value;
                if(health <= 0)
                {
                    health = 0;
                }
            }
        }
        #endregion

        #region Constructor
        public TDPlayer(Game1 game, int pixelSize, int levelInX, int levelInY)
        {
            this.game = game;
            this.pixelSize = pixelSize - 6;
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
        float bulletSpeed = 3.5f;
        float bulletMaxX = 15f;
        float bulletMaxY = 15f;
        bool isShootDelay = false;
        float shootDelay = .8f;//In seconds
        float iShootDelay;
        bool startShoot = false;
        public float bulletDmg = .65f;
        public float bulletTravelDist = 64 * 3;
        #endregion

        public void Load(ContentManager Content, Rectangle bounds)
        {
            texture = Content.Load<Texture2D>("TopDown/Textures/Player");
            upperBound = 0 + (bounds.Height * -(levelInY - 1));
            lowerBound = bounds.Height + (bounds.Height * -(levelInY - 1));
            this.bounds = bounds;
            this.content = Content;
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

        public void Update(GameTime gameTime, TopDownMap map)
        {
            this.map = map;
            kb = Keyboard.GetState();
            rectangle = new Rectangle((int)position.X, (int)position.Y, pixelSize, pixelSize);
            switch(playerState)
            {
                case PlayerState.Movement:
                    Input();
                    if (levelInX >= 1 && levelInY >= 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (changeLevel)
                                break;
                        }
                    }
                    else if (levelInY > 1 && levelInX == 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (changeLevel)
                                break;
                        }
                    }
                    else if (levelInY > 1 && levelInX > 1)
                    {
                        foreach (WallTiles tile in map.WallTiles)
                        {
                            Collision(tile.Rectangle, map.Width + (map.Width * (levelInX - 1)), map.Height - (map.Height * (levelInY - 1)), bounds);
                            if (changeLevel)
                                break;
                        }
                    }
                    break;
            }
            
        }

        private void Input()
        {
            //Else ifs for cardinal
            if (kb.IsKeyDown(Keys.D))
            {
                position.X += moveSpeed;
                shootDir = "right";
            }
            if (kb.IsKeyDown(Keys.A))
            {
                position.X += -moveSpeed;
                shootDir = "left";
            }
            if (kb.IsKeyDown(Keys.W))
            {
                position.Y += -moveSpeed;
                shootDir = "up";
            }
            if (kb.IsKeyDown(Keys.S))
            {
                position.Y += moveSpeed;
                shootDir = "down";
            }

            if(kb.IsKeyDown(Keys.Enter) && prevKb.IsKeyUp(Keys.Enter))
            {
                switch(shootDir)
                {
                    case "up":
                        bullets.Add(new Bullet(new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Top), -bulletSpeed, new Vector2(bulletMaxX, -bulletMaxY), content, false, bulletTravelDist));
                        break;
                    case "down":
                        bullets.Add(new Bullet(new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Top), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist));
                        break;
                    case "left":
                        bullets.Add(new Bullet(new Vector2(rectangle.Left, rectangle.Y + (rectangle.Height/2)), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                        break;
                    case "right":
                        bullets.Add(new Bullet(new Vector2(rectangle.Right, rectangle.Y + (rectangle.Height / 2)), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                        break;
                }
                
            }
            prevKb = kb;
        }

        public void Collision(Rectangle newRect, int xOffset, int yOffset, Rectangle bounds)
        {

            if(rectangle.TouchTopOf(newRect))
            {
                while(rectangle.Bottom > newRect.Top)
                {
                    rectangle.Y -= 1;
                }
                position.Y -= moveSpeed;
            }
            if(rectangle.TouchBottomOf(newRect))
            {
                while(rectangle.Top < newRect.Bottom)
                {
                    rectangle.Y += 1;
                }
                position.Y += moveSpeed;
            }
            if(rectangle.TouchLeftOf(newRect))
            {
                while(rectangle.Right > newRect.Left)
                {
                    rectangle.X -= 1;
                }
                position.X -= moveSpeed;
            }
            if(rectangle.TouchRightOf(newRect))
            {
                while(rectangle.Left < newRect.Right)
                {
                    rectangle.X += 1;
                }
                position.X += moveSpeed;
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
                    CheckBorderCollisionBottom(bounds.Height, newRect, (yOffset - rectangle.Height) + ((yOffset - rectangle.Height) * -(levelInY - 1)));
                }//else do negative 
            }
            else if (PosYLevels.yIndex - 1 >= 0 && (position.Y + (rectangle.Height / 1.2f)) > lowerBound)
                CheckBorderCollisionBottom(bounds.Height, newRect, (yOffset - rectangle.Height) + ((yOffset - rectangle.Height) * -(levelInY - 1)));
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

                        DiagLevels.Points.Add(new Vector2(levelInX, levelInY));
                        DiagLevels.diagIndex = DiagLevels.Points.IndexOf(new Vector2(levelInX - 1, levelInY - 1));

                        if(game.levelCount >= bossRoom)
                        {
                            game.GenerateNewMap(false, false, true, true);
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
            spriteBatch.Draw(texture, rectangle, Color.White);

            foreach(Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

    }
}
