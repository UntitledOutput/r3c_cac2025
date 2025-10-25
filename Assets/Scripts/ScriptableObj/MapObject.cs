using Controllers;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Map Object", menuName = "CAC/New Map Object", order = 0)]
    public class MapObject : ScriptableObject
    {
        public MatchSectionObject StartSection;
        public int SectionCount;

    }
}