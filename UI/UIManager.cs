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
                    game.ChangeGameState(Game1.GameScene.InGame);
                    UIHelper.SetElementVisibility("MainMenu", false, uiElements);
                    UIHelper.SetElementVisibility("Settings", false, uiElements);
                    UIHelper.SetElementVisibility("StartNewGame", false, uiElements);
                    UIHelper.SetElementVisibility("LoadGame", false, uiElements);
                    UIHelper.SetElementVisibility("StartGameReturn", false, uiElements);
                    game.StartNewGame();
                    break;
                //case "Left":
                //    tanks[playerNumber].TurretRotation += 0.01f;
                //    break;
                //case "Right":
                //    tanks[playerNumber].TurretRotation -= 0.01f;
                //    break;
                //case "Up":
                //    tanks[playerNumber].GunElevation -= 0.01f;
                //    break;
                //case "Down":
                //    tanks[playerNumber].GunElevation += 0.01f;
                //    break;
                //case "Fire":
                //    Vector3 fireAngle = Vector3.Zero;
                //    float rotation = tanks[playerNumber].TankRotation;
                //    rotation += tanks[playerNumber].TurretRotation;
                //    float elevation = tanks[playerNumber].GunElevation;

                    //    Matrix rotMatrix = Matrix.CreateFromYawPitchRoll(rotation, MathHelper.ToRadians(90) + elevation, 0);

                    //    fireAngle = Vector3.Transform(Vector3.Up, rotMatrix);
                    //    fireAngle.Normalize();


                    //    if (tanks[playerNumber] == tanks[0])
                    //    {
                    //        ShotManager.FireShot(tanks[playerNumber].Position + new Vector3(0f, 1f, 0f) + fireAngle * 2, fireAngle * shotPower);
                    //    }
                    //    else
                    //    {
                    //        ShotManager.FireShot(tanks[playerNumber].Position + new Vector3(0f, 1f, 0f) + fireAngle * 2, fireAngle * shotPower2);
                    //    }
                    //    break;
                    //case "PowerUp":
                    //    if (tanks[playerNumber] == tanks[0])
                    //        shotPower += .5f;
                    //    if (tanks[playerNumber] == tanks[1])
                    //        shotPower2 += .5f;
                    //    break;
                    //case "PowerDown":
                    //    if (tanks[playerNumber] == tanks[0])
                    //        shotPower -= .5f;
                    //    if (tanks[playerNumber] == tanks[1])
                    //        shotPower2 -= .5f;
                    //    break;
            }
        }

        public void UpdateTextBlock(string keyWord)
        {
            switch(keyWord)
            {
                case "TitleCrawl":
                    StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/TextFiles/TitleCrawl.txt");
                    string text = sr.ReadToEnd();
                    UIHelper.SetElementText(uiElements[keyWord], text);
                    break;
                case "MainMenuTitle":
                    StreamReader sr1 = new StreamReader(Directory.GetCurrentDirectory() + "/TextFiles/MainMenuTitle.txt");
                    string text1 = sr1.ReadToEnd();
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

        public void UpdateButton(string keyWord, float moveSpeed)
        {
            switch(keyWord)
            {
                case "StartGame":
                    if(uiElements["StartNewGame"].Position.Y > uiElements["MainMenuPlay"].Position.Y)
                    {
                        uiElements["StartNewGame"].SetY(uiElements["StartNewGame"].Position.Y - moveSpeed);
                    }
                    if (uiElements["StartNewGame"].Position.Y == uiElements["MainMenuPlay"].Position.Y)
                    {
                        uiElements["StartNewGame"].SetY(uiElements["MainMenuPlay"].Position.Y);
                        uiElements["MainMenuPlay"].SetY(buttonPos.Y);

                        UIHelper.SetRectangle(uiElements["StartNewGame"], new Rectangle((int)uiElements["StartNewGame"].Position.X, (int)uiElements["StartNewGame"].Position.Y,
                            UIHelper.GetRectangle(uiElements["StartNewGame"]).Width, UIHelper.GetRectangle(uiElements["StartNewGame"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuPlay"], new Rectangle((int)uiElements["MainMenuPlay"].Position.X, (int)uiElements["MainMenuPlay"].Position.Y,
                           UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));

                    }

                    if (uiElements["LoadGame"].Position.Y > uiElements["MainMenuExit"].Position.Y)
                    {
                        uiElements["LoadGame"].SetY(uiElements["LoadGame"].Position.Y - moveSpeed);
                    }
                    if (uiElements["LoadGame"].Position.Y == uiElements["MainMenuExit"].Position.Y)
                    {
                        uiElements["LoadGame"].SetY(uiElements["MainMenuExit"].Position.Y);
                        uiElements["MainMenuExit"].SetY(buttonPos2.Y);

                        UIHelper.SetRectangle(uiElements["LoadGame"], new Rectangle((int)uiElements["LoadGame"].Position.X, (int)uiElements["LoadGame"].Position.Y,
                           UIHelper.GetRectangle(uiElements["LoadGame"]).Width, UIHelper.GetRectangle(uiElements["LoadGame"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuExit"], new Rectangle((int)uiElements["MainMenuExit"].Position.X, (int)uiElements["MainMenuExit"].Position.Y,
                          UIHelper.GetRectangle(uiElements["MainMenuExit"]).Width, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Height));
                    }
                    break;
                case "MainMenu":
                    if (uiElements["MainMenuPlay"].Position.Y > uiElements["StartNewGame"].Position.Y)
                    {
                        uiElements["MainMenuPlay"].SetY(uiElements["MainMenuPlay"].Position.Y - moveSpeed);
                    }
                    if(uiElements["MainMenuPlay"].Position.Y == uiElements["StartNewGame"].Position.Y)
                    {
                        uiElements["MainMenuPlay"].SetY(uiElements["StartNewGame"].Position.Y);
                        uiElements["StartNewGame"].SetY(buttonPos.Y);

                        UIHelper.SetRectangle(uiElements["StartNewGame"], new Rectangle((int)uiElements["StartNewGame"].Position.X, (int)uiElements["StartNewGame"].Position.Y,
                          UIHelper.GetRectangle(uiElements["StartNewGame"]).Width, UIHelper.GetRectangle(uiElements["StartNewGame"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuPlay"], new Rectangle((int)uiElements["MainMenuPlay"].Position.X, (int)uiElements["MainMenuPlay"].Position.Y,
                           UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Width, UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Height));
                    }

                    if (uiElements["MainMenuExit"].Position.Y > uiElements["LoadGame"].Position.Y)
                    {
                        uiElements["MainMenuExit"].SetY(uiElements["MainMenuExit"].Position.Y - moveSpeed);
                    }
                    if (uiElements["MainMenuExit"].Position.Y == uiElements["LoadGame"].Position.Y)
                    {
                        uiElements["MainMenuExit"].SetY(uiElements["LoadGame"].Position.Y);
                        uiElements["LoadGame"].SetY(buttonPos2.Y);

                        UIHelper.SetRectangle(uiElements["LoadGame"], new Rectangle((int)uiElements["LoadGame"].Position.X, (int)uiElements["LoadGame"].Position.Y,
                           UIHelper.GetRectangle(uiElements["LoadGame"]).Width, UIHelper.GetRectangle(uiElements["LoadGame"]).Height));

                        UIHelper.SetRectangle(uiElements["MainMenuExit"], new Rectangle((int)uiElements["MainMenuExit"].Position.X, (int)uiElements["MainMenuExit"].Position.Y,
                          UIHelper.GetRectangle(uiElements["MainMenuExit"]).Width, UIHelper.GetRectangle(uiElements["MainMenuExit"]).Height));
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
            uiElements.Add("TitleCrawl", UIHelper.CreateTextblock("TitleCrawl", "This is the title crawl", ((int)dims.X / 2) - (550/2), 20)); //Replace 550 with UIHelper.ScrollBG.Width
            UIHelper.SetElementRect(uiElements["TitleCrawl"], new Rectangle(new Point((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["TitleCrawl"], new Rectangle((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y, 550, (int)(dims.Y + (dims.Y/4))));

            ///Main Menu elements
            //Title
            uiElements.Add("MainMenuTitle", UIHelper.CreateTextblock("MainMenuTitle", "AUTO-Matic", ((int)dims.X / 2) - (450 / 2), //450 with UIHelper.MenuTitle.Width
                UIHelper.GetElementBGRect(uiElements["TitleCrawl"]).Height + 100));
            UIHelper.SetElementRect(uiElements["MainMenuTitle"], new Rectangle(new Point((int)(uiElements["MainMenuTitle"].Position.X + 180), (int)uiElements["MainMenuTitle"].Position.Y + 90), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["MainMenuTitle"], new Rectangle((int)uiElements["MainMenuTitle"].Position.X, (int)uiElements["MainMenuTitle"].Position.Y, 450, 200));
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
            uiElements.Add("SettingsMenuTitle", UIHelper.CreateTextblock("SettingsMenuTitle", titleTxt, (int)(((dims.X * 2)) - (titleTxt.Length * 11.0f)), // dividing the txt * 11 / 2 makes in longer?
                UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Top - 30 ));
            UIHelper.SetElementRect(uiElements["SettingsMenuTitle"], new Rectangle(uiElements["SettingsMenuTitle"].Position.ToPoint(), new Point(titleTxt.Length, 20)));

            //Settings Box
            uiElements.Add("SettingsButtonBox", UIHelper.CreateTextblock("SettingsButtonBox", "The options for settings will go here", (int)(((dims.X * 2))) - (525 / 2),
                (int)uiElements["SettingsMenuTitle"].Position.Y + 50));
            UIHelper.SetElementRect(uiElements["SettingsButtonBox"], new Rectangle(uiElements["SettingsButtonBox"].Position.ToPoint(), new Point(100, 10)));
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

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (UIWidget widget in uiElements.Values)
                widget.Draw(spriteBatch);
        }
    }
}
