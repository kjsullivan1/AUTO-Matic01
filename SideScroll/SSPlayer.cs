using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic.SideScroll
{
    class SSPlayer
    {
        ContentManager content;

        //Player States
        enum AnimationStates { Walking, Death, Idle, Jump, Shoot, Dash}
        AnimationStates animState = AnimationStates.Idle;

        public enum PlayerStates { Movement, Shooting, Jumping, Dashing}
        public PlayerStates playerState = PlayerStates.Movement;
        PlayerStates prevPlayerState;

        #region Fields
        float collisionOffsetX = 20f;
        int pixelSize = 64;
        Vector2 position = Vector2.Zero;
        Vector2 prevVel = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        Rectangle playerRect;
        public bool isFalling = true;
        bool isColliding = false;
        Game1 game;
        KeyboardState prevKb;
        public bool blockBottom = false;

        float moveSpeed = .35f;
        float iMoveSpeed;
        float fallMoveSpeed = .15f;


        float mass = 20.0f;
        public float accel = 0;
        public float force = 0;
        public float friction = 0;
        float coeFric = 0;
        public float changeInTime = 0;

        float maxRunSpeed = 5f;
        float terminalVel = 12f;
        float maxJumpSpeed = 8f;
        float maxDashSpeed = 22.5f;
        float maxDashAirSpeed = 15f;
        #endregion

        #region Shooting
        Texture2D gunTexture;
        List<Bullet> bullets = new List<Bullet>();
        MouseState prevMs;
        float bulletSpeed = .5f;
        float bulletMaxX = 15f;
        float bulletMaxY = 0;
        bool isShootDelay = false;
        float shootDelay = .8f;//In seconds
        float iShootDelay;
        #endregion

        #region Jumping
        float jumpForce = 18f;
        float iJumpF;
        int jumpDelay = 0;
        int maxJumpDelay = 5;
        bool canJump = false;
        public float JumpForce
        {
            get
            {
                return jumpForce;
            }
            set
            {
                if (value > iJumpF)
                {
                    jumpForce = iJumpF;
                }
                else if (value <= 0)
                {
                    jumpForce = 0;
                }
                else
                {
                    jumpForce = value;
                }
            }
        }
        #endregion
        
        #region Dashing
        public float dashForceX = 16f;
        public float dashForceY = 0f;
        public bool canDash = true;
        //public bool isDashing = false;
        float dashCoolDown = 0;
        float dashCoolDownMax = 3;
        Vector2 DashForce
        {
            get
            {
                return new Vector2(dashForceX, dashForceY);
            }
        }
        #endregion
        
        #region Velocity
        Vector2 Velocity
        {
            get
            {
                Vector2 pos = velocity;

                if (pos.X > maxRunSpeed && !isFalling && prevPlayerState != PlayerStates.Dashing)
                {
                    pos = new Vector2(maxRunSpeed, pos.Y);
                }
                if (pos.X > maxDashAirSpeed && prevPlayerState == PlayerStates.Dashing && isFalling)
                {
                    pos = new Vector2(maxDashAirSpeed, pos.Y);

                }
                if (pos.X > maxDashSpeed && prevPlayerState == PlayerStates.Dashing && !isFalling)
                {
                    pos = new Vector2(maxDashSpeed, pos.Y);
                }
                if (pos.X < maxRunSpeed && prevPlayerState == PlayerStates.Dashing && pos.X > 0)
                {
                    //pos = new Vector2(maxDashSpeed, pos.Y); isdasing = false was here 
                }

                if (pos.X < -maxRunSpeed && !isFalling && prevPlayerState != PlayerStates.Dashing)
                {
                    pos = new Vector2(-maxRunSpeed, pos.Y);
                }
                if (pos.X < -maxDashSpeed && prevPlayerState == PlayerStates.Dashing && !isFalling)
                {
                    pos = new Vector2(-maxDashSpeed, pos.Y);
                    //isDashing = false;
                }
                if (pos.X < -maxDashAirSpeed && prevPlayerState == PlayerStates.Dashing && isFalling)
                {
                    pos = new Vector2(-maxDashAirSpeed, pos.Y);
                }
                if (pos.X > -maxRunSpeed && prevPlayerState == PlayerStates.Dashing && pos.X < 0)
                {
                    //pos = new Vector2(-maxDashAirSpeed, pos.Y);
                }


                if (pos.Y > terminalVel && isFalling)
                {
                    pos = new Vector2(pos.X, terminalVel);
                }
                if (pos.Y < -maxJumpSpeed && isFalling)
                {
                    pos = new Vector2(pos.X, -maxJumpSpeed);
                }
                velocity = pos;
                return velocity;
            }
        }
        #endregion

        #region Helpers
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Rectangle Rectangle
        {
            get { return playerRect; }
        }
        public SSPlayer(Game1 game1, int pixelSize)
        {
            game = game1;
            this.pixelSize = pixelSize - 4;

        }
        #endregion

        #region Physics 
        public float Force
        {
            get
            {

                return Math.Abs(force * 10f);

            }
        }

        public float Acceleration
        {
            get
            {
                if (velocity == Vector2.Zero)
                {
                    return 0;
                }
                else
                {
                    return accel;
                }
            }
        }

        public float Friction
        {
            get { return Math.Abs(friction); }
        }

        public int Y
        {
            get { return (int)position.Y; }
            set { position.Y = value; }
        }

        public int X
        {
            get { return (int)position.X; }
            set { position.X = value; }
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
                case AnimationStates.Idle:
                    texture = content.Load<Texture2D>("SideScroll/Animations/PlayerIdle");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(6, 1);
                    fpms = 120;
                    break;
                case AnimationStates.Walking:
                    texture = content.Load<Texture2D>("SideScroll/Animations/PlayerWalk");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(8, 1);
                    fpms = 120;
                    break;
                case AnimationStates.Jump:
                    texture = content.Load<Texture2D>("SideScroll/Animations/PlayerJump");
                    FrameSize = new Point(64, 64);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(4, 1);
                    fpms = 95;
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

            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, Position);

            animManager.isRight = isRight;
            animManager.isLeft = isLeft;
            animManager.isUp = isUp;
            animManager.isDown = isDown;
        }
        #endregion

        public void Load(ContentManager Content, Rectangle bounds, float friction)
        {
            content = Content;
            texture = Content.Load<Texture2D>("SideScroll/MapTiles/Tile4");
            prevVel = velocity;
            //maxVelocity = new Vector2(maxRunSpeed, terminalVel);
            this.friction = friction;
            iJumpF = jumpForce;
            ChangeAnimation();
            iMoveSpeed = moveSpeed;
            iShootDelay = shootDelay;

        }

        public void Update(GameTime gameTime, Vector2 gravity)
        {
            if (!isColliding)
            {
                Input();
            }

            if(Velocity == Vector2.Zero && !isFalling)
            {
                if (animState != AnimationStates.Idle)
                {
                    animState = AnimationStates.Idle;
                    ChangeAnimation();
                }
            }

            if (!canDash)
            {
                dashCoolDown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (dashCoolDown >= dashCoolDownMax)
            {
                dashCoolDown = 0;
                canDash = true;
            }

            changeInTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (prevVel != Velocity)
            {
                accel = Math.Abs((prevVel.Length() - Velocity.Length())) / changeInTime;
            }
            force = mass * Acceleration;

            switch (playerState)
            {
                #region Movement
                case PlayerStates.Movement:
                    if(prevPlayerState == PlayerStates.Dashing)
                    {
                        //Slow down over time instead of instant set to run speed
                    }

                    if (isFalling)
                    {
                        velocity.Y += gravity.Y;
                        moveSpeed = fallMoveSpeed;

                        if (Velocity.X > 0)
                        {
                            velocity.X -= gravity.X;
                            if (Velocity.X < 0)
                            {
                                velocity.X = 0;
                            }
                        }
                        else if (Velocity.X < 0)
                        {
                            velocity.X += gravity.X;
                            if (Velocity.X > 0)
                            {
                                velocity.X = 0;
                            }
                        }
                        else
                        {
                            moveSpeed = iMoveSpeed;
                            //dashForceY = 0f;
                        }
                    }

                  
                    break;
                #endregion

                #region Dashing
                case PlayerStates.Dashing:
                    gravity.Y = 0;
                    canDash = false;

                    if(Velocity.X == maxDashSpeed && !isFalling)
                    {
                        prevPlayerState = playerState;
                        playerState = PlayerStates.Movement;
                    }
                    break;
                #endregion

                #region Jumping
                case PlayerStates.Jumping:
                    if(Velocity.Y <= 0)
                    {
                        if(Velocity.Y >= 0)
                        {
                            playerState = PlayerStates.Movement;
                            isFalling = true;
                        }
                    }

                    velocity.Y += gravity.Y;
                    if (animState == AnimationStates.Jump)
                    {
                        if (animManager.GetCurrFrame().X >= animManager.GetSheetSize().X - 1)
                        {
                            //animManager.SetFPMS(0);
                            //animManager.SetFrameToEnd();
                            animManager.StopLoop();
                        }


                    }
                    break;
                    #endregion
            }



            position += Velocity;
            animManager.Update(gameTime, position);
            //switch (playerState)
            //{
            //    case PlayerStates.Movement:

            //        break;
            //}
        }


        private void Input()
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if(kb.IsKeyDown(Keys.D) && kb.IsKeyDown(Keys.A))
            {
                switch(playerState)
                {
                    #region Movement
                    case PlayerStates.Movement:
                        bool right = false, left = false;

                        if(Velocity.X > 0)
                        {
                            right = true;
                            animManager.isRight = true;
                            animManager.isLeft = false;

                            if(animState != AnimationStates.Walking && animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Walking;
                                ChangeAnimation();
                            }
                        }
                        else
                        {
                            left = true;
                            animManager.isRight = false;
                            animManager.isLeft = true;
                            if (animState != AnimationStates.Walking && animState != AnimationStates.Jump)
                            {

                                animState = AnimationStates.Walking;
                                ChangeAnimation();
                            }
                        }

                        velocity.X += -Velocity.X * (moveSpeed * 2);
                        if (Velocity.X < 0 && right)
                        {
                            velocity.X = 0;
                            accel = 0;
                        }
                        else if (Velocity.X > 0 && left)
                        {
                            velocity.X = 0;
                            accel = 0;
                        }

                        if(kb.IsKeyDown(Keys.LeftShift) && canDash)
                        {
                            playerState = PlayerStates.Dashing;

                            prevKb = kb;
                            if (right)
                            {
                                velocity = new Vector2(DashForce.X, -DashForce.Y);
                            }
                            else
                            {
                                velocity = new Vector2(-(DashForce.X), -DashForce.Y);
                            }


                            position.Y -= 1f;
                            if (prevVel != Velocity)
                            {
                                accel = force / mass;
                            }
                        }

                        if(kb.IsKeyDown(Keys.Space) && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;

                            if (animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Jump;
                                ChangeAnimation();
                            }
                            prevKb = kb;

                            velocity.Y = -jumpForce;

                            if (JumpForce != 0)
                                position.Y -= 1f;

                            if (prevVel != Velocity)
                            {
                                accel = force / mass;
                            }
                            canJump = false;
                        }
                        break;
                        #endregion
                }
            }
            else if(kb.IsKeyDown(Keys.D))
            {
                switch(playerState)
                {
                    #region Movement
                    case PlayerStates.Movement:
                        if (animState != AnimationStates.Walking && animState != AnimationStates.Jump)
                        {
                            animState = AnimationStates.Walking;
                            ChangeAnimation();
                        }
                        animManager.isRight = true;
                        animManager.isLeft = false;

                        if (Velocity.X < .000001 && Velocity.X != 0/* && Velocity.X > 0*/)//Get rid of last statement for change
                        {
                            velocity.X = -Velocity.X * friction;
                            velocity.X += moveSpeed / 2;// * friction; //for dragging
                           
                            //accel = force / mass;
                        }
                        else if (Velocity.X < 0)
                        {
                            velocity.X += -Velocity.X * (moveSpeed * 2);
                        }
                        else
                        {
                            if (isFalling)
                            {
                                if (Velocity.X < 0)
                                {
                                    velocity.X = -Velocity.X / 1.5f;//Can change to turn with exact momentum
                                }
                            }

                            velocity.X += moveSpeed;// * friction; //for dragging
                           



                            //JumpForce += .5f;
                            //accel = force / mass;
                        }

                        if(kb.IsKeyDown(Keys.LeftShift) && canDash)
                        {
                            playerState = PlayerStates.Dashing;
                            prevKb = kb;

                            velocity = new Vector2(DashForce.X, -DashForce.Y);
                            position.Y -= 1f;
                        }

                        if (kb.IsKeyDown(Keys.Space) && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;

                            if (animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Jump;
                                ChangeAnimation();
                            }
                            prevKb = kb;

                            velocity.Y = -jumpForce;

                            position.Y -= 1f;

                            if (prevVel != Velocity)
                            {
                                accel = force / mass;
                            }
                            canJump = false;
                        }

                        prevKb = kb;

                        break;
                    #endregion
                }
            }
            else if(kb.IsKeyDown(Keys.A))
            {
                switch(playerState)
                {
                    #region Movement
                    case PlayerStates.Movement:

                        animManager.isRight = false;
                        animManager.isLeft = true;
                        if (animState != AnimationStates.Walking && animState != AnimationStates.Jump)
                        {
                            animState = AnimationStates.Walking;
                            ChangeAnimation();
                        }

                        if (Velocity.X > -.000001 && Velocity.X != 0 /*&& Velocity.X < 0*/)//Future test commented out
                        {
                            velocity.X = -Velocity.X * friction;
                            velocity.X += -moveSpeed / 2; // * friction for dragging
                                                          //accel = force / (mass);
                           // trueTurn = false;
                        }
                        else if (Velocity.X > 0)
                        {
                            velocity.X += -Velocity.X * (moveSpeed * 2);
                        }
                        else
                        {
                            //trueTurn = false;
                            if (isFalling)
                            {
                                if (Velocity.X > 0)
                                {
                                    velocity.X = -Velocity.X / 1.5f;//Can change to turn with exact momentum
                                }
                            }
                            velocity.X += -moveSpeed;
                            //JumpForce += .5f;
                            //accel = force / (mass);
                        }

                        if(kb.IsKeyDown(Keys.LeftShift) && canDash)
                        {
                            playerState = PlayerStates.Dashing;
                            prevKb = kb;

                            velocity = new Vector2(-(DashForce.X), -DashForce.Y);

                            position.Y -= 1f;
                        }

                        if (kb.IsKeyDown(Keys.Space) && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;

                            if (animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Jump;
                                ChangeAnimation();
                            }
                            prevKb = kb;

                            velocity.Y = -jumpForce;

                            
                                position.Y -= 1f;

                            if (prevVel != Velocity)
                            {
                                accel = force / mass;
                            }
                            canJump = false;
                        }

                        prevKb = kb;
                        break;
                    #endregion
                }
            }
            else
            {
                switch(playerState)
                {
                    #region Movement
                    case PlayerStates.Movement:
                        if (Velocity.X > 0 && !isFalling)
                        {
                            animManager.isRight = true;
                            animManager.isLeft = false;

                            if (animState != AnimationStates.Walking && animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Walking;
                                ChangeAnimation();
                            }

                            if (Velocity.X + -friction < 0)
                            {
                                //prevVel = velocity;
                                velocity = Vector2.Zero;

                                accel = 0;
                            }
                            else
                            {
                                //prevVel = velocity;
                                velocity += new Vector2(-friction, 0);
                                //if (prevVel != velocity)
                                //{
                                //    accel = (velocity.Length() - prevVel.Length()) / changeInTime;
                                //}

                            }

                        }
                        else if (Velocity.X < 0 && !isFalling)
                        {
                            animManager.isRight = false;
                            animManager.isLeft = true;

                            if (animState != AnimationStates.Walking && animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Walking;
                                ChangeAnimation();
                            }


                            if (Velocity.X + friction > 0)
                            {
                                //prevVel = velocity;
                                velocity = Vector2.Zero;
                                accel = 0;
                            }
                            else
                            {

                                velocity += new Vector2(friction, 0);

                                //if (prevVel != velocity)
                                //{
                                //    accel = (velocity.Length() - prevVel.Length()) / changeInTime;
                                //}

                            }
                        }

                        if(isFalling)
                        {
                            if(velocity.X > 0)
                            {
                                velocity.X -= 1.5f;
                                if (velocity.X < 0)
                                    velocity.X = 0;
                            }
                            else if (velocity.X < 0)
                            {
                                velocity.X += 1.5f;
                                if (velocity.X > 0)
                                    velocity.X = 0;
                            }


                        }

                        if ((kb.IsKeyDown(Keys.LeftShift)) && canDash)
                        {

                            playerState = PlayerStates.Dashing;
                            prevKb = kb;
                            if (animManager.isRight)
                            {
                                velocity = new Vector2(DashForce.X, -DashForce.Y);
                            }
                            else
                            {
                                velocity = new Vector2(-(DashForce.X), -DashForce.Y);
                            }


                            if (DashForce.Y != 0)
                                position.Y -= 1f;
                            //if (prevVel != Velocity)
                            //{
                            //    accel = force / mass;
                            //}

                        }

                        if (kb.IsKeyDown(Keys.Space) && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;

                            if (animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Jump;
                                ChangeAnimation();
                            }
                            prevKb = kb;

                            velocity.Y = -jumpForce;

                            
                                position.Y -= 1f;

                            if (prevVel != Velocity)
                            {
                                accel = force / mass;
                            }
                            canJump = false;
                        }
                        break;
                    #endregion
                }
            }

            prevKb = kb;
        }

        public void Collision(Rectangle newRect /*int xOffset, int yOffset, int levelInX, int levelInY, Rectangle bounds*/)
        {
            isColliding = false;
            //blockBottom = false;
            if (playerRect.TouchTopOf(newRect))
            {


                if (isFalling)
                {
                    while (playerRect.Bottom > newRect.Top)
                    {
                        velocity.Y += -(Velocity.Y);
                        position.Y -= .1f;
                        playerRect.Y = (int)position.Y;
                    }

                    jumpDelay = 0;
                    //velocity.X += (float)Math.Cos(velocity.X);
                    isFalling = false;
                    
                    //isDashing = false;
                    animState = AnimationStates.Idle;
                    ChangeAnimation();
                    //maxVelocity.X = 6;
                    prevKb = Keyboard.GetState();
                    isColliding = false;
                }
                else if (playerState == PlayerStates.Jumping)
                {
                    if (velocity.X > 0)
                    {
                        velocity.X = 0;
                        position.X += -moveSpeed;
                    }
                    else if (velocity.X < 0)
                    {
                        velocity.X = 0;
                        position.X += moveSpeed;
                    }

                }
                else
                {
                    while (playerRect.Bottom > newRect.Top)
                    {
                        velocity.Y += -(Velocity.Y);
                        position.Y -= 1f;
                        playerRect.Y = (int)position.Y;
                    }
                    jumpDelay++;
                    if (jumpDelay >= maxJumpDelay)
                    {
                        canJump = true;
                    }
                }



                //position.Y += -(velocity.Y);


                //isColliding = true;
                blockBottom = true;


            }

            if (playerRect.TouchLeftOf(newRect))
            {

                if (isFalling || playerState == PlayerStates.Jumping)
                {
                    while (playerRect.Right > newRect.Left)
                    {
                        position.X -= .01f;
                        playerRect.X = (int)position.X;
                    }
                    if (velocity.X >= 0)
                    {
                        velocity.X = 0;
                        position.X += -moveSpeed;
                    }
                    isColliding = true;
                }
                else
                {
                    while (playerRect.Right > newRect.Left)
                    {
                        position.X -= .01f;
                        playerRect.X = (int)position.X;
                    }
                    if (velocity.X >= 0)
                    {
                        velocity.X = 0;
                        position.X += -moveSpeed * 2;
                    }
                    //position.X += -velocity.X;
                    //velocity.X = -2;

                }

            }
            if (playerRect.TouchRightOf(newRect))
            {
                if (isFalling || playerState == PlayerStates.Jumping)
                {
                    while (playerRect.Left < newRect.Right)
                    {
                        position.X += .01f;
                        playerRect.X = (int)position.X;
                    }
                    if (velocity.X < 0)
                    {
                        velocity.X = 0;
                        position.X += moveSpeed;
                    }
                }
                else
                {
                    while (playerRect.Left < newRect.Right)
                    {
                        position.X += .01f;
                        playerRect.X = (int)position.X;
                    }
                    if (velocity.X <= 0)
                    {
                        velocity.X = 0;
                        position.X += .1f;
                    }
                    //position.X += -velocity.X;
                    //velocity.X = +2;
                }



                isColliding = true;
            }
            if (playerRect.TouchBottomOf(newRect))
            {

                if (isFalling || playerState == PlayerStates.Jumping)
                {
                    while (playerRect.Top < newRect.Bottom)
                    {
                        velocity.Y = 0;
                        position.Y += .01f;
                        playerRect.Y = (int)position.Y;
                    }

                    prevKb = Keyboard.GetState();
                }
                else
                {
                    while (playerRect.Top < newRect.Bottom)
                    {
                        velocity.Y = 0;
                        position.Y += .01f;
                        playerRect.Y = (int)position.Y;
                    }
                }



                //position.Y += -(velocity.Y);


                isColliding = true;
            }

            if (animManager.isLeft)
            {
                playerRect = new Rectangle((int)(position.X + (collisionOffsetX - 6)), (int)position.Y, pixelSize / 2, pixelSize);
            }
            else if (animManager.isRight)
            {
                playerRect = new Rectangle((int)(position.X + collisionOffsetX), (int)position.Y, pixelSize / 2, pixelSize);
            }
    


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //if(animManager.isLeft)
            //{
            //    spriteBatch.Draw(texture,position:position ,sourceRectangle: playerRect, color: Color.White, effects: SpriteEffects.FlipHorizontally);
            //}
            //else if(animManager.isRight)
            //{
            //    spriteBatch.Draw(texture, playerRect, Color.White);
            //}
            spriteBatch.Draw(texture, playerRect, Color.White);
            animManager.Draw(spriteBatch);

            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}
