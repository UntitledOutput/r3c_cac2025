using System;
using System.Collections.Generic;
using Controllers;
using ScriptableObj;
using UnityEngine;

namespace DefaultNamespace
{
    public class DataController : IntraDataBehavior
    {
        public static DataController Instance;


        [Serializable]
        public class GeneralSaveData
        {
            public string Name;

            public int GlassCount, PlasticCount, MetalCount;
            public int GlassBitCount, PlasticBitCount, MetalBitCount;

            public List<AbilityObject.AbilityUpgrade> availableUpgrades = new List<AbilityObject.AbilityUpgrade>();
            public List<AbilityObject> availableAbilities = new List<AbilityObject>();
            public List<AllyObject.AllyInstance> availableAllies;
                
            public List<EnemyObject> enemyInventory;

            public List<AllyObject.AllyInstance> allies;
            public List<AbilityObject> abilities;
            public List<AbilityObject.AbilityUpgrade> upgrades;
        }


        public static GeneralSaveData saveData => Instance._save;

        [SerializeField] private GeneralSaveData _save;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                DestroyImmediate(gameObject);

                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

        }

        private void Update()
        {
            if (!Instance)
                Instance = this;
        }
    }
}