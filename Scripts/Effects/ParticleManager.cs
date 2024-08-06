using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public void AddParticle(Vector3 position, Vector3 velocity, float duration, float scale)
        {
            //for (int x = 0; x < particles.Count; x++)
            //{
            //    if (!particles[x].IsActive)
            //    {
            //        particles[x].Activate(position, velocity, duration, scale);
            //        return;
            //    }
            //}
        }

        public void MakeExplosion(Rectangle startRect, Rectangle boundRect, int width)
        {
            particles.Clear();
            for(int i = 0; i < particleCount; i++)
            {
                particles.Add(new Particle(startRect, rand, width));
            }

            ParticleEffect effect;
            effect.particles = particles;
            effect.boundingRect = boundRect;

            particleEffects.Add(effect);


        }
        #endregion

        public void SetParticles(int particle)
        {
            particleCount = particle;
        }

        #region Update
        public void Update(GameTime gameTime)
        {

            for(int j = particleEffects.Count - 1; j >= 0; j--)
            {
                for(int i = particleEffects[j].particles.Count - 1; i >= 0; i--)
                {
                    particleEffects[j].particles[i].position += particleEffects[j].particles[i].velocity;
                    particleEffects[j].particles[i].duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    particleEffects[j].particles[i].rect = new Rectangle((int)particleEffects[j].particles[i].position.X, (int)particleEffects[j].particles[i].position.Y,
                       particleEffects[j].particles[i].rect.Width, particleEffects[j].particles[i].rect.Height);

                    if (!particleEffects[j].boundingRect.Contains(particleEffects[j].particles[i].position.ToPoint()) ||
                        particleEffects[j].particles[i].duration <= 0)
                    {
                        particleEffects[j].particles.RemoveAt(i);
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

            //graphicsDevice.BlendState = BlendState.Additive;
            //particleEffect.CurrentTechnique = particleEffect.Techniques["ParticleTechnique"];

            //particleEffect.Parameters["particleTexture"].SetValue(particleTexture);
            //particleEffect.Parameters["View"].SetValue(camera.transform);
            //particleEffect.Parameters["Projection"].SetValue(camera.transform);

            //graphicsDevice.RasterizerState = RasterizerState.CullNone;
            //graphicsDevice.BlendState = BlendState.Additive;
            //graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            //foreach (Particle particle in particles)
            //{
            //    if (particle.IsActive)
            //        particle.Draw(camera, particleEffect);
            //}

            //graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //graphicsDevice.BlendState = BlendState.Opaque;
            //graphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
        #endregion

       
        public void CreateEffect(int numParticles)
        {
            //for (int x = 0; x < numParticles; x++)
            //{
            //    particles.Add(new Particle());
            //}
        }

        #region Helper Methods
        //public void MakeExplosion(Vector3 position, int particleCount)
        //{
        //    for (int i = 0; i < particleCount; i++)
        //    {
        //        float duration = (float)(rand.Next(0, 20)) / 10f + 2;
        //        float x = ((float)rand.NextDouble() - 0.5f) * 1.5f;
        //        float y = ((float)rand.Next(1, 100)) / 10f;
        //        float z = ((float)rand.NextDouble() - 0.5f) * 1.5f;
        //        float s = (float)rand.NextDouble() + 1.0f;
        //        Vector3 direction = Vector3.Normalize(new Vector3(x, y, 0)) * (((float)rand.NextDouble() * 3f) + 6f);

        //        AddParticle(position + new Vector3(0, -2, 0), direction, duration, s);
        //    }
        //}
        #endregion
    }

    struct ParticleEffect
    {
        public List<Particle> particles;
        public Rectangle boundingRect;
    }

    class Particle
    {
        public Rectangle rect;
        public Vector2 velocity;
        int maxVel = 18;
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
            string afterPoint = r.Next(3, 6).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }
    }
}
