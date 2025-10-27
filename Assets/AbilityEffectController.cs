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
    public float Radius = 1f;

    private Renderer _renderer;
    private ParticleSystem[] _particleSystems;

    public List<ActorBehavior> actorsInRange = new List<ActorBehavior>();
    public ActorBehavior actor;

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

        if (!_renderer)
            _renderer = GetComponentInChildren<Renderer>();
        
        
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
        _renderer.material.SetFloat(EffectTime, 0);
    }

    // Update is called once per frame
    protected virtual void Update()
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
        var caughtColliders = Physics.OverlapSphere(transform.position, Radius*transform.lossyScale.magnitude, LayerMask.GetMask("Enemy", "Player", "Default"));
        
        foreach (var caughtCollider in caughtColliders)
        {
            var actorBehavior = caughtCollider.GetComponent<ActorBehavior>();
            if (ability && actorBehavior && actorBehavior.Team == ability.Target)
            {
                actorBehavior.ChangeHealth(-(ability.Damage+(upgrade?.DamageChange ?? 0))*Time.fixedDeltaTime,actor);

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,Radius*transform.lossyScale.magnitude);
    }
}