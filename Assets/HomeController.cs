
using System;
using Controllers;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
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
    private Animator _spinnerAnimator;

    public List<AbilityObject> abilities;
    public List<AbilityObject.AbilityUpgrade> upgrades;
    
    public float SpinnerRotateAmt;
    public bool IsReadyToSpin = true;
    private int SectionIndex = 0;
    
    public enum SubRecycleType
    {
        Glass = 0,
        Plastic,
        Metal
    }

    [ButtonMethod]
    public void Spin()
    {
        if (IsReadyToSpin)
        {
            IsReadyToSpin = false;
            _spinnerAnimator.SetTrigger("Spin");
            SpinnerRotateAmt += 90;
            SectionIndex = (SectionIndex + 1 > 3) ? 0 : SectionIndex + 1;
            AlignUISection();
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spinnerAnimator = GameObject.Find("Spinner00").GetComponent<Animator>();

        _playFrame = BaseUtils.RecursiveFind("PlayFrame") as RectTransform;
        _playAbilityIcons = _playFrame.RecursiveFind("AbilityIconContent").GetAllChildren()
            .Select((transform1 => transform1.GetChild(0).GetComponent<Image>())).ToList();
        
        _upgradeFrame = _playFrame.parent.Find("UpgradeFrame") as RectTransform;
        _upgradeContent = _upgradeFrame.RecursiveFind<RectTransform>("UpgradeContent");
        _upgradeTemplate = _upgradeContent.Find("UpgradeTemplate") as RectTransform;
        _upgradeAbilityIcons = _upgradeFrame.RecursiveFind("AbilityIconContent").GetAllChildren()
            .Select((transform1 => transform1.GetChild(0).GetComponent<Image>())).ToList();
        
        _selectAbilityIcons = _upgradeFrame.RecursiveFind("SelectIconContent").GetAllChildren()
            .Select((transform1 => transform1.GetChild(0).GetComponent<Image>())).ToList();
        _selectPreview = _upgradeFrame.RecursiveFind("SelectPreview") as RectTransform;
        _selectContent = _upgradeFrame.RecursiveFind("SelectContent") as RectTransform;
        _selectTemplate = _selectContent.RecursiveFind<RectTransform>("SelectTemplate");
        
        _recycleFrame = _playFrame.parent.Find("RecycleFrame") as RectTransform;
        _recycleGlassCount = _recycleFrame.RecursiveFind<TMP_Text>("GlassCount");
        _recyclePlasticCount = _recycleFrame.RecursiveFind<TMP_Text>("PlasticCount");
        _recycleMetalCount = _recycleFrame.RecursiveFind<TMP_Text>("MetalCount");
        
        _recycleGlassBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubGlassCount");
        _recyclePlasticBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubPlasticCount");
        _recycleMetalBitCount = _recycleFrame.RecursiveFind<TMP_Text>("SubMetalCount");
        
        
        if (upgrades.Count < abilities.Count)
        {
            while (upgrades.Count < abilities.Count)
            {
                upgrades.Add(null);
            }
        }
        
        SetupPlayFrame();
        SetupUpgradeFrame();
        
        var matchCtrl = FindAnyObjectByType<MatchController>();
        if (matchCtrl)
        {
            var data = matchCtrl.CreatePostMatchData();
            
            foreach (var dataNewCollectible in data.newCollectibles)
            {
                switch (dataNewCollectible.Key)
                {
                    case CollectibleController.CollectibleType.GenericTrash:
                        var trashLeft = dataNewCollectible.Value;

                        int glassCount = 0, metalCount = 0, plasticCount = 0;
                        
                        for (int i = 0; i < dataNewCollectible.Value; i++)
                        {
                            var val = Random.Range(0, trashLeft);
                            switch (i)
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

                            trashLeft -= val;
                        }

                        DataController.saveData.GlassCount += glassCount;
                        DataController.saveData.MetalCount += metalCount;
                        DataController.saveData.PlasticCount += plasticCount;
                        break;
                }
            }
            
            DestroyImmediate(matchCtrl.gameObject);
        }
        
        AlignUISection();
        
    }

    private void Update()
    {
         if (InputSystem.NavigateNext)
         {
             Spin();
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
        SceneManager.LoadScene("GameScene");
    }
    
    // ui section

    private Sprite MissingSprite = null;

    private RectTransform _playFrame;
    private List<Image> _playAbilityIcons = new List<Image>();
    
    private RectTransform _upgradeFrame;
    private RectTransform _upgradeContent;
    private RectTransform _upgradeTemplate;
    private List<Image> _upgradeAbilityIcons = new List<Image>();
    
    private RectTransform _selectPreview;
    private RectTransform _selectContent;
    private RectTransform _selectTemplate;
    private List<Image> _selectAbilityIcons = new List<Image>();
    private int _selectedAbilitySlot = -1;
    private int _upgradeAbilitySlot = -1;

    private RectTransform _recycleFrame;
    private TMP_Text _recycleGlassCount;
    private TMP_Text _recyclePlasticCount;
    private TMP_Text _recycleMetalCount;
    private TMP_Text _recycleGlassBitCount;
    private TMP_Text _recyclePlasticBitCount;
    private TMP_Text _recycleMetalBitCount;

    public void AlignUISection()
    {
        
        _playFrame.gameObject.SetActive(false);
        _upgradeFrame.gameObject.SetActive(false);
        _recycleFrame.gameObject.SetActive(false);
        switch (SectionIndex)
        {
            case 0:
                _playFrame.gameObject.SetActive(true);
                break;
            case 1:
                _upgradeFrame.gameObject.SetActive(true);
                break;
            case 2:
                _recycleFrame.gameObject.SetActive(true);
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
                _upgradeAbilityIcons[i].sprite = abilities[i].Icon;
                _selectAbilityIcons[i].sprite = abilities[i].Icon;
                _playAbilityIcons[i].sprite = abilities[i].Icon;
                
            }
            else
            {
                _upgradeAbilityIcons[i].sprite = MissingSprite;
                _selectAbilityIcons[i].sprite = MissingSprite;
                _playAbilityIcons[i].sprite = MissingSprite;
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
        
        ChangeUpgradeSection(0);
        ChangeSelectSection(0);
    }
    
    public void ChangeUpgradeSection(int abilityIndex)
    {

        _upgradeAbilitySlot = abilityIndex;
        var ability = abilities[abilityIndex];
        
        _upgradeContent.RemoveAllChildrenExcept(_upgradeTemplate.name);

        if (ability)
        {
            foreach (var abilityUpgrade in ability.Upgrades)
            {
                var upgradeFrame = Instantiate(_upgradeTemplate, _upgradeTemplate.parent);
                upgradeFrame.Find("UpgradeName").GetComponent<TMP_Text>().text = abilityUpgrade.Name;
                upgradeFrame.Find("UpgradeDesc").GetComponent<TMP_Text>().text = abilityUpgrade.Description;
                upgradeFrame.Find("UpgradeIcon").GetComponent<Image>().sprite = abilityUpgrade.Icon;

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

                    ChangeUpgradeSection(abilityIndex);
                }));

                upgradeFrame.gameObject.SetActive(true);
            }
        }
    }

    public void ChangeSelectSection(int abilityIndex)
    {
        _selectedAbilitySlot = abilityIndex;
        ChangeSelectPreview(abilities[abilityIndex]);
    }

    public void ChangeAbilityInSlot(AbilityObject ability, int abilityIndex)
    {
        if (ability)
        {
            foreach (var abilityObject in abilities)
            {
                if (abilityObject == ability)
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
        if (abilityIndex == _upgradeAbilitySlot)
            ChangeUpgradeSection(abilityIndex);
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
}
