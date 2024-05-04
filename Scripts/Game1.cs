using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using AUTO_Matic.SideScroll;
using AUTO_Matic.TopDown;
using System;
using AUTO_Matic.Scripts.TopDown;
using System.Threading;

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

        UIManager UIManager = new UIManager();

        public enum Scenes { TitleScreen, InGame, Exit }
        public Scenes currScene = Scenes.TitleScreen;

        public enum GameStates { SideScroll, TopDown, Paused}
        public GameStates GameState = GameStates.SideScroll;

        public enum MenuStates { TitleCrawl, MainMenu, Settings, StartGame}
        public MenuStates MenuState = MenuStates.TitleCrawl;
        bool startCrawl = false;

        Vector2 mainMenuPos;

        KeyboardState prevKB;

        Point screenCenter;
        Point saveMousePoint;
        MouseState ms;
        MouseState prevMs;
        Vector2 mousePos = Vector2.Zero;
        int count = 0;

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

        #region TopDown
        TopDownMap tdMap;
        TDPlayer tdPlayer;
        public List<Vector2> BoundIndexes = new List<Vector2>();
        int pixelBits = 64;
        List<TDEnemy> tdEnemies = new List<TDEnemy>();
        public int levelCount = 0;

        public Rectangle Boss;
        float shootRate = .5f;
        float maxShootRate;
        bool canShoot = false;
        List<Bullet> bossBullets = new List<Bullet>();
        float bulletSpeed = 6.25f;
        float maxBulletSpeed = 20;
        float bulletDmg = .5f;
        float bossHealth = 8.65f;
        #endregion
        //SSEnemy enemy;

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
            graphics.PreferredBackBufferWidth = 1920;/*(int)(graphics.PreferredBackBufferWidth * 1.5f)*/
            graphics.PreferredBackBufferHeight = 1080;/*(int)(graphics.PreferredBackBufferHeight * 1.5f);*/

            //graphics.HardwareModeSwitch = false;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            maxShootRate = shootRate;
            camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2));

            
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
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region UI Menus
            UIHelper.TitleFont = Content.Load<SpriteFont>(@"Fonts\TitleFont");

            UIHelper.ButtonTexture = Content.Load<Texture2D>(@"Textures\Button");
            UIHelper.ButtonFont = Content.Load<SpriteFont>(@"Fonts\CrawlFont");
            UIHelper.CrawlBgTxture = Content.Load<Texture2D>(@"Textures\TitleCrawlBG");
            UIHelper.Bounds = new Rectangle(new Point((int)camera.Position.X, (int)camera.Position.Y), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            UIHelper.CrawlFont = Content.Load<SpriteFont>(@"Fonts\CrawlFont");
            UIManager.CreateUIElements(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), this);
            UIHelper.SetElementVisibility("TitleCrawl", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("MainMenuTitle", true, UIManager.uiElements);

            UIHelper.SetElementVisibility("SettingsMenuTitle", true, UIManager.uiElements);
            #endregion

            //ssPlayer.Load(Content, Window.ClientBounds, friction);
            if(currScene == Scenes.InGame)
                StartNewGame();
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
            if(!isBoss)
            {
                Random rand = new Random();
                int num = rand.Next(1, 11);

                string filePath = Content.RootDirectory + "/TopDown/Maps/Map" + num + ".txt";

                if (xLevel)
                    tdPlayer.PosXLevels.xLevels.Add(tdMap.GenerateMap(filePath));
                if (yLevel)
                    tdPlayer.PosYLevels.yLevels.Add(tdMap.GenerateMap(filePath));
                if (dLevel)
                    tdPlayer.DiagLevels.dLevels.Add(tdMap.GenerateMap(filePath));

                tdEnemies.Clear();

                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points);

                Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                    (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                    new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
                foreach (Vector2 enemySpawn in tdMap.EnemySpawns)
                {
                    if (currBounds.Contains(enemySpawn))
                        tdEnemies.Add(new TDEnemy(Content, enemySpawn, tdMap, tdMap.GenerateMap(filePath)));
                }
            }
            if(isBoss)
            {
                string filePath = Content.RootDirectory + "/TopDown/Maps/Map" + 0 + ".txt";

                if (xLevel)
                    tdPlayer.PosXLevels.xLevels.Add(tdMap.GenerateMap(filePath));
                if (yLevel)
                    tdPlayer.PosYLevels.yLevels.Add(tdMap.GenerateMap(filePath));
                if (dLevel)
                    tdPlayer.DiagLevels.dLevels.Add(tdMap.GenerateMap(filePath));

                tdEnemies.Clear();

                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points);

                Rectangle currBounds = new Rectangle(new Point((0) + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)),
                    (0) - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))),
                    new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
                foreach (Vector2 enemySpawn in tdMap.EnemySpawns)
                {
                    if (currBounds.Contains(enemySpawn))
                        Boss = new Rectangle((int)enemySpawn.X, (int)enemySpawn.Y, 320, 320);
                }

                canShoot = true;
            }
        }


        public void StartDungeon()
        {
            tdPlayer = new TDPlayer(this, 64, 12, 12);
            tdMap = new TopDownMap();
            Boss = new Rectangle();
            levelCount = 0;

            graphics.PreferredBackBufferWidth = 64 * 25; //1600  // pixelBits * col
            graphics.PreferredBackBufferHeight = 64 * 15; //960  // pixelBits * row
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)), graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))));
            camera.Zoom = 1f;
            Tiles.Content = Content;

            if (tdPlayer.levelInX == 1 && tdPlayer.levelInY == 1)
            {
                GenerateNewMap(true, false, false, false);
            }
            else if (tdPlayer.levelInX > 1 && tdPlayer.levelInY == 1)
            {
                GenerateNewMap(true, false, false, false);
            }
            else if (tdPlayer.levelInY > 1 && tdPlayer.levelInX == 1)
            {
                GenerateNewMap(false, true, false, false);
            }
            else if (tdPlayer.levelInX > 1 && tdPlayer.levelInY > 1)
            {
                GenerateNewMap(false, false, true, false);
            }
            BoundIndexes.Clear();
            BoundIndexes.Add(camera.Position);
            tdPlayer.Load(Content, camera.viewport.Bounds);
            tdPlayer.position = new Vector2((graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)) + (64 * 2), -(graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1)) + (64 * 2));
        }

        public void StartNewGame()
        {
            camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth / 2 - (64*3.5f), graphics.PreferredBackBufferHeight / 2));
            this.IsMouseVisible = false;
            Tile.Content = Content;

            string filePath = Content.RootDirectory + "/SideScroll/Maps/Map0.txt";
            SideTileMap.LoadMap(filePath);
            enemies.Clear();
            for(int i = 0; i < SideTileMap.enemySpawns.Count - 1; i++)
            {
                enemies.Add(new SSEnemy(Content, Window.ClientBounds, 5, SideTileMap.enemySpawns[i], true));
            }

            camera.Zoom = 1.35f;

            ssPlayer.Load(Content, Window.ClientBounds, friction, SideTileMap.playerSpawns[0]);
            healthBar.Clear();
            for (int i = 0; i < 3; i++)
            {
                healthBar.Add(new Rectangle(25 + (i * 96), 136, 96, 64));
            }
            
            //enemy = new SSEnemy(Content, GraphicsDevice.Viewport.Bounds, 5);
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
            //pos = new Vector2(Window.ClientBounds.Width / 2 + (Window.ClientBounds.Width * (levelInX - 1)), graphics.PreferredBackBufferHeight / 2 + (graphics.PreferredBackBufferHeight * (levelInY - 1)));
            if ((graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)) > pos.X))
            {
                pos.X += graphics.PreferredBackBufferWidth / pixelBits;
            }
            else if ((graphics.PreferredBackBufferWidth / 2 + (graphics.PreferredBackBufferWidth * (tdPlayer.levelInX - 1)) < pos.X))
            {
                pos.X -= graphics.PreferredBackBufferWidth / pixelBits;
            }
            if (((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1))) < pos.Y))
            {
                pos.Y -= graphics.PreferredBackBufferHeight / pixelBits;
            }
            else if ((graphics.PreferredBackBufferHeight / 2 - (graphics.PreferredBackBufferHeight * (tdPlayer.levelInY - 1)) > pos.Y))
            {
                pos.Y += graphics.PreferredBackBufferHeight / pixelBits;
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


            return pos;


        }


       
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState kb = Keyboard.GetState();

            switch(currScene)
            {
                case Scenes.TitleScreen:
                    Menus(kb);
                    break;
                case Scenes.InGame:
                    camera.Update(new Vector2(camera.X, camera.Y));
                    switch(GameState)
                    {
                        
                        case GameStates.SideScroll: //Default
                            if(kb.IsKeyDown(Keys.P))//Reset Pos
                            {
                                StartNewGame();

                            }
                            if(updateDoor)
                            {
                                UpdateDoor(topIndex, bottomIndex);
                            }
                            ssPlayer.Update(gameTime, Gravity, enemies);
                            if(ssPlayer.Position.X > 1430)
                            {
                                if(camera.X <= graphics.PreferredBackBufferWidth)
                                {
                                    camera.X += 64;
                                }
                                //camera.X = graphics.PreferredBackBufferWidth;
                                //if(healthBar[0].X == ssPlayer.X && ssPlayer.X > 1435)
                                //{

                                //}
                                //else
                                //{
                                //    if(healthBar.Count != 0)
                                //    {
                                //        for (int i = 0; i < healthBar.Count - 1; i++)
                                //        {
                                //            healthBar[i] = new Rectangle(1430, 120, 96, 64);
                                //        }
                                //    }
                                   
                                //}
                               
                            }
                            if(ssPlayer.Position.X > 2610)
                            {
                                if(camera.X <= 2610)
                                {
                                    camera.X += 64;
                                }
                                //camera.X = graphics.PreferredBackBufferWidth + 64 * 7;
                                //if (healthBar[0].X == ssPlayer.X && ssPlayer.X > 2615)
                                //{

                                //}
                                //else
                                //{
                                //    if (healthBar.Count != 0)
                                //    {
                                //        for (int i = 0; i < healthBar.Count - 1; i++)
                                //        {
                                //            healthBar[i] = new Rectangle(2610, 120, 96, 64);
                                //        }
                                //    }

                                //}

                            }

                            for(int i = 0; i < enemies.Count; i++)
                            {
                                enemies[i].Update(gameTime, Gravity, ssPlayer, this);
                                for (int j = i + 1; j < enemies.Count; j++)
                                {
                                    if(enemies[i].enemyRect.Intersects(enemies[j].enemyRect))
                                    {
                                        if (enemies[i].velocity.X == enemies[j].velocity.X)
                                            enemies[i].velocity.X /= 2;
                                        else
                                        {
                                            if(enemies[i].velocity.X != enemies[i].maxRunSpeed || enemies[i].velocity.X != -enemies[i].maxRunSpeed)
                                            {
                                                if (enemies[i].velocity.X < 0)
                                                    enemies[i].velocity.X = -enemies[i].maxRunSpeed;
                                                else if (enemies[i].velocity.X > 0)
                                                    enemies[i].velocity.X = enemies[i].maxRunSpeed;

                                            }
                                        }
                                            
                                    }
                                   
                                }
                            }
                            //foreach(SSEnemy enemy in enemies)
                            //{
                            //    enemy.Update(gameTime, Gravity, ssPlayer, this);
                            //}
                            //enemy.Update(gameTime, Gravity, ssPlayer);

                            ssPlayer.blockBottom = false;

                          
                            foreach (GroundTile tile in SideTileMap.GroundTiles)
                            {
                                ssPlayer.Collision(tile.Rectangle);
                                
                            }
                            foreach (TopDoorTile tile in SideTileMap.TopDoorTiles)
                            {
                                ssPlayer.Collision(tile.Rectangle);

                            }
                            foreach (BottomDoorTile tile in SideTileMap.BottomDoorTiles)
                            {
                                ssPlayer.Collision(tile.Rectangle);

                            }
                            foreach (PlatformTile tile1 in SideTileMap.PlatformTiles)
                            {
                                ssPlayer.Collision(tile1.Rectangle);
                            }
                            if (ssPlayer.blockBottom == false && ssPlayer.playerState != SSPlayer.PlayerStates.Jumping)
                            {
                                ssPlayer.isFalling = true;

                            }
                            if(kb.IsKeyDown(Keys.RightShift))
                            {
                                StartDungeon();
                                GameState = GameStates.TopDown;
                            }
                            break;
                        case GameStates.TopDown:
                            tdPlayer.Update(gameTime, tdMap);
                            camera.Update(CameraPos());
                            if(kb.IsKeyDown(Keys.L))//Switching back to sidescroll
                            {
                                GameState = GameStates.SideScroll;
                                //Camera position not updated
                            }
                            if(tdPlayer.changeLevel)
                            {
                                tdMap.Refresh(tdPlayer.PosXLevels.xLevels, tdPlayer.PosYLevels.yLevels, tdPlayer.DiagLevels.dLevels, 64, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight,
                                    tdPlayer.PosXLevels.Points, tdPlayer.PosYLevels.Points, tdPlayer.DiagLevels.Points);
                                tdPlayer.changeLevel = false;
                                levelCount++;
                            }
                            
                            for(int i = tdEnemies.Count - 1; i >= 0; i--)
                            {
                                tdEnemies[i].Upate(gameTime, tdPlayer.rectangle);
                            }

                            if(tdPlayer.bullets.Count != 0)
                            {
                                for(int i = tdPlayer.bullets.Count - 1; i >=0; i--)
                                {
                                    tdPlayer.bullets[i].Update();
                                }
                            }

                            foreach(WallTiles tile in tdMap.WallTiles)
                            {
                                if (tdPlayer.bullets.Count != 0)
                                {
                                    for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                    {
                                        if(tdPlayer.bullets[i].rect.Intersects(tile.Rectangle))
                                        {
                                            tdPlayer.bullets.RemoveAt(i);
                                        }
                                    }
                                }
                            }

                            if(tdEnemies.Count != 0)
                            {
                                for (int j = tdEnemies.Count - 1; j >= 0; j--)
                                {
                                    if (tdPlayer.bullets.Count != 0)
                                    {
                                        for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                        {
                                            if (tdPlayer.bullets[i].rect.Intersects(tdEnemies[j].Rectangle))
                                            {
                                                tdEnemies[j].Health -= tdPlayer.bulletDmg;
                                                if(tdEnemies[j].Health <= 0)
                                                {
                                                    tdEnemies.RemoveAt(j);
                                                    break;
                                                }
                                                tdPlayer.bullets.RemoveAt(i);
                                            }
                                        }
                                    }
                                }
                            }
                            if(!canShoot)
                            {
                                shootRate -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                                if(shootRate < 0)
                                {
                                    shootRate = maxShootRate;
                                    canShoot = true;
                                }
                            }

                            if(canShoot)
                            {
                                if(tdPlayer.position.X < Boss.X + Boss.Width/2)
                                {
                                    bossBullets.Add(new Bullet(new Vector2(Boss.X + Boss.Width / 2, Boss.Y + Boss.Height / 2), -bulletSpeed, new Vector2(-maxBulletSpeed, maxBulletSpeed), Content, true, 64 * 6));
                                }
                                if(tdPlayer.position.X > Boss.X + Boss.Width/2)
                                {
                                    bossBullets.Add(new Bullet(new Vector2(Boss.X + Boss.Width / 2, Boss.Y + Boss.Height / 2), bulletSpeed, new Vector2(maxBulletSpeed, maxBulletSpeed), Content, true, 64 * 6));
                                }
                                if(tdPlayer.position.Y < Boss.Y + Boss.Height/2)
                                {
                                    bossBullets.Add(new Bullet(new Vector2(Boss.X + Boss.Width / 2, Boss.Y + Boss.Height / 2), -bulletSpeed, new Vector2(maxBulletSpeed, -maxBulletSpeed), Content, false, 64 * 6));
                                }
                                if (tdPlayer.position.Y > Boss.Y + Boss.Height / 2)
                                {
                                    bossBullets.Add(new Bullet(new Vector2(Boss.X + Boss.Width / 2, Boss.Y + Boss.Height / 2), bulletSpeed, new Vector2(maxBulletSpeed, maxBulletSpeed), Content, false, 64 * 6));
                                }

                                canShoot = false;
                            }

                            foreach(Bullet bullet in bossBullets)
                            {
                                bullet.Update();
                            }


                            for(int i = bossBullets.Count - 1; i >= 0; i--)
                            {
                                if(bossBullets[i].rect.Intersects(tdPlayer.rectangle))
                                {
                                    tdPlayer.Health -= bulletDmg;

                                    if(tdPlayer.Health <= 0)
                                    {
                                        StartDungeon();
                                    }

                                    bossBullets.RemoveAt(i);
                                }
                            }
                           
                            if(levelCount >= tdPlayer.bossRoom)
                            {
                                for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
                                {
                                    if (tdPlayer.bullets[i].rect.Intersects(Boss))
                                    {
                                         bossHealth -= tdPlayer.bulletDmg;
                                        if (bossHealth <= 0)
                                        {
                                            Boss = new Rectangle();
                                            break;
                                        }
                                        tdPlayer.bullets.RemoveAt(i);
                                    }
                                }
                            }
                            
                            break;
                    }

                    break;

            }

           

            // TODO: Add your update logic here
            base.Update(gameTime);
        }


        public void OpenDoor(Rectangle bottomTile)
        {
            //Find top door
            int bottomTileIndex = 0;
            int topTileIndex = 0;
            for(int i = 0; i < SideTileMap.BottomDoorTiles.Count; i++)
            {
                if(SideTileMap.BottomDoorTiles[i].Rectangle == bottomTile)
                {
                    bottomTileIndex = i;
                }
            }
            bottomTile.Y -= pixelBits;
            for (int i = 0; i < SideTileMap.TopDoorTiles.Count; i++)
            {
                if(SideTileMap.TopDoorTiles[i].Rectangle == bottomTile)
                {
                    topTileIndex = i;
                }    
            }
            bottomIndex = bottomTileIndex;
            topIndex = topTileIndex;

            updateDoor = true;
        }

        void UpdateDoor(int topTile, int bottomTile)
        {
            SideTileMap.TopDoorTiles[topTile].setY((SideTileMap.TopDoorTiles[topTile].Rectangle.Y - doorOpenSpeed));
            SideTileMap.BottomDoorTiles[bottomTile].setY((SideTileMap.BottomDoorTiles[bottomTile].Rectangle.Y + doorOpenSpeed));

            if (MathHelper.Distance(SideTileMap.TopDoorTiles[topTile].Rectangle.Bottom, SideTileMap.BottomDoorTiles[bottomTile].Rectangle.Top) > pixelBits * 2)
            {
                updateDoor = false;
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Window.Title = camera.Position.ToString() + "   MousePos: " + mousePos.ToString();
            switch(currScene)
            {
                case Scenes.TitleScreen:
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

                    UIManager.Draw(spriteBatch);

                    spriteBatch.End();
                    break;
                case Scenes.InGame:
                    if(GameState == GameStates.SideScroll)
                    {
                        Window.Title = camera.Position.ToString() + " PlayerPos: " + ssPlayer.Position.ToString() + "  EnemyState0: " + enemies[0].enemyState.ToString() + "    EnemeyState1: " + enemies[1].enemyState.ToString();
                        //Window.Title = "Gravity: " + Gravity.Y.ToString() /*+ "  a = " + ((decimal)ssPlayer.Acceleration) + "   F = " + ((decimal)ssPlayer.Force) + " Friction = " + ssPlayer.friction */+ "   Vel = " + enemy.Velocity.ToString() + "   onPlatform = " + enemy.onPlatform + "   enemyState = " + enemy.enemyState.ToString();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

                        SideTileMap.Draw(spriteBatch);
                        ssPlayer.Draw(spriteBatch);
                        foreach(SSEnemy enemy in enemies)
                        {
                            enemy.Draw(spriteBatch);
                        }
                        //for(int i = 0; i < healthBar.Count; i++)
                        //{
                        //    spriteBatch.Draw(Content.Load<Texture2D>("TopDown/Textures/Player"), new Vector2(healthBar[i].X, healthBar[i].Y), healthBar[i], Color.Red);
                        //}
                       
                        //enemy.Draw(spriteBatch);
                        spriteBatch.End();
                    }
                    else if(GameState == GameStates.TopDown)
                    {
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                        tdMap.Draw(spriteBatch);
                        tdPlayer.Draw(spriteBatch);
                        foreach(TDEnemy enemy in tdEnemies)
                        {
                            enemy.Draw(spriteBatch);
                        }
                        foreach(Bullet bullet in bossBullets)
                        {
                            bullet.Draw(spriteBatch);
                        }
                        spriteBatch.Draw(Content.Load<Texture2D>("TopDown/MapTiles/Tile11"), Boss, Color.White);
                        spriteBatch.End();

                        //if (changeLevel)
                        //    map.Refresh(PosXLevel.xLevels, PosYLevel.yLevels, Diagonal.dLevels, pixelBits, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, PosXLevel.Points, PosYLevel.Points, Diagonal.Points);
                    }
                   
                    break;
            }


            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        private void Menus(KeyboardState kb)
        {
            float crawlSpeed = 3f;


            switch (MenuState)
            {
                case MenuStates.TitleCrawl:
                    if (kb.IsKeyDown(Keys.Enter) && prevKB.IsKeyUp(Keys.Enter) && !startCrawl)
                    {
                        prevKB = kb;
                        startCrawl = true;
                    }
                    UIManager.UpdateTextBlock("TitleCrawl");
                    if (startCrawl)
                    {
                        TitleCrawl(crawlSpeed);
                        if (kb.IsKeyDown(Keys.G) && prevKB.IsKeyUp(Keys.G))
                        {
                            camera.Update(new Vector2(camera.X, (UIHelper.GetElementBGRect(UIManager.uiElements["TitleCrawl"]).Bottom + (graphics.PreferredBackBufferHeight / 2))));
                            UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);
                            MenuState = MenuStates.MainMenu;
                            startCrawl = false;
                            prevKB = kb;
                        }
                    }


                    if (camera.Y >= (UIHelper.GetElementBGRect(UIManager.uiElements["TitleCrawl"]).Bottom + (graphics.PreferredBackBufferHeight / 2)) && startCrawl)
                    {
                        MenuState = MenuStates.MainMenu;
                        //screenCenter.X = this.Window.ClientBounds.Width / 2;
                        //screenCenter.Y = this.Window.ClientBounds.Height / 2;

                        UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);
                        mainMenuPos = camera.Position;
                        mainMenuPos.Y = (UIHelper.GetElementBGRect(UIManager.uiElements["TitleCrawl"]).Bottom + (graphics.PreferredBackBufferHeight / 2));
                        //break;
                    }
                    break;
                case MenuStates.MainMenu:
                    if (count == 0)
                    {
                        mainMenuPos = camera.Position;
                        count++;
                    }
                    UIHelper.SetElementVisibility("MainMenu", true, UIManager.uiElements);
                    UseMouse(kb, crawlSpeed);
                    UpdateCamera(mainMenuPos, 6);
                    UIManager.UpdateButton("MainMenu", 6);
                    break;
                case MenuStates.StartGame:

                    UIManager.UpdateButton("StartGame", 5f);
                    UseMouse(kb, crawlSpeed);
                    UpdateCamera(mainMenuPos, 6);
                    break;
                case MenuStates.Settings:
                    UseMouse(kb, crawlSpeed);
                    UpdateCamera(new Vector2(graphics.PreferredBackBufferWidth * 2, camera.Y), 6);
                    UIHelper.SetElementVisibility("Settings", true, UIManager.uiElements);
                    break;
            }
        }

        private void UseMouse(KeyboardState kb, float crawlSpeed)
        {
            this.IsMouseVisible = true;
            ms = Mouse.GetState();

            UIManager.UpdateTextBlock("MainMenuTitle");



            if ((ms.X > 0) && (ms.Y > 0) &&
               (ms.X < camera.viewport.Width) &&
               (ms.Y < camera.viewport.Height))
            {
                mousePos = new Vector2((int)(ms.Position.X + (camera.Position.X - (camera.viewport.Width/2))),
                    (int)(ms.Position.Y + (camera.Position.Y - (camera.viewport.Height / 2)))); //Can be Moved to UIManager. Sending the mspos
                if (ms.RightButton == ButtonState.Released)
                {
                    if (ms.LeftButton == ButtonState.Pressed && prevMs.LeftButton == ButtonState.Released)
                    {
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

            if (kb.IsKeyDown(Keys.Right))
            {
                camera.Update(new Vector2(camera.X + crawlSpeed, camera.Y));
            }

            if (kb.IsKeyDown(Keys.Left))
            {
                camera.Update(new Vector2(camera.X - crawlSpeed, camera.Y));
            }

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
            if (camera.X < pos.X)
            {
                camera.Update(new Vector2(camera.X + moveSpeed, camera.Y));
            }
            if (camera.X > pos.X)
            {
                camera.Update(new Vector2(camera.X - moveSpeed, camera.Y));
            }
            if (camera.Y > pos.Y)
            {
                camera.Update(new Vector2(camera.X, camera.Y - moveSpeed));
            }
            if (camera.Y < pos.Y)
            {
                camera.Update(new Vector2(camera.X, camera.Y + moveSpeed));
            }
        }

        void TitleCrawl(float crawlSpeed)
        {
            camera.Update(new Vector2(camera.X, camera.Y + crawlSpeed));
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
    }
}
