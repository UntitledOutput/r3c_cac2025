using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using External;
using MyBox;
using ScriptableObj;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        public List<AudioClip> AudioClips; 
        public MusicObject CurrentMusic { get; private set; }

        public void SetMusic(MusicObject m)
        {
            IEnumerator mus()
            {
                yield return new WaitUntil(() => _musicSource.audioSource);
                
                _musicSource.enabled = false;

                yield return new WaitUntil(
                    (() => !_musicSource.audioSource.isPlaying));
                
                _musicSource.audioSource.DOFade(0, 1.0f);

                yield return new WaitForSeconds(1.0f);

                _musicSource.audioSource.clip = m.Clip;
                _musicSource.loopStartTime = m.LoopStart;
                _musicSource.loopEndTime = m.LoopEnd;
                _musicSource.audioSource.volume = m.Volume;
                _musicSource.audioSource.Play();
                
                _musicSource.enabled = true;
                
                _musicSource.audioSource.DOFade(1, 1.0f);

            }

            CurrentMusic = m;

            StartCoroutine(mus());
        }

        public void StopMusic()
        {
            _musicSource.enabled = false;
        }

        private AudioSource _source;
        private AudioLooper _musicSource;

        private void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);

                return;
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;

            ConvertButtons();

            _source = gameObject.AddComponent<AudioSource>();
            _source.volume = 0.5f;
            
            _musicSource = new GameObject("MusicLooper").AddComponent<AudioLooper>();
            _musicSource.transform.parent = transform;
        }

        private void Start()
        {
        }

        private void Update()
        {
            Instance = this;
        }

        public void PlaySound(string _audio)
        {
            foreach (var audioClip in AudioClips)
            {
                if (audioClip.name == _audio)
                {
                    _source.PlayOneShot(audioClip);

                    break;
                }
                else if (_audio == "Untagged" && audioClip.name == "click_003")
                {
                    _source.PlayOneShot(audioClip);

                    break;
                }
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip)
                _source.PlayOneShot(clip);
        }

        public void ConvertButtons()
        {
            var button = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var button1 in button)
            {
                if (button1.GetComponent<UIButton>())
                    continue;

                var sourceEvent = button1.onClick;

                var gb = button1.gameObject;
                DestroyImmediate(button1);
                var uiButton = gb.AddComponent<UIButton>();

                uiButton.onClick = sourceEvent;

                // for (int i = 0; i < sourceEvent.GetPersistentEventCount(); i++)
                // {
                //     // Get information about the persistent listener from the source event
                //     Object targetObject = sourceEvent.GetPersistentTarget(i);
                //     string methodName = sourceEvent.GetPersistentMethodName(i);
                //
                //     Debug.Log(methodName);
                //     
                //     uiButton.AddPersistentListener(methodName, targetObject);
                // }
            }
        }
    }
}