
using System;
using Controllers;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using External;
using MyBox;
using ScriptableObj;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class HomeController : MonoBehaviour
{
    
    public List<AbilityObject> abilities => DataController.saveData.abilities;
    public List<AbilityObject.AbilityUpgrade> upgrades => DataController.saveData.upgrades;
    public List<AllyObject.AllyInstance> allies => DataController.saveData.allies;

    public bool IsReadyToSpin = true;
    private int SectionIndex = 0;
    
    
    
    public enum SubRecycleType
    {
        Glass = 0,
        Plastic,
        Metal
    }

    [ButtonMethod]
    public void MoveRight()
    {
        SectionIndex = (SectionIndex + 1 > 3) ? 0 : SectionIndex + 1;
        AlignUISection();
    }
    
    [ButtonMethod]
    public void MoveLeft()
    {
        SectionIndex = (SectionIndex - 1 < 0) ? 3 : SectionIndex - 1;
        AlignUISection();
    }

#if UNITY_EDITOR
    [Separator("Debug Settings")] 
    public int SectionCount = 1;
    public int RoundCount = 1;
#endif
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playFrame = BaseUtils.RecursiveFind("PlayFrame") as RectTransform;
        _playAbilityIcons = _playFrame.RecursiveFind("AbilityIconContent").GetAllChildren()
            .Select((transform1 => transform1.GetChild(0).GetComponent<Image>())).ToList();
        
        _changeFrame = _playFrame.parent.Find("ChangeFrame") as RectTransform;
        
        _upgradeFrame = _changeFrame.Find("AbilitySelectFrame") as RectTransform;
        _homeFrame = _changeFrame.Find("HomeFrame") as RectTransform;
        _allyFrame = _changeFrame.Find("AllySelectFrame") as RectTransform;
        
        _homeAbilityIcons = _changeFrame.RecursiveFind("HomeAbilityContent").GetAllChildren()
            .Select((transform1 => transform1.GetChild(0).GetComponent<Image>())).ToList();
        
        _homeAllyIcons = _changeFrame.RecursiveFind("HomeAllyContent").GetAllChildren()
            .Select((transform1 => transform1.GetChild(0).GetComponent<Image>())).ToList();
        
        _upgradeContent = _changeFrame.RecursiveFind<RectTransform>("UpgradeContent");
        _upgradeTemplate = _upgradeContent.Find("UpgradeTemplate") as RectTransform;

        _selectPreview = _changeFrame.RecursiveFind("SelectPreview") as RectTransform;
        _selectContent = _changeFrame.RecursiveFind("SelectContent") as RectTransform;
        _selectTemplate = _selectContent.RecursiveFind<RectTransform>("SelectTemplate");
        
        _allyPreview = _allyFrame.RecursiveFind("AllySelectPreview") as RectTransform;
        _allyContent = _allyFrame.RecursiveFind("AllySelectContent") as RectTransform;
        _allyTemplate = _allyFrame.RecursiveFind<RectTransform>("AllySelectTemplate");
        
        _recycleFrame = _playFrame.parent.Find("RecycleFrame") as RectTransform;
        _recycleGlassCount = _recycleFrame.RecursiveFind<TMP_Text>("GlassCount");
        _recyclePlasticCount = _recycleFrame.RecursiveFind<TMP_Text>("PlasticCount");
        _recycleMetalCount = _recycleFrame.RecursiveFind<TMP_Text>("MetalCount");
        _recycleGlassBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubGlassCount");
        _recyclePlasticBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubPlasticCount");
        _recycleMetalBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubMetalCount");
        
        _recycleEnemyContent = _recycleFrame.RecursiveFind("EnemyContent") as RectTransform;
        _recycleAllyContent = _recycleFrame.RecursiveFind("AllyContent") as RectTransform;
        _recycleTryContent = _recycleFrame.RecursiveFind("TryContent") as RectTransform;
        _recycleAllyInfo = _recycleFrame.RecursiveFind("AllyInfo") as RectTransform;
        _recycleAllyFrame = _recycleFrame.RecursiveFind("EnemyRecycleFrame") as RectTransform;
        _recycleResultFrame = _recycleFrame.RecursiveFind("RecycleResultFrame") as RectTransform;
        
        _recycleEnemyTemplate = _recycleEnemyContent.RecursiveFind<RectTransform>("EnemyButtonTemplate");
        _recycleAllyTemplate = _recycleAllyContent.RecursiveFind<RectTransform>("AllyButtonTemplate");

        
        _postMatchFrame = _playFrame.parent.Find("PostMatchFrame") as RectTransform;
        _postInfoFrame = _postMatchFrame.RecursiveFind("InfoFrame")  as RectTransform;
        _postFailFrame = _postMatchFrame.RecursiveFind("FailFrame")  as RectTransform;
        _postTrashInfo = _postInfoFrame.RecursiveFind("TrashInfo") as RectTransform;
        _postGlassInfo = _postInfoFrame.RecursiveFind("GlassSection") as RectTransform;
        _postPlasticInfo = _postInfoFrame.RecursiveFind("PlasticSection") as RectTransform;
        _postMetalInfo = _postInfoFrame.RecursiveFind("MetalSection") as RectTransform;
        
        if (upgrades.Count < abilities.Count)
        {
            while (upgrades.Count < abilities.Count)
            {
                upgrades.Add(null);
            }
        }
        
        if (allies.Count < 3)
        {
            while (allies.Count < 3)
            {
                allies.Add(null);
            }
        }
        
        AlignUISection();

        var matchCtrl = FindAnyObjectByType<MatchController>();
        if (matchCtrl)
        {
            var postMatchData = matchCtrl.CreatePostMatchData();

            DestroyImmediate(matchCtrl.gameObject);
            
            RunPostMatchSequence(postMatchData);
        }
        else
        {
            // var testPostData = new MatchController.PostMatchData();
            // testPostData.roundCount = 5;
            // testPostData.roundProgress = 5;
            // testPostData.newCollectibles[CollectibleController.CollectibleType.GenericTrash] = 500;
            //
            // RunPostMatchSequence(testPostData);
        }
        
                                
        SetupPlayFrame();
        SetupUpgradeFrame();
        SetupRecycleFrame();
        

        
    }

    private void Update()
    {
         if (!_isPostInfoRunning)
         {
             if (InputSystem.NavigateNext)
                MoveRight();
             else if (InputSystem.NavigateBack)
                 MoveLeft();
             
         }

         if (upgrades.Count < abilities.Count)
         {
             while (upgrades.Count < abilities.Count)
             {
                 upgrades.Add(null);
             }
         }
         
         _recyclePlasticCount.text = $"{DataController.saveData.PlasticCount:00}";
         _recycleGlassCount.text = $"{DataController.saveData.GlassCount:00}";
         _recycleMetalCount.text = $"{DataController.saveData.MetalCount:00}";
         
         _recyclePlasticBitCount.text = $"{DataController.saveData.PlasticBitCount:00}";
         _recycleGlassBitCount.text = $"{DataController.saveData.GlassBitCount:00}";
         _recycleMetalBitCount.text = $"{DataController.saveData.MetalBitCount:00}";
         
    }

    public void StartMatch()
    {
        var matchController = new GameObject().AddComponent<MatchController>();
        matchController.PassedAbilities = abilities;
        matchController.PassedUpgrades = upgrades;
        matchController.PassedAllies = allies;

#if UNITY_EDITOR
        matchController.RoundAmount = RoundCount;
        matchController.SectionCount = SectionCount;
#endif
        
        SceneManager.LoadScene("GameScene");
    }
    
    // ui section

    private Sprite MissingSprite = null;

    private RectTransform _playFrame;
    private List<Image> _playAbilityIcons = new List<Image>();
    

    private RectTransform _changeFrame;

    private RectTransform _homeFrame;
    private List<Image> _homeAbilityIcons = new List<Image>();
    private List<Image> _homeAllyIcons = new List<Image>();
    
    private RectTransform _upgradeFrame;
    private RectTransform _upgradeContent;
    private RectTransform _upgradeTemplate;
    
    private RectTransform _selectPreview;
    private RectTransform _selectContent;
    private RectTransform _selectTemplate;
    private int _selectedAbilitySlot = -1;

    private RectTransform _allyFrame;
    
    
    private RectTransform _allyPreview;
    private RectTransform _allyContent;
    private RectTransform _allyTemplate;
    private int __selectedAllySlot = -1;
    

    private RectTransform _recycleFrame;
    private TMP_Text _recycleGlassCount;
    private TMP_Text _recyclePlasticCount;
    private TMP_Text _recycleMetalCount;
    private TMP_Text _recycleGlassBitCount;
    private TMP_Text _recyclePlasticBitCount;
    private TMP_Text _recycleMetalBitCount;

    private RectTransform _recycleEnemyContent;
    private RectTransform _recycleAllyContent;
    private RectTransform _recycleTryContent;
    private RectTransform _recycleAllyInfo;
    private RectTransform _recycleAllyFrame;
    private RectTransform _recycleEnemyTemplate;
    private RectTransform _recycleAllyTemplate;
    private RectTransform _recycleResultFrame;
    private List<EnemyObject> _recycleEnemies = new List<EnemyObject>();
    private int _selectedAllySlot = -1;
    
    private RectTransform _postMatchFrame;
    private RectTransform _postInfoFrame;
    private RectTransform _postFailFrame;
    private RectTransform _postTrashInfo;
    private RectTransform _postGlassInfo, _postPlasticInfo, _postMetalInfo;
    private bool _isPostInfoRunning = false;

    public void AlignUISection()
    {
        
        _playFrame.gameObject.SetActive(false);
        _changeFrame.gameObject.SetActive(false);
        _recycleFrame.gameObject.SetActive(false);
        _postMatchFrame.gameObject.SetActive(false);
        switch (SectionIndex)
        {
            case 0:
                _playFrame.gameObject.SetActive(true);
                SetupPlayFrame();
                break;
            case 1:
                _changeFrame.gameObject.SetActive(true);
                SetupUpgradeFrame();
                break;
            case 2:
                _recycleFrame.gameObject.SetActive(true);
                SetupRecycleFrame();
                break;
        }
    }
    
    public void SetupPlayFrame()
    {

    }

    public void UpdateIcons()
    {
        for (var i = 0; i < abilities.Count; i++)
        {
            if (abilities[i])
            {
                _homeAbilityIcons[i].sprite = abilities[i].Icon;
                _playAbilityIcons[i].sprite = abilities[i].Icon;
                
            }
            else
            {
                _homeAbilityIcons[i].sprite = MissingSprite;
                _playAbilityIcons[i].sprite = MissingSprite;
            }
        }

        for (var i = 0; i < allies.Count; i++)
        {
            if (allies[i] != null && allies[i].ally)
            {
                _homeAllyIcons[i].sprite = allies[i].ally.Icon;
            }
            else
            {
                _homeAllyIcons[i].sprite = MissingSprite;
            }
        }
    }
    
    public void SetupUpgradeFrame()
    {
        UpdateIcons();
        
        _selectContent.RemoveAllChildrenExcept(_selectTemplate.name);

        {
            var upgradeFrame = Instantiate(_selectTemplate, _selectTemplate.parent);
            upgradeFrame.Find("AbilityIcon").GetComponent<Image>().sprite = MissingSprite;
            upgradeFrame.GetComponent<Button>().onClick.AddListener((() => ChangeAbilityInSlot(null,_selectedAbilitySlot)));
            
            upgradeFrame.gameObject.SetActive(true);
        }
        
        foreach (var ability in DataController.saveData.availableAbilities)
        {
            var upgradeFrame = Instantiate(_selectTemplate, _selectTemplate.parent);
            upgradeFrame.Find("AbilityIcon").GetComponent<Image>().sprite = ability.Icon;
            upgradeFrame.GetComponent<Button>().onClick.AddListener((() => ChangeAbilityInSlot(ability,_selectedAbilitySlot)));
            
            upgradeFrame.gameObject.SetActive(true);
        }

        SetupAllyFrame();
        
        //ChangeSelectSection(0);
    }

    public void SetupAllyFrame()
    {
        UpdateIcons();
        
        _allyContent.RemoveAllChildrenExcept(_allyTemplate.name);

        {
            var upgradeFrame = Instantiate(_allyTemplate, _allyTemplate.parent);
            upgradeFrame.Find("AllyIcon").GetComponent<Image>().sprite = MissingSprite;
            upgradeFrame.GetComponent<Button>().onClick.AddListener((() => ChangeAllyInSlot(null,__selectedAllySlot)));
            
            upgradeFrame.gameObject.SetActive(true);
        }
        
        for (var i = 0; i < DataController.saveData.availableAllies.Count; i++)
        {
            var ally = DataController.saveData.availableAllies[i];
            if (ally != null && ally.ally)
            {
                var upgradeFrame = Instantiate(_allyTemplate, _allyTemplate.parent);
                upgradeFrame.Find("AllyIcon").GetComponent<Image>().sprite = ally.ally.Icon;
                var i1 = i;
                upgradeFrame.GetComponent<Button>().onClick
                    .AddListener((() => ChangeAllyInSlot(DataController.saveData.availableAllies[i1], __selectedAllySlot)));

                upgradeFrame.gameObject.SetActive(true);
            }
        }

    }

    public void CloseSelectSection()
    {
        _upgradeFrame.gameObject.SetActive(false);
        _homeFrame.gameObject.SetActive(true);
    }

    public void CloseAllySection()
    {
        _allyFrame.gameObject.SetActive(false);
        _homeFrame.gameObject.SetActive(true);
    }

    public void ChangeAllySection(int allyIndex)
    {
        _allyFrame.gameObject.SetActive(true);
        _homeFrame.gameObject.SetActive(false);

        __selectedAllySlot = allyIndex;
        ChangeAllyPreview(allies[allyIndex]);

        var ally = allies[allyIndex];
        
        _allyFrame.Find("AllyButton").GetChild(0).GetComponent<Image>().sprite =
            (ally != null && ally.ally) ? ally.ally.Icon : MissingSprite;
        
    }
    
    public void ChangeSelectSection(int abilityIndex)
    {
        _upgradeFrame.gameObject.SetActive(true);
        _homeFrame.gameObject.SetActive(false);
        
        _selectedAbilitySlot = abilityIndex;
        ChangeSelectPreview(abilities[abilityIndex]);

        var ability = abilities[abilityIndex];
        
        _upgradeContent.RemoveAllChildrenExcept(_upgradeTemplate.name);

        _upgradeFrame.Find("AbilityButton").GetChild(0).GetComponent<Image>().sprite =
            ability != null ? ability.Icon : MissingSprite;

        if (ability)
        {
            foreach (var abilityUpgrade in ability.Upgrades)
            {
                var upgradeFrame = Instantiate(_upgradeTemplate, _upgradeTemplate.parent);
                upgradeFrame.Find("UpgradeName").GetComponent<TMP_Text>().text = abilityUpgrade.Name;
                //upgradeFrame.Find("UpgradeDesc").GetComponent<TMP_Text>().text = abilityUpgrade.Description;
                upgradeFrame.Find("UpgradeIcon").GetComponent<Image>().sprite = abilityUpgrade.Icon;

                upgradeFrame.RecursiveFind("CulletCost").GetComponent<TMP_Text>().text = abilityUpgrade.CulletPrice.ToString();
                upgradeFrame.RecursiveFind("PelletCost").GetComponent<TMP_Text>().text = abilityUpgrade.PelletPrice.ToString();
                upgradeFrame.RecursiveFind("MetalCost").GetComponent<TMP_Text>().text = abilityUpgrade.SheetPrice.ToString();

                Color statusColor = BaseUtils.ColorFromHex("8E8E8E");
                string statusString = "Unavailable";

                if (upgrades[abilityIndex] == abilityUpgrade ) // upgrade is bought and active
                {

                    statusColor = BaseUtils.ColorFromHex("0BD149");
                    statusString = "Active";
                }
                else // upgrade is inactive
                {
                    if (DataController.saveData.availableUpgrades.Contains(abilityUpgrade))  // upgrade is bought but inactive
                    {
                        statusColor = BaseUtils.ColorFromHex("E55858");
                        statusString = "Inactive";
                    } else  // upgrade is not bought
                    {
                        if (abilityUpgrade.CulletPrice <= DataController.saveData.GlassBitCount &&
                            abilityUpgrade.PelletPrice <= DataController.saveData.PlasticBitCount &&
                            abilityUpgrade.SheetPrice <= DataController.saveData.MetalBitCount)
                        {
                            // upgrade is available to buy
                            statusColor = BaseUtils.ColorFromHex("ECB52C");
                            statusString = "Available";
                        }
                        else
                        {
                            // upgrade is unavailable to buy
                            statusColor = BaseUtils.ColorFromHex("8E8E8E");
                            statusString = "Unavailable";
                        }
                    }
                }

                upgradeFrame.Find("StatusField").GetComponent<Image>().color = statusColor;
                upgradeFrame.Find("StatusField").GetComponentInChildren<TMP_Text>().text = statusString;
                
                upgradeFrame.GetComponent<Button>().onClick.AddListener((() =>
                {
                    if (DataController.saveData.availableUpgrades.Contains(abilityUpgrade))
                    {
                        if (upgrades[abilityIndex] != abilityUpgrade)
                        {
                            upgrades[abilityIndex] = abilityUpgrade;
                        }
                        else
                        {
                            upgrades[abilityIndex] = null;
                        }
                    }
                    else
                    {
                        // buying an upgrade

                        if (abilityUpgrade.CulletPrice <= DataController.saveData.GlassBitCount &&
                            abilityUpgrade.PelletPrice <= DataController.saveData.PlasticBitCount &&
                            abilityUpgrade.SheetPrice <= DataController.saveData.MetalBitCount)
                        {
                            DataController.saveData.availableUpgrades.Add(abilityUpgrade);
                            upgrades[abilityIndex] = abilityUpgrade;
                            DataController.saveData.GlassBitCount -= abilityUpgrade.CulletPrice;
                            DataController.saveData.PlasticBitCount -= abilityUpgrade.PelletPrice;
                            DataController.saveData.MetalBitCount -= abilityUpgrade.SheetPrice;
                        } 
                    }

                    ChangeSelectSection(abilityIndex);
                }));

                upgradeFrame.gameObject.SetActive(true);
            }
        }
    }

    public void ChangeAbilityInSlot(AbilityObject ability, int abilityIndex)
    {
        if (ability)
        {
            foreach (var abilityObject in abilities)
            {
                if (abilityObject == ability || abilityObject.Type == ability.Type)
                {
                    return;
                }
            }
        }
        else
        {
            var found = 0;
            foreach (var abilityObject in abilities)
            {
                if (abilityObject != null)
                {
                    found++;
                }
            }

            // if the only ability listed is the one being changed to null
            if (found == 1 && abilities[abilityIndex] != null)
            {
                return;
            }
        }

        abilities[abilityIndex] = ability;
        upgrades[abilityIndex] = null;
        
        UpdateIcons();
        ChangeSelectSection(abilityIndex);
    }

    public void ChangeAllyInSlot(AllyObject.AllyInstance ally, int allyIndex)
    {
        if (ally != null && ally.ally != null)
        {
            for (var i = 0; i < allies.Count; i++)
            {
                var abilityObject = allies[i];

                if (allyIndex == i)
                {
                    continue;
                }


                if (abilityObject != null &&
                    abilityObject.ally &&
                    (abilityObject.ally == ally.ally || ally.ally.Type == abilityObject.ally.Type))
                {
                    return;
                }
            }
        }

        allies[allyIndex] = ally;

        UpdateIcons();
        ChangeAllySection(allyIndex);
    }

    public void ChangeSelectPreview(AbilityObject ability)
    {
        if (ability)
        {
            _selectPreview.Find("AbilityName").GetComponent<TMP_Text>().text = ability.DisplayName;
            _selectPreview.Find("AbilityDesc").GetComponent<TMP_Text>().text = ability.Description;
            _selectPreview.RecursiveFind("AbilityIcon").GetComponent<Image>().sprite = ability.Icon;
        }
        else
        {
            _selectPreview.Find("AbilityName").GetComponent<TMP_Text>().text = "None";
            _selectPreview.Find("AbilityDesc").GetComponent<TMP_Text>().text = "";
            _selectPreview.RecursiveFind("AbilityIcon").GetComponent<Image>().sprite = MissingSprite;
        }
    }
    
    public void ChangeAllyPreview(AllyObject.AllyInstance ally)
    {
        if (ally != null && ally.ally != null)
        {
            _allyPreview.Find("AllyName").GetComponent<TMP_Text>().text = ally.ally.Name;
            _allyPreview.Find("AllyExp").GetComponent<TMP_Text>().text = $"Exp. {ally.level}";
            _allyPreview.Find("AllyLevel").gameObject.SetActive(true);
            _allyPreview.Find("AllyLevel").GetComponent<Slider>().value = ally.levelProgress;
            _allyPreview.RecursiveFind("AllyIcon").GetComponent<Image>().sprite = ally.ally.Icon;
        }
        else
        {
            _allyPreview.Find("AllyName").GetComponent<TMP_Text>().text = "No Ally Selected";
            _allyPreview.Find("AllyExp").GetComponent<TMP_Text>().text = $"";
            _allyPreview.Find("AllyLevel").gameObject.SetActive(false);
            _allyPreview.RecursiveFind("AllyIcon").GetComponent<Image>().sprite = MissingSprite;
        }
    }

    public void SetupRecycleFrame()
    {
        _recycleAllyFrame.gameObject.SetActive(false);
        
        _recycleEnemyContent.RemoveAllChildrenExcept(_recycleEnemyTemplate.name);
        foreach (var enemy in DataController.saveData.enemyInventory)
        {
            var enemyFrame = Instantiate(_recycleEnemyTemplate, _recycleEnemyTemplate.parent);
            enemyFrame.Find("Icon").GetComponent<Image>().sprite = enemy.Icon;
            enemyFrame.Find("EnemyName").GetComponent<TMP_Text>().text = enemy.DisplayName;
            enemyFrame.GetComponent<Button>().onClick.AddListener((() => AddToTryContent(enemy, enemyFrame)));
            
            enemyFrame.gameObject.SetActive(true);
        }
        
        _recycleAllyContent.RemoveAllChildrenExcept(new List<string>{_recycleAllyTemplate.name, "TryButton"});
        foreach (var ally in DataController.saveData.availableAllies)
        {
            if (ally != null && ally.ally)
            {
                var allyFrame = Instantiate(_recycleAllyTemplate, _recycleAllyTemplate.parent);
                allyFrame.Find("Icon").GetComponent<Image>().sprite = ally.ally.Icon;
                allyFrame.Find("AllyName").GetComponent<TMP_Text>().text = ally.ally.Name;
                allyFrame.GetComponent<Button>().onClick.AddListener(
                    (() => OpenRecycleMenu(DataController.saveData.availableAllies.IndexOf(ally))));

                allyFrame.gameObject.SetActive(true);
            }
        }
    }

    public void AddToTryContent(EnemyObject o, RectTransform rect)
    {
        DataController.saveData.enemyInventory.Remove(o);
        _recycleEnemies.Add(o);
        
        rect.SetParent(_recycleTryContent);
        rect.GetComponent<Button>().onClick.RemoveAllListeners();
        rect.GetComponent<Button>().onClick.AddListener((() =>
        {
            DataController.saveData.enemyInventory.Add(o);
            _recycleEnemies.Remove(o);
            SetupRecycleFrame();
            _recycleAllyFrame.gameObject.SetActive(true);
            Destroy(rect.gameObject);
        }));
        
        SetupRecycleFrame();
        _recycleAllyFrame.gameObject.SetActive(true);
    }

    public void OpenRecycleMenu(int index)
    {
        _selectedAllySlot = index;
        foreach (Transform o in _recycleTryContent)
        {
            o.GetComponent<Button>().onClick.Invoke();
        }
        _recycleTryContent.RemoveAllChildren();
        SetupRecycleFrame();

        if (index < 0)
        {
            _recycleAllyInfo.Find("Icon").GetComponent<Image>().sprite = MissingSprite;
            _recycleAllyInfo.Find("NameText").GetComponent<TMP_Text>().text = "Trying For a New Ally";
            _recycleAllyInfo.Find("ExpText").GetComponent<TMP_Text>().text = "";
            _recycleAllyInfo.Find("Slider").GetComponent<Slider>().gameObject.SetActive(false);
        }
        else
        {
            _recycleAllyInfo.Find("Icon").GetComponent<Image>().sprite = DataController.saveData.availableAllies[index].ally.Icon;
            _recycleAllyInfo.Find("NameText").GetComponent<TMP_Text>().text = DataController.saveData.availableAllies[index].ally.Name;
            _recycleAllyInfo.Find("ExpText").GetComponent<TMP_Text>().text = $"Exp. {DataController.saveData.availableAllies[index].level}";
            _recycleAllyInfo.Find("Slider").GetComponent<Slider>().gameObject.SetActive(true);
            _recycleAllyInfo.Find("Slider").GetComponent<Slider>().value =
                DataController.saveData.availableAllies[index].levelProgress;
        }
        
        _recycleAllyFrame.gameObject.SetActive(true);
    }

    public void RecycleEnemies()
    {
        if (_recycleEnemies.Count > 0)
        {
            IEnumerator recycleCoroutine()
            {
                var recycleIcon = _recycleAllyInfo.Find("Icon") as RectTransform;
                IEnumerator moveCoroutine(Image r)
                {
                    yield return r.rectTransform.DOMove(recycleIcon.position, 0.25f).SetEase(Ease.InOutQuad);
                    
                    yield return new WaitForSeconds(0.25f);
                    
                    yield return r.DOFade(0, 0.125f).SetEase(Ease.InOutQuad);

                    recycleIcon.DOScale(1.25f, 0.1f).SetEase(Ease.InOutBack).OnKill(
                        (() =>
                        {
                            recycleIcon.DOScale(1.0f, 0.1f).SetEase(Ease.InOutBack);
                        }));
                    
                    yield return new WaitForSeconds(0.25f);


                    
                    Destroy(r.gameObject);
                }

                IEnumerator rotateCoroutine(RectTransform r)
                {
                    while (r.gameObject.activeSelf)
                    {
                        r.eulerAngles += new Vector3(0, 0, Time.deltaTime*10f);
                        yield return null;
                    }
                }

                while (_recycleTryContent.childCount > 0)
                {
                    var o = _recycleTryContent.GetChild(0) as RectTransform;
                    var img = new GameObject().AddComponent<Image>();
                    img.rectTransform.SetParent(_recycleFrame);
                    img.rectTransform.sizeDelta = Vector2.one * 75f;
                    
                    img.rectTransform.anchorMax = Vector2.one * 0.5f;
                    img.rectTransform.anchorMin = Vector2.one * 0.5f;

                    img.rectTransform.position = o.position;
                    img.sprite = o.Find("Icon").GetComponent<Image>().sprite;

                    DestroyImmediate(o.gameObject);

                    StartCoroutine(moveCoroutine(img));

                    yield return new WaitForSeconds(0.125f / 2f);
                }

                yield return new WaitForSeconds(1.0f);

                // making new ally
                if (_selectedAllySlot == -1)
                {
                    AllyObject ally = Resources.LoadAll<AllyObject>("Settings/Allies").GetRandom();

                    _recycleResultFrame.Find("Icon").GetComponent<Image>().sprite = ally.Icon;
                    _recycleResultFrame.Find("AllyName").GetComponent<TMP_Text>().text = ally.Name;

                    var isNew = false;
                    foreach (var saveDataAvailableAlly in DataController.saveData.availableAllies)
                    {
                        if (saveDataAvailableAlly.ally == ally)
                        {
                            isNew = true;

                            break;
                        }
                    }
                    
                    foreach (var saveDataAvailableAlly in DataController.saveData.allies)
                    {
                        if (saveDataAvailableAlly != null && saveDataAvailableAlly.ally == ally)
                        {
                            isNew = true;

                            break;
                        }
                    }
                    
                    _recycleResultFrame.Find("NewLabel").gameObject.SetActive(isNew);
                        
                    
                    _recycleResultFrame.Find("Fade").GetComponent<Image>().color = Color.clear;
                    _recycleResultFrame.Find("Fade").GetComponent<Image>().DOFade(0.8f, 0.5f).SetEase(Ease.InOutQuad);

                    _recycleResultFrame.Find("BgEffect").GetComponent<RectTransform>().localScale = Vector3.zero;
                    _recycleResultFrame.Find("BgEffect").GetComponent<RectTransform>().DOScale(1f, 0.5f)
                        .SetEase(Ease.InOutBack);

                    StartCoroutine(rotateCoroutine(_recycleResultFrame.Find("BgEffect").GetComponent<RectTransform>()));

                    yield return new WaitForSeconds(0.15f);
                    
                    _recycleResultFrame.Find("Icon").GetComponent<RectTransform>().localScale = Vector3.zero;
                    _recycleResultFrame.Find("Icon").GetComponent<RectTransform>().DOScale(1f, 1.0f)
                        .SetEase(Ease.InOutBack);

                    yield return new WaitForSeconds(0.05f);
                    
                    _recycleResultFrame.Find("AllyName").GetComponent<RectTransform>().localScale = Vector3.zero;
                    _recycleResultFrame.Find("AllyName").GetComponent<RectTransform>().DOScale(1f, 1.0f)
                        .SetEase(Ease.InOutBack);
                    
                    yield return new WaitForSeconds(0.05f);
                    
                    _recycleResultFrame.Find("NewLabel").GetComponent<RectTransform>().localScale = Vector3.zero;
                    _recycleResultFrame.Find("NewLabel").GetComponent<RectTransform>().DOScale(1f, 1.0f)
                        .SetEase(Ease.InOutBack);

                    _recycleResultFrame.gameObject.SetActive(true);
                    
                    yield return new WaitForSeconds(2.5f);
                    
                    _recycleResultFrame.Find("Fade").GetComponent<Image>().DOFade(0.0f, 0.5f).SetEase(Ease.InOutQuad);

                    
                    _recycleResultFrame.Find("BgEffect").GetComponent<RectTransform>().DOScale(0f, 0.5f)
                        .SetEase(Ease.InOutBack);
                    
                    _recycleResultFrame.Find("Icon").GetComponent<RectTransform>().DOScale(0f, 1.0f)
                        .SetEase(Ease.InOutBack);
                    
                    _recycleResultFrame.Find("AllyName").GetComponent<RectTransform>().DOScale(0f, 1.0f)
                        .SetEase(Ease.InOutBack);
                    
                    _recycleResultFrame.Find("NewLabel").GetComponent<RectTransform>().DOScale(0f, 1.0f)
                        .SetEase(Ease.InOutBack);

                    yield return new WaitForSeconds(1.5f);
                    
                    _recycleResultFrame.gameObject.SetActive(false);
                    
                    DataController.saveData.availableAllies.Add(new AllyObject.AllyInstance(ally));
                    
                    SetupRecycleFrame();
                }
                else
                {
                    
                }

            }

            StartCoroutine(recycleCoroutine());
        }
    }

    public void ConvertCurrency(int type)
    {
        var currency = (SubRecycleType)type;

        switch (currency)
        {
            case SubRecycleType.Glass:
                if (DataController.saveData.GlassCount >= 10)
                {
                    DataController.saveData.GlassBitCount++;
                    DataController.saveData.GlassCount -= 10;
                }
                break;
            case SubRecycleType.Metal:
                if (DataController.saveData.MetalCount >= 10)
                {
                    DataController.saveData.MetalBitCount++;
                    DataController.saveData.MetalCount -= 10;
                }
                break;
            case SubRecycleType.Plastic:
                if (DataController.saveData.PlasticCount >= 10)
                {
                    DataController.saveData.PlasticBitCount++;
                    DataController.saveData.PlasticCount -= 10;
                }
                break;
        }
    }

    public void RunPostMatchSequence(MatchController.PostMatchData pmd)
    {
        void calculateDistribution(int count, out int glass, out int plastic, out int metal)
        {
            var trashLeft = count;

            int glassCount = 0, metalCount = 0, plasticCount = 0;

            int tl_itr = 0;
            while (trashLeft > 0)
            {
                var val = Random.Range(1, trashLeft);
                switch (tl_itr)
                {
                    case 0: // glass
                        glassCount += val;

                        break;
                    case 1: // metal
                        metalCount += val;

                        break;
                    case 2: // plastic
                        plasticCount += val;

                        break;
                }

                tl_itr++;
                if (tl_itr > 2)
                    tl_itr = 0;

                trashLeft -= val;
            }
            
            glass = glassCount;
            metal = metalCount;
            plastic = plasticCount;
        }

        IEnumerator pms()
        {
            _isPostInfoRunning = true;
            _postInfoFrame.gameObject.SetActive(pmd.won);
            _postFailFrame.gameObject.SetActive(!pmd.won);

            if (pmd.won)
            {
                _postTrashInfo.RecursiveFind<TMP_Text>("Count").text =
                    pmd.newCollectibles[CollectibleController.CollectibleType.GenericTrash].ToString();

                _postGlassInfo.RecursiveFind<TMP_Text>("Count").text = DataController.saveData.GlassCount.ToString();
                _postPlasticInfo.RecursiveFind<TMP_Text>("Count").text =
                    DataController.saveData.PlasticCount.ToString();

                _postMetalInfo.RecursiveFind<TMP_Text>("Count").text = DataController.saveData.MetalCount.ToString();

                var template = _postInfoFrame.RecursiveFind("EnemyTemplate");
                
                foreach (var pmdCaughtEnemy in pmd.CaughtEnemies)
                {
                    var enemyIcon = Instantiate(template.gameObject, template.parent);
                    enemyIcon.RecursiveFind<Image>("Icon").sprite = pmdCaughtEnemy.Icon;
                    enemyIcon.gameObject.SetActive(true);
                }

            }
            _postMatchFrame.position = _postMatchFrame.position.SetY(-540);

            _postMatchFrame.gameObject.SetActive(true);

            yield return _postMatchFrame.DOMoveY(540, 0.5f).SetEase(Ease.InOutQuad);

            if (pmd.won)
            {
                yield return new WaitForSeconds(1.0f);
                calculateDistribution(pmd.newCollectibles[CollectibleController.CollectibleType.GenericTrash], out var _glass, out var _plastic, out int _metal);
                var trashCount = _postTrashInfo.Find("Count").GetComponent<TMP_Text>();
                
                IEnumerator pointCoroutine(TMP_Text target, Sprite sprite)
                {
                    var icon = new GameObject().AddComponent<Image>();
                    icon.transform.SetParent(_postMatchFrame);

                    var rectIcon = icon.transform as RectTransform;
                    rectIcon.position = trashCount.rectTransform.position;

                    rectIcon.anchorMin = Vector2.one * 0.5f;
                    rectIcon.anchorMax = Vector2.one * 0.5f;

                    rectIcon.sizeDelta = Vector2.one * 25f;
                    icon.sprite = sprite;

                    yield return rectIcon.DOMove(target.rectTransform.position, 0.125f/2).SetEase(Ease.InOutQuad);

                    yield return new WaitForSeconds(0.125f/2);
                    
                    yield return icon.DOFade(0, 0.05f/2).SetEase(Ease.InOutQuad);

                    yield return new WaitForSeconds(0.075f/2);
                    

                    var ct = int.Parse(target.text);
                    ct++;
                    target.text = ct.ToString();

                    ct = int.Parse(trashCount.text);
                    ct--;
                    trashCount.text = ct.ToString();

                    target.DOColor(Color.green, 0.1f).OnComplete((() => target.DOColor(Color.white, 0.1f)));
                    trashCount.DOColor(Color.red, 0.1f).OnComplete((() => trashCount.DOColor(Color.white, 0.1f)));
                }
                

                {
                    var textInfo = _postGlassInfo.Find("Count").GetComponent<TMP_Text>();
                    for (int i = 0; i < _glass; i++)
                    {
                        StartCoroutine(pointCoroutine(textInfo, textInfo.transform.parent.Find("Icon").GetComponent<Image>().sprite));

                        yield return new WaitForSeconds(0.125f/2);
                    }
                }
                
                {
                    var textInfo = _postPlasticInfo.Find("Count").GetComponent<TMP_Text>();
                    for (int i = 0; i < _plastic; i++)
                    {
                        StartCoroutine(pointCoroutine(textInfo,textInfo.transform.parent.Find("Icon").GetComponent<Image>().sprite));

                        yield return new WaitForSeconds(0.125f/2);
                    }
                }
                
                {
                    var textInfo = _postMetalInfo.Find("Count").GetComponent<TMP_Text>();
                    for (int i = 0; i < _metal; i++)
                    {
                        StartCoroutine(pointCoroutine(textInfo,textInfo.transform.parent.Find("Icon").GetComponent<Image>().sprite));

                        yield return new WaitForSeconds(0.125f/2);
                    }
                }

                DataController.saveData.GlassCount += _glass;
                DataController.saveData.PlasticCount += _plastic;
                DataController.saveData.MetalCount += _metal;
                DataController.saveData.enemyInventory.AddRange(pmd.CaughtEnemies);

            }

            yield return new WaitForSeconds(2.5f);
            
            yield return _postMatchFrame.DOMoveY(-540, 0.5f).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(0.75f);
            
            _postMatchFrame.gameObject.SetActive(false);
            _isPostInfoRunning = false;
            
            _postInfoFrame.RecursiveFind("EnemyTemplate").RemoveAllChildrenExcept(new List<string>(){"EnemyTemplate", "TrashInfo"});
            
            yield break;
        }

        StartCoroutine(pms());
    }
}
