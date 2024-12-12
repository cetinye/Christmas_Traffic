using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Christmas_Traffic
{
    public class AudioManager : MonoBehaviour
    {
        public enum SoundType
        {
            Background,
            BlowUp,
            Collision,
            Correct,
            Road,
            Tap,
            TimesUp
        }

        public static AudioManager Instance;
        public List<Sound> sounds = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (var s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.loop = s.loop;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            CancelInvoke();
            Instance = null;
        }

        public void Play(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (sound == null || sound.source == null)
            {
                Debug.LogWarning($"Sound {name} not found or its AudioSource is null.");
                return;
            }

            sound.source.Play();
        }

        public void PlayOneShot(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (sound == null || sound.source == null)
            {
                Debug.LogWarning($"Sound {name} not found or its AudioSource is null.");
                return;
            }

            sound.source.PlayOneShot(sound.clip);
        }

        public void PlayIf(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (sound == null || sound.source == null)
            {
                Debug.LogWarning($"Sound {name} not found or its AudioSource is null.");
                return;
            }
            if (!sound.source.isPlaying)
                sound.source.Play();
        }

        public void Stop(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (sound == null || sound.source == null)
            {
                Debug.LogWarning($"Sound {name} not found or its AudioSource is null.");
                return;
            }
            sound.source.Stop();
        }

        public AudioSource GetSoundSource(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            return sound.source;
        }

        public void PlayAfterXSeconds(SoundType name, float timeToWait)
        {
            StartCoroutine(DelayedPlay(name, timeToWait));
        }

        private IEnumerator DelayedPlay(SoundType name, float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            var sound = sounds.Find(sound => sound.name == name);
            if (sound == null || sound.source == null)
            {
                Debug.LogWarning($"Sound {name} not found or its AudioSource is null.");
                yield break;
            }
            sound.source.Play();
        }

        public void PlayWFadeOut(SoundType name, float fadeOutAfterXSeconds, float fadeOutTime)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (sound == null || sound.source == null)
            {
                Debug.LogWarning($"Sound {name} not found or its AudioSource is null.");
                return;
            }

            sound.source.PlayOneShot(sound.clip);
            StartCoroutine(FadeOut(sound.source, fadeOutAfterXSeconds, fadeOutTime));
        }

        private IEnumerator FadeOut(AudioSource sound, float fadeOutAfterXSeconds, float fadeOutTime)
        {
            yield return new WaitForSeconds(fadeOutAfterXSeconds);
            sound.DOFade(0f, fadeOutTime).SetEase(Ease.Linear);
        }
    }

    [Serializable]
    public class Sound
    {
        public AudioClip clip;

        [Range(0f, 1f)] public float volume;

        public bool loop;

        [HideInInspector] public AudioSource source;
        public AudioManager.SoundType name;
    }
}