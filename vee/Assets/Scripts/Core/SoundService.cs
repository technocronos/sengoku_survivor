using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs
{
    public sealed class SoundService : SingletonMonoBehaviour<SoundService>
    {
        [SerializeField]
        private AudioSource bgmCh;

        [SerializeField]
        private AudioSource[] seCh;

        public bool IsPlayingBgm { get { return this.bgmCh.isPlaying; } }

        private string currentBgm;

        public void PlayBgm(string filename, bool loop = true)
        {
            if (this.currentBgm == filename)
            {
                return;
            }
            this.currentBgm = filename;

            var clip = Resources.Load<AudioClip>($"Bgms/{filename}");
            this.PlayBgm(clip, loop);
        }

        public void PlayBgm(AudioClip clip, bool loop = true)
        {
            this.bgmCh.clip = clip;
            this.bgmCh.loop = loop;
            this.bgmCh.Play();
        }

        public void StopBgm()
        {
            this.bgmCh.Stop();
        }

        public void PlaySe(string filename)
        {
            var clip = Resources.Load<AudioClip>($"Ses/{filename}");
            var seCh = System.Array.Find(this.seCh, i => !i.isPlaying);
            seCh = seCh != null ? seCh : this.seCh[0];
            seCh.PlayOneShot(clip);
        }

        public void StopSe()
        {
            foreach (var i in this.seCh)
            {
                i.Stop();
            }
        }
    }
}
