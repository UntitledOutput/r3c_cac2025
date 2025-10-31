using System;
using System.Collections.Generic;
using Controllers;
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
        public AnimationCurve SeverityCurve = AnimationCurve.Linear(0,1,1,1);

        public float GetDetectionRadius(bool isMega) {
            
            
            float rad = DetectionRadius;

            if (isMega)
                rad += MegaDetectionOffset;
            if (MatchController._instance)
            rad += SeverityCurve.Evaluate((float)MatchController._instance.SeverityLevel/MatchController.MaxSeverityLevel) * SvDetectionOffset;

            return rad;
        }


        public float GetStoppingDistance(bool isMega) {
            float dist = StoppingDistance;

            if (isMega)
                dist += MegaStoppingOffset;
            
            if (MatchController._instance)
            dist += SeverityCurve.Evaluate((float)MatchController._instance.SeverityLevel/MatchController.MaxSeverityLevel) * SvStoppingOffset;

            return dist;
        }

        public float GetMaxHealth(bool isMega) {
            float health = HealthMax;

            if (isMega)
                health += MegaHealthOffset;
            
            if (MatchController._instance)
                health += SeverityCurve.Evaluate((float)MatchController._instance.SeverityLevel/MatchController.MaxSeverityLevel) *SvHealthOffset;

            return health;
        }

        public float GetDamageMultiplier(bool isMega) {
            float mul = 1;

            if (isMega)
                mul *= MegaDamageMultiplier;
            if (MatchController._instance)
            mul *= (SeverityCurve.Evaluate((float)MatchController._instance.SeverityLevel/MatchController.MaxSeverityLevel))* SvDamageMultiplier;

            mul = Mathf.Max(mul, 1);
            
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