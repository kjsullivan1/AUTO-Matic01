﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.SideScroll.Enemy;
using AUTO_Matic.Scripts;
using AUTO_Matic.SideScroll;

namespace AUTO_Matic.Scripts.SideScroll.Enemy
{
    class FlyingEnemy
    {
        enum AnimationStates { Walking, Idle, Death, Jump, Shoot }
        AnimationStates animState = AnimationStates.Idle;

        public enum EnemyStates { Idle, GoTo, Attacking, Jumping, Launch }
        public EnemyStates enemyState = EnemyStates.Idle;
        public EnemyStates prevState;


        Rectangle enemyRect;
        float moveSpeed = .5f;
        Vector2 pos;
        float gravResistance;
        public float health;
        static int pixelSize = 64;
        Vector2 velocity = Vector2.Zero;
        float yOffset = 64 * 3; //How high from the ground 
        Rectangle groundRect;
        Texture2D visionTexture;
        //Texture2D texture;
        int visionLength;
        List<Rectangle> vision = new List<Rectangle>();
        float attackDelay = 1.5f;
        float launchStr = 12;
        float initYPos;
        float maxSpeed = 4f;
        bool blockBottom, blockRight, blockLeft, blockTop;

        private void CreateVision()
        {
            vision.Clear();
            Vector2 pos = new Vector2((int)enemyRect.X, (int)(enemyRect.Y / pixelSize) * 64);
            #region Full vision around enemy

            for (int i = 1; i < visionLength + 1; i++)
            {
                vision.Add(new Rectangle((int)pos.X, (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));//Up
                vision.Add(new Rectangle((int)pos.X, (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));//Down
            }

            //vision.Add(new Rectangle((int)pos.X, (int)pos.Y - (pixelSize * 1), pixelSize, pixelSize));//Up
            for (int i = 1; i < visionLength + 1; i++)//Right
            {
                vision.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y, pixelSize, pixelSize));
                for (int j = i + 1; j < visionLength + 1; j++) //Right and Up
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y - (pixelSize * j), pixelSize, pixelSize));
                }
                for (int k = i + 1; k < visionLength + 1; k++)//Right and down
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y + (pixelSize * k), pixelSize, pixelSize));
                }
            }

            for (int i = 1; i < visionLength + 1; i++)//Left
            {
                vision.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y, pixelSize, pixelSize));
                for (int j = i + 1; j < visionLength + 1; j++) //Left and Up
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y - (pixelSize * j), pixelSize, pixelSize));
                }
                for (int k = i + 1; k < visionLength + 1; k++)//Left and down
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y + (pixelSize * k), pixelSize, pixelSize));
                }
            }

            for (int i = 1; i < visionLength + 1; i++) //Up and down
            {
                //vision.Add(new Rectangle(pos.X + (int)(rectangle.Width / 3.5f), enemyRect.Top + (enemyRect.Height * 1), enemyRect.Width / 2, pixelSize / 4));
                for (int j = i; j < visionLength + 1; j++) //Left and up 
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * j), (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));
                }
                for (int j = i; j < visionLength + 1; j++) //Right and Up
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * j), (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));
                }
                //vision.Add(new Rectangle((int)pos.X, (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));//Down
                for (int k = i; k < visionLength + 1; k++)//Left and down
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                }
                for (int k = i; k < visionLength + 1; k++)//Right and down
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                }

                //if (enemyState != EnemyStates.Jumping && prevState != EnemyStates.Jumping)
                //{
                //    for (int k = i; k < visionLength + 1; k++)//Left and down
                //    {
                //        vision.Add(new Rectangle((int)pos.X - (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                //    }
                //    for (int k = i; k < visionLength + 1; k++)//Right and down
                //    {
                //        vision.Add(new Rectangle((int)pos.X + (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                //    }
                //}


            }
            //vision.Add(new Rectangle(enemyRect.X + (int)(enemyRect.Width/3.5f), enemyRect.Top + (enemyRect.Height * 1), enemyRect.Width/2, pixelSize/4));
            #endregion
        }

        #region Animations
        AnimationManager animManager;
        Texture2D texture;
        Point FrameSize;//Size of frame
        Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;
        public void ChangeAnimation()
        {
            switch (animState)
            {
                //case AnimationStates.Idle:
                //    texture = content.Load<Texture2D>("SideScroll/Animations/PlayerIdle");
                //    FrameSize = new Point(64, 64);
                //    CurrFrame = new Point(0, 0);
                //    SheetSize = new Point(6, 1);
                //    fpms = 120;
                //    break;
                case AnimationStates.Walking:
                default:
                    //if (isShoot)
                    //{
                    //    texture = content.Load<Texture2D>("SideScroll/Animations/RangedEnemyWalk");
                    //    FrameSize = new Point(64, 64);
                    //    CurrFrame = new Point(0, 0);
                    //    SheetSize = new Point(4, 1);
                    //    fpms = 120;
                    //}
                    //else
                    //{
                    //    texture = content.Load<Texture2D>("SideScroll/Animations/MeleeEnemyWalk");
                    //    FrameSize = new Point(64, 64);
                    //    CurrFrame = new Point(0, 0);
                    //    SheetSize = new Point(4, 1);
                    //    fpms = 120;
                    //}
                    break;
                    //case AnimationStates.Jump:
                    //    texture = content.Load<Texture2D>("SideScroll/Animations/PlayerJump");
                    //    FrameSize = new Point(64, 64);
                    //    CurrFrame = new Point(0, 0);
                    //    SheetSize = new Point(4, 1);
                    //    fpms = 95;
                    //    break;
            }

            bool isRight = true, isLeft = false, isUp = false, isDown = false;
            if (animManager != null)
            {
                isRight = animManager.isRight;
                isLeft = animManager.isLeft;
                isUp = animManager.isUp;
                isDown = animManager.isDown;
            }

            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, pos);

            animManager.isRight = isRight;
            animManager.isLeft = isLeft;
            animManager.isUp = isUp;
            animManager.isDown = isDown;
        }
        #endregion

        public FlyingEnemy(ContentManager contentManager, int visionLength, Vector2 position)
        {
            this.visionLength = visionLength;
            pos = position;
            enemyRect = new Rectangle((int)position.X, (int)position.Y, pixelSize, pixelSize);
            texture = contentManager.Load<Texture2D>(@"Textures\white");
            visionTexture = contentManager.Load<Texture2D>(@"Textures\Red");
            CreateVision();
            prevState = enemyState;

        }

        public void Update(GameTime gameTime, Vector2 gravity, SSPlayer player, SideTileMap map)
        {
            blockBottom = false;
            blockLeft = false;
            blockRight = false;
            blockTop = false;

            switch(enemyState)
            {
                case EnemyStates.Idle:
                    if (groundRect == Rectangle.Empty)
                    {
                        groundRect = new Rectangle(enemyRect.X + enemyRect.Width / 2, enemyRect.Bottom - enemyRect.Height/2, enemyRect.Width/2, enemyRect.Height/2);
                    }
                    //groundRect = enemyRect;
                    foreach (Rectangle rect in vision)
                    {
                        if(rect.Intersects(player.playerRect))
                        {
                           // bool found = false;
                            
                                //foreach (GroundTile ground in map.GetGroundTiles())
                                //{
                                //    if (groundRect.TouchTopOf(ground.Rectangle))
                                //    {
                                //        groundRect = ground.Rectangle;
                                //        //found = true;
                                //        break;
                                //    }
                                //}
                            
                            

                            velocity = new Vector2(0, -launchStr);
                            initYPos = pos.Y;
                            prevState = enemyState;
                            enemyState = EnemyStates.Launch;
                           
                        }
                    }
                    break;
                case EnemyStates.Launch:
                    pos += velocity;
                    CreateVision();
                    groundRect = new Rectangle((int)(groundRect.X + gravity.X), (int)(groundRect.Y + gravity.Y), groundRect.Width, groundRect.Height);
                    foreach (GroundTile ground in map.GetGroundTiles())
                    {
                        if (groundRect.TouchTopOf(ground.Rectangle))
                        {
                            groundRect = new Rectangle(groundRect.X, ground.Rectangle.Y - groundRect.Height, groundRect.Width, groundRect.Height);
                        }
                    }
                    if (MathHelper.Distance(initYPos, pos.Y) >= yOffset)
                    {
                        prevState = enemyState;
                        enemyState = EnemyStates.GoTo;
                        velocity = Vector2.Zero;
                    }
                    break;
                case EnemyStates.GoTo:

                   

                    groundRect = new Rectangle((int)(groundRect.X + gravity.X), (int)(groundRect.Y + gravity.Y * 10), groundRect.Width, groundRect.Height);
                    
                   

                    foreach(GroundTile ground in SideTileMap.GroundTiles)
                    {
                        Collision(ground.Rectangle);
                        if(groundRect.TouchTopOf(ground.Rectangle))
                        {
                            groundRect.Y = ground.Rectangle.Y - groundRect.Height;
                        }
                    }
                    foreach (WallTile wall in SideTileMap.WallTiles)
                    {
                        Collision(wall.Rectangle);
                    }
                    foreach(PlatformTile platform in SideTileMap.PlatformTiles)
                    {
                        Collision(platform.Rectangle);
                    }

                    if(pos.X < player.Position.X)
                    {
                        if(velocity.X < 0)
                        {
                            velocity.X = -velocity.X;
                            velocity.X += moveSpeed;

                            if (velocity.X > maxSpeed)
                            {
                                velocity.X = maxSpeed;
                            }
                        }
                        else if(velocity.X > 0)
                        {
                            velocity.X += moveSpeed;
                            if(velocity.X > maxSpeed)
                            {
                                velocity.X = maxSpeed;
                            }
                        }
                        else
                        {
                            velocity.X += moveSpeed;
                        }


                    }
                    else if(pos.X > player.Position.X)
                    {
                        if(velocity.X > 0)
                        {
                            velocity.X = -velocity.X;
                            velocity.X -= moveSpeed/2;

                            if(velocity.X < -maxSpeed)
                            {
                                velocity.X = -maxSpeed;
                            }
                        }
                        else if(velocity.X < 0)
                        {
                            velocity.X -= moveSpeed/2;

                            if(velocity.X < -maxSpeed)
                            {
                                velocity.X = -maxSpeed;
                            }
                        }
                        else
                        {
                            velocity.X -= moveSpeed;
                        }
                    }

                  
                    if (MathHelper.Distance(enemyRect.Y, player.playerRect.Y) > yOffset && !blockBottom)
                    {
                        if(velocity.Y < 0)
                        {
                            velocity.Y = -velocity.Y;
                        }
                        velocity.Y += moveSpeed/2;
                        if(velocity.Y > maxSpeed)
                        {
                            velocity.Y = maxSpeed;
                        }

                        if(MathHelper.Distance(enemyRect.Y + velocity.Y, player.playerRect.Y) < yOffset)
                        {
                            velocity.Y = 0;
                        }
                    }
                    else if (MathHelper.Distance(enemyRect.Y, player.playerRect.Y) < yOffset && !blockTop)
                    {
                        if(velocity.Y > 0)
                        {
                            velocity.Y = -velocity.Y;
                        }
                        velocity.Y -= moveSpeed/2;
                        if(velocity.Y < -maxSpeed)
                        {
                            velocity.Y = -maxSpeed;
                        }

                        if (MathHelper.Distance(enemyRect.Y + velocity.Y, player.playerRect.Y) > yOffset)
                        {
                            velocity.Y = 0;
                        }
                    }

                    if (velocity.X > 0 && !blockRight)
                    {
                        groundRect = new Rectangle((int)(groundRect.X + velocity.X), (int)(groundRect.Y + velocity.Y), groundRect.Width, groundRect.Height);
                    }
                    if (velocity.X < 0 && !blockLeft)
                    {
                        groundRect = new Rectangle((int)(groundRect.X + velocity.X), (int)(groundRect.Y + velocity.Y), groundRect.Width, groundRect.Height);
                    }
                    pos += velocity;
                    break;
            }

            

            enemyRect = new Rectangle((int)pos.X, (int)pos.Y, pixelSize, pixelSize);
        }

        public void Draw(SpriteBatch spriteBatch)
        {


            //tRect.X += tRect.Width/5;
            spriteBatch.Draw(texture, enemyRect, Color.White);
            spriteBatch.Draw(texture, groundRect, Color.White);
            //spriteBatch.Draw(texture, HitBox, Color.BlueViolet);
            //foreach (Bullet bullet in bullets)
            //{
            //    bullet.Draw(spriteBatch);
            //}

            //animManager.Draw(spriteBatch, Color.White);
            //foreach (Rectangle rect in vision)
            //{
            //    spriteBatch.Draw(visionTexture, rect, Color.White * .25f);


            //}
            //animManager.Draw(spriteBatch);
        }

        public void Collision(Rectangle newRect)
        {

            if (enemyRect.TouchTopOf(newRect)) //Touch Ground
            {
                blockBottom = true;
                while (enemyRect.Bottom > newRect.Top)
                {
                    pos.Y -= 1f;
                    enemyRect.Y = (int)pos.Y;
                }
                //if (isFalling)
                //{
                //    while (enemyRect.Bottom > newRect.Top)
                //    {
                //        position.Y -= 1f;
                //        enemyRect.Y = (int)position.Y;
                //    }
                //    velocity.Y = 0;
                //    isFalling = false;
                //    bounds = SideTileMap.GetNumTilesOfGround(newRect.Y / 64, newRect.X / 64); //How many tiles are available left and right
                //    jumpDelay = 0;
                //    maxRunSpeed = 3.75f;
                //    // if(bounds = new Vector2(0,0))
                //    if (bounds == Vector2.Zero)
                //    {
                //        bounds = new Vector2(64 - enemyRect.Width, 64 - enemyRect.Width);
                //    }
                //    else
                //    {
                //        bounds *= 64; //Calculate distance travled from this number to insure bound correction   
                //    }

                //    landingPos = position;
                //}
                //if (enemyState == EnemyStates.Jumping)
                //{
                //    //Dont check if jumping
                //}
                //else
                //{
                //    while (enemyRect.Bottom > newRect.Top)//Keep enemyRect on the ground
                //    {
                //        position.Y -= 1f;
                //        enemyRect.Y = (int)position.Y;

                //    }
                //    //jumpDelay++;
                //    //if(jumpDelay > maxJumpDelay)
                //    //{

                //    //}
                //    //if(velocity.X > 0)
                //    //{
                //    //    canWalk = SideTileMap.CanWalk(newRect.Y / 64, newRect.X / 64, "right");
                //    //}
                //    //if(velocity.X < 0)
                //    //{
                //    //    canWalk = SideTileMap.CanWalk(newRect.Y / 64, newRect.X / 64, "right");
                //    //}
                //}


            }

            if (enemyRect.TouchLeftOf(newRect))//enemy is colliding to the right
            {
                blockRight = true;

                while (enemyRect.Right > newRect.Left)
                {
                    pos.X -= 1f;
                    enemyRect.X = (int)pos.X;
                }

                //position.X += -Velocity.X;
                //enemyRect.X = (int)position.X;
            }

            if (enemyRect.TouchRightOf(newRect))//enemy is Colliding to the left
            {
                blockLeft = true;

                while (enemyRect.Left < newRect.Right)
                {
                    pos.X += 1;
                    enemyRect.X = (int)pos.X;
                }

                //position.X += -Velocity.X;
                //enemyRect.X = (int)position.X;
            }

            if (enemyRect.TouchBottomOf(newRect)) //Colliding Top or touching bottom of tile
            {
                blockTop = false;
                while (enemyRect.Top < newRect.Bottom)
                {
                    pos.Y += 1;
                    enemyRect.Y = (int)pos.Y;
                }

                //if (velocity.Y < 0)
                //{
                //    velocity.Y = 0;
                //}
                //if (!isFalling)
                //{
                //    isFalling = true;
                //}
            }
        }
    }
}