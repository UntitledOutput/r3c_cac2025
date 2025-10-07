using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using External;
using MyBox;
using ScriptableObj;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class MatchController : MonoBehaviour
    {
        public static MatchController _instance;

        [SerializeField] private int _enemiesAlive = 0;

        private Dictionary<CollectibleController.CollectibleType, int> collectibleCount =
            new Dictionary<CollectibleController.CollectibleType, int>();

        public class MapSection
        {
            public MatchSectionObject Ref;

            public List<MapSection> Nexts = new List<MapSection>();

            public MapSection(MatchSectionObject _ref, ref int sectionCount, int maxSectionCount)
            {
                sectionCount++;
                Ref = _ref;

                if (sectionCount < maxSectionCount)
                {
                    for (int i = 0; i < _ref.AllowedConnections.Count; i++)
                    {
                        var saved_count = sectionCount;
                        Nexts.Add(new MapSection(_ref.AllowedConnections[i].list.GetRandom(), ref saved_count, maxSectionCount));
                    }
                }
            }
        }
        
        public class MatchRound
        {
            public MapSection root;

            public List<MatchRound> NextRounds;

            public MatchRound()
            {
                var sectionCt = 0;

                root = new MapSection(Resources.Load<MatchSectionObject>("MapSections/Test00/StartSection"), ref sectionCt, _instance.SectionCount);
            }

            public void GenerateNextRounds()
            {
                var endCount = 0;
                FindEndpoint(ref endCount, root);

                for (int i = 0; i < endCount; i++)
                {
                    NextRounds.Add(new MatchRound());
                }
            }

            private static void FindEndpoint(ref int end, MapSection section)
            {
                if (section.Nexts.Count <= 0)
                {
                    end++;
                }
                else
                {
                    foreach (var sectionNext in section.Nexts)
                    {
                        FindEndpoint(ref end,sectionNext);
                    }
                }
            }
        }

        public MatchRound _currentRound;


        public int SectionCount = 2;
        public int RoundAmount = 2;
        public List<AbilityObject> PassedAbilities;
        public List<AbilityObject.AbilityUpgrade> PassedUpgrades;
        
        private int RoundCount = 0;
        
        public void RegisterEnemy(EnemyController _enemy)
        {
            _enemiesAlive++;
        }

        public void DeregisterEnemy(EnemyController _enemy)
        {
            _enemiesAlive--;
        }

        public void IncrementCollectible(CollectibleController.CollectibleType collectibleType)
        {
            if (!collectibleCount.ContainsKey(collectibleType))
            {
                collectibleCount[collectibleType] = 0;
            }

            collectibleCount[collectibleType]++;

            _collectibleTexts[(int)collectibleType].DOKill();
            
            _collectibleTexts[(int)collectibleType].DOColor(Color.green, 0.5f).OnComplete(
                (() => _collectibleTexts[(int)collectibleType].DOColor(Color.white, 0.5f)));
        }

        [SerializeField] private List<GateController> _gateControllers = new List<GateController>();
        private List<TMP_Text> _collectibleTexts = new List<TMP_Text>();

        public class PostMatchData
        {
            public Dictionary<CollectibleController.CollectibleType, int> newCollectibles = new Dictionary<CollectibleController.CollectibleType, int>();
        }

        public PostMatchData CreatePostMatchData()
        {
            var data = new PostMatchData();

            data.newCollectibles = collectibleCount;
            
            return data;
        }

        private void Start()
        {

            if (_instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            OnSceneLoaded(SceneManager.GetActiveScene());

            SceneManager.sceneLoaded += OnSceneLoaded;

        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            OnSceneLoaded(arg0);
        }

        private void OnSceneLoaded(Scene arg0)
        {
            if (arg0.name == "HomeScene")
                return;

            if (_currentRound == null)
            {
                _currentRound = new MatchRound();
            }
            
            FindAnyObjectByType<MapController>().SetupMap(_currentRound);
            
            _enemiesAlive = 0;
            _gateControllers = FindObjectsByType<GateController>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
            
            if (PassedUpgrades.Count < PassedAbilities.Count)
                while(PassedUpgrades.Count < PassedAbilities.Count)
                    PassedUpgrades.Add(null);
            FindAnyObjectByType<PlayerController>().GetComponent<AbilityController>().SetupAbilities(PassedAbilities,PassedUpgrades);
            
            _collectibleTexts.Clear();

            var scores = BaseUtils.RecursiveFind("ScoreCounters");
            for (int i = 0; i <  (int)CollectibleController.CollectibleType.End; i++)
            {
                var type = (CollectibleController.CollectibleType)i;

                var score = scores.Find(type.ToString());
                var text = score.GetComponentInChildren<TMP_Text>();
                _collectibleTexts.Insert(i,text);
            }
        }

        public void NextStage()
        {
            if (RoundAmount >= RoundCount)
            {
                SceneManager.LoadScene("HomeScene");
            }
            else
            {
                var chosenGate = GateController.ChosenGateIndex;
                GateController.ChosenGateIndex = -1;
                _currentRound = _currentRound.NextRounds[chosenGate];
                
                SceneManager.LoadScene("GameScene");
            }
            RoundAmount++;
        }

        private void Update()
        {
            foreach (var (collectible, value) in collectibleCount)
            {
                var text = _collectibleTexts[(int)collectible];
                text.text = $"{value:00}";
            }

            if (!_instance)
                _instance = this;
        }

        private void LateUpdate()
        {
            if (_enemiesAlive <= 0)
            {
                foreach (var _gateController in _gateControllers)
                {
                    
                    if (!_gateController.IsActive)
                        _gateController.IsActive = true;
                }

            }
            else
            {
                foreach (var _gateController in _gateControllers)
                {
                    
                    _gateController.IsActive = false;
                }
            }
        }
        
        
    }
}