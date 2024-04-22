using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

        enum GameStates { TitleScreen, Game, Exit }
        GameStates GameState = GameStates.TitleScreen;

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
            graphics.PreferredBackBufferWidth = (int)(graphics.PreferredBackBufferWidth * 1.5f);
            //graphics.PreferredBackBufferHeight = (int)(graphics.PreferredBackBufferHeight * 1.5f);
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
          
            camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2));

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

            switch(GameState)
            {
                case GameStates.TitleScreen:
                    Menus(kb);
                    break;
                case GameStates.Game:
                    break;

            }

           

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void Menus(KeyboardState kb)
        {
            float crawlSpeed = 2f;
      
            
            switch (MenuState)
            {
                case MenuStates.TitleCrawl:
                    if (kb.IsKeyDown(Keys.Enter) && prevKB.IsKeyUp(Keys.Enter) && !startCrawl)
                    {
                        prevKB = kb;
                        startCrawl = true;
                    }
                    UIManager.UpdateTextBlock("TitleCrawl");
                   if(startCrawl)
                   {
                        TitleCrawl(crawlSpeed);
                        if(kb.IsKeyDown(Keys.G) && prevKB.IsKeyUp(Keys.G))
                        {
                            camera.Update(new Vector2(camera.X, 1000 + crawlSpeed));
                            UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);
                            MenuState = MenuStates.MainMenu;
                            startCrawl = false;
                            prevKB = kb;
                        }
                   }

                   if(camera.Y > 1000 && startCrawl)
                   {
                        MenuState = MenuStates.MainMenu;
                        screenCenter.X = this.Window.ClientBounds.Width / 2;
                        screenCenter.Y = this.Window.ClientBounds.Height / 2;
                       
                        UIHelper.SetElementVisibility("TitleCrawl", false, UIManager.uiElements);
                        mainMenuPos = camera.Position;
                        //break;
                   }
                    break;
                case MenuStates.MainMenu:
                    if(count == 0)
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
               (ms.X < graphics.PreferredBackBufferWidth) &&
               (ms.Y < graphics.PreferredBackBufferHeight))
            {
                mousePos = new Vector2((int)(ms.Position.X + (camera.Position.X - (graphics.PreferredBackBufferWidth / 2))),
                    (int)(ms.Position.Y + (camera.Position.Y - (graphics.PreferredBackBufferHeight / 2)))); //Can be Moved to UIManager. Sending the mspos
                if (ms.RightButton == ButtonState.Released)
                {
                    if (ms.LeftButton == ButtonState.Pressed && prevMs.LeftButton == ButtonState.Released)
                    {
                        foreach (UIWidget widget in UIManager.uiElements.Values)
                        {
                            if (widget is UIButton)
                            {
                                ((UIButton)widget).HitTest(
                                new Point((int)mousePos.X, (int)mousePos.Y));
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

        void UpdateCamera(Vector2 pos, float moveSpeed)
        {
            if(camera.X < pos.X)
            {
                camera.Update(new Vector2(camera.X + moveSpeed, camera.Y));
            }
            if(camera.X > pos.X)
            {
                camera.Update(new Vector2(camera.X - moveSpeed, camera.Y));
            }
            if(camera.Y > pos.Y)
            {
                camera.Update(new Vector2(camera.X , camera.Y - moveSpeed));
            }
            if(camera.Y < pos.Y)
            {
                camera.Update(new Vector2(camera.X, camera.Y + moveSpeed));
            }
        }

        void TitleCrawl(float crawlSpeed)
        {
            camera.Update(new Vector2(camera.X, camera.Y + crawlSpeed));
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Window.Title = camera.Position.ToString() + "   MousePos: " + mousePos.ToString();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            UIManager.Draw(spriteBatch);

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
