
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Music Object", menuName = "CAC/New Music", order = 0)]
    public class MusicObject : ScriptableObject
    {
        public string Title;
        public AudioClip Clip;

        [Range(0,1)] public float Volume = 1;
        public float LoopStart;
        public float LoopEnd;

    }
}