using Controllers;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Round Preset", menuName = "CAC/New Round Preset", order = 0)]
    public class RoundPreset : ScriptableObject
    {
        public int RoundCount;
        public int SectionCount;
        public Sprite MapCover;
        public string MapName;
        public int SeverityLevel;

        public MapObject Start;

    }
}