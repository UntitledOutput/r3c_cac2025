using System;
using System.Collections.Generic;
using Controllers;
using External;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public static Sprite MissingSprite;
    
    private TMP_Text _timerText;
    private Slider _healthSlider;

    [Serializable]
    private class AbilityUIButton
    {
        public Image Icon;
        public TMP_Text AmmoCount;
        public Image ReloadMeter;

        public void UpdateButton(AbilityController.AbilityInstance instance)
        {
            if (instance.data)
            {
                Icon.sprite = instance.data.Icon;
                AmmoCount.text = $"{instance.ammo}/{instance.data.AmmoCount + (instance.upgrade?.AmmoChange ?? 0)}";
                ReloadMeter.fillAmount = instance.reload;
            }
            else
            {
                Icon.sprite = null;
                AmmoCount.text = $"";
                ReloadMeter.fillAmount = 0;
            }
        }
    }

    private List<AbilityUIButton> _abilityButtons = new List<AbilityUIButton>();
    private AbilityController _playerAbilityController;
    private PlayerController _playerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var abilityFrame = transform.RecursiveFind("AbilityFrame");
        var template = abilityFrame.Find("AbilityTemplate");
        
        foreach (var passedAbility in MatchController._instance.PassedAbilities)
        {
            if (!passedAbility)
                continue;
            
            var abilityButton = Instantiate(template, template.parent);
            abilityButton.gameObject.SetActive(true);
            var uiButton = new AbilityUIButton();
            uiButton.Icon = abilityButton.Find("Icon").GetComponent<Image>();
            uiButton.AmmoCount = abilityButton.RecursiveFind("AmmoCount").GetComponent<TMP_Text>();
            uiButton.ReloadMeter = abilityButton.RecursiveFind("ReloadMeter").GetComponent<Image>();
            
            _abilityButtons.Add(uiButton);
        }

        _healthSlider = transform.RecursiveFind<Slider>("HealthSlider");

        _playerController = FindAnyObjectByType<PlayerController>();
        _playerAbilityController = _playerController.GetComponent<AbilityController>();
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < _abilityButtons.Count; i++)
        {
            _abilityButtons[i].UpdateButton(_playerAbilityController.Abilities[i]);
        }

        _healthSlider.value = Mathf.Lerp(_healthSlider.value, _playerController.Health, Time.deltaTime * 5f);
    }

    public void GoToNextStage()
    {
        MatchController._instance.NextStage();
    }
}
