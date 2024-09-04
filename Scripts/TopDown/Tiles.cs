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
        public void Draw(SpriteBatch spriteBatch, string dir)
        {
            switch (dir)
            {
                case "right":
                    spriteBatch.Draw(texture, rectangle, Color.White);
                    break;
                case "left":
                    spriteBatch.Draw(texture, destinationRectangle: rectangle, color: Color.White, effects: SpriteEffects.FlipHorizontally);
                    break;
                case "up":
                    spriteBatch.Draw(texture, rectangle, Color.White);
                    break;
                case "down":
                    spriteBatch.Draw(texture, destinationRectangle: rectangle, color: Color.White, effects: SpriteEffects.FlipVertically);
                    break;
            }
        }
    }

    class EnemySpawn :Tiles
    {
        
        public EnemySpawn(int[] spawnPoints, Rectangle rect)
        {
            this.mapPoint = spawnPoints;
            this.Rectangle = rect;
        }
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



    class SlamTiles : Tiles
    {
        public SlamTiles(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("TopDown/MapTiles/Tile" + i);
            this.Rectangle = newRect;
        }
    }

    class EnvironmentTile : Tiles
    {
        public enum Type { SpeedBoost, DamageOverTime}
        public Type effectType = Type.SpeedBoost;
        public string direction;
        public EnvironmentTile(int i, Rectangle newRect, string dir)
        {
            if(i == 62 || i == 59) //Speed Boost Tiles   //62 is facing right and 59 is facing up
            {
                effectType = Type.SpeedBoost;
            }
            else if(i == 60 || i ==  63)//Damage over time //60 faces vertical and 63 faces horizontal
            {
                effectType = Type.DamageOverTime;
            }

            texture = Content.Load<Texture2D>("TopDown/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            direction = dir;
        }

           //switch(dir)
           //     {
           //         case "right":
           //             break;
           //         case "left":
           //             break;
           //         case "up":
           //             break;
           //         case "down":
           //             break;
           //     }
}

    class PlayerSpawn :Tiles
    {
        //sawn player
    }
}
