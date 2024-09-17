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



        public Texture2D CurrTexture;
        private Point FrameSize;
        private Point CurrFrame;
        private Point SheetSize;
        private int MiliSecsPerFrame;
        private Vector2 position;
        bool stopLoop = false;
        int widthMod = 0;


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
        public void SetWidthMod(int mod)
        {
            widthMod = mod;
        }
        public void SetPos(Vector2 pos)
        {
            position = pos;
        }
        public void SetFrameToEnd()
        {
            CurrFrame.X = SheetSize.X - 1;
        }
        public void SetCurrFrame(int frame)
        {
            CurrFrame.X = frame;
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
                
                spriteBatch.Draw(CurrTexture, position, new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X + widthMod, FrameSize.Y), color);
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
        //This is made to spite the fact spriteBatch cant consistantly use 'rotation:' as the variable will constantly shift in and out of existance depending on the overload
        public void DrawCheat(SpriteBatch spriteBatch, float angle, Rectangle originRect)
        {
            spriteBatch.Draw(CurrTexture, position: new Vector2(position.X, position.Y), sourceRectangle: new Rectangle(0, 0, 64, 64), color: Color.White,
                origin: new Vector2(64 / 2, 64 / 2), rotation: angle);
        }

        public void DrawSpriteSheet(SpriteBatch spriteBatch, Color color, float angle, Rectangle originRect)
        {
            if((int)MathHelper.ToDegrees(angle) == 90)
            {
                spriteBatch.Draw(CurrTexture, position: new Vector2(position.X + 55, position.Y), sourceRectangle: new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
         color: color, origin: new Vector2(originRect.Width / 2, originRect.Height / 2), rotation: angle);
            }
            else if((int)MathHelper.ToDegrees(angle) == 180)
            {
                spriteBatch.Draw(CurrTexture, position: new Vector2(position.X, position.Y + 25), sourceRectangle: new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
color: color, origin: new Vector2(originRect.Width / 2, originRect.Height / 2), rotation: angle);
            }
            else
            {
                switch (angle)
                {
                    case 0:
                        spriteBatch.Draw(CurrTexture, position: position, sourceRectangle: new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
          color: color, origin: new Vector2(originRect.Width / 2, originRect.Height / 2));
                        break;
                    case 180:
                        spriteBatch.Draw(CurrTexture, position: position, sourceRectangle: new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
        color: color, origin: new Vector2(originRect.Width / 2, originRect.Height / 2), effects: SpriteEffects.FlipHorizontally);
                        break;
                    default:
                        spriteBatch.Draw(CurrTexture, position: position, sourceRectangle: new Rectangle(CurrFrame.X * FrameSize.X, CurrFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
    color: color, origin: new Vector2(originRect.Width / 2, originRect.Height / 2), rotation: angle);
                        break;
                }
            }

           
      

        }
    }
}
