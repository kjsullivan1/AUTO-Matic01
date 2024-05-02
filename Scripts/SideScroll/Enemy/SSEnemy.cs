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

        float maxRunSpeed = 3.75f;
        float terminalVel = 12f;
        float maxJumpSpeed = 8f;
        int maxJumpForce = 50;
        int minJumpForce = 16;
        float maxAirSpeed;
        
        #endregion

        #region Velocity
        Vector2 velocity = Vector2.Zero;
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
        public SSEnemy(ContentManager manager, Rectangle Bounds, int visionLength, Vector2 position)
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
        int leftOnX = 0;
        #endregion

        #region Attack Helpers
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

                //vision.Add(new Rectangle(enemyRect.X, enemyRect.Top + (enemyRect.Height * i), pixelSize, pixelSize));//Down
                //if(onPlatform)
                //{
                //    for (int k = i; k < visionLength + 1; k++)//Left and down
                //    {
                //        vision.Add(new Rectangle(enemyRect.X - (enemyRect.Width * k), enemyRect.Top + (enemyRect.Height * i), pixelSize, pixelSize));
                //    }
                //    for (int k = i; k < visionLength + 1; k++)//Right and down
                //    {
                //        vision.Add(new Rectangle(enemyRect.X + (enemyRect.Width * k), enemyRect.Top + (enemyRect.Height * i), pixelSize, pixelSize));
                //    }
                //}
                
            }
            vision.Add(new Rectangle(enemyRect.X + (int)(enemyRect.Width/3.5f), enemyRect.Top + (enemyRect.Height * 1), enemyRect.Width/2, pixelSize/4));
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
                    if(position == TargetPos)
                    {
                        goTo = false;
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

            switch(enemyState)
            {
                case EnemyStates.Idle:
                    foreach(Rectangle rect in vision)
                    {
                        if(rect.Intersects(player.playerRect))
                        {
                            if(enemyRect.Right < player.playerRect.X)
                            {
                                TargetPos = new Vector2(player.playerRect.X, enemyRect.Y); //Keep Y to only set the X coordinate...Y handled seperately
                            }
                            else if(enemyRect.Left > player.playerRect.X + player.playerRect.Width)
                            {
                                TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                            }
                            else if(player.playerRect.Bottom < enemyRect.Top)//player is above enemy...Y is handled here
                            {
                                TargetPos = new Vector2(player.playerRect.X, player.playerRect.Top);
                            }
                            enemyState = EnemyStates.GoTo;
                        }

                    }
                    break;
                case EnemyStates.GoTo:

                    if(!goTo)
                    {
                        foreach (Rectangle rect in vision)
                        {
                            if (rect.Intersects(player.playerRect))
                            {
                                if (enemyRect.Right < player.playerRect.X)
                                {
                                    TargetPos = new Vector2(player.playerRect.X, enemyRect.Y); //Keep Y to only set the X coordinate...Y handled seperately
                                }
                                else if (enemyRect.Left > player.playerRect.X + player.playerRect.Width)
                                {
                                    TargetPos = new Vector2(player.playerRect.X + player.playerRect.Width, enemyRect.Y);
                                }
                            }
                        }
                    }
                   
                    if (player.playerRect.Bottom < enemyRect.Top && player.velocity.Y >= 0 && blockBottom /*&& player.blockBottom*/ || blockLeft || blockRight)
                    {
                        //if (player.velocity.Y >= -5)
                        //{
                        //    leftOnX = player.playerRect.X; //where the playere was last to set direction priority if jump fails
                        //}

                        if (player.blockBottom)
                        {
                            enemyState = EnemyStates.Jumping;
                        }
                        else
                        {
                            leftOnX = (int)velocity.X;
                            velocity.X = 0;
                        }
                        break;
                    }
                    GoTo(bounds);
                    break;
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

                        if (blockLeft || blockRight)
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
                                        if (MathHelper.Distance(goalRect.X, player.Position.X) > MathHelper.Distance(rect.X, player.Position.X))
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
                                    if(leftOnX != 0)
                                    {
                                        velocity.X = leftOnX;
                                    }
                                    else if(position.X > ((goalRect.X + goalRect.Width) + goalRect.Width/2))
                                    {
                                        velocity.X = -maxRunSpeed;
                                        TargetPos = new Vector2(((goalRect.X + goalRect.Width) ), enemyRect.Y);
                                    }
                                    else if(position.X < goalRect.Left)
                                    {
                                        velocity.X = maxRunSpeed;
                                        TargetPos = new Vector2(((goalRect.X - goalRect.Width)), enemyRect.Y);
                                    }
                                    else
                                    {
                                        if(RandFloat(0,1) > .5f)
                                        {
                                            TargetPos = new Vector2(((goalRect.X - goalRect.Width)), enemyRect.Y);
                                        }
                                        else
                                        {
                                            TargetPos = new Vector2(((goalRect.X + goalRect.Width)), enemyRect.Y);
                                        }
                                    }

                                    if (blockRight)
                                    {
                                        velocity.X = -maxRunSpeed;
                                    }
                                    else if (blockLeft)
                                    {
                                        velocity.X = maxRunSpeed;
                                    }
                                    //else if (RandFloat(0, 1) > .5f)
                                    //{
                                    //    velocity.X = maxRunSpeed;
                                    //}
                                    //else
                                    //{
                                    //    velocity.X = -maxRunSpeed;
                                    //}



                                    enemyState = EnemyStates.GoTo;
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
                game.TakeDamage();
                if(velocity.X > 0)
                {
                    position.X += enemyRect.Width * 2;
                    player.velocity.X -= maxRunSpeed;
                }
                if(velocity.X < 0)
                {
                    position.X -= enemyRect.Width * 2;
                    player.velocity.X += maxRunSpeed;
                }
                
            }

            position += velocity;

            enemyRect = new Rectangle((int)position.X, (int)position.Y, 48, 48);

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

            foreach (Rectangle rect in vision)
            {
                spriteBatch.Draw(visionTxture, rect, Color.White * .25f);


            }
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
                   // if(bounds = new Vector2(0,0))
                    if(bounds == Vector2.Zero)
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

        void TestCollision(Vector2 startPos, Rectangle tile, Vector2 goalPos, Vector2 tempVel, bool isPlatforms)
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

                startPos += velocity;

                foreach(GroundTile tile in SideTileMap.GroundTiles)
                {
                    TestCollision(startPos, tile.Rectangle, goalPos, velocity, false);
                    if(jumpFail)
                    {
                        num = 2;
                    }
                }
                foreach(PlatformTile tile1 in SideTileMap.PlatformTiles)
                {
                    TestCollision(startPos, tile1.Rectangle, goalPos, velocity, true);
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
