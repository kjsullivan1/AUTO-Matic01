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
    class Bullet
    {
        Vector2 position;
        Vector2 velocity;
        //float moveSpeed;
        public Vector2 maxSpeed;
        Texture2D bulletTexture;
        int width = 14;
        int height = 14;
        public Rectangle rect;
        bool shootX = true;
        bool shootY = false;
        public Vector2 bulletSpeed;
        float angle;
        public Color color;

        Vector2 startPos;
        float travelDist;
        public bool delete = false;
        public enum BulletTypes { Player, Boss, Bomb, Bullet}
        BulletTypes bulletType = BulletTypes.Player;
        public BulletTypes BulletType
        {
            set
            {
                bulletType = value;
                ChangeAnimation();
            }
            get
            {
                return bulletType;
            }
        }


        #region Animations
        public bool isPlayer = false;
        public bool isBoss = false;

        ContentManager content;
        public AnimationManager animManager;
        Texture2D texture;
        Point FrameSize;//Size of frame
        public Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;
        public AnimationManager animManagerRobo;
        Texture2D textureRobo;
        Point FrameSizeRobo;//Size of frame
        Point CurrFrameRobo;//Location of currFram on the sheet
        Point SheetSizeRobo;//num of frames.xy
        int fpmsRobo;

        public void ChangeAnimation()
        { 

            switch(BulletType)
            {
                case BulletTypes.Player:
                    texture = content.Load<Texture2D>("SideScroll/Animations/EnergyBlast");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(2, 0);
                    SheetSize = new Point(10, 1);
                    fpms = 60;
                    break;
                case BulletTypes.Boss:
                    texture = content.Load<Texture2D>("SideScroll/Animations/EnergyBlast_Red");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(2, 0);
                    SheetSize = new Point(10, 1);
                    fpms = 60;
                    break;
                case BulletTypes.Bullet:
                    texture = content.Load<Texture2D>("SideScroll/Animations/Bullet");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(1, 0);
                    SheetSize = new Point(1, 1);
                    fpms = 60;
                    break;
                case BulletTypes.Bomb:
                    texture = content.Load<Texture2D>("SideScroll/Animations/Bomb");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(1, 0);
                    SheetSize = new Point(1, 1);
                    fpms = 60;
                    break;


            }
           

            bool isRight = true, isLeft = false, isUp = false, isDown = false;
            if (animManager != null)
            {
                isRight = animManager.isRight;
                isLeft = animManager.isLeft;
                isUp = animManager.isUp;
                isDown = animManager.isDown;
            }

            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, position);

            animManager.isRight = isRight;
            animManager.isLeft = isLeft;
            animManager.isUp = isUp;
            animManager.isDown = isDown;
        }
        #endregion

        public Bullet(Vector2 pos, float speed, Vector2 maxSpeed, ContentManager content, bool isX, float travelDist,
            bool isY = false, float speedY = 0, float angle = 0, int size = 14)
        {
            position = pos;
            startPos = pos;
            this.travelDist = travelDist;
            this.maxSpeed = maxSpeed;
            bulletTexture = content.Load<Texture2D>("Textures/white");
            rect = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            shootX = isX;
            shootY = isY;
            bulletSpeed = new Vector2(speed, speedY);
            this.angle = angle;
            width = size;
            height = size;
            this.content = content;
            ChangeAnimation();
            
        }

        public void Update(GameTime gameTime)
        {
            if(shootX && shootY)
            {
                if(angle != 0)
                {
                    velocity += bulletSpeed /** angle*/;
                }
                else
                {
                    velocity += bulletSpeed;
                }
                
            }
            else if (shootX)
            {
                if (velocity.X < maxSpeed.X)
                {
                    velocity.X += bulletSpeed.X;
                }
                else
                {
                    velocity.X = maxSpeed.X;
                }

            }
            else if(shootY)
            {

                if (velocity.Y < maxSpeed.Y)
                    velocity.Y += bulletSpeed.Y;
                else
                    velocity.Y = maxSpeed.Y;
            }
           
            position += velocity;

            if(Distance(position, startPos) > travelDist)
            {
                delete = true;
            }
            rect = new Rectangle((int)position.X, (int)position.Y, width, height);
            
            if(isPlayer)
                animManager.Update(gameTime, new Vector2(position.X, position.Y - 28));
            else
                animManager.Update(gameTime, new Vector2(position.X, position.Y));

            //if (bulletSpeed.X < 0)
            //{
            //    animManager.isLeft = true;
            //    animManager.isRight = false;
            //}
            //else if(bulletSpeed.X > 0)
            //{
            //    animManager.isRight = true;
            //    animManager.isLeft = false;
            //}
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(bulletTexture, new Rectangle((int)position.X, (int)position.Y, width, height), Color.White);
            switch(BulletType)
            {
                case BulletTypes.Player:
                    animManager.DrawSpriteSheet(spriteBatch, Color.White, angle, rect);
                    //spriteBatch.Draw(bulletTexture, new Rectangle((int)position.X, (int)position.Y, width, height), color);
                    break;
                case BulletTypes.Bullet:
                    animManager.DrawCheat(spriteBatch, angle, rect);
                    break;
                default:
                    animManager.Draw(spriteBatch, Color.White, angle, rect);
                    break;
            }

          


        }
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            //spriteBatch.Draw(bulletTexture, new Rectangle((int)position.X, (int)position.Y, width, height), color);
            animManager.Draw(spriteBatch, Color.White, angle: angle, originRect: rect);
        }

        public float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }
    }
}
