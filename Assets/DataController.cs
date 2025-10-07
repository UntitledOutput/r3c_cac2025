using System;
using System.Collections.Generic;
using ScriptableObj;
using UnityEngine;

namespace DefaultNamespace
{
    public class DataController : MonoBehaviour
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
        }


        public static GeneralSaveData saveData => Instance._save;

        [SerializeField] private GeneralSaveData _save;

        private void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);

                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
    }
}