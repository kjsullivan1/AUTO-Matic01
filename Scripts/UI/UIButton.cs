using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    class UIButton : UIWidget
    {
        public Texture2D Texture { get; set; }
        public bool Disabled { get; set; }
        public bool Pressed { get; set; }
        public Rectangle Bounds { get; set; }

        public Vector2 TextOffset { get; set; }
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Color txtColor { get; set; }

        #region Event-related Items
        public delegate void ClickHandler(object sender, UIButtonArgs e);
        public event ClickHandler Clicked;
        #endregion

        #region Constructor
        public UIButton(string id, Vector2 position, Vector2 RectBounds, SpriteFont font, string text, Color textTint, Texture2D texture) : base(id, position)
        {
            Texture = texture;
            this.Bounds = new Rectangle(
            (int)position.X,
            (int)position.Y,
            (int)RectBounds.X,
            (int)RectBounds.Y);
            TextOffset = new Vector2((RectBounds.X / 2) - 15, (RectBounds.Y / 2) - 10);
            Font = font;
            Text = text;
            txtColor = textTint;
        }
        #endregion

        public void SetBounds(int width, int height)
        {
            this.Bounds = new Rectangle((int)Bounds.X, (int)Bounds.Y, width, height);
        }

        #region Helper Methods
        public bool Contains(Point location)
        {
            return Visible && Bounds.Contains(location);
        }
        public bool Contains(Vector2 location)
        {
            return Contains(new Point((int)location.X, (int)location.Y));
        }
        public void HitTest(Point location)
        {
            if (Visible && !Disabled)
            {
                if (Contains(location))
                {
                    Pressed = true;
                    Clicked(
                    this,
                    new UIButtonArgs(
                    this.ID,
                    new Vector2(location.X, location.Y)));
                }
                else
                {
                    Pressed = false;
                }
            }
        }
        #endregion


        #region Draw
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                Point drawBase = Point.Zero;
                if (Disabled)
                    drawBase = new Point(0, Bounds.Height);//Disabled image
                if (Pressed)
                    drawBase = new Point(0, Bounds.Height * 2);//Make the pressed image
              
                spriteBatch.Draw(Texture, Bounds, Color.White);
                spriteBatch.DrawString(Font, Text, new Vector2(Bounds.X + TextOffset.X, Bounds.Y + TextOffset.Y), Color.Black);

            }
            base.Draw(spriteBatch);
        }
        #endregion
    
    }
}
