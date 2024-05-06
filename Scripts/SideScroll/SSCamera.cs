using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AUTO_Matic.Scripts.SideScroll
{
    public class SSCamera
    {
        public Matrix transform;
        public int width = 0;
        public int height = 0;
        Vector2 center;
        public Viewport viewport;
        private float zoom = 1.75f;
        public bool onBorder = false;
       
        public Vector2 Position
        {
            get { return center; }
        }
        public float X
        {
            get { return center.X; }
            set { center.X = value; }
        }

        public float Y
        {
            get { return center.Y; }
            set { center.Y = value; }
        }


        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; if (zoom < 0.1f) zoom = .1f; }
        }

        public SSCamera(Viewport newViewport, Vector2 startPos, int width, int height)
        {
            viewport = newViewport;
            center = startPos;
            this.width = width;
            this.height = height;
        }

        public void Update(Vector2 position)
        {
            //if (position.Y - viewport.Height / 2 <= 0)
            //{
            //    //onBorder = true;

            //    center = new Vector2(center.X, viewport.Height / 1.75f);
            //    transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width, viewport.Height, 0));

            ////}
            //if (position.X - viewport.Width / 4.75f <= 0)
            //{
            //    onBorder = true;
            //    center = new Vector2(viewport.Width / 1.75f, position.Y);
            //    transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width, viewport.Height / 2, 0));
            //}
            //else if (position.X + viewport.Width / 4.75f >= width)
            //{
            //    onBorder = true;
            //    center = new Vector2(viewport.Width / 2, position.Y);
            //    transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(position.X + viewport.Width, viewport.Height / 2, 0));
            //}
            //else
            {
                onBorder = false;
                center = new Vector2(position.X, position.Y);
                transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
            }

           
            //else if(position.Y + viewport.Height/2 >= height)
            //{

            //}
            //else
            //{
            //    onBorder = false;
            //    center = new Vector2(center.X, position.Y);
            //    transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
            //}

        }
        public Rectangle FollowBox
        {
            get { return new Rectangle((int)(Position.X + viewport.Width/2), (int)(Position.Y + viewport.Height/2), 400, 400); }
        }
    }
}
