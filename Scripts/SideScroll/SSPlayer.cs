using AUTO_Matic.Scripts;
using AUTO_Matic.Scripts.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.Scripts.Effects;

namespace AUTO_Matic.SideScroll
{
    class SSPlayer
    {
        ContentManager content;
        Game1 gameData;

        //Player States
        enum AnimationStates { Walking, Death, Idle, Jump, Shoot, Dash }
        AnimationStates animState = AnimationStates.Idle;
        enum PilotAnimStates { Walking, Idle}
        PilotAnimStates pilotAnimState = PilotAnimStates.Idle;

        public enum PlayerStates { Movement, Shooting, Jumping, Dashing, Pilot, Knockback }
        public PlayerStates playerState = PlayerStates.Movement;
        PlayerStates prevPlayerState;
        bool groundPound = false;
        public bool killEnemy = false;

        ParticleManager particles = new ParticleManager();
        SoundManager sounds;

        #region Fields
        float dashHelperBuffer = 10f;
        float collisionOffsetX = 20f;
        float collisionInputCooldown = .25f;
        int pixelSize = 64;
        public Vector2 position = Vector2.Zero;
        Vector2 prevVel = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        public Rectangle playerRect;
        public Rectangle RoboRect;
        public bool isPilot = false;
        float jumpOutDelay = .75f;
        public bool isFalling = false;
        public bool isCollidingRight = false;
        public bool isCollidingLeft = false;
        public bool isCollidingDown = false;
        Game1 game;
        KeyboardState prevKb;
        public bool blockBottom = false;

        public bool coyote = false;
        public float coyoteTime = .0834f;
        public float coyoteTimeMax = .0834f;

        float moveSpeed = 1.5f;
        float iMoveSpeed;
        float iMaxRunSpeed;
        //Controler input helpers
        Vector2 controllerMoveDir;
        GamePadButtons currControllerBtn;
        GamePadButtons prevControllerBtn;
        GamePadDPad prevDpad;

        float health = 10f;
        public int redFrames = 4;
        public int redCount = 0;
        int whiteFrames = 10;
        int whiteCount = 0;
        public bool damaged = false;

        public List<Rectangle> breakTiles = new List<Rectangle>();
        public float knockBackX;
        public float knockBackY;
        float gravX = 0;

        bool interactActive = false;
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
                //damaged = true;
               
                health = value;
                if (health <= 0)
                    health = 0;
                if(health >= 10)
                {
                    health = 10;
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
        UIManager KeyBindData;
        #endregion

        #region Shooting
        Texture2D gunTexture;
        public List<Bullet> bullets = new List<Bullet>();
        MouseState prevMs;
        float bulletSpeed = 3.5f;
        float bulletMaxX = 15f;
        float bulletMaxY = 0;
        bool isShootDelay = false;
        float shootDelay = 1.25f;//In seconds

        float pistolDelay = 0f;
        float maxPistolDelay = 1.55f;

        float burstDelay = .65f;
        float maxBurstDelay = .65f;

        float shotGunDelay = 2.45f;
        float maxShotgunDelay = 2.45f;

        float laserDelay = 2f;
        float maxLaserDelay = 2f;

        float bombDelay = 1f;
        float maxBombDelay = 1f;


        float iShootDelay;
        bool startShoot = false;
        public float bulletDmg = .85f;
        float pistolDmg = .85f;
        float shotGunDmg = 1.75f;
        float burstDmg = 1.35f;
        float bombDmg = 2.25f;
        float laserDmg = 1.65f;
        float bulletTravelDist = 64 * 3;
        WeaponWheel weaponWheel;
        int selectedWeapon = 0;

        public List<Bomb> bombs = new List<Bomb>();
        List<Explosion> explosions = new List<Explosion>();
        enum WeaponType { Pistol, Shotgun, Laser, Burst, Bomb}
        WeaponType currWeapon = WeaponType.Pistol;
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
        float dashCoolDownMax = .75f;
        bool isDashing = false;
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
                if(pos.X > maxAirSpeed && velocity.Y != 0 && playerState != PlayerStates.Dashing && !isCollidingDown)
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
                if (pos.X < -maxAirSpeed && velocity.Y != 0 && playerState != PlayerStates.Dashing && !isCollidingDown)
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
            get
            {
                if (animManager.isRight)
                {
                    return new Rectangle(playerRect.Right, playerRect.Top + playerRect.Height / 3, (int)(playerRect.Width / 1.5f), playerRect.Height / 2);
                }
                else
                {
                    return new Rectangle(playerRect.Left - playerRect.Width / 2, playerRect.Top + playerRect.Height / 3, (int)(playerRect.Width / 1.5f), playerRect.Height / 2);
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



        public void Load(ContentManager Content, Rectangle bounds, float friction, Vector2 pos, UIManager uiManager)
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
            iShootDelay = pistolDelay;
            shootDelay = 0;
            bombs.Clear();
            particles.Initialize(content.Load<Texture2D>("Textures/white"));
            KeyBindData = uiManager;
            sounds = new SoundManager("SoundEffects/Shoot", Content, false, KeyBindData.MasterVolume, KeyBindData.EffectVolume, KeyBindData.MusicVolume);

            sounds.ChangeVolume(uiManager.MasterVolume, uiManager.EffectVolume, uiManager.MusicVolume);
        }

        public void Update(GameTime gameTime, Vector2 gravity, List<SSEnemy> enemies,Game1 game,bool fade = false)
        {
            gameData = game;

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

                if (!canDash && playerState != PlayerStates.Dashing && !isDashing)
                {
                    dashCoolDown += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (dashCoolDown >= dashCoolDownMax && playerState != PlayerStates.Dashing)
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
                        isDashing = true;
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
                    #region Knockback
                    case PlayerStates.Knockback:
                        if (gravX == 0)
                        {
                            velocity = new Vector2(knockBackX, knockBackY);
                            if (knockBackX > 0)
                                gravX = -1;
                            else
                                gravX = 1;
                        }

                        foreach(WallTile wall in SideTileMap.WallTiles)
                        {
                            if(playerRect.Intersects(wall.Rectangle))
                            {
                                gravX = 0;
                                playerState = PlayerStates.Movement;
                            }
                        }
                        foreach(GroundTile ground in SideTileMap.GroundTiles)
                        {
                            if(playerRect.Intersects(ground.Rectangle))
                            {
                                gravX = 0;
                                playerState = PlayerStates.Movement;
                            }
                        }

                        if(!damaged)
                        {
                            gravX = 0;
                            playerState = PlayerStates.Movement;
                        }
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
                     
                        if (currWeapon == WeaponType.Burst)
                        {
                            for(int j = i - 1; j >= 0; j--)
                            {
                                if(bullets[j].rect.Intersects(bullets[i].rect))
                                {
                                    if (bullets[j].maxSpeed.X > 0)
                                        bullets[j].maxSpeed.X = bulletMaxX / 10;
                                    else
                                        bullets[j].maxSpeed.X = -bulletMaxX / 10;
                                }
                                else
                                {
                                    if (bullets[j].maxSpeed.X > 0)
                                        bullets[j].maxSpeed.X = bulletMaxX;
                                    else
                                        bullets[j].maxSpeed.X = -bulletMaxX;
                                }
                            }
                        }
                        else if(currWeapon == WeaponType.Laser)
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                if (bullets[j].rect.Intersects(bullets[i].rect))
                                {
                                    if (bullets[j].maxSpeed.X > 0)
                                        bullets[j].maxSpeed.X = bulletMaxX / 5;
                                    else
                                        bullets[j].maxSpeed.X = -bulletMaxX / 5;
                                }
                                else
                                {
                                    if (bullets[j].maxSpeed.X > 0)
                                        bullets[j].maxSpeed.X = bulletMaxX;
                                    else
                                        bullets[j].maxSpeed.X = -bulletMaxX;
                                }
                            }
                        }
                        bullets[i].Update(gameTime);
                        foreach (SSEnemy enemy in enemies)
                        {
                            if (bullets[i].rect.Intersects(enemy.enemyRect))
                            {
                                if (!enemy.damaged)
                                {
                                    enemy.healthBar.RecieveDamage(bulletDmg);
                                    enemy.Health -= bulletDmg;
                                    
                                    if(enemy.dead == false)
                                    {
                                        sounds.AddSound("SoundEffects/Enemy Hit_01", false);
                                        sounds.PlaySound();
                                    }
                                 
                                }
                                   
                               
                                
                                ApplyKnockback(enemy);

                                bullets[i].delete = true;
                                break;
                            }
                        }
                        if (bullets[i].delete)
                        {
                            bullets.RemoveAt(i);
                            break;
                        }
                       

                    }
                }

                for(int i = bombs.Count - 1; i >= 0; i--)
                {
                    bombs[i].Update(gameTime, gravity, enemies);

                    if(bombs[i].delete)
                    {
                        explosions.Add(new Explosion(bombs[i].circle, 2, (int)(bombs[i].circle.Bounds.Width * 2.5f)));

                 

                        int radiusDif = explosions[explosions.Count - 1].maxSize - explosions[explosions.Count - 1].rect.Radius;


                        particles.MakeExplosion(explosions[explosions.Count - 1].rect.Bounds,
                               new Circle(new Vector2(explosions[explosions.Count - 1].rect.Bounds.X - radiusDif,
                               explosions[explosions.Count - 1].rect.Bounds.Y - radiusDif), explosions[explosions.Count - 1].maxSize / 2),
                               20);

                        sounds.AddSound("SoundEffects/explosion", false);
                        sounds.PlaySound();

                        bombs.RemoveAt(i);
                    }
                }

                for(int i =explosions.Count - 1; i >=0; i--)
                {
                    explosions[i].Update(gameTime);

                    for(int j = enemies.Count - 1; j >= 0; j--)
                    {
                        if (explosions[i].rect.Intersects(enemies[j].enemyRect))
                        {
                            if (enemies[j].damaged == false)
                            {
                                enemies[j].healthBar.RecieveDamage(bulletDmg);
                                enemies[j].Health -= bulletDmg;
                                if(!enemies[j].dead)
                                {
                                    sounds.AddSound("SoundEffects/Enemy Hit_01", false);
                                    sounds.PlaySound();
                                }

                            }
                               
                           
                 

                            ApplyKnockback(enemies[j]);
                        }
                           
                    }
                    

                    if (explosions[i].rect.Radius >= explosions[i].maxSize)
                    {
                        //particles.CreateEffect(20);

                        explosions.RemoveAt(i);

                    }
                }

                bool active = false;
                foreach (DungeonEntrance dungeonEntrance in SideTileMap.DungeonEntrances)
                {
                    if (InteractionBox.Intersects(dungeonEntrance.Rectangle))
                    {
                        KeyBindData.CreateInteractUI(new Point(dungeonEntrance.Rectangle.X, dungeonEntrance.Rectangle.Y - 64));
                        interactActive = true;
                        active = true;
                        //if (game.GameState == Game1.GameStates.Tutorial)
                        //{
                        //    game.prevGameState = Game1.GameStates.Tutorial;
                        //    game.StartNewGame();
                        //    break;
                        //}
                        //else if (game.GetDungeonNum() >= game.bossKillCount)
                        //    game.StartDungeon();
                    }

                }
                if(interactActive && !active)
                {
                    KeyBindData.RemoveInteractUI();
                    interactActive = false;
                }
                particles.Update(gameTime);
                sounds.Update(gameTime);
                //switch (playerState)
                //{
                //    case PlayerStates.Movement:

                //        break;
                //}
            }

        }

        private void ApplyKnockback(SSEnemy enemy)
        {
            if(enemy.enemyState == SSEnemy.EnemyStates.Knockback)
            {

            }
            else
            {
                enemy.prevState = enemy.enemyState;
                enemy.enemyState = SSEnemy.EnemyStates.Knockback;

                switch (currWeapon)
                {
                    case WeaponType.Pistol:
                        enemy.knockBackX = 5;
                        enemy.knockBackY = -3;
                        break;
                    case WeaponType.Shotgun:
                        enemy.knockBackX = 8;
                        enemy.knockBackY = -3;
                        break;
                    case WeaponType.Laser:
                        enemy.knockBackX = 2;
                        enemy.knockBackY = -3;
                        break;
                    case WeaponType.Burst:
                        enemy.knockBackX = 6.5f;
                        enemy.knockBackY = -3;
                        break;
                    case WeaponType.Bomb:
                        enemy.knockBackX = 7;
                        enemy.knockBackY = -6;
                        break;
                }
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

            if(kb.IsKeyDown(KeyBindData.SideScrollInputs[1]) && kb.IsKeyDown(KeyBindData.SideScrollInputs[0]) /*&& !isColliding*/)
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

                        if(kb.IsKeyDown(KeyBindData.SideScrollInputs[4]) && canDash || currControllerBtn.B == ButtonState.Pressed && canDash || currControllerBtn.LeftShoulder == ButtonState.Pressed && canDash)
                        {
                            playerState = PlayerStates.Dashing;
                            sounds.AddSound("SoundEffects/AirDashSound", false);
                            sounds.PlaySound();
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

                        if(kb.IsKeyDown(KeyBindData.SideScrollInputs[5]) && canJump && !isFalling || currControllerBtn.A == ButtonState.Pressed && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;
                            sounds.AddSound("SoundEffects/jump", false);
                            sounds.PlaySound();
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
                                if (playerRect.Top/64 == enemy.enemyRect.Y/64)
                                {
                                    if (playerRect.Center.X < enemy.enemyRect.Center.X)
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.Right, playerRect.Y));
                                    }
                                    else if (playerRect.Center.X > enemy.enemyRect.Center.X)
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.X, playerRect.Y));
                                    }
                                    else
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.Center.X, playerRect.Y));
                                    }
                                }
                                    //enemy.leftOn.Add(new Vector2(playerRect.X, playerRect.Y));
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
            else if(kb.IsKeyDown(KeyBindData.SideScrollInputs[1]) && !isCollidingRight || controllerMoveDir.X > 0  && !isCollidingRight && controllerMoveDir.Y > -.9)
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

                        if(kb.IsKeyDown(KeyBindData.SideScrollInputs[4]) && canDash || currControllerBtn.B == ButtonState.Pressed && canDash || currControllerBtn.LeftShoulder == ButtonState.Pressed && canDash)
                        {
                            playerState = PlayerStates.Dashing;
                            prevKb = kb;

                            sounds.AddSound("SoundEffects/AirDashSound", false);
                            sounds.PlaySound();

                            velocity = new Vector2(DashForce.X, -DashForce.Y);
                            position.Y -= 1f;

                            startDashPos = Position;
                        }

                        if (kb.IsKeyDown(KeyBindData.SideScrollInputs[5]) && canJump && !isFalling || currControllerBtn.A == ButtonState.Pressed && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;
                            sounds.AddSound("SoundEffects/jump", false);
                            sounds.PlaySound();
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
                                if (playerRect.Top/64 == enemy.enemyRect.Y/64)
                                {
                                    if (playerRect.Center.X < enemy.enemyRect.Center.X)
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.Right, playerRect.Y));
                                    }
                                    else if (playerRect.Center.X > enemy.enemyRect.Center.X)
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.X, playerRect.Y));
                                    }
                                    else
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.Center.X, playerRect.Y));
                                    }
                                }
                                    //enemy.leftOn.Add(new Vector2(playerRect.X, playerRect.Y));
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
            else if(kb.IsKeyDown(KeyBindData.SideScrollInputs[0]) && !isCollidingLeft/*&& !isColliding*/ || controllerMoveDir.X < 0 && !isCollidingLeft && controllerMoveDir.Y > -.9)
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

                        if(kb.IsKeyDown(KeyBindData.SideScrollInputs[4]) && canDash || currControllerBtn.B == ButtonState.Pressed && canDash || currControllerBtn.LeftShoulder == ButtonState.Pressed && canDash)
                        {
                            playerState = PlayerStates.Dashing;
                            prevKb = kb;

                            sounds.AddSound("SoundEffects/AirDashSound", false);
                            sounds.PlaySound();

                            velocity = new Vector2(-(DashForce.X), -DashForce.Y);

                            position.Y -= 1f;

                            startDashPos = Position;
                        }

                        if (kb.IsKeyDown(KeyBindData.SideScrollInputs[5]) && canJump && !isFalling || currControllerBtn.A == ButtonState.Pressed && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;
                            sounds.AddSound("SoundEffects/jump", false);
                            sounds.PlaySound();
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
                                if (playerRect.Top/64 == enemy.enemyRect.Y/64)
                                {
                                    if (playerRect.Center.X < enemy.enemyRect.Center.X)
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.Right, playerRect.Y));
                                    }
                                    else if (playerRect.Center.X > enemy.enemyRect.Center.X)
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.X, playerRect.Y));
                                    }
                                    else
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.Center.X, playerRect.Y));
                                    }
                                }
                                    //enemy.leftOn.Add(new Vector2(playerRect.X, playerRect.Y));
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
            else if(kb.IsKeyUp(KeyBindData.SideScrollInputs[1]) && kb.IsKeyUp(KeyBindData.SideScrollInputs[0]) || controllerMoveDir == Vector2.Zero)
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
                                ChangeAnimation(isPilot);
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

                        if ((kb.IsKeyDown(KeyBindData.SideScrollInputs[4])) && canDash || currControllerBtn.B == ButtonState.Pressed && canDash || currControllerBtn.LeftShoulder == ButtonState.Pressed && canDash)
                        {

                            playerState = PlayerStates.Dashing;

                            sounds.AddSound("SoundEffects/AirDashSound", false);
                            sounds.PlaySound();

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

                        if (kb.IsKeyDown(KeyBindData.SideScrollInputs[5]) && canJump && !isFalling || currControllerBtn.A == ButtonState.Pressed && canJump && !isFalling)
                        {
                            playerState = PlayerStates.Jumping;
                            sounds.AddSound("SoundEffects/jump", false);
                            sounds.PlaySound();
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
                                if(playerRect.Top/64 == enemy.enemyRect.Y/64)
                                {
                                    if(playerRect.Center.X < enemy.enemyRect.Center.X)
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.Right, playerRect.Y));
                                    }
                                    else if(playerRect.Center.X > enemy.enemyRect.Center.X)
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.X, playerRect.Y));
                                    }
                                    else
                                    {
                                        enemy.leftOn.Add(new Vector2(playerRect.Center.X, playerRect.Y));
                                    }
                                   
                                }
                                   
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

            if(kb.IsKeyDown(KeyBindData.SideScrollInputs[6]) && blockBottom && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[6]) || currControllerBtn.Y == ButtonState.Pressed && prevControllerBtn.Y == ButtonState.Released && blockBottom)
            {
                foreach(BottomDoorTile doorTile in SideTileMap.BottomDoorTiles)
                {
                    if (InteractionBox.Intersects(doorTile.Rectangle))
                    {
                        game.OpenDoor(doorTile.Rectangle);
                        
                        isCollidingLeft = false;
                        isCollidingRight = false;
                        break;
                    }
                }

               // bool active = false;
                foreach(DungeonEntrance dungeonEntrance in SideTileMap.DungeonEntrances)
                {
                    if(InteractionBox.Intersects(dungeonEntrance.Rectangle))
                    {
                        //KeyBindData.CreateInteractUI(new Point(playerRect.X, playerRect.Y));
                        //active = true;
                        if(game.GameState == Game1.GameStates.Tutorial)
                        {
                            game.prevGameState = Game1.GameStates.Tutorial;
                            game.StartNewGame();
                            break;
                        }
                        else if(game.GetDungeonNum() >= game.bossKillCount)
                            game.StartDungeon();
                    }
                    
                }
                //if(!active && KeyBindData.uiElements.ContainsKey("InteractBox"))
                //{
                //    KeyBindData.RemoveInteractUI();
                //}
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


            if (kb.IsKeyDown(KeyBindData.SideScrollInputs[2]) && playerState != PlayerStates.Shooting && blockBottom || controllerMoveDir.Y < -.9 && playerState != PlayerStates.Shooting && blockBottom)
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
            else if(kb.IsKeyDown(KeyBindData.SideScrollInputs[2]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[2]) && playerState != PlayerStates.Shooting && playerState != PlayerStates.Shooting && velocity.Y >= 0 && velocity.Y < 7 
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

            if (kb.IsKeyDown(KeyBindData.SideScrollInputs[2]) && playerState == PlayerStates.Shooting && prevKb.IsKeyDown(KeyBindData.SideScrollInputs[2]) && blockBottom || controllerMoveDir.Y < -.9 && playerState == PlayerStates.Shooting && blockBottom)
            {
                weaponWheel = new WeaponWheel(this, 24);
                weaponWheel.active = true;
                playerState = PlayerStates.Shooting;
                float fallSpeed = 2;
                if(velocity.Y > 0)
                {
                    velocity.Y += fallSpeed;
                }
                float num1 = 180;
                switch (currWeapon)
                {
                    case WeaponType.Pistol:
                        pistolDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if(kb.IsKeyDown(KeyBindData.SideScrollInputs[3]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[3]) && pistolDelay <= 0 
                            || currControllerBtn.X == ButtonState.Pressed && prevControllerBtn.X == ButtonState.Released && pistolDelay <= 0)
                        {
                            if (animManager.isRight)
                            {
                                bulletTravelDist = 64 * 3;
                                bulletSpeed = 3.5f;

                                bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width,
                                    playerRect.Center.Y - (14 / 2)), bulletSpeed,
                                    new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                sounds.AddSound("SoundEffects/Shoot", false);
                                sounds.PlaySound();
                            }
                            else if(animManager.isLeft)
                            {
                                
                                bulletTravelDist = 64 * 3;
                                bulletSpeed = 3.5f;
                                bullets.Add(new Bullet(new Vector2(position.X/* - (18 / 2)*/,
                                    playerRect.Center.Y - (14 / 2)), -bulletSpeed,
                                    new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, angle: num1));
                                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                sounds.AddSound("SoundEffects/Shoot", false);
                                sounds.PlaySound();
                            }

                            pistolDelay = maxPistolDelay;
                        }
                        break;
                    case WeaponType.Shotgun:
                        shotGunDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (kb.IsKeyDown(KeyBindData.SideScrollInputs[3]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[3]) && shotGunDelay <= 0
                           || currControllerBtn.X == ButtonState.Pressed && prevControllerBtn.X == ButtonState.Released && shotGunDelay <= 0)
                        {
                            if(animManager.isRight)
                            {
                                bulletTravelDist = 64 * 1.25f;
                                bulletSpeed = 3.5f * 1.25f;
                                sounds.AddSound("SoundEffects/Shoot", false, -1);
                                sounds.PlaySound();
                                sounds.AddSound("SoundEffects/Shoot", false, -1f);
                                sounds.PlaySound();

                                //Top 
                                bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width,
                                   playerRect.Center.Y - (14 / 2)), bulletSpeed,
                                   new Vector2(bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 3));
                                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                bullets[bullets.Count - 1].animManager.SetCurrFrame(4);
                                //Center
                                bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width,
                                   playerRect.Center.Y - (14 / 2)), bulletSpeed,
                                   new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist));
                                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                bullets[bullets.Count - 1].animManager.SetCurrFrame(4);
                                //Bottom
                                bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width,
                                   playerRect.Center.Y - (14 / 2)), bulletSpeed,
                                   new Vector2(bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 3));
                                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                bullets[bullets.Count - 1].animManager.SetCurrFrame(4);
                            }
                            else if(animManager.isLeft)
                            {
                                bulletTravelDist = 64 * 1.5f;
                                bulletSpeed = 3.5f * 1.25f;

                                sounds.AddSound("SoundEffects/Shoot", false, -1f);
                                sounds.PlaySound();
                                sounds.AddSound("SoundEffects/Shoot", false, -1f);
                                sounds.PlaySound();


                                //Top 
                                bullets.Add(new Bullet(new Vector2(position.X /*- (18/2)*/,
                                   playerRect.Center.Y - (14 / 2)), -bulletSpeed,
                                   new Vector2(-bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, true, -bulletSpeed / 3, angle: num1));
                                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                bullets[bullets.Count - 1].animManager.SetCurrFrame(4);
                                //Center
                                bullets.Add(new Bullet(new Vector2(position.X /*- (18/2)*/,
                                   playerRect.Center.Y - (14 / 2)), -bulletSpeed,
                                   new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, angle: num1));
                                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                bullets[bullets.Count - 1].animManager.SetCurrFrame(4);
                                //sounds.AddSound("SoundEffects/Shoot", false, -1);
                                //sounds.PlaySound();
                                //Bottom
                                bullets.Add(new Bullet(new Vector2(position.X /*- (18/2)*/,
                                   playerRect.Center.Y - (14 / 2)), -bulletSpeed,
                                   new Vector2(-bulletMaxX, bulletMaxY), content, true, bulletTravelDist, true, bulletSpeed / 3, angle: num1));
                                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                bullets[bullets.Count - 1].animManager.SetCurrFrame(4);
                            }

                            shotGunDelay = maxShotgunDelay;
                        }

                            break;
                    case WeaponType.Burst:
                        burstDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (kb.IsKeyDown(KeyBindData.SideScrollInputs[3]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[3]) && burstDelay <= 0
                         || currControllerBtn.X == ButtonState.Pressed && prevControllerBtn.X == ButtonState.Released && burstDelay <= 0)
                        {
                            if(animManager.isRight)
                            {
                                bulletSpeed = 3.5f;
                                bulletTravelDist = 64 * 3.5f;
                                for (int i = 0; i < 3; i++)
                                {
                                    bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width,
                                  playerRect.Center.Y - (14 / 2)), bulletSpeed,
                                  new Vector2(bulletMaxX, -bulletMaxY), content, true, bulletTravelDist));
                                    bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                    sounds.AddSound("SoundEffects/Shoot", false, -.5f);
                                    sounds.PlaySound();
                                }
                            }
                            else if(animManager.isLeft)
                            {
                                bulletSpeed = 3.5f;
                                bulletTravelDist = 64 * 3f;
                                for (int i = 0; i < 3; i++)
                                {
                                    bullets.Add(new Bullet(new Vector2(position.X /*- (18 / 2)*/,
                                  playerRect.Center.Y - (14 / 2)), -bulletSpeed,
                                  new Vector2(-bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, angle: num1));
                                    bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                    sounds.AddSound("SoundEffects/Shoot", false, -.5f);
                                    sounds.PlaySound();
                                }
                            }

                            burstDelay = maxBurstDelay;
                        }
                        break;
                    case WeaponType.Bomb:

                        bombDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (kb.IsKeyDown(KeyBindData.SideScrollInputs[3]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[3]) && bombDelay <= 0
                   || currControllerBtn.X == ButtonState.Pressed && prevControllerBtn.X == ButtonState.Released && bombDelay <= 0)
                        {
                            if(animManager.isRight)
                            {
                                sounds.AddSound("SoundEffects/ThrowBomb", false);
                                sounds.PlaySound();

                                bulletSpeed = 3.5f;
                                bombs.Add(new Bomb(new Circle(new Vector2(position.X + playerRect.Width,
                                  (playerRect.Center.Y - (15) + playerRect.Height / 2f)), 15), bulletSpeed, -bulletSpeed * 5f, content));
                            }
                            else if(animManager.isLeft)
                            {
                                sounds.AddSound("SoundEffects/ThrowBomb", false);
                                sounds.PlaySound();

                                bulletSpeed = 3.5f;
                                bombs.Add(new Bomb(new Circle(new Vector2(position.X/* - (18 / 2)*/,
                                    (playerRect.Center.Y - (15) + playerRect.Height / 2f)), 15), -bulletSpeed, -bulletSpeed * 5f, content));
                            }

                            bombDelay = maxBombDelay;
                        }
                        break;
                    case WeaponType.Laser:

                        laserDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (kb.IsKeyDown(KeyBindData.SideScrollInputs[3]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[3]) && laserDelay <= 0
                   || currControllerBtn.X == ButtonState.Pressed && prevControllerBtn.X == ButtonState.Released && laserDelay <= 0)
                        {
                            if(animManager.isRight)
                            {
                                bulletSpeed = 4.5f;
                                bulletTravelDist = 64 * 4f;
                                for (int i = 0; i < 8; i++)
                                {
                                    bullets.Add(new Bullet(new Vector2(position.X + playerRect.Width,
                                  playerRect.Center.Y - (14 / 2)), bulletSpeed,
                                  new Vector2(bulletMaxX, -bulletMaxY), content, true, bulletTravelDist));
                                    bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                    sounds.AddSound("SoundEffects/Shoot", false, .5f);
                                    sounds.PlaySound();
                                }
                            }
                            else if(animManager.isLeft)
                            {
                                bulletSpeed = 4.5f;
                                bulletTravelDist = 64 * 4f;
                                for (int i = 0; i < 8; i++)
                                {
                                    bullets.Add(new Bullet(new Vector2(position.X /*- (18 / 2)*/,
                                  playerRect.Center.Y - (14 / 2)), -bulletSpeed,
                                  new Vector2(-bulletMaxX, -bulletMaxY), content, true, bulletTravelDist, angle: num1));
                                    bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Player;
                                    sounds.AddSound("SoundEffects/Shoot", false, .5f);
                                    sounds.PlaySound();
                                }
                            }

                            laserDelay = maxLaserDelay;
                        }
                            break;
                }
               
            }
            else if(playerState == PlayerStates.Shooting)
            {
                playerState = PlayerStates.Movement;
            }
            int num = kb.GetPressedKeys().Count<Keys>();
            if (kb.IsKeyUp(KeyBindData.SideScrollInputs[2]) && controllerMoveDir.Y > -.9)
            {
                weaponWheel.active = false;
            }

            Vector2 RightStick = GamePad.GetState(0).ThumbSticks.Right;

            //Weapon Wheel actions
            if(weaponWheel.active)
            {
                if (GamePad.GetState(0).IsButtonDown(Buttons.RightStick) || GamePad.GetState(0).IsButtonDown(Buttons.RightTrigger) ||
                    kb.IsKeyDown(KeyBindData.SideScrollInputs[7]))
                {
                    currWeapon = WeaponType.Pistol;
                    selectedWeapon = 0;

                    bulletDmg = pistolDmg;
                    SetShootDelays();
                }
                else if(kb.IsKeyDown(KeyBindData.SideScrollInputs[9]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[9]) && gameData.bossKillCount >= 2||
                    RightStick.X > 0 && gameData.bossKillCount >= 2)
                {
                    currWeapon = WeaponType.Burst;
                    selectedWeapon = 2;

                    bulletDmg = burstDmg;
                    SetShootDelays();
                    //if (selectedWeapon == 0)
                    //{
                    //    currWeapon = WeaponType.Burst;
                    //    selectedWeapon = 2;

                    //    bulletDmg = burstDmg;
                    //    shootDelay = burstDelay;
                    //    iShootDelay = shootDelay;

                    //}
                    //else if (selectedWeapon == 1)
                    //{
                    //    currWeapon = WeaponType.Pistol;
                    //    selectedWeapon = 0;

                    //    bulletDmg = pistolDmg;
                    //    shootDelay = pistolDelay;
                    //    iShootDelay = shootDelay;
                    //}


                }
                else if(kb.IsKeyDown(KeyBindData.SideScrollInputs[8]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[8]) && gameData.bossKillCount >= 4||
                    RightStick.X < 0 && gameData.bossKillCount >= 4)
                {
                    //if (selectedWeapon == 2)
                    //{
                    //    currWeapon = WeaponType.Pistol;
                    //    selectedWeapon = 0;

                    //    bulletDmg = pistolDmg;
                    //    shootDelay = pistolDelay;
                    //    iShootDelay = shootDelay;
                    //}
                    //else if (selectedWeapon == 0)
                    //{
                    //    currWeapon = WeaponType.Laser;
                    //    selectedWeapon = 1;

                    //    bulletDmg = laserDmg;
                    //    shootDelay = laserDelay;
                    //    iShootDelay = shootDelay;
                    //}
                    currWeapon = WeaponType.Laser;
                    selectedWeapon = 1;

                    bulletDmg = laserDmg;
                    SetShootDelays();
                }
                else if(kb.IsKeyDown(KeyBindData.SideScrollInputs[10]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[10]) && gameData.bossKillCount >= 3||
                   RightStick.Y > 0 && gameData.bossKillCount >= 3)
                {
                    currWeapon = WeaponType.Bomb;
                    selectedWeapon = 3;

                    bulletDmg = bombDmg;
                    SetShootDelays();
                    //if (selectedWeapon == 0)
                    //{
                    //    currWeapon = WeaponType.Bomb;
                    //    selectedWeapon = 3;

                    //    bulletDmg = bombDmg;
                    //    shootDelay = bombDelay;
                    //    iShootDelay = shootDelay;

                    //}
                    //else if (selectedWeapon == 4)
                    //{
                    //    currWeapon = WeaponType.Pistol;
                    //    selectedWeapon = 0;

                    //    bulletDmg = pistolDmg;
                    //    shootDelay = pistolDelay;
                    //    iShootDelay = shootDelay;
                    //}
                }
                else if(kb.IsKeyDown(KeyBindData.SideScrollInputs[11]) && prevKb.IsKeyUp(KeyBindData.SideScrollInputs[11]) && gameData.bossKillCount >= 1||
                    RightStick.Y < 0 && gameData.bossKillCount >= 1)
                {
                    //if (selectedWeapon == 0)
                    //{
                    //    currWeapon = WeaponType.Shotgun;
                    //    selectedWeapon = 4;

                    //    bulletDmg = shotGunDmg;
                    //    shootDelay = shotGunDelay;
                    //    iShootDelay = shootDelay;

                    //}
                    //else if (selectedWeapon == 3)
                    //{
                    //    currWeapon = WeaponType.Pistol;
                    //    selectedWeapon = 0;

                    //    bulletDmg = pistolDmg;
                    //    shootDelay = pistolDelay;
                    //    iShootDelay = shootDelay;
                    //}
                    currWeapon = WeaponType.Shotgun;
                    selectedWeapon = 4;

                    bulletDmg = shotGunDmg;
                    SetShootDelays();
                }
               
            }
            prevKb = kb;
            prevControllerBtn = currControllerBtn;
            prevDpad = GamePad.GetState(0).DPad;
        }

        private void SetShootDelays()
        {

            pistolDelay = maxPistolDelay;

            shotGunDelay = maxShotgunDelay;
            burstDelay = maxBurstDelay;

            bombDelay = maxBombDelay;

            laserDelay = maxLaserDelay;
        }

        public void Collision(Rectangle newRect /*int xOffset, int yOffset, int levelInX, int levelInY, Rectangle bounds*/, bool isEnemy = false)
        {
            //isColliding = false;
            //blockBottom = false;


            if (!isEnemy)
            {
                if (playerRect.TouchTopOf(newRect))
                {
                    coyoteTime = coyoteTimeMax;

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
                        isDashing = false;
                      

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
                Rectangle tempRect = new Rectangle(playerRect.X - 10, playerRect.Y, playerRect.Width + 20, playerRect.Height);
                if (playerRect.TouchTopOf(newRect))
                {

                    isCollidingDown = true;
                    if (velocity.Y > 0 && playerState != PlayerStates.Dashing)
                    {
                        while (playerRect.Bottom > newRect.Top - 1)
                        {
                            velocity.Y += -(Velocity.Y);
                            position.Y -= .1f;
                            playerRect.Y = (int)position.Y;
                        }
                    }

                 

                    if(groundPound && tempRect.TouchTopOf(newRect))
                    {
                        
                        killEnemy = true;
                        //if (playerRect.Center.X < newRect.Center.X)
                        //{
                        //    velocity = new Vector2(-8, -6);
                        //}
                        //else if (playerRect.Center.X >= newRect.Center.X)
                        //{
                        //    velocity = new Vector2(8, -6);
                        //}
                        groundPound = false;
                    }
                    else if (playerRect.Center.X < newRect.Center.X)
                    {
                        velocity = new Vector2(-8, 0);
                    }
                    else if (playerRect.Center.X >= newRect.Center.X)
                    {
                        velocity = new Vector2(8, 0);
                    }
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
                    animManager.Draw(spriteBatch, Color.White);
                    redCount++;
                }
                if(whiteCount < redCount)
                {
                    animManager.Draw(spriteBatch, Color.Red * .5f);
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

            foreach(Bomb bomb in bombs)
            {
                bomb.Draw(spriteBatch);
            }

            particles.Draw(spriteBatch);
         

            if(weaponWheel.active)
            {
                for(int i = 0; i < weaponWheel.WeaponSlots.Count; i++)
                {
                    if (selectedWeapon == i)
                        spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), weaponWheel.WeaponSlots[i].rect, Color.Blue * .35f);
                    else
                        spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), weaponWheel.WeaponSlots[i].rect, Color.White * .35f);

                }
                   
            }
        }

        public int DashIndex()//Returns the index needed for the dash icon    0: Full, 1: Empty, 2+ Growing rate
        {
            int index = 0;

            if(canDash)
            {

            }
            else
            {
                float percent = dashCoolDown/dashCoolDownMax * 100;


                if (percent <= 15)
                    index = 1;
                else if (percent <= 40)
                    index = 2;
                else if (percent <= 65)
                    index = 3;
                else
                    index = 4;
                
            }

            return index;
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
    }

    class WeaponSlot
    {
        public Rectangle rect;
        public Texture2D texture;
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
}
