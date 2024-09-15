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
        float maxMoveSpeedY = 12;
        float maxMoveSpeed = 5.5f;
        public int min = 0;
        Vector2 prevPos = Vector2.One;
        int count = 0;
        bool stop = false;
        int maxHeight;

        int cameraWidth = 1100;
        int cameraHeight = 900;
        public Vector2 Position
        {
            get { return center; }
        }

        public Rectangle CameraBounds;

        public Rectangle ViewRect;

        public Vector2 position
        {
            get
            {
                return new Vector2(-transform.Translation.X, -transform.Translation.Y);
            }
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
            this.maxHeight = height;
        }

        public void Update(Vector2 position, bool dont, bool fade, bool paused = false)
        {
            count++;
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
            height = maxHeight;
            foreach(BorderTile tile in SideTileMap.BorderTiles)
            {
                if (tile.Rectangle.Intersects(new Rectangle(ViewRect.X, ViewRect.Bottom, ViewRect.Width, 10)))
                {
                    height = tile.Rectangle.Top - 4;
                }
            }
          
           
            if(dont)
            {
                //center = new Vector2(position.X, position.Y);
                //transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
                
                onBorderLeft = false;
                onBorderTop = false;
                onBorderRight = false;
                onBorderBottom = false;

                //if (center.X < position.X)
                //{
                //    center.X += moveSpeed;
                //}
                //if (center.X > position.X)
                //{
                //    center.X -= moveSpeed;
                //}
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
                // center = new Vector2(position - moveSpeed)
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

            
            

           
         
            #region comments
            ////If bordering the top and left
            //if (this.position.Y <= 0 && position.Y - viewport.Height / 2f <= 0 && this.position.X <= 0 && position.X - viewport.Width / 5f <= 0)
            //{
            //    //center = new Vector2(viewport.Width / 1.75f, position.Y);
            //    //center = new Vector2(center.X, viewport.Height / 1.75f);
            //    onBorderLeft = true;
            //    onBorderTop = true;
            //    //center = new Vector2(viewport.Width / 1.75f, viewport.Height / 1.75f);
            //    //transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width, viewport.Height, 0));
            //}
            //else if (this.position.X <= 0 && position.X - viewport.Width / 2f <= 0)//Just bordering the left
            //{
            //    onBorderLeft = true;
            //    //center = new Vector2(viewport.Width / 1.75f, position.Y);
            //    //transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width, viewport.Height / 2, 0));
            //}
            //else if((position.X + 32) + viewport.Width / 2f > width)//Bordering the right
            //{
            //    onBorderRight = true;
            ////    center = new Vector2(viewport.Width * 1.365f, position.Y);
            ////    transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width/2, viewport.Height / 2, 0));
            //}
            //if(this.position.Y <= 0 && position.Y - viewport.Height/2f <= 0 && this.position.X > 0) //Bordering the top
            //{
            //    onBorderTop = true;
            //    //center = new Vector2(center.X, viewport.Height / 1.75f);
            //    //transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width/2, viewport.Height, 0));
            //}
            //else if(this.position.Y <= 0 && position.Y - viewport.Height / 2f <= 0 && this.position.X <= 0) //Bordering the Left and top
            //{
            //    onBorderLeft = true;
            //    onBorderTop = true;
            //    //center = new Vector2(center.X, viewport.Height / 1.75f);
            //    //transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width, viewport.Height, 0));
            //}
            //else if(this.position.Y <= 0 && position.Y - viewport.Height / 2f <= 0 && position.X + viewport.Width / 2 > width)//Bordering top and right
            //{
            //    onBorderTop = true;
            //    onBorderRight = true;
            //    //center = new Vector2(viewport.Width * 1.365f, viewport.Height / 1.75f);
            //    //transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
            //}



            //if (this.position.Y + height/2 >= height && position.X + viewport.Width / 2 > width) //Bordering the bottom and right
            //{
            //        onBorderBottom = true;
            //        onBorderRight = true;
            //    //center = new Vector2(viewport.Width * 1.365f, 64 * 10);
            //    //transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width/2, viewport.Height / 2, 0));
            //}
            //else if(this.position.Y + height/2 >= height)//Bordering the bottom
            //{
            //        onBorderBottom = true;
            //    //center = new Vector2(center.X, 64 * 10);
            //    //transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
            //}
            ////if (position.Y + viewport.Height / 2f >= height && position.X + viewport.Width / 2 >= width)
            ////{
            ////    onBorderSide = true;
            ////    onBorderTop = true;
            ////    center = new Vector2(center.X, height / 2);
            ////    transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width, viewport.Height/2, 0));
            ////}
            ////else if (position.Y + viewport.Height / 2f >= height && this.position.X > 0)
            ////{
            ////    onBorderTop = true;
            ////    center = new Vector2(center.X, height/2);
            ////    transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height/2, 0));
            ////}
            ////else if (position.Y + viewport.Height / 2f >= height && this.position.X <= 0)
            ////{
            ////    onBorderSide = true;
            ////    onBorderTop = true;
            ////    center = new Vector2(center.X, height/2);
            ////    transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) * Matrix.CreateTranslation(new Vector3(viewport.Width, viewport.Height/2, 0));
            ////}

            //if (onBorderTop)
            //{
            //    center.Y += moveSpeed;
            //    onBorderTop = false;
            //}

            //if (onBorderBottom)
            //{
            //    onBorderBottom = false; 
            //    center.Y -= moveSpeed;
            //}

            //if(onBorderLeft)
            //{
            //    center.X += moveSpeed;
            //    onBorderLeft = false;
            //}
            //if(onBorderRight)
            //{
            //    center.X -= moveSpeed;
            //    onBorderRight = false;
            //}
            #endregion
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

        public void SetBounds(Rectangle rect)
        {
            CameraBounds = rect;
        }
        public Rectangle FollowBox
        {
            get { return new Rectangle((int)(Position.X + viewport.Width/2), (int)(Position.Y + viewport.Height/2), 400, 400); }
        }
    }
}
