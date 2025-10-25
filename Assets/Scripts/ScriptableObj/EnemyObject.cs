using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "CAC/New Enemy", order = 0)]
    public class EnemyObject : ScriptableObject
    {
        public string DisplayName;
        public GameObject Prefab;
        public Sprite Icon;
        
        [Separator("Stats")]
        
        public float DetectionRadius = 3.5f;
        public float StoppingDistance = 3f;
        public float HealthMax = 1f;

        public float MegaDetectionOffset = 0;
        public float MegaStoppingOffset = 0;
        public float MegaHealthOffset = 0;
        public float MegaDamageMultiplier = 1f;

        [Separator("Model Stats")] 
        public float SmallSize = 1f;
        public float MegaSize = 1.5f;

        public float Width = 1f;
        public float Height = 2f;
        
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

        [Separator("Abilities")]
        
        public List<EnemyAbility> Abilities;
    }
}