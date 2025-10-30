using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace External
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioLooper : MonoBehaviour
    {
        [SerializeField] public float loopStartTime;
        [SerializeField] public float loopEndTime;

        [SerializeField] private int loopStartSamples;
        [SerializeField] private int loopEndSamples;
        [SerializeField] private int loopLengthSamples;

        public AudioSource audioSource;

        private void Start()
        {
            audioSource = gameObject.GetOrAddComponent<AudioSource>();

        }

        public UnityEvent OnLoop;

        private void Update()
        {
            audioSource.loop = false;
            if (audioSource.clip)
            {
                loopStartSamples = (int)(loopStartTime * audioSource.clip.frequency);
                loopEndSamples = (int)(loopEndTime * audioSource.clip.frequency);
                loopLengthSamples = loopEndSamples - loopStartSamples;
                if (audioSource.timeSamples >= loopEndSamples)
                {
                    audioSource.timeSamples -= loopLengthSamples;
                    OnLoop.Invoke();
                }
            }
        }
    }
}