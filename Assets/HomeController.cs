
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

    private List<ClothingObject> _clothingObjects;
    
    public enum SubRecycleType
    {
        Glass = 0,
        Plastic,
        Metal
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

        _recycleFrame = _playFrame.parent.Find("RecycleFrame") as RectTransform;
        _recycleGlassCount = _recycleFrame.RecursiveFind<TMP_Text>("GlassCount");
        _recyclePlasticCount = _recycleFrame.RecursiveFind<TMP_Text>("PlasticCount");
        _recycleMetalCount = _recycleFrame.RecursiveFind<TMP_Text>("MetalCount");
        _recycleGlassBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubGlassCount");
        _recyclePlasticBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubPlasticCount");
        _recycleMetalBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubMetalCount");
        
        _recycleEnemyContent = _recycleFrame.RecursiveFind("EnemyContent") as RectTransform;
        _recycleTryContent = _recycleFrame.RecursiveFind("TryContent") as RectTransform;
        _recycleChanceFrame = _recycleFrame.RecursiveFind("EnemyRecycleChance") as RectTransform;
        _recycleChanceTemplate = _recycleFrame.RecursiveFind("ChanceTemplate") as RectTransform;
        _recycleAllyInfo = _recycleFrame.RecursiveFind("AllyInfo") as RectTransform;
        _recycleResultFrame = _recycleFrame.RecursiveFind("RecycleResultFrame") as RectTransform;
        
        _recycleItemCounter = _recycleFrame.RecursiveFind("ItemCounters") as RectTransform;
        _recycleSubItemCounter = _recycleFrame.RecursiveFind("SubItemCounter") as RectTransform;
        
        _recycleEnemyTemplate = _recycleEnemyContent.RecursiveFind<RectTransform>("EnemyButtonTemplate");
        
                
        _homeAbilityIcons = _recycleFrame.RecursiveFind("HomeAbilityContent").GetAllChildren()
            .Select((transform1 => transform1.GetComponent<Image>())).ToList();
        
        _homeAllyIcons = _recycleFrame.RecursiveFind("HomeAllyContent").GetAllChildren()
            .Select((transform1 => transform1.GetComponent<Image>())).ToList();

        _abilitySelectFrame = _recycleFrame.RecursiveFind("AbilitySelectFrame") as RectTransform;

        
        _abilityUpgradeFrame = _recycleFrame.RecursiveFind("AbilityUpgradeFrame") as RectTransform;
        
        _postMatchFrame = _playFrame.parent.Find("PostMatchFrame") as RectTransform;
        _postInfoFrame = _postMatchFrame.RecursiveFind("InfoFrame")  as RectTransform;
        _postFailFrame = _postMatchFrame.RecursiveFind("FailFrame")  as RectTransform;
        _postTrashInfo = _postInfoFrame.RecursiveFind("TrashInfo") as RectTransform;
        _postClothInfo = _postInfoFrame.RecursiveFind("ClothInfo") as RectTransform;
        _postGlassInfo = _postInfoFrame.RecursiveFind("GlassSection") as RectTransform;
        _postPlasticInfo = _postInfoFrame.RecursiveFind("PlasticSection") as RectTransform;
        _postMetalInfo = _postInfoFrame.RecursiveFind("MetalSection") as RectTransform;
        _postFinalClothInfo = _postInfoFrame.RecursiveFind("ClothSection") as RectTransform;

        _changeFrame = _playFrame.parent.Find("ChangeFrame") as RectTransform;
        _partChoiceFrame = _changeFrame.Find("PartSelectFrame") as RectTransform;
        _pieceChoiceFrame = _changeFrame.Find("PieceSelectFrame") as RectTransform;

        _clothingScrapCount = _pieceChoiceFrame.RecursiveFind<TMP_Text>("ScrapCount");
        
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
        
        AlignUISection(-1);

        var matchCtrl = FindAnyObjectByType<MatchController>();
        if (matchCtrl)
        {
            var postMatchData = matchCtrl.CreatePostMatchData();

            DestroyImmediate(matchCtrl.gameObject);

            LoadingScreenController.LoadingScreen.CloseLoadingScreen();
            
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

        _clothingObjects = Resources.LoadAll<ClothingObject>("Settings/Clothing/").ToList();
                                
        SetupPlayFrame();
        SetupRecycleFrame();
        
        SoundManager.Instance.SetMusic(Music);
        

        
    }

    public MusicObject Music;

    private void Update()
    {

         if (upgrades.Count < abilities.Count)
         {
             while (upgrades.Count < abilities.Count)
             {
                 upgrades.Add(null);
             }
         }
         

         
    }

    public void StartMatch()
    {
        var matchController = new GameObject().AddComponent<MatchController>();
        matchController.PassedAbilities = abilities;
        matchController.PassedUpgrades = upgrades;
        matchController.PassedAllies = allies;


        matchController.CurrentMap = DataController.saveData.NextMap.Start;
        matchController.RoundAmount = DataController.saveData.NextMap.RoundCount;
        matchController.SeverityLevel = DataController.saveData.NextMap.SeverityLevel;
        
        LoadingScreenController.LoadingScreen.OpenLoadingScreen((() =>
        {
            SceneManager.LoadScene("GameScene");
        }));
    }
    
    // ui section

    public Sprite MissingSprite = null;
    public Sprite TrashSprite = null;
    public Sprite GlassSprite, MetalSprite, PlasticSprite;

    private RectTransform _playFrame;
    private List<Image> _playAbilityIcons = new List<Image>();

    private List<Image> _homeAbilityIcons = new List<Image>();
    private List<Image> _homeAllyIcons = new List<Image>();

    private RectTransform _abilitySelectFrame;

    private RectTransform _abilityUpgradeFrame;

    private int _selectedSlotIndex = -1;
    private RectTransform _openSelectFrame;

    private RectTransform _recycleFrame;
    private TMP_Text _recycleGlassCount;
    private TMP_Text _recyclePlasticCount;
    private TMP_Text _recycleMetalCount;
    private TMP_Text _recycleGlassBitCount;
    private TMP_Text _recyclePlasticBitCount;
    private TMP_Text _recycleMetalBitCount;

    private RectTransform _recycleEnemyContent;
    private RectTransform _recycleTryContent;
    private RectTransform _recycleChanceFrame;
    private RectTransform _recycleChanceTemplate;
    private RectTransform _recycleAllyInfo;
    private RectTransform _recycleEnemyTemplate;
    private RectTransform _recycleAllyTemplate;
    private RectTransform _recycleResultFrame;

    private RectTransform _recycleItemCounter;
    private RectTransform _recycleSubItemCounter;
    
    private List<EnemyObject> _recycleEnemies = new List<EnemyObject>();
    
    private RectTransform _postMatchFrame;
    private RectTransform _postInfoFrame;
    private RectTransform _postFailFrame;
    private RectTransform _postTrashInfo;
    private RectTransform _postClothInfo;
    private RectTransform _postGlassInfo, _postPlasticInfo, _postMetalInfo, _postFinalClothInfo;
    private bool _isPostInfoRunning = false;

    private RectTransform _changeFrame;
    private RectTransform _partChoiceFrame;
    private RectTransform _pieceChoiceFrame;
    private TMP_Text _clothingScrapCount;

    public void AlignUISection(int SectionIndex)
    {
        
        _playFrame.gameObject.SetActive(false);
        _recycleFrame.gameObject.SetActive(false);
        _postMatchFrame.gameObject.SetActive(false);
        _changeFrame.gameObject.SetActive(false);
        switch (SectionIndex)
        {
            case 0:
                _playFrame.gameObject.SetActive(true);
                SetupPlayFrame();
                break;
            case 1:
                _changeFrame.gameObject.SetActive(true);
                SetupChangeFrame();
                break;
            case 2:
                _recycleFrame.gameObject.SetActive(true);
                SetupRecycleFrame();
                break;
        }
    }
    
    public void SetupPlayFrame()
    {
        _playFrame.RecursiveFind("CoverPhoto").GetComponent<Image>().sprite = DataController.saveData.NextMap.MapCover;
        _playFrame.RecursiveFind("NextMapText").GetComponent<TMP_Text>().text =
            $"Next Map:\n<b>{DataController.saveData.NextMap.MapName}</b>";
    }

    public void SetupChangeFrame()
    {
        var hatIcon = _partChoiceFrame.RecursiveFind<Image>("HatIcon");
        var clothingIcon = _partChoiceFrame.RecursiveFind<Image>("ShirtIcon");
        var pantsIcon = _partChoiceFrame.RecursiveFind<Image>("PantsIcon");
        var shoesIcon = _partChoiceFrame.RecursiveFind<Image>("ShoesIcon");

        hatIcon.sprite = DataController.saveData.HatObject?.Icon ?? MissingSprite;
        clothingIcon.sprite = DataController.saveData.ShirtObject?.Icon ?? MissingSprite;
        pantsIcon.sprite = DataController.saveData.PantsObject?.Icon ?? MissingSprite;
        shoesIcon.sprite = DataController.saveData.ShoesObject?.Icon ?? MissingSprite;
        
        _clothingScrapCount.text = DataController.saveData.ClothingScrapCount.ToString();
    }

    public void ChangeClothing(ClothingObject.ClothingType type, ClothingObject clothing)
    {
        if (!DataController.saveData.availableClothing.Contains(clothing))
        {
            if (DataController.saveData.ClothingScrapCount >= clothing.Cost)
            {
                DataController.saveData.ClothingScrapCount -= clothing.Cost;
                DataController.saveData.availableClothing.Add(clothing);
                
                _clothingScrapCount.DOColor(Color.red, 0.1f).OnComplete((() => _clothingScrapCount.DOColor(Color.white, 0.1f)));
            }
            else
            {
                return;
            }
        }

        switch (type)
        {
            case ClothingObject.ClothingType.Hat:
                DataController.saveData.HatObject = clothing;
                break;
            case ClothingObject.ClothingType.Shirt:
                DataController.saveData.ShirtObject = clothing;
                break;
            case ClothingObject.ClothingType.Bottom:
                DataController.saveData.PantsObject = clothing;
                break;
            case ClothingObject.ClothingType.Shoe:
                DataController.saveData.ShoesObject = clothing;
                break;
        }
        
        OpenPartFrame((int)type);
        PreviewPart(clothing,type);
        SetupChangeFrame();

        FindAnyObjectByType<PlayerController>().GetComponentInChildren<ClothingController>().ChangeClothing(DataController.saveData.BuildListOfClothing());
    }
    
    public void PreviewPart(ClothingObject clothingObject, ClothingObject.ClothingType type)
    {
        FindAnyObjectByType<PlayerController>().GetComponentInChildren<ClothingController>().FitClothing(clothingObject);

        var buyButton = _pieceChoiceFrame.RecursiveFind("BuyButton").GetComponent<Image>();

        buyButton.transform.Find("EquipText").gameObject.SetActive(DataController.saveData.availableClothing.Contains(clothingObject));
        buyButton.transform.Find("BuyFrame").gameObject.SetActive(!DataController.saveData.availableClothing.Contains(clothingObject));
        buyButton.transform.Find("BuyFrame").Find("PriceText").GetComponent<TMP_Text>().text = clothingObject.Cost.ToString();

        if (DataController.saveData.availableClothing.Contains(clothingObject))
        {
            bool equipped = false;
            switch (type)
            {
                case ClothingObject.ClothingType.Hat:
                    equipped = DataController.saveData.HatObject == clothingObject;
                    break;
                case ClothingObject.ClothingType.Shirt:
                    equipped=  DataController.saveData.ShirtObject == clothingObject;
                    break;
                case ClothingObject.ClothingType.Bottom:
                    equipped =DataController.saveData.PantsObject == clothingObject;
                    break;
                case ClothingObject.ClothingType.Shoe:
                    equipped = DataController.saveData.ShoesObject == clothingObject;
                    break;
            }

            if (equipped)
            {
                buyButton.color = BaseUtils.ColorFromHex("2653A2");
                buyButton.transform.Find("EquipText").GetComponent<TMP_Text>().text = "Equipped";
            }
            else
            {
                buyButton.color = BaseUtils.ColorFromHex("007CCC");
                buyButton.transform.Find("EquipText").GetComponent<TMP_Text>().text = "Equip";
            }
        }
        else
        {
            buyButton.color = BaseUtils.ColorFromHex("00CC12");
        }

        _pieceChoiceFrame.Find("PieceName").GetComponent<TMP_Text>().text = clothingObject.Name;
        
        buyButton.GetComponent<Button>().onClick.RemoveAllListeners();
        buyButton.GetComponent<Button>().onClick.AddListener((() =>
        {
            ChangeClothing(type,clothingObject);
        }));
    }

    public void ClearPreview()
    {
        FindAnyObjectByType<PlayerController>().GetComponentInChildren<ClothingController>().ChangeClothing(DataController.saveData.BuildListOfClothing());

    }
    
    public void OpenPartFrame(int t)
    {
        ClothingObject.ClothingType type = (ClothingObject.ClothingType)t;

        var pieceContent = _pieceChoiceFrame.RecursiveFind("PieceContent");
        pieceContent.RemoveAllChildrenExcept("PieceTemplate");

        var template = pieceContent.Find("PieceTemplate");
        template.gameObject.SetActive(false);
        
        foreach (var clothingObject in _clothingObjects)
        {
            if (clothingObject.Type == type)
            {
                var obj = Instantiate(template.gameObject, template.parent);
                obj.gameObject.SetActive(true);

                obj.transform.Find("Icon").GetComponent<Image>().sprite = clothingObject.Icon;
                obj.GetComponent<Button>().onClick.AddListener((() => {PreviewPart(clothingObject,type);}));
                PreviewPart(clothingObject,type);
            }
        }
        
        
    }
    
    public void UpdateIcons()
    {
        for (var i = 0; i < abilities.Count; i++)
        {
            if (abilities[i])
            {
                _homeAbilityIcons[i].sprite = abilities[i].Icon;
                
            }
            else
            {
                _homeAbilityIcons[i].sprite = MissingSprite;
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

    public void SetupRecycleFrame()
    {
        
        _recycleEnemyContent.RemoveAllChildrenExcept(_recycleEnemyTemplate.name);
        foreach (var enemy in DataController.saveData.enemyInventory)
        {
            var enemyFrame = Instantiate(_recycleEnemyTemplate, _recycleEnemyTemplate.parent);
            enemyFrame.Find("Icon").GetComponent<Image>().sprite = enemy.Icon;
            enemyFrame.Find("EnemyName").GetComponent<TMP_Text>().text = enemy.DisplayName;
            enemyFrame.GetComponent<Button>().onClick.AddListener((() => AddToTryContent(enemy, enemyFrame)));
            
            enemyFrame.gameObject.SetActive(true);
        }
        
        _recyclePlasticCount.text = $"{DataController.saveData.PlasticCount:00}";
        _recycleGlassCount.text = $"{DataController.saveData.GlassCount:00}";
        _recycleMetalCount.text = $"{DataController.saveData.MetalCount:00}";
         
        _recyclePlasticBitCount.text = $"{DataController.saveData.PlasticBitCount:00}";
        _recycleGlassBitCount.text = $"{DataController.saveData.GlassBitCount:00}";
        _recycleMetalBitCount.text = $"{DataController.saveData.MetalBitCount:00}";
        

    }

    public void UpdateAbilityButtons(RectTransform r)
    {
        for (int i = 0; i < r.childCount; i++)
        {
            var button = r.GetChild(i);
            button.RecursiveFind("Icon").GetComponent<Image>().sprite = abilities[i]?.Icon ?? MissingSprite;
            button.RecursiveFind("AbilityName").GetComponent<TMP_Text>().text = abilities[i]?.DisplayName ?? "No Ability in Slot";
            button.RecursiveFind("AbilityDesc").GetComponent<TMP_Text>().text = upgrades[i]?.DisplayName ?? "No Upgrade";
        }
    }
    
    public void UpdateAllyButtons(RectTransform r)
    {
        for (int i = 0; i < r.childCount; i++)
        {
            var button = r.GetChild(i);
            button.RecursiveFind("Icon").GetComponent<Image>().sprite = allies[i]?.ally?.Icon ?? MissingSprite;
            button.RecursiveFind("AllyName").GetComponent<TMP_Text>().text = allies[i]?.ally?.Name ?? "No Ally in Slot";
            button.RecursiveFind("AllyDesc").GetComponent<TMP_Text>().text = allies[i]?.ally?.Type.ToString();
        }
    }

    public void SetupAbilitySubFrame(RectTransform frame)
    {
        _openSelectFrame = frame;
        var content = frame.RecursiveFind("AbilityContent") as RectTransform;
        UpdateAbilityButtons(content);
    }
    public void SetupAllySubFrame(RectTransform frame)
    {
        _openSelectFrame = frame;
        var content = frame.RecursiveFind("AllyContent") as RectTransform;
        UpdateAllyButtons(content);
    }
    

    public void ChangeAbility(int abilityIndex, AbilityObject ability)
    {
        if (ability)
        {
            foreach (var abilityObject in abilities)
            {
                if (abilityObject && (abilityObject == ability || abilityObject.Type == ability.Type))
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

        UpdateAbilityButtons(_openSelectFrame.RecursiveFind("AbilityContent") as RectTransform);
        OpenSelectMenu(_selectedSlotIndex);
    }
    
    public void ChangeAlly(int allyIndex, AllyObject.AllyInstance ally)
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
        
        UpdateAllyButtons(_openSelectFrame.RecursiveFind("AllyContent") as RectTransform);
        OpenSelectMenu(_selectedSlotIndex);
    }

    private void ChangeUpgrade(int index, AbilityObject.AbilityUpgrade abilityUpgrade)
    {
        if (abilityUpgrade == null)
        {
            upgrades[index] = null;
        }
        else
        {

            if (DataController.saveData.availableUpgrades.Contains(abilityUpgrade))
            {
                upgrades[index] = abilityUpgrade;
            }
            else
            {
                // buying an upgrade

                if (abilityUpgrade.CulletPrice <= DataController.saveData.GlassBitCount &&
                    abilityUpgrade.PelletPrice <= DataController.saveData.PlasticBitCount &&
                    abilityUpgrade.SheetPrice <= DataController.saveData.MetalBitCount)
                {
                    DataController.saveData.availableUpgrades.Add(abilityUpgrade);
                    upgrades[index] = abilityUpgrade;
                    DataController.saveData.GlassBitCount -= abilityUpgrade.CulletPrice;
                    DataController.saveData.PlasticBitCount -= abilityUpgrade.PelletPrice;
                    DataController.saveData.MetalBitCount -= abilityUpgrade.SheetPrice;
                }
            }
        }

        UpdateAbilityButtons(_openSelectFrame.RecursiveFind("AbilityContent") as RectTransform);
        OpenSelectMenu(_selectedSlotIndex);
    }

    public void ToggleSelectMenu(int index)
    {
        var menu = _openSelectFrame.RecursiveFind("SelectMenu") as RectTransform;

        if (menu.gameObject.activeSelf && _selectedSlotIndex == index)
        {
            menu.gameObject.SetActive(false);
        }
        else
        {
            OpenSelectMenu(index);
        }
    }
    
    public void OpenSelectMenu(int index)
    {
        _selectedSlotIndex = index;
        var button = (_openSelectFrame.name == "AllySelectFrame" ? _openSelectFrame.RecursiveFind("AllyContent") : _openSelectFrame.RecursiveFind("AbilityContent")).GetChild(index) as RectTransform;
        var menu = _openSelectFrame.RecursiveFind("SelectMenu") as RectTransform;
        menu.gameObject.SetActiveFast(true);
        
        menu.RecursiveFind("SelectContent").RemoveAllChildrenExcept("SelectTemplate");

        menu.anchoredPosition = menu.anchoredPosition.SetY(button.anchoredPosition.y-100);
        
        var template = menu.RecursiveFind<RectTransform>("SelectTemplate");
        template.gameObject.SetActive(false);
        
        if (_openSelectFrame.name == "AbilitySelectFrame")
        {
            {
                var frame = Instantiate(template, template.parent);
                frame.Find("AbilityName").GetComponent<TMP_Text>().text = "None";
                frame.Find("AbilityType").GetComponent<TMP_Text>().text = "";
                frame.Find("Icon").GetComponent<Image>().sprite = MissingSprite;
                
                frame.GetComponent<Button>().onClick.AddListener((() =>
                {
                    ChangeAbility(index, null);
                }));
                
                frame.gameObject.SetActive(true);
            }
            
            foreach (var ability in DataController.saveData.availableAbilities)
            {
                if (abilities.Contains(ability))
                    continue;
                
                var frame = Instantiate(template, template.parent);
                frame.Find("AbilityName").GetComponent<TMP_Text>().text = ability.DisplayName;

                var typeName = ability.Type.ToString();
                
                switch (ability.Type)
                {
                    case AbilityObject.AbilityType.Projectile:
                        typeName = "Projectile";
                        break;
                    case AbilityObject.AbilityType.Bomb:
                        typeName = "Explosive";
                        break;
                    case AbilityObject.AbilityType.Effect:
                        break;
                    case AbilityObject.AbilityType.Melee:
                        break;
                    case AbilityObject.AbilityType.Trigger:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                frame.Find("AbilityType").GetComponent<TMP_Text>().text = typeName;
                frame.Find("Icon").GetComponent<Image>().sprite = ability.Icon;
                
                frame.GetComponent<Button>().onClick.AddListener((() =>
                {
                    ChangeAbility(index, ability);
                }));
                
                frame.gameObject.SetActive(true);
            }
        }
        else if (_openSelectFrame.name == "AbilityUpgradeFrame")
        {
            {
                var frame = Instantiate(template, template.parent);
                frame.Find("UpgradeName").GetComponent<TMP_Text>().text = "None";
                frame.Find("UpgradeCost").gameObject.SetActive(false);
                frame.Find("StatusField").gameObject.SetActive(false);
                
                frame.GetComponent<Button>().onClick.AddListener((() =>
                {
                    ChangeUpgrade(index, null);
                }));
                
                frame.gameObject.SetActive(true);
            }

            if (abilities[index])
            {
                foreach (var abilityUpgrade in abilities[index].Upgrades)
                {
                    var frame = Instantiate(template, template.parent);
                    frame.Find("UpgradeName").GetComponent<TMP_Text>().text = abilityUpgrade.DisplayName;

                    frame.RecursiveFind("CulletCost").GetComponent<TMP_Text>().text =
                        abilityUpgrade.CulletPrice.ToString();

                    frame.RecursiveFind("PelletCost").GetComponent<TMP_Text>().text =
                        abilityUpgrade.PelletPrice.ToString();

                    frame.RecursiveFind("MetalCost").GetComponent<TMP_Text>().text =
                        abilityUpgrade.SheetPrice.ToString();

                    Color statusColor = BaseUtils.ColorFromHex("8E8E8E");
                    string statusString = "Unavailable";

                    if (upgrades[index] == abilityUpgrade) // upgrade is bought and active
                    {

                        statusColor = BaseUtils.ColorFromHex("0BD149");
                        statusString = "Active";
                    }
                    else // upgrade is inactive
                    {
                        if (DataController.saveData.availableUpgrades
                            .Contains(abilityUpgrade)) // upgrade is bought but inactive
                        {
                            statusColor = BaseUtils.ColorFromHex("fecb00");
                            statusString = "Inactive";
                        }
                        else // upgrade is not bought
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
                                statusColor = BaseUtils.ColorFromHex("E55858");
                                statusString = "Not Enough";
                            }
                        }
                    }

                    frame.Find("StatusField").GetComponent<Image>().color = statusColor;
                    frame.Find("StatusField").GetComponentInChildren<TMP_Text>().text = statusString;

                    frame.GetComponent<Button>().onClick.AddListener((() => { ChangeUpgrade(index, abilityUpgrade); }));


                    frame.gameObject.SetActive(true);
                }
            }
        } else if (_openSelectFrame.name == "AllySelectFrame")
        {
            {
                var frame = Instantiate(template, template.parent);
                frame.Find("AllyName").GetComponent<TMP_Text>().text = "None";
                frame.Find("AllyType").GetComponent<TMP_Text>().text = "";
                frame.Find("Icon").GetComponent<Image>().sprite = MissingSprite;
                
                frame.GetComponent<Button>().onClick.AddListener((() =>
                {
                    ChangeAlly(index, null);
                }));
                
                frame.gameObject.SetActive(true);
            }
            
            foreach (var ally in DataController.saveData.availableAllies)
            {
                if (allies.Contains(ally))
                    continue;
                
                var frame = Instantiate(template, template.parent);
                frame.Find("AllyName").GetComponent<TMP_Text>().text = ally.ally.Name;

                var typeName = ally.ally.Type.ToString(); 
                frame.Find("AllyType").GetComponent<TMP_Text>().text = typeName;
                frame.Find("Icon").GetComponent<Image>().sprite = ally.ally.Icon;
                
                frame.GetComponent<Button>().onClick.AddListener((() =>
                {
                    ChangeAlly(index, ally);
                }));
                
                frame.gameObject.SetActive(true);
            }
        }
    }
     
    public void SetupAbilityFrame()
    {
        UpdateIcons();
    }

    public float TrashChance => 100f / (( (_recycleEnemies.Count - 1) *0.25f)+1);
    
    public void CalculateChance()
    {
        _recycleChanceFrame.RemoveAllChildrenExcept(_recycleChanceTemplate.name);
        
        Dictionary<Sprite, float> chances = new Dictionary<Sprite, float>();
        
        chances.Add(TrashSprite,TrashChance);

        if (TrashChance < 100)
        {

            var allyChance = 100 - TrashChance;
            var chancePerEnemy = allyChance / _recycleEnemies.Count;


            foreach (var recycleEnemy in _recycleEnemies)
            {
                foreach (var recycleEnemyPossibleAlly in recycleEnemy.PossibleAllies)
                {
                    chances.TryAdd(recycleEnemyPossibleAlly.Icon, 0);

                    chances[recycleEnemyPossibleAlly.Icon] += chancePerEnemy;
                }
            }

        }

        foreach (var (sprite, value) in chances)
        {
            var chanceObj = Instantiate(_recycleChanceTemplate, _recycleChanceTemplate.parent);

            chanceObj.Find("Icon").GetComponent<Image>().sprite = sprite;
            chanceObj.Find("ChanceText").GetComponent<TMP_Text>().text = $"%{value.ToString("F2")}";
            
            chanceObj.gameObject.SetActive(true);
        }

        _recycleChanceFrame.gameObject.SetActiveFast(_recycleEnemies.Count > 0);
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
            CalculateChance();
            Destroy(rect.gameObject);
        }));
        
        SetupRecycleFrame();
        
        CalculateChance();
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

                    recycleIcon.DOScale(1.25f, 0.1f).SetEase(Ease.OutBack).OnKill(
                        (() => { recycleIcon.DOScale(1.0f, 0.1f).SetEase(Ease.OutBack); }));

                    yield return new WaitForSeconds(0.25f);



                    Destroy(r.gameObject);
                }

                IEnumerator rotateCoroutine(RectTransform r)
                {
                    while (r.gameObject.activeSelf)
                    {
                        r.eulerAngles += new Vector3(0, 0, Time.deltaTime * 10f);

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

                bool newAlly = true;

                var chosenAlly = _recycleEnemies.GetRandom().PossibleAllies.GetRandom();

                // foreach (var saveDataAvailableAlly in DataController.saveData.availableAllies)
                // {
                //     if (saveDataAvailableAlly.ally == chosenAlly)
                //     {
                //         newAlly = false;
                //
                //         break;
                //     }
                // }

                if (Random.Range(0, 100) <= TrashChance)
                {
                    newAlly = false;
                }

                int trashPrice = _recycleEnemies.Count * 5;
                
                if (newAlly)
                {
                    _recycleResultFrame.Find("Icon").GetComponent<Image>().sprite = chosenAlly.Icon;
                    _recycleResultFrame.Find("AllyName").GetComponent<TMP_Text>().text = chosenAlly.Name;
                }
                else
                {
                    _recycleResultFrame.Find("Icon").GetComponent<Image>().sprite = TrashSprite;
                    _recycleResultFrame.Find("AllyName").GetComponent<TMP_Text>().text =
                        $"{trashPrice} Pieces of Trash!";
                }


                var isNew = true;

                if (newAlly)
                {
                    foreach (var saveDataAvailableAlly in DataController.saveData.availableAllies)
                    {
                        if (saveDataAvailableAlly.ally == chosenAlly)
                        {
                            isNew = false;

                            break;
                        }
                    }

                    foreach (var saveDataAvailableAlly in DataController.saveData.allies)
                    {
                        if (saveDataAvailableAlly != null && saveDataAvailableAlly.ally == chosenAlly)
                        {
                            isNew = false;

                            break;
                        }
                    }
                }
                else
                {
                    isNew = false;
                }

                _recycleResultFrame.Find("NewLabel").gameObject.SetActive(isNew);


                _recycleResultFrame.Find("Fade").GetComponent<Image>().color = Color.clear;
                _recycleResultFrame.Find("Fade").GetComponent<Image>().DOFade(0.8f, 0.5f).SetEase(Ease.InOutQuad);

                _recycleResultFrame.Find("BgEffect").GetComponent<RectTransform>().localScale = Vector3.zero;
                _recycleResultFrame.Find("BgEffect").GetComponent<RectTransform>().DOScale(1f, 0.5f)
                    .SetEase(Ease.OutBack);

                StartCoroutine(rotateCoroutine(_recycleResultFrame.Find("BgEffect").GetComponent<RectTransform>()));

                yield return new WaitForSeconds(0.15f);

                _recycleResultFrame.Find("Icon").GetComponent<RectTransform>().localScale = Vector3.zero;
                _recycleResultFrame.Find("Icon").GetComponent<RectTransform>().DOScale(1f, 1.0f)
                    .SetEase(Ease.OutBack);

                yield return new WaitForSeconds(0.05f);

                _recycleResultFrame.Find("AllyName").GetComponent<RectTransform>().localScale = Vector3.zero;
                _recycleResultFrame.Find("AllyName").GetComponent<RectTransform>().DOScale(1f, 1.0f)
                    .SetEase(Ease.OutBack);

                yield return new WaitForSeconds(0.05f);

                _recycleResultFrame.Find("NewLabel").GetComponent<RectTransform>().localScale = Vector3.zero;
                _recycleResultFrame.Find("NewLabel").GetComponent<RectTransform>().DOScale(1f, 1.0f)
                    .SetEase(Ease.OutBack);

                _recycleResultFrame.gameObject.SetActive(true);

                yield return new WaitForSeconds(2.5f);

                _recycleResultFrame.Find("Fade").GetComponent<Image>().DOFade(0.0f, 0.5f).SetEase(Ease.InOutQuad);


                _recycleResultFrame.Find("BgEffect").GetComponent<RectTransform>().DOScale(0f, 0.5f)
                    .SetEase(Ease.InBack);

                _recycleResultFrame.Find("Icon").GetComponent<RectTransform>().DOScale(0f, 1.0f)
                    .SetEase(Ease.InBack);

                _recycleResultFrame.Find("AllyName").GetComponent<RectTransform>().DOScale(0f, 1.0f)
                    .SetEase(Ease.InBack);

                _recycleResultFrame.Find("NewLabel").GetComponent<RectTransform>().DOScale(0f, 1.0f)
                    .SetEase(Ease.InBack);

                yield return new WaitForSeconds(1.5f);


                _recycleResultFrame.gameObject.SetActive(false);


                if (newAlly)
                {
                    DataController.saveData.availableAllies.Add(new AllyObject.AllyInstance(chosenAlly));
                }
                else
                {
                    calculateDistribution(trashPrice, out var _glass, out var _plastic, out int _metal);

                    DataController.saveData.GlassCount += _glass;
                    DataController.saveData.PlasticCount += _plastic;
                    DataController.saveData.MetalCount += _metal;
                }

                _recycleEnemies.Clear();
                
                SetupRecycleFrame();
                CalculateChance();
                


            }

            StartCoroutine(recycleCoroutine());
        }
    }

    public void ConvertCurrency(int type)
    {
        var currency = (SubRecycleType)type;

        int glassPrice = 5;
        int metalPrice = 5;
        int plasticPrice = 5;

        var bought = false;
        
        switch (currency)
        {
            case SubRecycleType.Glass:
                if (DataController.saveData.GlassCount >= glassPrice)
                {
                    DataController.saveData.GlassBitCount++;
                    DataController.saveData.GlassCount -= glassPrice;
                    bought = true;
                }
                break;
            case SubRecycleType.Metal:
                if (DataController.saveData.MetalCount >= metalPrice)
                {
                    DataController.saveData.MetalBitCount++;
                    DataController.saveData.MetalCount -= metalPrice;
                    bought = true;
                }
                break;
            case SubRecycleType.Plastic:
                if (DataController.saveData.PlasticCount >= plasticPrice)
                {
                    DataController.saveData.PlasticBitCount++;
                    DataController.saveData.PlasticCount -= plasticPrice;
                    bought = true;
                }
                break;
        }
        
        IEnumerator pointCoroutine(TMP_Text target, Sprite sprite)
        {
            var icon = new GameObject().AddComponent<Image>();
            icon.transform.SetParent(_recycleFrame);

            var trashCount = _recycleItemCounter.GetChild(type).GetComponentInChildren<TMP_Text>();
            
            var rectIcon = icon.transform as RectTransform;
            rectIcon.position = trashCount.rectTransform.position;

            rectIcon.anchorMin = Vector2.one * 0.5f;
            rectIcon.anchorMax = Vector2.one * 0.5f;

            rectIcon.sizeDelta = Vector2.one * 50f;
            icon.sprite = sprite;

            yield return rectIcon.DOMove(target.rectTransform.position, 0.25f).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(0.25f);
                    
            yield return icon.DOFade(0, 0.05f/2).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(0.075f/2);
                    



            var ct = int.Parse(trashCount.text);
            ct--;
            trashCount.text = ct.ToString();

            trashCount.DOColor(Color.red, 0.1f).OnComplete((() => trashCount.DOColor(Color.white, 0.1f)));
            
            Destroy(icon.gameObject);
        }

        IEnumerator c()
        {
            var count = 0;
            switch (currency)
            {
                case SubRecycleType.Glass:
                    count = glassPrice;
                    break;
                case SubRecycleType.Metal:
                    count = metalPrice;
                    break;
                case SubRecycleType.Plastic:
                    count = plasticPrice;
                    break;
            }

            var textInfo = _recycleSubItemCounter.GetChild(type).GetComponentInChildren<TMP_Text>();
            var _textInfo = _recycleItemCounter.GetChild(type).GetComponentInChildren<TMP_Text>();
            for (int i = 0; i < count; i++)
            {
                StartCoroutine(pointCoroutine(textInfo, _textInfo.transform.parent.RecursiveFind("IconImage").GetComponent<Image>().sprite));

                yield return new WaitForSeconds(0.125f/2);
            }
            
            var ct = int.Parse(textInfo.text);
            ct++;
            textInfo.text = ct.ToString();
            
            
            textInfo.DOColor(Color.green, 0.1f).OnComplete((() => textInfo.DOColor(Color.white, 0.1f)));
        }

        if (bought)
            StartCoroutine(c());

    }

    void calculateDistribution(int count, out int glass, out int plastic, out int metal)
    {
        var trashLeft = count;

        int glassCount = 0, metalCount = 0, plasticCount = 0;

        int tl_itr = Random.Range(0,2);
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
    
    public void RunPostMatchSequence(MatchController.PostMatchData pmd)
    {


        IEnumerator pms()
        {

            _isPostInfoRunning = true;
            _postInfoFrame.gameObject.SetActive(pmd.won);
            _postFailFrame.gameObject.SetActive(!pmd.won);
            FindFirstObjectByType<PlayerController>().BlockMovement = true;

            yield return new WaitForSeconds(5.0f);
            
            if (pmd.won)
            {
                _postTrashInfo.RecursiveFind<TMP_Text>("Count").text =
                    pmd.newCollectibles[CollectibleController.CollectibleType.GenericTrash].ToString();
                
                _postClothInfo.RecursiveFind<TMP_Text>("Count").text =
                    pmd.newCollectibles[CollectibleController.CollectibleType.Clothing].ToString();

                _postGlassInfo.RecursiveFind<TMP_Text>("Count").text = DataController.saveData.GlassCount.ToString();
                _postPlasticInfo.RecursiveFind<TMP_Text>("Count").text =
                    DataController.saveData.PlasticCount.ToString();

                _postMetalInfo.RecursiveFind<TMP_Text>("Count").text = DataController.saveData.MetalCount.ToString();
                _postFinalClothInfo.RecursiveFind<TMP_Text>("Count").text = DataController.saveData.ClothingScrapCount.ToString();

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

            yield return _postMatchFrame.DOAnchorPosY(0, 0.5f).SetEase(Ease.InOutQuad);

            if (pmd.won)
            {
                DataController.saveData.NextMap = DataController.saveData.NextMap.PossibleNextRounds.GetRandom();
                yield return new WaitForSeconds(1.0f);
                calculateDistribution(pmd.newCollectibles[CollectibleController.CollectibleType.GenericTrash], out var _glass, out var _plastic, out int _metal);
                var trashCount = _postTrashInfo.Find("Count").GetComponent<TMP_Text>();
                var clothCount = _postClothInfo.Find("Count").GetComponent<TMP_Text>();
                
                IEnumerator pointCoroutine(TMP_Text target, Sprite sprite, bool isTrash=true)
                {
                    var cot = isTrash ? trashCount : clothCount;
                    
                    var icon = new GameObject().AddComponent<Image>();
                    icon.transform.SetParent(_postMatchFrame);

                    var rectIcon = icon.transform as RectTransform;
                    rectIcon.position = cot.rectTransform.position;

                    rectIcon.anchorMin = Vector2.one * 0.5f;
                    rectIcon.anchorMax = Vector2.one * 0.5f;

                    rectIcon.sizeDelta = Vector2.one * 25f;
                    icon.sprite = sprite;

                    yield return rectIcon.DOMove(target.rectTransform.position, 0.125f/2).SetEase(Ease.InOutQuad);

                    yield return new WaitForSeconds(0.125f/2);
                    
                    yield return icon.DOFade(0, 0.05f/2).SetEase(Ease.InOutQuad);

                    yield return new WaitForSeconds(0.075f/2);
                    
                    Destroy(icon.gameObject);

                    var ct = int.Parse(target.text);
                    ct++;
                    target.text = ct.ToString();


                    
                    ct = int.Parse(cot.text);
                    ct--;
                    cot.text = ct.ToString();

                    target.DOColor(Color.green, 0.1f).OnComplete((() => target.DOColor(Color.white, 0.1f)));
                    cot.DOColor(Color.red, 0.1f).OnComplete((() => cot.DOColor(Color.white, 0.1f)));
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
                
                {
                    var textInfo = _postFinalClothInfo.Find("Count").GetComponent<TMP_Text>();
                    for (int i = 0; i < pmd.newCollectibles[CollectibleController.CollectibleType.Clothing]; i++)
                    {
                        StartCoroutine(pointCoroutine(textInfo,textInfo.transform.parent.Find("Icon").GetComponent<Image>().sprite,false));

                        yield return new WaitForSeconds(0.125f/2);
                    }
                }

                DataController.saveData.GlassCount += _glass;
                DataController.saveData.PlasticCount += _plastic;
                DataController.saveData.MetalCount += _metal;
                DataController.saveData.ClothingScrapCount += pmd.newCollectibles[CollectibleController.CollectibleType.Clothing];
                DataController.saveData.enemyInventory.AddRange(pmd.CaughtEnemies);

            }

            yield return new WaitForSeconds(2.5f);
            
            yield return _postMatchFrame.DOAnchorPosY(-540, 0.5f).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(0.75f);
            
            _postMatchFrame.gameObject.SetActive(false);
            _isPostInfoRunning = false;
            
            _postInfoFrame.RecursiveFind("EnemyTemplate").RemoveAllChildrenExcept(new List<string>(){"EnemyTemplate", "TrashInfo"});
            FindFirstObjectByType<PlayerController>().BlockMovement = false;
            
            yield break;
        }

        StartCoroutine(pms());
    }
}
