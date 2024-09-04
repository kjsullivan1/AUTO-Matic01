using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    class UITextBlock : UIWidget
    {
        public Vector2 TextOffset { get; set; }
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Color TextTint { get; set; }

        public Rectangle Rect { get; set; }
        public float Scale { get; set; }

        SpriteFont font;

        public Texture2D BackGroundTexture { get; set; }
        public Rectangle TextureRect { get; set; }
        public UITextBlock(string id, Vector2 position, Vector2 textOffset, SpriteFont font, string text, Color textTint)
            : base(id, position)
        {
            TextOffset = textOffset;
            Font = font;
            Text = text;
            TextTint = textTint;
        }

        public UITextBlock(string id, Vector2 position, Vector2 textOffset, SpriteFont font, string text, Color textTint, Texture2D bgTxture)
    : base(id, position)
        {
            TextOffset = textOffset;
            Font = font;
            Text = text;
            TextTint = textTint;
            this.font = font;
            BackGroundTexture = bgTxture;
            Scale = 1;

            if(Text != null && Text.Length > 0 && Rect != Rectangle.Empty)
            {
              
                int endOfLineX = Text.Length;
                if (endOfLineX > Rect.Width)
                {
                    string temp = "";
                    string temp2 = temp;
                    int i = 0;
                    int difference = Text.Length - temp.Length;
                    while (temp.Length < Text.Length)
                    {
                        if (difference > Rect.Width)
                        {
                            temp += Text.Substring(i, Rect.Width);

                            temp2 += Text.Substring(i, Rect.Width) + "\n";
                        }
                        else
                        {
                            temp += Text.Substring(i, difference);
                            temp2 += Text.Substring(i, difference) + "\n";
                        }

                        i = temp.Length;
                        //temp = temp2;

                        difference = Text.Length - temp.Length;

                    }


                    Text = temp2;
                }
            }
            
           

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
          
            if (Visible)
            {
                //for (int i = 0; i < Text.Length; i++)
                //{
                //    if (font.Characters.Contains(Text[i]) == false)
                //    {
                //        char text = Text[i];
                //        Text.Remove(i, 1);
                //    }
                     
                //}

                spriteBatch.Draw(BackGroundTexture,TextureRect, color: Color.White);
                if(Text == null)
                    spriteBatch.DrawString(spriteFont: Font, text: "", position: new Vector2(Rect.X, Rect.Y) + TextOffset, color: TextTint, rotation: 0, origin: new Vector2(0, 0), scale: Scale, SpriteEffects.None, layerDepth: 0);
                else
                    spriteBatch.DrawString(spriteFont: Font, text: Text, position: new Vector2(Rect.X, Rect.Y) + TextOffset, color: TextTint, rotation: 0, origin: new Vector2(0, 0), scale: Scale, SpriteEffects.None, layerDepth: 0);


                //Draw texture one layer back
            }

            base.Draw(spriteBatch);
        }

    }
}
