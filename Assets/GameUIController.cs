using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using DG.Tweening;
using External;
using MyBox;
using ScriptableObj;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public static Sprite MissingSprite;
    private static readonly int CatchProgress = Shader.PropertyToID("_CatchProgress");

    private TMP_Text _timerText;
    private Slider _healthSlider;

    [Serializable]
    private class AbilityUIButton
    {
        public Image Icon;
        public TMP_Text AmmoCount;
        public Image ReloadMeter;
        public Image Back;

        public void UpdateButton(AbilityController.AbilityInstance instance, bool isChosen)
        {
            if (instance != null && instance.data)
            {
                Icon.sprite = instance.data.Icon;
                AmmoCount.text = $"{instance.ammo}/{instance.data.AmmoCount + (instance.upgrade?.AmmoChange ?? 0)}";
                ReloadMeter.fillAmount = instance.ammo < instance.data.AmmoCount ? instance.reload : 1;
            }
            else
            {
                Icon.sprite = null;
                AmmoCount.text = $"";
                ReloadMeter.fillAmount = 0;
            }

            Back.color = Back.color.SetAlpha(isChosen ? 1 : 0);
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
            uiButton.Back = abilityButton.GetComponent<Image>();
            uiButton.AmmoCount = abilityButton.RecursiveFind("AmmoCount").GetComponent<TMP_Text>();
            uiButton.ReloadMeter = abilityButton.RecursiveFind("ReloadMeter").GetComponent<Image>();
            abilityButton.GetComponent<Button>().onClick.AddListener((() =>
            {
                if (MatchController._instance.PassedAbilities[(_playerController.AbilityController.ChosenAbility)]
                        .Type == AbilityObject.AbilityType.Effect && _playerController.AbilityController.ChosenAbility ==
                    MatchController._instance.PassedAbilities.IndexOf(passedAbility))
                {
                    var _shootPoint = _playerAbilityController.GetShootPoint();
                    if (_playerAbilityController.TryShoot(_shootPoint))
                    {
                        foreach (var allyController in _playerController.Allies)
                        {
                            if (allyController)
                                allyController.OnPlayerShot(_playerAbilityController.ChosenAbility,_shootPoint);
                        }
                    }
                }
                _playerController.AbilityController.ChosenAbility =
                    MatchController._instance.PassedAbilities.IndexOf(passedAbility);
            }));
            
            _abilityButtons.Add(uiButton);
        }

        _healthSlider = transform.RecursiveFind<Slider>("HealthSlider");

        _playerController = FindAnyObjectByType<PlayerController>();
        _playerAbilityController = _playerController.GetComponent<AbilityController>();

        _timerText = transform.RecursiveFind<TMP_Text>("TimerText");
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < _abilityButtons.Count; i++)
        {
            _abilityButtons[i].UpdateButton(_playerAbilityController.Abilities[i],                 _playerController.AbilityController.ChosenAbility == i);
        }

        _healthSlider.value = Mathf.Lerp(_healthSlider.value, _playerController.Health, Time.deltaTime * 5f);

        int seconds = (int)(MatchController._instance.timeLeft % 60f);
        int minutes = (int)(MatchController._instance.timeLeft / 60f);

        seconds = Mathf.Clamp(seconds, 0, int.MaxValue);
        minutes = Mathf.Clamp(minutes, 0, int.MaxValue);

        _timerText.text = $"{minutes:0}:{seconds:00}";
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
    
    public Shader CatchShader;
    public Shader CatchEyeShader;

    public void StartCatchProcess(EnemyController enemy)
    {
        IEnumerator catchProcess()
        {
            FindFirstObjectByType<PlayerController>().BlockMovement = true;
            
            FindFirstObjectByType<PlayerController>().Model.gameObject.SetActive(false);
            var allies = FindObjectsByType<AllyController>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
            foreach (var allyController in allies)
            {
                allyController.Model.gameObject.SetActive(false);
            }

            var capture_bot = Instantiate(Resources.Load<GameObject>("Prefabs/OnboardBot")).transform;
            capture_bot.transform.position = enemy.transform.position + (enemy.transform.right * 10f);
            
            var cam = Camera.main.GetComponent<CameraController>();
            cam.FocusOnPlayer = false;
            
            cam.FocusPoint = enemy.transform;
            cam.PositionOffset = (enemy.transform.forward * (enemy.Size*5.0f)) + (Vector3.up*enemy.Height/2.5f);
            cam.FocusOffset = (Vector3.up*enemy.Height/2.5f);

            while (Vector3.Distance(capture_bot.position, enemy.transform.position+new Vector3(0,4,0)) > 0.1f)
            {
                capture_bot.position = Vector3.Lerp(
                    capture_bot.position, enemy.transform.position + new Vector3(0, 4, 0), Time.deltaTime * 5f);

                capture_bot.rotation = Quaternion.Lerp(
                    capture_bot.rotation, enemy.transform.rotation, Time.deltaTime * 5f);

                yield return null;
            }
            
            var catchFrame = transform.Find("CatchFrame");
            var captureButtonFrame = catchFrame.Find("CaptureFrame") as RectTransform;
            
            var progressCircle = captureButtonFrame.Find("ProgressCircle").GetComponent<Image>();
            
            transform.Find("MainFrame").gameObject.SetActive(false);
            
            captureButtonFrame.position = captureButtonFrame.position.SetY(-400f);
            
            yield return captureButtonFrame.DOAnchorPosY(27.5f, 0.5f).SetEase(Ease.InOutQuad);
            
            catchFrame.gameObject.SetActive(true);

            _catchProgress = 0.5f;
            
            Vector3 orgPos = enemy.transform.position;
            Renderer[] enemyRenderers = enemy.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in enemyRenderers) {
                foreach (Material material in renderer.materials) {
                    if (!material.shader.name.Contains("OutlineShader")) {
                        if (material.shader.name.Contains("EyeShader"))
                        {
                            material.shader = CatchEyeShader;
                        }
                        else
                        {
                            material.shader = CatchShader;
                        }
                    }
                }
            }

            
            while (_catchProgress > 0 && _catchProgress < 1)
            {

                _catchProgress -= 0.1f * Time.deltaTime;

                progressCircle.fillAmount = Mathf.Lerp(progressCircle.fillAmount, _catchProgress, Time.deltaTime*10f);
                progressCircle.color = Color.Lerp(Color.red, Color.green, _catchProgress);

                var scale = Vector3.Lerp(Vector3.one, Vector3.zero, _catchProgress);
                var pos = Vector3.Lerp(orgPos, orgPos+new Vector3(0,3,0),_catchProgress);

                var offset = Vector3.Lerp(Vector3.zero, Vector3.down * 3f, _catchProgress);
                
                cam.PositionOffset = (enemy.transform.forward * (enemy.Size*5.0f)) + (Vector3.up*enemy.Height/2.5f) + offset;
                cam.FocusOffset = (Vector3.up*enemy.Height/2.5f) + offset;

                enemy.transform.localScale = Vector3.Lerp(enemy.transform.localScale, scale, Time.deltaTime * 10f);
                enemy.transform.position = Vector3.Lerp(enemy.transform.position, pos,Time.deltaTime*10f);
                foreach (Renderer renderer in enemyRenderers) {
                    foreach (Material material in renderer.materials)
                    {
                        material.SetFloat(CatchProgress, _catchProgress);
                    }
                }

                yield return null;
            }

            if (_catchProgress >= 0.75f)
            {
                progressCircle.fillAmount = 1;
            }

            yield return captureButtonFrame.DOAnchorPosY(-400f, 0.5f).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(0.5f);
            
            if (_catchProgress >= 1)
            {
                var captureText = catchFrame.Find("CaptureText") as RectTransform;

                captureText.anchoredPosition = captureText.anchoredPosition.SetY(400);
                captureText.eulerAngles = captureText.eulerAngles.SetZ(0f);
                captureText.gameObject.SetActive(true);
                
                yield return captureText.DOAnchorPosY(0, 1.0f).SetEase(Ease.InOutQuad);
                yield return captureText.DORotate(new Vector3(0,0,15f), 1.5f).SetEase(Ease.InOutBack);

                yield return new WaitForSeconds(2.5f);
                
                yield return captureText.DOAnchorPosY(-400, 1.0f).SetEase(Ease.InOutQuad);
                yield return captureText.DORotate(new Vector3(0,0,0f), 1.5f).SetEase(Ease.InOutBack);

                MatchController._instance.AddCaughtEnemy(enemy.enemyObject);

                yield return new WaitForSeconds(2.0f);
            }
            else
            {
                var captureText = catchFrame.Find("FailCaptureText") as RectTransform;

                captureText.anchoredPosition = captureText.anchoredPosition.SetY(400);
                captureText.eulerAngles = captureText.eulerAngles.SetZ(0f);
                captureText.gameObject.SetActive(true);
                
                yield return captureText.DOAnchorPosY(0f, 1.0f).SetEase(Ease.InOutQuad);
                yield return captureText.DORotate(new Vector3(0,0,5f), 1.5f).SetEase(Ease.InOutBack);

                yield return new WaitForSeconds(2.5f);
                
                yield return captureText.DOAnchorPosY(-400, 1.0f).SetEase(Ease.InOutQuad);
                yield return captureText.DORotate(new Vector3(0,0,0f), 1.5f).SetEase(Ease.InOutBack);


                yield return new WaitForSeconds(2.0f);
            }

            yield return new WaitForSeconds(1.0f);
            
            catchFrame.gameObject.SetActive(false);
            
            GoToNextStage();
            
            
        }

        StartCoroutine(catchProcess());
    }
    
    public void Fail(string reason)
    {
        FindFirstObjectByType<PlayerController>().BlockMovement = true;

        var enemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var enemyController in enemies)
        {
            enemyController.gameObject.SetActive(false);
        }
        
        transform.Find("FailFrame").gameObject.SetActive(true);

        IEnumerator fail()
        {

            var failFrame = transform.Find("FailFrame").GetComponent<Image>();
            failFrame.color = failFrame.color.SetAlpha(0);


            var failText = failFrame.transform.Find("FailText").GetComponent<TMP_Text>();
            failText.color = failText.color.SetAlpha(0);
            
            var failReason = failFrame.transform.Find("FailReason").GetComponent<TMP_Text>();
            failReason.text = reason;
            failReason.color = failReason.color.SetAlpha(0);


            var failButton = failFrame.transform.Find("FailButton").GetComponent<Button>();
            failButton.onClick.RemoveAllListeners();
            
            failFrame.DOFade(1, 1.0f).SetEase(Ease.InOutExpo);
            failText.DOFade(1, 1.0f).SetEase(Ease.InOutExpo);
            failReason.DOFade(1, 1.0f).SetEase(Ease.InOutExpo);

            yield return new WaitForSeconds(0.75f);
            failButton.onClick.AddListener((() => MatchController._instance.ReturnToHome(true)));

            failButton.image.color = failButton.image.color.SetAlpha(0);
            failButton.image.DOFade(1, 1.0f).SetEase(Ease.InOutExpo);
            failButton.onClick.AddListener((() => MatchController._instance.ReturnToHome(true)));
        }
        
        StartCoroutine(fail());

    }
}
