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
using AUTO_Matic.Scripts.Effects;

namespace AUTO_Matic.TopDown
{
    class TDPlayer
    {
        ContentManager content;
        enum AnimationStates {Idle ,Walking, Shooting, Death}
        AnimationStates animState = AnimationStates.Walking;

        public enum PlayerState {Movement, Shooting, Death, Hit, Dash}
        public PlayerState playerState = PlayerState.Movement;
        enum WeaponType { Pistol, Shotgun, Laser, Burst, Bomb }
        WeaponType currWeapon = WeaponType.Pistol;

        GamePadState joystick = GamePad.GetState(0);
        WeaponWheel weaponWheel;
        int selectedWeapon = 0;
        float weaponWheelActiveDelay = .25f;
        float wwActiveDelayMax = .25f;
        Vector2 controllerMoveDir;
        public Vector2 velocity;
        GamePadButtons currButtons;
        GamePadButtons prevButtons;
        public int bossRoom = 8;
        bool lockDir = false;
        Vector2 startPos;



        float meleeDmg = 2.25f;
        bool melee = false;
        float meleeDelay = .75f;
        float iMeleeDelay;

        string[] moveDirs = new string[2];

        bool speedBoosted = false;
        float boostedSpeed = 0;
        float speedBoostTime = 1.75f;
        float iSpeedBoostTime;
        float speedBoostInputDelay = .6f;
        float iSpeedBoostInputDelay;
        bool inputDelay = false;

        float fireDmg = .5f;
        float fireDmgRate = .15f;
        float iFireDmgRate = .15f;
        bool inDOT = false; //Is inside the DamageOverTime tile

        UIManager KeyBinds; 

        ParticleManager particles;
        SoundManager sounds;

        
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
            bool isRight = false, isLeft = false, isUp = false, isDown = false;
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
                        lockedDirection = "up";
                        isUp = true;
                       
                    }
                    else if(shootDir == "down")
                    {
                        texture = content.Load<Texture2D>("TopDown/Animations/PilotDownWalk");
                        lockedDirection = "down";
                        isDown = true;
                       
                    }
                    else if(shootDir == "right")
                    {
                        texture = content.Load<Texture2D>("TopDown/Animations/PilotRightWalk");
                        lockedDirection = "right";
                        isRight = true;
                       

                    }
                    else if(shootDir == "left")
                    {
                        texture = content.Load<Texture2D>("TopDown/Animations/PilotLeftWalk");
                        lockedDirection = "left";
                       isLeft = true;
                        
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
        float health = 10;
        public bool damaged = false;

        public int redFrames = 4;
        public int redCount = 0;
        int whiteFrames = 45;
        int whiteCount = 0;

        //List<Bullet> bombs = new List<Bullet>();

        public float Health
        {
            get { return health; }
            set {
                if(value < health && playerState != PlayerState.Dash)
                {
                    damaged = true;
                    health = value;
                }
                else if(value > health)
                    health = value;
                if (health <= 0)
                {
                    
                    
                    health = 0;
                }

                if(health >= 10)
                {
                    health = 10;
                }
            }
        }
        #endregion

        #region Constructor
        public TDPlayer(Game1 game, int pixelSize, int levelInX, int levelInY, UIManager uiManager, ContentManager content, int levelCount)
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

            KeyBinds = uiManager;

            sounds = new SoundManager(content, uiManager.MasterVolume, uiManager.EffectVolume, uiManager.MusicVolume);

            switch (levelCount)
            {
                case 0:
                    bossRoom = 4;
                    break;
                case 1:
                    bossRoom = 6;
                    break;
                case 2:
                    bossRoom = 8;
                    break;
                case 3:
                    bossRoom = 10;
                    break;
            }
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
        public List<Bullet> bombs = new List<Bullet>();
        public List<Explosion> explosions = new List<Explosion>();
        MouseState prevMs;
        float bulletSpeed = 2f;
        float bulletMaxX = 10f;
        float bulletMaxY = 10f;
        bool isShootDelay = false;
        float shootDelay = .35f;//In seconds
        float iShootDelay;

        float pistolDelay = .5f;
        float maxPistolDelay = .5f;
        float pistolDmg = 1.2f;

        float shotGunDelay = 1.15f;
        float maxShotgunDelay = 1.15f;
        float shotGunDmg = .85f;

        float burstDelay = .65f;
        float maxBurstDelay = .65f;
        float burstDmg = 1.2f;

        float laserDelay = 1.35f;
        float maxLaserDelay = 1.35f;
        float laserDmg = 1.35f;

        float bombDelay = 1.5f;
        float maxBombDelay = 1.5f;
        float bombDmg = .5f;

        bool startShoot = false;
        public float bulletDmg = 1.2f;
        public float bulletTravelDist = 64 * 4.75f;
        #endregion


        float dashDistance = 64 * 2.75f;
        float dashSpeed = 12f;
        Vector2 dashVelocity;

        string lockedDirection;

        float RandomPitch
        {
            get
            {
                Random rand = new Random();
                float num = 0;
                switch (rand.Next(0, 4))
                {
                    case 0:
                        num = 1;
                        break;
                    case 1:
                        num = .5f;
                        break;
                    case 2:
                        num = -1;
                        break;
                    case 3:
                        num = -.5f;
                        break;
                    default:
                        return 0;

                }

                return num;
            }


        }

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

            iSpeedBoostInputDelay = speedBoostInputDelay;
            iSpeedBoostTime = speedBoostTime;
            //iFireDmgRate = fireDmgRate;

            particles = new ParticleManager();
           
            weaponWheel = new WeaponWheel(this, 25);

           

        
        }

        public int DashIndex()//Returns the index needed for the dash icon    0: Full, 1: Empty, 2+ Growing rate
        {
            int index = 0;

            if (playerState != PlayerState.Dash)
            {
                
            }
            else if(playerState == PlayerState.Dash && dashVelocity != Vector2.Zero)
            {
               
                float percent = //This is because of lazy coding before, since I never truly kept track of velocity I have to create them as I go: Giving birth to dashVelocity!
                    DistForm(startPos, new Vector2(rectangle.X, rectangle.Y))/ 
                    DistForm(startPos, new Vector2(startPos.X + (dashDistance * dashVelocity.X), startPos.Y + (dashDistance * dashVelocity.Y)));

                if (percent < 15)
                    index = 1;
                else if (percent <= 40)
                    index = 2;
                else if (percent <= 65)
                    index = 3;
                else
                    index = 4;

            }

            return index;
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
                    Input(enemies, gameTime,map);
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
                                dashVelocity.Y = -1;
                                dashVelocity.X = 0;
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
                                dashVelocity.Y = 1;
                                dashVelocity.X = 0;
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
                                dashVelocity.X = 1;
                                dashVelocity.Y = 0;
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
                                dashVelocity.X = -1;
                                dashVelocity.Y = 0;
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
                                    isColliding = true;
                                }
                                if (rectangle.TouchRightOf(boss.worldRect))
                                {
                                    while (rectangle.Left < boss.worldRect.Right)
                                    {
                                        rectangle.X += 1;
                                        position.X += 1;
                                    }
                                    isColliding = true;
                                }
                                if (rectangle.TouchBottomOf(boss.worldRect))
                                {
                                    while (rectangle.Top < boss.worldRect.Bottom)
                                    {
                                        rectangle.Y += 1;
                                        position.Y += 1;
                                    }
                                    isColliding = true;
                                }
                                if (rectangle.TouchTopOf(boss.worldRect))
                                {
                                    while (rectangle.Bottom > boss.worldRect.Top)
                                    {
                                        rectangle.Y -= 1;
                                        position.Y -= 1;
                                    }
                                    isColliding = true;
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
                    if (currWeapon == WeaponType.Burst)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (bullets[j].rect.Intersects(bullets[i].rect))
                            {
                                if (bullets[j].bulletSpeed.X > 0)
                                    bullets[j].maxSpeed.X = bulletMaxX / 100;
                                else
                                    bullets[j].maxSpeed.X = -bulletMaxX / 100;

                                if (bullets[j].bulletSpeed.Y < 0)
                                    bullets[j].maxSpeed.Y = -bulletMaxY / 100;
                                else
                                    bullets[j].maxSpeed.Y = bulletMaxY / 100;
                            }
                            else
                            {
                                if (bullets[j].maxSpeed.X > 0)
                                    bullets[j].maxSpeed.X = bulletMaxX;
                                else
                                    bullets[j].maxSpeed.X = -bulletMaxX;

                                if (bullets[j].bulletSpeed.Y < 0)
                                    bullets[j].maxSpeed.Y = -bulletMaxY;
                                else
                                    bullets[j].maxSpeed.Y = bulletMaxY;
                            }
                        }
                    }
                    
                    bullets[i].Update(gameTime);
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

            for(int i = bombs.Count - 1; i >= 0; i--)
            {
                bombs[i].Update(gameTime);

                if (bombs[i].delete)
                {
                    explosions.Add(new Explosion(new Circle(new Vector2(bombs[i].rect.X, bombs[i].rect.Y), bombs[i].rect.Width),
                        1, (int)(bombs[i].rect.Width * 2.5f)));

                    int radiusDif = explosions[explosions.Count - 1].maxSize - explosions[explosions.Count - 1].rect.Radius;

                    particles.MakeExplosion(explosions[explosions.Count - 1].rect.Bounds,
                           new Circle(new Vector2(explosions[explosions.Count - 1].rect.Bounds.X - radiusDif,
                           explosions[explosions.Count - 1].rect.Bounds.Y - radiusDif), explosions[explosions.Count - 1].maxSize / 2),
                           20);

                    bombs.RemoveAt(i);
                }
            }

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].rect.Radius >= explosions[i].maxSize)
                {
                    //particles.CreateEffect(20);

                    explosions.RemoveAt(i);

                }
            }

            animManager.Update(gameTime, new Vector2(rectangle.X, rectangle.Y - (64 - rectangle.Height)));
            particles.Update(gameTime);
            weaponWheel.Update(this);
        }

        private void Input(List<TDEnemy> enemies, GameTime gameTime, TopDownMap map)
        {
            if(kb.IsKeyDown(KeyBinds.TopDownInputs[6]) && prevKb.IsKeyDown(KeyBinds.TopDownInputs[6]) && playerState != PlayerState.Dash||
                currButtons.RightShoulder == ButtonState.Pressed && prevButtons.RightShoulder == ButtonState.Pressed && playerState != PlayerState.Dash)
            {
                lockDir = true;//!lockDir
            }
            else
            {
                lockDir = false;
            }
            //Else ifs for cardinal
            if(inputDelay)
            {

            }
            else
            {
                if (kb.IsKeyDown(KeyBinds.TopDownInputs[1]) || controllerMoveDir.X > 0 /*&& controllerMoveDir.Y > -.9 && controllerMoveDir.Y < .9*/)
                {
                    velocity.X += moveSpeed;
                    if (!lockDir && shootDir != "right" || playerState == PlayerState.Dash)
                    {
                        shootDir = "right";

                        ChangeAnimation();
                    }

                    if (moveDirs[0] == null)
                    {
                        moveDirs[0] = "right";
                    }
                    else if (moveDirs[1] == null && moveDirs[0] != "right" || moveDirs[1] != "right" && moveDirs[0] != "right")
                    {
                        moveDirs[1] = "right";
                    }

                }
                if (kb.IsKeyDown(KeyBinds.TopDownInputs[0]) || controllerMoveDir.X < 0/* && controllerMoveDir.Y > -.9 && controllerMoveDir.Y < .9*/)
                {
                    velocity.X += -moveSpeed;
                    if (!lockDir && shootDir != "left" || playerState == PlayerState.Dash)
                    {
                        shootDir = "left";


                        ChangeAnimation();
                    }

                    if (moveDirs[0] == null)
                    {
                        moveDirs[0] = "left";
                    }
                    else if (moveDirs[1] == null && moveDirs[0] != "left" || moveDirs[1] != "left" && moveDirs[0] != "left")
                    {
                        moveDirs[1] = "left";
                    }
                }
                if (kb.IsKeyDown(KeyBinds.TopDownInputs[2]) ||/* controllerMoveDir.X < .6 &&*/ controllerMoveDir.Y > 0 /*&& controllerMoveDir.X > -.6*/)
                {
                    velocity.Y += -moveSpeed;
                    if (!lockDir && shootDir != "up" || playerState == PlayerState.Dash)
                    {
                        shootDir = "up";

                        ChangeAnimation();
                    }

                    if (moveDirs[0] == null)
                    {
                        moveDirs[0] = "up";
                    }
                    else if (moveDirs[1] == null && moveDirs[0] != "up" || moveDirs[1] != "up" && moveDirs[0] != "up")
                    {
                        moveDirs[1] = "up";
                    }
                }
                if (kb.IsKeyDown(KeyBinds.TopDownInputs[3]) ||/* controllerMoveDir.X < .6 &&*/ controllerMoveDir.Y < 0 /*&& controllerMoveDir.X > -.6*/ )
                {
                    velocity.Y += moveSpeed;
                    if (!lockDir && shootDir != "down" || playerState == PlayerState.Dash)
                    {
                        shootDir = "down";


                        ChangeAnimation();
                    }

                    if (moveDirs[0] == null)
                    {
                        moveDirs[0] = "down";
                    }
                    else if (moveDirs[1] == null && moveDirs[0] != "down" || moveDirs[1] != "down" && moveDirs[0] != "down")
                    {
                        moveDirs[1] = "down";
                    }
                }

                if (kb.IsKeyUp(KeyBinds.TopDownInputs[0]) && kb.IsKeyUp(KeyBinds.TopDownInputs[1]) && controllerMoveDir.X == 0)
                {
                    velocity.X = 0;
                }
                if (kb.IsKeyUp(KeyBinds.TopDownInputs[3]) && kb.IsKeyUp(KeyBinds.TopDownInputs[2]) && controllerMoveDir.Y == 0)
                {
                    velocity.Y = 0;
                }
            }

            
            if(GamePad.GetState(0).ThumbSticks.Right != Vector2.Zero || kb.IsKeyDown(KeyBinds.TopDownInputs[7]) || kb.IsKeyDown(KeyBinds.TopDownInputs[8]) || kb.IsKeyDown(KeyBinds.TopDownInputs[9])
                || kb.IsKeyDown(KeyBinds.TopDownInputs[10]) || kb.IsKeyDown(KeyBinds.TopDownInputs[11]))
            {
                weaponWheelActiveDelay = wwActiveDelayMax;

                weaponWheel.active = true;
            }
            else if(GamePad.GetState(0).ThumbSticks.Right == Vector2.Zero || kb.IsKeyUp(KeyBinds.TopDownInputs[7]) && kb.IsKeyUp(KeyBinds.TopDownInputs[8]) && kb.IsKeyUp(KeyBinds.TopDownInputs[9])
                && kb.IsKeyUp(KeyBinds.TopDownInputs[10]) && kb.IsKeyUp(KeyBinds.TopDownInputs[11]))
            {
                weaponWheelActiveDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(weaponWheelActiveDelay < 0)
                {
                    weaponWheel.active = false;
                }
            }
            

            if(weaponWheel.active)
            {
                Vector2 rightDir = GamePad.GetState(0).ThumbSticks.Right;

                if (GamePad.GetState(0).IsButtonDown(Buttons.RightStick) || GamePad.GetState(0).IsButtonDown(Buttons.RightTrigger) ||
                   kb.IsKeyDown(KeyBinds.TopDownInputs[7]))
                {
                    currWeapon = WeaponType.Pistol;
                    selectedWeapon = 0;

                    bulletDmg = pistolDmg;
                    SetShootDelays();
                }
                else if (kb.IsKeyDown(KeyBinds.TopDownInputs[9]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[9]) && game.bossKillCount >= 2 ||
                    rightDir.X > 0 && game.bossKillCount >= 2)
                {
                    currWeapon = WeaponType.Burst;
                    selectedWeapon = 2;

                    bulletDmg = burstDmg;
                    SetShootDelays();


                }
                else if (kb.IsKeyDown(KeyBinds.TopDownInputs[8]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[8]) && game.bossKillCount >= 4 ||
                    rightDir.X < 0 && game.bossKillCount >= 4)
                {

                    currWeapon = WeaponType.Laser;
                    selectedWeapon = 1;

                    bulletDmg = laserDmg;
                    SetShootDelays();
                }
                else if (kb.IsKeyDown(KeyBinds.TopDownInputs[10]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[10]) && game.bossKillCount >= 3 ||
                   rightDir.Y > 0 && game.bossKillCount >= 3)
                {
                    currWeapon = WeaponType.Bomb;
                    selectedWeapon = 3;

                    bulletDmg = bombDmg;
                    SetShootDelays();
                }
                else if (kb.IsKeyDown(KeyBinds.TopDownInputs[11]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[11]) && game.bossKillCount >= 1 ||
                    rightDir.Y < 0 && game.bossKillCount >= 1)
                {
                    currWeapon = WeaponType.Shotgun;
                    selectedWeapon = 4;

                    bulletDmg = shotGunDmg;
                    SetShootDelays();
                }
            }



            //if(kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D) && kb.IsKeyUp(Keys.S) && kb.IsKeyUp(Keys.W))
            //{
            //    moveDirs = new string[2];
            //}
           

        
            switch(currWeapon)
            {
                case WeaponType.Pistol:
                    pistolDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case WeaponType.Shotgun:
                    shotGunDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case WeaponType.Burst:
                    burstDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case WeaponType.Bomb:
                    bombDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case WeaponType.Laser:
                    laserDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
            }
        //shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(KeyBinds.TopDownInputs[5]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[5]) || currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released)
            {
                float num1 = 180;

                switch (currWeapon)
                {
                    case WeaponType.Pistol:
                       

                        if (kb.IsKeyDown(KeyBinds.TopDownInputs[5]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[5]) && pistolDelay <= 0
                            || currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released && pistolDelay <= 0)
                        {
                            bulletTravelDist = 64 * 4.75f;
                            switch (lockedDirection)
                            {
                                case "up":
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14/2), rectangle.Center.Y + (14/2)), -bulletSpeed, new Vector2(bulletMaxX, -bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed,
                                        angle: MathHelper.ToRadians(-90)));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height / 4, 0);
                                    break;
                                case "down":
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed,
                                        angle: MathHelper.ToRadians(90)));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height / 3f, 0);
                                    break;
                                case "left":
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X + (14), rectangle.Center.Y - (14 / 2)), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist,
                                        angle: MathHelper.ToRadians(180)));
                                    break;
                                case "right":
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(0, -rectangle.Height / 3);
                                    break;
                            }

                            sounds.AddSound("SoundEffects/Shoot", false);
                            sounds.PlaySound();
                            bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                            pistolDelay = maxPistolDelay;
                        }
                        break;
                    case WeaponType.Shotgun:

                        //bulletTravelDist = 64 * 2.75f;
                        if (kb.IsKeyDown(KeyBinds.TopDownInputs[5]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[5]) && shotGunDelay <= 0
                           || currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released && shotGunDelay <= 0)
                        {
                            sounds.AddSound("SoundEffects/Shoot", false, -.7f);
                            sounds.PlaySound();
                            sounds.AddSound("SoundEffects/Shoot", false, -.7f);
                            sounds.PlaySound();

                            bulletTravelDist = 64 * 2.75f;
                            float speedOffset = 3f;
                            switch (lockedDirection)
                            {
                                case "up":
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y + (14 / 2)), -bulletSpeed, new Vector2(bulletMaxX, -bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed,
                                        angle: MathHelper.ToRadians(-90)));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height / 4, 0);
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), -bulletSpeed/speedOffset, new Vector2(-bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed,
                                        angle: MathHelper.ToRadians(-90)));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height / 4, 0);
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed/speedOffset, new Vector2(bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed,
                                   angle: MathHelper.ToRadians(-90)));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height / 4, 0);
                                    break;
                                case "down":
                                    
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed,
                                        angle: MathHelper.ToRadians(90)));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height/3f, 0);
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), -bulletSpeed / speedOffset, new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed,
                                       angle: MathHelper.ToRadians(90)));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height/3f, 0);
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed / speedOffset, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed,
                                   angle: MathHelper.ToRadians(90)));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height/3f, 0);

                                    break;
                                case "left":

                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X + (14 / 2), rectangle.Center.Y - (14 / 2)), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist,
                                        angle: MathHelper.ToRadians(180)));
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X + (14 / 2), rectangle.Center.Y - (14 / 2)), -bulletSpeed, new Vector2(-bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed/speedOffset,
                                       angle: MathHelper.ToRadians(180)));
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X + (14 / 2), rectangle.Center.Y - (14 / 2)), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed/speedOffset,
                                   angle: MathHelper.ToRadians(180)));

                                    break;
                                case "right":
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(0, -rectangle.Height / 3);
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed, 
                                        new Vector2(bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / speedOffset));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(0, -rectangle.Height / 3);
                                    bullets.Add(new Bullet(new Vector2(rectangle.Center.X, rectangle.Center.Y - 22), bulletSpeed, new Vector2(bulletMaxX, -bulletMaxY),
                                        content, true, bulletTravelDist, true, -bulletSpeed / speedOffset));
                                    bullets[bullets.Count - 1].animOffset = new Vector2(0, -rectangle.Height / 3);
                                    break;
                            }

                            shotGunDelay = maxShotgunDelay;
                        }

                        break;
                    case WeaponType.Burst:
                       

                        if (kb.IsKeyDown(KeyBinds.TopDownInputs[5]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[5]) && burstDelay <= 0
                         || currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released && burstDelay <= 0)
                        {
                            bulletTravelDist = 64 * 5.5f;
                            switch(lockedDirection)
                            {
                                case "up":
                                    for (int j = 0; j < 3; j++)
                                    {
                                        bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed,
                                      new Vector2(bulletMaxX, -bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed, angle: MathHelper.ToRadians(-90)));
                                        bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                        bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Height/4, 0);


                                        sounds.AddSound("SoundEffects/Shoot", false, -.25f);
                                        sounds.PlaySound();
                                    }
                                    break;
                                case "down":
                                    for (int j = 0; j < 3; j++)
                                    {
                                        bullets.Add(new Bullet(new Vector2(rectangle.Center.X, rectangle.Center.Y - (14 / 2)), bulletSpeed,
                                      new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed, angle: MathHelper.ToRadians(90)));
                                        bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                        bullets[bullets.Count - 1].animOffset = new Vector2(-rectangle.Width/2, 0);
                                        sounds.AddSound("SoundEffects/Shoot", false, -.25f);
                                        sounds.PlaySound();
                                    }
                                    break;
                                case "right":
                                    for (int j = 0; j < 3; j++)
                                    {
                                        bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed,
                                      new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, false, bulletSpeed, angle: MathHelper.ToRadians(0)));
                                        bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                        bullets[bullets.Count - 1].animOffset = new Vector2(rectangle.Height/4, -rectangle.Height / 3);
                                        sounds.AddSound("SoundEffects/Shoot", false, -.25f);
                                        sounds.PlaySound();
                                    }
                                    break;
                                case "left":
                                    for (int j = 0; j < 3; j++)
                                    {
                                        bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), -bulletSpeed,
                                      new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, false, bulletSpeed, angle: MathHelper.ToRadians(180)));
                                        bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                        sounds.AddSound("SoundEffects/Shoot", false, -.25f);
                                        sounds.PlaySound();
                                    }
                                    break;
                            }


                            burstDelay = maxBurstDelay;
                        }
                        break;
                    case WeaponType.Bomb:

                        if (kb.IsKeyDown(KeyBinds.TopDownInputs[5]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[5]) && bombDelay <= 0
                       || currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released && bombDelay <= 0)
                        {
                            bulletTravelDist = 64 * 3.5f;

                            switch (lockedDirection)
                            {
                                case "up":
                                    bombs.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed,
                                new Vector2(bulletMaxX, -bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed));
                                    bombs[bombs.Count - 1].BulletType = Bullet.BulletTypes.Bomb;
                                    bombs[bombs.Count - 1].animOffset = new Vector2(-14/2, 0);
                                    sounds.AddSound("SoundEffects/ThrowBomb", false, RandomPitch);
                                    sounds.PlaySound();
                                    break;
                                case "down":
                                    bombs.Add(new Bullet(new Vector2(rectangle.Center.X, rectangle.Center.Y - (14 / 2)), bulletSpeed,
                                      new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed));
                                    bombs[bombs.Count - 1].BulletType = Bullet.BulletTypes.Bomb;
                                    bombs[bombs.Count - 1].animOffset = new Vector2(-14 / 2, 0);
                                    sounds.AddSound("SoundEffects/ThrowBomb", false, RandomPitch);
                                    sounds.PlaySound();
                                    break;
                                case "right":
                                    bombs.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), bulletSpeed,
                                      new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, false, bulletSpeed));

                                    bombs[bombs.Count - 1].BulletType = Bullet.BulletTypes.Bomb;
                                    bombs[bombs.Count - 1].animOffset = new Vector2(0, -14/2);
                                    sounds.AddSound("SoundEffects/ThrowBomb", false, RandomPitch);
                                    sounds.PlaySound();
                                    break;
                                case "left":

                                    bombs.Add(new Bullet(new Vector2(rectangle.Center.X - (14 / 2), rectangle.Center.Y - (14 / 2)), -bulletSpeed,
                                      new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, false, bulletSpeed));

                                    bombs[bombs.Count - 1].BulletType = Bullet.BulletTypes.Bomb;
                                   // bombs[bombs.Count - 1].animOffset = new Vector2(0, -14 / 2);
                                    sounds.AddSound("SoundEffects/ThrowBomb", false, RandomPitch);
                                    sounds.PlaySound();
                                    break;
                            }

                            bombDelay = maxBombDelay;
                        }
                           

                    //     if (kb.IsKeyDown(KeyBinds.TopDownInputs[5]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[5]) && bombDelay <= 0
                    //|| currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released && bombDelay <= 0)
                    //     {
                    //         if (animManager.isRight)
                    //         {
                    //             sounds.AddSound("SoundEffects/ThrowBomb", false);
                    //             sounds.PlaySound();

                    //             bulletSpeed = 3.5f;
                    //             bombs.Add(new Bomb(new Circle(new Vector2(position.X + rectangle.Width,
                    //               (rectangle.Center.Y - (15) + rectangle.Height / 2f)), 15), bulletSpeed, -bulletSpeed * 5f, content));
                    //         }
                    //         else if (animManager.isLeft)
                    //         {
                    //             sounds.AddSound("SoundEffects/ThrowBomb", false);
                    //             sounds.PlaySound();

                    //             bulletSpeed = 3.5f;
                    //             bombs.Add(new Bomb(new Circle(new Vector2(position.X/* - (18 / 2)*/,
                    //                 (rectangle.Center.Y - (15) + rectangle.Height / 2f)), 15), -bulletSpeed, -bulletSpeed * 5f, content));
                    //         }

                    //         bombDelay = maxBombDelay;
                    //     }
                         break;
                    case WeaponType.Laser:

                      

                   //     if (kb.IsKeyDown(KeyBinds.TopDownInputs[5]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[5]) && laserDelay <= 0
                   //|| currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released && laserDelay <= 0)
                   //     {
                   //         if (animManager.isRight)
                   //         {
                   //             bulletSpeed = 4.5f;
                   //             bulletTravelDist = 64 * 4f;
                   //             for (int j = 0; j < 8; j++)
                   //             {
                   //                 bullets.Add(new Bullet(new Vector2(position.X + rectangle.Width,
                   //               rectangle.Center.Y - (14 / 2)), bulletSpeed,
                   //               new Vector2(bulletMaxX, -bulletMaxY), content, true, bulletTravelDist));
                   //                 bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                   //                 sounds.AddSound("SoundEffects/Shoot", false, .5f);
                   //                 sounds.PlaySound();
                   //             }
                   //         }
                   //         else if (animManager.isLeft)
                   //         {
                   //             bulletSpeed = 4.5f;
                   //             bulletTravelDist = 64 * 4f;
                   //             for (int j = 0; j < 8; j++)
                   //             {
                   //                 bullets.Add(new Bullet(new Vector2(position.X /*- (18 / 2)*/,
                   //               rectangle.Center.Y - (14 / 2)), -bulletSpeed,
                   //               new Vector2(-bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, angle: num1));
                   //                 bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                   //                 sounds.AddSound("SoundEffects/Shoot", false, .5f);
                   //                 sounds.PlaySound();
                   //             }
                   //         }

                   //         laserDelay = maxLaserDelay;
                   //     }
                        break;
                }


                //if (shootDelay <= 0)
                //{
                //    switch (lockedDirection)
                //    {
                //        case "up":
                //            bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (22), rectangle.Y), -bulletSpeed, new Vector2(bulletMaxX, -bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed, 
                //                angle: MathHelper.ToRadians(-90)));

                //            break;
                //        case "down":
                //            bullets.Add(new Bullet(new Vector2(rectangle.Center.X - (22), rectangle.Bottom), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed,
                //                angle:MathHelper.ToRadians(90)));
                //            break;
                //        case "left":
                //            bullets.Add(new Bullet(new Vector2(rectangle.Center.X, rectangle.Center.Y), -bulletSpeed, new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist,
                //                angle:MathHelper.ToRadians(180)));
                //            break;
                //        case "right":
                //            bullets.Add(new Bullet(new Vector2(rectangle.Center.X, rectangle.Center.Y - (22)), bulletSpeed, new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                //            break;
                //    }
                //    bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;

                //    //if (animManager.isUp)
                //    //    bullets.Add(new Bullet(new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Top), -bulletSpeed, 
                //    //        new Vector2(bulletMaxX, -bulletMaxY), content, false, bulletTravelDist, true, -bulletSpeed));
                //    //else if(animManager.isDown)
                //    //    bullets.Add(new Bullet(new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Bottom), bulletSpeed, 
                //    //        new Vector2(bulletMaxX, bulletMaxY), content, false, bulletTravelDist, true, bulletSpeed));
                //    //else if(animManager.isLeft)
                //    //    bullets.Add(new Bullet(new Vector2(rectangle.Left, rectangle.Y + (rectangle.Height / 2)), -bulletSpeed, 
                //    //        new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                //    //else if(animManager.isRight)
                //    //    bullets.Add(new Bullet(new Vector2(rectangle.Right, rectangle.Y + (rectangle.Height / 2)), bulletSpeed, 
                //    //        new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));



                //    shootDelay = iShootDelay;
                //}


            }

            if (kb.IsKeyDown(KeyBinds.TopDownInputs[4]) && prevKb.IsKeyUp(KeyBinds.TopDownInputs[4]) || currButtons.B == ButtonState.Pressed && prevButtons.B == ButtonState.Released)
            {
                lockDir = false;
                playerState = PlayerState.Dash;

                if(moveDirs[1] != null)
                {
                    shootDir = moveDirs[1];
                }
                else if(moveDirs[0] != null)
                {
                    shootDir = moveDirs[0];
                }
               // nput(enemies, gameTime);

                startPos = position;
            }
            moveDirs = new string[2];
            //if(kb.IsKeyDown(Keys.F) && prevKb.IsKeyUp(Keys.F) && meleeDelay <= 0 || currButtons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released && meleeDelay <= 0)
            //{
            //    meleeDelay = iMeleeDelay;
            //    foreach(TDEnemy enemy in enemies)
            //    {
            //        if(MeleeHitbox.Intersects(enemy.Rectangle))
            //        {
            //            enemy.Health -= meleeDmg;
            //        }
            //    }
            //}
            prevKb = kb;
            prevButtons = currButtons;

            bool triggerEnter = false;
            int i = 0;
            int currFireTile = 0;
            foreach(EnvironmentTile environmentTile in map.EnvironmentTiles)
            {
                switch (environmentTile.effectType)
                {
                    case EnvironmentTile.Type.SpeedBoost:
                        if (new Rectangle(environmentTile.Rectangle.Center.X - 10, environmentTile.Rectangle.Center.Y - 10, 20, 20).Intersects(rectangle) /*&& !speedBoosted*/)
                        {
                            boostedSpeed = moveSpeed * 1.75f;
                            speedBoosted = true;
                            inputDelay = true;
                            speedBoostInputDelay = .6f;
                            switch (environmentTile.direction)
                            {
                                case "right":
                                    velocity.Y = 0;
                                    velocity.X += boostedSpeed;
                                    break;
                                case "left":
                                    velocity.Y = 0;
                                    velocity.X += -boostedSpeed;
                                    break;
                                case "up":
                                    velocity.Y = -boostedSpeed;
                                    velocity.X += 0;
                                    break;
                                case "down":
                                    velocity.X = 0;
                                    velocity.Y += boostedSpeed;
                                    break;
                            }
                        }
                       
                        break;
                    case EnvironmentTile.Type.DamageOverTime:
                        if (new Rectangle(environmentTile.Rectangle.Center.X - 20, environmentTile.Rectangle.Center.Y - 20, 40, 40).Intersects(rectangle) /*&& !speedBoosted*/)
                        {
                            triggerEnter = true;
                            currFireTile = i;
                            if (inDOT)
                            {

                            }
                            else
                            {
                                //triggerEnter = true;
                                inDOT = true;
                                fireDmgRate = iFireDmgRate;
                            }
                        }
                           
                        break;
                }
               
                i++;
            }
            if (!triggerEnter)
                inDOT = false;



            if(!speedBoosted)
            {
                if (velocity.X > moveSpeed)
                {
                    velocity.X = moveSpeed;
                }
                else if (velocity.X < -moveSpeed)
                {
                    velocity.X = -moveSpeed;
                }
                if (velocity.Y > moveSpeed)
                {
                    velocity.Y = moveSpeed;
                }
                else if (velocity.Y < -moveSpeed)
                {
                    velocity.Y = -moveSpeed;
                }

                if (velocity.X != 0 && velocity.Y != 0)
                {
                    velocity = new Vector2(velocity.X / 1.5f, velocity.Y / 1.5f);
                }
            }
            else if(speedBoosted)
            {
                
                if (velocity.X > boostedSpeed)
                {
                    velocity.X = boostedSpeed;
                }
                else if (velocity.X < -boostedSpeed)
                {
                    velocity.X = -boostedSpeed;
                }
                if (velocity.Y > boostedSpeed)
                {
                    velocity.Y = boostedSpeed;
                }
                else if (velocity.Y < -boostedSpeed)
                {
                    velocity.Y = -boostedSpeed;
                }

                if(velocity.X != 0 && velocity.Y != 0)
                {
                    velocity = new Vector2(velocity.X / 1.5f, velocity.Y / 1.5f);
                }
                
                speedBoostInputDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                speedBoostTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (speedBoostInputDelay <= 0)
                {
                    //speedBoostInputDelay = .25f;
                    inputDelay = false;
                }
                if(speedBoostTime <= 0 || !speedBoosted)
                {
                    speedBoostInputDelay = iSpeedBoostInputDelay;
                    speedBoostTime = iSpeedBoostTime;
                    inputDelay = false;
                    speedBoosted = false;
                }
               
            }
           
            if(inDOT)
            {
                fireDmgRate -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(fireDmgRate <= 0)
                {
                    Health -= fireDmg;
                    fireDmgRate = iFireDmgRate;
                    particles.MakeFireSpit(map.EnvironmentTiles[currFireTile].Rectangle,
                        new Circle(new Vector2(map.EnvironmentTiles[currFireTile].Rectangle.Center.X, map.EnvironmentTiles[currFireTile].Rectangle.Center.Y),
                        1), 32);
                    //particles.MakeFireSpit(map.EnvironmentTiles[currFireTile].Rectangle,
                    //  new Circle(new Vector2(map.EnvironmentTiles[currFireTile].Rectangle.Center.X, map.EnvironmentTiles[currFireTile].Rectangle.Center.Y),
                    //  1), 32);
                    //particles.MakeFireSpit(map.EnvironmentTiles[currFireTile].Rectangle,
                    //  new Circle(new Vector2(map.EnvironmentTiles[currFireTile].Rectangle.Center.X, map.EnvironmentTiles[currFireTile].Rectangle.Center.Y),
                    //  1), 32);
                }
            }

            position += velocity;
            if(controllerMoveDir != Vector2.Zero)
            {

            }

           
        }

        private void SetShootDelays()
        {
            shotGunDelay = maxShotgunDelay;
            pistolDelay = maxPistolDelay;
            burstDelay = maxBurstDelay;
            laserDelay = maxLaserDelay;
            bombDelay = maxBombDelay;
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


                if (speedBoosted)
                {
                    if (rectangle.Center.X > newRect.Center.X)
                    {
                        position.X += 2;
                        rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                    }
                    else
                    {
                        position.X -= 2;
                        rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                    }

                    inputDelay = false;
                }
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


                if (speedBoosted)
                {
                    if (rectangle.Center.X > newRect.Center.X)
                    {
                        position.X += 2;
                        rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                    }
                    else
                    {
                        position.X -= 2;
                        rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                    }
                    inputDelay = false;
                }
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

                if (speedBoosted)
                {
                    if (rectangle.Center.Y > newRect.Center.Y)
                    {
                        position.Y += 2;
                        rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                    }
                    else
                    {
                        position.Y -= 2;
                        rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                    }
                    inputDelay = false;
                }
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

                if(speedBoosted)
                {
                    if(rectangle.Center.Y > newRect.Center.Y)
                    {
                        position.Y += 2;
                        rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                    }
                    else
                    {
                        position.Y -= 2;
                        rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                    }
                    inputDelay = false;
                }
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

            if (game.levelCount == 0)
                game.levelCount++;
            position.X += pixelSize * 2;
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

            if (game.levelCount == 0)
                game.levelCount++;

            position.X -= pixelSize * 2;


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


            if (game.levelCount == 0)
                game.levelCount++;

            position.Y -= pixelSize * 2;

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

            if (game.levelCount == 0)
                game.levelCount++;


            position.Y += pixelSize * 2;
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
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
            foreach (Bullet bomb in bombs)
            {
                bomb.Draw(spriteBatch);
            }
            if (damaged)
            {
                if (redCount <= whiteCount || redCount == 0 && whiteCount == 0)
                {
                    animManager.Draw(spriteBatch, Color.White);
                    redCount+=3;
                }
                if (whiteCount < redCount)
                {
                    animManager.Draw(spriteBatch, Color.Red * .25f);
                    whiteCount++;
                }
                if (whiteCount == whiteFrames)
                {
                    damaged = false;
                    whiteCount = 0;
                    redCount = 0;
                }
            }
            else
            {
                animManager.Draw(spriteBatch, Color.White);
            }

            //spriteBatch.Draw(content.Load<Texture2D>("TopDown/Textures/Player"), MeleeHitbox, Color.White);
            //spriteBatch.Draw(texture, rectangle, Color.White);
            //animManager.Draw(spriteBatch, Color.White);

      
            weaponWheel.Draw(spriteBatch, content, selectedWeapon);
            particles.Draw(spriteBatch);
        }
        int DistForm(Vector2 pos1, Vector2 pos2)
        {
            int num = (int)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
            return num;

        }
    }


    


    class WeaponWheel
    {
        public List<WeaponSlot> WeaponSlots = new List<WeaponSlot>();
        public bool active;
        int size;
        public WeaponWheel(TDPlayer player, int size)
        {
            WeaponSlots = new List<WeaponSlot>();
            active = false;
            this.size = size;
            for (int i = 0; i < 5; i++)
            {
                WeaponSlots.Add(new WeaponSlot());

            }
            WeaponSlots[0].rect = new Rectangle(player.rectangle.Center.X - size / 2,
                (int)(player.rectangle.Center.Y - (size * 3f)), size, size); //Center
            WeaponSlots[1].rect = new Rectangle(WeaponSlots[0].rect.X - (size + 2),
                WeaponSlots[0].rect.Y, size, size);//Left
            WeaponSlots[2].rect = new Rectangle(WeaponSlots[0].rect.Right + 2,
                WeaponSlots[0].rect.Y, size, size);//Right
            WeaponSlots[3].rect = new Rectangle(WeaponSlots[0].rect.X,
                WeaponSlots[0].rect.Y - (size + 2), size, size);//Top
            WeaponSlots[4].rect = new Rectangle(player.rectangle.Center.X - size / 2,
                WeaponSlots[0].rect.Bottom + 2, size, size);//Bottom

        }

        public void Update(TDPlayer player)
        {


            WeaponSlots[0].rect = new Rectangle(player.rectangle.Center.X - size / 4,
                       (int)(player.rectangle.Center.Y - (size * 3.75f)), size, size); //Center
            WeaponSlots[1].rect = new Rectangle(WeaponSlots[0].rect.X - (size + 2),
                WeaponSlots[0].rect.Y, size, size);//Left
            WeaponSlots[2].rect = new Rectangle(WeaponSlots[0].rect.Right + 2,
                WeaponSlots[0].rect.Y, size, size);//Right
            WeaponSlots[3].rect = new Rectangle(WeaponSlots[0].rect.X,
                WeaponSlots[0].rect.Y - (size + 2), size, size);//Top
            WeaponSlots[4].rect = new Rectangle(player.rectangle.Center.X - size / 4,
                WeaponSlots[0].rect.Bottom + 2, size, size);//Bottom
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager content, int selectedWeapon)
        {
            if (active)
            {
                for (int i = 0; i < WeaponSlots.Count; i++)
                {
                    if (selectedWeapon == i)
                        spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), WeaponSlots[i].rect, Color.Blue * .35f);
                    else
                        spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), WeaponSlots[i].rect, Color.White * .35f);

                }

            }
        }
    }

    class WeaponSlot
    {
        public Rectangle rect;
        public Texture2D texture;
    }
}
