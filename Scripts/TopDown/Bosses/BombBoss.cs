using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTO_Matic.TopDown;
using Microsoft.Xna.Framework.Content;
using AUTO_Matic.Scripts.Effects;

namespace AUTO_Matic.Scripts.TopDown.Bosses
{
    class BombBoss
    {
        public Rectangle bossRect;
        ContentManager content;
        Random rand = new Random();
        ParticleManager particles;
        float dmgResistance = 2.35f;
        enum BossStates {Shoot};

        #region Animations

        enum AnimationStates { Idle, Shoot}
        AnimationStates animState = AnimationStates.Idle;
        BossHealthBar healthBar;
        AnimationManager animManager;
        Texture2D texture;
        Point FrameSize;//Size of frame
        Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;
        float rotateAngle;
        public void ChangeAnimation()
        {
            switch (animState)
            {
                case AnimationStates.Idle:
                    texture = content.Load<Texture2D>("TopDown/Animations/BombBossSquirm");
                    FrameSize = new Point(128, 128);
                    CurrFrame = new Point(1, 0);
                    SheetSize = new Point(4, 1);
                    fpms = 120;
                    break;
                case AnimationStates.Shoot:
                    texture = content.Load<Texture2D>("TopDown/Animations/BombBossShoot");
                    FrameSize = new Point(128, 128);
                    CurrFrame = new Point(0, 0);
                    SheetSize = new Point(5, 1);
                    fpms = 120;
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

            animManager = new AnimationManager(texture, FrameSize, CurrFrame, SheetSize, fpms, bossRect.Center.ToVector2());

            animManager.isRight = isRight;
            animManager.isLeft = isLeft;
            animManager.isUp = isUp;
            animManager.isDown = isDown;
        }
        #endregion

        #region Shooting

        public float angle;
        public List<Bullet> bullets = new List<Bullet>();
        int growthRate = 5;
        float shootDelay = 1.75f;//In seconds
        float iShootDelay;
        public float bulletDmg = 1.8f;
        public float bulletTravelDist = 64 * 8;
        Rectangle bounds;

        WallSide TopWalls;
        WallSide BottomWalls;
        WallSide RightWalls;
        WallSide LeftWalls;
        List<WallTiles> JumpWalls = new List<WallTiles>();
        List<Explosion> explosions = new List<Explosion>();
        bool locChange = false;
        float locChangeDelay;
        #endregion

        int sizeMod = 2;
        int hitCount = 0;
        int hitsUntilSwitch;
        int hitCountMin = 8;
        int hitCountMax = 15;
        public float bossHealth = 20;

        #region Constructor
        public BombBoss(Rectangle currBounds, ContentManager content, TopDownMap tdMap, int[,] map,
            TDPlayer tdPlayer, GraphicsDevice device, Effect effect, Texture2D explosionTexture)
        {
            int size = 64 * sizeMod;
            hitsUntilSwitch = rand.Next(hitCountMin, hitCountMax);
            bossRect = new Rectangle(((currBounds.X + currBounds.Width / 2) - size / 2), (((currBounds.Y + currBounds.Height / 2) - size / 2)), size, size);
            this.content = content;
            this.bounds = currBounds;
            iShootDelay = shootDelay;
            RightWalls.isUsed = false;
            LeftWalls.isUsed = false;
            TopWalls.isUsed = false;
            BottomWalls.isUsed = false;
            particles = new ParticleManager();
            particles.Initialize(explosionTexture);
            SetWalls(tdMap, map);
            healthBar = new BossHealthBar(new Rectangle(bossRect.X, bossRect.Y - 32, bossRect.Width, bossRect.Height / 3), content);
            bool left = false, top = false, right = false, bottom = false;
            if(MathHelper.Distance(RightWalls.walls[0].Rectangle.X, tdPlayer.rectangle.Center.X) >  
                MathHelper.Distance(LeftWalls.walls[0].Rectangle.Right, tdPlayer.rectangle.Center.X))
            {
                right = true;
            }
            else
            {
                left = true;
            }

            if(MathHelper.Distance(BottomWalls.walls[0].Rectangle.Y, tdPlayer.rectangle.Center.Y) >
                MathHelper.Distance(TopWalls.walls[0].Rectangle.Bottom, tdPlayer.rectangle.Center.Y))
            {
                bottom = true;
            }
            else
            {
                top = true;
            }
           
            if(left && top)
            {
                if(MathHelper.Distance(LeftWalls.walls[0].Rectangle.Right, tdPlayer.rectangle.Center.X) <
                    MathHelper.Distance(TopWalls.walls[0].Rectangle.Bottom, tdPlayer.rectangle.Center.Y))
                {
                    bossRect = new Rectangle(TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.X - bossRect.Width / 4,
                TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.Bottom /*- bossRect.Height/2*/,
                bossRect.Width, bossRect.Height);
                    TopWalls.isUsed = true;
                    //done = true;
                }
                else
                {
                    bossRect = new Rectangle(LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Right,
                      LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Y - bossRect.Height / 4,
                      bossRect.Width, bossRect.Height);
                    LeftWalls.isUsed = true;
                    // done = true;
                }
            }
            else if(left && bottom)
            {
                if (MathHelper.Distance(LeftWalls.walls[0].Rectangle.Right, tdPlayer.rectangle.Center.X) <
                    MathHelper.Distance(BottomWalls.walls[0].Rectangle.Y, tdPlayer.rectangle.Center.Y))
                {
                    bossRect = new Rectangle(BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.X - bossRect.Width / 4,
BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.Y - bossRect.Height/*/2*/,
bossRect.Width, bossRect.Height);
                    BottomWalls.isUsed = true;
                }
                else
                {
                    bossRect = new Rectangle(LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Right,
                LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Y - bossRect.Height / 4,
                bossRect.Width, bossRect.Height);
                    LeftWalls.isUsed = true;
                    //done = true;
                }
            }
            else if(bottom && right)
            {
                if (MathHelper.Distance(RightWalls.walls[0].Rectangle.X, tdPlayer.rectangle.Center.X) <
                  MathHelper.Distance(BottomWalls.walls[0].Rectangle.Y, tdPlayer.rectangle.Center.Y))
                {
                    bossRect = new Rectangle(BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.X - bossRect.Width / 4,
BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.Y - bossRect.Height/*/2*/,
bossRect.Width, bossRect.Height);
                    BottomWalls.isUsed = true;
                }
                else
                {
                    bossRect = new Rectangle(RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.X - bossRect.Width /*/ 2*/,
              RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.Y - bossRect.Height / 4,
              bossRect.Width, bossRect.Height);
                    RightWalls.isUsed = true;
                }
            }
            else if(right && top)
            {
                if (MathHelper.Distance(RightWalls.walls[0].Rectangle.X, tdPlayer.rectangle.Center.X) <
                 MathHelper.Distance(TopWalls.walls[0].Rectangle.Bottom, tdPlayer.rectangle.Center.Y))
                {
                    bossRect = new Rectangle(TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.X - bossRect.Width / 4,
                        TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.Bottom /*- bossRect.Height/2*/,
                        bossRect.Width, bossRect.Height);
                    TopWalls.isUsed = true;
                    //done = true;
                }
                else
                {
                    bossRect = new Rectangle(RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.X - bossRect.Width /*/ 2*/,
              RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.Y - bossRect.Height / 4,
              bossRect.Width, bossRect.Height);
                    RightWalls.isUsed = true;

                }
            }

            ChangeAnimation();
        }
        #endregion
        public void Update(GameTime gameTime, TopDownMap tdMap, TDPlayer tdPlayer)
        {
            if(locChange)
            {
                locChangeDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (locChangeDelay <= 0)
                    locChange = false;
            }
            else
            {
                Vector2 targetDir = new Vector2(tdPlayer.rectangle.X + tdPlayer.rectangle.Width / 2, tdPlayer.rectangle.Y + tdPlayer.rectangle.Height / 2) -
                          new Vector2(bossRect.Center.X, bossRect.Center.Y);
                angle = (float)Math.Atan2(targetDir.Y, targetDir.X);
                rotateAngle = (float)Math.Atan2(targetDir.Y, targetDir.X);
                shootDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;


                #region Shoot&Explosion
                LaunchBomb(tdPlayer);

               // particles.Update(gameTime);
                #endregion
            }

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(gameTime);

                if (bullets[i].rect.Intersects(tdPlayer.rectangle))
                {
                    if (!tdPlayer.damaged)
                    {
                        tdPlayer.Health -= bulletDmg;
                    }
                    bullets[i].delete = true;

                }

                if (bullets[i].delete)
                {
                    explosions.Add(new Explosion(new Circle(new Vector2(bullets[i].rect.X, bullets[i].rect.Y), bullets[i].rect.Width),
                        growthRate, (int)(bullets[i].rect.Width * 2.5f)));

                    int radiusDif = explosions[explosions.Count - 1].maxSize - explosions[explosions.Count - 1].rect.Radius;

                    particles.MakeExplosion(explosions[explosions.Count - 1].rect.Bounds,
                           new Circle(new Vector2(explosions[explosions.Count - 1].rect.Bounds.X - radiusDif,
                           explosions[explosions.Count - 1].rect.Bounds.Y - radiusDif), explosions[explosions.Count - 1].maxSize / 2),
                           20);

                    bullets.RemoveAt(i);
                }
            }

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].rect.Radius >= explosions[i].maxSize)
                {
                    explosions.RemoveAt(i);
                }
            }

            for (int i = tdPlayer.bullets.Count - 1; i >= 0; i--)
            {
                if (tdPlayer.bullets[i].rect.Intersects(bossRect) && !locChange)
                {
                    hitCount++;
                    bossHealth -= tdPlayer.bulletDmg / dmgResistance;
                    tdPlayer.bullets.RemoveAt(i);
                }
            }

            particles.Update(gameTime);
            
            if(hitCount >= hitsUntilSwitch)
            {
                hitsUntilSwitch = rand.Next(hitCountMin, hitCountMax);
                hitCount = 0;
                if (TopWalls.isUsed && BottomWalls.isUsed && RightWalls.isUsed && LeftWalls.isUsed)
                    SetWalls(false);
                ChangeLoc();

            }

            animManager.Update(gameTime, bossRect.Center.ToVector2());

            if (bossRect == new Rectangle(TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.X - bossRect.Width / 4,
              TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.Bottom - bossRect.Height / 2,
              bossRect.Width, bossRect.Height))
            {
                healthBar.Update(new Point(bossRect.Right + 2, bossRect.Center.Y));
            }
            else
            {
                healthBar.Update(new Point(bossRect.X, bossRect.Y - 32));
            }
            if (bossHealth <= 0)
                bossHealth = 0;

            healthBar.ChangeHealth(bossHealth);
        }

        private void SetWalls(bool value)
        {
            TopWalls.isUsed = value;
            BottomWalls.isUsed = value;
            RightWalls.isUsed = value;
            LeftWalls.isUsed = value;
        }

        private void LaunchBomb(TDPlayer tdPlayer)
        {
            if (shootDelay <= 0 && animState == AnimationStates.Idle)
            {
                animState = AnimationStates.Shoot;
                ChangeAnimation();
               
            }
            else if(animState == AnimationStates.Shoot && animManager.GetCurrFrame().X >= animManager.GetSheetSize().X -1)
            {
                shootDelay = iShootDelay;
                bulletTravelDist = DistForm(tdPlayer.rectangle.Center.ToVector2(), bossRect.Center.ToVector2());

                Vector2 bossPos = new Vector2(bossRect.Center.X - tdPlayer.rectangle.Width / 2, bossRect.Center.Y - tdPlayer.rectangle.Height / 2);

                float bulletSpeedX = (float)Math.Cos((double)angle) * 2;
                float bulletSpeedY = (float)Math.Sin((double)angle) * 2;

                bullets.Add(new Bullet(bossPos, bulletSpeedX, new Vector2(bulletSpeedX, bulletSpeedY),
                    content, true, bulletTravelDist, true, bulletSpeedY, size: 42));
                bullets[bullets.Count - 1].BulletType = Bullet.BulletTypes.Bomb;

                animState = AnimationStates.Idle;
                ChangeAnimation();
            }
        }

        private void ChangeLoc()
        {
            bool done = false;
            locChange = true;
            locChangeDelay = RandFloat(5, 9);
            while (!done)
            {

                switch (rand.Next(0, 4))
                {
                    case 0:
                        if (TopWalls.isUsed)
                        {

                        }
                        else
                        {
                            bossRect = new Rectangle(TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.X - bossRect.Width / 4,
              TopWalls.walls[TopWalls.walls.Count / 2].Rectangle.Bottom /*- bossRect.Height/2*/,
              bossRect.Width, bossRect.Height);
                            TopWalls.isUsed = true;
                            done = true;
                        }
                        break;
                    case 1:
                        if (BottomWalls.isUsed)
                        {

                        }
                        else
                        {
                            bossRect = new Rectangle(BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.X - bossRect.Width / 4,
BottomWalls.walls[BottomWalls.walls.Count / 2].Rectangle.Y - bossRect.Height/*/2*/,
bossRect.Width, bossRect.Height);
                            BottomWalls.isUsed = true;
                            done = true;
                        }
                        break;
                    case 2:
                        if (RightWalls.isUsed)
                        {

                        }
                        else
                        {
                            bossRect = new Rectangle(RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.X - bossRect.Width /*/ 2*/,
            RightWalls.walls[RightWalls.walls.Count / 2].Rectangle.Y - bossRect.Height / 4,
            bossRect.Width, bossRect.Height);
                            RightWalls.isUsed = true;
                            done = true;
                        }
                        break;
                    case 3:
                        if (LeftWalls.isUsed)
                        {

                        }
                        else
                        {
                            bossRect = new Rectangle(LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Right,
             LeftWalls.walls[LeftWalls.walls.Count / 2].Rectangle.Y - bossRect.Height / 4,
             bossRect.Width, bossRect.Height);
                            LeftWalls.isUsed = true;
                            done = true;
                        }
                        break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }

           
            for(int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch, content);
            }
            if (!locChange)
            {
                animManager.Draw(spriteBatch, Color.White, rotateAngle, bossRect);
                healthBar.Draw(spriteBatch);
            }
 
            particles.Draw(spriteBatch);
        }

        private void SetWalls(TopDownMap tdMap, int[,] map)
        {
            TopWalls.isUsed = false;
            BottomWalls.isUsed = false;
            RightWalls.isUsed = false;
            LeftWalls.isUsed = false;

            TopWalls.walls = new List<WallTiles>();
            BottomWalls.walls = new List<WallTiles>();
            LeftWalls.walls = new List<WallTiles>();
            RightWalls.walls = new List<WallTiles>();

            for (int i = tdMap.WallTiles.Count - 1; i >= 0; i--)
            {
                if (bounds.Intersects(tdMap.WallTiles[i].Rectangle) == false || bounds.Contains(tdMap.WallTiles[i].Rectangle) == false)
                {
                    tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                }
                else if (tdMap.GetPoint(tdMap.WallTiles[i].mapPoint[0], tdMap.WallTiles[i].mapPoint[1], map) == 10)
                {
                    tdMap.WallTiles.Remove(tdMap.WallTiles[i]);
                }
            }

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (y > 0 && x > 0)
                    {
                        //Possibly different list of walls that are in the map, boss reacts differently to them
                        if (tdMap.dMapDims[tdMap.dMapDims.Count - 1][y, x] == 10)
                        {
                            tdMap.dMapDims[tdMap.dMapDims.Count - 1][y, x] = 9;
                        }
                    }
                    if (y == 0 && x > 0 && x < map.GetLength(1) - 1 ||
                        y == map.GetLength(0) - 1 && x > 0 && x < map.GetLength(1) - 1)
                    {
                        if (map[y, x] != 10)
                        {
                            //Rectangle tRect = Rectangle.Empty;
                            foreach (WallTiles wall in tdMap.WallTiles)
                            {
                                if (wall.mapPoint[0] == y && wall.mapPoint[1] == x)
                                {
                                    if (y == 0)
                                    {
                                        TopWalls.walls.Add(wall);
                                    }
                                    else if (y == map.GetLength(0) - 1)
                                    {
                                        BottomWalls.walls.Add(wall);
                                    }
                                    //tRect = wall.Rectangle;
                                    JumpWalls.Add(wall);
                                    break;
                                }
                            }
                        }
                    }
                    if (y > 0 && y < map.GetLength(0) - 1 && x == 0 ||
                         x == map.GetLength(1) - 1 && y > 0 && y < map.GetLength(0) - 1)
                    {
                        if (map[y, x] != 10)
                        {
                            //Rectangle tRect = Rectangle.Empty;
                            foreach (WallTiles wall in tdMap.WallTiles)
                            {
                                if (wall.mapPoint[0] == y && wall.mapPoint[1] == x)
                                {
                                    if (x == 0)
                                    {
                                        LeftWalls.walls.Add(wall);
                                    }
                                    else if (x == map.GetLength(1) - 1)
                                    {
                                        RightWalls.walls.Add(wall);
                                    }

                                    JumpWalls.Add(wall);

                                    break;
                                }
                            }

                        }
                    }
                }
            }
        }


        int DistForm(Vector2 pos1, Vector2 pos2)
        {
            int num = (int)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
            return num;

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
    }

    struct WallSide
    {
        public List<WallTiles> walls;
        public bool isUsed;
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
