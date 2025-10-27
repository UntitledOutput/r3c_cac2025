using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace ScriptableObj
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "CAC/New Ability", order = 0)]
    public class AbilityObject : ScriptableObject
    {
        [Header("Display Info.")]
        public string DisplayName;
        [Multiline] public string Description;
        public Sprite Icon;
        

        
        public enum AbilityType
        {
            Shooter,
            Bomb,
            Effect,
            Melee,
            Trigger
        }
        
        public enum ReloadType
        {
            Manual,
            Auto,
        }

        [Header("Ability Info.")]
        public AbilityType Type;
        public ReloadType Reload;
        public ActorBehavior.ActorTeam Target;
        
        public int AmmoCount;
        public float Damage;
        
        [ConditionalField("Reload", false,ReloadType.Auto)]
        public float ReloadTime;
        
        [ConditionalField("Type", false,AbilityType.Bomb)]
        public float BlastRadius;
        
        [ConditionalField("Type", false,AbilityType.Bomb)]
        public float LaunchY;
        
        [ConditionalField("Type", true,new object[]{AbilityType.Effect, AbilityType.Melee}, 0)]
        public float Speed;
        
        [ConditionalField("Type", false,AbilityType.Melee, 0)]
        public float Distance;

        [ConditionalField("Type", false,AbilityType.Melee)]
        public int PossibleTargetCount = 1;

        public float Lifetime;
        
        [ConditionalField("Type", false,AbilityType.Shooter)]
        public float LifetimeLossOnHit;
        
        [ConditionalField("Type", false,AbilityType.Trigger)]
        public string Trigger;


        public GameObject Prefab;
        
        [Serializable]
        public class AbilityUpgrade
        {
            public string Name;
            public string Description;
            public Sprite Icon;

            [Header("Changes")]
            public int AmmoChange;
            public float DamageChange;
            public float ReloadChange;
            public float BlastChange;
            public float LaunchYChange;
            public float SpeedChange;
            public float LifetimeChange;
            public float LossChange;
            public float DistanceChange;
            public int TargetCountChange = 1;

            
            [Header("Prices")]
            public int CulletPrice;
            public int PelletPrice;
            public int SheetPrice;
            
            
        }

        [Header("Upgrade Info.")] 
        public List<AbilityUpgrade> Upgrades;
    }
}