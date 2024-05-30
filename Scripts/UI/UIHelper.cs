using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    static class UIHelper
    {
        public static Texture2D ButtonTexture;
        public static SpriteFont ButtonFont;
        public static SpriteFont CrawlFont;
        public static SpriteFont TitleFont;
        public static SpriteFont TutorialFont;
        public static Texture2D TutorialTexture;
        public static Texture2D CrawlBgTxture;
        public static List<Texture2D> HealthBar;
        public static Texture2D MainMenuBG;
        public static Rectangle Bounds;

        public static UIButton CreateButton(string id, string text, int x, int y)//ButtonTexture Width and Height need to change
        {
            UIButton b = new UIButton(id, new Vector2(x, y), new Vector2(200 ,75), ButtonFont, text, Color.White, ButtonTexture); //RectBounds:  new Vector2( ButtonTexture.Width, ButtonTexture.Height) for fitting exact size of texture
            b.Disabled = false;
            return b;
        }
        public static UITextBlock CreateTextblock(string id, string text, int x, int y)
        {
            switch(id)
            {
                case "TitleCrawl":
                    UITextBlock a = new UITextBlock(id, new Vector2(x, y), Vector2.Zero, CrawlFont, text, Color.White, CrawlBgTxture);
                    return a;
                case "SettingsMenuTitle":
                    UITextBlock c = new UITextBlock(id, new Vector2(x, y), Vector2.Zero, TitleFont, text, Color.White, CrawlBgTxture);
                    return c;
                case "HealthBar":
                    UITextBlock d = new UITextBlock(id, new Vector2(x, y), Vector2.Zero, TitleFont, "", Color.White, HealthBar[HealthBar.Count - 1]);
                    return d;
                case "MainMenuBackground":
                    UITextBlock e = new UITextBlock(id, new Vector2(x, y), Vector2.Zero, TitleFont, "", Color.White, MainMenuBG);
                    return e;
                case "TutorialBoxGame":
                    UITextBlock f = new UITextBlock(id, new Vector2(x, y), Vector2.Zero, TutorialFont, text, Color.Black, TutorialTexture);
                    return f;
                default:
                    UITextBlock b = new UITextBlock(id, new Vector2(x, y), Vector2.Zero, CrawlFont, text, Color.White, CrawlBgTxture);
                    return b;


            }
           
        }
        public static void SetButtonState(string keyWord, Boolean disabled, Dictionary<string, UIWidget> uiElements)
        {
            foreach (string widget in uiElements.Keys)
            {
                if (uiElements[widget].ID.Contains(keyWord))
                    if (uiElements[widget] is UIButton)
                        ((UIButton)uiElements[widget]).Disabled =
                       disabled;
            }
        }
        public static void SetElementVisibility(string keyWord, Boolean visible, Dictionary<string, UIWidget> uiElements)
        {
            foreach (string widget in uiElements.Keys)
            {
                if (uiElements[widget].ID.Contains(keyWord))
                    ((UIWidget)uiElements[widget]).Visible = visible;
            }
        }

        public static bool IsTextBlock(UIWidget uiElement)
        {
            if (uiElement is UITextBlock)
                return true;
            else
                return false;
        }

        public static bool IsButton(UIWidget uiElement)
        {
            if (uiElement is UIButton)
                return true;
            else
                return false;
        }

        public static void SetElementText(UIWidget uiElement, string text)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).Text = text;
        }

        public static void ChangeHealthBar(UIWidget uiElement, int index)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).BackGroundTexture = HealthBar[index];
        }

        public static void SetElementRect(UIWidget uiElement, Rectangle rect)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).Rect = rect;
        }
        
        public static void SetElementBGRect(UIWidget uiElement, Rectangle rect)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).TextureRect = rect;
        }
        public static void UpdateHealthBar(UIWidget uiElement, Rectangle bounds)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).TextureRect = new Rectangle(bounds.X, bounds.Y, ((UITextBlock)uiElement).TextureRect.Width, ((UITextBlock)uiElement).TextureRect.Height);
        }

        public static void UpdateHealthBarX(UIWidget uiElement, int xPos)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).TextureRect = new Rectangle(xPos, ((UITextBlock)uiElement).TextureRect.Y, ((UITextBlock)uiElement).TextureRect.Width, ((UITextBlock)uiElement).TextureRect.Height);
        }

        public static void UpdateHealthBarY(UIWidget uiElement, int yPos)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).TextureRect = new Rectangle(((UITextBlock)uiElement).TextureRect.X, yPos, ((UITextBlock)uiElement).TextureRect.Width, ((UITextBlock)uiElement).TextureRect.Height);
        }

        public static Rectangle GetElementBGRect(UIWidget uiElement)
        {
            if (uiElement is UITextBlock)
                return ((UITextBlock)uiElement).TextureRect;
            else
                return Rectangle.Empty;
        }

        public static Rectangle GetRectangle(UIWidget uiElement)
        {
            if (uiElement is UIButton)
                return ((UIButton)uiElement).Bounds;
            else
                return Rectangle.Empty;
        }

        public static void SetRectangle(UIWidget uiElement, int width, int height)
        {
            if (uiElement is UIButton)
                ((UIButton)uiElement).SetBounds(width, height);
        }

        public static void SetRectangle(UIWidget uiElement, Rectangle rectangle)
        {
            if (uiElement is UIButton)
                ((UIButton)uiElement).Bounds = rectangle;
        }
    }
}
