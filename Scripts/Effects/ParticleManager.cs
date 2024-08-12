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
                    rand, width));
            }

            ParticleEffect effect;
            effect.particles = particles;
            effect.boundingRect = new Rectangle();
            effect.boundingCircle = new Circle(new Vector2(boundRect.Bounds.X + boundRect.Bounds.Width/2, boundRect.Bounds.Y + boundRect.Bounds.Height/2), boundRect.Radius);
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
                    if(top)
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
            float num = (pos.Bounds.Center.ToVector2() - particle.rect.Center.ToVector2()).Length();
            if ( num < (pos.Bounds.Center.X + particle.rect.Center.X)/64)
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
    }

    class Particle
    {
        public Rectangle rect;
        public Vector2 velocity;
        int maxVel = 16;
        public float duration;
        public Vector2 position;
        public Color color;

        public Particle(Rectangle rect, Random rand, int width)
        {
            this.rect = new Rectangle(rect.X, rect.Y, width, width);
            velocity = new Vector2(rand.Next(-maxVel, maxVel), rand.Next(-maxVel, maxVel));
            duration = RandExplosion(0, 2);
            position = new Vector2(rect.X, rect.Y);

            RandomColor(rand);
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

        public float RandExplosion(int min, int max)
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(min, max).ToString();//number before decimal point
            string afterPoint = r.Next(9, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }
    }
}
