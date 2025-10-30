using System;
using System.Collections;
using DG.Tweening;
using External;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{

    private RectTransform _dialogueFrame;
    private TMP_Text _nameText, _speechText;
    private Image _finishIcon;
    
    public static DialogueController Instance;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _dialogueFrame = transform.Find("DialogueFrame").GetComponent<RectTransform>();
        _nameText = _dialogueFrame.RecursiveFind<TMP_Text>("NameText");
        _speechText = _dialogueFrame.RecursiveFind<TMP_Text>("SpeechText");
        _finishIcon = _dialogueFrame.RecursiveFind<Image>("FinishIcon");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool _dialogueComplete;
    public IEnumerator ShowDialogue(string text, string name)
    {
        _dialogueComplete = false;
        _dialogueFrame.GetComponent<Button>().enabled = false;
        _dialogueFrame.localScale = Vector3.zero;
        _nameText.text = name;

        _speechText.text = "";
     
        _dialogueFrame.gameObject.SetActive(true);
        
        _dialogueFrame.DOScale(1.0f, 0.1f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.1f);
        
        foreach (var c in text)
        {
            _speechText.text += c;

            for (int i = 0; i < 3; i++)
            {
                yield return null;
            }
        }
        
        _finishIcon.gameObject.SetActive(true);
        _dialogueFrame.GetComponent<Button>().enabled = true;

        yield return new WaitUntil((() => _dialogueComplete));
        
        _dialogueFrame.DOScale(0.0f, 0.1f).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(0.125f);
             
        _dialogueFrame.gameObject.SetActive(false);
    }

    public void OnComplete()
    {
        _dialogueComplete = true;
    }
}
