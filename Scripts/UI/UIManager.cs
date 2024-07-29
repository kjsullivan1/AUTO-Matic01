using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        List<Rectangle> savedPos = new List<Rectangle>();

        public void UIButton_Clicked(object sender, UIButtonArgs e)
        {
            string buttonName = e.ID;
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
                    break;
                case "SettingsReturnBtn":
                    game.ChangeMenuState(Game1.MenuStates.MainMenu);
                    transition = true;
                   
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
                    game.ChangeGameState(Game1.Scenes.InGame);
                    UIHelper.SetElementVisibility("MainMenu", false, uiElements);
                    UIHelper.SetElementVisibility("Settings", false, uiElements);
                    UIHelper.SetElementVisibility("StartNewGame", false, uiElements);
                    UIHelper.SetElementVisibility("LoadGame", false, uiElements);
                    UIHelper.SetElementVisibility("StartGameReturn", false, uiElements);
                    game.StartNewGame();
                    break;
              
            }
        }

        public void UpdateTextBlock(string keyWord)
        {
            switch(keyWord)
            {
                case "TitleCrawl":
                    //StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/TextFiles/TitleCrawl.txt");
                    string text = "This is the text crawl kjdashgflkjaswhfuksehfjkashdfjashdfkjahsfdkjhaskjdfhsakjfhjksahfjsdhfjkashdfjkhasgjkfhasdkfhasjkdfhjaskhdfjkashdfjkashdfkjashdfkjashdflkjhaskljdfhaskjdhfakjshdfkjashdfjkashdfkjdashfkjashfkjshadjkfhaskjdfhaskjhdfkjashdfkjashfjkahskjfdhaskjdfhaskjdfhkjashfdlkjashlkfsdhfkjashdfkjahlsdfkjhasjdfkhaslkdjfhaskjdfhkalsjdhfkjashdfjkashdfkjashdlfk";
                    UIHelper.SetElementText(uiElements[keyWord], text);
                    if(UIHelper.GetElementBGRect(uiElements[keyWord]).Bottom > 0)
                    {
                        UIHelper.SetElementBGRect(uiElements[keyWord], 
                            new Rectangle(UIHelper.GetElementBGRect(uiElements[keyWord]).X, (int)(UIHelper.GetElementBGRect(uiElements[keyWord]).Y - 1.5f),
                            UIHelper.GetElementBGRect(uiElements[keyWord]).Width, UIHelper.GetElementBGRect(uiElements[keyWord]).Height));

                        UIHelper.SetElementRect(uiElements[keyWord],
                           new Rectangle(UIHelper.GetElementRect(uiElements[keyWord]).X, (int)(UIHelper.GetElementRect(uiElements[keyWord]).Y - 1.5f),
                           UIHelper.GetElementRect(uiElements[keyWord]).Width, UIHelper.GetElementRect(uiElements[keyWord]).Height));
                    }
                    break;
                case "MainMenuTitle":
                    //StreamReader sr1 = new StreamReader(Directory.GetCurrentDirectory() + "/TextFiles/MainMenuTitle.txt");
                    string text1 = "";
                    UIHelper.SetElementText(uiElements[keyWord], text1);
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

        public void UpdateButton(string keyWord, float moveSpeed)
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
                          new Rectangle(((int)dims.X / 2) - (450 / 2), UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Y,
                          UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Width, UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Height));

                            UIHelper.SetElementRect(uiElements["MainMenuTitle"],
                        new Rectangle(((int)dims.X / 2) - (450 / 2), UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Y,
                        UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Width, UIHelper.GetElementRect(uiElements["MainMenuTitle"]).Height));


                            UIHelper.SetRectangle(uiElements["MainMenuPlay"], new Rectangle(((int)dims.X / 2) - (200 / 2),
                                UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Y, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));

                            UIHelper.SetRectangle(uiElements["MainMenuExit"], new Rectangle(((int)dims.X / 2) - (200 / 2),
                               UIHelper.GetRectangle(uiElements["MainMenuExit"]).Y, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Width, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Height));

                            UIHelper.SetRectangle(uiElements["MainMenuSetting"], new Rectangle(((int)dims.X / 2) - (200 / 2),
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
                    if(UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Right > -100)
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
                new Point(768, 432)));

            uiElements.Add("TitleCrawl", UIHelper.CreateTextblock("TitleCrawl", "This is t he title crawl", ((int)dims.X / 2) - (550/2), (int)(dims.Y /2))); //Replace 550 with UIHelper.ScrollBG.Width
            UIHelper.SetElementRect(uiElements["TitleCrawl"], new Rectangle(new Point((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["TitleCrawl"], new Rectangle((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y, 550, (int)(dims.Y + (dims.Y/4))));

         

            ///Main Menu elements
            //Title
            uiElements.Add("MainMenuTitle", UIHelper.CreateTextblock("MainMenuTitle", "", ((int)dims.X / 2) - (UIHelper.MenuTitle.Width / 2), //450 with UIHelper.MenuTitle.Width
                ((int)dims.Y /2) - (int)(UIHelper.MenuTitle.Height + 50)));
            UIHelper.SetElementRect(uiElements["MainMenuTitle"], new Rectangle(new Point((int)(uiElements["MainMenuTitle"].Position.X + 180), (int)uiElements["MainMenuTitle"].Position.Y + 90), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["MainMenuTitle"], new Rectangle((int)uiElements["MainMenuTitle"].Position.X, (int)uiElements["MainMenuTitle"].Position.Y, UIHelper.MenuTitle.Width, UIHelper.MenuTitle.Height));
            //Play
            uiElements.Add("MainMenuPlay", UIHelper.CreateButton("MainMenuPlay", "Play", ((int)dims.X / 2) - (200 / 2), //Multiplying the width * 2 in the Draw Method
                UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Bottom + 20));
            //Exit Button
            uiElements.Add("MainMenuExit", UIHelper.CreateButton("MainMenuExit", "Exit", ((int)dims.X / 2) - (200 / 2), 
                UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Bottom + 20));
            //Settings
            uiElements.Add("MainMenuSetting", UIHelper.CreateButton("MainMenuSetting", "", UIHelper.GetRectangle(uiElements["MainMenuExit"]).Right + 100, //Using width/2 of button
                /*(int)(dims.Y) - 60))*/  UIHelper.GetRectangle(uiElements["MainMenuExit"]).Top + 25)); //Height of Exit button - Height of Settings button 
            UIHelper.SetRectangle(uiElements["MainMenuSetting"], 50, 50);

            ///Settings Menu elements
            //Title
            string titleTxt = "Settings";
            uiElements.Add("SettingsMenuTitle", UIHelper.CreateTextblock("SettingsMenuTitle", titleTxt, (int)(((dims.X * 1.5f)) - (titleTxt.Length * 11.0f)), // dividing the txt * 11 / 2 makes in longer?
                UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Top - 100 ));
            UIHelper.SetElementRect(uiElements["SettingsMenuTitle"], new Rectangle(uiElements["SettingsMenuTitle"].Position.ToPoint(), new Point(titleTxt.Length, 20)));

            //Settings Box
            uiElements.Add("SettingsButtonBox", UIHelper.CreateTextblock("\n\tSettingsButtonBox", "Hello", (int)(((dims.X * 1.5f))) - (525 / 2),
                (int)uiElements["SettingsMenuTitle"].Position.Y + 50));
            UIHelper.SetElementRect(uiElements["SettingsButtonBox"], new Rectangle(uiElements["SettingsButtonBox"].Position.ToPoint(), new Point(350, 100)));
            UIHelper.SetElementBGRect(uiElements["SettingsButtonBox"], new Rectangle(uiElements["SettingsButtonBox"].Position.ToPoint(), new Point(525, 350)));

            //Return Button
            uiElements.Add("SettingsReturnBtn", UIHelper.CreateButton("SettingsReturnBtn", "", UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Right - 65, //50+15
                UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Bottom - 65));
            UIHelper.SetRectangle(uiElements["SettingsReturnBtn"], 50, 50);

            ///Start Menu Elements
            //New Game Btn
            uiElements.Add("StartNewGame", UIHelper.CreateButton("StartNewGame", "New", ((int)dims.X / 2) - (200 / 2),
                (int)(UIHelper.GetElementBGRect(uiElements["TitleCrawl"]).Bottom + dims.Y)));
            buttonPos = uiElements["StartNewGame"].Position;
            //Load Game
            uiElements.Add("LoadGame", UIHelper.CreateButton("LoadGame", "Load", ((int)dims.X / 2) - (200 / 2),
                (int)(UIHelper.GetRectangle(uiElements["StartNewGame"]).Bottom + 20)));
            buttonPos2 = uiElements["LoadGame"].Position;
            //Return 
            uiElements.Add("StartGameReturn", UIHelper.CreateButton("StartGameReturn", "", (int)uiElements["MainMenuSetting"].Position.X, (int)uiElements["MainMenuSetting"].Position.Y));
            UIHelper.SetRectangle(uiElements["StartGameReturn"], 50, 50);

            uiElements.Add("HealthBar", UIHelper.CreateTextblock("HealthBar", "", 64, 64));
            UIHelper.SetElementRect(uiElements["HealthBar"], new Rectangle(uiElements["HealthBar"].Position.ToPoint(), new Point(1280, 512)));
            UIHelper.SetElementBGRect(uiElements["HealthBar"], new Rectangle(uiElements["HealthBar"].Position.ToPoint(), new Point(1280/6, 512/6)));

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

        public void CreateTutorialUI(Vector2 dims, Game1 game)
        {
            if(uiElements.ContainsKey("TutorialBoxGame") == false)
            {
                uiElements.Add("TutorialBoxGame", UIHelper.CreateTextblock("TutorialBoxGame", "" +
                               "\n\n  Use WASD or Left Analog Stick to move" +
                               "\n\n  Press space bar or (A) button to jump" +
                               "\n\n  Hold down to get into shooting stance and press Enter key or (X) button to fire" +
                               "\n\n  If you time it right while in the air, you can ground pound" +
                               "\n\n  Press LShift key or (B) button to dash forwards" +
                               "\n\n  The E key and (Y) button allows you to open doors and go into the dungeon" +
                               "\n\n  Hold them to jump out and play as the pilot\n\n", (int)(dims.X + 192/2), (int)(dims.Y - 300)));
                UIHelper.SetElementRect(uiElements["TutorialBoxGame"], new Rectangle(uiElements["TutorialBoxGame"].Position.ToPoint(), new Point(450, 600)));
                UIHelper.SetElementBGRect(uiElements["TutorialBoxGame"], new Rectangle(uiElements["TutorialBoxGame"].Position.ToPoint(), new Point(400, 225)));
            }
           
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (UIWidget widget in uiElements.Values)
                widget.Draw(spriteBatch);
        }
    }
}
