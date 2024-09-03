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
    class BossHealthBar : HealthBar
    {
        //Rectangle rectangle;

        List<Texture2D> healthBars = new List<Texture2D>();
        public BossHealthBar(Rectangle rect, ContentManager content) : base(rect, content, 20)
        {
            this.rect = rect;
            this.health = 20;
            this.maxHealth = 20;
            this.maxWidth = rect.Width;
            this.content = content;

            for(int i = 0; i <= 20; i++)
            {
                healthBars.Add(content.Load<Texture2D>("TopDown/BossHealthBar/BossHealth" + i));
            }
        }

        public new void RecieveDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
                health = 0;
           
        }

        public void ChangeHealth(float health)
        {
            this.health = health;

            if(this.health >= maxHealth)
            {
                this.health = maxHealth;
            }
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(healthBars[(int)health], rect, Color.White);
        }
    }
}
