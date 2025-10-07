using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "CAC/New Enemy", order = 0)]
    public class EnemyObject : ScriptableObject
    {
        public string DisplayName;
        public GameObject Prefab;

        public float DetectionRadius = 3.5f;
        public float StoppingDistance = 3f;
        public float HealthMax = 1f;

        [Serializable]
        public enum EnemyAbilityType
        {
            OnChase,
            OnStop,
            Any
        }
        
        [Serializable]
        public class EnemyAbility
        {
            public AbilityObject ability;
            public EnemyAbilityType type;
            public float cooldown;
        }

        public List<EnemyAbility> Abilities;
    }
}