﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTO_Matic.Scripts.Effects
{
    class SoundManager
    {
        Microsoft.Xna.Framework.Audio.SoundEffect sound;
        SoundEffectInstance soundInstance;
        List<SoundEffectInstance> soundList = new List<SoundEffectInstance>();
        ContentManager content;
        bool canLoop = false;
        public string currEffectName = "";


        public SoundManager(string effectName, ContentManager content, bool canLoop)
        {
            sound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(@"Audio\" + effectName);
            this.content = content;
            soundInstance = sound.CreateInstance();
            soundInstance.IsLooped = canLoop;
            currEffectName = effectName;
        }

        public void AddSound(string effectName, bool canLoop, float pitch = -1001100)
        {
            sound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(@"Audio\" + effectName);
            soundInstance = sound.CreateInstance();
            soundInstance.IsLooped = canLoop;
            currEffectName = effectName;
            
            if(pitch != -1001100)
            {
                soundInstance.Pitch = pitch;
            }
            
        }

        public void ClearSounds()
        {

            StopCurrSounds();
            soundList.Clear();
           
        }

        public void StopCurrSounds()
        {
            for(int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Stop();
            }
        }

        public void PlaySound()
        {
            soundList.Add(soundInstance);
            soundList[soundList.Count - 1].Play();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < soundList.Count; i++)
            {
                if (soundList[i].IsLooped)
                {
                    
                }
                else
                {
                    if (soundList[i].State == SoundState.Stopped)
                    {
                        soundList.RemoveAt(i);
                    }

                }
            }
        }
    }
}
