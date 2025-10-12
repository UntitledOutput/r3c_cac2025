using System;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Ally", menuName = "Cac/New Ally", order = 0)]
    public class AllyObject : ScriptableObject
    {
        public string Name;
        public GameObject Prefab;
        public Sprite Icon;
        
        [Serializable]
        public class AllyInstance
        {
            public AllyObject ally;
            public int level = 1;
            public float levelProgress;

            public AllyInstance(AllyObject a)
            {
                ally = a;
            }
        }
    }
}