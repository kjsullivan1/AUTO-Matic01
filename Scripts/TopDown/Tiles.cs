using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic.TopDown
{
    class Tiles
    {
        public Texture2D texture;

        private Rectangle rectangle;
        public int[] mapPoint;
        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }


        public void setX(int x)
        {
            rectangle.X = x;
        }
        public void setY(int y)
        {
            rectangle.Y = y;
        }

        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }

    class EnemySpawn :Tiles
    {
        //create enemy
    }

    class FloorTiles : Tiles
    {
        public FloorTiles(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("TopDown/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            //mapPoint = i;
        }
    }

    class WallTiles : Tiles
    {
        public WallTiles(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("TopDown/MapTiles/Tile" + i);
            this.Rectangle = newRect;
           // mapPoint = i;
        }
    }

    class PlayerSpawn :Tiles
    {
        //sawn player
    }
}
