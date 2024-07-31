﻿using Microsoft.Xna.Framework;
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
        enum PilotAnimStates { Walking, Idle}
        PilotAnimStates pilotAnimState = PilotAnimStates.Idle;

        public enum PlayerStates { Movement, Shooting, Jumping, Dashing, Pilot}
        public PlayerStates playerState = PlayerStates.Movement;
        PlayerStates prevPlayerState;
        bool groundPound = false;
        public bool killEnemy = false;

        #region Fields
        float dashHelperBuffer = 10f;
        float collisionOffsetX = 20f;
        float collisionInputCooldown = .25f;
        int pixelSize = 64;
        Vector2 position = Vector2.Zero;
        Vector2 prevVel = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        public Rectangle playerRect;
        public Rectangle RoboRect;
        public bool isPilot = false;
        float jumpOutDelay = .75f;
        public bool isFalling = false;
        public bool isCollidingRight = false;
        public bool isCollidingLeft = false;
        Game1 game;
        KeyboardState prevKb;
        public bool blockBottom = false;

        float moveSpeed = 2.15f;
        float iMoveSpeed;
        float iMaxRunSpeed;
        //Controler input helpers
        Vector2 controllerMoveDir;
        GamePadButtons currControllerBtn;
        GamePadButtons prevControllerBtn;

        float health = 5f;
        public int redFrames = 4;
        public int redCount = 0;
        int whiteFrames = 10;
        int whiteCount = 0;
        public bool damaged = false;

        public List<Rectangle> breakTiles = new List<Rectangle>();
        public float Health
        {
            get
            {
                return health;
            }
            set
            {
                if(value < health)
                {
                    damaged = true;
                }
                damaged = true;
               
                health = value;
                if (health <= 0)
                    health = 0;
                if(health >= 5)
                {
                    health = 5;
                }
            }
        }

        float mass = 20.0f;
        public float accel = 0;
        public float force = 0;
        public float friction = 0;
        float coeFric = 0;
        public float changeInTime = 0;

        public float maxRunSpeed = 5.5f;
        float maxAirSpeed = 3.5f;
        float terminalVel = 12f;
        float groundPoundVel = 20f;
        float maxJumpSpeed = 8f;
        float maxDashSpeed = 22.5f;
        float maxDashAirSpeed = 15f;
        #endregion

        #region Shooting
        Texture2D gunTexture;
        List<Bullet> bullets = new List<Bullet>();
        MouseState prevMs;
        float bulletSpeed = 3.5f;
        float bulletMaxX = 15f;
        float bulletMaxY = 0;
        bool isShootDelay = false;
        float shootDelay = .8f;//In seconds
        float iShootDelay;
        bool startShoot = false;
        float bulletDmg = .65f;
        float bulletTravelDist = 64 * 3;
        WeaponWheel weaponWheel;

        enum WeaponType { Pistol, Shotgun, Laser, Burst}
        WeaponType currWeapon = WeaponType.Shotgun;
        #endregion

        #region Jumping
        float jumpForce = 18f;
        float iJumpF;
        int jumpDelay = 0;
        int maxJumpDelay = 3;
        public bool canJump = false;
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
        public float dashForceX = 25f;
        public float dashForceY = 0f;
        public bool canDash = true;
        float dashDistance = 3; //number of tiles that they can dash.
        Vector2 startDashPos = Vector2.Zero;
        //public bool isDashing = false;
        float dashCoolDown = 0;
        float dashCoolDownMax = 1.5f;
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

                if (pos.X > maxRunSpeed && velocity.Y == 0 && playerState != PlayerStates.Dashing)
                {
                    pos = new Vector2(maxRunSpeed, pos.Y);
                }
                if(pos.X > maxAirSpeed && velocity.Y != 0 && playerState != PlayerStates.Dashing)
                {
                    pos = new Vector2(maxAirSpeed, pos.Y);
                }
                if (pos.X > maxDashAirSpeed && playerState == PlayerStates.Dashing && isFalling)
                {
                    pos = new Vector2(maxDashAirSpeed, pos.Y);

                }
                if (pos.X > maxDashSpeed && playerState == PlayerStates.Dashing && !isFalling)
                {
                    pos = new Vector2(maxDashSpeed, pos.Y);
                }
             

                if (pos.X < -maxRunSpeed && velocity.Y == 0 && playerState != PlayerStates.Dashing)
                {
                    pos = new Vector2(-maxRunSpeed, pos.Y);
                }
                if (pos.X < -maxAirSpeed && velocity.Y != 0 && playerState != PlayerStates.Dashing)
                {
                    pos = new Vector2(-maxAirSpeed, pos.Y);
                }
                if (pos.X < -maxDashSpeed && playerState == PlayerStates.Dashing && !isFalling)
                {
                    pos = new Vector2(-maxDashSpeed, pos.Y);
                    //isDashing = false;
                }
                if (pos.X < -maxDashAirSpeed && playerState == PlayerStates.Dashing && isFalling)
                {
                    pos = new Vector2(-maxDashAirSpeed, pos.Y);
                }
       


                if (pos.Y > terminalVel && isFalling && !groundPound)
                {
                    pos = new Vector2(pos.X, terminalVel);
                }
                else if(pos.Y > groundPoundVel && isFalling && groundPound)
                {
                    pos = new Vector2(pos.X, groundPoundVel);
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
            get { return new Vector2(playerRect.X, playerRect.Y); }
            set { position = value; }
        }
        public Rectangle Rectangle
        {
            get { return playerRect; }
        }

        public Rectangle InteractionBox
        {
            get { 
               if(animManager.isRight)
               {
                    return new Rectangle(playerRect.Right, playerRect.Top + playerRect.Height/3, playerRect.Width/2, playerRect.Height/2);
               }
               else
                {
                    return new Rectangle(playerRect.Left - playerRect.Width/2, playerRect.Top + playerRect.Height / 3, playerRect.Width/2, playerRect.Height/2);
                }
            }
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
        public AnimationManager animManager;
        Texture2D texture;
        Point FrameSize;//Size of frame
        Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;
        public AnimationManager animManagerRobo;
        Texture2D textureRobo;
        Point FrameSizeRobo;//Size of frame
        Point CurrFrameRobo;//Location of currFram on the sheet
        Point SheetSizeRobo;//num of frames.xy
        int fpmsRobo;

        public void ChangeAnimation(bool onlyPilot = false)
        {
            
            if(!isPilot)
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
                    case AnimationStates.Shoot:
                        texture = content.Load<Texture2D>("SideScroll/Animations/PlayerShoot");
                        FrameSize = new Point(64, 64);
                        CurrFrame = new Point(0, 0);
                        SheetSize = new Point(3, 1);
                        fpms = 120;
                        break;
                }
            }
            else if(isPilot)
            {
                switch(animState)
                {
                    case AnimationStates.Idle:
                        texture = content.Load<Texture2D>("SideScroll/Animations/PilotIdle");
                        FrameSize = new Point(64, 64);
                        CurrFrame = new Point(0, 0);
                        SheetSize = new Point(8, 1);
                        fpms = 120;
                        break;
                    case AnimationStates.Walking:
                        texture = content.Load<Texture2D>("SideScroll/Animations/PilotRun");
                        FrameSize = new Point(64, 64);
                        CurrFrame = new Point(0, 0);
                        SheetSize = new Point(6, 1);
                        fpms = 120;
                        break;
                    default:
                        texture = content.Load<Texture2D>("SideScroll/Animations/PilotRun");
                        FrameSize = new Point(64, 64);
                        CurrFrame = new Point(0, 0);
                        SheetSize = new Point(6, 1);
                        fpms = 120;
                        break;

                }
                if(onlyPilot == false)
                {
                    if (textureRobo != content.Load<Texture2D>("SideScroll/Animations/PlayerIdle"))
                    {
                        textureRobo = content.Load<Texture2D>("SideScroll/Animations/PlayerIdle");
                        FrameSizeRobo = new Point(64, 64);
                        CurrFrameRobo = new Point(0, 0);
                        SheetSizeRobo = new Point(6, 1);
                        fpmsRobo = 120;


                    }
                    bool Right = true, Left = false, Up = false, Down = false;
                    if (animManagerRobo != null)
                    {
                        Right = animManagerRobo.isRight;
                        Left = animManagerRobo.isLeft;
                        Up = animManagerRobo.isUp;
                        Down = animManagerRobo.isDown;
                    }

                    animManagerRobo = new AnimationManager(textureRobo, FrameSizeRobo, CurrFrameRobo, SheetSizeRobo, fpmsRobo, new Vector2(RoboRect.X, RoboRect.Y));

                    animManagerRobo.isRight = Right;
                    animManagerRobo.isLeft = Left;
                    animManagerRobo.isUp = Up;
                    animManagerRobo.isDown = Down;
                }
              


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



        public void Load(ContentManager Content, Rectangle bounds, float friction, Vector2 pos)
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
            dashDistance = 3 * pixelSize;
            iMaxRunSpeed = maxRunSpeed;
            position = pos;
            weaponWheel = new WeaponWheel(this, 24);
            isCollidingRight = false;
            isCollidingLeft = false;
        }

        public void Update(GameTime gameTime, Vector2 gravity, List<SSEnemy> enemies, bool fade = false)
        {
            if(fade)
            {
                if(animState != AnimationStates.Idle)
                {
                    animState = AnimationStates.Idle;
                    ChangeAnimation();
                }
                animManager.Update(gameTime, position);
            }
            else
            {
                controllerMoveDir = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;

                currControllerBtn = GamePad.GetState(PlayerIndex.One).Buttons;


                if (isCollidingLeft && isCollidingRight)
                {
                    if (collisionInputCooldown <= 0)
                    {
                        isCollidingLeft = false;
                        isCollidingRight = false;
                        collisionInputCooldown = .15f;
                    }
                    else
                    {
                        collisionInputCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                else if (isCollidingLeft)
                {
                    if (collisionInputCooldown <= 0)
                    {
                        isCollidingLeft = false;
                        collisionInputCooldown = .15f;
                    }
                    else
                    {
                        collisionInputCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                else if (isCollidingRight)
                {
                    if (collisionInputCooldown <= 0)
                    {
                        isCollidingRight = false;
                        collisionInputCooldown = .15f;
                    }
                    else
                    {
                        collisionInputCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }

                Input(gameTime, enemies);
                if (Velocity == Vector2.Zero && !isFalling && playerState != PlayerStates.Shooting)
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

                        //if (prevPlayerState == PlayerStates.Dashing)
                        //{
                        //    //Slow down over time instead of instant set to run speed
                        //    maxRunSpeed -= moveSpeed * force;
                        //    if(maxRunSpeed < iMaxRunSpeed)
                        //    {
                        //        maxRunSpeed = iMaxRunSpeed;
                        //        prevPlayerState = PlayerStates.Movement;
                        //    }
                        //}
                        //else
                        //{

                        //}
                        maxRunSpeed = iMaxRunSpeed;

                        if (isFalling)
                        {
                            velocity.Y += gravity.Y;
                            //moveSpeed = fallMoveSpeed;

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
                        if (isFalling)
                        {
                            maxRunSpeed = maxDashAirSpeed;
                        }
                        else
                        {
                            maxRunSpeed = maxDashSpeed;
                        }
                        if (MathHelper.Distance(startDashPos.X, Position.X) >= dashDistance)
                        {
                            //prevPlayerState = playerState; //uncomment for dash testing with force 
                            playerState = PlayerStates.Movement;
                        }
                        //if (Velocity.X >= maxDashAirSpeed && isFalling && Velocity.X > 0)
                        //{
                        //    prevPlayerState = playerState;
                        //    playerState = PlayerStates.Movement;
                        //}
                        //else if (Velocity.X <= -maxDashAirSpeed && isFalling && Velocity.X < 0)
                        //{
                        //    prevPlayerState = playerState;
                        //    playerState = PlayerStates.Movement;
                        //}
                        //if (Velocity.X >= maxDashSpeed && !isFalling && Velocity.X > 0)
                        //{
                        //    prevPlayerState = playerState;
                        //    playerState = PlayerStates.Movement;
                        //}
                        //else if (Velocity.X <= -maxDashSpeed && isFalling && Velocity.X < 0)
                        //{
                        //    prevPlayerState = playerState;
                        //    playerState = PlayerStates.Movement;
                        //}
                        break;
                    #endregion

                    #region Jumping
                    case PlayerStates.Jumping:
                        if (Velocity.Y <= 0)
                        {
                            if (Velocity.Y >= 0)
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

                    #region Shooting
                    case PlayerStates.Shooting:
                        if (animManager.GetCurrFrame().X >= animManager.GetSheetSize().X - 1)
                        {
                            animManager.StopLoop();
                            startShoot = true;
                        }
                        else
                            startShoot = false;

                        if (startShoot)
                        {
                            Input(gameTime, enemies);
                        }
                        break;
                    #endregion
                    #region Pilot
                    case PlayerStates.Pilot:

                        BecomePilot();
                        playerState = PlayerStates.Movement;

                        break;
                        #endregion
                }



                position += Velocity;
                //playerRect = new Rectangle((int)(position.X + (collisionOffsetX - 6)), (int)position.Y, pixelSize / 2, pixelSize);
                // playerRect = new Rectangle()
                //if(!isPilot)
                //{
                //    animManager.Update(gameTime, position);

                //}
                //else
                //{  
                //    animState = AnimationStates.Idle;
                //    ChangeAnimation();
                //    animManager.isRight = true;
                //    animManager.isLeft = false;
                //    animManager.Update(gameTime, new Vector2(RoboRect.X, RoboRect.Y));
                //}
                animManager.Update(gameTime, position);
                if (isPilot && animManagerRobo != null)
                {
                    animManagerRobo.Update(gameTime, new Vector2(RoboRect.X, RoboRect.Y));
                }

                if (bullets.Count != 0)
                {
                    for (int i = bullets.Count - 1; i >= 0; i--)
                    {
                        bullets[i].Update();
                        if (bullets[i].delete)
                        {
                            bullets.RemoveAt(i);
                            break;
                        }
                        foreach (SSEnemy enemy in enemies)
                        {
                            if (bullets[i].rect.TouchBottomOf(enemy.enemyRect) || bullets[i].rect.TouchTopOf(enemy.enemyRect)
                            || bullets[i].rect.TouchLeftOf(enemy.enemyRect) || bullets[i].rect.TouchRightOf(enemy.enemyRect))
                            {
                                enemy.Health -= bulletDmg;
                                bullets.RemoveAt(i);
                                break;
                            }
                        }

                    }
                }

                //switch (playerState)
                //{
                //    case PlayerStates.Movement:

                //        break;
                //}
            }

        }

        private void BecomePilot()
        {
            isPilot = true;
            maxAirSpeed /= 2;
            moveSpeed /= 2;
            maxDashAirSpeed /= 1.5f;
            maxJumpSpeed /= 2f;
            maxRunSpeed /= 2f;
            bulletTravelDist /= 2f;
            //position.Y -= 128;
            //jumpForce /= 1.5f;
            dashDistance /= 2;
            RoboRect = new Rectangle(playerRect.X, playerRect.Y, 30, 60);
            ChangeAnimation();
        }
        private void BecomeRobo()
        {
            isPilot = false;
            maxAirSpeed *= 2;
            moveSpeed *= 2;
            maxDashAirSpeed *= 1.5f;
            maxJumpSpeed *= 2f;
            maxRunSpeed *= 2f;
            bulletTravelDist *= 2f;

            //jumpForce /= 1.5f;
            dashDistance = 3 * pixelSize;

            playerRect = RoboRect;
            ChangeAnimation();
        }

        private void Input(GameTime gameTime, List<SSEnemy> enemies)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

           
            //if(playerState == PlayerStates.Pilot)

            if(kb.IsKeyDown(Keys.D) && kb.IsKeyDown(Keys.A) /*&& !isColliding*/)
            {
                if (playerState == PlayerStates.Shooting)
                {
                    playerState = PlayerStates.Movement;
                }
                switch (playerState)
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
                                if(isPilot)
                                {
                                    ChangeAnimation(true);
                                }
                                else
                                {
                                    ChangeAnimation();
                                }
                                
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
                                if (isPilot)
                                {
                                    ChangeAnimation(true);
                                }
                                else
                                {
                                    ChangeAnimation();
                                }
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

                        if(kb.IsKeyDown(Keys.LeftShift) && canDash || currControllerBtn.B == ButtonState.Pressed && canDash || currControllerBtn.LeftShoulder == ButtonState.Pressed && canDash)
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

                            startDashPos = Position;
                        }

                        if(kb.IsKeyDown(Keys.Space) && canJump && !isFalling || currControllerBtn.A == ButtonState.Pressed && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;

                            if (animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Jump;
                                if (isPilot)
                                {
                                    ChangeAnimation(true);
                                }
                                else
                                {
                                    ChangeAnimation();
                                }
                            }
                            prevKb = kb;
                            foreach(SSEnemy enemy in enemies)
                            {
                                enemy.leftOnX.Add(playerRect.X);
                            }
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
                    case PlayerStates.Jumping:

                        break;
                }
            }
            else if(kb.IsKeyDown(Keys.D) && !isCollidingRight || controllerMoveDir.X > 0  && !isCollidingRight && controllerMoveDir.Y > -.9)
            {
                isCollidingLeft = false;
                if (playerState == PlayerStates.Shooting)
                {
                    playerState = PlayerStates.Movement;
                }
                switch (playerState)
                {
                    #region Movement
                    case PlayerStates.Movement:
                        if (animState != AnimationStates.Walking && animState != AnimationStates.Jump || animState != AnimationStates.Walking && blockBottom)
                        {
                            animState = AnimationStates.Walking;
                            if (isPilot)
                            {
                                ChangeAnimation(true);
                            }
                            else
                            {
                                ChangeAnimation();
                            }
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

                        if(kb.IsKeyDown(Keys.LeftShift) && canDash || currControllerBtn.B == ButtonState.Pressed && canDash || currControllerBtn.LeftShoulder == ButtonState.Pressed && canDash)
                        {
                            playerState = PlayerStates.Dashing;
                            prevKb = kb;

                            velocity = new Vector2(DashForce.X, -DashForce.Y);
                            position.Y -= 1f;

                            startDashPos = Position;
                        }

                        if (kb.IsKeyDown(Keys.Space) && canJump && !isFalling || currControllerBtn.A == ButtonState.Pressed && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;

                            if (animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Jump;
                                if (isPilot)
                                {
                                    ChangeAnimation(true);
                                }
                                else
                                {
                                    ChangeAnimation();
                                }
                            }
                            prevKb = kb;

                            velocity.Y = -jumpForce;
                            foreach (SSEnemy enemy in enemies)
                            {
                                enemy.leftOnX.Add(playerRect.X);
                            }
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
                    #region Jumping
                    case PlayerStates.Jumping:
                        if (Velocity.X >= 0)
                        {
                            velocity.X += moveSpeed;
                        }
                        else
                        {
                            velocity.X = -Velocity.X / 1.5f;//Can change to turn with exact momentum   
                        }
                        break;
                    #endregion
                }
            }
            else if(kb.IsKeyDown(Keys.A) && !isCollidingLeft/*&& !isColliding*/ || controllerMoveDir.X < 0 && !isCollidingLeft && controllerMoveDir.Y > -.9)
            {
                isCollidingRight = false;
                if (playerState == PlayerStates.Shooting)
                {
                    playerState = PlayerStates.Movement;
                }
                switch (playerState)
                {
                    #region Movement
                    case PlayerStates.Movement:

                        animManager.isRight = false;
                        animManager.isLeft = true;
                        if (animState != AnimationStates.Walking && animState != AnimationStates.Jump || animState != AnimationStates.Walking && blockBottom)
                        {
                            animState = AnimationStates.Walking;
                            if (isPilot)
                            {
                                ChangeAnimation(true);
                            }
                            else
                            {
                                ChangeAnimation();
                            }
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

                        if(kb.IsKeyDown(Keys.LeftShift) && canDash || currControllerBtn.B == ButtonState.Pressed && canDash || currControllerBtn.LeftShoulder == ButtonState.Pressed && canDash)
                        {
                            playerState = PlayerStates.Dashing;
                            prevKb = kb;

                            velocity = new Vector2(-(DashForce.X), -DashForce.Y);

                            position.Y -= 1f;

                            startDashPos = Position;
                        }

                        if (kb.IsKeyDown(Keys.Space) && canJump && !isFalling || currControllerBtn.A == ButtonState.Pressed && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;

                            if (animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Jump;
                                if (isPilot)
                                {
                                    ChangeAnimation(true);
                                }
                                else
                                {
                                    ChangeAnimation();
                                }
                            }
                            prevKb = kb;

                            velocity.Y = -jumpForce;
                            foreach (SSEnemy enemy in enemies)
                            {
                                enemy.leftOnX.Add(playerRect.X);
                            }

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
                    #region Jumping
                    case PlayerStates.Jumping:
                        if (Velocity.X <= 0)
                        {
                            velocity.X -= moveSpeed;
                        }
                        else
                        {
                            velocity.X = -Velocity.X / 1.5f;//Can change to turn with exact momentum   
                        }
                        break;
                        #endregion
                }
            }
            else if(kb.IsKeyUp(Keys.D) && kb.IsKeyUp(Keys.A) || controllerMoveDir == Vector2.Zero)
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
                                if (isPilot)
                                {
                                    ChangeAnimation(true);
                                }
                                else
                                {
                                    ChangeAnimation();
                                }
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
                                if (isPilot)
                                {
                                    ChangeAnimation(true);
                                }
                                else
                                {
                                    ChangeAnimation();
                                }
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
                        else if (velocity.X == 0)
                        {
                            if(animState != AnimationStates.Idle && isPilot)
                            {
                                if(animManagerRobo != null)
                                {
                                    animState = AnimationStates.Idle;
                                    ChangeAnimation();
                                }
                            }
                            else if(animState != AnimationStates.Idle && !isPilot)
                            {
                                animState = AnimationStates.Idle;
                                ChangeAnimation();
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

                        if ((kb.IsKeyDown(Keys.LeftShift)) && canDash || currControllerBtn.B == ButtonState.Pressed && canDash || currControllerBtn.LeftShoulder == ButtonState.Pressed && canDash)
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

                                position.Y -= 1f;

                            startDashPos = Position;
                            //if (prevVel != Velocity)
                            //{
                            //    accel = force / mass;
                            //}

                        }

                        if (kb.IsKeyDown(Keys.Space) && canJump && !isFalling || currControllerBtn.A == ButtonState.Pressed && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;

                            if (animState != AnimationStates.Jump)
                            {
                                animState = AnimationStates.Jump;
                                if (isPilot)
                                {
                                    ChangeAnimation(true);
                                }
                                else
                                {
                                    ChangeAnimation();
                                }
                            }
                            prevKb = kb;

                            velocity.Y = -jumpForce;
                            foreach (SSEnemy enemy in enemies)
                            {
                                enemy.leftOnX.Add(playerRect.X);
                            }

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

            if(kb.IsKeyDown(Keys.E) && blockBottom && prevKb.IsKeyUp(Keys.E) || currControllerBtn.Y == ButtonState.Pressed && prevControllerBtn.Y == ButtonState.Released && blockBottom)
            {
                foreach(BottomDoorTile doorTile in SideTileMap.BottomDoorTiles)
                {
                    if (InteractionBox.Intersects(doorTile.Rectangle))
                    {
                        game.OpenDoor(doorTile.Rectangle);
                        isCollidingLeft = false;
                        isCollidingRight = false;
                    }
                }
                foreach(DungeonEntrance dungeonEntrance in SideTileMap.DungeonEntrances)
                {
                    if(InteractionBox.Intersects(dungeonEntrance.Rectangle))
                    {
                        game.StartDungeon();
                    }
                }
               // jumpOutDelay = .75f;
            }

            //Holding a button to become pilot
            //if (kb.IsKeyDown(Keys.E) && blockBottom && prevKb.IsKeyDown(Keys.E) && !isPilot
            //    || currControllerBtn.Y == ButtonState.Pressed && prevControllerBtn.Y == ButtonState.Pressed && blockBottom && !isPilot)
            //{
            //    jumpOutDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //    if(jumpOutDelay <= 0)
            //    {
            //        jumpOutDelay = .75f;
            //        playerState = PlayerStates.Pilot;
            //    }
            //}
            //else if(kb.IsKeyDown(Keys.E) && blockBottom && prevKb.IsKeyDown(Keys.E) && isPilot && playerRect.Intersects(RoboRect) //Holding button to go back to robo
            //    || currControllerBtn.Y == ButtonState.Pressed && prevControllerBtn.Y == ButtonState.Pressed && blockBottom && isPilot && playerRect.Intersects(RoboRect))
            //{
            //    jumpOutDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //    if(jumpOutDelay <= 0)
            //    {
            //        BecomeRobo();
            //        jumpOutDelay = .75f;
            //    }
            //}
            //else
            //{
            //    jumpOutDelay = .75f;
            //}


            if (kb.IsKeyDown(Keys.S) && playerState != PlayerStates.Shooting && blockBottom || controllerMoveDir.Y < -.9 && playerState != PlayerStates.Shooting && blockBottom)
            {
                weaponWheel = new WeaponWheel(this, 24);
                weaponWheel.active = true;
                playerState = PlayerStates.Shooting;

                if (velocity.Y != 0)
                {
                    if(velocity.Y < 0)
                    {
                        velocity.Y = 0.1f;
                    }
                }
                velocity.X = 0;

                animState = AnimationStates.Shoot;
                if (isPilot)
                {
                    ChangeAnimation(true);
                }
                else
                {
                    ChangeAnimation();
                }
                animManager.StartLoop();
            }
            else if(kb.IsKeyDown(Keys.S) && prevKb.IsKeyUp(Keys.S) && playerState != PlayerStates.Shooting && playerState != PlayerStates.Shooting && velocity.Y >= 0 && velocity.Y < 7 
                || controllerMoveDir.Y < -.9 && playerState != PlayerStates.Shooting && playerState != PlayerStates.Shooting && velocity.Y >= 0 && velocity.Y < 5)
            {
                playerState = PlayerStates.Shooting;
                groundPound = true;
                if(velocity.Y >= 0)
                {
                    velocity.Y = groundPoundVel;
                }
                velocity.X = 0;
                animState = AnimationStates.Shoot;
                if (isPilot)
                {
                    ChangeAnimation(true);
                }
                else
                {
                    ChangeAnimation();
                }
                animManager.StartLoop();
            }

            if (kb.IsKeyDown(Keys.S) && playerState == PlayerStates.Shooting && prevKb.IsKeyDown(Keys.S) && blockBottom || controllerMoveDir.Y < -.9 && playerState == PlayerStates.Shooting && blockBottom)
            {
                weaponWheel = new WeaponWheel(this, 24);
                weaponWheel.active = true;
                playerState = PlayerStates.Shooting;
                float fallSpeed = 2;
                if(velocity.Y > 0)
                {
                    velocity.Y += fallSpeed;
                }

                if(kb.IsKeyDown(Keys.Enter) && prevKb.IsKeyUp(Keys.Enter)|| currControllerBtn.X == ButtonState.Pressed && prevControllerBtn.X == ButtonState.Released)
                {
                    if(animManager.isRight)
                    {
                        switch(currWeapon)
                        {
                            case WeaponType.Pistol:
                                bulletTravelDist = 64 * 3;
                                bulletSpeed = 3.5f;

                                bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width + (18 / 2), 
                                    position.Y + playerRect.Height / 1.5f), bulletSpeed, 
                                    new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                                break;

                            case WeaponType.Shotgun:
                                bulletTravelDist = 64 * 1f;
                                bulletSpeed = 3.5f * 2;

                                //Top 
                                bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width + (18 / 2),
                                   (position.Y + playerRect.Height / 1.5f)), bulletSpeed,
                                   new Vector2(bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed/3));
                                //Center
                                bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width + (18 / 2),
                                   position.Y + playerRect.Height / 1.5f), bulletSpeed,
                                   new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                                //Bottom
                                bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width + (18 / 2),
                                   position.Y + playerRect.Height / 1.5f), bulletSpeed,
                                   new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed/3));
                                break;


                        }
                        
                    }
                    if(animManager.isLeft)
                    {
                        switch(currWeapon)
                        {
                            case WeaponType.Pistol:
                                bulletTravelDist = 64 * 3;
                                bulletSpeed = 3.5f;
                                bullets.Add(new Bullet(new Vector2(position.X - (18 / 2), 
                                    position.Y + playerRect.Height / 1.5f), -bulletSpeed, 
                                    new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                                break;
                            case WeaponType.Shotgun:
                                bulletTravelDist = 64 * 1f;
                                bulletSpeed = 3.5f * 2;

                                //Top 
                                bullets.Add(new Bullet(new Vector2(position.X + (18/2),
                                   (position.Y + playerRect.Height / 1.5f)), -bulletSpeed,
                                   new Vector2(-bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 3));
                                //Center
                                bullets.Add(new Bullet(new Vector2(position.X + (18/2),
                                   position.Y + playerRect.Height / 1.5f), -bulletSpeed,
                                   new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                                //Bottom
                                bullets.Add(new Bullet(new Vector2(position.X + (18/2),
                                   position.Y + playerRect.Height / 1.5f), -bulletSpeed,
                                   new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 3));
                                break;
                        }
                        
                    }
                    
                }
               
            }
            else if(playerState == PlayerStates.Shooting)
            {
                playerState = PlayerStates.Movement;
            }
            int num = kb.GetPressedKeys().Count<Keys>();
            if (kb.IsKeyUp(Keys.S) && controllerMoveDir.Y > -.9)
            {
                weaponWheel.active = false;
            }
            prevKb = kb;
            prevControllerBtn = currControllerBtn;
        }

        public void Collision(Rectangle newRect /*int xOffset, int yOffset, int levelInX, int levelInY, Rectangle bounds*/, bool isEnemy = false)
        {
            //isColliding = false;
            //blockBottom = false;


            if (!isEnemy)
            {
                if (playerRect.TouchTopOf(newRect))
                {


                    if (velocity.Y > 0 && playerState != PlayerStates.Dashing)
                    {
                        while (playerRect.Bottom > newRect.Top - 1)
                        {
                            velocity.Y += -(Velocity.Y);
                            position.Y -= .1f;
                            playerRect.Y = (int)position.Y;
                        }
                        //if (velocity.X > 0)
                        //{
                        //    velocity.X = 0;
                        //    position.X += -moveSpeed;
                        //}
                        //else if (velocity.X < 0)
                        //{
                        //    velocity.X = 0;
                        //    position.X += moveSpeed;
                        //}
                        jumpDelay = 0;
                        //velocity.X += (float)Math.Cos(velocity.X);
                        isFalling = false;
                        //if (groundPound)
                        //{
                        //    if (SideTileMap.GetPoint((int)(newRect.Y / 64), (int)(newRect.X / 64)) == 26)
                        //    {
                        //        breakTiles.Add(newRect);
                        //    }
                        //}
                        groundPound = false;
                        //isDashing = false;

                      

                        //animState = AnimationStates.Idle;
                        //ChangeAnimation();
                        //maxVelocity.X = 6;
                        prevKb = Keyboard.GetState();
                        //isColliding = false;
                        blockBottom = true;
                    }
                    else if (playerState == PlayerStates.Jumping)
                    {
                        //if (velocity.X > 0)
                        //{
                        //    velocity.X = 0;
                        //    position.X += -moveSpeed;
                        //}
                        //else if (velocity.X < 0)
                        //{
                        //    velocity.X = 0;
                        //    position.X += moveSpeed;
                        //}

                    }
                    else if (playerState == PlayerStates.Dashing)
                    {

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
                        blockBottom = true;
                    }



                    //position.Y += -(velocity.Y);


                    //isColliding = true;



                }

                //if (playerRect.TouchTopOf(newRect) && playerRect.TouchLeftOf(newRect) || playerRect.TouchTopOf(newRect) && playerRect.TouchLeftOf(newRect))
                //{
                //    velocity.X = 0;
                //}
            }
            else if(isEnemy)
            {
                //if (playerRect.TouchTopOf(newRect))
                //{
                //    velocity.X = 0;
                //}
                if (playerRect.TouchTopOf(newRect))
                {


                    if (velocity.Y > 0 && playerState != PlayerStates.Dashing)
                    {
                        while (playerRect.Bottom > newRect.Top - 1)
                        {
                            velocity.Y += -(Velocity.Y);
                            position.Y -= .1f;
                            playerRect.Y = (int)position.Y;
                        }
                    }

                    if(groundPound)
                    {
                        killEnemy = true;

                    }
                }


            }

                if (playerRect.TouchLeftOf(newRect, isPilot))
            {
                while (playerRect.Right > newRect.Left)
                {
                    playerRect.X -= 1;
                    //position.X = playerRect.X;
                }
                //if (velocity.X > 0)
                //{
                //    position.X += -velocity.X;
                //    velocity.X = 0;

                //}
                switch (playerState)
                {
                    case PlayerStates.Movement:
                        if (velocity.X > 0)
                        {
                            position.X += -velocity.X;


                        }
                        velocity.X = 0;
                        //velocity.X += -Velocity.X * (moveSpeed * 2);
                        isCollidingRight = true;
                        break;
                    case PlayerStates.Jumping:
                        if (velocity.X > 0)
                        {
                            position.X += -velocity.X;
                            //velocity.X = 0;

                        }
                        break;
                    case PlayerStates.Dashing:
                        if (velocity.X > 0)
                        {
                            position.X += -velocity.X;
                           

                        }
                        //velocity.X = 0;
                        //velocity.X += -Velocity.X * (moveSpeed * 2);
                        isCollidingRight = true;
                        playerState = PlayerStates.Movement;
                        break;
                }
                //animManager.isLeft = true;
                //animManager.isRight = false;
                #region Comments
                //playerState = PlayerStates.Movement;
                //if (isFalling || playerState == PlayerStates.Jumping)
                //{
                //    while (playerRect.Right > newRect.Left)
                //    {
                //        playerRect.X -= 1;
                //    }
                //    if (velocity.X > 0)
                //    {
                //        position.X += -velocity.X;
                //        velocity.X = 0;

                //    }
                //    isColliding = true;
                //}
                //else
                //{

                //    while (playerRect.Right > newRect.Left)
                //    {
                //        playerRect.X -= 1;
                //    }
                //    if (velocity.X > 0)
                //    {
                //        position.X += -velocity.X;
                //        velocity.X = 0;

                //    }
                //    //position.X += -velocity.X;
                //    //velocity.X = -2;

                //}

                //switch (playerState)
                //{
                //    case PlayerStates.Dashing:
                //        playerState = PlayerStates.Movement;

                //        velocity.Y = (float)Math.Sin(Force) * dashHelperBuffer;
                //        break;
                //}

                #endregion


            }
            if (playerRect.TouchRightOf(newRect, isPilot))
            {
                while (playerRect.Left < newRect.Right)
                {
                    playerRect.X += 1;
                    //position.X = playerRect.X;
                }
                //if (velocity.X > 0)
                //{
                //    position.X += -velocity.X;
                //    velocity.X = 0;

                //}
                switch (playerState)
                {
                    case PlayerStates.Movement:
                        if (velocity.X < 0)
                        {
                            position.X += -velocity.X;


                        }
                        velocity.X = 0;
                        //velocity.X += -Velocity.X * (moveSpeed * 2);
                        isCollidingLeft = true;
                        break;
                    case PlayerStates.Jumping:
                        if (velocity.X < 0)
                        {
                            position.X += -velocity.X;

                        }
                        break;
                    case PlayerStates.Dashing:
                        if (velocity.X < 0)
                        {
                            position.X += -velocity.X;
                            //velocity.X = 0;

                        }
                        velocity.X = 0;
                        //velocity.X += -Velocity.X * (moveSpeed * 2);
                        isCollidingLeft = true;
                        playerState = PlayerStates.Movement;
                        break;
                }
                //animManager.isLeft = false;
                //animManager.isRight = true;
                if(isEnemy)
                {
                    velocity.X = 0;
                }
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

                //blockBottom = false;
                isCollidingRight = true;
            }

            if (animManager.isLeft && !isPilot)
            {
                playerRect = new Rectangle((int)(position.X + (collisionOffsetX)), (int)position.Y, pixelSize / 2, pixelSize);
            }
            else if(animManager.isLeft && isPilot)
            {
                playerRect = new Rectangle((int)(position.X + (collisionOffsetX)), (int)position.Y, 20, pixelSize);
            }
            if (animManager.isRight && !isPilot)
            {
                playerRect = new Rectangle((int)(position.X + (collisionOffsetX)), (int)position.Y, pixelSize / 2, pixelSize);
            }
            else if(animManager.isRight && isPilot)
            {
                playerRect = new Rectangle((int)(position.X + (collisionOffsetX)), (int)position.Y, 20, pixelSize);
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

           //spriteBatch.Draw(texture, RoboRect, Color.White);
           //spriteBatch.Draw(texture, playerRect, Color.White);
           //if(groundPound)
           //     spriteBatch.Draw(texture, GroundPoundRect, Color.White);
            if(damaged)
            {
                if(redCount <= whiteCount || redCount == 0 && whiteCount == 0)
                {
                    animManager.Draw(spriteBatch, Color.Red);
                    redCount++;
                }
                if(whiteCount < redCount)
                {
                    animManager.Draw(spriteBatch, Color.White * .5f);
                    whiteCount++;
                }
                if(whiteCount == whiteFrames)
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
            if (isPilot && animManagerRobo != null)
                animManagerRobo.Draw(spriteBatch, Color.White);
            //spriteBatch.Draw(texture, InteractionBox, Color.White);
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }

            if(weaponWheel.active)
            {
                foreach(WeaponSlot slot in weaponWheel.WeaponSlots)
                {
                    spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), slot.rect, Color.White * .35f);
                }
            }
        }
    }

    class WeaponWheel
    {
        public List<WeaponSlot> WeaponSlots = new List<WeaponSlot>();
        public bool active;
        int size;
        public WeaponWheel(SSPlayer player, int size)
        {
            WeaponSlots = new List<WeaponSlot>();
            active = false;
            this.size = size;
            for(int i = 0; i < 5; i++)
            {
                WeaponSlots.Add(new WeaponSlot());
              
            }
            WeaponSlots[0].rect = new Rectangle(player.playerRect.Center.X - size/2, 
                (int)(player.playerRect.Center.Y - (size * 3f)), size, size); //Center
            WeaponSlots[1].rect = new Rectangle(WeaponSlots[0].rect.X - (size + 2),
                WeaponSlots[0].rect.Y, size, size);//Left
            WeaponSlots[2].rect = new Rectangle(WeaponSlots[0].rect.Right + 2,
                WeaponSlots[0].rect.Y, size, size);//Right
            WeaponSlots[3].rect = new Rectangle(WeaponSlots[0].rect.X,
                WeaponSlots[0].rect.Y - (size + 2), size, size);//Top
            WeaponSlots[4].rect = new Rectangle(player.playerRect.Center.X - size / 2,
                WeaponSlots[0].rect.Bottom + 2, size, size);//Bottom

        }
        public void Update(SSPlayer player)
        {
            WeaponSlots[0].rect = new Rectangle(player.playerRect.Center.X - size / 2,
               player.playerRect.Center.Y - size, size, size);
            WeaponSlots[1].rect = new Rectangle(WeaponSlots[0].rect.X - (size + 2),
                WeaponSlots[0].rect.Y, size, size);
            WeaponSlots[2].rect = new Rectangle(WeaponSlots[0].rect.Right + 2,
                WeaponSlots[0].rect.Y, size, size);
            WeaponSlots[3].rect = new Rectangle(WeaponSlots[0].rect.X,
                WeaponSlots[0].rect.Y - (size + 2), size, size);
        }
    }

    class WeaponSlot
    {
        public Rectangle rect;
        public Texture2D texture;
    }
}
