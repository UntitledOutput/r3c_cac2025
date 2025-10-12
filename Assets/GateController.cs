using System;
using System.Collections;
using Controllers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GateController : MonoBehaviour
{
    private static readonly int IsActiveProperty = Shader.PropertyToID("_IsActive");
    public bool IsActive = true;
    public Material _fieldMaterial;

    private bool _lastActive = false;

    public int GateIndex;
    
    
    public static int ChosenGateIndex;
    private static int GateCount = 0;

    private void Awake()
    {
        GateCount = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            MatchController._instance.NextStage();
        }
    }
    
}
