using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic.Scripts
{
    class HealthDrop
    {
        public static Texture2D texture;
        public Rectangle rect;

        public HealthDrop(Rectangle positionRect)
        {
            rect = new Rectangle((positionRect.X + positionRect.Width / 2) - texture.Width / 2, (positionRect.Y + positionRect.Height / 2), 28, 28);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, Color.White);
        }
    }
}
