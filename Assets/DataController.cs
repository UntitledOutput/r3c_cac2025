using System;
using System.Collections.Generic;
using Controllers;
using ScriptableObj;
using Unity.VisualScripting;
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

            public int ClothingScrapCount;

            public List<AbilityObject.AbilityUpgrade> availableUpgrades = new List<AbilityObject.AbilityUpgrade>();
            public List<AbilityObject> availableAbilities = new List<AbilityObject>();
            public List<AllyObject.AllyInstance> availableAllies;
                
            public List<EnemyObject> enemyInventory;

            public List<AllyObject.AllyInstance> allies;
            public List<AbilityObject> abilities;
            [DoNotSerialize, HideInInspector] public List<AbilityObject.AbilityUpgrade> upgrades;

            public ClothingObject HatObject;
            public ClothingObject ShirtObject;
            public ClothingObject PantsObject;
            public ClothingObject ShoesObject;

            public List<ClothingObject> availableClothing;

            public List<ClothingObject> BuildListOfClothing()
            {
                List<ClothingObject> clothing = new List<ClothingObject>();

                clothing.Add(HatObject);
                clothing.Add(ShirtObject);
                clothing.Add(PantsObject);
                clothing.Add(ShoesObject);
                
                return clothing;
            }

            public MapObject NextMap;
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