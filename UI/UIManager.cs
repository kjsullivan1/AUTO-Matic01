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

        public void UIButton_Clicked(object sender, UIButtonArgs e)
        {
            string buttonName = e.ID;
            switch (buttonName)
            {
                case "ExitButton":
                    game.Exit();
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
            this.game = game;
            uiElements.Add("TitleCrawl", UIHelper.CreateTextblock("TitleCrawl", "This is the title crawl", (int)dims.X / 7, 20));
            UIHelper.SetElementRect(uiElements["TitleCrawl"], new Rectangle(new Point((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["TitleCrawl"], new Rectangle((int)uiElements["TitleCrawl"].Position.X, (int)uiElements["TitleCrawl"].Position.Y, 550, (int)(dims.Y + (dims.Y/2))));

            ////Main Menu elements
            //Title
            uiElements.Add("MainMenuTitle", UIHelper.CreateTextblock("MainMenuTitle", "AUTO-Matic", (int)(dims.X / 4.75f), UIHelper.GetElementBGRect(uiElements["TitleCrawl"]).Height + 100));
            UIHelper.SetElementRect(uiElements["MainMenuTitle"], new Rectangle(new Point((int)(uiElements["MainMenuTitle"].Position.X + 180), (int)uiElements["MainMenuTitle"].Position.Y + 90), new Point(80, 40)));
            UIHelper.SetElementBGRect(uiElements["MainMenuTitle"], new Rectangle((int)uiElements["MainMenuTitle"].Position.X, (int)uiElements["MainMenuTitle"].Position.Y, 450, 200));
            //Exit Button
            uiElements.Add("ExitButton", UIHelper.CreateButton("ExitButton", "Exit", (int)(dims.X - ((dims.X / 8) + 200)) , UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Bottom + 20));
            //Play
            uiElements.Add("PlayButton", UIHelper.CreateButton("PlayButton", "Play", (int)(dims.X / 8), UIHelper.GetElementBGRect(uiElements["MainMenuTitle"]).Bottom + 20));
            

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
