using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    class UIWidget
    {
        public string ID { get; private set; }
        public bool Visible { get; set; }
        public Vector2 Position { get; set; }

        public UIWidget(string id, Vector2 position)
        {
            ID = id;
            Position = position;
            Visible = false;
        }

        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
