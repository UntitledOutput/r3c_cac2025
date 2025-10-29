using System;
using System.Collections.Generic;
using External;
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

        [Separator("Mega Stats")]

        public float MegaDetectionOffset = 0;
        public float MegaStoppingOffset = 0;
        public float MegaHealthOffset = 0;
        public float MegaDamageMultiplier = 1f;

        [Separator("Severity Stats")]

        public float SvDetectionOffset = 0;
        public float SvStoppingOffset = 0;
        public float SvHealthOffset = 0;
        public float SvDamageMultiplier = 1f;
        public AnimationCurve SeverityCurve = AnimationCurve.Constant(0,MatchController.MaxSeverityLevel,1);

        public float GetDetectionRadius(bool isMega, int severity) {
            float rad = DetectionRadius;

            if (isMega)
                rad += MegaDetectionOffset;
            
            rad += SeverityCurve.Evaluate(severity) * SvDetectionOffset;

            return rad;
        }


        public float GetStoppingDistance(bool isMega, int severity) {
            float dist = StoppingDistance;

            if (isMega)
                dist += MegaStoppingOffset;
            
            dist += SeverityCurve.Evaluate(severity) * SvStoppingOffset;

            return rad;
        }

        public float GetMaxHealth(bool isMega, int severity) {
            float health = HealthMax;

            if (isMega)
                health += MegaHealthOffset;
            
            health += SeverityCurve.Evaluate(severity) *SvHealthOffset;

            return health;
        }

        public float GetDamageMultiplier(bool isMega, int severity) {
            float mul = 1;

            if (isMega)
                mul += MegaDamageMultiplier;

            mul += SeverityCurve.Evaluate(severity)* SvDamageMultiplier;

            return mul;
        }

        public BaseUtils.WeightedList<CollectibleController.CollectibleType> DropCollectible;

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

        [Separator("Recycling")] public List<AllyObject> PossibleAllies;
    }
}