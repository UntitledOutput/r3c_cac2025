using System;
using System.Collections;
using DG.Tweening;
using External;
using MyBox;
using TMPro;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    private RectTransform Top, Bottom, Icon;
    private TMP_Text TipText;
    private Canvas _canvas;

    public static LoadingScreenController LoadingScreen;

    private void Awake()
    {
        if (LoadingScreen)
            Destroy(gameObject);
        else
        {
            LoadingScreen = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Top = transform.Find("Top") as RectTransform;
        Bottom = transform.Find("Bottom") as RectTransform;
        Icon = transform.Find("IconSpin") as RectTransform;
        TipText = transform.RecursiveFind<TMP_Text>("TipText");
        _canvas = GetComponent<Canvas>();

        _canvas.enabled = false;
        
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Icon.Rotate(new Vector3(0,0,1),Time.deltaTime*50);
        LoadingScreen = this;
    }

    public void OpenLoadingScreen(Action onReady )
    {
        IEnumerator o()
        {

            Top.anchoredPosition = Top.anchoredPosition.SetY(300);
            Bottom.anchoredPosition = Bottom.anchoredPosition.SetY(-300);
            Icon.localScale = Vector3.zero;
            
            _canvas.enabled = true;

            Top.DOAnchorPosY(0, 2.0f).SetEase(Ease.InOutQuad);
            Bottom.DOAnchorPosY(0, 2.0f).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(1.0f);

            Icon.DOScale(1.0f, 2.5f).SetEase(Ease.OutBack);
        
            yield return new WaitForSeconds(3.5f);
            
            onReady.Invoke();
        }

        StartCoroutine(o());
    }

    public void CloseLoadingScreen()
    {
        IEnumerator o()
        {

            yield return new WaitForSeconds(1.0f);
            Top.anchoredPosition = Top.anchoredPosition.SetY(0);
            Bottom.anchoredPosition = Bottom.anchoredPosition.SetY(0);
            Icon.localScale = Vector3.one;
            


            Top.DOAnchorPosY(300, 2.0f).SetEase(Ease.InOutQuad);
            Bottom.DOAnchorPosY(-300, 2.0f).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(1.0f);

            Icon.DOScale(0.0f, 2.5f).SetEase(Ease.InBack);
        
            yield return new WaitForSeconds(3.5f);
            
            _canvas.enabled = false;
        }

        StartCoroutine(o());
    }


}
