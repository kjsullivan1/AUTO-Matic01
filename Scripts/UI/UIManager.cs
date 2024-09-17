using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework.Input;
using System.Collections;

namespace AUTO_Matic
{
    class UIManager
    {
        public Dictionary<string, UIWidget> uiElements = new Dictionary<string, UIWidget>();
        ContentManager filePath;
        Game1 game;

        Vector2 dims;

        Vector2 buttonPos;
        Vector2 buttonPos2;


        bool transition = false;
        public bool willDelete = false;

        List<Rectangle> savedPos = new List<Rectangle>();

        public bool isTopDownKeys = false;
        bool isSideScrollKeys = true;
        public List<Keys> TopDownInputs = new List<Keys>();
        string[] TopDownKeyBinds;
        public List<Keys> SideScrollInputs = new List<Keys>();
        string[] SideKeyBinds;
        public string currButton;
        public int currButtonIndex = 0;

        public List<Keys> DefaultSideScrolKeys = new List<Keys>();
        public List<Keys> DefaultTopDownKeys = new List<Keys>();

        public float MasterVolume = 1;
        public float MusicVolume = 1;
        public float EffectVolume = 1;

        bool isVolume = false;
        public Rectangle SelectedRect = Rectangle.Empty;
        public void UIButton_Clicked(object sender, UIButtonArgs e)
        {
            string buttonName = e.ID;

            if(buttonName.Contains("KeyBind"))
            {
                //UIHelper.SetButtonText(uiElements[buttonName], "");
                currButton = buttonName;
                game.keyBindActive = true;

                if(isTopDownKeys == false)
                {
                    for (int i = 0; i < SideKeyBinds.Length; i++)
                    {
                        string name = "KeyBindBtn" + SideKeyBinds[i];
                        if (name == buttonName)
                            currButtonIndex = i;
                    }
                }
                else
                {
                    for (int i = 0; i < TopDownKeyBinds.Length; i++)
                    {
                        string name = "KeyBindBtn" + TopDownKeyBinds[i];
                        if (name == buttonName)
                            currButtonIndex = i;
                    }
                }

               
            }
            else if(buttonName.Contains("VolumeSettings"))
            {
                float volumeModifier = .02f;
                if(buttonName.Contains("Return") && isVolume)
                {

                    SetSideKeyBinds(isSideScrollKeys);
                    SetTopDownKeyBinds(isTopDownKeys);
                    uiElements["WeaponWheelTitle"].Visible = true;
                    uiElements["SwapKeys"].Visible = true;
                    uiElements["SwapKeysDefault"].Visible = true;
                    uiElements["SwapToVolume"].Visible = true;
                    uiElements["SettingsReturnBtn"].Visible = true;
                    isVolume = false;

                    SetVolumeSettings(isVolume);

                }
                else if(buttonName.Contains("Master"))
                {
                    if(buttonName.Contains("+"))
                    {
                        MasterVolume += volumeModifier;
                        if (MasterVolume > 1)
                            MasterVolume = 1;
                    }
                    else if(buttonName.Contains("-"))
                    {
                        MasterVolume -= volumeModifier;
                        if (MasterVolume < 0)
                            MasterVolume = 0;
                    }

                    UIHelper.SetElementText(uiElements["VolumeSettingsMasterNum"], ((int)(MasterVolume * 100)).ToString());

                    game.UpdateSounds();
                }
                else if(buttonName.Contains("Effects"))
                {
                    if(buttonName.Contains("+"))
                    {
                        EffectVolume += volumeModifier;
                        if (EffectVolume > 1)
                            EffectVolume = 1;
                    }
                    else if(buttonName.Contains("-"))
                    {
                        EffectVolume -= volumeModifier;
                        if (EffectVolume < 0)
                            EffectVolume = 0;
                    }

                    UIHelper.SetElementText(uiElements["VolumeSettingsEffectsNum"], ((int)(EffectVolume * 100)).ToString());
                    game.UpdateSounds();
                }
                else if(buttonName.Contains("Music"))
                {
                    if (buttonName.Contains("+"))
                    {
                        MusicVolume += volumeModifier;
                        if (MusicVolume > 1)
                            MusicVolume = 1;
                    }
                    else if (buttonName.Contains("-"))
                    {
                        MusicVolume -= volumeModifier;
                        if (MusicVolume < 0)
                            MusicVolume = 0;
                    }

                    UIHelper.SetElementText(uiElements["VolumeSettingsMusicNum"], ((int)(MusicVolume * 100)).ToString());
                    game.UpdateSounds();
                }

                
            }
            else
            {
              

                switch (buttonName)
                {
                    case "MainMenuExit":
                        game.Exit();
                        break;
                    case "MainMenuPlay":
                        game.ChangeMenuState(Game1.MenuStates.StartGame);
                        UIHelper.SetElementVisibility("StartNewGame", true, uiElements);
                        UIHelper.SetElementVisibility("LoadGame", true, uiElements);
                        UIHelper.SetElementVisibility("StartGameReturn", true, uiElements);
                        UIHelper.SetButtonState("StartGameReturn", false, uiElements);

                        UIHelper.SetElementVisibility("MainMenuPlay", false, uiElements);
                        UIHelper.SetElementVisibility("MainMenuExit", false, uiElements);
                        UIHelper.SetElementVisibility("MainMenuSetting", false, uiElements);
                        UIHelper.SetButtonState("MainMenu", true, uiElements);

                        break;
                    case "MainMenuSetting":
                        game.ChangeMenuState(Game1.MenuStates.Settings);
                        UIHelper.SetElementVisibility("Settings", true, uiElements);
                        //SetVolumeSettings(isVolume);
                        break;
                    case "SettingsReturnBtn":

                     
                        game.ChangeMenuState(Game1.MenuStates.MainMenu);
                        transition = true;
                        game.SaveKeyBinds(SideScrollInputs, TopDownInputs);

                        break;
                    case "StartGameReturn":
                        game.ChangeMenuState(Game1.MenuStates.MainMenu);
                        UIHelper.SetElementVisibility("StartNewGame", false, uiElements);
                        UIHelper.SetElementVisibility("LoadGame", false, uiElements);
                        UIHelper.SetElementVisibility("StartGameReturn", false, uiElements);
                        UIHelper.SetButtonState("MainMenu", false, uiElements);
                        UIHelper.SetButtonState("StartGameReturn", true, uiElements);
                        break;
                    case "StartNewGame":

                        if(System.IO.File.Exists(game.dataPath) && game.isLoadedGame)
                        {
                            //CreateDeleteSave();
                            uiElements["StartNewGame"].Visible = false;
                            uiElements["LoadGame"].Visible = false;
                            
                            willDelete = true;
                           
                            //System.IO.File.Delete(game.dataPath);
                        }
                        else
                        {
                            game.ChangeGameState(Game1.Scenes.InGame);
                            UIHelper.SetElementVisibility("MainMenu", false, uiElements);
                            UIHelper.SetElementVisibility("Settings", false, uiElements);
                            UIHelper.SetElementVisibility("StartNewGame", false, uiElements);
                            UIHelper.SetElementVisibility("LoadGame", false, uiElements);
                            UIHelper.SetElementVisibility("StartGameReturn", false, uiElements);
                            game.LoadTutorial();

                            UIHelper.SetButtonState("MainMenuPlay", false, uiElements);
                            UIHelper.SetButtonState("MainMenuExit", false, uiElements);
                        }

                     
                        break;
                    case "LoadGame":
                        if(System.IO.File.Exists(game.dataPath))
                        {
                            game.ChangeGameState(Game1.Scenes.InGame);
                        UIHelper.SetElementVisibility("MainMenu", false, uiElements);
                        UIHelper.SetElementVisibility("Settings", false, uiElements);
                        UIHelper.SetElementVisibility("StartNewGame", false, uiElements);
                        UIHelper.SetElementVisibility("LoadGame", false, uiElements);
                        UIHelper.SetElementVisibility("StartGameReturn", false, uiElements);
                        UIHelper.SetButtonState("MainMenuPlay", false, uiElements);
                        UIHelper.SetButtonState("MainMenuExit", false, uiElements);

                        game.LoadGame();
                        }
                        
                        break;
                    case "DeleteSaveAcceptBtn":
                       
                            System.IO.File.Delete(game.dataPath);
                            game.Exit();
                        
                
                        break;
                    case "DeleteSaveDenyBtn":
                        SetDeleteWarnings(false);
                        uiElements["StartNewGame"].Visible = true;
                        uiElements["LoadGame"].Visible = true;
                        willDelete = false;
                        break;
                    case "SwapKeys":
                        isTopDownKeys = !isTopDownKeys;
                        isSideScrollKeys = !isSideScrollKeys;

                        SetSideKeyBinds(isSideScrollKeys);
                        SetTopDownKeyBinds(isTopDownKeys);
                        break;
                    case "SwapKeysDefault":
                        if (isSideScrollKeys)
                        {
                            //Considering I never replace the values of these. They will be changed regardless before entering this case statement... So i manually set them back
                            DefaultSideScrolKeys.Clear();
                            DefaultSideScrolKeys.Add(Keys.A); //While loading them in.... check all keys, converting them into strings, if they match, assign it to the proper index
                            DefaultSideScrolKeys.Add(Keys.D);
                            DefaultSideScrolKeys.Add(Keys.S);
                            DefaultSideScrolKeys.Add(Keys.Enter);
                            DefaultSideScrolKeys.Add(Keys.LeftShift);
                            DefaultSideScrolKeys.Add(Keys.Space);
                            DefaultSideScrolKeys.Add(Keys.E);
                            DefaultSideScrolKeys.Add(Keys.RightShift);
                            DefaultSideScrolKeys.Add(Keys.Left);
                            DefaultSideScrolKeys.Add(Keys.Right);
                            DefaultSideScrolKeys.Add(Keys.Up);
                            DefaultSideScrolKeys.Add(Keys.Down);
                            SideScrollInputs = DefaultSideScrolKeys;

                            for(int i = 0; i < SideScrollInputs.Count; i++)
                            {
                                UIHelper.SetButtonText(uiElements["KeyBindBtn" + SideKeyBinds[i]], SideScrollInputs[i].ToString());
                            }
                        } 
                        else if (isTopDownKeys)
                        {
                            DefaultTopDownKeys.Clear();
                            DefaultTopDownKeys.Add(Keys.A);
                            DefaultTopDownKeys.Add(Keys.D);
                            DefaultTopDownKeys.Add(Keys.W);
                            DefaultTopDownKeys.Add(Keys.S);
                            DefaultTopDownKeys.Add(Keys.Space);
                            DefaultTopDownKeys.Add(Keys.Enter);
                            DefaultTopDownKeys.Add(Keys.LeftShift);
                            DefaultTopDownKeys.Add(Keys.RightShift);
                            DefaultTopDownKeys.Add(Keys.Left);
                            DefaultTopDownKeys.Add(Keys.Right);
                            DefaultTopDownKeys.Add(Keys.Up);
                            DefaultTopDownKeys.Add(Keys.Down);
                            TopDownInputs = DefaultTopDownKeys;

                            for(int i = 0; i < TopDownInputs.Count; i++)
                            {
                                UIHelper.SetButtonText(uiElements["KeyBindBtn" + TopDownKeyBinds[i]], TopDownInputs[i].ToString());
                            }
                        }
                        break;
                    case "SwapToVolume":
                        SetSideKeyBinds(false);
                        SetTopDownKeyBinds(false);
                        uiElements["WeaponWheelTitle"].Visible = false;
                        uiElements["SwapKeys"].Visible = false;
                        uiElements["SwapKeysDefault"].Visible = false;
                        uiElements["SwapToVolume"].Visible = false;
                        uiElements["SettingsReturnBtn"].Visible = false;
                        isVolume = true;

                        SetVolumeSettings(isVolume);

                        break;
                  

                }
            }
          
        }

        public void SetToDefaultKeyBinds()
        {
            DefaultSideScrolKeys.Clear();
            DefaultSideScrolKeys.Add(Keys.A); //While loading them in.... check all keys, converting them into strings, if they match, assign it to the proper index
            DefaultSideScrolKeys.Add(Keys.D);
            DefaultSideScrolKeys.Add(Keys.S);
            DefaultSideScrolKeys.Add(Keys.Enter);
            DefaultSideScrolKeys.Add(Keys.LeftShift);
            DefaultSideScrolKeys.Add(Keys.Space);
            DefaultSideScrolKeys.Add(Keys.E);
            DefaultSideScrolKeys.Add(Keys.RightShift);
            DefaultSideScrolKeys.Add(Keys.Left);
            DefaultSideScrolKeys.Add(Keys.Right);
            DefaultSideScrolKeys.Add(Keys.Up);
            DefaultSideScrolKeys.Add(Keys.Down);
            SideScrollInputs = DefaultSideScrolKeys;

            DefaultTopDownKeys.Clear();
            DefaultTopDownKeys.Add(Keys.A);
            DefaultTopDownKeys.Add(Keys.D);
            DefaultTopDownKeys.Add(Keys.W);
            DefaultTopDownKeys.Add(Keys.S);
            DefaultTopDownKeys.Add(Keys.Space);
            DefaultTopDownKeys.Add(Keys.Enter);
            DefaultTopDownKeys.Add(Keys.LeftShift);
            DefaultTopDownKeys.Add(Keys.RightShift);
            DefaultTopDownKeys.Add(Keys.Left);
            DefaultTopDownKeys.Add(Keys.Right);
            DefaultTopDownKeys.Add(Keys.Up);
            DefaultTopDownKeys.Add(Keys.Down);
            TopDownInputs = DefaultTopDownKeys;
        }
        public void UpdateTextBlock(string keyWord, Rectangle currBounds, int bossRooms = 0)
        {
            switch(keyWord)
            {
                case "TitleCrawl":
                    //StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/TextFiles/TitleCrawl.txt");

                    string text = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n  In the 31st century, humanities reach has greatly expanded amongst the stars. \n  " +
                        "With massive colonies suspended in space with millions of inhabitants.\n  None of these structures is more" +
                        "immaculate than that of Nuton 6, a beacon of \n  research and hope for tomorrow equipped with its own advanced A.I. named\n  M.O.N.O.," +
                        "managing each of the four primary quadrants of the facility, through\n  its secondary access modules. However, one day when everything" +
                        "seemed\n  calm the AI robotic cohabitants of the satellite went haywire forcing everyone\n  off the colony... or almost everyone?\n";

                    //string text = " \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nSomehow, there is an unsupported character that is being used in our crawl\n text and it is currently Out Of Order.\n We apologize for SpriteFont's lack of " +
                    //    "support on basic grammatical characters";
                    UIHelper.SetElementText(uiElements[keyWord], text);
                    if(UIHelper.GetElementBGRect(uiElements[keyWord]).Bottom > 0)
                    {
                        float yPos = UIHelper.GetElementBGRect(uiElements[keyWord]).Y - .5f;

                        UIHelper.SetElementBGRect(uiElements[keyWord], 
                            new Rectangle(UIHelper.GetElementBGRect(uiElements[keyWord]).X, (int)(UIHelper.GetElementBGRect(uiElements[keyWord]).Y - .75f),
                            UIHelper.GetElementBGRect(uiElements[keyWord]).Width, UIHelper.GetElementBGRect(uiElements[keyWord]).Height));

                        UIHelper.SetElementRect(uiElements[keyWord],
                           new Rectangle(UIHelper.GetElementRect(uiElements[keyWord]).X, (int)(UIHelper.GetElementRect(uiElements[keyWord]).Y - .75f),
                           UIHelper.GetElementRect(uiElements[keyWord]).Width, UIHelper.GetElementRect(uiElements[keyWord]).Height));
                    }
                    break;
                case "MainMenuTitle":
                    //StreamReader sr1 = new StreamReader(Directory.GetCurrentDirectory() + "/TextFiles/MainMenuTitle.txt");
                    string text1 = "";
                    UIHelper.SetElementText(uiElements[keyWord], text1);
                    break;
                case "BossRoomCounter":
                    int num = bossRooms - 1;
                    if (num < 0)
                        num = 0;
                    UIHelper.SetElementText(uiElements[keyWord], "\n\n          " + num + " / "+ UIHelper.DungeonLevels + " Rooms Completed");

                    UIHelper.SetElementBGRect(uiElements[keyWord], new Rectangle(currBounds.Right - 290, currBounds.Top + 30,
                        UIHelper.GetElementBGRect(uiElements[keyWord]).Width, UIHelper.GetElementBGRect(uiElements[keyWord]).Height));

                    UIHelper.SetElementRect(uiElements[keyWord], new Rectangle(currBounds.Right - 290, currBounds.Top + 30,
                      UIHelper.GetElementBGRect(uiElements[keyWord]).Width, UIHelper.GetElementBGRect(uiElements[keyWord]).Height));


                    break;

            }

           
            //float p2Elevation =
            //MathHelper.ToDegrees(tanks[1].GunElevation) * -1;
            //float p2Rot = MathHelper.ToDegrees(tanks[1].TurretRotation);
            //p2Rot = 180 - p2Rot;
            //UIHelper.SetElementText(uiElements["p2Rotation"], "Angle: " + p2Rot.ToString("N2"));
            //UIHelper.SetElementText(uiElements["p2Elevation"], "Elevation: " + p2Elevation.ToString("N2"));
            //float p1Power = shotPower;
            //UIHelper.SetElementText(uiElements["p2Power"], "Power: " + p2Power.ToString("N2"));
        }

        public void SkipCrawl(string keyWord)
        {
            uiElements[keyWord].Visible = false;
        }

        public void UpdateButton(string keyWord, float moveSpeed, Game1 game)
        {
            switch(keyWord)
            {
                case "StartGame":
                    if (UIHelper.GetRectangle(uiElements["StartNewGame"]).Y > UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y)
                    {
                        UIHelper.SetRectangle(uiElements["StartNewGame"],
                            new Rectangle(UIHelper.GetRectangle(uiElements["StartNewGame"]).X, (int)(UIHelper.GetRectangle(uiElements["StartNewGame"]).Y - moveSpeed),
                            UIHelper.GetRectangle(uiElements["StartNewGame"]).Width, UIHelper.GetRectangle(uiElements["StartNewGame"]).Height));

                        //uiElements["StartNewGame"].SetY(uiElements["StartNewGame"].Position.Y - moveSpeed);
                    }
                    if (UIHelper.GetRectangle(uiElements["StartNewGame"]).Y <= UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y)
                    {
                        UIHelper.SetRectangle(uiElements["StartNewGame"], new Rectangle((int)uiElements["MainMenuPlay"].Position.X,
                            (int)uiElements["MainMenuPlay"].Position.Y, UIHelper.GetRectangle(uiElements["StartNewGame"]).Width,
                            UIHelper.GetRectangle(uiElements["StartNewGame"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuPlay"],
                            new Rectangle((int)uiElements["StartNewGame"].Position.X, (int)uiElements["StartNewGame"].Position.Y, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width,
                            UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));
                    }

                    if (UIHelper.GetRectangle(uiElements["LoadGame"]).Y > UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y)
                    {
                        UIHelper.SetRectangle(uiElements["LoadGame"],
                            new Rectangle(UIHelper.GetRectangle(uiElements["LoadGame"]).X, (int)(UIHelper.GetRectangle(uiElements["LoadGame"]).Y - moveSpeed),
                            UIHelper.GetRectangle(uiElements["LoadGame"]).Width, UIHelper.GetRectangle(uiElements["LoadGame"]).Height));

                        //uiElements["LoadGame"].SetY(uiElements["LoadGame"].Position.Y - moveSpeed);
                    }
                    if (UIHelper.GetRectangle(uiElements["LoadGame"]).Y <= UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y)
                    {
                        UIHelper.SetRectangle(uiElements["LoadGame"], new Rectangle((int)uiElements["MainMenuExit"].Position.X,
                            (int)uiElements["MainMenuExit"].Position.Y, UIHelper.GetRectangle(uiElements["LoadGame"]).Width,
                            UIHelper.GetRectangle(uiElements["LoadGame"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuExit"],
                            new Rectangle((int)uiElements["LoadGame"].Position.X, (int)uiElements["LoadGame"].Position.Y, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Width,
                            UIHelper.GetRectangle(uiElements["MainMenuExit"]).Height));

                      

                    }
                    break;
                case "MainMenu":
                    //Move Main Menu elements back to center
                    if(UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).X < ((int)dims.X / 2) - (UIHelper.MenuTitle.Width / 2))
                    {
                        //Move all the Main menu elements

                        UIHelper.SetElementBGRect(uiElements["MainMenuTitle"],
                          new Rectangle((int)(UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).X + moveSpeed), UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Y,
                          UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Width, UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Height));

                        UIHelper.SetElementRect(uiElements["MainMenuTitle"],
                          new Rectangle((int)(UIHelper.GetElementRect(uiElements["MainMenuTitle"]).X + moveSpeed), UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Y,
                          UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Width, UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Height));


                        UIHelper.SetRectangle(uiElements["MainMenuPlay"], new Rectangle((int)(UIHelper.GetRectangle(uiElements["MainMenuPlay"]).X + moveSpeed),
                            UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuExit"], new Rectangle((int)(UIHelper.GetRectangle(uiElements["MainMenuExit"]).X + moveSpeed),
                           UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Width, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuSetting"], new Rectangle((int)(UIHelper.GetRectangle(uiElements["MainMenuSetting"]).X + moveSpeed),
                           UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Y, UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Width, UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Height));

                        if (UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).X > ((int)dims.X / 2) - (UIHelper.MenuTitle.Width / 2))
                        {
                            UIHelper.SetElementBGRect(uiElements["MainMenuTitle"],
                          new Rectangle(((int)dims.X / 2) - UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Width/2, UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Y,
                          UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Width, UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Height));

                            UIHelper.SetElementRect(uiElements["MainMenuTitle"],
                        new Rectangle(((int)dims.X / 2) - UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Width / 2, UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Y,
                        UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Width, UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Height));


                            UIHelper.SetRectangle(uiElements["MainMenuPlay"], new Rectangle(((int)dims.X / 2) - (200 / 2),
                                UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));

                            UIHelper.SetRectangle(uiElements["MainMenuExit"], new Rectangle(((int)dims.X / 2) - (200 / 2),
                               UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Width, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Height));

                            UIHelper.SetRectangle(uiElements["MainMenuSetting"], new Rectangle(((int)dims.X / 2) + (200),
                               UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Y, UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Width, UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Height));

                            transition = false;
                        }
                    }

                    //All settings Move back
                    if (UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).X <
                        dims.X * 1.5f)
                    {
                        //UIHelper.SetElementBGRect(uiElements["SettingsMenuTitle"], new Rectangle((int)(UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).X + moveSpeed),
                        //    UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).Y, UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).Width,
                        //    UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).Height));

                        UIHelper.SetElementRect(uiElements["SettingsMenuTitle"], new Rectangle((int)(UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).X + moveSpeed),
                              UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Y, UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Width,
                              UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Height));

                        UIHelper.SetElementBGRect(uiElements["SettingsButtonBox"], new Rectangle((int)(UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).X + moveSpeed),
                             UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Y, UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Width,
                             UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Height));

                        UIHelper.SetElementRect(uiElements["SettingsButtonBox"], new Rectangle((int)(UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).X + moveSpeed),
                              UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Y, UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Width,
                              UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Height));

                        UIHelper.SetRectangle(uiElements["SettingsReturnBtn"], new Rectangle((int)(UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).X + moveSpeed),
                            UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Y, UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Width,
                            (UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Height)));
                        if (uiElements.ContainsKey("SideBindTitle"))
                        {

                            SetSideKeyBinds(false);
                            SetTopDownKeyBinds(false);
                            uiElements["WeaponWheelTitle"].Visible = false;
                            uiElements["SwapKeys"].Visible = false;
                            uiElements["SwapKeysDefault"].Visible = false;
                            uiElements["SwapToVolume"].Visible = false;
                           

                        }
                            
                    }

                        //Start/Load/Exit/NewGame buttons
                    if (UIHelper.GetRectangle(uiElements["StartNewGame"]).Y < UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y)
                    {
                        UIHelper.SetRectangle(uiElements["MainMenuPlay"],
                            new Rectangle(UIHelper.GetRectangle(uiElements["MainMenuPlay"]).X, (int)(UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y - moveSpeed),
                            UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));

                        //uiElements["StartNewGame"].SetY(uiElements["StartNewGame"].Position.Y - moveSpeed);
                    }
                    if (UIHelper.GetRectangle(uiElements["StartNewGame"]).Y >= UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y && !transition)
                    {
                        UIHelper.SetRectangle(uiElements["MainMenuPlay"], new Rectangle((int)uiElements["MainMenuPlay"].Position.X,
                            (int)uiElements["MainMenuPlay"].Position.Y, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width,
                            UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));

                        UIHelper.SetRectangle(uiElements["StartNewGame"],
                            new Rectangle((int)uiElements["StartNewGame"].Position.X, (int)uiElements["StartNewGame"].Position.Y, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width,
                            UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));
                    }

                    if (UIHelper.GetRectangle(uiElements["LoadGame"]).Y < UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y)
                    {
                        UIHelper.SetRectangle(uiElements["MainMenuExit"],
                            new Rectangle(UIHelper.GetRectangle(uiElements["MainMenuExit"]).X, (int)(UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y - moveSpeed),
                            UIHelper.GetRectangle(uiElements["LoadGame"]).Width, UIHelper.GetRectangle(uiElements["LoadGame"]).Height));

                        //uiElements["LoadGame"].SetY(uiElements["LoadGame"].Position.Y - moveSpeed);
                    }
                    if (UIHelper.GetRectangle(uiElements["LoadGame"]).Y >= UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y && !transition)
                    {
                        UIHelper.SetRectangle(uiElements["MainMenuExit"], new Rectangle((int)uiElements["MainMenuExit"].Position.X,
                            (int)uiElements["MainMenuExit"].Position.Y, UIHelper.GetRectangle(uiElements["LoadGame"]).Width,
                            UIHelper.GetRectangle(uiElements["LoadGame"]).Height));

                        UIHelper.SetRectangle(uiElements["LoadGame"],
                            new Rectangle((int)uiElements["LoadGame"].Position.X, (int)uiElements["LoadGame"].Position.Y, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Width,
                            UIHelper.GetRectangle(uiElements["MainMenuExit"]).Height));



                    }
                   
                    break;
                case "Settings":
                    //Move main menu elements to the left
                    //bool createKeyBinds = true;
                   // SetVolumeSettings(isVolume);
                    if (UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Right > -100)
                    {
                        //Move all Main menu elements

                        UIHelper.SetElementBGRect(uiElements["MainMenuTitle"],
                            new Rectangle((int)(UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).X - moveSpeed), UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Y,
                            UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Width, UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Height));

                        UIHelper.SetElementRect(uiElements["MainMenuTitle"],
                          new Rectangle((int)(UIHelper.GetElementRect(uiElements["MainMenuTitle"]).X - moveSpeed), UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Y,
                          UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Width, UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Height));


                        UIHelper.SetRectangle(uiElements["MainMenuPlay"], new Rectangle((int)(UIHelper.GetRectangle(uiElements["MainMenuPlay"]).X - moveSpeed),
                            UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuExit"], new Rectangle((int)(UIHelper.GetRectangle(uiElements["MainMenuExit"]).X - moveSpeed),
                           UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Width, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuSetting"], new Rectangle((int)(UIHelper.GetRectangle(uiElements["MainMenuSetting"]).X - moveSpeed),
                           UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Y, UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Width, UIHelper.GetRectangle(uiElements["MainMenuSetting"]).Height));

                    }

                    if(UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).X > 
                        (int)((dims.X/2) -(UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Width/2)))
                    {
                        UIHelper.SetElementBGRect(uiElements["SettingsMenuTitle"], new Rectangle((int)(UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).X - moveSpeed),
                            UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).Y, UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).Width,
                            UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).Height));

                        UIHelper.SetElementRect(uiElements["SettingsMenuTitle"], new Rectangle((int)(UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).X - moveSpeed),
                              UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Y, UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Width,
                              UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Height));

                        UIHelper.SetElementBGRect(uiElements["SettingsButtonBox"], new Rectangle((int)(UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).X - moveSpeed),
                             UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Y, UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Width,
                             UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Height));

                        UIHelper.SetElementRect(uiElements["SettingsButtonBox"], new Rectangle((int)(UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).X - moveSpeed),
                              UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Y, UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Width,
                              UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Height));

                        UIHelper.SetRectangle(uiElements["SettingsReturnBtn"], new Rectangle((int)(UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).X - moveSpeed),
                            UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Y, UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Width,
                            (UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Height)));
                       
                       

                        if (UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).X <
                       (int)((dims.X / 2) - (UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Width / 2)))
                        {
                           // UIHelper.SetElementRect(uiElements["SettingsMenuTitle"], new Rectangle((int)((dims.X / 2) - (UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Width / 2)),
                           //UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Y, UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Width,
                           //UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Height));

                            UIHelper.SetElementRect(uiElements["SettingsMenuTitle"], new Rectangle((int)((dims.X / 2) - (UIHelper.GetElementBGRect(uiElements["SettingsMenuTitle"]).Width / 2)),
                                  UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Y, UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Width,
                                  UIHelper.GetElementRect(uiElements["SettingsMenuTitle"]).Height));

                            UIHelper.SetElementBGRect(uiElements["SettingsButtonBox"], new Rectangle((int)(((dims.X /2))) - (525 / 2),
                       UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Y, UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Width,
                       UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Height));

                            UIHelper.SetElementRect(uiElements["SettingsButtonBox"], new Rectangle((int)(((dims.X /2))) - (525 / 2),
                                  UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Y, UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Width,
                                  UIHelper.GetElementRect(uiElements["SettingsButtonBox"]).Height));

                            UIHelper.SetRectangle(uiElements["SettingsReturnBtn"], new Rectangle(UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Right - 65,
                             UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Y, UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Width,
                             (UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Height)));

                            //CreateKeyBindsUI(game);
                            if(uiElements.ContainsKey("SideBindTitle") && isSideScrollKeys && isTopDownKeys == false && isVolume == false)
                            {
                                SetSideKeyBinds(true);
                                uiElements["WeaponWheelTitle"].Visible = true;
                                uiElements["SwapKeys"].Visible = true;
                                uiElements["SwapKeysDefault"].Visible = true;
                                uiElements["SwapToVolume"].Visible = true;
                               // SetVolumeSettings(isVolume);
                            }
                            //CreateKeyBindsUI(game);
                        }
                       
                    }
                    break;
            }
        }

        public void CreateUIElements(Vector2 dims, Game1 game)
        {
            //filePath = content;
            //Title crawl elements
            uiElements.Clear();
            this.dims = dims;
            this.game = game;

            //Background
            uiElements.Add("MainMenuBackground", UIHelper.CreateTextblock("MainMenuBackground", "", 0, 0));
            UIHelper.SetElementBGRect(uiElements["MainMenuBackground"], new Rectangle(new Point((int)uiElements["MainMenuBackground"].Position.X, (int)uiElements["MainMenuBackground"].Position.Y),
                new Point(1920, 1080)));

            uiElements.Add("TitleCrawl", UIHelper.CreateTextblock("TitleCrawl", "This is t he title crawl", ((int)dims.X / 2) - (550/2), (int)(dims.Y /2))); //Replace 550 with UIHelper.ScrollBG.Width
            UIHelper.SetElementRect(uiElements["TitleCrawl"], new Rectangle(new Point((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["TitleCrawl"], new Rectangle((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y, 550, (int)(dims.Y + (dims.Y/4))));

         

            ///Main Menu elements
            //Title
            uiElements.Add("MainMenuTitle", UIHelper.CreateTextblock("MainMenuTitle", "", ((int)dims.X / 2) - (UIHelper.MenuTitle.Width / 2), //450 with UIHelper.MenuTitle.Width
                ((int)dims.Y /2) - (int)(UIHelper.MenuTitle.Height)));
            UIHelper.SetElementRect(uiElements["MainMenuTitle"], new Rectangle(new Point((int)(uiElements["MainMenuTitle"].Position.X + 180), (int)uiElements["MainMenuTitle"].Position.Y + 90), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["MainMenuTitle"], new Rectangle((int)uiElements["MainMenuTitle"].Position.X, (int)uiElements["MainMenuTitle"].Position.Y, UIHelper.MenuTitle.Width, (int)(UIHelper.MenuTitle.Height)));
            //Play
            uiElements.Add("MainMenuPlay", UIHelper.CreateButton("MainMenuPlay", "Play", ((int)dims.X / 2) - (400 / 2), //Multiplying the width * 2 in the Draw Method
                UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Bottom + 60));
            //Exit Button
            uiElements.Add("MainMenuExit", UIHelper.CreateButton("MainMenuExit", "Exit", ((int)dims.X / 2) - (400 / 2), 
                UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Bottom + 60));
            //Settings
            uiElements.Add("MainMenuSetting", UIHelper.CreateButton("MainMenuSetting", "", UIHelper.GetRectangle(uiElements["MainMenuExit"]).Right + 100, //Using width/2 of button
                /*(int)(dims.Y) - 60))*/  UIHelper.GetRectangle(uiElements["MainMenuExit"]).Top + 25)); //Height of Exit button - Height of Settings button 
            UIHelper.SetRectangle(uiElements["MainMenuSetting"], 100, 100);

            ///Settings Menu elements
            //Title
            string titleTxt = "Settings";
            uiElements.Add("SettingsMenuTitle", UIHelper.CreateTextblock("SettingsMenuTitle", titleTxt, (int)(((dims.X * 1.5f)) - (titleTxt.Length * 11.0f)), // dividing the txt * 11 / 2 makes in longer?
                UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Top - 75));
            UIHelper.SetElementRect(uiElements["SettingsMenuTitle"], new Rectangle(uiElements["SettingsMenuTitle"].Position.ToPoint(), new Point(titleTxt.Length, 20)));

            uiElements["SettingsMenuTitle"].Visible = false;

            //Settings Box
            uiElements.Add("SettingsButtonBox", UIHelper.CreateTextblock("\n\tSettingsButtonBox", "", (int)(((dims.X * 1.5f))) - (525 / 2),
                (int)uiElements["SettingsMenuTitle"].Position.Y + 70));
            UIHelper.SetElementRect(uiElements["SettingsButtonBox"], new Rectangle(uiElements["SettingsButtonBox"].Position.ToPoint(), new Point(350, 100)));
            UIHelper.SetElementBGRect(uiElements["SettingsButtonBox"], new Rectangle(uiElements["SettingsButtonBox"].Position.ToPoint(), new Point(525, 350)));

            //Return Button
            uiElements.Add("SettingsReturnBtn", UIHelper.CreateButton("SettingsReturnBtn", "", UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Right - 65, //50+15
                UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Bottom - 65));
            UIHelper.SetRectangle(uiElements["SettingsReturnBtn"], 50, 50);

            ///Start Menu Elements
            //New Game Btn
            uiElements.Add("StartNewGame", UIHelper.CreateButton("StartNewGame", "New", ((int)dims.X / 2) - (400 / 2),
                (int)(UIHelper.GetElementBGRect(uiElements["TitleCrawl"]).Bottom + dims.Y)));
            buttonPos = uiElements["StartNewGame"].Position;
            //Load Game
            uiElements.Add("LoadGame", UIHelper.CreateButton("LoadGame", "Load", ((int)dims.X / 2) - (400 / 2),
                (int)(UIHelper.GetRectangle(uiElements["StartNewGame"]).Bottom + 60)));
            buttonPos2 = uiElements["LoadGame"].Position;

            //Delete save warning
            uiElements.Add("DeleteSaveBox", UIHelper.CreateTextblock("DeleteSaveBox", "\n\n                  You Currently have a save on file." +
                "\n                   Do you wish to delete it and restart?\n                  (you will need to restart the game :) )"
                , ((int)dims.X / 2 - (300 / 2)), (int)(dims.Y / 2 - (200 / 2))));
            UIHelper.SetElementRect(uiElements["DeleteSaveBox"], new Rectangle(uiElements["DeleteSaveBox"].Position.ToPoint(), new Point(300, 200)));
            UIHelper.SetElementBGRect(uiElements["DeleteSaveBox"], new Rectangle(uiElements["DeleteSaveBox"].Position.ToPoint(), new Point(300, 200)));

            uiElements.Add("DeleteSaveAcceptBtn", UIHelper.CreateButton("DeleteSaveAcceptBtn", "   ACCEPT", UIHelper.GetElementBGRect(uiElements["DeleteSaveBox"]).Center.X - (150 / 2), 
                UIHelper.GetElementBGRect(uiElements["DeleteSaveBox"]).Y + (int)(UIHelper.GetElementBGRect(uiElements["DeleteSaveBox"]).Height/2.75f)));
            UIHelper.SetRectangle(uiElements["DeleteSaveAcceptBtn"], 150, 50);

            uiElements.Add("DeleteSaveDenyBtn", UIHelper.CreateButton("DeleteSaveDenyBtn", "NEVERMIND", UIHelper.GetRectangle(uiElements["DeleteSaveAcceptBtn"]).X, UIHelper.GetRectangle(uiElements["DeleteSaveAcceptBtn"]).Bottom + 10));
            UIHelper.SetRectangle(uiElements["DeleteSaveDenyBtn"], 150, 50);

            SetDeleteWarnings(false);

            //Return 
            uiElements.Add("StartGameReturn", UIHelper.CreateButton("StartGameReturn", "", (int)uiElements["MainMenuSetting"].Position.X, (int)uiElements["MainMenuSetting"].Position.Y));
            UIHelper.SetRectangle(uiElements["StartGameReturn"], 100, 100);

            uiElements.Add("HealthBar", UIHelper.CreateTextblock("HealthBar", "", 64, 64));
            UIHelper.SetElementRect(uiElements["HealthBar"], new Rectangle(uiElements["HealthBar"].Position.ToPoint(), new Point(1280, 512)));
            UIHelper.SetElementBGRect(uiElements["HealthBar"], new Rectangle(uiElements["HealthBar"].Position.ToPoint(), new Point(1280/6, 512/6)));

            uiElements.Add("DashIcon", UIHelper.CreateTextblock("DashIcon", "", 64, 64));
            UIHelper.SetElementRect(uiElements["DashIcon"], new Rectangle(uiElements["DashIcon"].Position.ToPoint(), new Point(128, 128)));
            UIHelper.SetElementBGRect(uiElements["DashIcon"], new Rectangle(uiElements["DashIcon"].Position.ToPoint(), new Point(128, 128)));

            uiElements.Add("BossRoomCounter", UIHelper.CreateTextblock("BossRoomCounter", "\n\n          0/10 Rooms Complete", 0,0));
            UIHelper.SetElementRect(uiElements["BossRoomCounter"], new Rectangle(uiElements["BossRoomCounter"].Position.ToPoint(), new Point(128, 96)));
            UIHelper.SetElementBGRect(uiElements["BossRoomCounter"], new Rectangle(uiElements["BossRoomCounter"].Position.ToPoint(), new Point(215, 70)));

            //CreateKeyBindsUI(game);

            //uiElements.Add("p2Rotation", UIHelper.CreateTextblock("p2Rotation", "x", 580, 120));
            //uiElements.Add("p2Elevation", UIHelper.CreateTextblock("p2Elevation", "x", 580, 135));
            //uiElements.Add("p2PowerUp", UIHelper.CreateButton("p2PowerUp", "P+", 505, 5));
            //uiElements.Add("p2PowerDown", UIHelper.CreateButton("p2PowerDown", "P-", 505, 60));
            //uiElements.Add("p2Power", UIHelper.CreateTextblock("p2Power", "x", 580, 155));

            foreach (UIWidget widget in uiElements.Values)
            {
                if (widget is UIButton)
                {
                    ((UIButton)widget).Clicked += new
                    UIButton.ClickHandler(UIButton_Clicked);
                }
            }
        }

        public void SetDeleteWarnings(bool visible)
        {
            foreach(UIWidget widget in uiElements.Values)
            {
                if (widget.ID.Contains("DeleteSave"))
                    widget.Visible = visible;
            }
        }

        public void CreateTutorialUI(Vector2 dims, Game1 game)
        {
            if(uiElements.ContainsKey("TutorialBoxGame") == false)
            {
                string[] tutorialText = new string[SideTileMap.GetTextBoxes().Count];
                tutorialText[0] = " \n  [" + SideScrollInputs[5] + "] key or (A) button to jump \n" +
                                        "  [" + SideScrollInputs[4] + "] or (B) to dash";
                tutorialText[1] = " \n  [" + SideScrollInputs[0] + "][" + SideScrollInputs[1] + "] keys or (L) Analog to move";
                tutorialText[2] = " \n  [" + SideScrollInputs[2] +"] key or DOWN on (L) analog to get into a \n shooting stance or ground pound (Air) \n" +
                                  " \n  Press  ["+SideScrollInputs[3] +"] or (X) button to fire";
                tutorialText[3] = " \n  Use the [" + SideScrollInputs[11] + "][" + SideScrollInputs[9] + "][" + SideScrollInputs[10] + "][" + SideScrollInputs[8] + "] keys or (R)\n  analog to use the weapon wheel\n" +
                    "  Use [" + SideScrollInputs[7] + "] key or press the (R) \n  analog to return to Pistol";

                for(int i = 0; i < SideTileMap.GetTextBoxes().Count; i++)
                {
                    if(uiElements.ContainsKey("TutorialBox" + i) == false)
                    {
                        uiElements.Add("TutorialBox" + i, UIHelper.CreateTextblock("TutorialBox" + i, tutorialText[i], (int)SideTileMap.GetTextBoxes()[i].X, (int)SideTileMap.GetTextBoxes()[i].Y));
                        UIHelper.SetElementRect(uiElements["TutorialBox" + i], new Rectangle(uiElements["TutorialBox" + i].Position.ToPoint(), new Point(128, 96)));
                        UIHelper.SetElementBGRect(uiElements["TutorialBox" + i], new Rectangle(uiElements["TutorialBox" + i].Position.ToPoint(), new Point(215, 70)));
                    }
                    else
                    {
                        UIHelper.SetElementText(uiElements["TutorialBox" + i], tutorialText[i]);
                    }
                  
                }


            //    uiElements.Add("TutorialBoxGame", UIHelper.CreateTextblock("TutorialBoxGame", "" +
            //                   "\n\n  Use WASD or Left Analog Stick to move" +
            //                   "\n\n  Press space bar or (A) button to jump" +
            //                   "\n\n  Hold down to get into shooting stance and press Enter key or (X) button to fire" +
            //                   "\n\n  If you time it right while in the air, you can ground pound" +
            //                   "\n\n  Press LShift key or (B) button to dash forwards" +
            //                   "\n\n  The E key and (Y) button allows you to open doors and go into the dungeon" +
            //                   "\n\n  Hold them to jump out and play as the pilot\n\n", (int)(dims.X + 192/2), (int)(dims.Y - 300)));
            //    UIHelper.SetElementRect(uiElements["TutorialBoxGame"], new Rectangle(uiElements["TutorialBoxGame"].Position.ToPoint(), new Point(450, 600)));
            //    UIHelper.SetElementBGRect(uiElements["TutorialBoxGame"], new Rectangle(uiElements["TutorialBoxGame"].Position.ToPoint(), new Point(400, 225)));
            }
     

        }

        public void SetSideKeyBinds(bool visible)
        {
            for(int i = 0; i < SideKeyBinds.Length; i++)
            {
                uiElements["KeyBind" + SideKeyBinds[i]].Visible = visible;
                uiElements["KeyBindBtn" + SideKeyBinds[i]].Visible = visible;
                uiElements["SideBindTitle"].Visible = visible;
                //uiElements["WheaponWheelTitle"].Visible = visible;
                //uiElements["SwapKeyBinds"].Visible = visible;
            }
        }

        public void SetTopDownKeyBinds(bool visible)
        {
            for(int i = 0; i < TopDownKeyBinds.Length; i++)
            {
                uiElements["KeyBind" + TopDownKeyBinds[i]].Visible = visible;
                uiElements["KeyBindBtn" + TopDownKeyBinds[i]].Visible = visible;
                uiElements["TopDownBindTitle"].Visible = visible;
                //uiElements["Swap"]

            }
        }

        public void CreateSettingsUI(Game1 game)
        {
            if(uiElements.ContainsKey("SideBindTitle") == false)
            {
                uiElements["SettingsMenuTitle"].Visible = false;

                SideKeyBinds = new string[12]; //Dash, Jump, move: left/right, shoot, StartShoot, WeaponWheel: pistol/Ray/Burst/Bomb/Shotgun
                SideKeyBinds[0] = "Move Left:";
                SideKeyBinds[1] = "Move Right:";
                SideKeyBinds[2] = "Start Shoot:";
                SideKeyBinds[3] = "Shoot:";
                SideKeyBinds[4] = "Dash:";
                SideKeyBinds[5] = "Jump:";
                SideKeyBinds[6] = "Interact:";
                SideKeyBinds[7] = "Pistol:";
                SideKeyBinds[8] = "Ray:";
                SideKeyBinds[9] = "Burst:";
                SideKeyBinds[10] = "Bomb:";
                SideKeyBinds[11] = "Shotgun:";

                DefaultSideScrolKeys.Add(Keys.A); //While loading them in.... check all keys, converting them into strings, if they match, assign it to the proper index
                DefaultSideScrolKeys.Add(Keys.D);
                DefaultSideScrolKeys.Add(Keys.S);
                DefaultSideScrolKeys.Add(Keys.Enter);
                DefaultSideScrolKeys.Add(Keys.LeftShift);
                DefaultSideScrolKeys.Add(Keys.Space);
                DefaultSideScrolKeys.Add(Keys.E);
                DefaultSideScrolKeys.Add(Keys.RightShift);
                DefaultSideScrolKeys.Add(Keys.Left);
                DefaultSideScrolKeys.Add(Keys.Right);
                DefaultSideScrolKeys.Add(Keys.Up);
                DefaultSideScrolKeys.Add(Keys.Down);

       
                SideScrollInputs = game.GetSideScrollInputs();
                //string word = "Up";

                //ArrayList keys = new ArrayList(Enum.GetValues(typeof(Keys)));

                //Enum.Parse(typeof(Keys), word);

                TopDownKeyBinds = new string[12]; //Dash, Move: left/right/up/down, Shoot, WeaponWheel: Pistol/Ray/Burst/Bomb/Shotgun, Direction Locking, 
                TopDownKeyBinds[0] = "Move Left: ";
                TopDownKeyBinds[1] = "Move Right: ";
                TopDownKeyBinds[2] = "Move Up: ";
                TopDownKeyBinds[3] = "Move Down: ";
                TopDownKeyBinds[4] = "Dash: ";
                TopDownKeyBinds[5] = "Shoot: ";
                TopDownKeyBinds[6] = "Shoot Direction Lock: ";
                TopDownKeyBinds[7] = "Pistol: ";
                TopDownKeyBinds[8] = "Ray: ";
                TopDownKeyBinds[9] = "Burst: ";
                TopDownKeyBinds[10] = "Bomb: ";
                TopDownKeyBinds[11] = "Shotgun: ";

                DefaultTopDownKeys.Add(Keys.A);
                DefaultTopDownKeys.Add(Keys.D);
                DefaultTopDownKeys.Add(Keys.W);
                DefaultTopDownKeys.Add(Keys.S);
                DefaultTopDownKeys.Add(Keys.Space);
                DefaultTopDownKeys.Add(Keys.Enter);
                DefaultTopDownKeys.Add(Keys.LeftShift);
                DefaultTopDownKeys.Add(Keys.RightShift);
                DefaultTopDownKeys.Add(Keys.Left);
                DefaultTopDownKeys.Add(Keys.Right);
                DefaultTopDownKeys.Add(Keys.Up);
                DefaultTopDownKeys.Add(Keys.Down);

                
                TopDownInputs = game.GetTopDownInputs();

                if(SideScrollInputs == new List<Keys>() && TopDownInputs == new List<Keys>())
                {
                    SideScrollInputs = DefaultSideScrolKeys;
                    TopDownInputs = DefaultTopDownKeys;
                }

                int width = 128;
                int height = 25;

                uiElements.Add("SwapKeys", UIHelper.CreateButton("SwapKeys", "Swap to Top/Side Key Binds",
                    (int)((UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Center.X - (dims.X)) - (213 / 5)), (int)(UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Bottom - 42)));
                UIHelper.SetRectangle(uiElements["SwapKeys"], 213, height);

                uiElements.Add("SwapKeysDefault", UIHelper.CreateButton("SwapKeysDefault", "Set to Default Keys",
                    UIHelper.GetRectangle(uiElements["SwapKeys"]).X, UIHelper.GetRectangle(uiElements["SwapKeys"]).Y - (height + 2)));
                UIHelper.SetRectangle(uiElements["SwapKeysDefault"], 213, height);

                uiElements.Add("SwapToVolume", UIHelper.CreateButton("SwapToVolume", "Volume Settings",
                    UIHelper.GetRectangle(uiElements["SwapKeys"]).X - 145, UIHelper.GetRectangle(uiElements["SwapKeysDefault"]).Center.Y));
                UIHelper.SetRectangle(uiElements["SwapToVolume"], 140, height);

                //uiElements["SwapKeys"].Visible = true;

                CreateVolumeSettings();

                CreateSideScrollKeyBinds(width, height);

                CreateTopDownKeyBinds(width, height);

                foreach (UIWidget widget in uiElements.Values)
                {
                    if (widget is UIButton)
                    {

                        ((UIButton)widget).Clicked += new
                        UIButton.ClickHandler(UIButton_Clicked);
                    }
                }

                this.game = game;
            }
        }

        public void SetVolumeSettings(bool visible)
        {
            foreach(UIWidget widget in uiElements.Values)
            {
                if (widget.ID.Contains("VolumeSettings"))
                    widget.Visible = visible;
            }
        }

        public void CreateTopDownTutorial(Rectangle bounds)
        {
            int width = 64 * 5;
            int height = 64 * 3;
            if(uiElements.ContainsKey("TDTutorialBoxIntro") == false)
            {
                uiElements.Add("TDTutorialBoxIntro", UIHelper.CreateTextblock("TDTutorialBoxIntro", "    \n\n\n\n               Rooms until the boss " + UIHelper.DungeonLevels, bounds.Center.X - (width / 2), bounds.Y + (bounds.Height / 4)));// 4x3
                UIHelper.SetElementBGRect(uiElements["TDTutorialBoxIntro"], new Rectangle(uiElements["TDTutorialBoxIntro"].Position.ToPoint(), new Point(width, height)));
                UIHelper.SetElementRect(uiElements["TDTutorialBoxIntro"], new Rectangle(uiElements["TDTutorialBoxIntro"].Position.ToPoint(), new Point(width, height)));


                UIHelper.SetElementVisibility("TDTutorial", true, uiElements);
            }
            else
            {
                UIHelper.SetElementText(uiElements["TDTutorialBoxIntro"], "    \n\n\n\n                                      Rooms until the boss " + UIHelper.DungeonLevels + "\n\n" 
                    + "   For Controller, use the Left and Right sticks to move and\n   navigate weapon wheel. B to Dash and X to Shoot.  Use the\n   Right Shoulder button to lock direction" + 
                    "\n\n" +  "   For Keyboard, use " + TopDownInputs[2] + TopDownInputs[0] + TopDownInputs[3] + TopDownInputs[1] + " to move.  " + TopDownInputs[4] 
                    + " to dash and " + TopDownInputs[5] + "\n   to shoot.  Use the " + TopDownInputs[6] + " key to lock direction");
                UIHelper.SetElementVisibility("TDTutorial", true, uiElements);
            }
        }

        public void UpdateTopDownTutorial(Rectangle bounds)
        {
   
        }

        private void CreateVolumeSettings()
        {
            //Title
            uiElements.Add("VolumeSettingsTitle", UIHelper.CreateTextblock("VolumeSettingsTitle", "Volume Settings", (int)(1980 / 2 - (200 / 2)), (int)(380)));
            UIHelper.SetElementBGRect(uiElements["VolumeSettingsTitle"], new Rectangle(uiElements["VolumeSettingsTitle"].Position.ToPoint(), new Point(150, 25)));
            UIHelper.SetElementRect(uiElements["VolumeSettingsTitle"], new Rectangle(uiElements["VolumeSettingsTitle"].Position.ToPoint(), new Point(150, 25)));

            //Master volume title
            uiElements.Add("VolumeSettingsMaster", UIHelper.CreateTextblock("VolumeSettingsMaster", "Master Volume", (int)(1980 / 2.5f), (int)(1020 / 2.25f)));
            UIHelper.SetElementBGRect(uiElements["VolumeSettingsMaster"], new Rectangle(uiElements["VolumeSettingsMaster"].Position.ToPoint(), new Point(125, 25)));
            UIHelper.SetElementRect(uiElements["VolumeSettingsMaster"], new Rectangle(uiElements["VolumeSettingsMaster"].Position.ToPoint(), new Point(125, 25)));

            //Music volume title
            uiElements.Add("VolumeSettingsMusic", UIHelper.CreateTextblock("VolumeSettingsMusic", "Music Volume", UIHelper.GetElementBGRect(uiElements["VolumeSettingsMaster"]).X,
                UIHelper.GetElementBGRect(uiElements["VolumeSettingsMaster"]).Bottom + 5));
            UIHelper.SetElementBGRect(uiElements["VolumeSettingsMusic"], new Rectangle(uiElements["VolumeSettingsMusic"].Position.ToPoint(), new Point(125, 25)));
            UIHelper.SetElementRect(uiElements["VolumeSettingsMusic"], new Rectangle(uiElements["VolumeSettingsMusic"].Position.ToPoint(), new Point(125, 25)));

            //Effects volume title
            uiElements.Add("VolumeSettingsEffects", UIHelper.CreateTextblock("VolumeSettingsEffects", "Effect Volume", UIHelper.GetElementBGRect(uiElements["VolumeSettingsMaster"]).X,
                UIHelper.GetElementBGRect(uiElements["VolumeSettingsMusic"]).Bottom + 5));
            UIHelper.SetElementBGRect(uiElements["VolumeSettingsEffects"], new Rectangle(uiElements["VolumeSettingsEffects"].Position.ToPoint(), new Point(125, 25)));
            UIHelper.SetElementRect(uiElements["VolumeSettingsEffects"], new Rectangle(uiElements["VolumeSettingsEffects"].Position.ToPoint(), new Point(125, 25)));


            //Master Volume adjustment buttons
            uiElements.Add("VolumeSettingsMasterBtn-", UIHelper.CreateButton("VolumeSettingsMasterBtn-", "Minus", UIHelper.GetElementBGRect(uiElements["VolumeSettingsMaster"]).Right + 4,
                UIHelper.GetElementBGRect(uiElements["VolumeSettingsMaster"]).Y));
            UIHelper.SetRectangle(uiElements["VolumeSettingsMasterBtn-"], 55, 25);

            uiElements.Add("VolumeSettingsMasterNum", UIHelper.CreateTextblock("VolumeSettingsMasterNum", (MasterVolume * 100).ToString(), 
                UIHelper.GetRectangle(uiElements["VolumeSettingsMasterBtn-"]).Right + 10, UIHelper.GetRectangle(uiElements["VolumeSettingsMasterBtn-"]).Y));
            UIHelper.SetElementBGRect(uiElements["VolumeSettingsMasterNum"], new Rectangle(uiElements["VolumeSettingsMasterNum"].Position.ToPoint(), new Point(30, 25)));
            UIHelper.SetElementRect(uiElements["VolumeSettingsMasterNum"], new Rectangle(uiElements["VolumeSettingsMasterNum"].Position.ToPoint(), new Point(30, 25)));

            uiElements.Add("VolumeSettingsMasterBtn+", UIHelper.CreateButton("VolumeSettingsMasterBtn+", "Add", UIHelper.GetElementBGRect(uiElements["VolumeSettingsMasterNum"]).Right + 4,
         UIHelper.GetElementBGRect(uiElements["VolumeSettingsMasterNum"]).Y));
            UIHelper.SetRectangle(uiElements["VolumeSettingsMasterBtn+"], 55, 25);


            //Music volume adjustment buttons
            uiElements.Add("VolumeSettingsMusicBtn-", UIHelper.CreateButton("VolumeSettingsMusicBtn-", "Minus", UIHelper.GetElementBGRect(uiElements["VolumeSettingsMusic"]).Right + 4,
              UIHelper.GetElementBGRect(uiElements["VolumeSettingsMusic"]).Y));
            UIHelper.SetRectangle(uiElements["VolumeSettingsMusicBtn-"], 55, 25);

            uiElements.Add("VolumeSettingsMusicNum", UIHelper.CreateTextblock("VolumeSettingsMusicNum", (MusicVolume * 100).ToString(),
                UIHelper.GetRectangle(uiElements["VolumeSettingsMasterBtn-"]).Right + 10, UIHelper.GetRectangle(uiElements["VolumeSettingsMusicBtn-"]).Y));
            UIHelper.SetElementBGRect(uiElements["VolumeSettingsMusicNum"], new Rectangle(uiElements["VolumeSettingsMusicNum"].Position.ToPoint(), new Point(30, 25)));
            UIHelper.SetElementRect(uiElements["VolumeSettingsMusicNum"], new Rectangle(uiElements["VolumeSettingsMusicNum"].Position.ToPoint(), new Point(30, 25)));

            uiElements.Add("VolumeSettingsMusicBtn+", UIHelper.CreateButton("VolumeSettingsMusicBtn+", "Add", UIHelper.GetElementBGRect(uiElements["VolumeSettingsMusicNum"]).Right + 4,
         UIHelper.GetElementBGRect(uiElements["VolumeSettingsMusicNum"]).Y));
            UIHelper.SetRectangle(uiElements["VolumeSettingsMusicBtn+"], 55, 25);



            //Effects volume adjustment buttons
            uiElements.Add("VolumeSettingsEffectsBtn-", UIHelper.CreateButton("VolumeSettingsEffectsBtn-", "Minus", UIHelper.GetElementBGRect(uiElements["VolumeSettingsEffects"]).Right + 4,
              UIHelper.GetElementBGRect(uiElements["VolumeSettingsEffects"]).Y));
            UIHelper.SetRectangle(uiElements["VolumeSettingsEffectsBtn-"], 55, 25);

            uiElements.Add("VolumeSettingsEffectsNum", UIHelper.CreateTextblock("VolumeSettingsEffectsNum", (EffectVolume * 100).ToString(),
                UIHelper.GetRectangle(uiElements["VolumeSettingsEffectsBtn-"]).Right + 10, UIHelper.GetRectangle(uiElements["VolumeSettingsEffectsBtn-"]).Y));
            UIHelper.SetElementBGRect(uiElements["VolumeSettingsEffectsNum"], new Rectangle(uiElements["VolumeSettingsEffectsNum"].Position.ToPoint(), new Point(30, 25)));
            UIHelper.SetElementRect(uiElements["VolumeSettingsEffectsNum"], new Rectangle(uiElements["VolumeSettingsEffectsNum"].Position.ToPoint(), new Point(30, 25)));

            uiElements.Add("VolumeSettingsEffectsBtn+", UIHelper.CreateButton("VolumeSettingsEffectsBtn+", "Add", UIHelper.GetElementBGRect(uiElements["VolumeSettingsEffectsNum"]).Right + 4,
         UIHelper.GetElementBGRect(uiElements["VolumeSettingsEffectsNum"]).Y));
            UIHelper.SetRectangle(uiElements["VolumeSettingsEffectsBtn+"], 55, 25);


            //Return Btn
            uiElements.Add("VolumeSettingsReturnBtn", UIHelper.CreateButton("VolumeSettingsReturnBtn", "", UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Right - 65 - (int)(1920),
                UIHelper.GetRectangle(uiElements["SettingsReturnBtn"]).Y));
            UIHelper.SetRectangle(uiElements["VolumeSettingsReturnBtn"], 50, 50);
            SetVolumeSettings(false);
        }

        private void CreateTopDownKeyBinds(int width, int height)
        {
            for (int i = 0; i < TopDownKeyBinds.Length; i++)
            {
                if (i == 0)
                {
                    uiElements.Add("TopDownBindTitle", UIHelper.CreateTextblock("TopDownBindTitle", "       Top Down Key Binds",
                   (UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Center.X - (int)(dims.X)) - (150 / 2), 380));
                    //uiElements["TopDownBindTitle"].Visible = true;
                    UIHelper.SetElementBGRect(uiElements["TopDownBindTitle"], new Rectangle(uiElements["TopDownBindTitle"].Position.ToPoint(), new Point(150, 25)));
                    UIHelper.SetElementRect(uiElements["TopDownBindTitle"], new Rectangle(uiElements["TopDownBindTitle"].Position.ToPoint(), new Point(150, 25)));

                    uiElements.Add("KeyBind" + TopDownKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + TopDownKeyBinds[i], TopDownKeyBinds[i],
                   (int)((UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).X - (dims.X)) + 5), (int)(UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Y + 80)));
                    UIHelper.SetElementRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(64, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(64, height)));

                    uiElements.Add("KeyBindBtn" + TopDownKeyBinds[i],
                        UIHelper.CreateButton("KeyBindBtn" + TopDownKeyBinds[i], TopDownInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Right + 2,
                        UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + TopDownKeyBinds[i]], width, height);
                }
                else if (i <= 5)
                {
                    uiElements.Add("KeyBind" + TopDownKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + TopDownKeyBinds[i], TopDownKeyBinds[i], (int)(uiElements["KeyBind" + TopDownKeyBinds[i - 1]].Position.X),
                   UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i - 1]]).Bottom + 5));
                    UIHelper.SetElementRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(64, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(64, height)));

                    uiElements.Add("KeyBindBtn" + TopDownKeyBinds[i],
                       UIHelper.CreateButton("KeyBindBtn" + TopDownKeyBinds[i], TopDownInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Right + 2,
                       UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + TopDownKeyBinds[i]], width, height);
                }
                else if (i == 6)
                {
                    uiElements.Add("KeyBind" + TopDownKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + TopDownKeyBinds[i], TopDownKeyBinds[i], (int)(uiElements["KeyBind" + TopDownKeyBinds[i - 1]].Position.X),
                  UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i - 1]]).Bottom + 5));
                    UIHelper.SetElementRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(120, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(120, height)));

                    uiElements.Add("KeyBindBtn" + TopDownKeyBinds[i],
                       UIHelper.CreateButton("KeyBindBtn" + TopDownKeyBinds[i], TopDownInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Right + 2,
                       UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + TopDownKeyBinds[i]], width, height);
                }
                else if (i == 7)
                {
                    uiElements.Add("KeyBind" + TopDownKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + TopDownKeyBinds[i], TopDownKeyBinds[i], (int)(uiElements["WeaponWheelTitle"].Position.X),
                   UIHelper.GetElementBGRect(uiElements["WeaponWheelTitle"]).Bottom + 7));
                    UIHelper.SetElementRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(64, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(64, height)));

                    uiElements.Add("KeyBindBtn" + TopDownKeyBinds[i],
                       UIHelper.CreateButton("KeyBindBtn" + TopDownKeyBinds[i], TopDownInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Right + 2,
                       UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + TopDownKeyBinds[i]], width, height);
                }
                else
                {
                    uiElements.Add("KeyBind" + TopDownKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + TopDownKeyBinds[i], TopDownKeyBinds[i], (int)(uiElements["KeyBind" + TopDownKeyBinds[i - 1]].Position.X),
                  UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i - 1]]).Bottom + 5));
                    UIHelper.SetElementRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(64, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]], new Rectangle(uiElements["KeyBind" + TopDownKeyBinds[i]].Position.ToPoint(), new Point(64, height)));

                    uiElements.Add("KeyBindBtn" + TopDownKeyBinds[i],
                       UIHelper.CreateButton("KeyBindBtn" + TopDownKeyBinds[i], TopDownInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Right + 2,
                       UIHelper.GetElementBGRect(uiElements["KeyBind" + TopDownKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + TopDownKeyBinds[i]], width, height);
                }
            }
        }

        private void CreateSideScrollKeyBinds(int width, int height)
        {
            for (int i = 0; i < SideKeyBinds.Length; i++)
            {
                if (i == 0)
                {
                    uiElements.Add("SideBindTitle", UIHelper.CreateTextblock("SideBindTitle", "       Side Scroll Key Binds",
                        (UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Center.X - (int)(dims.X)) - (150 / 2), 380));
                   // uiElements["SideBindTitle"].Visible = true;
                    UIHelper.SetElementBGRect(uiElements["SideBindTitle"], new Rectangle(uiElements["SideBindTitle"].Position.ToPoint(), new Point(150, 25)));
                    UIHelper.SetElementRect(uiElements["SideBindTitle"], new Rectangle(uiElements["SideBindTitle"].Position.ToPoint(), new Point(150, 25)));

                    uiElements.Add("KeyBind" + SideKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + SideKeyBinds[i], SideKeyBinds[i],
                        (int)((UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).X - (dims.X)) + 5), (int)(UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Y + 80)));
                    UIHelper.SetElementRect(uiElements["KeyBind" + SideKeyBinds[i]], new Rectangle(uiElements["KeyBind" + SideKeyBinds[i]].Position.ToPoint(), new Point(64, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]], new Rectangle(uiElements["KeyBind" + SideKeyBinds[i]].Position.ToPoint(), new Point(64, height)));

                    uiElements.Add("KeyBindBtn" + SideKeyBinds[i],
                        UIHelper.CreateButton("KeyBindBtn" + SideKeyBinds[i], SideScrollInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]]).Right + 2,
                        UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + SideKeyBinds[i]], width, height);

                    //uiElements[SideKeyBinds[i]].Visible = true;
                }
                else if (i <= 6)
                {
                    uiElements.Add("KeyBind" + SideKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + SideKeyBinds[i], SideKeyBinds[i], (int)(uiElements["KeyBind" + SideKeyBinds[i - 1]].Position.X),
                        UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i - 1]]).Bottom + 5));
                    UIHelper.SetElementRect(uiElements["KeyBind" + SideKeyBinds[i]], new Rectangle(uiElements["KeyBind" + SideKeyBinds[i]].Position.ToPoint(), new Point(64, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]], new Rectangle(uiElements["KeyBind" + SideKeyBinds[i]].Position.ToPoint(), new Point(64, height)));

                    uiElements.Add("KeyBindBtn" + SideKeyBinds[i],
                       UIHelper.CreateButton("KeyBindBtn" + SideKeyBinds[i], SideScrollInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]]).Right + 2,
                       UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + SideKeyBinds[i]], width, height);

                    //uiElements[SideKeyBinds[i]].Visible = true;
                }
                else if (i == 7)
                {

                    uiElements.Add("WeaponWheelTitle", UIHelper.CreateTextblock("WeaponWheelTitle", "   Weapon Wheel",
                        UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[0]]).X + 300, UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[0]]).Y + 15));
                    //uiElements["WeaponWheelTitle"].Visible = true;
                    UIHelper.SetElementBGRect(uiElements["WeaponWheelTitle"], new Rectangle(uiElements["WeaponWheelTitle"].Position.ToPoint(), new Point(95, height)));
                    UIHelper.SetElementRect(uiElements["WeaponWheelTitle"], new Rectangle(uiElements["WeaponWheelTitle"].Position.ToPoint(), new Point(95, height)));

                    uiElements.Add("KeyBind" + SideKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + SideKeyBinds[i], SideKeyBinds[i], (int)(uiElements["WeaponWheelTitle"].Position.X),
                        UIHelper.GetElementBGRect(uiElements["WeaponWheelTitle"]).Bottom + 7));
                    UIHelper.SetElementRect(uiElements["KeyBind" + SideKeyBinds[i]], new Rectangle(uiElements["KeyBind" + SideKeyBinds[i]].Position.ToPoint(), new Point(64, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]], new Rectangle(uiElements["KeyBind" + SideKeyBinds[i]].Position.ToPoint(), new Point(64, height)));

                    uiElements.Add("KeyBindBtn" + SideKeyBinds[i],
                       UIHelper.CreateButton("KeyBindBtn" + SideKeyBinds[i], SideScrollInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]]).Right + 2,
                       UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + SideKeyBinds[i]], width, height);
                    //uiElements[SideKeyBinds[i]].Visible = true;
                }
                else
                {
                    uiElements.Add("KeyBind" + SideKeyBinds[i], UIHelper.CreateTextblock("KeyBind" + SideKeyBinds[i], SideKeyBinds[i], (int)(uiElements["KeyBind" + SideKeyBinds[i - 1]].Position.X),
                       UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i - 1]]).Bottom + 5));
                    UIHelper.SetElementRect(uiElements["KeyBind" + SideKeyBinds[i]], new Rectangle(uiElements["KeyBind" + SideKeyBinds[i]].Position.ToPoint(), new Point(64, height)));
                    UIHelper.SetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]], new Rectangle(uiElements["KeyBind" + SideKeyBinds[i]].Position.ToPoint(), new Point(64, height)));

                    uiElements.Add("KeyBindBtn" + SideKeyBinds[i],
                       UIHelper.CreateButton("KeyBindBtn" + SideKeyBinds[i], SideScrollInputs[i].ToString(), UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]]).Right + 2,
                       UIHelper.GetElementBGRect(uiElements["KeyBind" + SideKeyBinds[i]]).Y));

                    UIHelper.SetRectangle(uiElements["KeyBindBtn" + SideKeyBinds[i]], width, height);
                    //uiElements[SideKeyBinds[i]].Visible = true;
                }
            }
        }

        public void CreateInteractUI(Point pos, bool bossCount = false)
        {
            if(!uiElements.ContainsKey("InteractBox"))
            {
                uiElements.Add("InteractBox", UIHelper.CreateTextblock("InteractBox", "\n\n    " +
                "["+SideScrollInputs[6].ToString()+"]"+" or (Y)", pos.X, pos.Y));
                UIHelper.SetElementRect(uiElements["InteractBox"], new Rectangle(uiElements["InteractBox"].Position.ToPoint(), new Point(64, 64)));
                UIHelper.SetElementBGRect(uiElements["InteractBox"], new Rectangle(uiElements["InteractBox"].Position.ToPoint(), new Point(64, 64)));
            }
            

            if(bossCount && !uiElements.ContainsKey("InteractBoxBossWarning"))
            {
                uiElements.Add("InteractBoxBossWarning", UIHelper.CreateTextblock("InteractBoxBossWarning", "\n\n            There is still a boss in the area",
               pos.X - (int)(215 / 2), pos.Y - (int)(70 * 1.5f)));
                UIHelper.SetElementRect(uiElements["InteractBoxBossWarning"], new Rectangle(uiElements["InteractBoxBossWarning"].Position.ToPoint(), new Point(128, 96)));
                UIHelper.SetElementBGRect(uiElements["InteractBoxBossWarning"], new Rectangle(uiElements["InteractBoxBossWarning"].Position.ToPoint(), new Point(215, 70)));
            }


            UIHelper.SetElementVisibility("InteractBox", true, uiElements);


        }
        public void RemoveInteractUI()
        {
            if(uiElements.ContainsKey("InteractBox"))
                uiElements.Remove("InteractBox");
            if (uiElements.ContainsKey("InteractBoxBossWarning"))
                uiElements.Remove("InteractBoxBossWarning");

            UIHelper.SetElementVisibility("InteractBox", false, uiElements);

        }

        public void CreateEndGameUI(Rectangle viewRect)
        {
            if(!uiElements.ContainsKey("EndGameBackground"))
            {
                uiElements.Add("EndGameBackground", UIHelper.CreateTextblock("EndGameBackground", "", 0, 0));
                UIHelper.SetElementBGRect(uiElements["MainMenuBackground"], new Rectangle(uiElements["EndGameBackground"].Position.ToPoint(), new Point(1920, 1080)));
                UIHelper.SetElementRect(uiElements["MainMenuBackground"], new Rectangle(uiElements["EndGameBackground"].Position.ToPoint(), new Point(1920, 1080)));

                uiElements["MainMenuBackground"].Visible = true;

             
                UIHelper.SetRectangle(uiElements["PauseMenuReturn"], new Rectangle(viewRect.Center.X + (150/2), viewRect.Center.Y + (viewRect.Height/3), 150, 30));
                UIHelper.SetRectangle(uiElements["PauseMenuQuit"], new Rectangle(viewRect.Center.X + (150 / 2), UIHelper.GetRectangle(uiElements["PauseMenuReturn"]).Bottom + 5, 150, 30));


            }
            else
            {
                uiElements["MainMenuBackground"].Visible = true;
                uiElements["PauseMenuReturn"].Visible = true;
                uiElements["PauseMenuQuit"].Visible = true;
                uiElements["PauseMainMenuBtn"].Visible = false;
            }
        }

        public void CreatePauseMenu(Rectangle viewRect)
        {
            if(!uiElements.ContainsKey("PauseMenuBox"))
            {
                uiElements.Add("PauseMenuBox", UIHelper.CreateTextblock("PauseMenuBox", "", (int)(viewRect.Center.X - (150 / 2)), (int)(viewRect.Center.Y - (150 / 2))));
                UIHelper.SetElementBGRect(uiElements["PauseMenuBox"], new Rectangle(uiElements["PauseMenuBox"].Position.ToPoint(), new Point(200, 200)));
                UIHelper.SetElementRect(uiElements["PauseMenuBox"], new Rectangle(uiElements["PauseMenuBox"].Position.ToPoint(), new Point(200, 200)));

                //uiElements["PauseMenuBox"].Visible = true;

                uiElements.Add("SelectBox", UIHelper.CreateTextblock("SelectBox", "", 0, 0));
                UIHelper.SetElementBGRect(uiElements["SelectBox"], new Rectangle(uiElements["SelectBox"].Position.ToPoint(), new Point(150, 30)));
                UIHelper.SetElementRect(uiElements["SelectBox"], new Rectangle(uiElements["SelectBox"].Position.ToPoint(), new Point(150, 30)));

                uiElements.Add("PauseMainMenuBtn", UIHelper.CreateButton("PauseMainMenuBtn", "    Return to Game",
                    UIHelper.GetElementBGRect(uiElements["PauseMenuBox"]).Center.X - (150 / 2), UIHelper.GetElementBGRect(uiElements["PauseMenuBox"]).Center.Y - (int)(150 / 3)));
                UIHelper.SetRectangle(uiElements["PauseMainMenuBtn"], 150, 30);

                uiElements.Add("PauseMenuReturn", UIHelper.CreateButton("PauseMenuReturn", "    Return to Menu", 
                    UIHelper.GetRectangle(uiElements["PauseMainMenuBtn"]).X, UIHelper.GetRectangle(uiElements["PauseMainMenuBtn"]).Bottom + 20));
                UIHelper.SetRectangle(uiElements["PauseMenuReturn"], 150, 30);

                uiElements.Add("PauseMenuQuit", UIHelper.CreateButton("PauseMenuQuit", "        Quit Game", UIHelper.GetRectangle(uiElements["PauseMenuReturn"]).X,
                    UIHelper.GetRectangle(uiElements["PauseMenuReturn"]).Bottom + 20));
                UIHelper.SetRectangle(uiElements["PauseMenuQuit"], 150, 30);

                uiElements["PauseMenuReturn"].Visible = true;
                uiElements["PauseMainMenuBtn"].Visible = true;
                uiElements["PauseMenuQuit"].Visible = true;

            }
        }

        public void UpdatePauseUI(Rectangle rect)
        {
            if (uiElements.ContainsKey("PauseMenuBox"))
            {
                UIHelper.SetElementBGRect(uiElements["PauseMenuBox"], new Rectangle(new Point(rect.Center.X - (200/2), rect.Center.Y - (200/2)), new Point(200, 200)));
                UIHelper.SetElementRect(uiElements["PauseMenuBox"], new Rectangle(new Point(rect.Center.X - (200 / 2), rect.Center.Y - (200 / 2)), new Point(200, 200)));
               // uiElements["PauseMenuBox"].Visible = true;
            }
              
            if (uiElements.ContainsKey("PauseMainMenuBtn"))
            {
                UIHelper.SetRectangle(uiElements["PauseMainMenuBtn"], 
                    new Rectangle(UIHelper.GetElementBGRect(uiElements["PauseMenuBox"]).Center.X - (150 / 2),
                    UIHelper.GetElementBGRect(uiElements["PauseMenuBox"]).Center.Y - (int)(150 / 4), 150,25));
                uiElements["PauseMainMenuBtn"].Visible = true;
            }
              
            if (uiElements.ContainsKey("PauseMenuReturn"))
            {
                
                UIHelper.SetRectangle(uiElements["PauseMenuReturn"], 
                    new Rectangle(new Point(UIHelper.GetRectangle(uiElements["PauseMainMenuBtn"]).X, UIHelper.GetRectangle(uiElements["PauseMainMenuBtn"]).Bottom + 20), new Point(150,25)));
                uiElements["PauseMenuReturn"].Visible = true;
                
            }

            if (uiElements.ContainsKey("PauseMenuQuit"))
            {
                UIHelper.SetRectangle(uiElements["PauseMenuQuit"],
                   new Rectangle(new Point(UIHelper.GetRectangle(uiElements["PauseMenuReturn"]).X, UIHelper.GetRectangle(uiElements["PauseMenuReturn"]).Bottom + 20), new Point(150, 25)));
                uiElements["PauseMenuQuit"].Visible = true;
            }
                
        }

        public void RemovePauseMenu()
        {
            if (uiElements.ContainsKey("PauseMenuBox"))
                uiElements["PauseMenuBox"].Visible = false;
            if (uiElements.ContainsKey("PauseMainMenuBtn"))
                uiElements["PauseMainMenuBtn"].Visible = false;
            if (uiElements.ContainsKey("PauseMenuReturn"))
                uiElements["PauseMenuReturn"].Visible = false;
            if (uiElements.ContainsKey("PauseMenuQuit"))
                uiElements["PauseMenuQuit"].Visible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (uiElements.ContainsKey("PauseMainMenuBtn") && uiElements["PauseMainMenuBtn"].Visible && uiElements["PauseMenuQuit"].Visible == false)
                uiElements["PauseMainMenuBtn"].Visible = false;
            if (uiElements["VolumeSettingsTitle"].Visible && uiElements["SideBindTitle"].Visible == false && isVolume == false ||
                uiElements["VolumeSettingsTitle"].Visible && uiElements["SideBindTitle"].Visible == true && isVolume == false)
                SetVolumeSettings(false);
            if (isVolume)
            {
                uiElements["SettingsReturnBtn"].Visible = false;
                UIHelper.SetButtonState("SettingsReturnBtn", true, uiElements);
                uiElements["VolumeSettingsReturnBtn"].Visible = true;
                UIHelper.SetButtonState("VolumeSettingsReturnBtn", false, uiElements);

            }
            else
            {
                uiElements["VolumeSettingsReturnBtn"].Visible = false;
                UIHelper.SetButtonState("SettingsReturnBtn", false, uiElements);
                UIHelper.SetButtonState("VolumeSettingsReturnBtn", true, uiElements);
            }
               
         
            foreach (UIWidget widget in uiElements.Values)
                widget.Draw(spriteBatch);
        }
    }
}
