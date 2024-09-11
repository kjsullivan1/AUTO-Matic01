using Microsoft.Xna.Framework;
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
        List<SoundEffectInstance> MusicList = new List<SoundEffectInstance>(); //List of all music
        List<SoundEffectInstance> EffectList = new List<SoundEffectInstance>();
        List<List<SoundEffectInstance>> masterList = new List<List<SoundEffectInstance>>();

        float MasterVolume;
        float EffectVolume;
        float MusicVolume;
      
        List<List<SoundEffectInstance>> MasterList
        {
            get
            {
                masterList.Clear();
                masterList.Add(MusicList);
                masterList.Add(EffectList);

                return masterList;
            }
        }
        ContentManager content;
        bool canLoop = false;
        public string currEffectName = "";


        public SoundManager(string effectName, ContentManager content, bool canLoop, float masterVolume, float effectVolume, float musicVolume)
        {
            sound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(@"Audio\" + effectName);
            this.content = content;
            soundInstance = sound.CreateInstance();
            soundInstance.IsLooped = canLoop;
            currEffectName = effectName;

            this.MasterVolume = masterVolume;
            EffectVolume = effectVolume;
            MusicVolume = musicVolume;
        }

        public void ChangeVolume(float master, float effect,float music)
        {
            MasterVolume = master;
            EffectVolume = effect;
            MusicVolume = music;

            for(int i = 0; i < MasterList.Count; i++)
            {
                for(int j = 0; j < MasterList[i].Count; j++)
                {
                    MasterList[i][j].Volume = master;
                }
            }

            for(int i = 0; i < EffectList.Count; i++)
            {
                EffectList[i].Volume = master + (effect - 1);

                if (EffectList[i].Volume < 0)
                    EffectList[i].Volume = 0;
            }
            for(int i = 0; i < MusicList.Count; i++)
            {
                MusicList[i].Volume = master + (music - 1);
                if (MusicList[i].Volume < 0)
                    MusicList[i].Volume = 0;
            }
        }

        public void AddSound(string effectName, bool canLoop, float pitch = -1001100)
        {
            sound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(@"Audio\" + effectName);
            soundInstance = sound.CreateInstance();
            soundInstance.IsLooped = canLoop;
            //soundInstance.v
            currEffectName = effectName;

           
            if(pitch != -1001100)
            {
                soundInstance.Pitch = pitch;
            }
            
        }

        //public void AddSoundEffect(string effectName, bool canLoop, float pitch = -1001100)
        //{
        //    sound = content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(@"Audio\SoundEffects")
        //}

        public void ClearSounds()
        {

            StopCurrSounds();
            soundList.Clear();
            currEffectName = "";
           
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

            if(currEffectName.Contains("Theme") || currEffectName.Contains("Level"))//Music list
            {
                //soundInstance.Volume = MusicVolume;
                MusicList.Add(soundInstance);
            }
            else if(currEffectName.Contains("SoundEffects"))
            {
                EffectList.Add(soundInstance);
            }

            ChangeVolume(MasterVolume, EffectVolume, MusicVolume);

            soundList.Add(soundInstance);
            soundList[soundList.Count - 1].Play();
        }

        public void Update(GameTime gameTime)
        {
            //for (int i = 0; i < soundList.Count; i++)
            //{
            //    if (soundList[i].IsLooped)
            //    {
                    
            //    }
            //    else
            //    {
            //        if (soundList[i].State == SoundState.Stopped)
            //        {
            //            soundList.RemoveAt(i);
            //        }

            //    }
            //}

            for(int i = 0; i < MasterList.Count; i++)
            {
                for(int j = MasterList[i].Count - 1; j >= 0; j--)
                {
                    if(MasterList[i][j].IsLooped)
                    {
                        
                    }
                    else
                    {
                        if(MasterList[i][j].State == SoundState.Stopped)
                        {
                            MasterList[i].RemoveAt(j);
                        }
                    }
                }
            }
        }
    }
}
