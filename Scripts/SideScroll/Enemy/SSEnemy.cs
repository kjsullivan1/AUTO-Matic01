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

        public enum EnemyStates { Idle, GoTo, Attacking,Jumping}
        public EnemyStates enemyState = EnemyStates.Idle;
        EnemyStates prevState;

        #region Fields
        Vector2 bounds;
        int pixelSize = 64;
        Vector2 position = new Vector2(64 * 15 + 5, 0);
        public Rectangle enemyRect;
        bool blockBottom = false;
        bool isFalling = true;
        bool isColliding = false;
        Vector2 TargetPos;
        public float health = 3;
        public bool dead = false;
        public float Health
        {
            get { return health; }
            set { health = value; 
                if(health <= 0)
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
        Vector2 gravity;

        public float maxRunSpeed = 4f;
        float terminalVel = 12f;
        float maxJumpSpeed = 8f;
        int maxJumpForce = 20;
        int minJumpForce = 16;
        float maxAirSpeed;
        public bool isShoot;
        #endregion

        #region Velocity
        public Vector2 velocity = Vector2.Zero;
        Vector2 positionOffset = new Vector2(0, 0);
        public Vector2 Velocity
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
                //case AnimationStates.Walking:
                //    texture = content.Load<Texture2D>("SideScroll/Animations/PlayerWalk");
                //    FrameSize = new Point(64, 64);
                //    CurrFrame = new Point(0, 0);
                //    SheetSize = new Point(8, 1);
                //    fpms = 120;
                //    break;
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
            enemyRect = new Rectangle((int)position.X, (int)position.Y, 48, 48);
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
        }
        #endregion

        #region AI Helpers
        public List<Rectangle> vision = new List<Rectangle>();
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
        List<int> leftOnX = new List<int>();
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
        float attackDelay = 1.5f;
        float attackDelayMax;
        bool attackLeft = false, attackRight = false;
        int attackBoxWidth = 40, attackBoxHeight = 40;
        float meleeDmg = 1.5f;
        bool outOfRange = false;
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

            for(int i = 1; i < visionLength + 1; i++)
            {
                vision.Add(new Rectangle((int)pos.X, (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));//Up
            }
            //vision.Add(new Rectangle((int)pos.X, (int)pos.Y - (pixelSize * 1), pixelSize, pixelSize));//Up
            for (int i = 1; i < visionLength + 1; i++)//Right
            {
                vision.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y, pixelSize, pixelSize));
                for(int j = i + 1; j < visionLength + 1; j++) //Right and Up
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y - (pixelSize * j), pixelSize, pixelSize));
                }
                //for(int k = i + 1; k < visionLength + 1; k++)//Right and down
                //{
                //    vision.Add(new Rectangle(enemyRect.X + (enemyRect.Width * i), enemyRect.Top + (enemyRect.Height * k), pixelSize, pixelSize));
                //}
            }

            for (int i = 1; i < visionLength + 1; i++)//Left
            {
                vision.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y, pixelSize, pixelSize));
                for (int j = i + 1; j < visionLength + 1; j++) //Left and Up
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y - (pixelSize * j), pixelSize, pixelSize));
                }
                //for (int k = i + 1; k < visionLength + 1; k++)//Left and down
                //{
                //    vision.Add(new Rectangle(enemyRect.X - (enemyRect.Width * i), enemyRect.Top + (enemyRect.Height * k), pixelSize, pixelSize));
                //}
            }

            for(int i = 1; i < visionLength + 1; i++) //Up and down
            {
               
                for (int j = i; j < visionLength + 1; j++) //Left and up 
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * j), (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));
                }
                for (int j = i; j < visionLength + 1; j++) //Right and Up
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * j), (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));
                }

                if(enemyState != EnemyStates.Jumping && prevState != EnemyStates.Jumping)
                {
                    for (int k = i; k < visionLength + 1; k++)//Left and down
                    {
                        vision.Add(new Rectangle((int)pos.X - (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                    }
                    for (int k = i; k < visionLength + 1; k++)//Right and down
                    {
                        vision.Add(new Rectangle((int)pos.X + (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                    }
                }
               

            }
            //vision.Add(new Rectangle(enemyRect.X + (int)(enemyRect.Width/3.5f), enemyRect.Top + (enemyRect.Height * 1), enemyRect.Width/2, pixelSize/4));
            #endregion
        }


        void GoTo(Vector2 bounds)//bounds is the min and max bounds in the x direction left to right
        { //if + movespeed > Target velocity.x = 0
            switch(enemyState)
            {
                case EnemyStates.GoTo:
                    if (position.X < TargetPos.X)
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
                        else if (MathHelper.Distance(position.X, landingPos.X) >= bounds.Y && prevState != EnemyStates.Jumping && goTo )
                        {
                            velocity.X = 0;
                            position.X += 1;
                            enemyRect.X = (int)position.X;
                        }
                        else
                        {
                            if(velocity.X <= 0)
                            {
                                velocity.X = -velocity.X;
                            }
                            velocity.X += moveSpeed;
                        }
                    }
                    if (position.X > TargetPos.X)
                    {
                        if ((int)position.X - (int)velocity.X < (int)TargetPos.X)
                        {
                            velocity.X = 0;
                            if(prevState == EnemyStates.Jumping)
                            {
                                position.X = TargetPos.X;
                                goTo = false;
                                onPlatform = true;
                                prevState = EnemyStates.Idle;
                            }
                     
                            //change enemyState
                        }
                        else if (MathHelper.Distance(position.X, landingPos.X) >= bounds.X && prevState != EnemyStates.Jumping && goTo)
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
                    if(position.ToPoint() == TargetPos.ToPoint())
                    {
                        goTo = false;
                    }
                   
                    if(MathHelper.Distance(position.X, landingPos.X) > bounds.X && velocity.X < 0 && prevState != EnemyStates.Jumping && TargetPos.Y > enemyRect.Bottom == false && !isFalling)
                    {
                        position.X += Math.Abs(velocity.X);
                    }
                    if (MathHelper.Distance(position.X, landingPos.X) > bounds.Y && velocity.X > 0 && prevState != EnemyStates.Jumping && TargetPos.Y > enemyRect.Bottom == false && !isFalling)
                    {
                        position.X += -velocity.X;
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

        public void Update(GameTime gameTime, Vector2 gravity, SSPlayer player, Game1 game)
        {
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

            if(dead)
            {
                position = new Vector2(-100, 10000);
            }

            foreach(GroundTile tile in SideTileMap.GroundTiles)
            {
                if (enemyRect.TouchTopOf(tile.Rectangle))
                    onPlatform = false;
                Collision(tile.Rectangle);
            }

            if(onPlatform || velocity.Y != 0)
            {
                foreach(PlatformTile tile in SideTileMap.PlatformTiles)
                {
                    Collision(tile.Rectangle);
                }
            }
            

            //Collision
            if(blockBottom)
            {
                isFalling = false;
            }
            else
            {
                isFalling = true;
            }

            if(prevState == EnemyStates.Attacking)
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
                    foreach (Rectangle rect in vision)
                    {

                        if (rect.Intersects(player.playerRect))
                        {
                            if (isShoot)
                            {
                                if (enemyRect.Right < player.playerRect.X /*&& MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer*/)
                                {
                                    TargetPos = new Vector2(player.playerRect.X, enemyRect.Y); //Keep Y to only set the X coordinate...Y handled seperately
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
                                    TargetPos = new Vector2(player.playerRect.X, enemyRect.Y); //Keep Y to only set the X coordinate...Y handled seperately
                                }
                                else if (enemyRect.Left > player.playerRect.X + player.playerRect.Width && MathHelper.Distance(enemyRect.Left, player.playerRect.Right) > attackOffsetFromPlayer)
                                {
                                    TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                                }
                            }
                            enemyState = EnemyStates.GoTo;
                        }
                       
                    }
                    break;
                case EnemyStates.GoTo:

                    if (!goTo)
                    {
                        foreach (Rectangle rect in vision)
                        {
    
                            if (rect.Intersects(player.playerRect))
                            {
                                if(isShoot)
                                {
                                    if (enemyRect.Right < player.playerRect.X /*&& MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer*/)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X, enemyRect.Y); //Keep Y to only set the X coordinate...Y handled seperately
                                        outOfRange = true;
                                    }
                                    else if (enemyRect.Left > player.playerRect.X + player.playerRect.Width /*&& MathHelper.Distance(enemyRect.Left, player.playerRect.Right) > attackOffsetFromPlayer*/)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                                        outOfRange = true;
                                    }
                                }
                                else
                                {
                                    if (enemyRect.Right < player.playerRect.X && MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X, enemyRect.Y); //Keep Y to only set the X coordinate...Y handled seperately
                                    }
                                    else if (enemyRect.Left > player.playerRect.X + player.playerRect.Width && MathHelper.Distance(enemyRect.Left, player.playerRect.Right) > attackOffsetFromPlayer)
                                    {
                                        TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                                    }
                                }

                            }
                        }
                    }

                    if (player.velocity.Y < 0 && leftOnX.Count == 0)
                    {
                        leftOnX.Add(player.playerRect.X);
                    }
                    if (player.velocity.Y == 0 && player.playerRect.Bottom >= enemyRect.Bottom && player.isFalling == false)
                    {
                        leftOnX.Clear();
                    }

                    if (player.playerRect.Bottom < enemyRect.Top && player.velocity.Y >= 0 && blockBottom && !goTo/*&& player.blockBottom*/ || blockLeft || blockRight)
                    {
                        //if (player.velocity.Y >= -5)
                        //{
                        //    leftOnX = player.playerRect.X; //where the playere was last to set direction priority if jump fails
                        //}

                        if (player.blockBottom || blockLeft && !blockBottom|| blockRight && !blockBottom)
                        {
                            enemyState = EnemyStates.Jumping;
                        }
                        else
                        {
                            //leftOnX = (int)velocity.x;
                            //velocity.X = 0;
                        }
                        break;
                    }

                    
                    if (enemyRect.Left > player.playerRect.Right && player.playerRect.Bottom < enemyRect.Top == false && player.blockBottom && player.playerRect.Top > enemyRect.Bottom == false && prevState != EnemyStates.Jumping)
                    {
                        if(isShoot && !outOfRange)
                        {
                            if (MathHelper.Distance(enemyRect.Left, player.playerRect.Right) < attackOffsetFromPlayer)
                            {
                                attackLeft = true;
                                attackRight = false;
                                enemyState = EnemyStates.Attacking;
                                velocity.X = 0;
                            }
                        }
                        else
                        {
                            if (MathHelper.Distance(enemyRect.Left, player.playerRect.Right) < attackOffsetFromPlayer)
                            {
                                attackLeft = true;
                                attackRight = false;
                                enemyState = EnemyStates.Attacking;
                                velocity.X = 0;
                            }
                        }
                       
                    }
                    else if(enemyRect.Right < player.playerRect.Left && player.playerRect.Bottom < enemyRect.Top == false && player.blockBottom && player.playerRect.Top > enemyRect.Bottom == false && prevState != EnemyStates.Jumping)
                    {
                        if(isShoot && !outOfRange)
                        {
                            if (MathHelper.Distance(enemyRect.Right, player.playerRect.Left) > attackOffsetFromPlayer)
                            {
                                attackLeft = false;
                                attackRight = true;
                                enemyState = EnemyStates.Attacking;
                                velocity.X = 0;
                            }
                        }
                        else
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
                    else
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

                    GoTo(bounds);


                    break;
                #region Jump
                case EnemyStates.Jumping:
                    if(prevState != EnemyStates.Jumping /*|| player.playerRect.Bottom < enemyRect.Top && player.velocity.Y >= 0 && onPlatform*/)
                    {
                        possibleJumpLocations.Clear();
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
                                    if (possibleJumpLocations.Contains(tile.Rectangle) == false && enemyRect.TouchTopOf(tile.Rectangle) == false)
                                        possibleJumpLocations.Add(tile.Rectangle);

                                    //vision.Remove(vision[i]);
                                }
                            }
                        }

                        if (blockRight || blockLeft)
                        {
                            //int hello = 0;
                        }
                        else
                        {
                            if(possibleJumpLocations.Count != 0)
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
                                        goalRect = rect;
                                    }
                                    else if (MathHelper.Distance(goalRect.Y, position.Y) == MathHelper.Distance(rect.Y, position.Y))
                                    {
                                       
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
                                        //else
                                        //{
                                        //    if (MathHelper.Distance(goalRect.X, position.X) > MathHelper.Distance(rect.X, position.X))
                                        //    {
                                        //        goalRect = rect;
                                        //    }
                                        //}
                                    }
                                    else if (Distance(new Vector2(goalRect.X, goalRect.Y), position) > Distance(new Vector2(rect.X, rect.Y), position))
                                    {
                                        goalRect = rect;
                                    }
                                }

                                Vector2 tempVel = Velocity;
                                if (position.X < goalRect.X)
                                {
                                    tempVel.X = (int)maxRunSpeed;

                                }
                                if (position.X > goalRect.X)
                                {
                                    tempVel.X = (int)-maxRunSpeed;
                                }
                                float randForce = maxJumpForce;
                                int i = TestJump(goalRect, randForce, position, tempVel.X, false);

                                int jumpMin = 10;
                                int speedMin = 1;

                                while (i != 1)//Goes through every possible jump 
                                {
                                    for (int j = jumpMin; j < maxJumpForce; j++)
                                    {
                                        if (tempVel.X > 0)
                                        {
                                            for (int k = speedMin; k < maxRunSpeed; k++)
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
                                            for (int k = -speedMin; k < -maxRunSpeed; k--)
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

                                if (i == 2)
                                {
                                    if(leftOnX.Count != 0)
                                    {
                                        TargetPos = new Vector2(leftOnX[0], position.Y);
                                        leftOnX.Clear();
                                        goTo = true;
                                        maxRunSpeed *= 1.75f;
                                        bounds = SideTileMap.GetNumTilesOfGround((int)(position.Y/64) + 1, (int)position.X/64);
                                        if(bounds != Vector2.Zero)
                                        {
                                            bounds *= 64;
                                        }
                                        enemyState = EnemyStates.GoTo;
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

                                    if (blockRight)
                                    {
                                        velocity.X = -maxRunSpeed;
                                        forceRun = true;
                                    }
                                    else if (blockLeft)
                                    {
                                        velocity.X = maxRunSpeed;
                                        goTo = true;
                                    }
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
                                if (i == 1)
                                {
                                    velocity = new Vector2(tempVel.X, -randForce);
                                    TargetPos = new Vector2((goalRect.X + goalRect.Width / 2) - (enemyRect.Width / 2), goalRect.Top - enemyRect.Height);
                                    prevState = EnemyStates.Jumping;
                                    goTo = true;
                                    isFalling = true;
                                }
                            }
                            else
                            {
                                if(leftOnX.Count != 0)
                                {
                                    TargetPos = new Vector2(leftOnX[0], position.Y);
                                    leftOnX.Clear();
                                    goTo = true;
                                }
                                enemyState = EnemyStates.GoTo;
                            }
                           
                        }
                    }
                    else if(prevState == EnemyStates.Jumping)
                    {
                        foreach (PlatformTile tile in SideTileMap.PlatformTiles)
                        {
                            Collision(tile.Rectangle);
                        }
                        enemyState = EnemyStates.GoTo;
                        if (velocity.Y == 0 && blockBottom)
                        {
                            prevState = EnemyStates.Idle;
                        }

                    }
                    break;
                #endregion
                #region Attacking
                case EnemyStates.Attacking:
                    if(isShoot)
                    {
                        if(attackLeft)
                        {
                            if(shootDelay <= 0)
                            {
                                bullets.Add(new Bullet(new Vector2(HitBox.Right, HitBox.Y + HitBox.Height / 2), -bulletSpeed, new Vector2(-maxBulletSpeed.X, maxBulletSpeed.Y), content, true, bulletTravelDist));
                                shootDelay = maxShootDelay;
                            }
                                
                        }
                        if(attackRight)
                        {
                            if(shootDelay <= 0)
                            {
                                bullets.Add(new Bullet(new Vector2(HitBox.Right, HitBox.Y + HitBox.Height / 2), bulletSpeed, maxBulletSpeed, content, true, bulletTravelDist));
                                shootDelay = maxShootDelay;
                            }
                            
                        }
                       
                    }
                    else
                    {
                        if (attackDelay <= 0)
                        {
                            //Attack
                            if (player.playerRect.Intersects(HitBox) || player.playerRect.Intersects(enemyRect))
                            {
                                player.Health -= meleeDmg;
                            }

                            attackDelay = attackDelayMax;
                        }
                    }
                   
                    prevState = EnemyStates.Attacking;
                    enemyState = EnemyStates.GoTo;
                    break;
                #endregion
            }

            if (isFalling)
            {
                velocity.Y += gravity.Y;
            }
            
            if(velocity.X > 0 && velocity.X > maxRunSpeed)
            {
                velocity.X = maxRunSpeed;
            }
            if(velocity.X < 0 && velocity.X < -maxRunSpeed)
            {
                velocity.X = -maxRunSpeed;
            }
            if (velocity.Y > terminalVel)
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

            position += velocity;

            enemyRect = new Rectangle((int)position.X, (int)position.Y, 48, 48);

            if(isShoot)
            {
                if(bullets.Count != 0)
                {
                    for (int i = bullets.Count - 1; i >= 0; i--)
                    {
                        bullets[i].Update();
                        if(bullets[i].rect.Intersects(player.playerRect))
                        {
                            player.Health -= bulletDmg;
                            bullets.RemoveAt(i);
                        }
                    }
                }
                
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
            spriteBatch.Draw(texture, enemyRect, Color.White);
            //spriteBatch.Draw(texture, HitBox, Color.BlueViolet);
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
            //foreach (Rectangle rect in vision)
            //{
            //    spriteBatch.Draw(visionTxture, rect, Color.White * .25f);


            //}
            //animManager.Draw(spriteBatch);
        }

        public void Collision(Rectangle newRect)
        {
           
            if(enemyRect.TouchTopOf(newRect))
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
                    bounds = SideTileMap.GetNumTilesOfGround(newRect.Y / 64, newRect.X / 64);
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

                }
                else
                {
                    while (enemyRect.Bottom > newRect.Top)
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

            if(enemyRect.TouchBottomOf(newRect))
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

        Vector2 TestCollision(Vector2 startPos, Rectangle tile, Vector2 goalPos, Vector2 tempVel, bool isPlatforms)
        {
            Rectangle eRect = new Rectangle((int)startPos.X, (int)startPos.Y, enemyRect.Width, enemyRect.Height);
            if(eRect.TouchTopOf(tile))
            {

                if(tempVel.Y > 0)
                {
                    if (startPos.X == goalPos.X && isPlatforms || goalRect == tile)
                    {
                        jumpSuccess = true;
                    }
                    else if(!isPlatforms)
                    {
                        jumpFail = true;
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

            return startPos = new Vector2(eRect.X, eRect.Y);
        }

        void PlatformCollision(PlatformTile tile)
        {
            //onPlatform = false;

            Collision(tile.Rectangle);

            for (int i = vision.Count - 1; i >= 0; i--)
            {
                if (vision[i].Intersects(tile.Rectangle))
                {
                    if (i == 0)
                    {
                        blockTop = true;
                    }
                    if (vision[i].Bottom > enemyRect.Bottom && !onPlatform)
                    {
                        currPlatform = tile.Rectangle;
                        onPlatform = true;
                    }
                    vision.RemoveAt(i);
                    //j++;
                    //bool test1 = possibleJumpLocations.Contains(tile.Rectangle);
                    //bool test2 = player.playerRect.TouchTopOf(tile.Rectangle);
                    if (!possibleJumpLocations.Contains(tile.Rectangle) && enemyRect.TouchTopOf(tile.Rectangle) == false)
                        possibleJumpLocations.Add(tile.Rectangle);

                }
            }
        }

        public int TestJump(Rectangle goalRect, float jumpForce, Vector2 startPos, float moveSpeedX, bool failTop)
        {
            jumpFail = false;
            jumpSuccess = false;
            Vector2 goalPos = new Vector2((goalRect.X + goalRect.Width / 2) - (enemyRect.Width/2), goalRect.Top - enemyRect.Height);
            startPos.Y -= 1;
            int num = 0;
            Vector2 velocity = new Vector2(moveSpeedX, -jumpForce);
            while (num == 0)
            {
                if (startPos.X < goalPos.X)
                {
                    velocity.X += moveSpeed;
                    if (velocity.X > maxRunSpeed)
                    {
                        velocity.X = maxRunSpeed;
                    }

                    if (startPos.X + velocity.X > goalPos.X)
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

                    if (startPos.X + velocity.X < goalPos.X)
                    {
                        startPos.X = goalPos.X;
                        velocity.X = 0;
                    }
                }

                velocity.Y += gravity.Y;
                if(velocity.Y > terminalVel)
                {
                    velocity.Y = terminalVel;
                }

                if(velocity.Y == terminalVel && startPos.Y > goalRect.Bottom)
                {
                    num = 2;
                }
                startPos += velocity;

                foreach(GroundTile tile in SideTileMap.GroundTiles)
                {
                    startPos = TestCollision(startPos, tile.Rectangle, goalPos, velocity, false);
                    if(jumpFail)
                    {
                        num = 2;
                    }
                }
                foreach(PlatformTile tile1 in SideTileMap.PlatformTiles)
                {
                    startPos = TestCollision(startPos, tile1.Rectangle, goalPos, velocity, true);
                    if (jumpSuccess)
                        num = 1;
                    if (jumpFail)
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
