using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Match Section", menuName = "CAC/Match Section", order = 0)]
    public class MatchSectionObject : ScriptableObject
    {
        public GameObject Prefab;
        
        [System.Serializable]
        public class SectionList
        {
            public List<MatchSectionObject> list; // Or any other type
        }

        public List<SectionList> AllowedConnections;
    }
}