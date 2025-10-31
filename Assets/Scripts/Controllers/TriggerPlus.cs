using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerPlus : MonoBehaviour
{
    public UnityEvent OnEnter;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            OnEnter.Invoke();
        }
    }
}
