using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using AUTO_Matic.SideScroll;
using AUTO_Matic.TopDown;
using System;
using AUTO_Matic.Scripts.TopDown;
using AUTO_Matic.Scripts.SideScroll;
using System.Threading;
using AUTO_Matic.SideScroll.Enemy;
using AUTO_Matic.Scripts;
using AUTO_Matic.Scripts.SideScroll.Enemy;
using AUTO_Matic.Scripts.TopDown.AUTO_Matic.Scripts.TopDown;
using AUTO_Matic.Scripts.TopDown.Bosses;
using AUTO_Matic.Scripts.Effects;
using System.IO;


namespace AUTO_Matic
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        ControllerReticle controllerReticle;
        bool startGame = false;
        UIManager UIManager = new UIManager();
        List<HealthDrop> healthDrops = new List<HealthDrop>();
        Random rand = new Random();
        int dropRateSS = 40;
        int dropRateTD = 42;
        float healAmount = 2.25f;
        public int openDoorCount = 0; //Counts how many times opened doors in the main SideScroll section
        public int bossKillCount = 0; //How many bosses defeated

        Rectangle LeaveDungeon = Rectangle.Empty;
        public bool deleteSave = false;
        bool closeDoor = false;

        int MenuButtonIndex = 0;
        int PauseMenuButtonIndex = 0;

        public enum Scenes { TitleScreen, InGame, Exit }
        public Scenes currScene = Scenes.TitleScreen;

        public enum GameStates { Tutorial, SideScroll, TopDown, Paused, FinalBoss, EndGame, InGamePause}
        public GameStates GameState = GameStates.SideScroll;
        public GameStates prevGameState;

        public enum MenuStates { TitleCrawl, MainMenu, Settings, StartGame}
        public MenuStates MenuState = MenuStates.TitleCrawl;
        bool startCrawl = false;

        Vector2 mainMenuPos = new Vector2(768 / 2, 432 / 2);

        KeyboardState prevKB;
        GamePadButtons prevButtons;

        Point screenCenter;
        Point saveMousePoint;
        public MouseState ms;
        public MouseState prevMs;
        Vector2 mousePos = Vector2.Zero;
        int count = 0;
        bool dont = false;
        #region Side-Scroll
        SSPlayer ssPlayer;
        float gravityX = 0f;
        float gravityY = 1f;
        Vector2 Gravity
        {
            get { return new Vector2(gravityX, gravityY); }

        }

        public int mapWidth = 25;
        public int mapHeight = 12;
        int[,] currMap = new int[20, 35];
        float friction = .85f;
        bool updateDoor = false;
        int topIndex, bottomIndex;
        float doorOpenSpeed = 1f;
        #endregion

        List<Rectangle> healthBar = new List<Rectangle>();

        List<SSEnemy> enemies = new List<SSEnemy>();
        Vector2 moveDir;
        #region TopDown
        TopDownMap tdMap;
        TDPlayer tdPlayer;

        public List<Vector2> BoundIndexes = new List<Vector2>();
        int pixelBits = 64;
        List<TDEnemy> tdEnemies = new List<TDEnemy>();
        public int levelCount = 0;
        bool startBoss = false;
        ShotGunBoss shotGunBoss;
        LaserBoss laserBoss;
        SlamBoss slamBoss;
        BombBoss bombBoss;
        //float shootRate = .5f;
        //float maxShootRate;
        //bool canShoot = false;
        //List<Bullet> bossBullets = new List<Bullet>();
        //float bulletSpeed = 6.25f;
        //float maxBulletSpeed = 20;
        //float bulletDmg = .5f;
        //float bossHealth = 8.65f;
        //int[,] currTDMap;
        #endregion
        //SSEnemy enemy;
        SSCamera ssCamera;


        #region Paused Helpers
        bool black = true;
        bool fade = true;
        bool canChange = false;
        bool doorTrans = false;
        float rate = .010f;//.0099
        float iRate = 1;
        float sRate = 0;
        Vector2 fadePos = Vector2.Zero;
        Vector2 updatedPos;
        int minChange = 0;
        Rectangle selectedBox = Rectangle.Empty;
        #endregion

        #region SoundHelpers
        SoundManager soundManager;
        #endregion
        List<GroundTile> destroyedGround = new List<GroundTile>();
        List<PlatformTile> destroyedPlatfroms = new List<PlatformTile>();
        float respawnDelay = 0;
        float respawnDelaySet = .75f;
        bool tdCameraReached = false;

        List<GroundTile> offScreenGround = new List<GroundTile>();
        List<PlatformTile> offScreenPlatform = new List<PlatformTile>();
        List<BackgroundTile> offScreenBackground = new List<BackgroundTile>();
        List<WallTile> offScreenWall = new List<WallTile>();
        List<DungeonEntrance> dungeons = new List<DungeonEntrance>();

        List<HealthDrop> bossHealthDrops = new List<HealthDrop>();


        List<FlyingControlBeacon> flyingBeacons = new List<FlyingControlBeacon>();
        GamePadState prevGamePad;
        Rectangle tdPrevBounds = Rectangle.Empty;
        MapBuilder mapBuilder;
        FinalBoss finalBoss;

        Vector2 savedDoorY = Vector2.One;

        int dungeonNum = 0;

        Rectangle interactActive = Rectangle.Empty;
        float groundPoundX = 16;
        float groundPoundY = -6;

        Point healthDropDims = new Point(28, 28);

        Vector2 savedPos = Vector2.Zero;

        public bool keyBindActive = false;
        public bool changeKey = false;
        public KeyboardState kb = Keyboard.GetState();

        public Keys keyBindKey;
        int reloadCount = 0;
        public string dataPath;
        public string SideInputPath;
        public string TopDownInputPath;

        public bool isLoadedGame = false;
        bool fromDungeon = false; //Helps deal with sounds

        int killedEnemies = 0;
        int numEnemies = 0;
        class Door
        {
            BottomDoorTile bottomDoor;
            TopDoorTile topDoor;
            int topIndex, bottomIndex;
            float doorOpenSpeed = 1f;
            bool updateDoor = false;
        }
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 768;/*(int)(graphics.PreferredBackBufferWidth * 1.5f)*/
            graphics.PreferredBackBufferHeight = 432;/*(int)(graphics.PreferredBackBufferHeight * 1.5f);*/

            //graphics.HardwareModeSwitch = false;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
          
            //maxShootRate = shootRate;
            camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2));
            camera.Zoom = 1f;
           
            ssPlayer = new SSPlayer(this, 64);
            //StartNewGame();

            ms = Mouse.GetState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            controllerReticle = new ControllerReticle(new Rectangle(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2, 48, 48),
                Content.Load<Texture2D>("Textures/MouseReticle"));

            dataPath = Content.RootDirectory + "/SaveData/Save";
            SideInputPath = Content.RootDirectory + "/SaveData/SideInputSave";
            TopDownInputPath = Content.RootDirectory + "/SaveData/TopDownInputSave";
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tdPlayer = new TDPlayer(this, 64, 200, 200, UIManager, Content, bossKillCount);
            List<Texture2D> healthbars = new List<Texture2D>();
            for (int i = 0; i <= 10; i++)
            {
                healthbars.Add(Content.Load<Texture2D>(@"SideScroll\HealthBar\RoboHealthBar" + i));
            }
            List<Texture2D> dashIcons = new List<Texture2D>();
            for(int i = 0; i < 5; i++)
            {
                dashIcons.Add(Content.Load<Texture2D>(@"Textures\DashIcons\DashIcon" + i));
            }
            UIHelper.HealthBar = healthbars;
            UIHelper.DashIcon = dashIcons;

            #region UI Menus
            UIHelper.TitleFont = Content.Load<SpriteFont>(@"Fonts\TitleFont");
            
            UIHelper.ButtonTexture = Content.Load<Texture2D>(@"Textures\Button");
            UIHelper.ButtonFont = Content.Load<SpriteFont>(@"Fonts\CrawlFont");
            UIHelper.CrawlBgTxture = Content.Load<Texture2D>(@"Textures\TitleCrawlBG");
            UIHelper.MainMenuBG = Content.Load<Texture2D>(@"Textures\TitleScreen");
            UIHelper.TutorialTexture = Content.Load<Texture2D>(@"Textures\Textbox");
            UIHelper.MenuTitle = Content.Load<Texture2D>(@"Textures\Title");

            UIHelper.PlayGameBtn = Content.Load<Texture2D>(@"Textures\UI\PlayButton");
            UIHelper.ExitGameBtn = Content.Load<Texture2D>(@"Textures\UI\ExitButton");
            UIHelper.ReturnBtn = Content.Load<Texture2D>(@"Textures\UI\BackButton");
            UIHelper.SettingsBtn = Content.Load<Texture2D>(@"Textures\UI\SettingsButton");
            UIHelper.StartGameBtn = Content.Load<Texture2D>(@"Textures\UI\StartGameButton");
            UIHelper.LoadGameBtn = Content.Load<Texture2D>(@"Textures\UI\LoadGameButton");

            UIHelper.Bounds = new Rectangle(new Point((int)camera.Position.X, (int)camera.Position.Y), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            UIHelper.CrawlFont = Content.Load<SpriteFont>(@"Fonts\CrawlFont");
            UIHelper.TutorialFont = Content.Load<SpriteFont>(@"Fonts\TutorialFont");
            UIManager.CreateUIElements(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), this);
            UIHelper.SetElementVisibility("TitleCrawl", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("MainMenuTitle", true, UIManager.uiElements);

            UIHelper.SetElementVisibility("SettingsMenuTitle", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("MainMenuBackground", true, UIManager.uiElements);
            UIHelper.DungeonLevels = tdPlayer.bossRoom;

           
            #endregion

            HealthDrop.texture = Content.Load<Texture2D>(@"Textures\Health");
            Tile.Content = Content;
            soundManager = new SoundManager("Level0Side", Content, true, UIManager.MasterVolume, UIManager.EffectVolume, UIManager.MusicVolume);
            //ssPlayer.Load(Content, Window.ClientBounds, friction);
            if (currScene == Scenes.InGame)
            {
                //LoadTutorial();
              //StartNewGame();
               //LoadFinalBoss();
                
            }

            //If keyBindFile exists...Do same to Sound Settings
            if (File.Exists(SideInputPath) == false)
            {
                UIManager.SetToDefaultKeyBinds();
                SaveKeyBinds(UIManager.SideScrollInputs, UIManager.TopDownInputs);
            }

            if(File.Exists(dataPath))
            {
                string[] gameData = File.ReadAllLines(dataPath);
                UIManager.MasterVolume = float.Parse(gameData[12]);
                UIManager.EffectVolume = float.Parse(gameData[14]);
                UIManager.MusicVolume = float.Parse(gameData[13]);
            }
            UIManager.CreateSettingsUI(this);
            //UIHelper.SetElementText("VolumeSettingsMasterNum")

            UIManager.CreateTopDownTutorial(new Rectangle(new Point(0 + ((64 * 25) * (tdPlayer.levelInX - 1)), 0 - ((64 * 15) * (tdPlayer.levelInY - 1))),
                      new Point(64 * 25, 64 * 15)));


            prevGamePad = GamePad.GetState(0);
        





            // UIHelper.SetElementVisibility("ExitButton", true, UIManager.uiElements);


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void GenerateNewMap(bool xLevel, bool yLevel, bool dLevel, bool isBoss)
        {

            healthDrops.Clear();
            if(!isBoss && levelCount == 0)
            {
                tdMap.setBarrier = false;
                //levelCount++;
                //graphics.PreferredBackBufferWidth = 64 * 15; //1600  // pixelBits * col
                //graphics.PreferredBackBufferHeight = 64 * 15; //960  // pixelBits * row
                //graphics.IsFullScreen = false;
                //graphics.ApplyChanges();
                //camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)), graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))));
                //camera.Zoom = .5f;

                string filePath = Content.RootDirectory + "/TopDown/Maps/Map" + 0 + ".txt";

                if (xLevel)
                    tdPlayer.PosXLevels.xLevels.Add(tdMap.GenerateMap(filePath));
                if (yLevel)
                    tdPlayer.PosYLevels.yLevels.Add(tdMap.GenerateMap(filePath));
                if (dLevel)
                    tdPlayer.DiagLevels.dLevels.Add(tdMap.GenerateMap(filePath));

                tdEnemies.Clear();

                //tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                //    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points);

                Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                     (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                     new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
                int[,] mapDims = (tdMap.GenerateMap(filePath));

                healthDrops.Add(new HealthDrop(new Rectangle(currBounds.X + 64 * 2, currBounds.Y + 64 * 2, healthDropDims.X, healthDropDims.X)));
                healthDrops.Add(new HealthDrop(new Rectangle(currBounds.X + 64 * 2, (currBounds.Y + currBounds.Height) - 64 * 3, healthDropDims.X, healthDropDims.X)));
                healthDrops.Add(new HealthDrop(new Rectangle((currBounds.X + currBounds.Width) - 64 * 3, (currBounds.Y + currBounds.Height) - 64 * 3, healthDropDims.X, healthDropDims.X)));
                healthDrops.Add(new HealthDrop(new Rectangle((currBounds.X + currBounds.Width) - 64 * 3, (currBounds.Y) + 64 * 2, healthDropDims.X, healthDropDims.X)));

                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);

                SaveGame();
                //if (levelCount == 0)
                //    levelCount++;
            }
            else if(!isBoss && tdPlayer.bossRoom > levelCount)
            {
                tdMap.setBarrier = true;
                levelCount++;
                //Random rand = new Random();
                //int num = rand.Next(1, 11);

                //string filePath = Content.RootDirectory + "/TopDown/Maps/Map" + num + ".txt";
                currMap = mapBuilder.ChooseMap();
               
                Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                 (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                 new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));

                if (xLevel)
                    tdPlayer.PosXLevels.xLevels.Add(currMap);
                if (yLevel)
                    tdPlayer.PosYLevels.yLevels.Add(currMap);
                if (dLevel)
                    tdPlayer.DiagLevels.dLevels.Add(currMap);

                tdEnemies.Clear();
              
                tdPlayer.changeLevel = true;

                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                   tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);

                if (xLevel)
                {
                    tdPlayer.PosXLevels.xLevels.Remove(currMap);
                    tdPlayer.PosXLevels.xLevels.Add(mapBuilder.Start(75, tdMap, currBounds));
                }
                   
                if (yLevel)
                {
                    tdPlayer.PosYLevels.yLevels.Remove(currMap);
                    tdPlayer.PosYLevels.yLevels.Add(mapBuilder.Start(75, tdMap, currBounds));
                }
                   
                if (dLevel)
                {
                    tdPlayer.DiagLevels.dLevels.Remove(currMap);
                    tdPlayer.DiagLevels.dLevels.Add(mapBuilder.Start(75, tdMap, currBounds));
                }

                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                 tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);

                //numEnemies = tdMap.enemySpawnPoints.Count;
                //Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                //(0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                // new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
                //Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                //                    (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                //                    new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
                //foreach (Vector2 enemySpawn in tdMap.EnemySpawns)
                //{
                //    if (currBounds.Contains(enemySpawn))
                //        tdEnemies.Add(new TDEnemy(Content, enemySpawn, tdMap, currMap));
                //}

            }
            else if(isBoss && tdPlayer.bossRoom == levelCount)
            {
                //SaveGame();
                tdMap.setBarrier = false;
                levelCount++;
                //graphics.PreferredBackBufferWidth = 64 * 15; //1600  // pixelBits * col
                //graphics.PreferredBackBufferHeight = 64 * 15; //960  // pixelBits * row
                //graphics.IsFullScreen = false;
                //graphics.ApplyChanges();
                //camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)), graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))));
                //camera.Zoom = .5f;

                string filePath = Content.RootDirectory + "/TopDown/Maps/Map" + 0 + ".txt";

                if (xLevel)
                    tdPlayer.PosXLevels.xLevels.Add(tdMap.GenerateMap(filePath));
                if (yLevel)
                    tdPlayer.PosYLevels.yLevels.Add(tdMap.GenerateMap(filePath));
                if (dLevel)
                    tdPlayer.DiagLevels.dLevels.Add(tdMap.GenerateMap(filePath));

                tdEnemies.Clear();

                //tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                //    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points);

                Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                     (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                     new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
                int[,] mapDims = (tdMap.GenerateMap(filePath));

                healthDrops.Add(new HealthDrop(new Rectangle(currBounds.X + 64 * 2, currBounds.Y + 64 * 2, healthDropDims.X, healthDropDims.X)));
                healthDrops.Add(new HealthDrop(new Rectangle(currBounds.X + 64 * 2, (currBounds.Y + currBounds.Height) - 64 * 3, healthDropDims.X, healthDropDims.X)));
                healthDrops.Add(new HealthDrop(new Rectangle((currBounds.X + currBounds.Width) - 64 * 3, (currBounds.Y + currBounds.Height) - 64 * 3, healthDropDims.X, healthDropDims.X)));
                healthDrops.Add(new HealthDrop(new Rectangle((currBounds.X + currBounds.Width) - 64 * 3, (currBounds.Y) + 64 * 2, healthDropDims.X, healthDropDims.X)));

                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);

                SaveGame();

            }
            else if(isBoss && tdPlayer.bossRoom < levelCount)
            {
                UIManager.uiElements["BossRoomCounter"].Visible = false;
                string filePath;
                switch (dungeonNum)
                {
                    case 0:
                        filePath = Content.RootDirectory + "/TopDown/Maps/Map" + 99 + ".txt";
                        break;
                    case 1:
                        filePath = Content.RootDirectory + "/TopDown/Maps/Map" + 19 + ".txt";
                        break;
                    case 2:
                        filePath = Content.RootDirectory + "/TopDown/Maps/Map" + 69 + ".txt";
                        break;
                    case 3:
                        filePath = Content.RootDirectory + "/TopDown/Maps/Map" + 69 + ".txt";
                        break;
                    default:
                        filePath = Content.RootDirectory + "/TopDown/Maps/Map" + 19 + ".txt";
                        break;
                }
              

                if (xLevel)
                    tdPlayer.PosXLevels.xLevels.Add(tdMap.GenerateMap(filePath));
                if (yLevel)
                    tdPlayer.PosYLevels.yLevels.Add(tdMap.GenerateMap(filePath));
                if (dLevel)
                    tdPlayer.DiagLevels.dLevels.Add(tdMap.GenerateMap(filePath));

                tdEnemies.Clear();

                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);

                Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                     (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                     new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
                int[,] mapDims = (tdMap.GenerateMap(filePath));

                
                List<WallTiles> walls = new List<WallTiles>();
                for (int y = 0; y < mapDims.GetLength(0); y++)
                {
                    for (int x = 0; x < mapDims.GetLength(1); x++)
                    {
                        if (tdMap.GetPoint(y, x, mapDims) == 10)
                        {
                            walls.Add(new WallTiles(tdMap.WallIndexes[0], new Rectangle((0 + (64 * x) + currBounds.X), (0 - (64 * y) - currBounds.Y), 64, 64)));
                        }
                    }
                }

                switch(dungeonNum)
                {
                    case 0:
                        shotGunBoss = new ShotGunBoss(currBounds, 240, 240, Content, walls, currBounds, tdMap);
                        break;
                    case 1:
                        slamBoss = new SlamBoss(currBounds, Content, tdMap, mapDims, walls);
                        break;
                    case 2:
                        bombBoss = new BombBoss(currBounds, Content, tdMap, mapDims, tdPlayer, GraphicsDevice,
                    Content.Load<Effect>(@"Effects\Particles"), Content.Load<Texture2D>(@"Textures\white"));
                        break;
                    case 3:
                        laserBoss = new LaserBoss(currBounds, Content, tdMap, mapDims, GraphicsDevice);
                        break;
                }

                //laserBoss = new LaserBoss(currBounds, Content, tdMap, mapDims, GraphicsDevice);
                //
                //bombBoss = new BombBoss(currBounds, Content, tdMap, mapDims, tdPlayer, GraphicsDevice, 
                //   Content.Load<Effect>(@"Effects\Particles"), Content.Load<Texture2D>(@"Textures\white"));

                //for(int i = 0; i <= 4; i++)
                //{

                //}
                bossHealthDrops.Add(new HealthDrop(new Rectangle(currBounds.X + 64 * 2, currBounds.Y + 64 * 2, healthDropDims.X, healthDropDims.X)));
                bossHealthDrops.Add(new HealthDrop(new Rectangle(currBounds.X + 64 * 2, (currBounds.Y + currBounds.Height) - 64 * 3, healthDropDims.X, healthDropDims.X)));
                bossHealthDrops.Add(new HealthDrop(new Rectangle((currBounds.X + currBounds.Width) - 64 * 3, (currBounds.Y + currBounds.Height) - 64 * 3, healthDropDims.X, healthDropDims.X)));
                bossHealthDrops.Add(new HealthDrop(new Rectangle((currBounds.X + currBounds.Width) - 64 * 3, (currBounds.Y) + 64 * 2, healthDropDims.X, healthDropDims.X)));
                if (GameState == GameStates.Paused)
                    GameState = GameStates.Paused;
                startBoss = true;
                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);
                //tdPlayer.rectangle.X -= 200;
                //canShoot = true;
            }
        }


        public void StartDungeon(bool loadGame = false)
        {
            healthDrops.Clear();
            soundManager.ClearSounds();
            soundManager.AddSound("BossTheme", true);
            GameState = GameStates.TopDown;
            //tdMap.dMapDims.Clear();

            if(!loadGame)
            {
                SetDungeonNum();
                levelCount = 0;
            }
               

            tdPlayer = new TDPlayer(this, 64, 200, 200, UIManager, Content, bossKillCount);
            UIHelper.DungeonLevels = tdPlayer.bossRoom;
            //Update tutorial text
            tdMap = new TopDownMap();
            //Boss = new Rectangle();
     
            startBoss = false;
            graphics.PreferredBackBufferWidth = 64 * 25; //1600  // pixelBits * col
            graphics.PreferredBackBufferHeight = 64 * 15; //960  // pixelBits * row
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            camera = new Camera(GraphicsDevice.Viewport, new Vector2(0 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)), 0 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))));
            //camera.Zoom = .5f;
            Tiles.Content = Content;

            //Give the builder it's maps
            List<int[,]> maps = new List<int[,]>();
            for (int i = 1; i < 11; i++)
            {
                string filePath = Content.RootDirectory + "/TopDown/Maps/Map" + i + ".txt";
                maps.Add(tdMap.GenerateMap(filePath));
            }
            mapBuilder = new MapBuilder(maps); //Giving maps

            if (levelCount == 0)
            {
                loadGame = false;
                UIManager.CreateTopDownTutorial(new Rectangle(new Point(0 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)), 0 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                      new Point(camera.viewport.Bounds.Width, camera.viewport.Bounds.Width)));

               // UIHelper.SetElementText(UIManager.uiElements["TDTutorialBoxIntro"])
            }
               

            if (tdPlayer.levelInX == 1 && tdPlayer.levelInY == 1)
            {
                GenerateNewMap(true, false, false, loadGame);
            }
            else if (tdPlayer.levelInX > 1 && tdPlayer.levelInY == 1)
            {
                GenerateNewMap(true, false, false, loadGame);
            }
            else if (tdPlayer.levelInY > 1 && tdPlayer.levelInX == 1)
            {
                GenerateNewMap(false, true, false, loadGame);
            }
            else if (tdPlayer.levelInX > 1 && tdPlayer.levelInY > 1)
            {
                GenerateNewMap(false, false, true, loadGame);
            }
            BoundIndexes.Clear();
            BoundIndexes.Add(camera.Position);
            tdPlayer.Load(Content, camera.viewport.Bounds);
            tdPlayer.position = new Vector2((graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)) + (64 * 12), -(graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1)) + (64 * 10));

            UIManager.uiElements["BossRoomCounter"].Visible = true;



            //mapBuilder.Start(75, tdMap, currBounds);

            List<Texture2D> healthbars = new List<Texture2D>();
            for (int i = 0; i <= 10; i++)
            {
                healthbars.Add(Content.Load<Texture2D>(@"SideScroll\HealthBar\PilotHealthBar" + i));
            }

            UIHelper.HealthBar = healthbars;
            UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)tdPlayer.Health);
            UIHelper.ChangeDashIcon(UIManager.uiElements["DashIcon"], tdPlayer.DashIndex());

            prevGameState = GameState;
            SaveGame();
            GameState = GameStates.Paused;
        }

        public void SetDungeonNum()
        {
            DungeonEntrance entrance = dungeons[0];
            //dungeonNum = 1;
            for (int i = 1; i < dungeons.Count; i++)
            {
                if (MathHelper.Distance(entrance.Rectangle.X, ssPlayer.Rectangle.X) >
                    MathHelper.Distance(dungeons[i].Rectangle.X, ssPlayer.Rectangle.X))
                {
                    dungeonNum = i;
                    entrance = dungeons[i];
                }
            }
        }

        public int GetDungeonNum()
        {
            dungeons.Clear();
            DungeonEntrance closest = SideTileMap.DungeonEntrances[0];
            DungeonEntrance furthest = SideTileMap.DungeonEntrances[0];
            while (dungeons.Count < SideTileMap.DungeonEntrances.Count)
            {
                for (int i = 0; i < SideTileMap.DungeonEntrances.Count; i++)
                {
                    //closest = SideTileMap.DungeonEntrances[i];
                    if (!dungeons.Contains(SideTileMap.DungeonEntrances[i]))
                    {
                        closest = SideTileMap.DungeonEntrances[i];
                        furthest = SideTileMap.DungeonEntrances[i];
                        for (int j = 0; j < SideTileMap.DungeonEntrances.Count; j++)
                        {
                            if (!dungeons.Contains(SideTileMap.DungeonEntrances[j]))
                            {
                                if (closest.Rectangle.X > SideTileMap.DungeonEntrances[j].Rectangle.X)
                                {
                                    closest = SideTileMap.DungeonEntrances[j];
                                }
                                //else if (SideTileMap.DungeonEntrances[i].Rectangle.X > SideTileMap.DungeonEntrances[j].Rectangle.X)
                                //{
                                //    closest = SideTileMap.DungeonEntrances[j];
                                //    //furthest = SideTileMap.DungeonEntrances[]
                                //}
                                else if (furthest.Rectangle.X < SideTileMap.DungeonEntrances[j].Rectangle.X)
                                {
                                    furthest = SideTileMap.DungeonEntrances[j];
                                }
                            }

                        }
                    }
                }
                if (dungeons.Contains(closest) == false)
                    dungeons.Add(closest);

            }

            DungeonEntrance entrance = dungeons[0];
            int k = 0;
            int closestIndex = 0;
            for (k = 1; k < dungeons.Count; k++)
            {
                if (MathHelper.Distance(entrance.Rectangle.X, ssPlayer.position.X) >
                    MathHelper.Distance(dungeons[k].Rectangle.X, ssPlayer.position.X))
                {
                    dungeonNum = k;
                    entrance = dungeons[k];
                    closestIndex = k;
                    //return k;
                }
            }

            return closestIndex;
        }

        public void LoadTutorial()
        {
            healthDrops.Clear();
            string filePath = Content.RootDirectory + "/SideScroll/Maps/Map0.txt";
            SideTileMap.WallTiles.Clear();
            SideTileMap.GroundTiles.Clear();
            SideTileMap.PlatformTiles.Clear();
            SideTileMap.playerSpawns.Clear();
            SideTileMap.enemySpawns.Clear();
            SideTileMap.DungeonEntrances.Clear();
            SideTileMap.TopDoorTiles.Clear();
            SideTileMap.BottomDoorTiles.Clear();
            SideTileMap.BackgroundTiles.Clear();
            SideTileMap.LoadMap(filePath);

            //flyingEnemies.Clear();

            flyingBeacons.Clear();
            enemies.Clear();
            enemies.Clear();
            bossKillCount = 0;
            openDoorCount = 0;
            dungeonNum = 0;
            soundManager.ClearSounds();
            soundManager.ChangeVolume(UIManager.MasterVolume, UIManager.EffectVolume, UIManager.MusicVolume);


            fadePos = SideTileMap.playerSpawns[0];
            int k = 1;
            for (int i = 0; i < SideTileMap.enemySpawns.Count; i++)
            {
                //flyingEnemies.Add(new FlyingEnemy(Content, 5, SideTileMap.enemySpawns[i]));
                if (k == 1)
                    enemies.Add(new SSEnemy(Content, Window.ClientBounds, 5, SideTileMap.enemySpawns[i], true));
                else if (k == 2)
                {
                    enemies.Add(new SSEnemy(Content, Window.ClientBounds, 8, SideTileMap.enemySpawns[i], false));
                    k = 0;
                }
                else if (k == 3)
                {
                    //flyingEnemies.Add(new FlyingEnemy(Content, 5, SideTileMap.enemySpawns[i]));
                    k = 0;
                }

                k++;
            }

            for(int i = 0; i < SideTileMap.flyingBeacons.Count;i++)
            {

                flyingBeacons.Add(new FlyingControlBeacon(SideTileMap.flyingBeacons[i].Rectangle, Content));
            }
                //Add final boss here 
                graphics.PreferredBackBufferWidth = 1920;/*(int)(graphics.PreferredBackBufferWidth * 1.5f)*/
            graphics.PreferredBackBufferHeight = 1080;/*(int)(graphics.PreferredBackBufferHeight * 1.5f);*/

            //graphics.HardwareModeSwitch = false;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            //maxShootRate = shootRate;
            //camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
            //camera.Zoom = .5f;
            ssPlayer = new SSPlayer(this, 64);
            ssPlayer.Load(Content, Window.ClientBounds, friction, SideTileMap.playerSpawns[0], UIManager);

            //ssPlayer.Load(Content, Window.ClientBounds, friction, SideTileMap.playerSpawns[0]);
            ssPlayer.Health = 10;
            UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)ssPlayer.Health);
            UIHelper.ChangeDashIcon(UIManager.uiElements["DashIcon"], ssPlayer.DashIndex());
            UIHelper.SetElementVisibility("HealthBar", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("DashIcon", true, UIManager.uiElements);
            ssCamera = new SSCamera(GraphicsDevice.Viewport, new Vector2(SideTileMap.playerSpawns[0].X/* + (64 * 10.5f)*/,
                SideTileMap.playerSpawns[0].Y /*- (64 * 2.5f)*/), (int)SideTileMap.GetWorldDims().X, (int)SideTileMap.GetWorldDims().Y);
            //ssCamera.Zoom = 1f;

            //finalBoss = new FinalBoss(SideTileMap.enemySpawns[0], Content, GraphicsDevice);

            //ssCamera.Update(new Vector2(ssPlayer.playerRect.X, ssPlayer.playerRect.Y), dont);
           // ssCamera.Zoom = .5f;
            // ssCamera.Position = ssPlayer.Position;
            //enemy = new SSEnemy(Content, GraphicsDevice.Viewport.Bounds, 5);

            GameState = GameStates.Tutorial;
            prevGameState = GameState;
            GameState = GameStates.Paused;

            UIHelper.SetElementVisibility("MainMenu", false, UIManager.uiElements);
            UIHelper.SetElementVisibility("Settings", false, UIManager.uiElements);
            UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);

            //if (restart)
            //    LoadGame(false);

        }

        public void LoadFinalBoss()
        {
            healthDrops.Clear();
            string filePath = Content.RootDirectory + "/SideScroll/Maps/Map2.txt";
            SideTileMap.WallTiles.Clear();
            SideTileMap.GroundTiles.Clear();
            SideTileMap.PlatformTiles.Clear();
            SideTileMap.LoadMap(filePath);

            flyingBeacons.Clear();
            enemies.Clear();

            soundManager.ClearSounds();
            soundManager.AddSound("FinalBossTheme", true);

            //Add final boss here 
            graphics.PreferredBackBufferWidth = 1920;/*(int)(graphics.PreferredBackBufferWidth * 1.5f)*/
            graphics.PreferredBackBufferHeight = 1080;/*(int)(graphics.PreferredBackBufferHeight * 1.5f);*/

            //graphics.HardwareModeSwitch = false;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            //maxShootRate = shootRate;
            //camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
            //camera.Zoom = .75f;
            ssPlayer.Load(Content, Window.ClientBounds, friction, SideTileMap.playerSpawns[0], UIManager);

            //ssPlayer.Load(Content, Window.ClientBounds, friction, SideTileMap.playerSpawns[0]);
            ssPlayer.Health = 10;
            UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)ssPlayer.Health);
            UIHelper.ChangeDashIcon(UIManager.uiElements["DashIcon"], ssPlayer.DashIndex());
            UIHelper.SetElementVisibility("HealthBar", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("DashIcon", true, UIManager.uiElements);
            ssCamera = new SSCamera(GraphicsDevice.Viewport, new Vector2(SideTileMap.playerSpawns[0].X + (64 * 18f), 
                SideTileMap.playerSpawns[0].Y - (64 * 15f)), (int)SideTileMap.GetWorldDims().X, (int)SideTileMap.GetWorldDims().Y);
            ssCamera.Zoom = 1.22f;

            finalBoss = new FinalBoss(SideTileMap.enemySpawns[0], Content, GraphicsDevice);
            
            //ssCamera.Update(new Vector2(ssPlayer.playerRect.X, ssPlayer.playerRect.Y), dont);
            //ssCamera.Zoom = 1.25f;
            // ssCamera.Position = ssPlayer.Position;
            //enemy = new SSEnemy(Content, GraphicsDevice.Viewport.Bounds, 5);

            GameState = GameStates.FinalBoss;
            prevGameState = GameState;
            GameState = GameStates.Paused;

            UIHelper.SetElementVisibility("MainMenu", false, UIManager.uiElements);
            UIHelper.SetElementVisibility("Settings", false, UIManager.uiElements);
            UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);

        }

        public void StartNewGame(bool respawn = false, bool loadGame = false)
        {
            SideTileMap.WallTiles.Clear();
            SideTileMap.GroundTiles.Clear();
            SideTileMap.PlatformTiles.Clear();
            SideTileMap.playerSpawns.Clear();
            SideTileMap.enemySpawns.Clear();
            SideTileMap.DungeonEntrances.Clear();
            SideTileMap.TopDoorTiles.Clear();
            SideTileMap.BottomDoorTiles.Clear();
            SideTileMap.BackgroundTiles.Clear();
            


            graphics.PreferredBackBufferWidth = 1920;/*(int)(graphics.PreferredBackBufferWidth * 1.5f)*/
            graphics.PreferredBackBufferHeight = 1080;/*(int)(graphics.PreferredBackBufferHeight * 1.5f);*/

            //graphics.HardwareModeSwitch = false;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            //maxShootRate = shootRate;
            //camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));


            healthDrops.Clear();
            UIHelper.SetElementVisibility("MainMenu", false, UIManager.uiElements);
            UIHelper.SetElementVisibility("Settings", false, UIManager.uiElements);
            UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);
            UIHelper.SetElementVisibility("HealthBar", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("DashIcon", true, UIManager.uiElements);
            //camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth / 2 - (64*3.5f), graphics.PreferredBackBufferHeight / 2));


            this.IsMouseVisible = false;
            Tile.Content = Content;

            string filePath = Content.RootDirectory + "/SideScroll/Maps/Map1.txt";

            //dungeons.Add(SideTileMap.DungeonEntrances[0]);
            //for (int j = 1; j < SideTileMap.DungeonEntrances.Count; j++)
            //{
            //    if (dungeons[0].Rectangle.X > SideTileMap.DungeonEntrances[j].Rectangle.X)
            //    {
            //        dungeons.Clear();
            //        dungeons.Add(SideTileMap.DungeonEntrances[j]);
            //    }
            //}

            //dungeons = SideTileMap.DungeonEntrances;
            //dungeons.Sort();

           

            SideTileMap.LoadMap(filePath);
            flyingBeacons.Clear();
            enemies.Clear();
            int k = 1;
            for(int i = 0; i < SideTileMap.enemySpawns.Count - 1; i++)
            {
                //flyingEnemies.Add(new FlyingEnemy(Content, 5, SideTileMap.enemySpawns[i]));
                if (k == 1)
                    enemies.Add(new SSEnemy(Content, Window.ClientBounds, 5, SideTileMap.enemySpawns[i], true));
                else if (k == 2)
                {
                    enemies.Add(new SSEnemy(Content, Window.ClientBounds, 5, SideTileMap.enemySpawns[i], false));
                }
                else if (k == 3)
                {
                    //flyingEnemies.Add(new FlyingEnemy(Content, 5, SideTileMap.enemySpawns[i]));
                    k = 0;
                }

                k++;

            }


            for (int i = 0; i < SideTileMap.flyingBeacons.Count; i++)
            {
                flyingBeacons.Add(new FlyingControlBeacon(SideTileMap.flyingBeacons[i].Rectangle, Content));
            }
            //SideTileMap.SetFlyingEnemies(flyingEnemies);
            DungeonEntrance closest = SideTileMap.DungeonEntrances[0];
            DungeonEntrance furthest = SideTileMap.DungeonEntrances[0];
            while (dungeons.Count < SideTileMap.DungeonEntrances.Count)
            {
                for (int i = 0; i < SideTileMap.DungeonEntrances.Count; i++)
                {
                    //closest = SideTileMap.DungeonEntrances[i];
                    if (!dungeons.Contains(SideTileMap.DungeonEntrances[i]))
                    {
                        closest = SideTileMap.DungeonEntrances[i];
                        furthest = SideTileMap.DungeonEntrances[i];
                        for (int j = 0; j < SideTileMap.DungeonEntrances.Count; j++)
                        {
                            if (!dungeons.Contains(SideTileMap.DungeonEntrances[j]))
                            {
                                if (closest.Rectangle.X > SideTileMap.DungeonEntrances[j].Rectangle.X)
                                {
                                    closest = SideTileMap.DungeonEntrances[j];
                                }
                                //else if (SideTileMap.DungeonEntrances[i].Rectangle.X > SideTileMap.DungeonEntrances[j].Rectangle.X)
                                //{
                                //    closest = SideTileMap.DungeonEntrances[j];
                                //    //furthest = SideTileMap.DungeonEntrances[]
                                //}
                                else if (furthest.Rectangle.X < SideTileMap.DungeonEntrances[j].Rectangle.X)
                                {
                                    furthest = SideTileMap.DungeonEntrances[j];
                                }
                            }

                        }
                    }
                }
                if (dungeons.Contains(closest) == false)
                    dungeons.Add(closest);

            }
            if (dungeons.Contains(furthest) == false && dungeons.Count != SideTileMap.DungeonEntrances.Count)
                dungeons.Add(furthest);

            //camera.Zoom = 1.35f;

            //if(respawn && prevGameState != GameStates.Tutorial)
            //    ssPlayer.Load(Content, Window.ClientBounds, friction, savedPos);
            //else
            if(!loadGame)
            {
                ssCamera = new SSCamera(GraphicsDevice.Viewport, new Vector2(0, 0),
               (int)SideTileMap.GetWorldDims().X, (int)SideTileMap.GetWorldDims().Y);

                ssPlayer.Load(Content, Window.ClientBounds, friction, SideTileMap.playerSpawns[0], UIManager);

                fadePos = SideTileMap.playerSpawns[0];
            }

           
            
            if (!respawn)
            {
                openDoorCount = 0;
                bossKillCount = 0;
            }
            else if (respawn)
            {
                openDoorCount = 0;
            }

            //ssPlayer.Health = 10;
            UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)ssPlayer.Health);
            UIHelper.ChangeDashIcon(UIManager.uiElements["DashIcon"], ssPlayer.DashIndex());
            UIHelper.SetElementVisibility("HealthBar", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("DashIcon", true, UIManager.uiElements);
           
            //ssCamera.Update(new Vector2(ssPlayer.playerRect.X, ssPlayer.playerRect.Y), dont, fade);
           //ssCamera.Zoom = .5f;
            // ssCamera.Position = ssPlayer.Position;
            //enemy = new SSEnemy(Content, GraphicsDevice.Viewport.Bounds, 5);
            GameState = GameStates.SideScroll;
         
            //ChangeToSideScroll();
            prevGameState = GameState;
            GameState = GameStates.Paused;

         

            UIHelper.SetElementVisibility("TutorialBox", false, UIManager.uiElements);

            

        }

        public void TakeDamage()
        {
            if(healthBar.Count != 0)
            {
                healthBar.Remove(healthBar[healthBar.Count - 1]);
            }
            
        }

        Vector2 CameraPos()
        {
            Vector2 pos = camera.Position;
            tdCameraReached = true;
            //pos = new Vector2(Window.ClientBounds.Width / 2 + (Window.ClientBounds.Width * (levelInX - 1)), graphics.PreferredBackBufferHeight / 2 + (graphics.PreferredBackBufferHeight * (levelInY - 1)));
            if ((graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)) > pos.X))
            {
                if (pos.X + graphics.PreferredBackBufferWidth / pixelBits > graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)))
                {
                    pos.X = graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1));
                }
                else
                {
                    pos.X += graphics.PreferredBackBufferWidth / pixelBits;

                    //if ((graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)) < pos.X))
                    //{
                    //    pos.X = graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1));
                    //}

                    tdCameraReached = false;
                }
              
            }
            else if ((graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)) < pos.X))
            {
                if (pos.X - graphics.PreferredBackBufferWidth / pixelBits < graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)))
                {
                    pos.X = graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1));
                }
                else
                {
                    pos.X -= graphics.PreferredBackBufferWidth / pixelBits;

                    //if ((graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)) < pos.X))
                    //{
                    //    pos.X = graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1));
                    //}

                    tdCameraReached = false;
                }
            }
            if (((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))) < pos.Y))
            {
                if(pos.Y - graphics.PreferredBackBufferHeight / pixelBits < ((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1)))))
                {
                    pos.Y = ((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))));
                }
                else
                {

                    pos.Y -= graphics.PreferredBackBufferHeight / pixelBits;

                    //if (((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))) > pos.Y))
                    //{
                    //    pos.Y = ((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))));
                    //}
                    tdCameraReached = false;
                }

            }
            else if ((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1)) > pos.Y))
            {
                if (pos.Y + graphics.PreferredBackBufferHeight / pixelBits > ((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1)))))
                {
                    pos.Y = ((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))));
                }
                else
                {

                    pos.Y += graphics.PreferredBackBufferHeight / pixelBits;

                    //if (((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))) > pos.Y))
                    //{
                    //    pos.Y = ((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))));
                    //}
                    tdCameraReached = false;
                }
            }

            if (BoundIndexes.Contains(new Vector2((graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1))), 
                (graphics.PreferredBackBufferHeight / 2 + (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))))))
            {

            }
            else
            {
                BoundIndexes.Add(new Vector2((graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1))), 
                    (graphics.PreferredBackBufferHeight / 2 + (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1)))));
            }

           

            Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
               (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
               new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            if (tdCameraReached)
            {
                tdPrevBounds = currBounds;
            }
            for (int i = tdMap.WallTiles.Count - 1; i >= 0; i--)
            {
                if (currBounds.Contains(tdMap.WallTiles[i].Rectangle) == false && tdPrevBounds != Rectangle.Empty && tdPrevBounds.Contains(tdMap.WallTiles[i].Rectangle) == false)
                {
                    tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                }
            }
            for (int i = tdMap.FloorTiles.Count - 1; i >= 0; i--)
            {
                if (currBounds.Contains(tdMap.FloorTiles[i].Rectangle) == false && tdPrevBounds != Rectangle.Empty && tdPrevBounds.Contains(tdMap.FloorTiles[i].Rectangle) == false)
                {
                    tdMap.FloorTiles.Remove(tdMap.FloorTiles[i]);
                }
            }

            return pos;


        }


       
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.BigButton == ButtonState.Pressed /*|| Keyboard.GetState().IsKeyDown(Keys.Escape)*/)
            //    Exit();
            kb = Keyboard.GetState();
            
            moveDir = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
            switch (currScene)
            {
                case Scenes.TitleScreen:
                    Menus(kb);
                    break;
                case Scenes.InGame:

                    switch (GameState)
                    {
                        #region Tutorial
                        case GameStates.Tutorial:
                            soundManager.Update(gameTime);
                            Rectangle worldRect = new Rectangle(ssCamera.CameraBounds.X - (750 / 2), ssCamera.CameraBounds.Y - (950 / 2), ssCamera.CameraBounds.Width + 750, ssCamera.CameraBounds.Height + 950);
                            worldRect = CheckForTiles(worldRect); //Checks if tiles are in range to be added back

                            if (fade || doorTrans)
                            {



                                //ssCamera.Update(new Vector2(SideTileMap.playerSpawns[0].X, SideTileMap.playerSpawns[0].Y - 200), dont, fade);
                                ssCamera.Update(fadePos, dont, fade);
                                ssPlayer.Update(gameTime, Gravity, enemies, this ,true);
                                if (fade)
                                {
                                    //ssCamera.Update(new Vector2(fadePos.X, fadePos.Y - 50), false, fade);
                                    foreach (BottomDoorTile door in SideTileMap.BottomDoorTiles)
                                    {
                                        float num = MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X);
                                        if (ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X) < 64 * 8 && ssPlayer.Y/64 == door.Rectangle.Y/64)
                                        {
                                            dont = true; //dont move camera
                                        }
                                        else if (ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X) > 64 * 8 ||
                                            ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.Y, door.Rectangle.Y) > 64 * 8)
                                        {
                                            if (dont)
                                                dont = false;
                                        }

                                    }
                                    foreach (TopDoorTile door in SideTileMap.TopDoorTiles)
                                    {
                                        float num = MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X);
                                        if (ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X) < 64 * 8 && ssPlayer.Y / 64 == door.Rectangle.Y / 64)
                                        {
                                            dont = true; //dont move camera
                                        }
                                        else if (ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X) > 64 * 8 ||
                                            ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.Y, door.Rectangle.Y) > 64 * 8)
                                        {
                                            if (dont)
                                                dont = false;
                                        }

                                    }
                                    if (!doorTrans && openDoorCount == 0)
                                        ssCamera.Update(new Vector2(SideTileMap.playerSpawns[0].X, SideTileMap.playerSpawns[0].Y - 200), dont, fade);
                                }
                               
                                   
                                if (doorTrans)
                                {
                                    UpdateDoor(topIndex, bottomIndex);
                                    ssCamera.Update(fadePos, false, fade);
                                }
                             

                            }
                            //else if (closeDoor)
                            //{
                            //    ssCamera.Update(fadePos, dont, fade);
                            //    CloseDoor(topIndex, bottomIndex);
                            //}
                            else
                            {
                                SideScrollPhysics(gameTime, kb, worldRect);
                                if (kb.IsKeyDown(Keys.Escape) && prevKB.IsKeyUp(Keys.Escape) ||
                                    GamePad.GetState(PlayerIndex.One).Buttons.BigButton == ButtonState.Pressed && prevButtons.BigButton == ButtonState.Released ||
                                    GamePad.GetState(0).Buttons.Back == ButtonState.Pressed && prevButtons.Back == ButtonState.Released)
                                {
                                    prevGameState = GameState;
                                    GameState = GameStates.InGamePause;
                                    UIManager.CreatePauseMenu(ssCamera.ViewRect);
                                    UIManager.uiElements["HealthBar"].Visible = false;
                                    UIManager.uiElements["DashIcon"].Visible = false;
                                    //mainMenuPos = new Vector2(0, 0);
                                    //soundManager.ClearSounds();
                                }

                                if(ssPlayer.playerRect.Intersects(UIHelper.GetElementBGRect(UIManager.uiElements["DashIcon"])))
                                {
                                    UIManager.uiElements["DashIcon"].Visible = false;
                                }
                                else
                                    UIManager.uiElements["DashIcon"].Visible = true;
                            }
                            break;
                        #endregion
                        #region SideScroll
                        case GameStates.SideScroll: //Default
                            soundManager.Update(gameTime);
                            worldRect = new Rectangle(ssCamera.CameraBounds.X - (750 / 2), ssCamera.CameraBounds.Y - (950 / 2), ssCamera.CameraBounds.Width + 750, ssCamera.CameraBounds.Height + 950);
                            worldRect = CheckForTiles(worldRect); //Checks if tiles are in range to be added back

                            if (fade || doorTrans)
                            {
                                ssCamera.Update(fadePos, dont, fade);
                                UIManager.uiElements["DashIcon"].Visible = false;
                                UIManager.uiElements["HealthBar"].Visible = false;
                                ssPlayer.Update(gameTime, Gravity, enemies, this,true);
                                //ChangeToSideScroll();


                 


                                if (fade)
                                {
                                    foreach (BottomDoorTile door in SideTileMap.BottomDoorTiles)
                                    {
                                        float num = MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X);
                                       
                                    
                                        if (ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X) < 64 * 8/* && ssPlayer.Y/64 == door.Rectangle.Y/64*/)
                                        {
                                            dont = true; //dont move camera
                                        }
                                        else if (ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X) > 64 * 8 ||
                                            ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.Y, door.Rectangle.Y) > 64 * 8)
                                        {
                                            if (dont)
                                                dont = false;
                                        }

                                    }
                                   
                                }
                                if (updateDoor)
                                {
                                    UpdateDoor(topIndex, bottomIndex);
                                }

                            }
                            else if (updateDoor)
                            {
                                UpdateDoor(topIndex, bottomIndex);
                            }
                            else if (closeDoor)
                            {
                                if(reloadCount == 0)
                                {
                                    
                                    reloadCount++;
                                    closeDoor = false;
                                }
                                else
                                    CloseDoor(topIndex, bottomIndex);

                            }  
                            else
                            {
                                SideScrollPhysics(gameTime, kb, worldRect);
                                if (kb.IsKeyDown(Keys.J) && prevKB.IsKeyUp(Keys.J))
                                {
                                    GameState = GameStates.TopDown;
                                    StartDungeon();

                                }
                                if(kb.IsKeyDown(Keys.Escape) && prevKB.IsKeyUp(Keys.Escape) || 
                                    GamePad.GetState(PlayerIndex.One).Buttons.BigButton == ButtonState.Pressed && prevButtons.BigButton == ButtonState.Released
                                    || GamePad.GetState(0).Buttons.Back == ButtonState.Pressed && prevButtons.Back == ButtonState.Released)
                                {
                                    prevGameState = GameState;
                                    GameState = GameStates.InGamePause;
                                    UIManager.CreatePauseMenu(ssCamera.ViewRect);
                                    UIManager.uiElements["HealthBar"].Visible = false;
                                    UIManager.uiElements["DashIcon"].Visible = false;
                                    //soundManager.ClearSounds();

                                }
                                if (ssPlayer.playerRect.Intersects(UIHelper.GetElementBGRect(UIManager.uiElements["DashIcon"])))
                                {
                                    UIManager.uiElements["DashIcon"].Visible = false;
                                }
                                else
                                    UIManager.uiElements["DashIcon"].Visible = true;
                            }

                      

                            for(int i = flyingBeacons.Count - 1; i >= 0; i--)
                            {
                               
                                flyingBeacons[i].Update(gameTime, Gravity, ssPlayer, ssCamera.CameraBounds);

                                if (flyingBeacons[i].controlledEnemies.Count == 0)
                                {
                                    SideTileMap.flyingBeacons[i].active = false;
                                    flyingBeacons.RemoveAt(i);
                                   
                                }
                                   
                            }

                            //SSCamera.Move(ssPlayer.Position);
                            break;
                        #endregion
                        #region TopDown
                        case GameStates.TopDown:
                            if(fade)
                            {
                                camera.Update(CameraPos());
                            }
                            else
                            {
                                Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                                       (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                                       new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));

                                //if (killedEnemies < numEnemies && tdMap.setBarrier == false && tdCameraReached)
                                //{
                                //    tdMap.SetBarrier();
                                //    //tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                                //    //   tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);
                                //}
                                //else if(tdMap.setBarrier && killedEnemies == numEnemies)
                                //{
                                //    tdMap.RemoveBarrier();
                                //    //tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                                //    //   tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);
                                //}


                                if (kb.IsKeyDown(Keys.Escape) && prevKB.IsKeyUp(Keys.Escape) && !startBoss ||
                                       GamePad.GetState(PlayerIndex.One).Buttons.BigButton == ButtonState.Pressed && prevButtons.BigButton == ButtonState.Released && !startBoss ||
                                    GamePad.GetState(0).Buttons.Back == ButtonState.Pressed && prevButtons.Back == ButtonState.Released && !startBoss)
                                {
                                    prevGameState = GameState;
                                    GameState = GameStates.InGamePause;
                                    UIManager.CreatePauseMenu(new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                                       (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                                       new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)));
                                    UIManager.uiElements["HealthBar"].Visible = false;
                                    UIManager.uiElements["DashIcon"].Visible = false;
                                    //soundManager.ClearSounds();
                                }

                                if (tdPlayer.changeLevel)
                                {
                                    tdEnemies.Clear();
                                    tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                                        tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);

                                    currMap = tdPlayer.DiagLevels.dLevels[tdPlayer.DiagLevels.diagIndex];
                                    currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                                        (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                                        new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
                                    if (!startBoss)
                                    {//Clear map and remove nonused
                                        for (int i = tdMap.EnemySpawns.Count - 1; i >= 0; i--)
                                        {
                                            if (currBounds.Contains(tdMap.EnemySpawns[i]))
                                            {
                                                if (tdEnemies.Contains(new TDEnemy(Content, tdMap.EnemySpawns[i], tdMap, currMap, GraphicsDevice, bossKillCount)) == false)
                                                    tdEnemies.Add(new TDEnemy(Content, tdMap.EnemySpawns[i], tdMap, currMap, GraphicsDevice, bossKillCount));
                                            }
                                            else
                                            {
                                                tdMap.EnemySpawns.Remove(tdMap.EnemySpawns[i]);
                                            }


                                        }

                                    }
                                    numEnemies = tdMap.EnemySpawns.Count;
                                    killedEnemies = 0;

                                    tdPlayer.changeLevel = false;

                                }
                                if (tdPlayer.Health <= 0)
                                {
                                    LoadGame();
                                }

                                if(killedEnemies == numEnemies && levelCount != tdPlayer.bossRoom + 1 && tdMap.setBarrier)
                                {
                                    tdMap.RemoveBarrier();
                                    tdEnemies.Clear();
                                    tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                                        tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);
                                    

                                   
                                }
                                else if(levelCount == tdPlayer.bossRoom + 1 && tdMap.publicRefresh == false)
                                {
                                    float distFromBottom = MathHelper.Distance(tdPlayer.rectangle.Center.Y, currBounds.Bottom);
                                    float distFromTop = MathHelper.Distance(tdPlayer.rectangle.Center.Y, currBounds.Top);
                                    float distFromRight = MathHelper.Distance(tdPlayer.rectangle.Center.X, currBounds.Right);
                                    float distFromLeft = MathHelper.Distance(tdPlayer.rectangle.Center.X, currBounds.Left);

                                    if(distFromRight < distFromLeft && distFromRight < distFromBottom && distFromRight < distFromTop)//Right
                                    {
                                        tdMap.SetBarrierValues(currBounds, true, false, false, false);
                                    }
                                    else if(distFromLeft < distFromRight && distFromLeft < distFromBottom && distFromLeft < distFromTop)//Top
                                    {
                                        tdMap.SetBarrierValues(currBounds, false, true, false, false);
                                    }
                                    else if(distFromBottom < distFromTop && distFromBottom < distFromLeft && distFromBottom < distFromRight)//Bottom
                                    {
                                        tdMap.SetBarrierValues(currBounds, false, false, false, true);
                                    }
                                    else if(distFromTop < distFromBottom && distFromTop < distFromLeft && distFromTop < distFromRight)//Top
                                    {
                                        tdMap.SetBarrierValues(currBounds, false, false, true, false);
                                    }

                                    tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                                     tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points, dungeonNum);
                                }

                                UIHelper.UpdatePlayerUI(UIManager.uiElements["HealthBar"], new Rectangle(new Point((int)(camera.Position.X - GraphicsDevice.Viewport.Width / 2) + 20,
                                       (int)(camera.Position.Y - GraphicsDevice.Viewport.Height / 2) + 20), new Point(0, 0)));
                                UIHelper.UpdatePlayerUI(UIManager.uiElements["DashIcon"], 
                                    new Rectangle(new Point((int)(camera.Position.X + GraphicsDevice.Viewport.Width / 2) - 145,
                                       (int)(camera.Position.Y + GraphicsDevice.Viewport.Height / 2) -130), new Point(0, 0)));
                                

                                //camera.Update(new Vector2(camera.X, camera.Y));
                                tdPlayer.Update(gameTime, tdMap, shotGunBoss, tdEnemies);
                                camera.Update(CameraPos());
                                if (kb.IsKeyDown(Keys.J) && prevKB.IsKeyUp(Keys.J) || tdPlayer.rectangle.Intersects(LeaveDungeon))//Switching back to sidescroll
                                {
                                    ChangeToSideScroll();
                                    //Camera position not updated
                                }
                                UIHelper.ChangeDashIcon(UIManager.uiElements["DashIcon"], tdPlayer.DashIndex());

                                if (tdEnemies.Count != 0)
                                {
                                    bool hardBreak = false;
                                    for (int j = tdEnemies.Count - 1; j >= 0; j--)
                                    {
                                        if (tdPlayer.bullets.Count != 0)
                                        {
                                            for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                            {
                                                if (tdPlayer.bullets[i].rect.Intersects(tdEnemies[j].Rectangle))
                                                {
                                                    tdEnemies[j].Health -= tdPlayer.bulletDmg / (tdEnemies[j].dmgResistance * (bossKillCount + 1));
                                                    //if(tdEnemies[j].Health <= 0)
                                                    //{
                                                    //    tdEnemies.RemoveAt(j);
                                                    //    hardBreak = true;
                                                    //}
                                                    tdPlayer.bullets[i].delete = true;
                                                    break;

                                                }
                                            }
                                            if (hardBreak)
                                                break;
                                        }

                                        for(int i = tdPlayer.bombs.Count - 1; i>= 0; i--)
                                        {
                                            if(tdPlayer.bombs[i].rect.Intersects(tdEnemies[j].Rectangle))
                                            {
                                                tdEnemies[j].Health -= tdPlayer.bulletDmg / (tdEnemies[j].dmgResistance * (bossKillCount + 1));

                                                tdPlayer.bombs[i].delete = true;
                                            }
                                        }

                                        for(int i = tdPlayer.explosions.Count - 1; i >= 0; i--)
                                        {
                                            if(tdPlayer.explosions[i].rect.Intersects(tdEnemies[j].Rectangle))
                                                tdEnemies[j].Health -= tdPlayer.bulletDmg / (tdEnemies[j].dmgResistance * (bossKillCount + 1));

                                        }
                                    }
                                }

                                for (int i = tdEnemies.Count - 1; i >= 0; i--)
                                {
                                    for(int j = i - 1; j >= 0; j--)
                                    {
                                        if(tdEnemies[i].Rectangle.Intersects(tdEnemies[j].Rectangle))
                                        {
                                            while (tdEnemies[i].targetOffset == tdEnemies[j].targetOffset)
                                                tdEnemies[i].SetTargetPersonality();
                                            if(MathHelper.Distance(tdEnemies[i].Rectangle.Center.X, tdEnemies[j].Rectangle.Center.X) < 48)
                                            {
                                                tdEnemies[j].moveSpeed /= 2;
                                            }
                                          
                                        }
                                        else
                                        {
                                            tdEnemies[j].moveSpeed = tdEnemies[i].iMoveSpeed;
                                        }
                                    }
                                    tdEnemies[i].Upate(gameTime, tdPlayer, tdMap, currBounds);
                                    if (tdEnemies[i].Health <= 0)
                                    {
                                        killedEnemies++;
                                        if (rand.Next(0, 101) < dropRateTD)
                                        {
                                         
                                            healthDrops.Add(new HealthDrop(tdEnemies[i].Rectangle));

                                        }
                                        tdEnemies.RemoveAt(i);
                                    }
                                }

                                for (int i = healthDrops.Count - 1; i >= 0; i--)
                                {
                                    if (healthDrops[i].rect.Intersects(tdPlayer.rectangle))
                                    {
                                        tdPlayer.Health += healAmount;
                                        healthDrops.RemoveAt(i);

                                    }
                                }
                                for(int i = bossHealthDrops.Count - 1; i>= 0; i--)
                                {
                                    if (bossHealthDrops[i].rect.Intersects(tdPlayer.rectangle))
                                    {
                                        tdPlayer.Health += healAmount;
                                        bossHealthDrops.RemoveAt(i);

                                    }
                                }

                                //if (tdPlayer.bullets.Count != 0)
                                //{
                                //    for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                //    {
                                //        tdPlayer.bullets[i].Update(gameTime);
                                //    }
                                //}



                                foreach (WallTiles tile in tdMap.WallTiles)
                                {
                                    if (tdPlayer.bullets.Count != 0)
                                    {
                                        for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                        {
                                            if (tdPlayer.bullets[i].rect.Intersects(tile.Rectangle))
                                            {
                                                tdPlayer.bullets.RemoveAt(i);
                                            }
                                        }
                                    }
                                    for(int i = tdPlayer.bombs.Count -1; i >= 0; i--)
                                    {
                                        if (tdPlayer.bombs[i].rect.Intersects(tile.Rectangle))
                                            tdPlayer.bombs[i].delete = true;
                                    }
                                }

                                UIManager.UpdateTextBlock("BossRoomCounter", currBounds, levelCount);

                                //if(!canShoot)
                                //{
                                //    shootRate -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                                //    if(shootRate < 0)
                                //    {
                                //        shootRate = maxShootRate;
                                //        canShoot = true;
                                //    }
                                //}

                                //if(canShoot)
                                //{
                                //    if(tdPlayer.position.X < Boss.X + Boss.Width/2)
                                //    {
                                //        bossBullets.Add(new Bullet(new Vector2(Boss.X + Boss.Width / 2, Boss.Y + Boss.Height / 2), -bulletSpeed, new Vector2(-maxBulletSpeed, maxBulletSpeed), Content, true, 64 * 6));
                                //    }
                                //    if(tdPlayer.position.X > Boss.X + Boss.Width/2)
                                //    {
                                //        bossBullets.Add(new Bullet(new Vector2(Boss.X + Boss.Width / 2, Boss.Y + Boss.Height / 2), bulletSpeed, new Vector2(maxBulletSpeed, maxBulletSpeed), Content, true, 64 * 6));
                                //    }
                                //    if(tdPlayer.position.Y < Boss.Y + Boss.Height/2)
                                //    {
                                //        bossBullets.Add(new Bullet(new Vector2(Boss.X + Boss.Width / 2, Boss.Y + Boss.Height / 2), -bulletSpeed, new Vector2(maxBulletSpeed, -maxBulletSpeed), Content, false, 64 * 6));
                                //    }
                                //    if (tdPlayer.position.Y > Boss.Y + Boss.Height / 2)
                                //    {
                                //        bossBullets.Add(new Bullet(new Vector2(Boss.X + Boss.Width / 2, Boss.Y + Boss.Height / 2), bulletSpeed, new Vector2(maxBulletSpeed, maxBulletSpeed), Content, false, 64 * 6));
                                //    }

                                //    canShoot = false;
                                //}

                                //foreach(Bullet bullet in bossBullets)
                                //{
                                //    bullet.Update();
                                //}


                                //for(int i = bossBullets.Count - 1; i >= 0; i--)
                                //{
                                //    if(bossBullets[i].rect.Intersects(tdPlayer.rectangle))
                                //    {
                                //        tdPlayer.Health -= bulletDmg;

                                //        if(tdPlayer.Health <= 0)
                                //        {
                                //            StartDungeon();
                                //        }

                                //        bossBullets.RemoveAt(i);
                                //    }
                                //}
                                if (startBoss)
                                {
                                    switch(dungeonNum)
                                    {
                                        case 0:
                                            #region ShotGunBoss
                                            shotGunBoss.Update(gameTime, tdPlayer, tdMap);
                                            if (shotGunBoss.Health <= 0 && LeaveDungeon == Rectangle.Empty)
                                            {
                                                LeaveDungeon = new Rectangle(camera.viewport.Bounds.X + camera.viewport.Bounds.Width / 2, 
                                                    camera.viewport.Bounds.Y + camera.viewport.Bounds.Height / 2, 64, 64);
                                                ChangeToSideScroll();
                                                ssPlayer.Health = 10;
                                                bossKillCount++;
                                                levelCount = 0;
                                            }
                                         
                                            for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                            {
                                                if (shotGunBoss != null && tdPlayer.bullets[i].rect.Intersects(shotGunBoss.worldRect))
                                                {
                                                    shotGunBoss.Health -= tdPlayer.bulletDmg / shotGunBoss.dmgResistance;
                                                    tdPlayer.bullets.RemoveAt(i);
                                                }
                                            }
                                         
                                            #endregion
                                            break;
                                        case 1:

                                            slamBoss.Update(gameTime, tdPlayer, tdMap);
                                            if (slamBoss.health <= 0)
                                            {
                                                
                                                LeaveDungeon = new Rectangle(camera.viewport.Bounds.X + camera.viewport.Bounds.Width / 2,
                                                    camera.viewport.Bounds.Y + camera.viewport.Bounds.Height / 2, 64, 64);
                                                slamBoss.bossRect.X = 0;
                                                ChangeToSideScroll();
                                                ssPlayer.Health = 10;
                                                bossKillCount++;
                                                levelCount = 0;
                                            }
                                            

                                            break;
                                        case 2:
                                            bombBoss.Update(gameTime, tdMap, tdPlayer);

                                            if (bombBoss.bossHealth <= 0)
                                            {
                                                LeaveDungeon = new Rectangle(camera.viewport.Bounds.X + camera.viewport.Bounds.Width / 2,
                                                    camera.viewport.Bounds.Y + camera.viewport.Bounds.Height / 2, 64, 64);
                                                bombBoss.bossRect.X = 0;
                                                ChangeToSideScroll();
                                                ssPlayer.Health = 10;
                                                bossKillCount++;
                                                levelCount = 0;
                                            }



                                            break;
                                        case 3:
                                            laserBoss.Update(gameTime, tdPlayer, tdMap);

                                            int deadBosses = 0;
                                            for(int i = laserBoss.bossRects.Count - 1; i >=0; i--)
                                            {
                                                if(laserBoss.bossRects[i].health <= 0)
                                                {
                                                    laserBoss.bossRects.RemoveAt(i);
                                                    deadBosses++;
                                                }
                                            }

                                            if(laserBoss.bossRects.Count == 0)
                                            {
                                                LeaveDungeon = new Rectangle(camera.viewport.Bounds.X + camera.viewport.Bounds.Width / 2,
                                                camera.viewport.Bounds.Y + camera.viewport.Bounds.Height / 2, 64, 64);
                                                laserBoss.bossRects.Clear();
                                                //LoadFinalBoss();
                                                ChangeToSideScroll();
                                                ssPlayer.Health = 10;
                                                bossKillCount++;
                                                levelCount = 0;
                                            }
                                                
                                            break;
                                    }
                                   

                                    #region LaserBoss
                                  
                                    #endregion
                                    #region BombBoss
                                    //bombBoss.Update(gameTime, tdMap, tdPlayer);
                                    #endregion
                                    #region SlamBoss
                                    //slamBoss.Update(gameTime, tdPlayer, tdMap);
                                    #endregion
                                    for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                    {
                                        if (shotGunBoss != null && tdPlayer.bullets[i].rect.Intersects(shotGunBoss.worldRect))
                                        {
                                            shotGunBoss.Health -= tdPlayer.bulletDmg / shotGunBoss.dmgResistance;
                                            tdPlayer.bullets.RemoveAt(i);
                                        }
                                    }
                                }




                                //if(levelCount >= tdPlayer.bossRoom)
                                //{
                                //    for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                //    {
                                //        if (tdPlayer.bullets[i].rect.Intersects(Boss))
                                //        {
                                //             bossHealth -= tdPlayer.bulletDmg;
                                //            if (bossHealth <= 0)
                                //            {
                                //            {
                                //                Boss = new Rectangle();
                                //                break;
                                //            }
                                //            tdPlayer.bullets.RemoveAt(i);
                                //        }
                                //    }
                                //}
                                UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)tdPlayer.Health);
                            }
                            
                            break;
                        #endregion
                        #region Paused
                        case GameStates.Paused:
                            
                            if(prevGameState == GameStates.SideScroll || prevGameState == GameStates.Tutorial)
                            {
                                if (fadePos == Vector2.Zero )
                                {
                                    fadePos = new Vector2(SideTileMap.playerSpawns[0].X, SideTileMap.playerSpawns[0].Y);
                                }
                                ssCamera.Update(fadePos, dont, fade, true);
                                //ssPlayer.Update(gameTime, -ssPlayer.velocity, enemies);
                                if (ssCamera.reached == false)
                                {
                                    black = true;
                                }
                                else if(ssCamera.reached)
                                {
                                    GameState = prevGameState;
                                    fade = true;

                                    SaveGame();


                                    Vector2 tempPos = ssPlayer.position;

                                    if(dungeonNum >= 2 && bossKillCount >= 3)
                                        ssPlayer.position.X += 64 * 20;
                                    else
                                        ssPlayer.position.X += 64 * 6;

                                    if (dungeonNum < bossKillCount && soundManager.currEffectName == "BossTheme" /*|| soundManager.currEffectName.Contains(GetDun) == false*/) //Just came from a dungeon on first load from a LoadGame
                                    {
                                        soundManager.ClearSounds();
                                        soundManager.AddSound("Level" + dungeonNum + "Side", true);
                                    }
                                    else if(soundManager.currEffectName.Contains("Level") == false) //First load in sidescroll
                                    {
                                        soundManager.ClearSounds();
                                        soundManager.AddSound("Level" + GetDungeonNum() + "Side", true);
                                    }
                                    ssPlayer.position = tempPos;
                                    soundManager.PlaySound();
                                 
                                    //SaveGame();
                                }
                            }
                            else if(prevGameState == GameStates.TopDown)
                            {
                                //camera.Update(new Vector2(camera.X, camera.Y));
                                //tdPlayer.Update(gameTime, tdMap, shotGunBoss);
                                camera.Update(CameraPos());
                                if(tdCameraReached == false)
                                {
                                    black = true;
                                }
                                else if(tdCameraReached)
                                {
                                    GameState = prevGameState;
                                    fade = true;
                                    soundManager.PlaySound();
                                    //SaveGame();
                                }
                            }
                            else if(prevGameState == GameStates.FinalBoss)
                            {
                                ssCamera.Update(new Vector2(SideTileMap.playerSpawns[0].X + (64 * 10.5f), SideTileMap.playerSpawns[0].Y - (64 * 5.25f)), dont, fade);
                                //ssPlayer.Update(gameTime, -ssPlayer.velocity, enemies);
                                if (ssCamera.reached == false)
                                {
                                    black = true;
                                }
                                else if (ssCamera.reached)
                                {
                                    GameState = prevGameState;
                                    fade = true;
                                    soundManager.PlaySound();
                                    SaveGame();
                                }
                            }
                            break;
                        case GameStates.FinalBoss:
                            if(fade)
                            {
                                ssPlayer.Update(gameTime, Gravity, enemies, this,true);
                                fade = false;
                                // UIHelper.UpdateHealthBar(UIManager.uiElements["HealthBar"], new Rectangle(new Point(ssCamera.CameraBounds.X + 20,
                                //  ssCamera.CameraBounds.Y + 20), new Point(0, 0)));
                                //ssCamera.Update(ssCamera.Position, true);
                            }
                            else
                            {
                                SideTileMap.RepeatBG.Clear();
                                UIHelper.UpdatePlayerUI(UIManager.uiElements["HealthBar"], new Rectangle(new Point(40,
                                60), new Point(0, 0)));
                                UIHelper.UpdatePlayerUI(UIManager.uiElements["DashIcon"], 
                                    new Rectangle(new Point(UIHelper.GetElementBGRect(UIManager.uiElements["HealthBar"]).X
                                    , UIHelper.GetElementBGRect(UIManager.uiElements["HealthBar"]).Bottom + 60), new Point(0, 0)));

                                UIHelper.ChangeDashIcon(UIManager.uiElements["DashIcon"], ssPlayer.DashIndex());

                                ssPlayer.Update(gameTime, Gravity, enemies, this);
                                ssCamera.Update(new Vector2(SideTileMap.playerSpawns[0].X + (64 * 10.5f), SideTileMap.playerSpawns[0].Y - (64 * 5.25f)), false, fade);


                                if(ssPlayer.damaged)
                                    UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)ssPlayer.Health);
                                if (ssPlayer.Health <= 0)
                                    LoadFinalBoss();
                                #region Collisions
                                ssPlayer.blockBottom = false;

                                //ssPlayer.isColliding = false;

                                foreach (GroundTile tile in SideTileMap.GroundTiles)
                                {
                                    ssPlayer.Collision(tile.Rectangle);
                                    if (ssPlayer.isCollidingRight == true &&
                                    ssPlayer.isCollidingLeft == true)
                                    {
                                        ssPlayer.isCollidingRight = false;
                                        ssPlayer.isCollidingLeft = false;
                                    }

                                    bool trueBreak = false;
                                    foreach (Rectangle breakables in ssPlayer.breakTiles)
                                    {
                                        if (breakables == tile.Rectangle)
                                        {
                                            destroyedGround.Add(tile);
                                            SideTileMap.GroundTiles.Remove(tile);
                                            ssPlayer.breakTiles.Remove(breakables);
                                            respawnDelay = respawnDelaySet;
                                            trueBreak = true;
                                            break;
                                        }
                                    }
                                    if (trueBreak)
                                        break;
                                }

                                foreach (PlatformTile tile1 in SideTileMap.PlatformTiles)
                                {
                                    ssPlayer.Collision(tile1.Rectangle);
                                    bool trueBreak = false;
                                    foreach (Rectangle breakables in ssPlayer.breakTiles)
                                    {
                                        if (breakables == tile1.Rectangle)
                                        {
                                            destroyedPlatfroms.Add(tile1);
                                            SideTileMap.PlatformTiles.Remove(tile1);
                                            ssPlayer.breakTiles.Remove(breakables);
                                            respawnDelay = respawnDelaySet;
                                            trueBreak = true;
                                            break;
                                        }
                                    }
                                    if (trueBreak)
                                        break;
                                }
                                foreach (WallTile tile2 in SideTileMap.WallTiles)
                                {
                                    ssPlayer.Collision(tile2.Rectangle);
                                }
                                foreach(TopDoorTile tile3 in SideTileMap.TopDoorTiles)
                                {
                                    ssPlayer.Collision(tile3.Rectangle);
                                }
                                foreach(BottomDoorTile tile4 in SideTileMap.BottomDoorTiles)
                                {
                                    ssPlayer.Collision(tile4.Rectangle);
                                }

                                if (ssPlayer.blockBottom == false && ssPlayer.playerState != SSPlayer.PlayerStates.Jumping)
                                {
                                    ssPlayer.coyoteTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                                    if (ssPlayer.coyoteTime <= 0)
                                    {
                                        ssPlayer.isFalling = true;
                                        ssPlayer.coyoteTime = ssPlayer.coyoteTimeMax;
                                        ssPlayer.coyote = false;
                                    }


                                }
                                #endregion
                            }
                            

                            finalBoss.Update(gameTime, ssPlayer);

                            if(finalBoss.Health <= 0)
                            {
                                GameState = GameStates.EndGame;
                                UIManager.CreatePauseMenu(ssCamera.ViewRect);
                      
                                UIManager.CreateEndGameUI(ssCamera.ViewRect);
                                UIHelper.SetElementVisibility("EndGame", true, UIManager.uiElements);
                                camera = new Camera(GraphicsDevice.Viewport, new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2));
                                PauseMenuButtonIndex = 0;
                                soundManager.ClearSounds();
                            }

                            break;
                        #endregion
                        #region InGamePause
                        case GameStates.InGamePause:

                            

                            if(prevGameState == GameStates.SideScroll || prevGameState == GameStates.Tutorial)
                            {

                                //UIManager.uiElements["PauseMainMenuBtn"].Visible = true;
                                //UIManager.uiElements["PauseMenuReturn"].Visible = true;
                                //UIManager.CreatePauseMenu(ssCamera.ViewRect);
                                UIManager.UpdatePauseUI(ssCamera.ViewRect);
                                UIManager.uiElements["DashIcon"].Visible = false;
                                UIHelper.SetElementVisibility("TutorialBox", false, UIManager.uiElements);
                                UIHelper.SetElementVisibility("InteractBox", false, UIManager.uiElements);
                                ssCamera.Update(new Vector2(ssPlayer.Rectangle.X, ssPlayer.Rectangle.Y), dont, fade);

                               
                            }
                            else if(prevGameState == GameStates.TopDown)
                            {
                                //UIManager.CreatePauseMenu(new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                                       //(0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                                       //new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)));
                                UIManager.UpdatePauseUI(new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                                       (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                                       new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)));
                                camera.Update(CameraPos());

                            }

                            if (kb.IsKeyDown(Keys.Escape) && prevKB.IsKeyUp(Keys.Escape) || 
                                GamePad.GetState(0).Buttons.BigButton == ButtonState.Pressed && prevButtons.BigButton == ButtonState.Released ||
                                GamePad.GetState(0).Buttons.B == ButtonState.Pressed && prevGamePad.Buttons.B == ButtonState.Released)
                            {
                                GameState = prevGameState;
                                UIManager.RemovePauseMenu();
                                UIManager.uiElements["HealthBar"].Visible = true;
                                UIManager.uiElements["DashIcon"].Visible = true;

                                if(GameState == GameStates.Tutorial)
                                    UIHelper.SetElementVisibility("TutorialBox", true, UIManager.uiElements);
                                else if(GameState == GameStates.SideScroll)
                                    UIHelper.SetElementVisibility("InteractBox", true, UIManager.uiElements);
                            }

                            if (kb.IsKeyDown(Keys.Up) && prevKB.IsKeyUp(Keys.Up) || GamePad.GetState(0).ThumbSticks.Left.Y < -.9 && prevGamePad.ThumbSticks.Left.Y == 0 ||
                                GamePad.GetState(0).DPad.Up == ButtonState.Pressed && prevGamePad.DPad.Up == ButtonState.Released)
                            {
                                PauseMenuButtonIndex--;
                                if(PauseMenuButtonIndex < 0)
                                    PauseMenuButtonIndex = 0;
                            }
                            if (kb.IsKeyDown(Keys.Down) && prevKB.IsKeyUp(Keys.Down)|| GamePad.GetState(0).ThumbSticks.Left.Y > .9 && prevGamePad.ThumbSticks.Left.Y == 0 ||
                                GamePad.GetState(0).DPad.Down == ButtonState.Pressed && prevGamePad.DPad.Down == ButtonState.Released)
                            {
                                PauseMenuButtonIndex++;
                                if (PauseMenuButtonIndex > 2)
                                    PauseMenuButtonIndex = 2;
                            }
                            prevGamePad = GamePad.GetState(0);

                            if (kb.IsKeyDown(Keys.Enter) ||
                                GamePad.GetState(0).Buttons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released)
                            {
                                UIManager.RemovePauseMenu();

                                if (PauseMenuButtonIndex == 1)
                                {
                                    currScene = Scenes.TitleScreen;
                                    MenuState = MenuStates.MainMenu;

                                    graphics.PreferredBackBufferWidth = 768;/*(int)(graphics.PreferredBackBufferWidth * 1.5f)*/
                                    graphics.PreferredBackBufferHeight = 432;/*(int)(graphics.PreferredBackBufferHeight * 1.5f);*/
                                    graphics.ApplyChanges();

                                    mainMenuPos = new Vector2(385, 210);
                                    //graphics.HardwareModeSwitch = false;
                                    //graphics.IsFullScreen = true;

                                    //maxShootRate = shootRate;
                                    camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
                                    camera.Zoom = 1f;
                                    soundManager.ClearSounds();
                                    UIManager.uiElements["HealthBar"].Visible = false;
                                    UIManager.uiElements["DashIcon"].Visible = false;
                                    UIHelper.SetButtonState("MainMenuSetting", false, UIManager.uiElements);

                                }
                                else if (PauseMenuButtonIndex == 0)
                                {
                                    GameState = prevGameState;
                                    UIManager.uiElements["HealthBar"].Visible = true;
                                    UIManager.uiElements["DashIcon"].Visible = true;

                                    if (GameState == GameStates.Tutorial)
                                        UIHelper.SetElementVisibility("TutorialBox", true, UIManager.uiElements);
                                    //UIManager.RemovePauseMenu();
                                }
                                else if(PauseMenuButtonIndex == 2)
                                {
                                    Exit();
                                }

                                PauseMenuButtonIndex = 0;
                                //UIManager.RemovePauseMenu();
                            }

                            int borderWidth = 8; //  is /2
                            switch(PauseMenuButtonIndex)
                            {
                                case 1:
                                    selectedBox = new Rectangle(UIHelper.GetRectangle(UIManager.uiElements["PauseMenuReturn"]).X - borderWidth / 2, UIHelper.GetRectangle(UIManager.uiElements["PauseMenuReturn"]).Y - borderWidth / 2,
                                        UIHelper.GetRectangle(UIManager.uiElements["PauseMenuReturn"]).Width + borderWidth, UIHelper.GetRectangle(UIManager.uiElements["PauseMenuReturn"]).Height + borderWidth);
                                    break;
                                case 0:
                                    selectedBox = new Rectangle(UIHelper.GetRectangle(UIManager.uiElements["PauseMainMenuBtn"]).X - borderWidth / 2, UIHelper.GetRectangle(UIManager.uiElements["PauseMainMenuBtn"]).Y - borderWidth / 2,
                                        UIHelper.GetRectangle(UIManager.uiElements["PauseMainMenuBtn"]).Width + borderWidth, UIHelper.GetRectangle(UIManager.uiElements["PauseMainMenuBtn"]).Height + borderWidth);
                                    break;
                                case 2:
                                    selectedBox = new Rectangle(UIHelper.GetRectangle(UIManager.uiElements["PauseMenuQuit"]).X - borderWidth / 2, UIHelper.GetRectangle(UIManager.uiElements["PauseMenuQuit"]).Y - borderWidth / 2,
                                        UIHelper.GetRectangle(UIManager.uiElements["PauseMenuQuit"]).Width + borderWidth, UIHelper.GetRectangle(UIManager.uiElements["PauseMenuQuit"]).Height + borderWidth);
                                    break;
                            }

                            break;
                        #endregion
                        #region End Game
                        case GameStates.EndGame:
                            camera.Update(new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2));

                            if (kb.IsKeyDown(Keys.Up) && prevKB.IsKeyUp(Keys.Up) || GamePad.GetState(0).ThumbSticks.Left.Y < -.9 && prevGamePad.ThumbSticks.Left.Y == 0 ||
                               GamePad.GetState(0).DPad.Up == ButtonState.Pressed && prevGamePad.DPad.Up == ButtonState.Released)
                            {
                                PauseMenuButtonIndex--;
                                if (PauseMenuButtonIndex < 0)
                                    PauseMenuButtonIndex = 0;
                            }
                            if (kb.IsKeyDown(Keys.Down) && prevKB.IsKeyUp(Keys.Down) || GamePad.GetState(0).ThumbSticks.Left.Y > .9 && prevGamePad.ThumbSticks.Left.Y == 0 ||
                                GamePad.GetState(0).DPad.Down == ButtonState.Pressed && prevGamePad.DPad.Down == ButtonState.Released)
                            {
                                PauseMenuButtonIndex++;
                                if (PauseMenuButtonIndex > 1)
                                    PauseMenuButtonIndex = 1;
                            }
                            prevGamePad = GamePad.GetState(0);

                            if (kb.IsKeyDown(Keys.Enter) && prevKB.IsKeyUp(Keys.Enter)||
                                GamePad.GetState(0).Buttons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released)
                            {
                                UIManager.RemovePauseMenu();

                                if (PauseMenuButtonIndex == 0)
                                {
                                    currScene = Scenes.TitleScreen;
                                    MenuState = MenuStates.MainMenu;

                                    graphics.PreferredBackBufferWidth = 768;/*(int)(graphics.PreferredBackBufferWidth * 1.5f)*/
                                    graphics.PreferredBackBufferHeight = 432;/*(int)(graphics.PreferredBackBufferHeight * 1.5f);*/
                                    graphics.ApplyChanges();

                                    mainMenuPos = new Vector2(385, 210);
                                    //graphics.HardwareModeSwitch = false;
                                    //graphics.IsFullScreen = true;

                                    //maxShootRate = shootRate;
                                    camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
                                    camera.Zoom = 1f;
                                    soundManager.ClearSounds();
                                    UIManager.uiElements["HealthBar"].Visible = false;
                                    UIManager.uiElements["DashIcon"].Visible = false;
                                    UIHelper.SetButtonState("MainMenuSetting", false, UIManager.uiElements);
                                    UIHelper.SetElementVisibility("EndGame", false, UIManager.uiElements);

                                }
                                else if (PauseMenuButtonIndex == 1)
                                {
                                    Exit();
                                }

                                //else if (PauseMenuButtonIndex == 0)
                                //{
                                //    GameState = prevGameState;
                                //    UIManager.uiElements["HealthBar"].Visible = true;
                                //    UIManager.uiElements["DashIcon"].Visible = true;
                                //    //UIManager.RemovePauseMenu();
                                //}
                                
                                PauseMenuButtonIndex = 0;
                                //UIManager.RemovePauseMenu();
                            }

                            borderWidth = 8; //  is /2
                            switch (PauseMenuButtonIndex)
                            {
                                case 0: //Main Menu
                                    selectedBox = new Rectangle(UIHelper.GetRectangle(UIManager.uiElements["PauseMenuReturn"]).X - borderWidth / 2, UIHelper.GetRectangle(UIManager.uiElements["PauseMenuReturn"]).Y - borderWidth / 2,
                                            UIHelper.GetRectangle(UIManager.uiElements["PauseMenuReturn"]).Width + borderWidth, UIHelper.GetRectangle(UIManager.uiElements["PauseMenuReturn"]).Height + borderWidth);
                                    break;
                                case 1: //Quit Game
                                    selectedBox = new Rectangle(UIHelper.GetRectangle(UIManager.uiElements["PauseMenuQuit"]).X - borderWidth / 2, UIHelper.GetRectangle(UIManager.uiElements["PauseMenuQuit"]).Y - borderWidth / 2,
                                            UIHelper.GetRectangle(UIManager.uiElements["PauseMenuQuit"]).Width + borderWidth, UIHelper.GetRectangle(UIManager.uiElements["PauseMenuQuit"]).Height + borderWidth);
                                    break;
                                   
                            }

                           
                            break;
                            #endregion
                    }


                    break;
                


            }



            // TODO: Add your update logic here
            prevButtons = GamePad.GetState(PlayerIndex.One).Buttons;
            prevKB = kb;
            base.Update(gameTime);
        }

        private void SideScrollPhysics(GameTime gameTime, KeyboardState kb, Rectangle worldRect)
        {
            dont = false;

            foreach (BottomDoorTile door in SideTileMap.BottomDoorTiles)
            {

                float num = MathHelper.Distance(ssPlayer.playerRect.Center.X, door.Rectangle.Center.X);
                Rectangle doorRect = door.Rectangle;
                if (ssPlayer.isFalling)
                {
                    doorRect = new Rectangle(door.Rectangle.X, door.Rectangle.Bottom, door.Rectangle.Width, 1);
                }
                if (ssCamera.CameraBounds.Contains(doorRect) && num < 64 * 12 && ssPlayer.Y / 64 == door.Rectangle.Y / 64 || 
                    ssCamera.CameraBounds.Contains(doorRect) && num < 64 * 12 && MathHelper.Distance(ssPlayer.playerRect.Y, door.Rectangle.Bottom + 64) > 64)
                {

                    dont = true;
                }
                else if (ssCamera.CameraBounds.Contains(doorRect) && MathHelper.Distance(ssPlayer.playerRect.X, doorRect.X) > 64 * 12
                        /*|| ssCamera.CameraBounds.Contains(doorRect) && MathHelper.Distance(ssPlayer.playerRect.Y, door.Rectangle.Bottom + 64) <= 64 */)
                {

                    dont = false;
                }

                bool warning = false;
                if (ssPlayer.animManager.isRight && openDoorCount > bossKillCount && GameState == GameStates.SideScroll)
                    warning = true;
                

                if (new Rectangle(door.Rectangle.X - 64, door.Rectangle.Y, door.Rectangle.Width, door.Rectangle.Height).Contains(ssPlayer.playerRect) && interactActive == Rectangle.Empty)
                {
                   
                    UIManager.CreateInteractUI(new Point(ssPlayer.playerRect.Center.X - 32, ssPlayer.playerRect.Y- 64), warning);
                    interactActive = new Rectangle(door.Rectangle.X - 64, door.Rectangle.Y, door.Rectangle.Width, door.Rectangle.Height);
                    UIHelper.SetElementVisibility("InteractBox", true, UIManager.uiElements);
                }
                else if(new Rectangle(door.Rectangle.Right, door.Rectangle.Y, door.Rectangle.Width, door.Rectangle.Height).Contains(ssPlayer.playerRect) && interactActive == Rectangle.Empty)
                {
                    UIManager.CreateInteractUI(new Point(ssPlayer.playerRect.Center.X - 32, ssPlayer.playerRect.Y - 64), warning);
                    interactActive = new Rectangle(door.Rectangle.Right, door.Rectangle.Y, door.Rectangle.Width, door.Rectangle.Height);
                    UIHelper.SetElementVisibility("InteractBox", true, UIManager.uiElements);
                }
                else if(interactActive.Contains(ssPlayer.playerRect) == false)
                {
                    UIManager.RemoveInteractUI();
                    interactActive = Rectangle.Empty;
                    UIHelper.SetElementVisibility("InteractBox", false, UIManager.uiElements);
                }
            }

            //foreach (TopDoorTile door in SideTileMap.TopDoorTiles)
            //{

            //    float num = MathHelper.Distance(ssPlayer.playerRect.Center.X, door.Rectangle.Center.X);
            //    if (ssCamera.CameraBounds.Contains(door.Rectangle) && num < 64 * 12/* && ssPlayer.Y/64 == door.Rectangle.Y/64*/)
            //    {

            //        dont = true;
            //    }
            //    else if (ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.X, door.Rectangle.X) > 64 * 12
            //            /*|| ssCamera.CameraBounds.Contains(door.Rectangle) && MathHelper.Distance(ssPlayer.playerRect.Y, door.Rectangle.Y) > 64 * 10*/)
            //    {

            //        dont = false;
            //    }
            //}

            if (dont == false && canChange)
            {
                ssCamera.min = 0;
            }

                //dont = false;

            if (ssPlayer.isPilot)
            {
                ssCamera.Update(new Vector2(ssPlayer.playerRect.X, ssPlayer.playerRect.Y - (60 - ssPlayer.playerRect.Height)), dont, fade);
            }
            else
            {
                ssCamera.Update(new Vector2(ssPlayer.playerRect.X, ssPlayer.playerRect.Y), dont, fade);
            }

            UIHelper.UpdatePlayerUI(UIManager.uiElements["HealthBar"], new Rectangle(new Point(ssCamera.CameraBounds.X + 20,
             ssCamera.CameraBounds.Y + 20), new Point(0, 0)));
            UIHelper.UpdatePlayerUI(UIManager.uiElements["DashIcon"], new Rectangle(new Point(ssCamera.ViewRect.Right - 145, ssCamera.ViewRect.Bottom - 130), new Point(0, 0)));
            UIHelper.ChangeDashIcon(UIManager.uiElements["DashIcon"], ssPlayer.DashIndex());



            if (kb.IsKeyDown(Keys.P) || ssPlayer.Health <= 0 || ssPlayer.playerRect.Y > SideTileMap.GetWorldDims().Y)//Reset Pos
            {
                //if (GameState == GameStates.SideScroll)
                //    StartNewGame(true);
                //else if (GameState == GameStates.Tutorial)
                //    LoadTutorial();
                //else if (GameState == GameStates.FinalBoss)
                //    LoadFinalBoss();
                ////StartNewGame();
                //ssCamera = new SSCamera(GraphicsDevice.Viewport, new Vector2(0, 0), (int)SideTileMap.GetWorldDims().X, (int)SideTileMap.GetWorldDims().Y);
                //fadePos = SideTileMap.playerSpawns[0];

                //soundManager.ClearSounds();
                //soundManager.AddSound("Level0Side", true);
                LoadGame();
            }
            UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)ssPlayer.Health);
            if (updateDoor)
            {
                UpdateDoor(topIndex, bottomIndex);
            }
            ssPlayer.Update(gameTime, Gravity, enemies, this);


            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (ssCamera.ViewRect.Intersects(enemies[i].enemyRect) /*|| enemies[i].enemyState != SSEnemy.EnemyStates.Idle*/)
                {
                    enemies[i].Update(gameTime, Gravity, ssPlayer, this, new Vector2(groundPoundX, groundPoundY));
                    for (int j = i + 1; j < enemies.Count; j++)
                    {
                        if (enemies[i].enemyRect.Intersects(enemies[j].enemyRect) && worldRect.Intersects(enemies[j].enemyRect))
                        {
                            if (enemies[i].velocity.X == enemies[j].velocity.X)
                                enemies[i].velocity.X /= 2;
                            else
                            {
                                if (enemies[i].velocity.X != enemies[i].maxRunSpeed || enemies[i].velocity.X != -enemies[i].maxRunSpeed)
                                {
                                    if (enemies[i].velocity.X < 0)
                                        enemies[i].velocity.X = -enemies[i].maxRunSpeed;
                                    else if (enemies[i].velocity.X > 0)
                                        enemies[i].velocity.X = enemies[i].maxRunSpeed;

                                }
                            }

                        }

                    }

                    if (enemies[i].dead)
                    {
                        if (rand.Next(0, 101) < dropRateSS)
                        {
                            healthDrops.Add(new HealthDrop(new Rectangle(enemies[i].enemyRect.X, enemies[i].enemyRect.Y, healthDropDims.X, healthDropDims.X)));
                        }

                        soundManager.AddSound("SoundEffects/Enemy Dies_01", false);
                        soundManager.PlaySound();
                        enemies.RemoveAt(i);
                    }
                }

            }

            for (int i = healthDrops.Count - 1; i >= 0; i--)
            {
                if (healthDrops[i].rect.Intersects(ssPlayer.playerRect))
                {
                    ssPlayer.Health += healAmount;

                    healthDrops.RemoveAt(i);

                }
            }
            //foreach(SSEnemy enemy in enemies)
            //{
            //    enemy.Update(gameTime, Gravity, ssPlayer, this);
            //}
            //enemy.Update(gameTime, Gravity, ssPlayer);

            ssPlayer.blockBottom = false;

            //ssPlayer.isColliding = false;

            foreach (GroundTile tile in SideTileMap.GroundTiles)
            {
                ssPlayer.Collision(tile.Rectangle);
                if (ssPlayer.isCollidingRight == true &&
                ssPlayer.isCollidingLeft == true)
                {
                    ssPlayer.isCollidingRight = false;
                    ssPlayer.isCollidingLeft = false;
                }

                //bool trueBreak = false;
                //foreach (Rectangle breakables in ssPlayer.breakTiles)
                //{
                //    if (breakables == tile.Rectangle)
                //    {
                //        destroyedGround.Add(tile);
                //        SideTileMap.GroundTiles.Remove(tile);
                //        ssPlayer.breakTiles.Remove(breakables);
                //        respawnDelay = respawnDelaySet;
                //        trueBreak = true;
                //        break;
                //    }
                //}
                //if (trueBreak)
                //    break;
            }

            foreach (PlatformTile tile1 in SideTileMap.PlatformTiles)
            {
                ssPlayer.Collision(tile1.Rectangle);
                //bool trueBreak = false;
                //foreach (Rectangle breakables in ssPlayer.breakTiles)
                //{
                //    if (breakables == tile1.Rectangle)
                //    {
                //        destroyedPlatfroms.Add(tile1);
                //        SideTileMap.PlatformTiles.Remove(tile1);
                //        ssPlayer.breakTiles.Remove(breakables);
                //        respawnDelay = respawnDelaySet;
                //        trueBreak = true;
                //        break;
                //    }
                //}
                //if (trueBreak)
                //    break;
            }

            //if (respawnDelay > 0 && destroyedGround.Count != 0)
            //{
            //    respawnDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    if (respawnDelay <= 0)
            //    {
            //        SideTileMap.GroundTiles.Add(destroyedGround[0]);
            //        destroyedGround.Remove(destroyedGround[0]);
            //        respawnDelay = respawnDelaySet;
            //    }
            //}

            //if (respawnDelay > 0 && destroyedPlatfroms.Count != 0)
            //{
            //    respawnDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    if (respawnDelay <= 0)
            //    {
            //        SideTileMap.PlatformTiles.Add(destroyedPlatfroms[0]);
            //        destroyedPlatfroms.Remove(destroyedPlatfroms[0]);
            //        respawnDelay = respawnDelaySet;
            //    }
            //}


            ssPlayer.isCollidingDown = false;
            foreach (SSEnemy enemy in enemies)
            {
                ssPlayer.Collision(enemy.enemyRect, true);
                if (ssPlayer.killEnemy && enemy.enemyState != SSEnemy.EnemyStates.Knockback)
                {
                    enemy.prevState = enemy.enemyState;
                    enemy.enemyState = SSEnemy.EnemyStates.Knockback;

      
                    //enemies.Remove(enemy);
                    //ssPlayer.killEnemy = false;
                   
                    break;
                }
            }

            foreach (TopDoorTile tile in SideTileMap.TopDoorTiles)
            {
                ssPlayer.Collision(tile.Rectangle);
                if (ssPlayer.isCollidingRight == true &&
               ssPlayer.isCollidingLeft == true)
                {
                    ssPlayer.isCollidingRight = false;
                    ssPlayer.isCollidingLeft = false;
                }

            }
            foreach (BottomDoorTile tile in SideTileMap.BottomDoorTiles)
            {
                ssPlayer.Collision(tile.Rectangle);
                if (ssPlayer.isCollidingRight == true &&
               ssPlayer.isCollidingLeft == true)
                {
                    ssPlayer.isCollidingRight = false;
                    ssPlayer.isCollidingLeft = false;
                }

            }

            foreach (WallTile tile2 in SideTileMap.WallTiles)
            {
                ssPlayer.Collision(tile2.Rectangle);
            }
            if (ssPlayer.blockBottom == false && ssPlayer.playerState != SSPlayer.PlayerStates.Jumping)
            {
                ssPlayer.coyoteTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (ssPlayer.coyoteTime <= 0)
                {
                    ssPlayer.isFalling = true;
                    ssPlayer.coyoteTime = ssPlayer.coyoteTimeMax;
                    ssPlayer.coyote = false;
                }

            }
            double framerate = (1 / gameTime.ElapsedGameTime.TotalSeconds);
         
        }

        private void ChangeToSideScroll()
        {
            
            
            GameState = GameStates.SideScroll;
            //ssCamera = new SSCamera(GraphicsDevice.Viewport, new Vector2(0, 0), (int)SideTileMap.GetWorldDims().X, (int)SideTileMap.GetWorldDims().Y);
            graphics.PreferredBackBufferWidth = 1920;/*(int)(graphics.PreferredBackBufferWidth * 1.5f)*/
            graphics.PreferredBackBufferHeight = 1080;/*(int)(graphics.PreferredBackBufferHeight * 1.5f);*/
            soundManager.ClearSounds();

            try
            {
                
                soundManager.AddSound("Level" + (dungeonNum) + "Side", true);
                soundManager.PlaySound();
            }
            catch
            {

            }
            //graphics.HardwareModeSwitch = false;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            List<Texture2D> healthbars = new List<Texture2D>();
            for (int i = 0; i <= 10; i++)
            {
                healthbars.Add(Content.Load<Texture2D>(@"SideScroll\HealthBar\RoboHealthBar" + i));
            }

            UIHelper.HealthBar = healthbars;
            
            UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)ssPlayer.Health);
            prevGameState = GameState;
            GameState = GameStates.Paused;

            
            fadePos = new Vector2(ssPlayer.position.X, ssPlayer.position.Y);
        }

        private Rectangle CheckForTiles(Rectangle worldRect)
        {
            for (int i = 0; i < SideTileMap.GroundTiles.Count - 1; i++)
            {
                if (worldRect.Intersects(SideTileMap.GroundTiles[i].Rectangle) == false)
                {
                    offScreenGround.Add(SideTileMap.GroundTiles[i]);
                    SideTileMap.GroundTiles.Remove(SideTileMap.GroundTiles[i]);
                }



            }
            for (int i = offScreenGround.Count - 1; i >= 0; i--)
            {
                if (worldRect.Intersects(offScreenGround[i].Rectangle))
                {
                    SideTileMap.GroundTiles.Add(offScreenGround[i]);
                    offScreenGround.RemoveAt(i);
                }
            }


            for (int i = 0; i < SideTileMap.PlatformTiles.Count - 1; i++)
            {
                if (worldRect.Intersects(SideTileMap.PlatformTiles[i].Rectangle) == false)
                {
                    offScreenPlatform.Add(SideTileMap.PlatformTiles[i]);
                    SideTileMap.PlatformTiles.Remove(SideTileMap.PlatformTiles[i]);
                }

            }
            for (int i = offScreenPlatform.Count - 1; i >= 0; i--)
            {
                if (worldRect.Intersects(offScreenPlatform[i].Rectangle))
                {
                    SideTileMap.PlatformTiles.Add(offScreenPlatform[i]);
                    offScreenPlatform.RemoveAt(i);
                }
            }

            return worldRect;
        }

        public void OpenDoor(Rectangle bottomTile)
        {
            bool open = true;
            if(GameState == GameStates.SideScroll)
            {
                if (openDoorCount == 0 && ssPlayer.animManager.isLeft || openDoorCount > bossKillCount && !ssPlayer.animManager.isLeft)
                {
                    open = false;
                    //if(openDoorCount > bossKillCount)
                    //    UIManager.CreateInteractUI(new Point(ssPlayer.playerRect.Center.X - 32, ssPlayer.playerRect.Y - 64), true);
                }
                   
            }
            if(open)
            {
                UIManager.RemoveInteractUI();
                //SetDungeonNum();
                if(openDoorCount >= 1 && ssPlayer.animManager.isRight || openDoorCount > 1 && ssPlayer.animManager.isLeft)
                    soundManager.ClearSounds();
     

                //Find top door
                int bottomTileIndex = 0;
                int topTileIndex = 0;
                for (int i = 0; i < SideTileMap.BottomDoorTiles.Count; i++)
                {
                    if (SideTileMap.BottomDoorTiles[i].Rectangle == bottomTile)
                    {
                        bottomTileIndex = i;
                    }
                }
                bottomTile.Y -= pixelBits;
                for (int i = 0; i < SideTileMap.TopDoorTiles.Count; i++)
                {
                    if (SideTileMap.TopDoorTiles[i].Rectangle == bottomTile)
                    {
                        topTileIndex = i;
                    }
                }
                bottomIndex = bottomTileIndex;
                topIndex = topTileIndex;

                updateDoor = true;
                prevGameState = GameState;
                doorTrans = true;
                savedDoorY = new Vector2(SideTileMap.TopDoorTiles[topIndex].Rectangle.Top, SideTileMap.BottomDoorTiles[bottomIndex].Rectangle.Top);
                if (ssPlayer.animManager.isRight)
                {
                    updatedPos = new Vector2(SideTileMap.BottomDoorTiles[bottomIndex].Rectangle.Right + ssPlayer.playerRect.Width / 2,
                        SideTileMap.BottomDoorTiles[bottomIndex].Rectangle.Top + (SideTileMap.BottomDoorTiles[bottomIndex].Rectangle.Height - ssPlayer.playerRect.Height));

                    savedPos = new Vector2(savedPos.X + ssCamera.ViewRect.Width / 2, savedPos.Y);
                    if (GameState == GameStates.SideScroll)
                    {
                        openDoorCount++;
                    }
                    if (bossKillCount == 4)
                        LoadFinalBoss();
                    else if (openDoorCount > 1)
                    {
                        try
                        {
                            soundManager.AddSound("Level" + (openDoorCount - 1) + "Side", true);
                            soundManager.PlaySound();
                        }
                        catch
                        {

                        }
                    }
                   
                        

                }
                else if (ssPlayer.animManager.isLeft)
                {
                    updatedPos = new Vector2(SideTileMap.BottomDoorTiles[bottomIndex].Rectangle.Left - ssPlayer.playerRect.Width - 20,
                       SideTileMap.BottomDoorTiles[bottomIndex].Rectangle.Top + (SideTileMap.BottomDoorTiles[bottomIndex].Rectangle.Height - ssPlayer.playerRect.Height));
                    savedPos = new Vector2(savedPos.X - ssCamera.ViewRect.Width / 2, savedPos.Y);
                    if (GameState == GameStates.SideScroll)
                    {
                        openDoorCount--;
                    }
                    if (openDoorCount >= 1)
                    {
                        try
                        {
                            soundManager.AddSound("Level" + (openDoorCount - 1) + "Side", true);
                            soundManager.PlaySound();
                        }
                        catch
                        {

                        }
                    }
                }

           
                dont = false;
                fadePos = ssCamera.Position;

                if (GameState == GameStates.Tutorial)
                {
                    minChange = SideTileMap.BottomDoorTiles[0].Rectangle.X;
                    openDoorCount++;
                }
                else if (GameState == GameStates.SideScroll)
                {
                    minChange = SideTileMap.BottomDoorTiles[bottomIndex].Rectangle.X;
                }

            }


        }

        void UpdateDoor(int topTile, int bottomTile)
        {
            SideTileMap.TopDoorTiles[topTile].setY((SideTileMap.TopDoorTiles[topTile].Rectangle.Y - doorOpenSpeed));
            SideTileMap.BottomDoorTiles[bottomTile].setY((SideTileMap.BottomDoorTiles[bottomTile].Rectangle.Y + doorOpenSpeed));

            if (MathHelper.Distance(SideTileMap.TopDoorTiles[topTile].Rectangle.Bottom, SideTileMap.BottomDoorTiles[bottomTile].Rectangle.Top) > pixelBits * 2)
            {
                updateDoor = false;

                //SideTileMap.TopDoorTiles[topTile].active = false;
                //SideTileMap.BottomDoorTiles[bottomTile].active = false;
                //dont = false;

                closeDoor = true;
            }


        }

        void CloseDoor(int topTile, int bottomTile)
        {
            SideTileMap.TopDoorTiles[topTile].setY((SideTileMap.TopDoorTiles[topTile].Rectangle.Y + doorOpenSpeed));
            SideTileMap.BottomDoorTiles[bottomTile].setY((SideTileMap.BottomDoorTiles[bottomTile].Rectangle.Y - doorOpenSpeed));

            if(SideTileMap.TopDoorTiles[topTile].Rectangle.Bottom - 1 >= SideTileMap.BottomDoorTiles[bottomTile].Rectangle.Top)
            {
                closeDoor = false;

                SideTileMap.TopDoorTiles[topTile].setY(savedDoorY.X);
                SideTileMap.BottomDoorTiles[bottomTile].setY(savedDoorY.Y);
                UIManager.uiElements["DashIcon"].Visible = true;
                UIManager.uiElements["HealthBar"].Visible = true;

                if(openDoorCount > 1)
                    SaveGame();

            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Window.Title = camera.Position.ToString() + "   MousePos: " + mousePos.ToString();
            switch(currScene)
            {
                case Scenes.TitleScreen:
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

                    UIManager.Draw(spriteBatch);
                    controllerReticle.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case Scenes.InGame:
                    if(GameState == GameStates.SideScroll || GameState == GameStates.Tutorial)
                    {
                        if(fade)
                        {
                            iRate -= rate;
                            if(iRate < 0)
                            {
                                iRate = 1;
                                fade = false;
                                UIManager.uiElements["HealthBar"].Visible = true;
                                UIManager.uiElements["DashIcon"].Visible = true;
                                if (GameState == GameStates.Tutorial && openDoorCount == 0)
                                    startGame = true;
                            }
                            
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, ssCamera.transform);

                            SideTileMap.Draw(spriteBatch, Content, ssCamera);
                            //spriteBatch.Draw(Content.Load<Texture2D>("SideScroll/MapTiles/BG1"), ssCamera.CameraBounds, Color.White);
                            ssPlayer.Draw(spriteBatch);
                            spriteBatch.Draw(Content.Load<Texture2D>("TopDown/Textures/Player"), ssCamera.CameraBounds, Color.Black * (iRate));
                           
                            spriteBatch.End();
                        }
                        else if(doorTrans)
                        {
                            sRate += rate * 1.5f;

                            if(sRate > 1)
                            {
                                sRate = 0;
                                fade = true;
                                
                                if(ssPlayer.animManager.isRight)
                                {
                                    if(GameState == GameStates.Tutorial)
                                    {
                                        ssCamera.min = SideTileMap.BottomDoorTiles[0].Rectangle.X;
                                        ssCamera.center.X += ssCamera.CameraBounds.Width;
                                        
                                    }
                                    else if(GameState == GameStates.SideScroll)
                                    {
                                        ssCamera.center.X += ssCamera.CameraBounds.Width;
                                        ssCamera.min = (int)((ssCamera.center.X - 64) - ssCamera.CameraBounds.Width / 2);
                                        canChange = true;
                                    }
                                   
                                }
                                else if(ssPlayer.animManager.isLeft)
                                {
                                    ssCamera.center.X -= ssCamera.CameraBounds.Width;
                                    ssCamera.min = 0;
                                }
                               
                                if(GameState == GameStates.SideScroll)
                                {
                                    ssPlayer.playerRect = new Rectangle((int)updatedPos.X, (int)updatedPos.Y, ssPlayer.playerRect.Width, ssPlayer.playerRect.Height);
                                    ssPlayer.Position = updatedPos;
                                }
                                else if(GameState == GameStates.Tutorial)
                                {
                                    ssPlayer.playerRect = new Rectangle((int)ssPlayer.Position.X + 160, SideTileMap.BottomDoorTiles[0].Rectangle.Y,
                                  ssPlayer.playerRect.Width, ssPlayer.playerRect.Height);
                                    ssPlayer.Position = new Vector2(ssPlayer.playerRect.X, ssPlayer.playerRect.Y);

                                    ssCamera.center.Y = ssPlayer.playerRect.Y;
                                    //ssCamera.center.X += ssCamera.CameraBounds.Width/1.75f;
                                    fadePos = new Vector2(SideTileMap.BottomDoorTiles[0].Rectangle.X + ssCamera.CameraBounds.Width/2, ssPlayer.playerRect.Y);
                                }
                              
                               


                                doorTrans = false;
                            }

                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, ssCamera.transform);

                            SideTileMap.Draw(spriteBatch, Content, ssCamera);
                            ssPlayer.Draw(spriteBatch);
                            spriteBatch.Draw(Content.Load<Texture2D>("TopDown/Textures/Player"), ssCamera.CameraBounds, Color.Black * (sRate));
                            spriteBatch.End();
                        }
                        else
                        {
                            if (startGame)
                            {
                                UIManager.CreateTutorialUI(SideTileMap.playerSpawns[0], this);
                                UIHelper.SetElementVisibility("Tutorial", true, UIManager.uiElements);
                                startGame = false;
                               
                                UIHelper.ChangeHealthBar(UIManager.uiElements["HealthBar"], (int)ssPlayer.Health);
                                UIManager.CreatePauseMenu(Rectangle.Empty);
                                SaveGame();
                            }
                            //UIManager.CreateTutorialUI(SideTileMap.playerSpawns[0], this);
                            //UIHelper.SetElementVisibility("Tutorial", true, UIManager.uiElements);
                            Window.Title = camera.Position.ToString() + " Player.playrRect " + new Vector2(ssPlayer.playerRect.X, ssPlayer.playerRect.Y).ToString() /*+ "    EnemyBounds: " + enemies[0].bounds.ToString()*/ + "    AnalogStickDir: " + moveDir.ToString();
                            //Window.Title = "Gravity: " + Gravity.Y.ToString() /*+ "  a = " + ((decimal)ssPlayer.Acceleration) + "   F = " + ((decimal)ssPlayer.Force) + " Friction = " + ssPlayer.friction */+ "   Vel = " + enemy.Velocity.ToString() + "   onPlatform = " + enemy.onPlatform + "   enemyState = " + enemy.enemyState.ToString();
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, ssCamera.transform);

                            SideTileMap.Draw(spriteBatch, Content, ssCamera);
                           
                            ssPlayer.Draw(spriteBatch);
                            foreach (SSEnemy enemy in enemies)
                            {
                                enemy.Draw(spriteBatch);
                            }
                            foreach (FlyingControlBeacon beacon in flyingBeacons)
                            {
                                beacon.Draw(spriteBatch);
                            }
                            foreach (HealthDrop health in healthDrops)
                            {
                                health.Draw(spriteBatch);
                            }
                            UIManager.Draw(spriteBatch);
                            //for(int i = 0; i < healthBar.Count; i++)
                            //{
                            //    spriteBatch.Draw(Content.Load<Texture2D>("TopDown/Textures/Player"), new Vector2(healthBar[i].X, healthBar[i].Y), healthBar[i], Color.Red);
                            //}

                            //enemy.Draw(spriteBatch);

                            // spriteBatch.Draw(Content.Load<Texture2D>("TopDown/Textures/Player"), ssCamera.CameraBounds, Color.White * .5f);
                            spriteBatch.End();
                        }
                        
                    }
                    else if(GameState == GameStates.TopDown)
                    {
                        if (fade)
                        {
                            iRate -= rate;
                            if (iRate < 0)
                            {
                                iRate = 1;
                                fade = false;
                            }
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

                            tdMap.Draw(spriteBatch);
                            tdPlayer.Draw(spriteBatch);
                            spriteBatch.Draw(Content.Load<Texture2D>("TopDown/Textures/Player"), new Rectangle(new Point((int)(camera.Position.X - camera.viewport.Width/2), (int)(camera.Position.Y - camera.viewport.Height/2)), 
                                new Point(camera.viewport.Width, camera.viewport.Height)), Color.Black * iRate);
                            UIManager.Draw(spriteBatch);
                            spriteBatch.End();
                        }
                        else
                        {
                            if(shotGunBoss != null)
                            {
                                Window.Title = "AnalogStickDir: " + moveDir.ToString() + "   IsColliding " + shotGunBoss.moveBack;
                            }
                          
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                            tdMap.Draw(spriteBatch);
                            if (startBoss)
                            {
                                if(shotGunBoss != null)
                                shotGunBoss.Draw(spriteBatch);

                                Window.Title = " ";
                                switch(dungeonNum)
                                {
                                    case 0:
                                        if (shotGunBoss != null)
                                            shotGunBoss.Draw(spriteBatch);
                                        break;
                                    case 1:
                                        slamBoss.Draw(spriteBatch);
                                        break;
                                    case 2:
                                        bombBoss.Draw(spriteBatch);
                                        break;
                                    case 3:
                                        for (int i = 0; i < laserBoss.bossRects.Count; i++)
                                        {
                                            //angles[i] = laserBoss.bossRects[i].angle;
                                            Window.Title += "Boss" + i + ": " + laserBoss.bossRects[i].angle;

                                        }
                                        laserBoss.Draw(spriteBatch);
                                        break;
                                }
                                if (LeaveDungeon != Rectangle.Empty)
                                    spriteBatch.Draw(Content.Load<Texture2D>("TopDown/MapTiles/Tile15"), LeaveDungeon, Color.White);
                                //float[] angles = new float[laserBoss.bossRects.Count];
                                    #region LaserBoss
                                    //for (int i = 0; i < laserBoss.bossRects.Count; i++)
                                    //{
                                    //    //angles[i] = laserBoss.bossRects[i].angle;
                                    //    Window.Title += "Boss" + i + ": " + laserBoss.bossRects[i].angle;

                                    //}
                                    //laserBoss.Draw(spriteBatch);
                                    #endregion
                                    //bombBoss.Draw(spriteBatch);
                                    //Window.Title = "Angle: " + bombBoss.angle;
                                    //slamBoss.Draw(spriteBatch);
                                    //Window.Title = "Angle: " + slamBoss.angle;
                                foreach (HealthDrop health in bossHealthDrops)
                                {
                                    health.Draw(spriteBatch);
                                }
                            }
                            tdPlayer.Draw(spriteBatch);

                            foreach (TDEnemy enemy in tdEnemies)
                            {
                                enemy.Draw(spriteBatch);
                            }
                            foreach (HealthDrop health in healthDrops)
                            {
                                health.Draw(spriteBatch);
                            }
                           
                            UIManager.Draw(spriteBatch);
                            //foreach(Bullet bullet in bossBullets)
                            //{
                            //    bullet.Draw(spriteBatch);
                            //}
                            //spriteBatch.Draw(Content.Load<Texture2D>("TopDown/MapTiles/Tile11"), Boss, Color.White);
                            spriteBatch.End();

                            //if (changeLevel)
                            //    map.Refresh(PosXLevel.xLevels, PosYLevel.yLevels, Diagonal.dLevels, pixelBits, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, PosXLevel.Points, PosYLevel.Points, Diagonal.Points);
                        }

                    }
                    else if(GameState == GameStates.Paused)
                    {
                        if(black)
                            GraphicsDevice.Clear(Color.Black * .25f);
                        //Window.Title = ssCamera.CameraBounds.ToString();
                    }
                    else if(GameState == GameStates.FinalBoss)
                    {
                        if (fade)
                        {
                            
                            iRate -= rate;
                            if (iRate < 0)
                            {
                                iRate = 1;
                                fade = false;
                                startGame = true;
                            }

                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, ssCamera.transform);

                            SideTileMap.Draw(spriteBatch, Content, ssCamera, true);
                            //spriteBatch.Draw(Content.Load<Texture2D>("SideScroll/MapTiles/BG1"), ssCamera.CameraBounds, Color.White);
                            ssPlayer.Draw(spriteBatch);
                            spriteBatch.Draw(Content.Load<Texture2D>("TopDown/Textures/Player"), ssCamera.CameraBounds, Color.Black * (iRate));

                            spriteBatch.End();
                        }
                        else
                        {
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, ssCamera.transform);

                            SideTileMap.Draw(spriteBatch, Content, ssCamera, true);
                            UIManager.Draw(spriteBatch);
                            ssPlayer.Draw(spriteBatch);
                            finalBoss.Draw(spriteBatch);

                            spriteBatch.End();
                        }
                    }
                    else if(GameState == GameStates.InGamePause)
                    {
                        if(prevGameState == GameStates.Tutorial || prevGameState == GameStates.SideScroll)
                        {
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, ssCamera.transform);
                            SideTileMap.Draw(spriteBatch, Content, ssCamera);
                            ssPlayer.Draw(spriteBatch);
                            spriteBatch.Draw(Content.Load<Texture2D>("Textures/white"), selectedBox, Color.SteelBlue);
                            UIManager.Draw(spriteBatch);
                            spriteBatch.End();

                        }
                        else if(prevGameState == GameStates.TopDown)
                        {
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                            tdMap.Draw(spriteBatch);
                            tdPlayer.Draw(spriteBatch);
                            spriteBatch.Draw(Content.Load<Texture2D>("Textures/white"), selectedBox, Color.SteelBlue);
                            UIManager.Draw(spriteBatch);
                            spriteBatch.End();
                        }
                            
                    }
                    else if(GameState == GameStates.EndGame)
                    {
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                        spriteBatch.Draw(Content.Load<Texture2D>("Textures/white"), selectedBox, Color.SteelBlue);
                        UIManager.Draw(spriteBatch);
                        //Draw Image of player
                        spriteBatch.End();
                    }
                   
                    break;
            }


            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        private void Menus(KeyboardState kb)
        {
            float crawlSpeed = 10f;
            GamePadButtons currButtons = GamePad.GetState(PlayerIndex.One).Buttons;
            if (UIManager.willDelete)
                UIManager.SetDeleteWarnings(true);
            //if(UIManager.uiElements.ContainsKey("PauseMainMenuBtn") /*&& UIManager.uiElements["PauseMainMenuBtn"].Visible*/)
            //{
            //    UIManager.uiElements["PauseMenuReturn"].Visible = false;
            //    UIManager.uiElements["PauseMainMenuBtn"].Visible = false;
            //    UIManager.uiElements["PauseMenuQuit"].Visible = false;
            //}
            switch (MenuState)
            {
                case MenuStates.TitleCrawl:
                    UIManager.UpdateTextBlock("MainMenuBackground", Rectangle.Empty);
                    UIHelper.SetElementVisibility("MainMenuTitle", false, UIManager.uiElements);
                    camera.Update(mainMenuPos);
                    startCrawl = true;

                    if(startCrawl)
                    {
                        UIManager.UpdateTextBlock("TitleCrawl", Rectangle.Empty);

                        if ( kb.GetPressedKeys().Length > 0
                            || currButtons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released || currButtons.B == ButtonState.Pressed && prevButtons.B == ButtonState.Released
                            || currButtons.Y == ButtonState.Pressed && prevButtons.Y == ButtonState.Released || currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released ||
                            UIHelper.GetElementBGRect(UIManager.uiElements["TitleCrawl"]).Bottom <= 0)
                        {
                            UIManager.SkipCrawl("TitleCrawl");
                            startCrawl = false;
                            MenuState = MenuStates.MainMenu;
                            soundManager.AddSound("menuTheme", true);
                            soundManager.PlaySound();
                            //UIHelper.SetElementVisibility("MainMenuBackground", false, UIManager.ui)
                        }
                    }
                    #region ReversedPsychology
                    //if (kb.IsKeyDown(Keys.Enter) && prevKB.IsKeyUp(Keys.Enter) && !startCrawl || currButtons.Start == ButtonState.Pressed && prevButtons.Start == ButtonState.Released)
                    //{
                    //    prevKB = kb;
                    //    startCrawl = true;
                    //}
                    //UIManager.UpdateTextBlock("TitleCrawl");
                    //if (startCrawl)
                    //{
                    //    TitleCrawl(3);
                    //    if (kb.IsKeyDown(Keys.G) && prevKB.IsKeyUp(Keys.G) || currButtons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released || currButtons.B == ButtonState.Pressed && prevButtons.B == ButtonState.Released 
                    //        || currButtons.Y == ButtonState.Pressed && prevButtons.Y == ButtonState.Released || currButtons.X == ButtonState.Pressed && prevButtons.X == ButtonState.Released)
                    //    {
                    //        camera.Update(new Vector2(camera.X, (UIHelper.GetElementBGRect(UIManager.uiElements["TitleCrawl"]).Bottom + (graphics.PreferredBackBufferHeight / 2))));
                    //        UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);
                    //        MenuState = MenuStates.MainMenu;
                    //        startCrawl = false;
                    //        prevKB = kb;
                    //    }
                    //}


                    //if (camera.Y >= (UIHelper.GetElementBGRect(UIManager.uiElements["TitleCrawl"]).Bottom + (graphics.PreferredBackBufferHeight / 2)) && startCrawl)
                    //{
                    //    MenuState = MenuStates.MainMenu;
                    //    //screenCenter.X = this.Window.ClientBounds.Width / 2;
                    //    //screenCenter.Y = this.Window.ClientBounds.Height / 2;

                    //    UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);
                    //    mainMenuPos = camera.Position;
                    //    mainMenuPos.Y = (UIHelper.GetElementBGRect(UIManager.uiElements["TitleCrawl"]).Bottom + (graphics.PreferredBackBufferHeight / 2));
                    //    //break;
                    //}
                    #endregion
                    break;
                case MenuStates.MainMenu:
                    //if (count == 0)
                    //{
                    //    mainMenuPos = camera.Position;
                    //    count++;
                    //}
                   
                    if(soundManager.currEffectName != "menuTheme")
                    {
                        soundManager.ClearSounds();
                        soundManager.AddSound("menuTheme", true);
                        soundManager.PlaySound();
                    }

                    UIHelper.SetElementVisibility("MainMenu", true, UIManager.uiElements);
                    UseMouse(kb, crawlSpeed);
                    UpdateCamera(mainMenuPos, 10);
                    UIManager.UpdateButton("MainMenu", crawlSpeed, this);
                  
        
                    //if (currButtons.Start == ButtonState.Pressed && prevButtons.Start == ButtonState.Released || 
                    //    currButtons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released && MenuButtonIndex == 0)
                    //{
                    //    MenuState = MenuStates.StartGame;
                    //    MenuButtonIndex = 0;
                    //}
                    //if (prevButtons.B == ButtonState.Pressed && currButtons.B == ButtonState.Released)
                    //{
                    //    MenuState = MenuStates.Settings;
                    //}
                    
                    //if(currButtons.Back == ButtonState.Pressed && prevButtons.Back == ButtonState.Released ||
                    //    currButtons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released && MenuButtonIndex == 1)
                    //{
                    //    Exit();
                    //}

                    //if(GamePad.GetState(0).DPad.Down == ButtonState.Pressed)
                    //{
                    //    MenuButtonIndex = 1;
                    //}
                    //if(GamePad.GetState(0).DPad.Up == ButtonState.Pressed)
                    //{
                    //    MenuButtonIndex = 0;
                    //}

                    break;
                case MenuStates.StartGame:

                    UIManager.UpdateButton("StartGame", crawlSpeed, this);
                    UIHelper.SetButtonState("StartNewGame", false, UIManager.uiElements);
                    UIHelper.SetElementVisibility("StartNewGame", true, UIManager.uiElements);
                    UseMouse(kb, crawlSpeed);
                    UpdateCamera(mainMenuPos, 28);
                    if (currButtons.Start == ButtonState.Pressed && prevButtons.Start == ButtonState.Released ||
                        currButtons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released && MenuButtonIndex == 0)
                    {
                        ChangeGameState(Game1.Scenes.InGame);
                        UIHelper.SetElementVisibility("MainMenu", false,UIManager.uiElements);
                        UIHelper.SetElementVisibility("Settings", false,UIManager.uiElements);
                        UIHelper.SetElementVisibility("StartNewGame", false,UIManager.uiElements);
                        UIHelper.SetElementVisibility("LoadGame", false, UIManager.uiElements);
                        UIHelper.SetElementVisibility("StartGameReturn", false, UIManager.uiElements);
                        LoadTutorial();
                    }

                    if (GamePad.GetState(0).DPad.Down == ButtonState.Pressed)
                    {
                        MenuButtonIndex = 1;
                    }
                    if (GamePad.GetState(0).DPad.Up == ButtonState.Pressed)
                    {
                        MenuButtonIndex = 0;
                    }

                    break;
                case MenuStates.Settings:
                    UIHelper.SetElementVisibility("Settings", true, UIManager.uiElements);
                   // UIManager.CreateKeyBindsUI(this);
                    UseMouse(kb, crawlSpeed);
                    UIManager.UpdateButton("Settings", crawlSpeed, this);
                    UIManager.UpdateTextBlock("Settings", Rectangle.Empty);
                    //UpdateCamera(new Vector2(graphics.PreferredBackBufferWidth * 1.5f, camera.Y), crawlSpeed);
                  
                    if(keyBindActive && kb.GetPressedKeys().Length > 0)
                    {
                        keyBindKey = kb.GetPressedKeys()[0];
                        UIHelper.SetButtonText(UIManager.uiElements[UIManager.currButton], keyBindKey.ToString());

                        if(UIManager.isTopDownKeys)
                        {
                            UIManager.TopDownInputs[UIManager.currButtonIndex] = keyBindKey;
                        }
                        else
                        {
                            UIManager.SideScrollInputs[UIManager.currButtonIndex] = keyBindKey;
                        }
                        //UIManager.SideScrollInputs[UIManager.currButtonIndex] = keyBindKey;
                        //changeKey = true;
                        keyBindActive = false;

                        
                    }

                    if (prevButtons.B == ButtonState.Pressed && currButtons.B == ButtonState.Released)
                    {
                        MenuState = MenuStates.MainMenu;
                        
                    }
                    break;
            }
           
        }

        private void UseMouse(KeyboardState kb, float crawlSpeed)
        {
            this.IsMouseVisible = true;
            ms = Mouse.GetState();

            UIManager.UpdateTextBlock("MainMenuTitle", Rectangle.Empty);

            //bool hitButton = false;

            if ((ms.X > 0) && (ms.Y > 0) &&
               (ms.X < camera.viewport.Width) &&
               (ms.Y < camera.viewport.Height))
            {
                mousePos = new Vector2((int)(ms.Position.X + (camera.Position.X - (camera.viewport.Width/2))),
                    (int)(ms.Position.Y + (camera.Position.Y - (camera.viewport.Height / 2)))); //Can be Moved to UIManager. Sending the mspos
                if (ms.RightButton == ButtonState.Released)
                {
                    if (ms.LeftButton == ButtonState.Released && prevMs.LeftButton == ButtonState.Pressed)
                    {
                        if (keyBindActive)
                            keyBindActive = false;
                        foreach (UIWidget widget in UIManager.uiElements.Values)
                        {
                            if (widget is UIButton)
                            {
                                if(graphics.IsFullScreen)
                                {
                                    ((UIButton)widget).HitTest(
                                new Point((int)mousePos.X, (int)mousePos.Y));
                                }
                                else
                                {
                                    ((UIButton)widget).HitTest(
                                new Point((int)mousePos.X, (int)mousePos.Y));
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        foreach (UIWidget widget in UIManager.uiElements.Values)
                        {
                            if (widget is UIButton)
                            {
                                ((UIButton)widget).Pressed = false;
                            }
                        }
                    }
                }
            }
            else
                mousePos = Vector2.Zero;


            //float reticleSpeed = 3f;

            GamePadState controller = GamePad.GetState(0);
            if(controller.ThumbSticks.Left != Vector2.Zero)
            {
                controllerReticle.visible = true;
            }

            if(controller.ThumbSticks.Left.X < 0 && controller.ThumbSticks.Left.X > -.4f)
            {
                controllerReticle.pos.X -= (int)controllerReticle.moveSpeed / 2;
            }
            else if(controller.ThumbSticks.Left.X < 0)
            {
                controllerReticle.pos.X -= (int)controllerReticle.moveSpeed;
            }

            if (controller.ThumbSticks.Left.X > 0 && controller.ThumbSticks.Left.X < .4f)
            {
                controllerReticle.pos.X += (int)controllerReticle.moveSpeed / 2;
            }
            else if (controller.ThumbSticks.Left.X > 0)
            {
                controllerReticle.pos.X += (int)controllerReticle.moveSpeed;
            }

            if(controller.ThumbSticks.Left.Y < 0 && controller.ThumbSticks.Left.Y > -.4f)
            {
                controllerReticle.pos.Y += (int)controllerReticle.moveSpeed / 2;
            }
            else if(controller.ThumbSticks.Left.Y < 0)
            {
                controllerReticle.pos.Y += (int)controllerReticle.moveSpeed;
            }

            if(controller.ThumbSticks.Left.Y > 0 && controller.ThumbSticks.Left.Y < .4f)
            {
                controllerReticle.pos.Y -= (int)controllerReticle.moveSpeed / 2;
            }
            else if(controller.ThumbSticks.Left.Y > 0)
            {
                controllerReticle.pos.Y -= (int)controllerReticle.moveSpeed;
            }



            if(controller.Buttons.A == ButtonState.Pressed && prevButtons.A == ButtonState.Released)
            {
                foreach (UIWidget widget in UIManager.uiElements.Values)
                {
                    if (widget is UIButton)
                    {

                        ((UIButton)widget).HitTest(
                    new Point(controllerReticle.rect.Center.X, controllerReticle.rect.Center.Y));


                    }
                }
            }
            //if (kb.IsKeyDown(Keys.Right))
            //{
            //    camera.Update(new Vector2(camera.X + crawlSpeed, camera.Y));
            //}

            //if (kb.IsKeyDown(Keys.Left))
            //{
            //    camera.Update(new Vector2(camera.X - crawlSpeed, camera.Y));
            //}
            controllerReticle.Update();

            if(controllerReticle.rect.Right > graphics.PreferredBackBufferWidth)
            {
                controllerReticle.rect.X = graphics.PreferredBackBufferWidth - controllerReticle.rect.Width;
                controllerReticle.pos.X = controllerReticle.rect.X;
            }
            else if(controllerReticle.rect.X < 0)
            {
                controllerReticle.rect.X = 0;
                controllerReticle.pos.X = 0;
            }

            if(controllerReticle.rect.Y < 0)
            {
                controllerReticle.rect.Y = 0;
                controllerReticle.pos.Y = 0;
            }
            else if(controllerReticle.rect.Bottom > graphics.PreferredBackBufferHeight)
            {
                controllerReticle.rect.Y = graphics.PreferredBackBufferHeight - controllerReticle.rect.Height;
                controllerReticle.pos.Y = controllerReticle.rect.Y;
            }
           
            prevButtons = controller.Buttons;
            prevMs = ms;
        }

        public void ChangeMenuState(MenuStates menuState)
        {
            MenuState = menuState;
        }

        public void ChangeGameState(Scenes gamestate)
        {
            currScene = gamestate;
        }



        void UpdateCamera(Vector2 pos, float moveSpeed)
        {
            bool reached = true;

            if (camera.X < pos.X)
            {
                camera.Update(new Vector2(camera.X + moveSpeed, camera.Y));
                reached = false;
            }
            if (camera.X > pos.X)
            {
                camera.Update(new Vector2(camera.X - moveSpeed, camera.Y));
                reached = false;
            }
            if (camera.Y > pos.Y)
            {
                camera.Update(new Vector2(camera.X, camera.Y - moveSpeed));
                reached = false;
            }
            if (camera.Y < pos.Y)
            {
                camera.Update(new Vector2(camera.X, camera.Y + moveSpeed));
                reached = false;
            }

            if(reached)
                graphics.ApplyChanges();
        }

        void TitleCrawl(float crawlSpeed)
        {
            //camera.Update(new Vector2(camera.X, camera.Y + crawlSpeed));
            
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

        public void SaveGame()
        {
            //PlayerPosition TD && SS, BossKillCount, GameState, KeyBinds, ViewRect 

            Vector2 ssPlayerPos = new Vector2((int)ssPlayer.position.X, (int)ssPlayer.position.Y); //Saves player position
            int killedBosses = bossKillCount; //Bosses killed (Used with weapon wheel and to determine where in the game you are

            GameStates currGameState = GameState; // Current GameState

            List<Keys> SideScrollInputs = UIManager.SideScrollInputs; //Side Scroll Inputs
            List<Keys> TopDownInputs = UIManager.TopDownInputs; // Top Down Inputs

            Rectangle sideScrollView;
            Rectangle topDownView;


            sideScrollView = ssCamera.CameraBounds; //Current position at time of save
            topDownView = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)); //The position that the first level will be drawn (constant on first load of a dungeon)

            //Saving non key bind data do a list of strings
            List<string> savedData = new List<string>();
            savedData.Add(currGameState.ToString()); //0
            savedData.Add(ssPlayerPos.X.ToString());//1
            savedData.Add(ssPlayerPos.Y.ToString());//2

            savedData.Add(killedBosses.ToString());//3

            savedData.Add(sideScrollView.Center.X.ToString());//4
            savedData.Add(sideScrollView.Center.Y.ToString());//5
            savedData.Add(SideTileMap.GetWorldDims().X.ToString());//6
            savedData.Add(SideTileMap.GetWorldDims().Y.ToString());//7

            savedData.Add(openDoorCount.ToString());//8
            savedData.Add(levelCount.ToString());//9
            savedData.Add((ssCamera.ViewRect.Width / 2).ToString());//10
            savedData.Add(dungeonNum.ToString());//11

            savedData.Add(UIManager.MasterVolume.ToString());//12
            savedData.Add(UIManager.MusicVolume.ToString());//13
            savedData.Add(UIManager.EffectVolume.ToString());//14
            savedData.Add(ssPlayer.Health.ToString()); //15
            //savedData.Add(fadePos.X.ToString());
            //savedData.Add(fadePos.Y.ToString());
            //savedData.Add(topDownView.X.ToString());
            //savedData.Add(topDownView.Y.ToString());
            //savedData.Add(topDownView.Width.ToString());
            //savedData.Add(topDownView.Height.ToString());

            //Saving the data
            using (var sw = new StreamWriter(System.IO.File.Create(dataPath)))
            {
                for (int i = 0; i < savedData.Count; i++)
                {
                    sw.Write(savedData[i] + "\n");
                }

                sw.Flush();

                sw.Close();
            }

            SaveKeyBinds(SideScrollInputs, TopDownInputs);

        }

        public void SaveKeyBinds(List<Keys> SideScrollInputs, List<Keys> TopDownInputs)
        {
            //The same to the Key Binds
            List<string> sideInputData = new List<string>();
            List<string> topInputData = new List<string>();
            for (int i = 0; i < SideScrollInputs.Count; i++)
            {
                sideInputData.Add(SideScrollInputs[i].ToString());
            }
            for (int i = 0; i < TopDownInputs.Count; i++)
            {
                topInputData.Add(TopDownInputs[i].ToString());
            }

            //Saving Side Key Binds

            try
            {
                using (var sw = new StreamWriter(System.IO.File.Create(SideInputPath)))
                {
                    for (int i = 0; i < sideInputData.Count; i++)
                    {
                        sw.Write(sideInputData[i] + "\n");
                    }

                    sw.Flush();

                    sw.Close();
                }

                //Saving TopDown Key Binds
                using (var sw = new StreamWriter(System.IO.File.Create(TopDownInputPath)))
                {
                    for (int i = 0; i < topInputData.Count; i++)
                    {
                        sw.Write(topInputData[i] + "\n");
                    }

                    sw.Flush();

                    sw.Close();
                }
            }
            catch
            { 
                
            }
          
        }

        public List<Keys> GetSideScrollInputs()
        {
            List<Keys> inputs = new List<Keys>();
            string[] sideInputData = File.ReadAllLines(SideInputPath);

            for(int i = 0; i < sideInputData.Length; i++)
            {
                inputs.Add((Keys)Enum.Parse(typeof(Keys), sideInputData[i]));
            }

            return inputs;
        }

        public List<Keys> GetTopDownInputs()
        {
            List<Keys> inputs = new List<Keys>();
            string[] topDownInputData = File.ReadAllLines(TopDownInputPath);

            for (int i = 0; i < topDownInputData.Length; i++)
            {
                inputs.Add((Keys)Enum.Parse(typeof(Keys), topDownInputData[i]));
            }

            return inputs;
        }

        public void LoadGame(bool restart = false)
        {
            if(System.IO.File.Exists(dataPath))
            {
                isLoadedGame = true;
                
                string[] gameData = File.ReadAllLines(dataPath); //Data File: CurrGameState , ssPlayerX, ssPlayerY, killedBosses, Camera XYWH |||| Input Files: SideInputs, TopInputs
                string[] sideInputData = File.ReadAllLines(SideInputPath);
                string[] topInputData = File.ReadAllLines(TopDownInputPath);


                object gameState = Enum.Parse(typeof(GameStates), gameData[0]);

                bossKillCount = int.Parse(gameData[3]);

                for(int i = 0; i < sideInputData.Length; i++)
                {
                    UIManager.SideScrollInputs[i] = (Keys)Enum.Parse(typeof(Keys), sideInputData[i]);
                }
                for(int i = 0; i < topInputData.Length; i++)
                {
                    UIManager.TopDownInputs[i] = (Keys)Enum.Parse(typeof(Keys), topInputData[i]);
                }

                if (!isLoadedGame)
                {
                    soundManager.ChangeVolume(float.Parse(gameData[12]), float.Parse(gameData[14]), float.Parse(gameData[13]));
                    //UIManager.MasterVolume = float.Parse(gameData[12]);
                    //UIManager.EffectVolume = float.Parse(gameData[14]);
                    //UIManager.MusicVolume = float.Parse(gameData[13]);
                }
                else if (isLoadedGame)
                    soundManager.ChangeVolume(UIManager.MasterVolume, UIManager.EffectVolume, UIManager.MusicVolume);

                UIHelper.SetElementText(UIManager.uiElements["VolumeSettingsMasterNum"], ((int)(UIManager.MasterVolume * 100)).ToString());
                UIHelper.SetElementText(UIManager.uiElements["VolumeSettingsEffectsNum"], ((int)(UIManager.EffectVolume * 100)).ToString());
                UIHelper.SetElementText(UIManager.uiElements["VolumeSettingsMusicNum"], ((int)(UIManager.MusicVolume * 100)).ToString());
                //ssPlayer.Load(Content, Window.ClientBounds, friction, new Vector2(int.Parse(gameData[1]), int.Parse(gameData[2])), UIManager);
               

                //ssCamera = new SSCamera(GraphicsDevice.Viewport, new Vector2(int.Parse(gameData[4]), int.Parse(gameData[5])), int.Parse(gameData[6]), int.Parse(gameData[7]));
                
                if((GameStates)gameState != GameStates.Tutorial)
                {
                    StartNewGame(true, true);
                    ssPlayer.Load(Content, Window.ClientBounds, friction, new Vector2(int.Parse(gameData[1]), int.Parse(gameData[2])), UIManager);

                    if(isLoadedGame && ssPlayer.Health <= 0)
                    {
                        ssPlayer.Health = 10;
                    }
                    else if(!isLoadedGame)
                    {
                        ssPlayer.Health = float.Parse(gameData[15]);
                        if (ssPlayer.Health <= 0)
                        {
                            ssPlayer.Health = 10;
                        }
                    }
                  
                 
                    //ssCamera = new SSCamera(GraphicsDevice.Viewport, new Vector2(int.Parse(gameData[4]), int.Parse(gameData[5])), int.Parse(gameData[6]), int.Parse(gameData[7]));
                    openDoorCount = int.Parse(gameData[8]);
                    if (openDoorCount >= 0)
                    {
                        ssCamera = new SSCamera(GraphicsDevice.Viewport, new Vector2(int.Parse(gameData[4]) /*+ (int.Parse(gameData[10]))*/, int.Parse(gameData[5])), int.Parse(gameData[6]), int.Parse(gameData[7]));
                        //ssCamera.SetBounds(new Rectangle(int.Parse(gameData[4]), int.Parse(gameData[5]), int.Parse(gameData[6]), int.Parse(gameData[7])));

                        if (openDoorCount == 1)
                            fadePos = new Vector2(int.Parse(gameData[1]) - (int.Parse(gameData[10])/2), int.Parse(gameData[2]));
                        else if (openDoorCount == 4)
                            fadePos = new Vector2(int.Parse(gameData[1]) - int.Parse(gameData[10]), int.Parse(gameData[2]));
                        else
                            fadePos = new Vector2(int.Parse(gameData[1])/* + int.Parse(gameData[10])*/, int.Parse(gameData[2]));

                        foreach (BottomDoorTile doorTile in SideTileMap.BottomDoorTiles)
                        {
                            if (openDoorCount >= 2 && MathHelper.Distance(doorTile.Rectangle.Center.X, ssPlayer.position.X) < 200 )
                                fadePos = new Vector2(int.Parse(gameData[1]) + (int.Parse(gameData[10]) - 66), int.Parse(gameData[2]));
                            //else if (openDoorCount == 1 && MathHelper.Distance(doorTile.Rectangle.Center.X, ssPlayer.position.X) < 200)
                            //    fadePos = new Vector2(int.Parse(gameData[1])/* + int.Parse(gameData[10])*/, int.Parse(gameData[2]));
                        }

                      
                    }
                    reloadCount++;
                    soundManager.ClearSounds();
                    dungeonNum = int.Parse(gameData[11]);
                    if (((GameStates)gameState) == GameStates.TopDown)
                    {
                        int levelCount = int.Parse(gameData[9]);
                        tdPlayer = new TDPlayer(this, 64, 200, 200, UIManager, Content, dungeonNum);
                        if (levelCount > tdPlayer.bossRoom)
                            this.levelCount = levelCount - 1;
                        else
                            this.levelCount = 0;
                        
                        
                        StartDungeon(true);
                        //soundManager.AddSound("BossTheme", true);

                    }
                    else if ((GameStates)gameState == GameStates.FinalBoss)
                    {
                        LoadFinalBoss();
                    }
                }
                else if((GameStates)gameState == GameStates.Tutorial)
                {
                    LoadTutorial();
                }
                
            }
        }

        public void UpdateSounds()
        {
            soundManager.ChangeVolume(UIManager.MasterVolume, UIManager.EffectVolume, UIManager.MusicVolume);
        }
    }

    class ControllerReticle
    {
        public Rectangle rect;
        public float moveSpeed = 5f;
        public Texture2D texture;
        public bool visible = false;
        public Point pos;

        public ControllerReticle(Rectangle rect, Texture2D texture)
        {
            this.rect = rect;
            this.texture = texture;
            pos = new Point(rect.X, rect.Y);
        }

        public void Update()
        {
            if(visible)
                rect = new Rectangle(pos.X, pos.Y, rect.Width, rect.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(visible)
                spriteBatch.Draw(texture, rect, Color.White);
        }
    }
}
