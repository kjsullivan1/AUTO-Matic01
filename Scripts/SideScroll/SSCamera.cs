using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AUTO_Matic.Scripts;
using AUTO_Matic.SideScroll;
using AUTO_Matic.Scripts.SideScroll;

namespace AUTO_Matic.Scripts.SideScroll
{
    public class SSCamera
    {
        public Matrix transform;
        public int width = 0;
        public int height = 0;
        public Vector2 center;
        public Viewport viewport;
        private float zoom = 1.75f;
        public bool onBorderLeft = false;
        public bool onBorderTop = false;
        public bool onBorderBottom = false;
        public bool onBorderRight = false;
        public bool reached = false;
        float moveSpeed = 4.25f;
        float moveSpeedY = 4.25f;
        float maxMoveSpeedY = 12f;
        float maxMoveSpeed = 5.5f;
        public int min = 0;
        Vector2 prevPos = Vector2.One;
        int count = 0;
        int maxHeight;

        int cameraWidth = 1100;
        int cameraHeight = 900;
        public bool freeze = false;
        public Vector2 Position
        {
            get { return center; }
        }

        public Rectangle CameraBounds;

        public Rectangle ViewRect;

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
            this.maxHeight = height;
        }

        public void Update(Vector2 position, bool dont, bool fade, bool paused = false)
        {
            count++;
            height = maxHeight;
            foreach(BorderTile tile in SideTileMap.BorderTiles)
            {
                if (tile.Rectangle.Intersects(new Rectangle(ViewRect.X, ViewRect.Bottom, ViewRect.Width, 10)))
                {
                    height = tile.Rectangle.Top - 4;
                }
            }
            
            if(freeze)
            {

            }
            else if(dont)
            {
                onBorderLeft = false;
                onBorderTop = false;
                onBorderRight = false;
                onBorderBottom = false;

                if ((int)center.Y < (int)position.Y && (center.Y + 620 / 2) + moveSpeed < height && center.Y + moveSpeed < (int)position.Y)
                {
                    
                    center.Y += moveSpeed;
                    reached = false;
                }
                
                if ((int)center.Y > (int)position.Y && center.Y - moveSpeed > (int)position.Y)
                {
                    center.Y -= moveSpeed;
                    reached = false;
                }

            }
            else
            {

                if (MathHelper.Distance(center.X, position.X) > 64 * 3)
                {
                    if(paused)
                    {
                        moveSpeed = maxMoveSpeed * 10;
                    }
                    else
                        moveSpeed = maxMoveSpeed;
                }
                else if(MathHelper.Distance(center.X, position.X) > 64 * 4)
                {
                    if (paused)
                        moveSpeed = maxMoveSpeed * 8;
                    else
                        moveSpeed = maxMoveSpeed * 1.5f;
                }
                else
                {
                    moveSpeed = 4.3f;
                }

                if (MathHelper.Distance(center.Y, position.Y) > 64 * 2 && MathHelper.Distance(center.Y, position.Y) < 64 * 3)
                {
                    moveSpeedY = maxMoveSpeedY;
                }
                else if (MathHelper.Distance(center.Y, position.Y) > 64 * 3)
                {
                    moveSpeedY = maxMoveSpeedY * 1.5f;
                }
                else
                {
                    moveSpeedY = 4.3f;
                }
                if ((int)center.X < position.X && (center.X + cameraWidth / 2) + moveSpeed < width)
                {
                    center.X += moveSpeed;
                    reached = false;
                }
                if ((int)center.X > position.X && (center.X - cameraWidth / 2) - moveSpeed > min)
                {
                    center.X -= moveSpeed;
                    reached = false;
                }
                if ((int)center.Y < position.Y && (center.Y + 620/2) + moveSpeedY < height)
                {
                    center.Y += moveSpeedY;
                    reached = false;
                }
                if ((int)center.Y > position.Y && (int)((center.Y - 620/2) - moveSpeedY) > 0)
                {
                    center.Y -= moveSpeedY;
                    reached = false;
                }
                if(center == position)
                {
                    reached = true;
                }

                onBorderLeft = false;
                onBorderTop = false;
                onBorderRight = false;
                onBorderBottom = false;
            }

            if (count > 1)
            {
                if (MathHelper.Distance(prevPos.X, center.X) == 0 && MathHelper.Distance(prevPos.Y, center.Y) == 0 )
                {
                    if(CameraBounds.X < 0)
                    {
                        CameraBounds.X = 0;
                        center.X = 550.4f;
                    }
                    reached = true;
                }
                prevPos = center;
                count = 0;
            }

            transform = Matrix.CreateTranslation(new Vector3((int)-center.X, (int)-center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
            CameraBounds = new Rectangle(new Point((int)(center.X - cameraWidth / 2), (int)(center.Y - cameraHeight/2) + 128), new Point(cameraWidth, height));

            Rectangle viewRect = CameraBounds;
            viewRect.Height = 620;
            if (viewRect.Left < min)
            {
                center.X += moveSpeed;
                reached = false;
                //center.X = min + viewRect.Width / 2;

            }
            if (viewRect.Top < 0 )
            {
                center.Y += moveSpeedY;
                if (!fade)
                    CameraBounds.Y = 0;
                //center.Y = 0;

                if (paused)
                {
                    center.Y = 0 + ViewRect.Height / 2;
                }
                else
                {
                    reached = false;
                }
            }

            if (viewRect.Right > width)
            {
                center.X -= moveSpeed;
                reached = false;
            }

            if (viewRect.Bottom > height)
            {
                 center.Y -= moveSpeedY;
                if (paused)
                    center.Y = height - viewRect.Height / 2;
                //center.Y = height - viewRect.Height / 2;
                reached = false;
            }

            ViewRect = viewRect;

        }
    }
}
