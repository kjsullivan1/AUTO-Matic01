using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AUTO_Matic.Scripts.TopDown.AUTO_Matic.Scripts.TopDown;
using AUTO_Matic.Scripts.TopDown;
namespace AUTO_Matic.Scripts.Effects
{
    class ParticleManager
    {
        #region Fields
        //private static GraphicsDevice graphicsDevice;
        //private static List<Particle> particles = new List<Particle>();
        //private static Effect particleEffect;
        private static Texture2D particleTexture;
        private static Random rand = new Random();

        List<Particle> particles = new List<Particle>();
        int particleCount = 200;
        List<ParticleEffect> particleEffects = new List<ParticleEffect>();
        #endregion

        #region Initialization
        public void Initialize(Texture2D texture)
        {
            //graphicsDevice = device;
            //particleEffect = effect;
            particleTexture = texture;
            //Particle.GraphicsDevice = device;


        
        }
        #endregion

        #region Particle Creation

        public void MakeExplosion(Rectangle startRect, Circle boundRect, int width)
        {
            particles.Clear();
            for(int i = 0; i < particleCount; i++)
            {
                //particles.Add(new Particle(new Rectangle(startRect.X + startRect.Width/2, startRect.Y - 1, startRect.Width, startRect.Height), 
                //    rand, width));
                particles.Add(new Particle(startRect,
                    rand, width, ParticleEffect.Type.Explosion));
            }

            ParticleEffect effect;
            effect.effectType = ParticleEffect.Type.Explosion;
            effect.particles = particles;
            effect.boundingRect = new Rectangle();
            effect.boundingCircle = new Circle(new Vector2(boundRect.Bounds.X + boundRect.Bounds.Width/2, boundRect.Bounds.Y + boundRect.Bounds.Height/2), boundRect.Radius);
            //effect.boundingCircle = boundRect;

            particleEffects.Add(effect);


        }

        public void MakeSplash(Rectangle startRect, Circle boundRect, int width)
        {
            particles.Clear();

            for (int i = 0; i < particleCount; i++)
            {
                particles.Add(new Particle(startRect, rand, width, ParticleEffect.Type.Splash));
            }

            ParticleEffect effect;
            effect.effectType = ParticleEffect.Type.Splash;
            effect.particles = particles;
            effect.boundingRect = new Rectangle();
            effect.boundingCircle = new Circle(new Vector2(boundRect.Bounds.X + boundRect.Bounds.Width / 2, boundRect.Bounds.Y + boundRect.Bounds.Height / 2), boundRect.Radius);

            particleEffects.Add(effect);

        }

        public void MakeFireSpit(Rectangle startRect, Circle boundRect, int width)
        {
            particles.Clear();

            for (int i = 0; i < particleCount; i++)
            {
                //particles.Add(new Particle(new Rectangle(startRect.X + startRect.Width/2, startRect.Y - 1, startRect.Width, startRect.Height), 
                //    rand, width));
                particles.Add(new Particle(startRect,
                    rand, width, ParticleEffect.Type.Pillar));
            }

            ParticleEffect effect;
            effect.effectType = ParticleEffect.Type.Pillar;
            effect.particles = particles;
            effect.boundingRect = new Rectangle();
            effect.boundingCircle = new Circle(new Vector2(boundRect.Bounds.X + boundRect.Bounds.Width / 2, boundRect.Bounds.Y + boundRect.Bounds.Height / 2), boundRect.Radius);
            //effect.boundingCircle = boundRect;

            particleEffects.Add(effect);

        }
        #endregion

        public void SetParticles(int particle)
        {
            particleCount = particle;
        }

        #region Update
        public void Update(GameTime gameTime,AUTO_Matic.TopDown.TDPlayer tdPlayer = null,bool top = false)
        {

            for(int j = particleEffects.Count - 1; j >= 0; j--)
            {
                for(int i = particleEffects[j].particles.Count - 1; i >= 0; i--)
                {
                    particleEffects[j].particles[i].position += particleEffects[j].particles[i].velocity;
                    particleEffects[j].particles[i].duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    particleEffects[j].particles[i].rect = new Rectangle((int)particleEffects[j].particles[i].position.X, (int)particleEffects[j].particles[i].position.Y,
                       particleEffects[j].particles[i].rect.Width, particleEffects[j].particles[i].rect.Height);

                    switch(particleEffects[j].effectType)
                    {
                        case ParticleEffect.Type.Explosion:
                            if (top)
                            {
                                if (/*!particleEffects[j].boundingRect.Contains(particleEffects[j].particles[i].position.ToPoint())*/
                            !CollideCircleTop(particleEffects[j].boundingCircle, particleEffects[j].particles[i], tdPlayer) || particleEffects[j].particles[i].duration <= 0)
                                {
                                    particleEffects[j].particles.RemoveAt(i);
                                }
                            }
                            else
                            {
                                if (/*!particleEffects[j].boundingRect.Contains(particleEffects[j].particles[i].position.ToPoint())*/
                            !CollideCircle(particleEffects[j].boundingCircle, particleEffects[j].particles[i]) || particleEffects[j].particles[i].duration <= 0)
                                {
                                    particleEffects[j].particles.RemoveAt(i);
                                }
                            }
                            break;
                        case ParticleEffect.Type.Pillar:
                            if(particleEffects[j].particles[i].duration <= 0 ||
                                MathHelper.Distance(particleEffects[j].particles[i].startPos.Y, particleEffects[j].particles[i].position.Y) > 64)
                            {
                                particleEffects[j].particles.RemoveAt(i);
                            }
                            break;
                        case ParticleEffect.Type.Splash:
                            if(!CollideCircleTop(particleEffects[j].boundingCircle, particleEffects[j].particles[i], tdPlayer) || particleEffects[j].particles[i].duration <= 0)
                            {
                                particleEffects[j].particles.RemoveAt(i);
                            }
                            else if(particleEffects[j].particles[i].rect.Width < particleEffects[j].particles[i].maxWidth)
                            {
                                particleEffects[j].particles[i].position -= new Vector2(1, 1);
                                particleEffects[j].particles[i].rect = new Rectangle(particleEffects[j].particles[i].position.ToPoint(),
                                    new Point(particleEffects[j].particles[i].rect.Width + 1, particleEffects[j].particles[i].rect.Height + 1));
                            }
                            else if(particleEffects[j].particles[i].rect.Width >= particleEffects[j].particles[i].maxWidth || particleEffects[j].particles[i].reahcedPeak)
                            {
                                particleEffects[j].particles[i].reahcedPeak = true;
                                particleEffects[j].particles[i].position += new Vector2(1, 1);
                                particleEffects[j].particles[i].rect = new Rectangle(particleEffects[j].particles[i].position.ToPoint(),
                                    new Point(particleEffects[j].particles[i].rect.Width - 1, particleEffects[j].particles[i].rect.Height - 1));
                            }
                            break;
                    }
                    
                
                   
                }

                if(particleEffects[j].particles.Count == 0)
                {
                    particleEffects.RemoveAt(j);
                }
            }

            
            //foreach (Particle particle in particles)
            //{
            //    if (particle.IsActive)
            //        particle.Update(gameTime);
            //}
        }
        #endregion

        public bool CollideCircle(Circle pos, Particle particle)
        {
            //int div = (int)(ssPlayer.Position.X / 1980);
            //if (div == 0)
            //    div = 1;
            float num = (pos.Bounds.Center.ToVector2() - particle.rect.Center.ToVector2()).Length();
            if ( num < (pos.Radius + (particle.rect.Width/2)))
            {
                return true;
            }
            else
                return false;
        }
        public bool CollideCircleTop(Circle pos, Particle particle, AUTO_Matic.TopDown.TDPlayer tdPlayer)
        {
            int div = tdPlayer.levelInY;
            if (tdPlayer.levelInX < div)
                div = tdPlayer.levelInX;

            float num = (pos.Bounds.Center.ToVector2() - particle.rect.Center.ToVector2()).Length();
            if (num < (pos.Bounds.Center.X + particle.rect.Center.X)/(div * 10))
            {
                return true;
            }
            else
                return false;
        }

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(ParticleEffect effect in particleEffects)
            {
                for(int i = 0; i < effect.particles.Count; i++)
                {
                    spriteBatch.Draw(particleTexture, effect.particles[i].rect, effect.particles[i].color);
                }
            }

        }
        #endregion



    }

    struct ParticleEffect
    {
        public List<Particle> particles;
        public Rectangle boundingRect;
        public Circle boundingCircle;

        public enum Type { Explosion, Pillar, Splash}
        public Type effectType; 
    }

    class Particle
    {
        public Rectangle rect;
        public Vector2 velocity;
        int maxVel = 10;
        public float duration;
        public Vector2 position;
        public Color color;
        public Vector2 startPos;
        public int maxWidth;
        public int minWidth;
        public bool reahcedPeak = false;

        public Particle(Rectangle rect, Random rand, int width, ParticleEffect.Type effectType)
        {
            this.rect = new Rectangle(rect.X, rect.Y, width, width);

            switch (effectType)
            {
                case ParticleEffect.Type.Explosion:
                    
                    velocity = new Vector2(rand.Next(-maxVel, maxVel), rand.Next(-maxVel, maxVel));
                    duration = RandExplosion(0, 2);
                    position = new Vector2(rect.Center.X, rect.Center.Y);

                    RandomColor(rand);
                    break;
                case ParticleEffect.Type.Pillar:

                    velocity = new Vector2(RandPillarX(), rand.Next(-maxVel, 0));
                    duration = RandExplosion(0, 1);
                    position = new Vector2(rect.Center.X, rect.Center.Y);

                    RandomColor(rand);
                    break;
                case ParticleEffect.Type.Splash:
                    velocity = new Vector2(rand.Next((int)(-maxVel / 1.5f), (int)(maxVel / 1.5f)), rand.Next((int)(-maxVel / 1.5f), (int)(maxVel / 1.5f)));
                    duration = RandExplosion(1, 2);
                    position = new Vector2(rect.Center.X, rect.Center.Y);
                    minWidth = width;
                    maxWidth = rand.Next((int)(width * 1.25f), (int)(width * 2.5f));

                    RandomSplashColor(rand);
                    break;
            }

            startPos = position;
        }

        private void RandomColor(Random rand)
        {
            switch (rand.Next(0, 4))
            {
                //red orange yellow white

                case 0:
                    color = Color.Red;
                    break;
                case 1:
                    color = Color.OrangeRed;
                    break;
                case 2:
                    color = Color.Yellow;
                    break;
                case 3:
                    color = Color.WhiteSmoke;
                    break;
            }
        }

        private void RandomSplashColor(Random rand)
        {
            switch(rand.Next(0,4))
            {
                case 0:
                    color = Color.CornflowerBlue;
                    break;
                case 1:
                    color = Color.LightSkyBlue;
                    break;
                case 2:
                    color = Color.LightSteelBlue;
                    break;
                case 3:
                    color = Color.MediumBlue;
                    break;
            }
        }

        public float RandExplosion(int min, int max)
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(min, max).ToString();//number before decimal point
            string afterPoint = r.Next(3, 4).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }

        public float RandPillar(int min, int max)
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(0, 1).ToString();//number before decimal point
            string afterPoint = r.Next(min, max).ToString();
            string afterPoint2 = r.Next(0, 4).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }

        public float RandPillarX()
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(0, 2).ToString();//number before decimal point
            string afterPoint = r.Next(0, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = "";
            if (r.Next(0,2) < 1)
            {
                combined = "-"+beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            }
            else
            {
                combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            }
            
            return decimalNumber = float.Parse(combined);
        }
    
    }
}
