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
        Rectangle bounds;
        int pixelSize = 64;
        Vector2 position = new Vector2(64 * 8, 0);
        Rectangle enemyRect;
        bool blockBottom = false;
        bool isFalling = true;
        bool isColliding = false;
        Vector2 TargetPos;

        float moveSpeed = 2.5f;
        float mass = 20.0f;
        public float accel = 0;
        public float force = 0;
        public float friction = 0;
        float coeFric = 0;
        public float changeInTime = 0;
        //bool canJump;
        int jumpDelay = 0;
        int maxJumpDelay = 5;
        Vector2 gravity;

        float maxRunSpeed = 4.2f;
        float terminalVel = 12f;
        float maxJumpSpeed = 8f;
        int maxJumpForce = 18;
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
        #endregion\

        #region Constructor
        public SSEnemy(ContentManager manager, Rectangle Bounds, int visionLength)
        {
            content = manager;
            bounds = Bounds;
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
        bool onPlatform = false;
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
                //for (int k = i; k < visionLength + 1; k++)//Left and down
                //{
                //    vision.Add(new Rectangle(enemyRect.X - (enemyRect.Width * k), enemyRect.Top + (enemyRect.Height * i), pixelSize, pixelSize));
                //}
                //for (int k = i; k < visionLength + 1; k++)//Right and down
                //{
                //    vision.Add(new Rectangle(enemyRect.X + (enemyRect.Width * k), enemyRect.Top + (enemyRect.Height * i), pixelSize, pixelSize));
                //}
            }
            vision.Add(new Rectangle(enemyRect.X, enemyRect.Top + (enemyRect.Height * 1), pixelSize, pixelSize));
            #endregion
        }


        void GoTo(Vector2 target)
        {
            Vector2 tTarget = target;
            Vector2 tempPos = Vector2.Zero;
            if (position.X > target.X)
            {
                tTarget = new Vector2((int)(target.X / pixelSize) * (pixelSize), (int)(target.Y / pixelSize) * pixelSize);
                tempPos = new Vector2((int)(position.X / pixelSize) * pixelSize, (int)(position.Y / pixelSize) * pixelSize);
            }
            else if(position.X < target.X)
            {
                tTarget = new Vector2((int)(target.X / pixelSize) * (pixelSize), (int)(target.Y / pixelSize) * pixelSize);
                tempPos = new Vector2((int)(position.X / pixelSize) * pixelSize, (int)(position.Y / pixelSize) * pixelSize);
            }



            if (MathHelper.Distance(tempPos.X, target.X) < positionOffset.X)
            {

            }
            else if ((int)tempPos.X < (int)tTarget.X)
            { 
                tTarget -= positionOffset;

                if((int)tempPos.X < (int)tTarget.X && !isColliding)
                {
                    velocity.X += moveSpeed;
                }
            }
            else if((int)tempPos.X > (int)tTarget.X)
            {
                tTarget += positionOffset;
              
                if ((int)tempPos.X > (int)tTarget.X && !isColliding)
                {
                    velocity.X -= moveSpeed;
                }

            }

            if((int)tempPos.X == (int)tTarget.X && !isColliding /*&& (int)tempPos.Y == (int)tTarget.Y*/ && prevState == EnemyStates.Jumping)
            {
                //enemyState = EnemyStates.OnPlatform;
                 enemyState = EnemyStates.GoTo;

                //onPlatform = true;
            }
            else if((int)tempPos.X == (int)tTarget.X && !isColliding && tempPos.Y == tTarget.Y )
            {
                velocity.X = 0;
                enemyState = EnemyStates.Attacking;
                //if(MathHelper.Distance((int)target.X, (int)tempPos.X) < positionOffset.X)
                //{
                //    //velocity.X += moveSpeed;
                //}
            }
         
            

        }

        public void Update(GameTime gameTime, Vector2 gravity, SSPlayer player)
        {
            this.gravity = gravity;

            CreateVision();

            possibleJumpLocations.Clear();
            //int j = 0;
            blockBottom = false;
            isColliding = false;
            blockTop = false;
            foreach (EmptyTile tile in SideTileMap.EmptyTiles)
            {
                TileCollision(tile);
            }
            if (blockBottom)
            {
                isFalling = false;
            }
            if (!blockBottom)
            {
                isFalling = true;
            }

            switch (enemyState)
            {
                case EnemyStates.Idle:
                    foreach(Rectangle rect in vision)
                    {
                        if(rect.Intersects(player.playerRect))
                        {
                            TargetPos = new Vector2(player.X + (player.playerRect.Width / 2), player.Y);
                            //prevState = enemyState;
                            enemyState = EnemyStates.GoTo;
                        }
                    }
                    break;
                case EnemyStates.GoTo:

                   


                    if ((enemyRect.Bottom > (int)player.playerRect.Bottom && player.isFalling == false && velocity.Y == 0) /*&& prevState != EnemyStates.Jumping*/)
                    {
                        prevState = enemyState;
                        enemyState = EnemyStates.Jumping;
                        canJump = true;
                    }
                    else
                    {
                        Vector2 boundX = new Vector2(currPlatform.X, currPlatform.X + currPlatform.Width);
                        foreach (Rectangle rect in vision)
                        {
                            if (rect.Intersects(player.playerRect))
                            {
                                TargetPos = new Vector2(player.X + (player.playerRect.Width / 2), player.Y);
                            }
                        }
                    }
                    //prevState = enemyState;
                    GoTo(TargetPos);
                    break;
                case EnemyStates.Attacking:
                    if(position.X < player.X)
                    {
                        if (MathHelper.Distance(position.X, player.X) > positionOffset.X * 2)
                        {
                            //prevState = enemyState;
                            enemyState = EnemyStates.GoTo;
                            TargetPos = new Vector2(player.X + (player.playerRect.Width / 2), player.Y);
                        }
                    }
                    else if(position.X > player.X)
                    {
                        if (MathHelper.Distance(position.X, player.X) > positionOffset.X)
                        {
                            //prevState = enemyState;
                            enemyState = EnemyStates.GoTo;
                            TargetPos = new Vector2(player.X + (player.playerRect.Width/2), player.Y);
                        }
                    }

                    if ((enemyRect.Bottom > (int)player.playerRect.Bottom && player.canJump == true && velocity.Y == 0))
                    {
                        prevState = enemyState;
                        enemyState = EnemyStates.Jumping;
                        goalRect = possibleJumpLocations[0];
                        canJump = true;
                    }


                    break;
                case EnemyStates.Jumping:
                 
                    //Determine a X distance goal from the target pos and see if you can reach it. If you can...Execute
                    if (prevState != EnemyStates.Jumping && blockBottom && !isFalling && possibleJumpLocations.Count != 0 && !blockTop/*&& !canJump*/)
                    {
                        float randForce = RandFloat(minJumpForce, maxJumpForce);
                        //canJump = true;
                        goalRect = possibleJumpLocations[0];
                        foreach(Rectangle rect in possibleJumpLocations)
                        {

                            if(MathHelper.Distance(goalRect.Y ,position.Y) > MathHelper.Distance(rect.Y , position.Y))
                            {
                                goalRect = rect;
                            }
                            else if(MathHelper.Distance(goalRect.Y, position.Y) == MathHelper.Distance(rect.Y, position.Y))
                            {
                                if(MathHelper.Distance(goalRect.X, position.X) > MathHelper.Distance(rect.X, position.X))
                                {
                                    goalRect = rect;
                                }
                            }
                        }
                        int i = TestJump(goalRect, randForce, position, velocity.X);
                        while (i != 1)
                        {
                            randForce = RandFloat(minJumpForce, maxJumpForce);
                           
                            i = TestJump(goalRect, randForce, position, velocity.X);

                            if((int)randForce == (int)maxJumpForce - 1 && i != 1)//Couldnt succeed on max attempt try calculating distance needed to make jump
                            {
                                i = 2;
                                break;
                            }
                            if (i == 2) 
                            {
                                break;
                            }
                        }
                        if(i == 1)
                        {
                            velocity = new Vector2(velocity.X, -randForce);
                            prevState = EnemyStates.Jumping;
                            if(goalRect.X < position.X)
                            {
                                TargetPos = new Vector2(goalRect.X, goalRect.Top + (enemyRect.Height));
                            }
                            else if(goalRect.X > position.X)
                            {
                                TargetPos = new Vector2(goalRect.X + (goalRect.Width), goalRect.Top + (enemyRect.Height));
                            }
                            else
                            {
                                TargetPos = new Vector2(goalRect.X + (goalRect.Width/2), goalRect.Top + (enemyRect.Height));
                            }
                           

                        }
                        else if(i == 2) //Calculate distance needed to travel to get to jumping distance...Maybe make this loop itself
                        {
                            prevState = enemyState;
                            enemyState = EnemyStates.GoTo;
                        }
                       
                    }
                    else if (prevState == EnemyStates.Jumping)
                    {
                        if(velocity.Y == 0 && (int)position.X == (int)TargetPos.X && blockBottom)
                        {
                            enemyState = EnemyStates.GoTo;
                        }
                        else
                        {
                            GoTo(TargetPos); //This is the execution and what happens during and fixes direction going in 
                        }
                       
                    }


                    #region Failed GA
                    //if (possibleJumpLocations.Count != 0 && !gaJump.isStarted)
                    //{
                    //    gaJump.CreatePopulation(possibleJumpLocations, position, this, bounds);
                    //    gaJump.CalcFitness();
                    //    while (gaJump.GetBestFitness() != 25)
                    //    {
                    //        gaJump.CreatePopulation(possibleJumpLocations, position, this, bounds);
                    //        gaJump.CalcFitness();
                    //    }
                    //    jumpInfo.startPos = gaJump.GetBestChromosome().startPos;
                    //    jumpInfo.jumpForce = gaJump.GetBestChromosome().jumpForce;

                    //}
                    //if((canJump && position == jumpInfo.startPos) || isColliding)
                    //{
                    //    velocity.Y = jumpInfo.jumpForce * 2;
                    //    position.Y -= 1;
                    //    gaJump.isStarted = false;
                    //    canJump = false;
                    //    enemyState = EnemyStates.GoTo;
                    //}
                    //else if(!isColliding)
                    //{
                    //    if((int)position.X < jumpInfo.startPos.X)
                    //    {

                    //            velocity.X += moveSpeed;


                    //    }
                    //    else if((int)position.X > jumpInfo.startPos.X)
                    //    {



                    //            velocity.X -= moveSpeed;

                    //    }

                    //    //if ((int)position.X < player.Position.X)
                    //    //{
                    //    //    if (velocity.X >= 0)
                    //    //    {
                    //    //        if (velocity.X >= 0)
                    //    //        {
                    //    //            velocity.X += moveSpeed;
                    //    //        }
                    //    //        else
                    //    //        {
                    //    //            velocity.X = -velocity.X;
                    //    //        }
                    //    //    }
                    //    //}
                    //    //else if ((int)position.Y > player.Position.X)
                    //    //{
                    //    //    if (velocity.X <= 0)
                    //    //    {
                    //    //        velocity.X -= moveSpeed;
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        velocity.X = -velocity.X;
                    //    //    }
                    //    //}
                    //    //GoTo(jumpInfo.startPos);
                    //}

                    //if ((enemyRect.Bottom == (int)player.playerRect.Bottom && player.canJump == true))
                    //{
                    //    enemyState = EnemyStates.GoTo;
                    //}
                    #endregion
                    break;
            }

           
            if (isFalling)
            {
                velocity.Y += gravity.Y;
            }
             position += Velocity;

            enemyRect = new Rectangle((int)position.X, (int)position.Y, 48, 48);

            //if(velocity.Y != 0 && blockBottom)
            //{
            //    if(enemyState == EnemyStates.Jumping)
            //    {
            //        enemyState = EnemyStates.GoTo;
            //    }
            //}

           
       
        }

        private void TileCollision(EmptyTile tile)
        {
            Collision(tile.Rectangle);

   
            for (int i = vision.Count - 1; i >= 0; i--)
            {
                if (vision[i].Intersects(tile.Rectangle))
                {
                    if (i == 0)
                    {
                        blockTop = true;
                    }
                    vision.RemoveAt(i);
                    //j++;
                    //bool test1 = possibleJumpLocations.Contains(tile.Rectangle);
                    //bool test2 = player.playerRect.TouchTopOf(tile.Rectangle);
                    if (!possibleJumpLocations.Contains(tile.Rectangle) && enemyRect.TouchTopOf(tile.Rectangle) == false)
                        possibleJumpLocations.Add(tile.Rectangle);
                    if(enemyRect.TouchTopOf(tile.Rectangle) == true)
                    {
                        currPlatform = tile.Rectangle;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
           
            Rectangle tRect = enemyRect;
            //tRect.X += tRect.Width/5;
            spriteBatch.Draw(texture, tRect, Color.White);

            foreach (Rectangle rect in vision)
            {
                spriteBatch.Draw(visionTxture, rect, Color.White * .25f);


            }
            //animManager.Draw(spriteBatch);
        }

        public void Collision(Rectangle newRect)
        {
             //blockBottom = false;
         //isColliding = false;
            if (enemyRect.TouchTopOf(newRect))
            {
                //isColliding = true;
                blockBottom = true;
                //canJump = true;
                if(isFalling)
                {
                    while (enemyRect.Bottom > newRect.Top)
                    {
                        velocity.Y += -(Velocity.Y);
                        position.Y -= .1f;
                        enemyRect.Y = (int)position.Y;
                    }

                    isFalling = false;
                    
                    //animManager state changer here
                }
                else if(enemyState == EnemyStates.Jumping)
                {

                }
                else
                {
                    while (enemyRect.Bottom > newRect.Top)
                    {
                        velocity.Y += -(Velocity.Y);
                        position.Y -= .1f;
                        enemyRect.Y = (int)position.Y;
                    }

                    
                    //jumpDelay++;
                    //if(jumpDelay >= maxJumpDelay)
                    //{
                    //    canJump = true;
                    //}
                }
            }

            if (enemyRect.TouchLeftOf(newRect))
            {
                while (enemyRect.Right > newRect.Left)
                {
                    position.X -= Math.Abs(Velocity.X);
                    enemyRect.X = (int)position.X;
                    //position.X = enemyRect.X;
                }
                isColliding = true;
                switch(enemyState)
                {
                    case EnemyStates.GoTo:
                        if(velocity.X < 0)
                        {

                        }
                        else
                        {
                            position.X += -Velocity.X;
                        }
                        //canJump = true;
                        break;
                    case EnemyStates.Jumping:
                        if (velocity.X < 0)
                        {

                        }
                        else
                        {
                            position.X += -Velocity.X;
                        }
                        break;
                }
               // enemyRect.X = (int)position.X;
            }

            if(enemyRect.TouchRightOf(newRect))
            {
                float currX = position.X;
                while (enemyRect.Left < newRect.Right)
                {
                    position.X += Math.Abs(Velocity.X);
                    enemyRect.X = (int)position.X;
                }

                if(MathHelper.Distance(currX, position.X) > pixelSize * 2)
                {
                    position.X = (newRect.Left + enemyRect.Width) - 1;
                    enemyRect.X = (int)position.X;
                }

                isColliding = true;
                switch (enemyState)
                {
                    case EnemyStates.GoTo:
                        if(velocity.X > 0)
                        {

                        }
                        else
                        {
                            position.X += -Velocity.X;
                        }
                        //canJump = true;
                        break;
                    case EnemyStates.Jumping:
                        if (velocity.X > 0)
                        {

                        }
                        else
                        {
                            position.X += -Velocity.X;
                        }
                        break;
                }
            }

            if (enemyRect.TouchBottomOf(newRect))
            {

                if (isFalling || enemyState == EnemyStates.Jumping)
                {
                    while (enemyRect.Top < newRect.Bottom)
                    {
                        //velocity.Y = 0;
                        position.Y += .01f;
                        enemyRect.Y = (int)position.Y;
                    }

                    if (velocity.Y < 0)
                    {
                        velocity.Y = 0;
                    }


                }
                else
                {
                    while (enemyRect.Top < newRect.Bottom)
                    {
                        velocity.Y = 0;
                        position.Y += .01f;
                        enemyRect.Y = (int)position.Y;
                    }
                    if (velocity.Y < 0)
                    {
                        velocity.Y = 0;
                    }
                }



                //position.Y += -(velocity.Y);


                isColliding = true;
            }




        }

        public int TestJump(Rectangle goalRect, float jumpForce, Vector2 startPos, float moveSpeedX)
        {
            jumpSuccess = false;
            jumpFail = false;
            bool isJumping = true;
            Vector2 tempVel = new Vector2(moveSpeedX, -jumpForce);
            Vector2 tempPos = position;
            Rectangle testRect = enemyRect;
            int num = 0;
            while(num == 0)
            {

                

                if(isJumping)
                {
                    tempVel += gravity;
                    //tempPos += tempVel;
                    if(tempVel.Y > terminalVel)
                    {
                        num = 2;
                        break;
                        
                    }
                }

                tempPos += tempVel;
                isColliding = false;
                testRect = new Rectangle((int)tempPos.X, (int)tempPos.Y, enemyRect.Width, enemyRect.Height);
                jumpFail = false;
                jumpSuccess = false;
                foreach(EmptyTile tile in SideTileMap.EmptyTiles)
                {
                    testRect = TestCollision(tile.Rectangle, testRect, tempVel, tempPos, goalRect, isJumping);

                    if(jumpSuccess)
                    {
                        num = 1;
                        break;
                    }
                    else if(jumpFail)
                    {
                        num = 2;
                        break;
                    }
                }

               
            }

            return num;

        }

        public float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }

        public Rectangle TestCollision(Rectangle newRect, Rectangle testRect, Vector2 tempVel, Vector2 tempPos, Rectangle goalRect, bool isJumping)
        {
            //blockBottom = false;
            //isColliding = false;
            if (testRect.TouchTopOf(newRect))
            {
                while (testRect.Bottom > newRect.Top)
                {
                    tempVel.Y += -(tempVel.Y);
                    tempPos.Y -= .1f;
                    testRect.Y = (int)tempPos.Y;
                }

                if(isJumping)
                {
                    if (newRect == goalRect)
                    {
                        jumpSuccess = true;
                    }
                    else
                    {
                        jumpFail = true;
                    }
                }
               
                //isColliding = true;
                //blockBottom = true;
                //if (isFalling)
                //{
                //    while (testRect.Bottom > newRect.Top)
                //    {
                //        velocity.Y += -(Velocity.Y);
                //        position.Y -= .1f;
                //        enemyRect.Y = (int)position.Y;
                //    }

                //    isFalling = false;

                //    //animManager state changer here
                //}
                //else if (enemyState == EnemyStates.Jumping)
                //{

                //}
                //else
                //{
                //    while (testRect.Bottom > newRect.Top)
                //    {
                //        tempVel.Y += -(tempVel.Y);
                //        testRect.Y -= .1f;
                //        enemy.Y = (int)position.Y;
                //    }
                //    jumpDelay++;
                //    if (jumpDelay >= maxJumpDelay)
                //    {
                //        canJump = true;
                //    }
                //}
            }

            if (testRect.TouchLeftOf(newRect))
            {
                while (testRect.Right > newRect.Left)
                {
                    testRect.X -= 1;
                    //position.X = playerRect.X;
                }
                isColliding = true;
                //switch (enemyState)
                //{
                //    case EnemyStates.GoTo:
                //        position.X += -Velocity.X;
                //        break;
                //}
            }

            if (testRect.TouchRightOf(newRect))
            {
                while (testRect.Left < newRect.Right)
                {
                    testRect.X += 1;
                    //position.X = playerRect.X;
                }

                isColliding = true;
                //switch (enemyState)
                //{
                //    case EnemyStates.GoTo:
                //        position.X += -Velocity.X;
                //        break;
                //}
            }

            if (testRect.TouchBottomOf(newRect))
            {

                jumpFail = true;



                //position.Y += -(velocity.Y);


                
            }

            return testRect;

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
