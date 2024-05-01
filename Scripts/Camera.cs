using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    class Camera
    {
        public Matrix transform;

        Vector2 center;
        public Viewport viewport;
        private float zoom = 1f;
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

        public Camera(Viewport newViewport, Vector2 startPos)
        {
            viewport = newViewport;
            center = startPos;
        }

        public void Update(Vector2 position)
        {
            center = new Vector2(position.X, position.Y);
            transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        }
    }
}
