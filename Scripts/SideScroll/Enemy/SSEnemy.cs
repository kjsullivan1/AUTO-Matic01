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

namespace AUTO_Matic.SideScroll
{
    class SSEnemy
    {
        ContentManager content;

        enum AnimationStates { Walking, Idle, Death, Jump, Shoot}
        AnimationStates animState = AnimationStates.Idle;

        public enum EnemyStates { Idle, GoTo, Attacking,Jumping, Falling, Knockback}
        public EnemyStates enemyState = EnemyStates.Idle;
        public EnemyStates prevState;

        #region Fields
        public Vector2 bounds;//The bounds of how much the AI can move left and right
        int pixelSize = 64;
        Vector2 position = new Vector2(64 * 15 + 5, 0);
        public Rectangle enemyRect;
        bool blockBottom = false; 
        bool isFalling = true;
        bool isColliding = false;

        Vector2 TargetPos; //Position of where the GOTO targets 
        public float health = 5;
        public bool dead = false;
        bool damaged = false;

        public int redFrames = 4;
        public int redCount = 0;
        int whiteFrames = 30;
        int whiteCount = 0;

        public float knockBackX;
        public float knockBackY;

        float gravX;
        public float Health
        {
            get { return health; }
            set { 
                if(value < health && !damaged)
                {
                    damaged = true;
                    health = value;
                }
               
                if (health <= 0)
                {
                    dead = true;
                }
            }
        }

        float moveSpeed = .5f;
        float mass = 20.0f;
        public float accel = 0;
        public float force = 0;
        public float friction = 0;
        float coeFric = 0;
        public float changeInTime = 0;
        //bool canJump;
        int jumpDelay = 5;
        int maxJumpDelay = 0;
        Vector2 gravity;//Stored version of gravity 

        public float maxRunSpeed = 5f;
        float terminalVel = 12f;
        float maxJumpSpeed = 8f;
        int maxJumpForce = 20;
        int minJumpForce = 16;
        float maxAirSpeed;
        public bool isShoot;
        int leftOnY = 0;
        #endregion

        #region Velocity
        public Vector2 velocity = Vector2.Zero;
        Vector2 positionOffset = new Vector2(0, 0);
        public Vector2 Velocity  //Used to keep velocity within limits depedning on the state
        {
            get
            {
                Vector2 vel = velocity;
                if(vel.X > 0 && vel.X > maxRunSpeed)
                {
                    vel.X = maxRunSpeed;
                }
                if(vel.X < 0 && vel.X < -maxRunSpeed)
                {
                    vel.X = -maxRunSpeed;
                }

                if(vel.Y > terminalVel)
                {
                    vel.Y = terminalVel;
                }

                velocity = vel;
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }
        #endregion

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
                    if(isShoot)
                    {
                        texture = content.Load<Texture2D>("SideScroll/Animations/RangedEnemyWalk");
                        FrameSize = new Point(64, 64);
                        CurrFrame = new Point(0, 0);
                        SheetSize = new Point(4, 1);
                        fpms = 120;
                    }
                    else
                    {
                        texture = content.Load<Texture2D>("SideScroll/Animations/MeleeEnemyWalk");
                        FrameSize = new Point(64, 64);
                        CurrFrame = new Point(0, 0);
                        SheetSize = new Point(4, 1);
                        fpms = 120;
                    }
                    break;
                default:
                    if (isShoot)
                    {
                        texture = content.Load<Texture2D>("SideScroll/Animations/RangedEnemyWalk");
                        FrameSize = new Point(64, 64);
                        CurrFrame = new Point(0, 0);
                        SheetSize = new Point(4, 1);
                        fpms = 120;
                    }
                    else
                    {
                        texture = content.Load<Texture2D>("SideScroll/Animations/MeleeEnemyWalk");
                        FrameSize = new Point(64, 64);
                        CurrFrame = new Point(0, 0);
                        SheetSize = new Point(4, 1);
                        fpms = 120;
                    }
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

            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, position);

            animManager.isRight = isRight;
            animManager.isLeft = isLeft;
            animManager.isUp = isUp;
            animManager.isDown = isDown;
        }
        #endregion

        #region Constructor
        public SSEnemy(ContentManager manager, Rectangle Bounds, int visionLength, Vector2 position, bool isShoot)
        {
            this.position = position;
            enemyRect = new Rectangle((int)position.X, (int)position.Y, 20, 48);
            content = manager;
            bounds = new Vector2(0, Bounds.Width);
            texture = manager.Load<Texture2D>(@"Textures\TitleCrawlBG");
            visionTxture = content.Load<Texture2D>(@"Textures\Red");
            this.visionLength = visionLength;
            positionOffset = new Vector2(positionOffset.X * pixelSize, positionOffset.Y * pixelSize);
            CreateVision();
            this.isShoot = isShoot;
            attackDelayMax = attackDelay;
            maxShootDelay = shootDelay;
            if(isShoot)
                attackOffsetFromPlayer = 64 * 3;

            animState = AnimationStates.Walking;
            ChangeAnimation();
        }
        #endregion

        #region AI Helpers
        public List<Rectangle> vision = new List<Rectangle>(); //The trigger rectangles for vision
        public List<Rectangle> possibleJumpLocations = new List<Rectangle>(); 
        int visionLength = 1;
        Texture2D visionTxture;
        GAJump gaJump = new GAJump();
        bool canJump = false;
        bool jumpSuccess = false;
        bool jumpFail = false;
        JumpChromosome jumpInfo;
        Rectangle goalRect;
        bool blockTop = false;
        Rectangle currPlatform;
        public bool onPlatform = false;
        bool blockLeft = false;
        bool blockRight = false;
        bool goTo = false; //A hard setting goTo that forces to go towards TargetPos regardless of checks
        Vector2 landingPos;
        bool canWalk = true;
        public List<int> leftOnX = new List<int>(); //This is used for when the player jump/falls from current platform
        bool forceRun = false;
        bool atBounds = false;
        #endregion

        #region Attack Helpers
        public List<Bullet> bullets = new List<Bullet>();
        float bulletSpeed = 5;
        float bulletDmg = 1f;
        float shootDelay = .8f;
        float maxShootDelay;
        float bulletTravelDist = 64 * 4;
        Vector2 maxBulletSpeed = new Vector2(15, 15);


        #endregion


        #region Attacks
        float attackOffsetFromPlayer = 10f;
        float attackOffset = 10f;
        float attackDelay = 2.25f;
        float attackDelayMax;
        bool attackLeft = false, attackRight = false;
        int attackBoxWidth = 40, attackBoxHeight = 40;
        float meleeDmg = 1.5f;
        bool outOfRange = false;
        bool cantReach = true;
        float waitTime = .75f;
        Rectangle HitBox
        {
            get
            {
                if(attackLeft)
                {
                    return new Rectangle((int)((enemyRect.X - attackBoxWidth) - attackOffset), (enemyRect.Y + enemyRect.Height / 2) - attackBoxHeight / 2, attackBoxWidth, attackBoxHeight);
                }
                else
                {
                    return new Rectangle((int)((enemyRect.X + enemyRect.Width) + attackOffset), (enemyRect.Y + enemyRect.Height/2) - attackBoxHeight / 2, attackBoxWidth, attackBoxHeight);
                }
               
            }
        }
        #endregion
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


        void GoTo()//bounds is the min and max bounds in the x direction left to right
        { //if + movespeed > Target velocity.x = 0
            switch(enemyState)
            {
              
                case EnemyStates.GoTo:
                    if(animState != AnimationStates.Walking)
                    {
                        animState = AnimationStates.Walking;
                        ChangeAnimation();
                    }
                    
                    //if(MathHelper.Distance(position.X, landingPos.X) > bounds.X && velocity.X < 0)
                    //{
                    //    bounds = SideTileMap.GetNumTilesOfGround((int)(position.Y / 64) + 1, (int)position.X / 64); //Recalc bounds 
                    //    if (bounds != Vector2.Zero)
                    //    {
                    //        bounds *= 64;
                    //    }
                    //    if (landingPos.Y != position.Y)
                    //    {
                    //        landingPos = position;
                    //    }
                        
                    //}
                    //else if(MathHelper.Distance(position.X, landingPos.X) > bounds.Y && velocity.X > 0)
                    //{
                    //    bounds = SideTileMap.GetNumTilesOfGround((int)(position.Y / 64) + 1, (int)position.X / 64); //Recalc bounds 
                    //    if (bounds != Vector2.Zero)
                    //    {
                    //        bounds *= 64;
                    //    }
                    //    if (landingPos.Y != position.Y)
                    //    {
                    //        landingPos = position;
                    //    }
                    //}
                    //else
                    {
                        if (position.X < TargetPos.X) //Going Right
                        {
                            if ((int)position.X + (int)velocity.X > (int)TargetPos.X)
                            {
                                velocity.X = 0;
                                if (prevState == EnemyStates.Jumping)
                                {
                                    position.X = TargetPos.X;
                                    goTo = false;
                                    onPlatform = true;
                                    prevState = EnemyStates.Idle;
                                }

                                //Change enemyState
                            }
                            else if (MathHelper.Distance(position.X, landingPos.X) >= bounds.Y && prevState != EnemyStates.Jumping && goTo && prevState != EnemyStates.Falling)//checking border
                            {
                                velocity.X = 0;
                                position.X += 1;
                                enemyRect.X = (int)position.X;
                            }
                            else
                            {
                                if (velocity.X <= 0)
                                {
                                    velocity.X = -velocity.X;
                                }
                                velocity.X += moveSpeed;
                            }
                        }
                        if (position.X > TargetPos.X) //Going left
                        {
                            if ((int)position.X - (int)velocity.X < (int)TargetPos.X) //Normal
                            {
                                velocity.X = 0;
                                if (prevState == EnemyStates.Jumping)
                                {
                                    position.X = TargetPos.X;
                                    goTo = false;
                                    onPlatform = true;
                                    prevState = EnemyStates.Idle;
                                }

                                //change enemyState
                            }
                            else if (MathHelper.Distance(position.X, landingPos.X) >= bounds.X && prevState != EnemyStates.Jumping && goTo && prevState != EnemyStates.Falling) //Border check
                            {
                                velocity.X = 0;
                                position.X -= 1;
                                enemyRect.X = (int)position.X;
                            }
                            else
                            {
                                if (velocity.X >= 0)
                                {
                                    velocity.X = -velocity.X;
                                }
                                velocity.X -= moveSpeed;
                            }
                        }
                        if ((int)(position.X + .5f) == (int)TargetPos.X)//Reached target
                        {
                            goTo = false;
                        }
                        else if(velocity.X < 0 && position.X + velocity.X < TargetPos.X)
                        {
                            goTo = false;
                        }
                        else if(velocity.X > 0 && position.X + velocity.X > TargetPos.X)
                        {
                            goTo = false;
                        }

                        //if (MathHelper.Distance(position.X, landingPos.X) > bounds.X && velocity.X < 0 && prevState != EnemyStates.Jumping && prevState != EnemyStates.Falling && TargetPos.Y > enemyRect.Bottom == false && !isFalling)
                        //{
                        //    position.X += Math.Abs(velocity.X);
                        //}
                        //if (MathHelper.Distance(position.X, landingPos.X) > bounds.Y && velocity.X > 0 && prevState != EnemyStates.Jumping && prevState != EnemyStates.Falling && TargetPos.Y > enemyRect.Bottom == false && !isFalling)
                        //{
                        //    if (velocity.X < 0)
                        //    {
                        //        position.X += velocity.X;
                        //    }
                        //    else
                        //        position.X += -velocity.X;
                        //}
                    }
                    
                    if(blockLeft && velocity.X < 0)
                    {
                        enemyState = EnemyStates.Idle;
                    }
                    if(blockRight && velocity.X > 0)
                    {
                        enemyState = EnemyStates.Idle;
                    }

                    break;
                //case EnemyStates.Jumping:
                //    if(position.X < TargetPos.X)
                //    {
                //        if ((int)position.X + (int)velocity.X > (int)TargetPos.X)
                //        {
                //            velocity.X = 0;
                //            goTo = false;
                //            onPlatform = true;
                //            //prevState = EnemyStates.Idle;
                //        }
                //        else
                //        {
                            
                //            velocity.X += moveSpeed;
                //        }
                //    }
                //    if(position.X > TargetPos.X)
                //    {
                //        if((int)position.X - (int)velocity.X < (int)TargetPos.X)
                //        {
                //            velocity.X = 0;
                //            goTo = false;
                //            onPlatform = true;
                //            //prevState = EnemyStates.Idle;
                //        }
                //        else
                //        {
                //            velocity.X -= moveSpeed;
                //        }
                //    }
                //    break;
            }
        }

        public void Update(GameTime gameTime, Vector2 gravity, SSPlayer player, Game1 game, Vector2 knockbackForce)
        {
            //Reset and clear 
            this.gravity = gravity;
            CreateVision();
            possibleJumpLocations.Clear();
            blockBottom = false;
            //blockTop = false;
            //onPlatform = false;
            //isFalling = true;
            blockLeft = false;
            blockRight = false;
            outOfRange = false;

            //if(dead)
            //{
            //    position = new Vector2(-100, 10000);
            //}

            foreach(GroundTile tile in SideTileMap.GroundTiles)
            {
                if (enemyRect.TouchTopOf(tile.Rectangle))
                    onPlatform = false;
                Collision(tile.Rectangle);
            }

            if(onPlatform || velocity.Y != 0) //If already on platform or moving vertically
            {
                foreach(PlatformTile tile in SideTileMap.PlatformTiles)
                {
                    Collision(tile.Rectangle);
                }
            }
            

            //Collision
            if(blockBottom) //If colliding bottom, def not falling
            {
                isFalling = false;
            }
            else
            {
                isFalling = true;
            }

            if(prevState == EnemyStates.Attacking)//Cooldowns for attacks
            {
                if(isShoot)
                {
                    shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    attackDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
              
                
            }

            switch(enemyState)
            {
                case EnemyStates.Idle:
                    //Waits for player to enter vision
                    if(animState != AnimationStates.Idle)
                    {
                        animState = AnimationStates.Idle;
                        ChangeAnimation();
                    }
                    if(cantReach)
                    {
                       if((int)enemyRect.Y/64 == (int)player.playerRect.Y/64 && player.blockBottom)
                       {
                            cantReach = false;
                       }
                    }
                    else
                    {
                        foreach (Rectangle rect in vision)
                        {

                            if (rect.Intersects(player.playerRect))
                            {
                                if (isShoot)
                                {
                                    if (enemyRect.Right < player.playerRect.X /*&& MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer*/)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X, enemyRect.Y);
                                    }
                                    else if (enemyRect.Left > player.playerRect.X + player.playerRect.Width /*&& MathHelper.Distance(enemyRect.Left, player.playerRect.Right) > attackOffsetFromPlayer*/)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                                    }
                                }
                                else
                                {
                                    if (enemyRect.Right < player.playerRect.X && MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X, enemyRect.Y);
                                    }
                                    else if (enemyRect.Left > player.playerRect.X + player.playerRect.Width && MathHelper.Distance(enemyRect.Left, player.playerRect.Right) > attackOffsetFromPlayer)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                                    }
                                }
                                //bounds = SideTileMap.GetNumTilesOfGround((int)(position.Y / 64) + 1, (int)position.X / 64); //Recalc bounds 
                                //if (bounds != Vector2.Zero)
                                //{
                                //    bounds *= 64;
                                //}
                                if (MathHelper.Distance(enemyRect.X + enemyRect.Width / 2, player.playerRect.X + player.playerRect.Width / 2) > (visionLength) * 64)
                                {
                                    enemyState = EnemyStates.Idle;
                                    break;
                                }
                                prevState = enemyState;
                                enemyState = EnemyStates.GoTo;
                                break;

                            }

                        }
                    }
                    leftOnX.Clear();
                   
                    velocity.X = 0;
                    break;
                case EnemyStates.GoTo:
                    //bool outOfSight = true;
                    if (!goTo) //If not forced to into goTo
                    {
                      
                        foreach (Rectangle rect in vision)//Update TargetPos if player is in vision
                        {
    
                            if (rect.Intersects(player.playerRect))
                            {
                                //outOfSight = false;
                                if(isShoot)
                                {
                                    if (enemyRect.Right < player.playerRect.X /*&& MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer*/)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X, enemyRect.Y); //Keep Y to only set the X coordinate...Y handled seperately
                                        outOfRange = false;
                                    }
                                    else if (enemyRect.Left > player.playerRect.X + player.playerRect.Width /*&& MathHelper.Distance(enemyRect.Left, player.playerRect.Right) > attackOffsetFromPlayer*/)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                                        outOfRange = false;
                                    }
                                   
                                }
                                else
                                {
                                    if (enemyRect.Right < player.playerRect.X && MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X, enemyRect.Y); //Keep Y to only set the X coordinate...Y handled seperately
                                    }
                                  
                                    if (enemyRect.Left > player.playerRect.X + player.playerRect.Width && MathHelper.Distance(enemyRect.Left, player.playerRect.Right) > attackOffsetFromPlayer)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                                    }
                                 
                                }
                                bounds = SideTileMap.GetNumTilesOfGround((int)(position.Y / 64) + 1, (int)position.X / 64); //Recalc bounds 
                                if (bounds != Vector2.Zero)
                                {
                                    bounds *= 64;
                                }
                                //landingPos = position;
                            }
                            //if(outOfSight && prevState != EnemyStates.Idle)
                            //{
                            //    enemyState = EnemyStates.Idle;
                            //    break;
                            //}
                        }
                    }

                    if(animState != AnimationStates.Walking)
                    {
                        animState = AnimationStates.Walking;
                        ChangeAnimation();
                    }

                    if (player.velocity.Y == 0 && player.playerRect.Bottom >= enemyRect.Bottom && player.isFalling == false)
                    {
                        leftOnX.Clear();//Clears if player touches the ground,but should account for when the player jumps multi tiles high 
                        leftOnY = 0;
                    }


                    //Player is above the enemy, isnt't forced into goTo and the enemy is on the ground
                    if (player.playerRect.Bottom < enemyRect.Top && player.velocity.Y >= 0 && blockBottom && !goTo/*&& player.blockBottom*/ || blockLeft && goTo|| blockRight && goTo)
                    {
                        //if (player.velocity.Y >= -5)
                        //{
                        //    leftOnX = player.playerRect.X; //where the playere was last to set direction priority if jump fails
                        //}
                        goTo = false;
                        if (player.blockBottom || blockLeft && !blockBottom|| blockRight && !blockBottom)
                        {
                            if(!isShoot)
                                enemyState = EnemyStates.Jumping;  //Calculate and determine possible jumps
                            break;
                        }
                        else
                        {
                            //leftOnX = (int)velocity.x;
                            //velocity.X = 0;
                        }
                       
                    }
                    //else if (velocity.X < 0 && bounds.X == 0 && blockBottom && player.velocity.Y <= 0 && !goTo && leftOnY != 0 && leftOnY > enemyRect.Bottom)
                    //{
                    //    enemyState = EnemyStates.Falling;
                    //    break;
                    //}
                    //else if (leftOnY == 0 && bounds.X == 0 && velocity.X < 0 && blockBottom && player.velocity.Y <= 0 && !goTo && player.playerRect.Top > enemyRect.Bottom)
                    //{
                    //    enemyState = EnemyStates.Falling;
                    //    break;
                    //}
                    //else if (velocity.X > 0 && bounds.Y == 0 && blockBottom && player.velocity.Y <= 0 && !goTo && leftOnY != 0 && leftOnY > enemyRect.Bottom)
                    //{
                    //    enemyState = EnemyStates.Falling;
                    //    break;
                    //}
                    //else if (leftOnY == 0 && velocity.X > 0 && bounds.Y == 0 && blockBottom && player.velocity.Y <= 0 && !goTo && player.playerRect.Top > enemyRect.Bottom)
                    //{
                    //    enemyState = EnemyStates.Falling;
                    //    break;
                    //}
                    //else if (leftOnY != 0 && leftOnY > enemyRect.Bottom && player.velocity.Y <= 0 && blockBottom && !goTo)//Player is below enemy and will check to see if can go down
                    //{
                    //    enemyState = EnemyStates.Falling;
                    //    break;
                    //}
                    //else if (player.playerRect.Top > enemyRect.Bottom && player.velocity.Y <= 0 && blockBottom && !goTo)
                    //{
                    //    enemyState = EnemyStates.Falling;
                    //    break;
                    //}

                    //Attack conditions Makes sure the enemy isnt jumping and that they are on the same level
                    //left
                    if (enemyRect.Left > player.playerRect.Right && player.playerRect.Bottom < enemyRect.Top == false && player.blockBottom && player.playerRect.Top > enemyRect.Bottom == false && prevState != EnemyStates.Jumping)
                    {
                        if(isShoot && !outOfRange && blockBottom)
                        {
                            if (MathHelper.Distance(enemyRect.Left, player.playerRect.Right) < attackOffsetFromPlayer)
                            {
                                attackLeft = true;
                                attackRight = false;
                                enemyState = EnemyStates.Attacking;
                                velocity.X = 0;
                            }
                        }
                        else if(blockBottom && !isShoot)
                        {
                            if (MathHelper.Distance(enemyRect.Left, player.playerRect.Right) < attackOffsetFromPlayer)
                            {
                                attackLeft = true;
                                attackRight = false;
                                enemyState = EnemyStates.Attacking;
                                velocity.X = 0;
                            }
                        }
                       
                    }//right
                    else if(enemyRect.Right < player.playerRect.Left && player.playerRect.Bottom < enemyRect.Top == false && player.blockBottom && player.playerRect.Top > enemyRect.Bottom == false && prevState != EnemyStates.Jumping)
                    {
                        if(isShoot && !outOfRange && blockBottom)
                        {
                            if (MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer)
                            {
                                attackLeft = false;
                                attackRight = true;
                                enemyState = EnemyStates.Attacking;
                                velocity.X = 0;
                            }
                        }
                        else if(blockBottom && !isShoot)
                        {
                            if (MathHelper.Distance(enemyRect.Right, player.playerRect.Left) < attackOffsetFromPlayer)
                            {
                                attackLeft = false;
                                attackRight = true;
                                enemyState = EnemyStates.Attacking;
                                velocity.X = 0;
                            }
                        }
                        
                    }
                    else //Adjust enemy to stop colliding with player
                    {
                        if(enemyRect.Intersects(player.playerRect))
                        {
                            if(position.X < player.Position.X)
                            {
                                TargetPos = new Vector2(player.Position.X - attackOffsetFromPlayer);
                            }
                            else if(position.X > player.Position.X)
                            {
                                TargetPos = new Vector2(player.playerRect.Right + attackOffsetFromPlayer);
                            }
                            else if(position.X == player.Position .X)
                            {
                                if(player.animManager.isRight)
                                {
                                    TargetPos = new Vector2(player.playerRect.Right + attackOffsetFromPlayer);
                                }
                                else
                                {
                                    TargetPos = new Vector2(player.Position.X - attackOffsetFromPlayer);
                                }
                            }
                        }
                    }
                    float num = MathHelper.Distance(enemyRect.X + enemyRect.Width / 2, player.playerRect.X + player.playerRect.Width / 2);
                    if (MathHelper.Distance(enemyRect.X + enemyRect.Width / 2, player.playerRect.X + player.playerRect.Width / 2) > (visionLength + 1) * 64)
                    {
                        enemyState = EnemyStates.Idle;
                        break;
                    }

                    if(enemyState == EnemyStates.GoTo)
                    {
                        GoTo();
                    }
                   


                    break;
                #region Jump
                case EnemyStates.Jumping: //This is just a caculation to determine if they can jump from current position
                    if(prevState != EnemyStates.Jumping /*|| player.playerRect.Bottom < enemyRect.Top && player.velocity.Y >= 0 && onPlatform*/)
                    {
                        possibleJumpLocations.Clear();
                        //Grab platform tiles
                        foreach(PlatformTile tile in SideTileMap.PlatformTiles)
                        {
                            for(int i = vision.Count - 1; i >= 0; i--)
                            {
                                if (vision[i].Intersects(tile.Rectangle))
                                {
                                    //if (rect.Bottom > enemyRect.Bottom && !onPlatform)//this is the small tile on the bottom
                                    //{
                                    //    currPlatform = tile.Rectangle;
                                    //    onPlatform = true;
                                    //}
                                    if (possibleJumpLocations.Contains(tile.Rectangle) == false && enemyRect.TouchTopOf(tile.Rectangle) == false && (int)(enemyRect.Y/64) >= tile.Rectangle.Y/64
                                        && SideTileMap.PlatformIndexes.Contains(SideTileMap.GetPoint((tile.Rectangle.Y/64) - 1, tile.Rectangle.X/64)) == false &&
                                        SideTileMap.GroundIndexes.Contains(SideTileMap.GetPoint((tile.Rectangle.Y / 64) - 1, tile.Rectangle.X / 64)) == false)
                                    {
                                        possibleJumpLocations.Add(tile.Rectangle);

                                        vision.Remove(vision[i]);
                                    }
                                       
                                }
                            }
                        }

                       
                        
                        {
                            if(possibleJumpLocations.Count != 0) //Determine which of the possible locations is best
                            {
                                goalRect = possibleJumpLocations[0];
                                //Rectangle closestToPlayer = possibleJumpLocations[0]; in the code using this it grabs the wrong tile?
                                //foreach(Rectangle rectangle in possibleJumpLocations)
                                //{
                                //    if (Distance(new Vector2(closestToPlayer.X, closestToPlayer.Y), player.Position) < Distance(new Vector2(rectangle.X, rectangle.Y), player.Position)) ;
                                //}
                                foreach (Rectangle rect in possibleJumpLocations)
                                {
                                    if (MathHelper.Distance(goalRect.Y, position.Y) > MathHelper.Distance(rect.Y, position.Y))
                                    {
                                        goalRect = rect; //If the location is the closest on the Y, prioritize as the best
                                    }
                                    else if (MathHelper.Distance(goalRect.Y, position.Y) == MathHelper.Distance(rect.Y, position.Y))
                                    {
                                        //If the locations are even of the same Y 

                                        //if (MathHelper.Distance(goalRect.X, position.X) > MathHelper.Distance(rect.X, position.X))
                                        //{
                                        //    goalRect = rect;
                                        //    //if (closestToPlayer.Y == goalRect.Y)
                                        //    //{
                                        //    //    if(MathHelper.Distance(closestToPlayer.X, position.X) < MathHelper.Distance(goalRect.X, position.X))
                                        //    //    {
                                        //    //        goalRect = closestToPlayer;
                                        //    //    }
                                        //    //}
                                        //}

                                        //This is closest to player, it gets weird
                                        //if (MathHelper.Distance(goalRect.X, player.Position.X) > MathHelper.Distance(rect.X, player.Position.X)) //If closest to player 
                                        //{
                                        //    goalRect = rect;
                                        //    //Save second best in case this doesnt work?...Saved the closest to player (works better than second best?)
                                        //}
                                        if (MathHelper.Distance(goalRect.X + goalRect.Width/2, position.X + enemyRect.Width/2) > MathHelper.Distance(rect.X + rect.Width/2, position.X + enemyRect.Width/2)) //If closest to player 
                                        {
                                            goalRect = rect;
                                            //Save second best in case this doesnt work?...Saved the closest to player (works better than second best?)
                                        }
                                        //else
                                        //{
                                        //    if (MathHelper.Distance(goalRect.X, position.X) > MathHelper.Distance(rect.X, position.X))
                                        //    {
                                        //        goalRect = rect;
                                        //    }
                                        //}
                                    }
                                    //else if (Distance(new Vector2(goalRect.X, goalRect.Y), position) > Distance(new Vector2(rect.X, rect.Y), position))
                                    //{
                                    //    goalRect = rect; //If all else fails, whichever is the closest in general
                                    //}
                                }

                                Vector2 tempVel = Velocity; //Correct velocity to go in direction of the goalRect
                                if (position.X < goalRect.X)
                                {
                                    tempVel.X = (int)maxRunSpeed;

                                }
                                if (position.X > goalRect.X)
                                {
                                    tempVel.X = (int)-maxRunSpeed;
                                }
                                float randForce = maxJumpForce;
                                int i = TestJump(goalRect, randForce, position, tempVel.X, false); //Test jump at max values
                                //Success would == 1
                                int jumpMin = 10;
                                int speedMin = 3;

                                while (i != 1)//Goes through every possible jump 
                                {
                                    for (int j = jumpMin; j < 26; j++)
                                    {
                                        if (tempVel.X > 0)
                                        {
                                            for (int k = speedMin; k < maxRunSpeed + 10; k++)
                                            {
                                                i = TestJump(goalRect, j, position, k, false);

                                                if (i == 1)
                                                {
                                                    tempVel.X = k;
                                                    break;
                                                }

                                            }
                                        }
                                        if (tempVel.X < 0)
                                        {
                                            for (int k = -speedMin; k > -maxRunSpeed - 10; k--)
                                            {
                                                i = TestJump(goalRect, j, position, k, false);
                                                if (i == 1)
                                                {
                                                    tempVel.X = k;
                                                    break;
                                                }
                                            }
                                        }
                                        if (i == 1)
                                        {
                                            randForce = j;
                                            break;
                                        }

                                    }
                                    if (i == 2) //Failed every jump
                                    {
                                        break;
                                    }
                                }

                                if (i == 2) //Cant jump conditions
                                {
                                    if (leftOnX.Count != 0) //If there is a leftOnX
                                    {
                                        //Set to target and foce it to goTo 
                                        TargetPos = new Vector2(leftOnX[0], position.Y);
                                        leftOnX.RemoveAt(0);
                                        goTo = true;
                                        enemyState = EnemyStates.GoTo;
                                        goTo = true;
                                        maxRunSpeed *= 1.75f;
                                        //bounds = SideTileMap.GetNumTilesOfGround((int)(position.Y/64) + 1, (int)position.X/64); //Recalc bounds 
                                        //if(bounds != Vector2.Zero)
                                        //{
                                        //    bounds *= 64;
                                        //}
                                        //landingPos = position;

                                        enemyState = EnemyStates.GoTo;
                                    }
                                    else if(leftOnX.Count == 0 && velocity.X > 0 && bounds.Y > 0 || leftOnX.Count == 0 && velocity.X < 0 && bounds.X > 0)
                                    {
                                        if(velocity.X > 0)
                                        {
                                            TargetPos = new Vector2(enemyRect.X + bounds.Y, enemyRect.Y);
                                        }
                                        if (velocity.X < 0)
                                        {
                                            TargetPos = new Vector2(enemyRect.X - bounds.X, enemyRect.Y);
                                        }
                                        goTo = true;
                                        enemyState = EnemyStates.GoTo;
                                    }
                                    else
                                    {
                                        enemyState = EnemyStates.Idle;
                                        cantReach = true;
                                    }

                                    //else if(position.X > ((goalRect.X + goalRect.Width) + goalRect.Width/2))
                                    //{
                                    //    velocity.X = -maxRunSpeed;
                                    //    forceRun = true;
                                    //}
                                    //else if(position.X < goalRect.Left)
                                    //{
                                    //    velocity.X = Math.Abs(maxAirSpeed);
                                    //    forceRun = true;
                                    //}
                                    //else
                                    //{
                                    //    goTo = true;
                                    //    if(RandFloat(0,1) > .5f)
                                    //    {
                                    //        velocity.X = -maxRunSpeed;
                                    //    }
                                    //    else
                                    //    {
                                    //        velocity.X = maxRunSpeed;
                                    //    }
                                    //}

                                    //if (blockRight && velocity.X > 0)
                                    //{
                                    //    velocity.X = -maxRunSpeed;
                                    //    enemyState = EnemyStates.GoTo;
                                    //    //goTo = true;
                                    //}
                                    //else if (blockLeft)
                                    //{
                                    //    velocity.X = maxRunSpeed;
                                    //    enemyState = EnemyStates.GoTo;
                                    //    //goTo = true;
                                    //}
                                    //else
                                    //{
                                    //    enemyState = EnemyStates.Idle;
                                    //    cantReach = true;
                                    //}
                                    //else if (RandFloat(0, 1) > .5f)
                                    //{
                                    //    velocity.X = maxRunSpeed;
                                    //}
                                    //else
                                    //{
                                    //    velocity.X = -maxRunSpeed;
                                    //}
                                    //goTo = true; 
                                    //enemyState = EnemyStates.GoTo;
                                    //velocity.X = 0;
                                    //if (position.X < player.Position.X)
                                    //{
                                    //    //SetTarget to the left most position of the current bounds

                                    //    //if(position.x == bounds.x)
                                    //    //{
                                    //    //Can't reach...change logic
                                    //    //}
                                    //}
                                    //if (position.X > player.Position.X)
                                    //{
                                    //    //SetTarget to right most position of current bounds

                                    //    //if(position.x == bounds.x + width)
                                    //    //{
                                    //    //Can't reach...change logic
                                    //    //}
                                    //}


                                }
                                if (i == 1)  //Success
                                {
                                    velocity = new Vector2(tempVel.X, -randForce); //Set velocity to the success values 
                                    TargetPos = new Vector2((goalRect.X + goalRect.Width / 2) - (enemyRect.Width / 2), goalRect.Top - enemyRect.Height); //Target pos is the center of the goalRect
                                    prevState = EnemyStates.Jumping;
                                    goTo = true;
                                    isFalling = true;
                                }
                            }
                            else
                            {
                                if(leftOnX.Count != 0)//Set the target to where the player left the y Axis
                                {
                                    TargetPos = new Vector2(leftOnX[0], position.Y);
                                    leftOnX.Clear();
                                    goTo = true;
                                }
                                enemyState = EnemyStates.GoTo;
                            }
                           
                        }
                    }
                    else if(prevState == EnemyStates.Jumping) //If the player is currently jumping
                    {
                        foreach (PlatformTile tile in SideTileMap.PlatformTiles)
                        {
                            Collision(tile.Rectangle);
                        }
                        enemyState = EnemyStates.GoTo; //Keep jumpinh
                        if (velocity.Y == 0 && blockBottom)
                        {
                            prevState = EnemyStates.Idle; //Reset prevState to allow to jump again
                        }

                    }
                    break;
                #endregion
                #region Attacking
                case EnemyStates.Attacking:
                    if(isShoot) //Is a shooting type enemy
                    {
                        if(attackLeft)
                        {
                            if(shootDelay <= 0)  //If can shooot shoot to the left
                            {
                                bullets.Add(new Bullet(new Vector2(enemyRect.Left, enemyRect.Y + enemyRect.Height / 2), -bulletSpeed, new Vector2(-maxBulletSpeed.X, maxBulletSpeed.Y), content, true, bulletTravelDist));
                                shootDelay = maxShootDelay;
                            }
                                
                        }
                        if(attackRight) 
                        {
                            if(shootDelay <= 0)//If delay is over, shoot right
                            {
                                bullets.Add(new Bullet(new Vector2(enemyRect.Right, enemyRect.Y + enemyRect.Height / 2), bulletSpeed, maxBulletSpeed, content, true, bulletTravelDist));
                                shootDelay = maxShootDelay;
                            }
                            
                        }
                       
                    }
                    else
                    {
                        if (attackDelay <= 0)//Melee attack
                        {
                            //Attack if in hitbox or touching the enemy itself
                            if (player.playerRect.Intersects(HitBox) || player.playerRect.Intersects(enemyRect))
                            {
                                player.Health -= meleeDmg;
                                player.playerState = SSPlayer.PlayerStates.Knockback;
                                if (enemyRect.Center.X < player.playerRect.Center.X)
                                    player.knockBackX = 5;
                                else
                                    player.knockBackX = -5;
                                player.knockBackY = -3;
                                player.playerRect.Y -= 2;
                                player.position.Y -= 2;
                            }

                            attackDelay = attackDelayMax;
                        }
                    }

                    if (MathHelper.Distance(enemyRect.Center.X, player.playerRect.Center.X) > attackOffsetFromPlayer)
                    {
                        prevState = EnemyStates.Attacking;
                        enemyState = EnemyStates.GoTo;
                    }


                    break;
                #endregion
                #region Falling
                case EnemyStates.Falling:
                    if (prevState != EnemyStates.Falling /*|| player.playerRect.Bottom < enemyRect.Top && player.velocity.Y >= 0 && onPlatform*/)
                    {
                        possibleJumpLocations.Clear();
                        //Grab platform tiles
                        foreach (PlatformTile tile in SideTileMap.PlatformTiles)
                        {
                            for (int k = vision.Count - 1; k >= 0; k--)
                            {
                                if (vision[k].Intersects(tile.Rectangle))
                                {
                                    
                                    if (possibleJumpLocations.Contains(tile.Rectangle) == false && enemyRect.TouchTopOf(tile.Rectangle) == false && tile.Rectangle.Y/64 > (enemyRect.Y/64) + 1)
                                    {
                                        possibleJumpLocations.Add(tile.Rectangle);

                                        vision.Remove(vision[k]);
                                    }
                                       

                                }
                            }
                        }

                        if (possibleJumpLocations.Count != 0) //Determine which of the possible locations is best
                        {
                            goalRect = possibleJumpLocations[0];
                            foreach (Rectangle rect in possibleJumpLocations)
                            {
                                if (MathHelper.Distance(goalRect.Y, position.Y) > MathHelper.Distance(rect.Y, position.Y) && rect.Y >= position.Y)
                                {
                                    goalRect = rect; //If the location is the closest on the Y, prioritize as the best
                                }
                                else if (MathHelper.Distance(goalRect.Y, position.Y) == MathHelper.Distance(rect.Y, position.Y) && rect.Y >= position.Y)
                                {
                                    //This is closest to player, it gets weird
                                    //if (MathHelper.Distance(goalRect.X, player.Position.X) > MathHelper.Distance(rect.X, player.Position.X)) //If closest to player 
                                    //{
                                    //    goalRect = rect;
                                    //    //Save second best in case this doesnt work?...Saved the closest to player (works better than second best?)
                                    //}
                                    if (MathHelper.Distance(goalRect.X, position.X) > MathHelper.Distance(rect.X, position.X)) //If closest to player 
                                    {
                                        goalRect = rect;
                                        //Save second best in case this doesnt work?...Saved the closest to player (works better than second best?)
                                    }
                                }
                            }
                        }

                        Vector2 tempVel = Velocity; //Correct velocity to go in direction of the goalRect
                        if (position.X < goalRect.X)
                        {
                            tempVel.X = (int)maxRunSpeed;

                        }
                        if (position.X > goalRect.X)
                        {
                            tempVel.X = (int)-maxRunSpeed;
                        }
                        float randForce = 0;
                        int i = TestFall(goalRect, randForce, position, tempVel.X); //Test jump at max values
                                                                                           //Success would == 1
                        int jumpMin = 10;
                        int speedMin = 1;

                        while(i != 1)
                        {
                            for (int j = jumpMin; j < maxJumpForce; j++)
                            {
                                if (tempVel.X > 0)
                                {
                                    for (int k = speedMin; k < maxRunSpeed; k++)
                                    {
                                        i = TestFall(goalRect, j, position, k);

                                        if (i == 1)
                                        {
                                            tempVel.X = k;
                                            break;
                                        }

                                    }
                                }
                                if (tempVel.X < 0)
                                {
                                    for (int k = -speedMin; k > -maxRunSpeed; k--)
                                    {
                                        i = TestFall(goalRect, j, position, k);
                                        if (i == 1)
                                        {
                                            tempVel.X = k;
                                            break;
                                        }
                                    }
                                }
                                if (i == 1)
                                {
                                    randForce = j;
                                    break;
                                }

                            }
                            if (i == 2) //Failed every jump
                            {
                                velocity.X = 0;
                                break;
                            }
                          
                        }
                        if (i == 1)  //Success
                        {
                            velocity = new Vector2(tempVel.X, -randForce); //Set velocity to the success values 
                            TargetPos = new Vector2((goalRect.X + goalRect.Width / 2) - (enemyRect.Width / 2), goalRect.Top - enemyRect.Height); //Target pos is the center of the goalRect
                            prevState = EnemyStates.Falling;
                            goTo = true;
                            //isFalling = true;
                        }
                        if(i == 2)
                        {
                            enemyState = EnemyStates.GoTo;
                        }

                    }
                    else if(prevState == EnemyStates.Falling)
                    {
                        enemyState = EnemyStates.GoTo; //Keep jumpinh
                        if (velocity.Y == 0 && blockBottom)
                        {
                            prevState = EnemyStates.Idle; //Reset prevState to allow to jump again
                        }
                    }
                        break;
                #endregion
                #region Knockback
                case EnemyStates.Knockback:
                    
                    if (player.killEnemy)
                    {
                        if (player.playerRect.Center.X < enemyRect.Center.X)
                        {
                            velocity = new Vector2(knockbackForce.X, knockbackForce.Y);
                            enemyRect.Y -= 2;
                            position.Y -= 2;

                            gravX = -1;
                        }
                        else
                        {
                            velocity = new Vector2(-knockbackForce.X, knockbackForce.Y);
                            enemyRect.Y -= 2;
                            position.Y -= 2;
                            gravX = 1;
                        }
                    }
                    else if(gravX == 0)
                    {

                        if (player.playerRect.Center.X < enemyRect.Center.X)
                        {
                            velocity = new Vector2(knockBackX, knockBackY);
                            enemyRect.Y -= 2;
                            position.Y -= 2;

                            gravX = -1;
                        }
                        else
                        {
                            velocity = new Vector2(-knockBackX, knockBackY);
                            enemyRect.Y -= 2;
                            position.Y -= 2;
                            gravX = 1;
                        }
                    }

                    //if (gravX == 0)
                    //{
                    //    if (player.playerRect.Center.X < enemyRect.Center.X)
                    //    {
                    //        velocity = new Vector2(knockbackForce.X / 2, knockbackForce.Y);
                    //        enemyRect.Y -= 2;
                    //        position.Y -= 2;

                    //        gravX = -1;
                    //    }
                    //    else
                    //    {
                    //        velocity = new Vector2(-knockbackForce.X / 2, knockbackForce.Y);
                    //        enemyRect.Y -= 2;
                    //        position.Y -= 2;
                    //        gravX = 1;
                    //    }
                    //}

                    player.killEnemy = false;
                    foreach(GroundTile tile in SideTileMap.GroundTiles)
                    {
                        if(enemyRect.TouchTopOf(tile.Rectangle))
                        {
                            enemyState = EnemyStates.GoTo;
                            gravX = 0;
                        }
                    }
                    foreach(WallTile tile in SideTileMap.WallTiles)
                    {
                        if (enemyRect.Intersects(tile.Rectangle))
                        {
                            enemyState = EnemyStates.GoTo;
                            gravX = 0;
                        }
                    }
                    foreach(PlatformTile tile in SideTileMap.PlatformTiles)
                    {
                        if (enemyRect.Intersects(tile.Rectangle))
                        {
                            enemyState = EnemyStates.GoTo;
                            gravX = 0;
                        }
                    }

                    if(!damaged)
                    {
                        enemyState = EnemyStates.GoTo;
                        gravX = 0;
                    }


                    break;
                #endregion
            }

            if (isFalling) //Only add gravity if falling
            {
                velocity.Y += gravity.Y;
                velocity.X += gravX;
            }
            
            if(velocity.X > 0 && velocity.X > maxRunSpeed && enemyState != EnemyStates.Knockback)//Going right and velocity checks
            {
                velocity.X = maxRunSpeed;
            }
            if(velocity.X < 0 && velocity.X < -maxRunSpeed && enemyState != EnemyStates.Knockback)//Going left and velocity checks
            {
                velocity.X = -maxRunSpeed;
            }
            if (velocity.Y > terminalVel) //max fallSpeed check
                velocity.Y = terminalVel;

            if(MathHelper.Distance(enemyRect.X, player.Position.X) < player.playerRect.Width)
            {
                //game.TakeDamage();
                //if(velocity.X > 0)
                //{
                //    position.X += enemyRect.Width * 2;
                //    player.velocity.X -= maxRunSpeed;
                //}
                //if(velocity.X < 0)
                //{
                //    position.X -= enemyRect.Width * 2;
                //    player.velocity.X += maxRunSpeed;
                //}
                
            }

            position += velocity; //Move

            if(isShoot)
            {
                enemyRect = new Rectangle((int)position.X, (int)position.Y, 48, 48);
                animManager.Update(gameTime, new Vector2(position.X - enemyRect.Width/4.5f, position.Y - enemyRect.Height / 3));
            }
            else
            {
                enemyRect = new Rectangle((int)position.X, (int)position.Y, 20, 48);
                animManager.Update(gameTime, new Vector2(position.X - enemyRect.Width, position.Y - enemyRect.Height / 3));
            }
           

            if(isShoot) //Update bullets
            {
                if(bullets.Count != 0)
                {
                    for (int i = bullets.Count - 1; i >= 0; i--)
                    {
                        bullets[i].Update();
                        if(bullets[i].rect.Intersects(player.playerRect))
                        {
                            player.Health -= bulletDmg;
                            player.playerState = SSPlayer.PlayerStates.Knockback;
                            if (enemyRect.Center.X < player.playerRect.Center.X)
                                player.knockBackX = 1;
                            else
                                player.knockBackX = -1;
                            player.knockBackY = -1;
                            player.playerRect.Y -= 2;
                            player.position.Y -= 2;
                            bullets.RemoveAt(i);
                        }
                        else if(bullets[i].delete)
                        {
                            bullets.RemoveAt(i);
                        }
                    }
                }
                
            }
            if(velocity.X < 0)
            {
                animManager.isRight = true;
                animManager.isLeft = false;
            }
            else if(velocity.X > 0)
            {
                
                animManager.isLeft = true;
                animManager.isRight = false;
            }
            
       
            

        }

        private void TileCollision(GroundTile tile)
        {
            Collision(tile.Rectangle);

            if(tile.Rectangle.Top < enemyRect.Top)
            {
                //for (int i = vision.Count - 1; i >= 0; i--)
                //{
                //    if (vision[i].Intersects(tile.Rectangle))
                //    {
                //        if (i == 0)
                //        {
                //            blockTop = true;
                //        }
                //        if (i == vision.Count - 1)
                //        {
                //            currPlatform = tile.Rectangle;
                //            onPlatform = true;
                //        }
                //        vision.RemoveAt(i);
                //        //j++;
                //        //bool test1 = possibleJumpLocations.Contains(tile.Rectangle);
                //        //bool test2 = player.playerRect.TouchTopOf(tile.Rectangle);
                //        if (!possibleJumpLocations.Contains(tile.Rectangle) && enemyRect.TouchTopOf(tile.Rectangle) == false)
                //            possibleJumpLocations.Add(tile.Rectangle);

                //    }
                //}
            }
           

        }

        public void Draw(SpriteBatch spriteBatch)
        {


            //tRect.X += tRect.Width/5;
            if (damaged)
            {
                if (redCount <= whiteCount || redCount == 0 && whiteCount == 0)
                {
                    animManager.Draw(spriteBatch, Color.White);
                    redCount += 3;
                }
                if (whiteCount < redCount)
                {
                    animManager.Draw(spriteBatch, Color.Red);
                    whiteCount++;
                }
                if (whiteCount == whiteFrames)
                {
                    damaged = false;
                    whiteCount = 0;
                    redCount = 0;
                }
            }
            else
            {
                animManager.Draw(spriteBatch, Color.White);
            }

            //spriteBatch.Draw(texture, enemyRect, Color.White);
            //spriteBatch.Draw(texture, HitBox, Color.BlueViolet);
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }

            //animManager.Draw(spriteBatch, Color.White);
            //foreach (Rectangle rect in vision)
            //{
            //    spriteBatch.Draw(visionTxture, rect, Color.White * .25f);


            //}
            //animManager.Draw(spriteBatch);
        }

        public void Collision(Rectangle newRect)
        {
           
            if(enemyRect.TouchTopOf(newRect)) //Touch Ground
            {
                blockBottom = true;

                if(isFalling)
                {
                    while (enemyRect.Bottom > newRect.Top)
                    {
                        position.Y -= 1f;
                        enemyRect.Y = (int)position.Y;
                    }
                    velocity.Y = 0;
                    isFalling = false;
                    bounds = SideTileMap.GetNumTilesOfGround(newRect.Y / 64, newRect.X / 64); //How many tiles are available left and right
                    jumpDelay = 0;
                    maxRunSpeed = 3.75f;
                    // if(bounds = new Vector2(0,0))
                    if (bounds == Vector2.Zero)
                    {
                        bounds = new Vector2(64 - enemyRect.Width, 64 - enemyRect.Width);
                    }
                    else
                    {
                        bounds *= 64; //Calculate distance travled from this number to insure bound correction   
                    }
                  
                    landingPos = position;
                }
                if(enemyState == EnemyStates.Jumping)
                {
                    //Dont check if jumping
                }
                else
                {
                    while (enemyRect.Bottom > newRect.Top)//Keep enemyRect on the ground
                    {
                        position.Y -= 1f;
                        enemyRect.Y = (int)position.Y;
                        
                    }
                    //jumpDelay++;
                    //if(jumpDelay > maxJumpDelay)
                    //{

                    //}
                    //if(velocity.X > 0)
                    //{
                    //    canWalk = SideTileMap.CanWalk(newRect.Y / 64, newRect.X / 64, "right");
                    //}
                    //if(velocity.X < 0)
                    //{
                    //    canWalk = SideTileMap.CanWalk(newRect.Y / 64, newRect.X / 64, "right");
                    //}
                }

               
            }

            if(enemyRect.TouchLeftOf(newRect))//enemy is colliding to the right
            {
                blockRight = true;
                
                while(enemyRect.Right > newRect.Left)
                {
                    position.X -= 1f;
                    enemyRect.X = (int)position.X;
                }

                position.X += -Velocity.X;
                enemyRect.X = (int)position.X;
            }

            if(enemyRect.TouchRightOf(newRect))//enemy is Colliding to the left
            {
                blockLeft = true;

                while(enemyRect.Left < newRect.Right)
                {
                    position.X += 1;
                    enemyRect.X = (int)position.X;
                }

                position.X += -Velocity.X;
                enemyRect.X = (int)position.X;
            }

            if(enemyRect.TouchBottomOf(newRect)) //Colliding Top or touching bottom of tile
            {
                while(enemyRect.Top < newRect.Bottom)
                {
                    position.Y += 1;
                    enemyRect.Y = (int)position.Y;
                }

                if(velocity.Y < 0)
                {
                    velocity.Y = 0;
                }
                if(!isFalling)
                {
                    isFalling = true;
                }
            }
        }

        Vector2 TestCollision(Vector2 startPos, Rectangle tile, Vector2 goalPos, Vector2 tempVel, bool isPlatforms) //Same as collisions but restructured for the TestJump()
        {
            Rectangle eRect = new Rectangle((int)startPos.X, (int)startPos.Y, enemyRect.Width, enemyRect.Height);
            if(eRect.TouchTopOf(tile))
            {
                //If touching the top of a tile, check if it is the goalRect
                if(tempVel.Y > 0)
                {
                    if (startPos.X == goalPos.X && isPlatforms || goalRect == tile && startPos.Y + enemyRect.Height <= tile.Top)
                    {
                        jumpSuccess = true;
                    }
                }
                else if(tempVel.Y < 0) //Simulation of while jumping dont collide 
                {

                }
               
            }

            if(eRect.TouchLeftOf(tile))
            {
                while (eRect.Right > tile.Left)
                {
                    eRect.X -= 1;
                  
                }

                eRect.X += (int)-tempVel.X;
               
            }

            if(eRect.TouchRightOf(tile))
            {
                while (eRect.Left < tile.Right)
                {
                    eRect.X += 1;

                }

                eRect.X += (int)-tempVel.X;
            }

            if(eRect.TouchBottomOf(tile))
            {
                while(eRect.Top < tile.Bottom)
                {
                    eRect.Y += 1;
                }
                if(tempVel.Y < 0)
                    tempVel.Y = 0;
                if(isPlatforms)
                {
                    if (tile == goalRect)
                        jumpFail = true;
                }
            }

            return new Vector2(eRect.X, eRect.Y);
        }

        public int TestFall(Rectangle goalRect, float jumpForce, Vector2 startPos, float moveSpeedX)
        {
            jumpFail = false;
            jumpSuccess = false;
            Vector2 goalPos = new Vector2((goalRect.X + goalRect.Width / 2) - (enemyRect.Width / 2), goalRect.Top - enemyRect.Height);//Set to the center of the goalRect
            int num = 0;
            Vector2 velocity = new Vector2(moveSpeedX, -jumpForce);

            while (num == 0) //Simulation of Update, but breaks when results occur
            {

                //Determine if going left or right
                //If the jump will go past the x, it will stop 
                if (startPos.X < goalPos.X)
                {
                    velocity.X += moveSpeed;
                    if (velocity.X > maxRunSpeed)
                    {
                        velocity.X = maxRunSpeed;
                    }

                    if (startPos.X + velocity.X > goalPos.X) //Reached x
                    {
                        startPos.X = goalPos.X;
                        velocity.X = 0;
                    }

                }
                if (startPos.X > goalPos.X)
                {
                    velocity.X -= moveSpeed;
                    if (velocity.X < -maxRunSpeed)
                    {
                        velocity.X = -maxRunSpeed;
                    }

                    if (startPos.X + velocity.X < goalPos.X)//Reached x
                    {
                        startPos.X = goalPos.X;
                        velocity.X = 0;
                    }
                }

                velocity.Y += gravity.Y; //Gravity
                if (velocity.Y > terminalVel)
                {
                    velocity.Y = terminalVel;
                }

                if (velocity.Y == terminalVel && startPos.Y > goalRect.Bottom) //If going down and is below the goalRect 
                {
                    num = 2;
                    break;
                }
                startPos += velocity;

                foreach (GroundTile tile in SideTileMap.GroundTiles)
                {
                    startPos = TestCollision(startPos, tile.Rectangle, goalPos, velocity, false);
                    if (jumpFail)//Touched the ground and wasnt the goalRect
                    {
                        num = 2;
                    }
                }
                foreach (PlatformTile tile1 in SideTileMap.PlatformTiles)
                {
                    startPos = TestCollision(startPos, tile1.Rectangle, goalPos, velocity, true);
                    if (jumpSuccess)//Success
                        num = 1;
                    if (jumpFail)//Missed goal
                        num = 2;
                }
            }
            return num;
        }
    

        public int TestJump(Rectangle goalRect, float jumpForce, Vector2 startPos, float moveSpeedX, bool failTop)
        {
            jumpFail = false;
            jumpSuccess = false;
            Vector2 goalPos = new Vector2((goalRect.X + goalRect.Width / 2) - (enemyRect.Width/2), goalRect.Top - enemyRect.Height);//Set to the center of the goalRect
            startPos.Y -= 1;
            int num = 0;
            Vector2 velocity = new Vector2(moveSpeedX, -jumpForce);
            while (num == 0) //Simulation of Update, but breaks when results occur
            {

                //Determine if going left or right
                //If the jump will go past the x, it will stop 
                if (startPos.X < goalPos.X)
                {
                    velocity.X += moveSpeed;
                    if (velocity.X > maxRunSpeed)
                    {
                        velocity.X = maxRunSpeed;
                    }

                    if (startPos.X + velocity.X > goalPos.X) //Reached x
                    {
                        startPos.X = goalPos.X;
                        velocity.X = 0;
                    }

                }
                if (startPos.X > goalPos.X)
                {
                    velocity.X -= moveSpeed;
                    if (velocity.X < -maxRunSpeed)
                    {
                        velocity.X = -maxRunSpeed;
                    }

                    if (startPos.X + velocity.X < goalPos.X)//Reached x
                    {
                        startPos.X = goalPos.X;
                        velocity.X = 0;
                    }
                }

                velocity.Y += gravity.Y; //Gravity
                if(velocity.Y > terminalVel)
                {
                    velocity.Y = terminalVel;
                }

                if(velocity.Y == terminalVel && startPos.Y + enemyRect.Height > goalRect.Top) //If going down and is below the goalRect 
                {
                    num = 2;
                }
                startPos += velocity;

                foreach(GroundTile tile in SideTileMap.GroundTiles)
                {
                    startPos = TestCollision(startPos, tile.Rectangle, goalPos, velocity, false);
                    if(jumpFail)//Touched the ground and wasnt the goalRect
                    {
                        num = 2;
                    }
                }
                foreach(PlatformTile tile1 in SideTileMap.PlatformTiles)
                {
                    startPos = TestCollision(startPos, tile1.Rectangle, goalPos, velocity, true);
                    if (jumpSuccess)//Success
                        num = 1;
                    if (jumpFail)//Missed goal
                        num = 2;
                }
            }
            return num;
        }

        public float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }

        public float RandFloat(int min, int max)
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(min, max).ToString();//number before decimal point
            string afterPoint = r.Next(0, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }
        public float RandNegFloat(int min, int max)
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(min, max).ToString();//number before decimal point
            string afterPoint = r.Next(0, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = -float.Parse(combined);
        }

    }

    struct JumpChromosome
    {
        public Vector2 startPos;
        public float jumpForce;
    }
}
