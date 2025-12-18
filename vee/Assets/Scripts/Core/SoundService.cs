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

        //ボリューム保存用のkeyとデフォルト値
        private const string BGM_VOLUME_KEY = "BGM_VOLUME_KEY";
        private const string SE_VOLUME_KEY = "SE_VOLUME_KEY";
        public const float BGM_VOLUME_DEFULT = 8f;
        public const float SE_VOLUME_DEFULT = 0.5f;

        private void Awake()
        {
            // 保存されたボリュームを読み込んで適用
            if (this.bgmCh != null)
            {
                this.bgmCh.volume = this.getBGMVol();
            }
            if (this.seCh != null && this.seCh.Length > 0)
            {
                float seVol = this.getSEVol();
                foreach (var ch in this.seCh)
                {
                    if (ch != null)
                    {
                        ch.volume = seVol;
                    }
                }
            }
        }

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
            this.currentBgm = "";
        }

        public void UnpauseBgm()
        {
            bgmCh.UnPause();
        }

        public void PauseBgm()
        {
            bgmCh.Pause();
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

        public float getBGMVol()
        {
            return PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
        }
        
        public float getSEVol()
        {
            return PlayerPrefs.GetFloat(SE_VOLUME_KEY, SE_VOLUME_DEFULT);
        }

        /// <summary>
        /// BGMのボリュームを設定する（0.0～1.0の範囲を推奨）
        /// </summary>
        /// <param name="volume">ボリューム値</param>
        public void SetBgmVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
            PlayerPrefs.Save();
            if (this.bgmCh != null)
            {
                this.bgmCh.volume = volume;
            }
        }

        /// <summary>
        /// SEのボリュームを設定する（0.0～1.0の範囲を推奨）
        /// </summary>
        /// <param name="volume">ボリューム値</param>
        public void SetSeVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SE_VOLUME_KEY, volume);
            PlayerPrefs.Save();
            if (this.seCh != null && this.seCh.Length > 0)
            {
                foreach (var ch in this.seCh)
                {
                    if (ch != null)
                    {
                        ch.volume = volume;
                    }
                }
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
