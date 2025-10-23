using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Ally", menuName = "Cac/New Ally", order = 0)]
    public class AllyObject : ScriptableObject
    {
        
        public enum AllyType
        {
            Support,
            Offensive,
            Defensive
        }
        
        public string Name;
        public GameObject Prefab;
        public Sprite Icon;

        public AllyType Type;
        public List<AbilityObject> abilities;
        
        
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