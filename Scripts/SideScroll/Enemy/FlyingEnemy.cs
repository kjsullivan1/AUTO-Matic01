using Microsoft.Xna.Framework;
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
using AUTO_Matic;
using AUTO_Matic.Scripts.Effects;
using AUTO_Matic.Scripts.SideScroll;

namespace AUTO_Matic.Scripts.SideScroll.Enemy
{
    class FlyingEnemy
    {
        enum AnimationStates { Walking, Idle, Death, Jump, Shoot }
        AnimationStates animState = AnimationStates.Walking;

        public enum EnemyStates { Idle, GoTo, Attacking, Jumping, Launch, Dead }
        public EnemyStates enemyState = EnemyStates.Idle;
        public EnemyStates prevState;

        int growthRate = 5;
        ParticleManager particles;

        List<Bullet> bullets = new List<Bullet>();
        List<Explosion> explosions = new List<Explosion>();
        float attackDist = 64 * 2.5f;
        float iShootDelay;
        float shootDelay = 1.35f; 
        float bulletTravelDist;
        public Rectangle enemyRect;
        public float moveSpeed = .5f;
        int unblockedCount = 0;
        Vector2 pos;
        float gravResistance;
        public float health = 20;
        static int pixelSize = 64;
        Vector2 velocity = Vector2.Zero;
        float yOffset = 64 * 3; //How high from the ground 
        Rectangle groundRect;
        public Rectangle collisionRect;
        Texture2D visionTexture;
        //Texture2D texture;
        int visionLength;
        List<Rectangle> vision = new List<Rectangle>();
        float attackDelay = 1.5f;
        float launchStr = 12;
        float initYPos;
        float maxSpeed = 4f;
        bool blockBottom, blockRight, blockLeft, blockTop;

        FlyingControlBeacon controlBeacon;// Range that it is allowed to move
        public bool delete = false; //Overall death

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
        Texture2D collisionTexture;
        ContentManager content;
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
                    texture = content.Load<Texture2D>("SideScroll/Animations/DroneEnemy");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(2, 1);
                    fpms = 120;
                    break;
                default:
                    texture = content.Load<Texture2D>("SideScroll/Animations/DroneEnemy");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(2, 1);
                    fpms = 120;
                
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

            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, new Vector2(enemyRect.X, enemyRect.Y));

            animManager.isRight = isRight;
            animManager.isLeft = isLeft;
            animManager.isUp = isUp;
            animManager.isDown = isDown;
            animManager.StopLoop();
        }
        #endregion

        public FlyingEnemy(ContentManager contentManager, int visionLength, Vector2 position, FlyingControlBeacon controlBeacon)
        {
            this.visionLength = visionLength;
            this.controlBeacon = controlBeacon;
            pos = position;
            enemyRect = new Rectangle((int)position.X, (int)position.Y, pixelSize, pixelSize);
            texture = contentManager.Load<Texture2D>(@"SideScroll/Animations/DroneEnemy");
            collisionTexture = contentManager.Load<Texture2D>(@"Textures\Button");
            visionTexture = contentManager.Load<Texture2D>(@"Textures\Red");
            CreateVision();
            prevState = enemyState;
            content = contentManager;
            animState = AnimationStates.Walking;
            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, new Vector2(enemyRect.X, enemyRect.Y));
            ChangeAnimation();
            particles = new ParticleManager();
            particles.Initialize(contentManager.Load<Texture2D>(@"Textures\white"));
            
            

            iShootDelay = shootDelay;
        }

        public void Update(GameTime gameTime, Vector2 gravity, SSPlayer player, SideTileMap map, Rectangle currBounds)
        {
            blockBottom = false;
            blockLeft = false;
            blockRight = false;
            blockTop = false;
            //yOffset = 64 * 3;

            if (health == 0)
            {
                enemyState = EnemyStates.Dead;
            }
           
             

            switch (enemyState)
            {
                case EnemyStates.Idle:
                    if (groundRect == Rectangle.Empty)
                    {
                        groundRect = new Rectangle(enemyRect.X + enemyRect.Width / 2, enemyRect.Bottom - enemyRect.Height / 2, enemyRect.Width, enemyRect.Height / 2);
                    }
                    //groundRect = enemyRect;
                    foreach (Rectangle rect in vision)
                    {
                        if (rect.Intersects(player.playerRect))
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

                    if (controlBeacon.Health < controlBeacon.MaxHealth)
                    {
                        velocity = new Vector2(0, -launchStr);
                        initYPos = pos.Y;
                        prevState = enemyState;
                        enemyState = EnemyStates.Launch;
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
                            collisionRect = groundRect;
                            collisionRect.Width /= 2;
                            //collisionRect.X += collisionRect.Width / 2;
                            groundRect = new Rectangle(enemyRect.X, enemyRect.Y + groundRect.Height / 2,
                                  groundRect.Width, groundRect.Height);
                            break;
                        }
                    }
                    foreach (PlatformTile platform in map.GetPlatformTiles())
                    {
                        if (groundRect.TouchTopOf(platform.Rectangle))
                        {

                            groundRect = new Rectangle(groundRect.X, platform.Rectangle.Y - groundRect.Height, groundRect.Width, groundRect.Height);
                            collisionRect = groundRect;
                            collisionRect.Width /= 2;
                            //collisionRect.X += collisionRect.Width / 2;
                            groundRect = new Rectangle(enemyRect.X, enemyRect.Y + groundRect.Height / 2,
                                groundRect.Width, groundRect.Height);
                            break;
                        }
                    }
                    if (MathHelper.Distance(initYPos, pos.Y) >= yOffset || blockTop)
                    {
                        prevState = enemyState;
                        enemyState = EnemyStates.GoTo;
                        velocity = Vector2.Zero;
                    }

                    if (groundRect.Y > collisionRect.Y)
                    {
                        collisionRect.Y = enemyRect.Y + collisionRect.Height;
                    }
                    break;
                case EnemyStates.GoTo:
                    groundRect = new Rectangle((int)(groundRect.X + gravity.X), (int)(groundRect.Y + gravity.Y * 10), groundRect.Width, groundRect.Height);
                    collisionRect.X = groundRect.X;


                    foreach (GroundTile ground in SideTileMap.GroundTiles)
                    {
                        Collision(ground.Rectangle);
                        if (groundRect.TouchTopOf(ground.Rectangle))
                        {

                            groundRect = new Rectangle(groundRect.X, ground.Rectangle.Y - groundRect.Height, groundRect.Width, groundRect.Height);
                            collisionRect = groundRect;
                            collisionRect.Width /= 2;
                            //collisionRect.X += collisionRect.Width / 2;
                            groundRect = new Rectangle(enemyRect.X, enemyRect.Y + groundRect.Height / 2,
                                groundRect.Width, groundRect.Height);
                            break;
                        }
                    }
                    foreach (WallTile wall in SideTileMap.WallTiles)
                    {
                        Collision(wall.Rectangle);
                    }
                    foreach (PlatformTile platform in SideTileMap.PlatformTiles)
                    {
                        Collision(platform.Rectangle);
                        if (groundRect.TouchTopOf(platform.Rectangle))
                        {

                            groundRect = new Rectangle(groundRect.X, platform.Rectangle.Y - groundRect.Height, groundRect.Width, groundRect.Height);
                            collisionRect = groundRect;
                            collisionRect.Width /= 2;
                            //collisionRect.X += collisionRect.Width / 2;
                            groundRect = new Rectangle(enemyRect.X, enemyRect.Y + groundRect.Height / 2,
                                 groundRect.Width, groundRect.Height);
                            break;
                        }
                    }


                    #region Basic Movement
                    bool canMove = false;

                    canMove = controlBeacon.InRange(enemyRect);
                   
                    if ((int)pos.X < (int)player.Position.X)
                    {
                        if (velocity.X < 0)
                        {
                            velocity.X = -velocity.X;
                            velocity.X += moveSpeed;

                            if (velocity.X > maxSpeed)
                            {
                                velocity.X = maxSpeed;
                            }

                            if (pos.X + velocity.X > player.Position.X)
                            {
                                velocity.X = 0;
                            }
                        }
                        else if (velocity.X >= 0)
                        {
                            velocity.X += moveSpeed;
                            if (velocity.X > maxSpeed)
                            {
                                velocity.X = maxSpeed;
                            }

                            if (pos.X + velocity.X > player.Position.X)
                            {
                                velocity.X = 0;
                            }
                        }



                    }
                    else if ((int)pos.X > (int)player.Position.X)
                    {
                        if (velocity.X > 0)
                        {
                            velocity.X = -velocity.X;
                            velocity.X -= moveSpeed / 2;

                            if (velocity.X < -maxSpeed)
                            {
                                velocity.X = -maxSpeed;
                            }
                        }
                        else if (velocity.X <= 0)
                        {
                            velocity.X -= moveSpeed / 2;

                            if (velocity.X < -maxSpeed)
                            {
                                velocity.X = -maxSpeed;
                            }
                        }

                        if (pos.X + velocity.X < player.Position.X)
                        {
                            velocity.X = 0;
                        }
                    }
                    if (blockLeft && player.Position.Y > pos.Y || blockRight && player.Position.Y > pos.Y)
                    {
                        yOffset = 64;
                        //unblockedCount = 0;


                    }
                    else if (!blockLeft && yOffset == 64 || !blockRight && yOffset == 64)
                    {
                        unblockedCount++;
                        if (unblockedCount > 5)
                        {
                            unblockedCount = 0;
                            yOffset = 64 * 3;
                        }


                        //pos.X += velocity.X * 10;
                    }

                    if (MathHelper.Distance(enemyRect.Y + velocity.Y, player.playerRect.Y) > yOffset && !blockBottom)
                    {
                        if (velocity.Y < 0)
                        {
                            velocity.Y = -velocity.Y;
                        }
                        velocity.Y += moveSpeed / 2;
                        if (velocity.Y > maxSpeed / 1.5f)
                        {
                            velocity.Y = maxSpeed / 1.5f;
                        }

                        if (MathHelper.Distance(enemyRect.Y + velocity.Y, player.playerRect.Y) < yOffset)
                        {
                            velocity.Y = 0;
                        }
                    }
                    else if (MathHelper.Distance(enemyRect.Y + velocity.Y, player.playerRect.Y) < yOffset && !blockTop)
                    {
                        if (velocity.Y > 0)
                        {
                            velocity.Y = -velocity.Y;
                        }

                        velocity.Y -= moveSpeed / 2;
                        if (velocity.Y < -maxSpeed / 1.5f)
                        {
                            velocity.Y = -maxSpeed / 1.5f;
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
                    if (groundRect.Y > collisionRect.Y)
                    {
                        collisionRect.Y = enemyRect.Y + collisionRect.Height;
                    }

                    if (canMove)
                        pos += velocity;
                    else
                        pos -= velocity;
                    #endregion


                    if (groundRect.Intersects(player.playerRect) && currBounds.Contains(enemyRect) ||
                        MathHelper.Distance(enemyRect.Center.X, player.playerRect.Center.X) < attackDist && enemyRect.Center.Y < player.playerRect.Bottom)
                    {
                        enemyState = EnemyStates.Attacking;
                    }

                    break;
                case EnemyStates.Attacking:
                    shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (shootDelay <= 0)
                    {
                        Vector2 targetDir = new Vector2(player.playerRect.X + player.playerRect.Width / 2, player.playerRect.Y + player.playerRect.Height / 2) -
                          new Vector2(enemyRect.Center.X, enemyRect.Center.Y);
                        float angle = (float)Math.Atan2(targetDir.Y, targetDir.X);
                        LaunchBomb(player, angle);
                    }

                    if(MathHelper.Distance(enemyRect.Center.X, player.playerRect.Center.X) > attackDist)
                    {
                        enemyState = EnemyStates.GoTo;
                    }
                    break;
                case EnemyStates.Dead:
                    pos.Y += gravity.Y;

                    foreach(GroundTile tile in SideTileMap.GroundTiles)
                    {
                        if(enemyRect.TouchTopOf(tile.Rectangle))
                        {
                            delete = true;
                        }
                    }

                    foreach(PlatformTile  tile in SideTileMap.PlatformTiles)
                    {
                        if(enemyRect.TouchTopOf(tile.Rectangle))
                        {
                            delete = true;
                        }
                    }
                    break;
            }

            UpdateBombs(gameTime, player);
            particles.Update(gameTime);
            if (groundRect.Y > currBounds.Bottom)
            {
                groundRect = new Rectangle(enemyRect.X + enemyRect.Width / 2, enemyRect.Bottom - enemyRect.Height / 2,
                    enemyRect.Width, enemyRect.Height / 2);
            }

            enemyRect = new Rectangle((int)pos.X, (int)pos.Y, pixelSize, pixelSize);
            animManager.Update(gameTime, new Vector2(enemyRect.X, enemyRect.Y));
            
        }

        private void UpdateBombs(GameTime gameTime, SSPlayer player)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(gameTime);
                if (bullets[i].delete || player.playerRect.Contains(bullets[i].rect.Center))
                {
                    explosions.Add(new Explosion(new Circle(new Vector2(bullets[i].rect.X, bullets[i].rect.Y), bullets[i].rect.Width),
                       growthRate, (int)(bullets[i].rect.Width * 2.5f)));

                    int radiusDif = explosions[explosions.Count - 1].maxSize - explosions[explosions.Count - 1].rect.Radius;

                    particles.MakeExplosion(explosions[explosions.Count - 1].rect.Bounds, 
                        new Circle(new Vector2(explosions[explosions.Count - 1].rect.Bounds.X - radiusDif,
                        explosions[explosions.Count - 1].rect.Bounds.Y - radiusDif), explosions[explosions.Count - 1].maxSize/2),
                        20);

                    //particles.MakeExplosion(explosions[explosions.Count - 1].rect.Bounds, new Circle(new Vector2(explosions[explosions.Count - 1].rect.Bounds.X - radiusDif, explosions[explosions.Count - 1].rect.Bounds.Y - radiusDif), explosions[explosions.Count - 1].rect.Radius)

                    //   new Rectangle(explosions[explosions.Count - 1].rect.Bounds.X - radiusDif, explosions[explosions.Count - 1].rect.Bounds.Y - radiusDif,
                    //   explosions[explosions.Count - 1].rect.Bounds.Width + radiusDif, explosions[explosions.Count - 1].rect.Bounds.Height + radiusDif),
                    //   20);

                    bullets.RemoveAt(i);
                }
            }

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].rect.Radius >= explosions[i].maxSize)
                {
                    //particles.CreateEffect(20);

                    explosions.RemoveAt(i);

                }
            }
        }

        private void LaunchBomb(SSPlayer ssPlayer, float angle)
        {
                shootDelay = iShootDelay;
                bulletTravelDist = DistForm(ssPlayer.playerRect.Center.ToVector2(), enemyRect.Center.ToVector2());

                Vector2 bossPos = new Vector2(enemyRect.Center.X - ssPlayer.playerRect.Width / 2, enemyRect.Center.Y - ssPlayer.playerRect.Height / 2);

                float bulletSpeedX = (float)Math.Cos((double)angle) * 2;
                float bulletSpeedY = (float)Math.Sin((double)angle) * 2;

                bullets.Add(new Bullet(bossPos, bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY),
                    content, true, bulletTravelDist, true, bulletSpeedY, size: 21));
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {


            //tRect.X += tRect.Width/5;
            animManager.Draw(spriteBatch, Color.White);
            //spriteBatch.Draw(texture, enemyRect, Color.White);
            //spriteBatch.Draw(texture, groundRect, Color.White);
            //spriteBatch.Draw(collisionTexture, collisionRect, Color.Blue);

            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }

            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch, content);
            }

            particles.Draw(spriteBatch);
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

            if (enemyRect.TouchTopOf(newRect, true)) //Touch Ground
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

            if (enemyRect.TouchBottomOf(newRect, true)) //Colliding Top or touching bottom of tile
            {
                blockTop = true;
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

            if (enemyRect.TouchLeftOf(newRect, true))//enemy is colliding to the right
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

            if (enemyRect.TouchRightOf(newRect, true))//enemy is Colliding to the left
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

           
        }
        int DistForm(Vector2 pos1, Vector2 pos2)
        {
            int num = (int)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
            return num;

        }
    }


}

class Explosion
{
    public Circle rect;
    public int growthRate;
    public int maxSize;
    //public ParticleManager particles;

    public Explosion(Circle rect, int rate, int max)
    {
        this.rect = rect;
        growthRate = rate;
        maxSize = max;
        //particles.CreateEffect(20);
    }

    public void Update(GameTime gameTime)
    {
        if (rect.Radius < maxSize)
        {
            rect.Radius += growthRate;
            rect.Position = new Vector2(rect.Bounds.X - growthRate, rect.Bounds.Y - growthRate);
        }



    }

    public void Draw(SpriteBatch spriteBatch, ContentManager content)
    {
        //spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), rect.Bounds, Color.FloralWhite * .25f);
    }
}
