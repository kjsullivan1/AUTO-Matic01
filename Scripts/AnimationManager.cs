using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic
{
    class AnimationManager
    {
        public bool isRight = true;
        public bool isLeft = false;
        public bool isUp = false;
        public bool isDown = false;



        private Texture2D CurrTexture;
        private Point FrameSize;
        private Point CurrFrame;
        private Point SheetSize;
        private int MiliSecsPerFrame;
        private Vector2 position;
        bool stopLoop = false;


        int timeSinceLastFrame = 0;

        public AnimationManager(Texture2D texture, Point frameSize, Point currFrame, Point sheetSize, int fpms, Vector2 pos)
        {
            CurrTexture = texture;
            FrameSize = frameSize;
            CurrFrame = currFrame;
            SheetSize = sheetSize;
            MiliSecsPerFrame = fpms;
            position = pos;

        }

        public void SetPos(Vector2 pos)
        {
            position = pos;
        }
        public void SetFrameToEnd()
        {
            CurrFrame.X = SheetSize.X - 1;
        }
        public Point GetCurrFrame()
        {
            return CurrFrame;
        }
        public Point GetSheetSize()
        {
            return SheetSize;
        }
        public void StopLoop()
        {
            stopLoop = true;
        }
        public void StartLoop()
        {
            stopLoop = false;
        }
        public void UpdateTexture(Texture2D newTexture, Point frameSize, Point currFrame, Point sheetSize, int fpms)
        {
            CurrTexture = newTexture;
            FrameSize = frameSize;
            CurrFrame = currFrame;
            SheetSize = sheetSize;
            MiliSecsPerFrame = fpms;
        }

        public void Update(GameTime gameTime, Vector2 pos)
        {
            if (stopLoop)
            {

            }
            else
            {
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;

                if (timeSinceLastFrame > MiliSecsPerFrame)
                {
                    timeSinceLastFrame -= MiliSecsPerFrame;

                    CurrFrame.X++;

                    if (CurrFrame.X >= SheetSize.X)
                    {
                        CurrFrame.X = 0;
                        CurrFrame.Y++;
                        if (CurrFrame.Y >= SheetSize.Y)
                        {
                            CurrFrame.Y = 0;
                        }
                    }
                }
            }


            position = pos;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            if (isRight)
            {
                
                spriteBatch.Draw(CurrTexture, position, new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y), color);
            }
            else if (isLeft)
            {
                spriteBatch.Draw(CurrTexture, position: position, sourceRectangle: new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y), color: color, effects: SpriteEffects.FlipHorizontally);
            }


        }

        public void Draw(SpriteBatch spriteBatch, Color color, float angle, Rectangle originRect)
        {
 

            spriteBatch.Draw(CurrTexture,position:position, sourceRectangle: new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
                color: color, origin: new Vector2(originRect.Width/2, originRect.Height/2), rotation: angle);

          

            //if (isRight)
            //{

            //    spriteBatch.Draw(CurrTexture, position, new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y), color);
            //}
            //else if (isLeft)
            //{
            //    spriteBatch.Draw(CurrTexture, position: position, sourceRectangle: new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y), color: color, effects: SpriteEffects.FlipHorizontally);
            //}


        }
    }
}
