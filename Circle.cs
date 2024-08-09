using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    class Circle
    {
        public Vector2 Position;
        public int Radius;
        int widthOffset = 0;
        //public Texture2D texture;
        public Circle(Vector2 position, int radius)
        {
            Position = position;
            Radius = radius;
        }
        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, 2 * (this.Radius + widthOffset), 2 * this.Radius); }
            set { Bounds = value; }
        }


 


        public void SetWidth(int width)
        {
            widthOffset += width;
        }
        public bool Intersects(Circle circle)
        {
            double x1 = Position.X;
            double y1 = Position.Y;
            double x2 = circle.Position.X;
            double y2 = circle.Position.Y;

            double distance = Math.Abs(Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2)));
            if (distance <= Radius + circle.Radius)
            {
                return true;
            }

            return false;
        }
        public bool Intersects(Rectangle rect)
        {
            double x1 = Position.X;
            double y1 = Position.Y;
            double x2 = rect.X;
            double y2 = rect.Y;

            double distance = Math.Abs(Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2)));
            if (distance <= Radius + rect.Width)
            {
                return true;
            }

            return false;
        }
    }
}
