﻿using Microsoft.Xna.Framework;
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



      

        public void UIButton_Clicked(object sender, UIButtonArgs e)
        {
            string buttonName = e.ID;
            switch (buttonName)
            {
                case "MainMenuExit":
                    game.Exit();
                    break;
                case "MainMenuPlay":
                    break;
                case "MainMenuSetting":
                    game.ChangeMenuState(Game1.MenuStates.Settings);
                    break;
                case "SettingsReturnBtn":
                    game.ChangeMenuState(Game1.MenuStates.MainMenu);
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

        public void CreateUIElements(Vector2 dims, Game1 game)
        {
            //filePath = content;
            //Title crawl elements
            uiElements.Clear();
            this.game = game;
            uiElements.Add("TitleCrawl", UIHelper.CreateTextblock("TitleCrawl", "This is the title crawl", ((int)dims.X / 2) - (550/2), 20)); //Replace 550 with UIHelper.ScrollBG.Width
            UIHelper.SetElementRect(uiElements["TitleCrawl"], new Rectangle(new Point((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["TitleCrawl"], new Rectangle((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y, 550, (int)(dims.Y + (dims.Y/2))));

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
            uiElements.Add("SettingsReturnBtn", UIHelper.CreateButton("SettingsReturnBtn", "", UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Right - 65,
                UIHelper.GetElementBGRect(uiElements["SettingsButtonBox"]).Bottom - 65));
            UIHelper.SetRectangle(uiElements["SettingsReturnBtn"], 50, 50);



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