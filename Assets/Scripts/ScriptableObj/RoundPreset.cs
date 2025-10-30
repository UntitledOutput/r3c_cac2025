using Controllers;
using External;
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
        [Range(0,8)] public int SeverityLevel;

        public BaseUtils.WeightedList<EnemyObject> SmallEnemies;
        public BaseUtils.WeightedList<EnemyObject> BigEnemies;

        
        public MapObject Start;

        public BaseUtils.WeightedList<RoundPreset> PossibleNextRounds;

    }
}