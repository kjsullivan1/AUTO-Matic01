﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework.Content;
using AUTO_Matic.SideScroll;
using AUTO_Matic.Scripts.Effects;

namespace AUTO_Matic.Scripts.SideScroll.Enemy
{
    class FinalBoss
    {
        public Rectangle bossRect;

        enum BossStates { ChooseWeapon, SpinToPlace, Attack ,Move, Return}
        BossStates bossState = BossStates.ChooseWeapon;

        List<Rectangle> SideWall = new List<Rectangle>(); //15 
        List<Rectangle> TopWall = new List<Rectangle>(); //25
        List<Rectangle> rayRects = new List<Rectangle>();
        ContentManager content;
        Random random = new Random();
        public Vector2 velocity = Vector2.Zero;
        float moveSpeed = 5.5f;
        Vector2 startPos;

        ParticleManager particles;
        List<ShootingLocs> shootLocs = new List<ShootingLocs>();
        List<Vector2> goToLocs = new List<Vector2>(); //locations the shootLocs will goTo
        float returnDelay = .75f;
        float iReturn;
        #region Animations
        BossHealthBar healthBar;
        #endregion

        #region Shooting
        float angle;
        Texture2D line;
        bool stopRotate = false;
        bool useTopWall = false;
        bool clockWise = false;
        int rotateSpeed = 3;
        float autoShootDelay = 0;
        float iAutoDelay = .25f;
        int autoShotCount = 0;
        int maxAutoShots = 8;
        //public float angle;
        public List<Bullet> bullets = new List<Bullet>();
        public List<Bullet> bombs = new List<Bullet>();
        List<Explosion> explosions = new List<Explosion>();
        Rectangle destRect = new Rectangle();
        float chargeTime = 4;
        float fireTime = .75f;
        float bulletSpeed = 5f;
        float shootDelay = 1.5f;//In seconds
        float iShootDelay;
        public float bulletDmg = 1.5f;
        public float bulletTravelDist = 64 * 24;
        Texture2D visionTxture;
        int width;
        int height;
        Rectangle bounds;
        Rectangle tempRect;
        string chosenWeapon;
        float burstDelay = .05f;
        #endregion

        int sizeMod = 2;
        float health = 20;
        bool damaged = false;
        float dmgTime = .25f;
        public float Health
        {
            get { return health; }
            set
            {
                health = value;
                damaged = true;
                if (health <= 0)
                    health = 0;
                healthBar.ChangeHealth(health);
            }
        }

        #region Constructor
        public FinalBoss(Vector2 pos, ContentManager content, GraphicsDevice graphics)
        {
            bossRect = new Rectangle(pos.ToPoint(), new Point(64 * sizeMod, 64 * sizeMod));
            startPos = pos;
            iShootDelay = shootDelay;
            iReturn = returnDelay;
            this.content = content;
            line = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
            line.SetData(new[] { Color.White });
            particles = new ParticleManager();
            particles.Initialize(content.Load<Texture2D>(@"Textures\white"));
            //iAutoDelay = autoShootDelay;
            int mod = 48;

            shootLocs.Add(new ShootingLocs(new Rectangle(bossRect.Right, bossRect.Y - (bossRect.Height - mod),
                                bossRect.Width - mod, bossRect.Height - mod)));
            shootLocs[0].dir = new Vector2(1, 1); //TopRight
            goToLocs.Add(new Vector2(bossRect.Right, bossRect.Y - (bossRect.Height - mod)));
            shootLocs[0].type = "Shotgun";

            shootLocs.Add(new ShootingLocs(new Rectangle(bossRect.X - (bossRect.Width - mod), bossRect.Y - (bossRect.Height - mod),
                    bossRect.Width - mod, bossRect.Height - mod)));
            shootLocs[1].dir = new Vector2(-1, 1);//TopLeft
            goToLocs.Add(new Vector2(bossRect.X - (bossRect.Width - mod), bossRect.Y - (bossRect.Height - mod)));
            shootLocs[1].type = "Auto";

            shootLocs.Add(new ShootingLocs(new Rectangle(bossRect.Right, bossRect.Bottom /*- (bossRect.Height - mod)*/,
                    bossRect.Width - mod, bossRect.Height - mod)));
            shootLocs[2].dir = new Vector2(1, -1);//BottomRight
            goToLocs.Add(new Vector2(bossRect.Right, bossRect.Bottom));
            shootLocs[2].type = "Laser";

            shootLocs.Add(new ShootingLocs(new Rectangle(bossRect.X - (bossRect.Width - mod), bossRect.Bottom /*- (bossRect.Height - mod)*/,
                   bossRect.Width - mod, bossRect.Height - mod)));
            shootLocs[3].dir = new Vector2(-1, -1);//BottomLeft
            goToLocs.Add(new Vector2(bossRect.X - (bossRect.Width - mod), bossRect.Bottom));
            shootLocs[3].type = "Bomb";


            for(int i = 0; i < 15; i++)
            {
                SideWall.Add(new Rectangle((int)bossRect.Right, (int)(bossRect.Y + (64 * i)), 64, 64));
            }

            for(int i = 0; i < 25; i++)
            {
                TopWall.Add(new Rectangle(bossRect.Right - (64 + (64 * i)), bossRect.Y - 64, 64, 64));
            }

            bounds = new Rectangle(TopWall[TopWall.Count - 1].X, TopWall[TopWall.Count - 1].Y, 1980 * 2, 1980 * 2);

            healthBar = new BossHealthBar(new Rectangle(bounds.Center.X - (int)(bounds.Width / 2.6f), bounds.Y + 60, bounds.Width / 5, (int)(bossRect.Height / 2.75f)), content);

        }
        #endregion
        
        public void Update(GameTime gameTime, SSPlayer ssPlayer)
        {
            UpdateShootLocs();

            switch(bossState)
            {
                #region ChooseWeapon
                case BossStates.ChooseWeapon:
                    //Choose to either go vertical or horizontal
                    stopRotate = false;
                    if (random.Next(0,101) < 51)
                    {
                        useTopWall = true;
                    }
                    else
                    {
                        useTopWall = false;
                    }
                    //Next choose the weapon type 
                    switch (random.Next(0,4))
                    {
                        case 0:
                            chosenWeapon = "Shotgun";
                            bulletDmg = 1.85f;
                            break;
                        case 1:
                            chosenWeapon = "Laser";
                            bulletDmg = 1.2f;
                            break;
                        case 2:
                            chosenWeapon = "Auto";
                            bulletDmg = .75f;
                            break;
                        case 3:
                            chosenWeapon = "Bomb";
                            bulletDmg = 3.1f;
                            break;
                    }
                    bossState = BossStates.SpinToPlace;
                    break;
                #endregion
                #region SpinToPlace
                case BossStates.SpinToPlace:
                    Vector2 chosenDir = new Vector2();
                    foreach (ShootingLocs loc in shootLocs)
                    {
                        if (loc.type == chosenWeapon)
                        {
                            chosenDir = loc.dir;
                            if (useTopWall)
                            {
                                if (loc.rect.Y == bossRect.Bottom && loc.rect.X >= bossRect.Center.X - loc.rect.Width/2)
                                {
                                    stopRotate = true;
                                    bossState = BossStates.Move;
                                }
                            }
                            else
                            {
                                if (loc.rect.X == bossRect.X - loc.rect.Width && loc.rect.Y >= bossRect.Center.Y - loc.rect.Height/2)
                                {
                                    stopRotate = true;
                                    bossState = BossStates.Move;
                                }
                            }
                            break;
                        }
                        

                    }
                    if (stopRotate)
                    {
                        Vector2 topLeft = new Vector2(-1, 1);
                        Vector2 topRight = new Vector2(1, 1);
                        Vector2 bottomRight = new Vector2(1, -1);
                        Vector2 bottomLeft = new Vector2(-1, -1);
                        for (int i = 0; i < shootLocs.Count; i++)
                        {
                            if (shootLocs[i].dir != chosenDir)
                            {
                                if (shootLocs[i].dir == topLeft)
                                {
                                    if (clockWise)
                                    {
                                        shootLocs[i].rect.X = bossRect.Center.X - shootLocs[i].rect.Width / 2;
                                    }
                                    else
                                    {
                                        shootLocs[i].rect.Y = bossRect.Center.Y - shootLocs[i].rect.Height / 2;
                                    }
                                }
                                else if (shootLocs[i].dir == topRight)
                                {
                                    if (clockWise)
                                    {
                                        shootLocs[i].rect.Y = bossRect.Center.Y - shootLocs[i].rect.Height / 2;
                                    }
                                    else
                                    {
                                        shootLocs[i].rect.X = bossRect.Center.X - shootLocs[i].rect.Width / 2;
                                    }
                                }
                                else if (shootLocs[i].dir == bottomLeft)
                                {
                                    if (clockWise)
                                    {
                                        shootLocs[i].rect.Y = bossRect.Center.Y - shootLocs[i].rect.Height / 2;
                                    }
                                    else
                                    {
                                        shootLocs[i].rect.X = bossRect.Center.X - shootLocs[i].rect.Width / 2;
                                    }
                                }
                                else if (shootLocs[i].dir == bottomRight)
                                {
                                    if (clockWise)
                                    {
                                        shootLocs[i].rect.X = bossRect.Center.X - shootLocs[i].rect.Width / 2;
                                    }
                                    else
                                    {
                                        shootLocs[i].rect.Y = bossRect.Center.Y - shootLocs[i].rect.Height / 2;
                                    }
                                }
                            }
                        }
                    }
                    break;
                #endregion
                #region Move
                case BossStates.Move:
                    bossRect = new Rectangle((int)(bossRect.X + velocity.X), (int)(bossRect.Y + velocity.Y), bossRect.Width, bossRect.Height);
                    Vector2 bossPos = new Vector2(bossRect.Center.X - ssPlayer.Rectangle.Width / 2,
                        bossRect.Center.Y - ssPlayer.Rectangle.Height / 2);

                    if(useTopWall)
                    {
                        if(bossPos.X > ssPlayer.Rectangle.Center.X)
                        { 
                            if(bossPos.X + velocity.X > ssPlayer.Rectangle.Center.X)
                            {
                                velocity.X = -moveSpeed;
                            }
                            else
                            {
                                velocity.X = 0;
                                bossState = BossStates.Attack;
                            }
               
                        }
                        else
                        {
                            velocity.X = 0;
                            bossState = BossStates.Attack;
                        }
                    }
                    else
                    {
                        if(bossPos.Y < ssPlayer.Rectangle.Y)
                        {
                            if(bossPos.Y + velocity.Y < ssPlayer.Rectangle.Y)
                            {
                                velocity.Y = moveSpeed;
                            }
                            else
                            {
                                velocity.Y = 0;
                                bossState = BossStates.Attack;
                            }
                        }
                        else
                        {
                            velocity.Y = 0;
                            bossState = BossStates.Attack;
                        }
                    }

                    break;
                #endregion
                #region Attack
                case BossStates.Attack:
                    Vector2 targetDir = new Vector2(ssPlayer.playerRect.Center.X, ssPlayer.playerRect.Center.Y) - new Vector2(bossRect.Center.X, bossRect.Center.Y);
                    angle = (float)Math.Atan2(targetDir.Y, targetDir.X); //sub by 90 if problems occur
                    bossPos = new Vector2(bossRect.Center.X - ssPlayer.Rectangle.Width / 2,
                       bossRect.Center.Y - ssPlayer.Rectangle.Height / 2);
                    switch (chosenWeapon)
                    {
                        case "Shotgun":

                            //bossRect = new Rectangle(((bounds.X + bounds.Width / 2) - width / 2), (((bounds.Y + bounds.Height / 2) - height / 2)), width, height);
                            ShootShotgun(ssPlayer, angle);
                            bossState = BossStates.Return;
                            //stopRotate = false;
                            break;
                        case "Laser":
                            SetRay(angle, ssPlayer, bossPos, gameTime);
                            if(fireTime <= 0)
                            {
                                bossState = BossStates.Return;
                                destRect = Rectangle.Empty;
                                fireTime = .75f;
                                chargeTime = 4;
                            }
                               
                            //stopRotate = false;
                            break;
                        case "Auto":

                            autoShootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                           
                            if (autoShotCount < maxAutoShots && autoShootDelay <= 0)
                            {
                                burstDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                                autoShotCount++;
                                FireSemiAuto(ssPlayer, angle, gameTime);
                                autoShootDelay = iAutoDelay;
                            }
                            else if(autoShotCount >= maxAutoShots)
                            {
                                bossState = BossStates.Return;
                                autoShotCount = 0;
                            }
                           
                            //stopRotate = false;
                            break;
                        case "Bomb":
                            LaunchBomb(ssPlayer, angle);
                            bossState = BossStates.Return;
                           // stopRotate = false;
                            break;
                    }

                    break;
                #endregion
                #region Return
                case BossStates.Return:
                    returnDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //stopRotate = false;
                    if(returnDelay <= 0)
                    {
                        
                        rayRects.Clear();
                        bossRect = new Rectangle((int)(bossRect.X + velocity.X), (int)(bossRect.Y + velocity.Y),
                            bossRect.Width, bossRect.Height);

                        if (useTopWall)
                        {
                            if (bossRect.X < startPos.X)
                            {
                                if (velocity.X < 0)
                                {
                                    velocity.X = -velocity.X;
                                }
                                velocity.X = moveSpeed;
                            }
                            else
                            {
                                bossRect.X = (int)startPos.X;
                                velocity.X = 0;
                                useTopWall = false;
                                bossState = BossStates.ChooseWeapon;
                                returnDelay = iReturn;
                            }
                        }
                        else
                        {
                            if (bossRect.Y > startPos.Y)
                            {
                                if (velocity.Y > 0)
                                {
                                    velocity.Y = -velocity.Y;
                                }
                                velocity.Y = -moveSpeed;
                            }
                            else
                            {
                                velocity.Y = 0;
                                bossRect.Y = (int)startPos.Y;
                                bossState = BossStates.ChooseWeapon;
                                returnDelay = iReturn;
                            }
                        }
                    }
                    
                    break;
                    #endregion
            }

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(gameTime);

                if(bullets[i].rect.Intersects(ssPlayer.Rectangle))
                {
                    bullets[i].delete = true;
                    if(!ssPlayer.damaged)
                        DamagePlayer(ssPlayer);
                }

                if (bullets[i].delete)
                {
                    bullets.RemoveAt(i);
                }
            }
            for(int i = bombs.Count - 1; i >= 0; i--)
            {
                bombs[i].Update(gameTime);

                if(bombs[i].rect.Intersects(ssPlayer.Rectangle))
                {
                    bombs[i].delete = true;
                    if(!ssPlayer.damaged)
                        DamagePlayer(ssPlayer);
                }

                if(bombs[i].delete)
                {
                    explosions.Add(new Explosion(new Circle(new Vector2(bombs[i].rect.Center.X, bombs[i].rect.Center.Y), bombs[i].rect.Width),
                         3, (int)(bombs[i].rect.Width * 2.5f)));

                    int radiusDif = explosions[explosions.Count - 1].maxSize - explosions[explosions.Count - 1].rect.Radius;
                    particles.MakeExplosion(explosions[explosions.Count - 1].rect.Bounds,
                           new Circle(new Vector2(explosions[explosions.Count - 1].rect.Bounds.X - radiusDif,
                           explosions[explosions.Count - 1].rect.Bounds.Y - radiusDif), explosions[explosions.Count - 1].maxSize / 2),
                           20);
                    bombs.RemoveAt(i);
                 
                }
            }

            for(int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].rect.Radius >= explosions[i].maxSize)
                {
                    explosions.RemoveAt(i);
                }
            }

            for(int i = ssPlayer.bullets.Count - 1; i >= 0; i--)
            {
                if(ssPlayer.bullets[i].rect.Intersects(bossRect) && !damaged)
                {
                    Health -= ssPlayer.bulletDmg;
                    ssPlayer.bullets[i].delete = true;
                }
            }

            for(int i = ssPlayer.bombs.Count - 1; i >= 0; i--)
            {
                if(ssPlayer.bombs[i].circle.Intersects(bossRect) && !damaged)
                {
                    Health -= ssPlayer.bulletDmg;
                    ssPlayer.bombs[i].delete = true;
                }
            }

            if (damaged)
                dmgTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(dmgTime <= 0)
            {
                damaged = false;
                dmgTime = .25f;
            }

            particles.Update(gameTime);
            healthBar.Update(new Point(healthBar.rect.X, healthBar.rect.Y));

        }

        private void DamagePlayer(SSPlayer ssPlayer)
        {

            ssPlayer.Health -= bulletDmg;
            ssPlayer.playerState = SSPlayer.PlayerStates.Knockback;
            switch (chosenWeapon)
            {
                case "Shotgun":

                    if (bossRect.Center.X < ssPlayer.playerRect.Center.X)
                        ssPlayer.knockBackX = 5;
                    else
                        ssPlayer.knockBackX = -5;
                    ssPlayer.knockBackY = -3;
                    ssPlayer.playerRect.Y -= 2;
                    ssPlayer.position.Y -= 2;
                    break;
                case "Laser":
                    if (bossRect.Center.X < ssPlayer.playerRect.Center.X)
                        ssPlayer.knockBackX = 5;
                    else
                        ssPlayer.knockBackX = -5;
                    ssPlayer.knockBackY = -1;
                    ssPlayer.playerRect.Y -= 2;
                    ssPlayer.position.Y -= 2;
                    break;
                case "Auto":
                    if (bossRect.Center.X < ssPlayer.playerRect.Center.X)
                        ssPlayer.knockBackX = 5;
                    else
                        ssPlayer.knockBackX = -5;
                    ssPlayer.knockBackY = -2;
                    ssPlayer.playerRect.Y -= 2;
                    ssPlayer.position.Y -= 2;
                    break;
                case "Bomb":
                    if (bossRect.Center.X < ssPlayer.playerRect.Center.X)
                        ssPlayer.knockBackX = 7;
                    else
                        ssPlayer.knockBackX = -7;
                    ssPlayer.knockBackY = -5;
                    ssPlayer.playerRect.Y -= 2;
                    ssPlayer.position.Y -= 2;
                    break;
            }
          
        }

        private void ShootShotgun(SSPlayer ssPlayer, float angle)
        {
            float bulletUpSpeedX = (float)Math.Cos((double)angle - .15f) * bulletSpeed;
            float bulletUpSpeedY = (float)Math.Sin((double)angle - .15f) * bulletSpeed;

            float bulletSpeedX = (float)Math.Cos((double)angle) * bulletSpeed; //Direct SpeedX
            float bulletSpeedY = (float)Math.Sin((double)angle) * bulletSpeed; //DirectSpeedY

            float bulletDownSpeedX = (float)Math.Cos((double)angle + .15f) * bulletSpeed;
            float bulletDownSpeedY = (float)Math.Sin((double)angle + .15f) * bulletSpeed;
            
            bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY),
                content, true, bulletTravelDist, true, bulletSpeedY, angle: angle));
            bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;

            bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletUpSpeedX, new Vector2(bulletUpSpeedX, bulletUpSpeedY),
             content, true, bulletTravelDist, true, bulletUpSpeedY,  angle: angle));
            bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;

            bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletDownSpeedX, new Vector2(bulletDownSpeedX, bulletDownSpeedY),
             content, true, bulletTravelDist, true, bulletDownSpeedY, angle: angle));
            bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;
        }

        private void UpdateShootLocs()
        {
            foreach (ShootingLocs loc in shootLocs)
            {
                loc.rect = new Rectangle((int)(loc.rect.X + velocity.X), (int)(loc.rect.Y + velocity.Y), loc.rect.Width, loc.rect.Height);
                Vector2 locPos = new Vector2(loc.rect.X, loc.rect.Y);

                if(stopRotate)
                {

                }
                else
                {
                    if (loc.dir == new Vector2(1, 1))
                    {
                        if (clockWise)
                        {
                            //if the y first and use else statements to guarentee y movement first 
                            if (locPos.Y < goToLocs[2].Y)
                            {
                                loc.rect.Y += rotateSpeed;
                                if (loc.rect.Y > goToLocs[2].Y)
                                {
                                    loc.rect.Y = (int)goToLocs[2].Y;
                                    loc.dir = new Vector2(1, -1);
                                }
                            }
                            else if (loc.rect.Y >= goToLocs[2].Y)
                            {
                                loc.rect.Y = (int)goToLocs[2].Y;
                                loc.dir = new Vector2(1, -1);
                            }

                        }
                        else
                        {
                            if (locPos.X > goToLocs[1].X)
                            {
                                loc.rect.X -= rotateSpeed;

                                if (loc.rect.X < goToLocs[1].X)
                                {
                                    loc.rect.X = (int)goToLocs[1].X;
                                    loc.dir = new Vector2(-1, 1);
                                }
                            }
                            else if (loc.rect.X <= goToLocs[1].X)
                            {
                                loc.rect.X = (int)goToLocs[1].X;
                                loc.dir = new Vector2(-1, 1);
                            }
                        }
                    }
                    else if (loc.dir == new Vector2(-1, 1))
                    {
                        if (clockWise)
                        {
                            if (locPos.X < goToLocs[0].X)
                            {
                                loc.rect.X += rotateSpeed;

                                if (loc.rect.X > goToLocs[0].X)
                                {
                                    loc.rect.X = (int)goToLocs[0].X;
                                    loc.dir = new Vector2(1, 1);
                                }
                            }
                            else if (loc.rect.X >= goToLocs[0].X)
                            {
                                loc.rect.X = (int)goToLocs[0].X;
                                loc.dir = new Vector2(1, 1);
                            }
                        }
                        else
                        {
                            if (locPos.Y < goToLocs[3].Y)
                            {
                                loc.rect.Y += rotateSpeed;

                                if (loc.rect.Y > goToLocs[3].Y)
                                {
                                    loc.rect.Y = (int)goToLocs[3].Y;
                                    loc.dir = new Vector2(-1, -1);
                                }
                            }
                            else if (loc.rect.Y >= goToLocs[3].Y)
                            {
                                loc.rect.Y = (int)goToLocs[3].Y;
                                loc.dir = new Vector2(-1, -1);
                            }
                        }
                    }
                    else if (loc.dir == new Vector2(-1, -1))
                    {
                        if (clockWise)
                        {
                            if (locPos.Y > goToLocs[1].Y)
                            {
                                loc.rect.Y -= rotateSpeed;

                                if (loc.rect.Y < goToLocs[1].Y)
                                {
                                    loc.rect.Y = (int)goToLocs[1].Y;
                                    loc.dir = new Vector2(-1, 1);
                                }
                            }
                            else if (loc.rect.Y <= goToLocs[1].Y)
                            {
                                loc.rect.Y = (int)goToLocs[1].Y;
                                loc.dir = new Vector2(-1, 1);
                            }
                        }
                        else
                        {


                            if (locPos.X < goToLocs[2].X)
                            {
                                loc.rect.X += rotateSpeed;

                                if (loc.rect.X > goToLocs[2].X)
                                {
                                    loc.rect.X = (int)goToLocs[2].X;
                                    loc.dir = new Vector2(1, -1);
                                }
                            }
                            else if (loc.rect.X >= goToLocs[2].X)
                            {
                                loc.rect.X = (int)goToLocs[2].X;
                                loc.dir = new Vector2(1, -1);
                            }
                        }
                    }
                    else if (loc.dir == new Vector2(1, -1))
                    {
                        if (clockWise)
                        {
                            if (locPos.X > goToLocs[3].X)
                            {
                                loc.rect.X -= rotateSpeed;

                                if (loc.rect.X < goToLocs[3].X)
                                {
                                    loc.rect.X = (int)goToLocs[3].X;
                                    loc.dir = new Vector2(-1, -1);
                                }
                            }
                            else if (loc.rect.X <= goToLocs[3].X)
                            {
                                loc.rect.X = (int)goToLocs[3].X;
                                loc.dir = new Vector2(-1, -1);
                            }

                        }
                        else
                        {

                            if (locPos.Y > goToLocs[0].Y)
                            {
                                loc.rect.Y -= rotateSpeed;

                                if (loc.rect.Y < goToLocs[0].Y)
                                {
                                    loc.rect.Y = (int)goToLocs[0].Y;
                                    loc.dir = new Vector2(1, 1);
                                }
                            }
                            else if (loc.rect.Y <= goToLocs[0].Y)
                            {
                                loc.rect.Y = (int)goToLocs[0].Y;
                                loc.dir = new Vector2(1, 1);
                            }
                        }
                    }
                }
              
            }
        }

        private void FireSemiAuto(SSPlayer ssPlayer, float angle, GameTime gameTime)
        {
            Vector2 targetDir = new Vector2(ssPlayer.playerRect.X + ssPlayer.playerRect.Width / 2, ssPlayer.playerRect.Y + ssPlayer.playerRect.Height / 2) -
              new Vector2(bossRect.Center.X, bossRect.Center.Y);
            angle = (float)Math.Atan2(targetDir.Y, targetDir.X);

            Vector2 bossPos = new Vector2(bossRect.Center.X, bossRect.Center.Y);

            float bulletSpeedX = (float)Math.Cos((double)angle) * 2;
            float bulletSpeedY = (float)Math.Sin((double)angle) * 2;


            if (burstDelay <= 0)
            {
                #region BurstShot
                bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY),
                    content, true, bulletTravelDist, true, bulletSpeedY, angle: angle));
                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;
                bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletSpeedX * 1.5f, new Vector2(bulletSpeedX, bulletSpeedY),
               content, true, bulletTravelDist, true, bulletSpeedY * 1.5f, angle: angle));
                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;
                bullets.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletSpeedX / 1.5f, new Vector2(bulletSpeedX, bulletSpeedY),
              content, true, bulletTravelDist, true, bulletSpeedY / 1.5f, angle: angle));
                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;
                #endregion
                burstDelay = .05f;
            }
        }

        private void LaunchBomb(SSPlayer ssPlayer, float angle)
        {
           
                bulletTravelDist = DistForm(ssPlayer.Rectangle.Center.ToVector2(), bossRect.Center.ToVector2()) + bossRect.Width;


            float bulletSpeedX = (float)Math.Cos((double)angle) * 2;
            float bulletSpeedY = (float)Math.Sin((double)angle) * 2;

            bombs.Add(new Bullet(new Vector2(bossRect.Center.X, bossRect.Center.Y), bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY),
                content, true, bulletTravelDist, true, bulletSpeedY, size: 42));
            bombs[bombs.Count - 1].BulletType = Bullet.BulletTypes.Bomb;
        }
        void SetRay(float angle, SSPlayer playerRect, Vector2 bossPos, GameTime gameTime)
        {

            int size = 64;
            chargeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            destRect = new Rectangle(bossRect.Center.X, bossRect.Center.Y,
                  distForm(new Vector2(bossRect.Center.X, bossRect.Center.Y),
                  new Vector2(playerRect.playerRect.Center.X, playerRect.playerRect.Center.Y)), 28);
            if (chargeTime <= 0 && fireTime >= 0)
            {

                fireTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                float bulletSpeedX = (float)Math.Cos((double)angle) * 8;
                float bulletSpeedY = (float)Math.Sin((double)angle) * 8;
                bullets.Add(new Bullet(new Vector2(destRect.X, destRect.Y), bulletSpeedX,
                    new Vector2(bulletSpeedX, bulletSpeedY), content, true, bounds.Width, true, bulletSpeedY, size: 20, angle: angle));
                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Boss;



            }
        }
        int distForm(Vector2 pos1, Vector2 pos2)
        {
            return (int)Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2));

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //for(int i = 0; i < TopWall.Count; i++)
            //{
            //    spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), TopWall[i], Color.Black);
            //}
            //for (int i = 0; i < SideWall.Count; i++)
            //{
            //    spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), SideWall[i], Color.Black);
            //}
            if (destRect != Rectangle.Empty)
            {
                spriteBatch.Draw(line, destinationRectangle: destRect, color: Color.Crimson * .5f, rotation: angle);
            }
            healthBar.Draw(spriteBatch);
            spriteBatch.Draw(content.Load<Texture2D>("SideScroll/Animations/FinalBoss/MonoBoss"), bossRect, Color.White);

            foreach(ShootingLocs loc in shootLocs)
            {
                if(loc.type == chosenWeapon)
                {
                    spriteBatch.Draw(content.Load<Texture2D>("SideScroll/Animations/FinalBoss/BossGun"), loc.rect, Color.Blue);
                }
                else
                {
                    spriteBatch.Draw(content.Load<Texture2D>("SideScroll/Animations/FinalBoss/BossGun"), loc.rect, Color.White);
                }
          
            }

         

            for(int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }

            for(int i = 0; i < bombs.Count; i++)
            {
                bombs[i].Draw(spriteBatch);
            }
            for(int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch, content);
            }
            
            particles.Draw(spriteBatch);
         
        }
        int DistForm(Vector2 pos1, Vector2 pos2)
        {
            int num = (int)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
            return num;

        }
        
    }

    class ShootingLocs
    {
        public Rectangle rect;
        public string type;
        public Vector2 dir;

        public ShootingLocs(Rectangle rect)
        {
            this.rect = rect;
            dir = Vector2.Zero;
        }

    }
    class Explosion
    {
        public Circle rect;
        public int growthRate;
        public int maxSize;

        public Explosion(Circle rect, int rate, int max)
        {
            this.rect = rect;
            growthRate = rate;
            maxSize = max;
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
            spriteBatch.Draw(content.Load<Texture2D>("Textures/Button"), rect.Bounds, Color.FloralWhite * .25f);
        }
    }
}
