using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic.Scripts
{
    class HealthBar
    {
        public ContentManager content;
        public Rectangle rect;
        public float health;
        public float maxHealth;
        public int maxWidth;

        public HealthBar(Rectangle rectangle, ContentManager contentManager, float health)
        {
            rect = rectangle;
            content = contentManager;
            this.health = health;
            maxHealth = health;
            maxWidth = rectangle.Width;
        }

        public void RecieveDamage(float dmg)
        {
            health -= dmg;
            float healthDiff = maxHealth - health;
            int segmentedHealth = maxWidth/(int)maxHealth;
            int widthCalc = (int)(segmentedHealth * healthDiff);

            if(widthCalc < 0)
            {
                widthCalc = 0;
            }

            rect = new Rectangle(rect.X, rect.Y, maxWidth - widthCalc, rect.Height);
        }

        public void Update(Point pos)
        {
            rect = new Rectangle(pos.X, pos.Y, rect.Width, rect.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(content.Load<Texture2D>(@"Textures/white"), rect, Color.Red);
        }
    }
}
