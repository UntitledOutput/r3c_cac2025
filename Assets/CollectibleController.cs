using System;
using Controllers;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    
    public enum CollectibleType
    {
        GenericTrash,
        Clothing,
        End
    }

    public CollectibleType Type;

    private PlayerController _player;
    private Transform _model;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _model = transform.Find("Model");
    }

    // Update is called once per frame
    void Update()
    {
        var distFromPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distFromPlayer <= 4)
        {
            var dir = _player.transform.position - transform.position;
            dir = dir.normalized;

            transform.position += dir * (Time.deltaTime * (4 - distFromPlayer));
        }
    }

    private void FixedUpdate()
    {
        _model.localEulerAngles += new Vector3(0, Time.fixedDeltaTime*45f, 0);
        _model.transform.localPosition = new Vector3(0, Mathf.Sin(Time.time * 0.75f) * 0.25f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player.gameObject)
        {
            MatchController._instance.IncrementCollectible(Type);
            Destroy(gameObject);
        }
    }
}
