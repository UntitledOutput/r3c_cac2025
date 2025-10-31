using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using External;
using MyBox;
using ScriptableObj;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class MatchController : IntraDataBehavior
    {
        public static MatchController _instance;

        public static int MaxSeverityLevel = 8;
        public int SeverityLevel;

        [SerializeField] private int _enemiesAlive = 0;
        public float timeCount { get; private set; }

        public bool IsPlaying { get; private set; } = true;

        public float timeLeft => CurrentMap.TimeLimitInSeconds - timeCount;
        
        private Dictionary<CollectibleController.CollectibleType, int> collectibleCount =
            new Dictionary<CollectibleController.CollectibleType, int>();

        private List<EnemyObject> caughtEnemies = new List<EnemyObject>();

        [Serializable]
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
        
        [Serializable]
        public class MatchRound
        {
            public MapSection root;

            public List<MatchRound> NextRounds = new List<MatchRound>();
            public bool IsSetup { get; private set; }

            public MatchRound()
            {

            }

            public void Setup()
            {
                var sectionCt = 0;
                root = new MapSection(_instance.CurrentMap.StartSection, ref sectionCt, DataController.saveData.NextMap.SectionCount);
                GenerateNextRounds();
                IsSetup = true;
            }

            public void GenerateNextRounds()
            {
                var endCount = 0;
                FindEndpoint(ref endCount, root);

                for (int i = 0; i < endCount; i++)
                {
                    var round = new MatchRound();
                    NextRounds.Add(round);
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
        
        public int RoundAmount = 1;
        public List<AbilityObject> PassedAbilities;
        public List<AbilityObject.AbilityUpgrade> PassedUpgrades;
        public List<AllyObject.AllyInstance> PassedAllies;

        
        public MapObject CurrentMap;
 
        [ReadOnly] public int RoundProgress { get; private set; }
        private int RoundsFullyCompleted;

        public void RegisterEnemy(EnemyController _enemy)
        {
            _enemiesAlive++;
        }

        public void DeregisterEnemy(EnemyController _enemy)
        {
            _enemiesAlive--;
        }

        public void IncrementCollectible(CollectibleController.CollectibleType collectibleType, int count =1 )
        {
            if (!collectibleCount.ContainsKey(collectibleType))
            {
                collectibleCount[collectibleType] = 0;
            }

            collectibleCount[collectibleType] += count;

            _collectibleTexts[(int)collectibleType].DOKill();
            
            _collectibleTexts[(int)collectibleType].DOColor(Color.green, 0.5f).OnComplete(
                (() => _collectibleTexts[(int)collectibleType].DOColor(Color.white, 0.5f)));
        }

        public void AddCaughtEnemy(EnemyObject enemy)
        {
            caughtEnemies.Add(enemy);
        }
        

        [SerializeField] private List<GateController> _gateControllers = new List<GateController>();
        private List<TMP_Text> _collectibleTexts = new List<TMP_Text>();

        public class PostMatchData
        {
            public int roundCount;
            public int roundProgress;
            public bool won => roundProgress >= roundCount && !Failed;
            
            public Dictionary<CollectibleController.CollectibleType, int> newCollectibles = new Dictionary<CollectibleController.CollectibleType, int>();
            public List<EnemyObject> CaughtEnemies;
            public bool Failed = false;
            
        }

        private bool _Failed = false;

        public PostMatchData CreatePostMatchData()
        {
            var data = new PostMatchData();

            data.roundCount = RoundAmount;
            data.roundProgress = RoundProgress;
            
            data.newCollectibles = collectibleCount;
            data.CaughtEnemies = caughtEnemies;
            data.Failed = _Failed;
            
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

            _currentRound = null;
            RoundProgress = 1;
            
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

            
            IsPlaying = true;
            
            if (_currentRound == null)
            {
                _currentRound = new MatchRound();
            }
            
            if (!_currentRound.IsSetup)
                _currentRound.Setup();
            
            FindAnyObjectByType<MapController>().SetupMap(_currentRound);

            timeCount = 0;
            _enemiesAlive = 0;
            _gateControllers = FindObjectsByType<GateController>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
            
            
            if (PassedUpgrades.Count < PassedAbilities.Count)
                while(PassedUpgrades.Count < PassedAbilities.Count)
                    PassedUpgrades.Add(null);
            FindAnyObjectByType<PlayerController>().GetComponent<AbilityController>().SetupAbilities(PassedAbilities,PassedUpgrades);
            
            _collectibleTexts.Clear();
            
            SoundManager.Instance.SetMusic(Resources.Load<MusicObject>("Music/Settings/GameScene00"));

            var scores = BaseUtils.RecursiveFind("ScoreCounters");
            for (int i = 0; i <  (int)CollectibleController.CollectibleType.End; i++)
            {
                var type = (CollectibleController.CollectibleType)i;

                var score = scores.Find(type.ToString());
                var text = score.GetComponentInChildren<TMP_Text>();
                _collectibleTexts.Insert(i,text);
            }
            
            LoadingScreenController.LoadingScreen.CloseLoadingScreen();
            
        }

        public void NextStage()
        {
            IsPlaying = false;
            RoundProgress++;
            if (RoundProgress > RoundAmount)
            {

                ReturnToHome();
            }
            else
            {
                var chosenGate = GateController.ChosenGateIndex;
                GateController.ChosenGateIndex = -1;
                _currentRound = _currentRound.NextRounds[chosenGate];
                timeCount = 0;
                IsPlaying = true;
                
                LoadingScreenController.LoadingScreen.OpenLoadingScreen((() =>
                {
                    SceneManager.LoadScene("GameScene");
                }));
                

            }
        }

        public void ReturnToHome(bool fail = false)
        {
            if (fail)
                _Failed = true;
            LoadingScreenController.LoadingScreen.OpenLoadingScreen((() =>
            {
                SceneManager.LoadScene("HomeScene");
            }));

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

            if (IsPlaying)
            {
                timeCount += Time.deltaTime;

                if (timeCount >= CurrentMap.TimeLimitInSeconds)
                {
                    IsPlaying = false;
                    FindAnyObjectByType<GameUIController>().Fail("You ran out of time.");
                }
            }
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