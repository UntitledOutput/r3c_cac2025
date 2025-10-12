using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using DG.Tweening;
using External;
using MyBox;
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

    private float _catchProgress;
    
    public void HandleCatchButton()
    {
        _catchProgress += 0.05f;
    }
    
    public void StartCatchProcess(EnemyController enemy)
    {
        IEnumerator catchProcess()
        {

            var cam = Camera.main.GetComponent<CameraController>();
            cam.FocusOnPlayer = false;
            
            cam.FocusPoint = enemy.transform;
            cam.PositionOffset = (enemy.transform.forward * (enemy.Size*5.0f)) + (Vector3.up*enemy.Height/2.5f);
            cam.FocusOffset = (Vector3.up*enemy.Height/2.5f);
            
            
            
            var catchFrame = transform.Find("CatchFrame");
            var captureButtonFrame = catchFrame.Find("CaptureFrame");
            
            var progressCircle = captureButtonFrame.Find("ProgressCircle").GetComponent<Image>();
            
            transform.Find("MainFrame").gameObject.SetActive(false);
            
            captureButtonFrame.position = captureButtonFrame.position.SetY(-400f);
            
            yield return captureButtonFrame.DOMoveY(27.5f, 0.5f).SetEase(Ease.InOutQuad);
            
            catchFrame.gameObject.SetActive(true);

            _catchProgress = 0.5f;
            
            while (_catchProgress > 0 && _catchProgress < 1)
            {

                _catchProgress -= 0.1f * Time.deltaTime;

                progressCircle.fillAmount = _catchProgress;
                progressCircle.color = Color.Lerp(Color.red, Color.green, _catchProgress);
                
                yield return null;
            }

            yield return captureButtonFrame.DOMoveY(-400f, 0.5f).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(0.5f);
            
            if (_catchProgress >= 1)
            {
                var captureText = catchFrame.Find("CaptureText") as RectTransform;

                captureText.position = captureText.position.SetY(1300);
                captureText.eulerAngles = captureText.eulerAngles.SetZ(0f);
                captureText.gameObject.SetActive(true);
                
                yield return captureText.DOMoveY(540f, 1.0f).SetEase(Ease.InOutQuad);
                yield return captureText.DORotate(new Vector3(0,0,15f), 1.5f).SetEase(Ease.InOutBack);

                yield return new WaitForSeconds(2.5f);
                
                yield return captureText.DOMoveY(-250, 1.0f).SetEase(Ease.InOutQuad);
                yield return captureText.DORotate(new Vector3(0,0,0f), 1.5f).SetEase(Ease.InOutBack);

                MatchController._instance.AddCaughtEnemy(enemy.enemyObject);

                yield return new WaitForSeconds(2.0f);
            }
            else
            {
                var captureText = catchFrame.Find("FailCaptureText") as RectTransform;

                captureText.position = captureText.position.SetY(1300);
                captureText.eulerAngles = captureText.eulerAngles.SetZ(0f);
                captureText.gameObject.SetActive(true);
                
                yield return captureText.DOMoveY(540f, 1.0f).SetEase(Ease.InOutQuad);
                yield return captureText.DORotate(new Vector3(0,0,5f), 1.5f).SetEase(Ease.InOutBack);

                yield return new WaitForSeconds(2.5f);
                
                yield return captureText.DOMoveY(-250, 1.0f).SetEase(Ease.InOutQuad);
                yield return captureText.DORotate(new Vector3(0,0,0f), 1.5f).SetEase(Ease.InOutBack);


                yield return new WaitForSeconds(2.0f);
            }

            yield return new WaitForSeconds(1.0f);
            
            catchFrame.gameObject.SetActive(false);
            
            GoToNextStage();
            
            
        }

        StartCoroutine(catchProcess());
    }
    
    public void Fail()
    {
        transform.Find("FailFrame").gameObject.SetActive(true);

        
        var failFrame = transform.Find("FailFrame").GetComponent<Image>();
        failFrame.color = failFrame.color.SetAlpha(0);
        failFrame.DOFade(1, 1.0f).SetEase(Ease.InOutExpo);

        var failText = failFrame.transform.Find("FailText").GetComponent<TMP_Text>();
        failText.color = failText.color.SetAlpha(0);
        failText.DOFade(1, 1.0f).SetEase(Ease.InOutExpo);
        
        var failButton = failFrame.transform.Find("FailButton").GetComponent<Button>();
        failButton.onClick.AddListener((() => MatchController._instance.ReturnToHome()));
        
        failButton.image.color = failButton.image.color.SetAlpha(0);
        failButton.image.DOFade(1, 1.0f).SetEase(Ease.InOutExpo);

    }
}
