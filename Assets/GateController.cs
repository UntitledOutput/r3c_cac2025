using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GateController : MonoBehaviour
{
    private static readonly int IsActiveProperty = Shader.PropertyToID("_IsActive");
    public bool IsActive = true;
    public Material _fieldMaterial;

    private bool _lastActive = false;

    private RectTransform _mainFrame, _endFrame;

    public int GateIndex;
    
    
    public static int ChosenGateIndex;
    private static int GateCount = -1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainFrame = GameObject.Find("Canvas").transform.Find("MainFrame").GetComponent<RectTransform>();
        _endFrame = GameObject.Find("Canvas").transform.Find("EndFrame").GetComponent<RectTransform>();
        
        _endFrame.gameObject.SetActive(false);

        GateIndex = GateCount;
        GateCount++;
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldMaterial == null)
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                foreach (var rendererMaterial in renderer.materials)
                {
                    if (rendererMaterial.shader.name == "Shader Graphs/HexGate")
                    {
                        _fieldMaterial = rendererMaterial;
                        break;
                    }
                    else
                    {

                    }
                }

                if (_fieldMaterial != null)
                {
                    break;
                }


            }
            
        }
        else
        {

            if (_lastActive != IsActive)
            {
                _fieldMaterial.SetFloat("_StartTime", Time.time);

            }
            _fieldMaterial.SetInt(IsActiveProperty, IsActive ? 1 : 0);

            _lastActive = IsActive;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var plr = other.GetComponent<PlayerController>();

        if (plr && IsActive)
        {
            plr.BlockMovement = true;
            ChosenGateIndex = GateIndex;
            GateCount = 0;
            StartCoroutine(ClosingCoroutine(plr));
        }
    }

    IEnumerator ClosingCoroutine(PlayerController p)
    {
        _mainFrame.gameObject.SetActive(false);
        
        /*// move to center of gate
        {
            var end = transform.position + (transform.forward * 5);
            while (Vector3.Distance(p.transform.position, end) > 1.0f)
            {
                var dir = Vector3.MoveTowards(p.transform.position, end, Time.deltaTime*p.MoveSpeed);
                p.rigidbody.MovePosition(dir);

                yield return null;
            }
        }
        
        // move into gate
        {
            var end = transform.position - (transform.forward * 3);
            while (Vector3.Distance(p.transform.position, transform.position) > 1.0f)
            {
                var dir = Vector3.MoveTowards(p.transform.position, end, Time.deltaTime*p.MoveSpeed);
                p.rigidbody.MovePosition(dir);


                yield return null;
            }
        }*/

        var endFrame = _endFrame.GetComponentInChildren<Image>();
        _endFrame.gameObject.SetActive(true);

        endFrame.color = Color.clear;
        yield return endFrame.DOColor(Color.black, 1.0f);

        yield return new WaitForSeconds(0.9f);
        
        var postRound = _endFrame.Find("PostRoundFrame").GetComponent<RectTransform>();
        postRound.anchoredPosition = new Vector2(0, -1080);
        yield return         postRound.DOLocalMoveY(0, 1.0f);
    }
}
