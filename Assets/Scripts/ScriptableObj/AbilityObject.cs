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
            Melee
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
        
        [ConditionalField("Type", true,AbilityType.Effect)]
        public float Speed;

        public float Lifetime;
        
        [ConditionalField("Type", false,AbilityType.Shooter)]
        public float LifetimeLossOnHit;


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
            
            [Header("Prices")]
            public int CulletPrice;
            public int PelletPrice;
            public int SheetPrice;
            
            
        }

        [Header("Upgrade Info.")] 
        public List<AbilityUpgrade> Upgrades;
    }
}