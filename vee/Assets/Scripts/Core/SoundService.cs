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

        private readonly Dictionary<string, AudioClip> bgmClipsCache = new Dictionary<string, AudioClip>();
        private readonly Dictionary<string, AudioClip> seClipsCache = new Dictionary<string, AudioClip>();

        public bool IsPlayingBgm { get { return this.bgmCh.isPlaying; } }

        private string currentBgm;

        public void PlayBgm(string filename, bool loop = true)
        {
            if (this.currentBgm == filename)
            {
                return;
            }
            this.currentBgm = filename;

            if (!bgmClipsCache.ContainsKey(filename))
            {
                bgmClipsCache.Add(filename, Resources.Load<AudioClip>($"Bgms/{filename}"));
            }
            var clip = bgmClipsCache[filename];
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
            if (!seClipsCache.ContainsKey(filename))
            {
                seClipsCache.Add(filename, Resources.Load<AudioClip>($"Ses/{filename}"));
            }
            var clip = seClipsCache[filename];
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

        private void OnDestroy()
        {
            foreach(var entry in bgmClipsCache)
            {
                Resources.UnloadAsset(entry.Value);
            }
            foreach (var entry in seClipsCache)
            {
                Resources.UnloadAsset(entry.Value);
            }
        }
    }
}
