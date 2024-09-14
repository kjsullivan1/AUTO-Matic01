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
    class Tile
    {
        public Texture2D texture;

        private Rectangle rectangle;
        public Vector2 position;
        public int[,] mapPoint;
        public int[] MapPoint;
        public bool active = true;
        public int index = 0;
        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }


        public void setX(float x)
        {
            position.X = x;
        }
        public void setY(float y)
        {
            position.Y = y;
        }

        private static ContentManager content;
        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(active)
            {
                rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);
                spriteBatch.Draw(texture, rectangle, Color.White);
            }
         
        }
    }


    class RepeatBackground:Tile
    {
        public RepeatBackground(Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/BG2");
            this.Rectangle = newRect;
            position = new Vector2(newRect.X, newRect.Y);
        }
    }
    class GroundTile : Tile
    {
        public GroundTile(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            position = new Vector2(newRect.X, newRect.Y);
        }

    }
   

    class WallTile:Tile
    {
        public WallTile(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            position = new Vector2(newRect.X, newRect.Y);
        }
    }

    class PlatformTile:Tile
    {
        public PlatformTile(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            this.MapPoint = new[] { newRect.Y / newRect.Height, newRect.X / newRect.Width };
            position = new Vector2(newRect.X, newRect.Y);
        }
    }

    class BackgroundTile: Tile
    {
        BackgroundTile tile;
        public bool active = true;
        public BackgroundTile(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            this.MapPoint = new[] { mapPoint.GetLength(0), mapPoint.GetLength(1)};
            tile = this;
            position = new Vector2(newRect.X, newRect.Y);
            index = i;
        }
    }

    class TopDoorTile:Tile
    {
        public TopDoorTile tile;
        public TopDoorTile(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            tile = this;
            position = new Vector2(newRect.X, newRect.Y);
            
        }

    }

    class BottomDoorTile : Tile
    {
        public BottomDoorTile tile;
        public BottomDoorTile(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            tile = this;
            position = new Vector2(newRect.X, newRect.Y);
        }

        public BottomDoorTile() { }

    }

    class DungeonEntrance :Tile
    {
        public DungeonEntrance(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            position = new Vector2(newRect.X, newRect.Y);
        }
    }

    class ControllBeacon :Tile
    {
        public ControllBeacon(int i , Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            position = new Vector2(newRect.X, newRect.Y);
        }
    }

    class BorderTile :Tile
    {
        public BorderTile(int i, Rectangle newRect)
        {
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile" + i);
            this.Rectangle = newRect;
            this.mapPoint = new int[newRect.Y / newRect.Height, newRect.X / newRect.Width];
            position = new Vector2(newRect.X, newRect.Y);
        }
    }
}
