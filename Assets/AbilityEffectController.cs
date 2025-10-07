using System;
using System.Collections.Generic;
using MyBox;
using ScriptableObj;
using UnityEngine;

public class AbilityEffectController : MonoBehaviour
{
    private static readonly int EffectTime = Shader.PropertyToID("_EffectTime");
    public AbilityObject ability;
    public AbilityObject.AbilityUpgrade upgrade;

    private float _time = 0;

    private Renderer _renderer;
    private ParticleSystem[] _particleSystems;

    private List<ActorBehavior> actorsInRange = new List<ActorBehavior>();

#if UNITY_EDITOR
    [ButtonMethod]
    public void Try()
    {
        _time = 0;
        enabled = true;
        _renderer.material.SetFloat(EffectTime, 0);
        foreach (var system in _particleSystems)
        {
            system.Play();
        }
    }
#endif
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
        _renderer.material.SetFloat(EffectTime, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (ability && ability.Type != AbilityObject.AbilityType.Effect)
        {
            ability = null;
            _time = 0;
        }

        if (ability)
        {
            if (_time < 1)
            {
                _time += Time.deltaTime / (ability.Lifetime + (upgrade?.LifetimeChange ?? 0));

                if (_time >= 1)
                {
                    Destroy(gameObject);
                    foreach (var system in _particleSystems)
                    {
                        system.Stop();
                    }
                }
            }

            _renderer.material.SetFloat(EffectTime, Mathf.Lerp(_renderer.material.GetFloat(EffectTime), _time, Time.deltaTime*5f));

            
            

        }
    }

    private void FixedUpdate()
    {
        foreach (var actorBehavior in actorsInRange)
        {
            if (ability && actorBehavior && actorBehavior.Team == ability.Target)
            {
                actorBehavior.ChangeHealth(-(ability.Damage+(upgrade?.DamageChange ?? 0))*Time.fixedDeltaTime);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        var actorBehavior = other.GetComponent<ActorBehavior>();
        actorsInRange.Add(actorBehavior);
    }

    private void OnTriggerExit(Collider other)
    {
        var actorBehavior = other.GetComponent<ActorBehavior>();
        actorsInRange.Remove(actorBehavior);
    }
}
