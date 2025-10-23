using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using External;
using MyBox;
using NUnit.Framework;
using ScriptableObj;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.UI;

public class EnemyController : ActorBehavior
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int IsMega = Shader.PropertyToID("_IsMega");
    private bool _isAlive = true;
    private Slider _slider;
    
    private enum EnemyState
    {
        Idle,
        Attacking,
        Stunned,
    }

    [SerializeField, ReadOnly] private EnemyState _enemyState = EnemyState.Idle;
    private PlayerController _player;

    public EnemyObject enemyObject;
    public bool IsMegaEnemy;

    private AbilityController _abilityController;
    
    [Serializable]
    
    public class EnemyAbilityInstance
    {
        public EnemyObject.EnemyAbility Ability { get; private set; }
        [SerializeField] private float cooldownTimer = 0;
        [SerializeField] private int abilityIndex = -1;
        private AbilityController _controller;

        public bool Trigger(Vector3 shootPoint, Animator animator)
        {
            if (abilityIndex < 0)
                return false;
            
            if (cooldownTimer > 0)
                return false;

            _controller.ChosenAbility = abilityIndex;

            var worked = _controller.TryShoot(shootPoint);

            if (worked)
            {
                cooldownTimer = Ability.cooldown;
                animator.SetTrigger(Attack);
            }

            return worked;
        }
        
        public void Update()
        {
            cooldownTimer -= Time.deltaTime;
            if (abilityIndex == -1)
                abilityIndex = _controller.Abilities.Select((instance => instance.data)).ToList().IndexOf(Ability.ability);
        }
        
        public EnemyAbilityInstance(EnemyObject.EnemyAbility ability, AbilityController controller)
        {
            Ability = ability;
            this._controller = controller;
        }
    }

    [SerializeField] private List<EnemyAbilityInstance> abilities = new List<EnemyAbilityInstance>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enemyObject == null)
        {
            enemyObject = IsMegaEnemy
                ? _matchController.LargeEnemies.GetRandom()
                : _matchController.SmallEnemies.GetRandom();
        }
        
        var model = transform.Find("Model");
        var enemyModel = Instantiate(enemyObject.Prefab, model);

        if (IsMegaEnemy)
        {
            enemyModel.transform.localScale = Vector3.one * enemyObject.MegaSize;
        }
        else
        {
            enemyModel.transform.localScale = Vector3.one * enemyObject.SmallSize;
        }
        
        _agent.radius = enemyObject.Width * (IsMegaEnemy ? enemyObject.MegaSize : enemyObject.SmallSize);
        _agent.height = enemyObject.Height * (IsMegaEnemy ? enemyObject.MegaSize : enemyObject.SmallSize);

        _capsuleCollider.radius = _agent.radius;
        _capsuleCollider.height = _agent.height;
        
        _slider = GetComponentInChildren<Slider>();

        _animator = GetComponentInChildren<Animator>();

        var source = new ConstraintSource();
        source.sourceTransform = Camera.main.transform;
        source.weight = 1;
        _slider.GetComponentInParent<LookAtConstraint>().SetSource(0,source);
        _slider.GetComponentInParent<LookAtConstraint>().constraintActive = true;
        _slider.GetComponentInParent<Canvas>().worldCamera = Camera.main;

        _health = enemyObject.HealthMax + ((IsMegaEnemy ? enemyObject.MegaHealthOffset : 0));
        
        _matchController.RegisterEnemy(this);
        Team = ActorTeam.Enemy;
        
        _abilityController = gameObject.AddComponent<AbilityController>();
        foreach (var enemyObjectAbility in enemyObject.Abilities)
        {
            abilities.Add(new EnemyAbilityInstance(enemyObjectAbility,_abilityController));
        }
        
        _abilityController.SetupAbilities(abilities.Select((instance => instance.Ability.ability)).ToList(), null);

        var renderers = GetComponentsInChildren<Renderer>();
        
        foreach (var renderer in renderers)
        {
            foreach (var rendererMaterial in renderer.materials)
            {
                rendererMaterial.SetFloat(IsMega, IsMegaEnemy ? 1 : 0);
            }
        }

    }

    public void TriggerAbilityOnCondition(Vector3 shootPoint,EnemyObject.EnemyAbilityType type)
    {

        foreach (var enemyAbilityInstance in abilities)
        {
            if (enemyAbilityInstance.Ability.type == type || enemyAbilityInstance.Ability.type == EnemyObject.EnemyAbilityType.Any)
            {

                if (enemyAbilityInstance.Trigger(shootPoint, _animator))
                {
                    return;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_health <= 0 && _isAlive)
        {
            OnDeath();

            return;
        }
        
        foreach (var enemyAbilityInstance in abilities)
        {
            enemyAbilityInstance.Update();
        }
        

        if (_isAlive)
        {

                    
            _slider.value = Mathf.Lerp(_slider.value, (_health / (enemyObject.HealthMax + (IsMegaEnemy ? enemyObject.MegaHealthOffset : 0))), Time.deltaTime * 10f);

            if ((_health / (enemyObject.HealthMax + (IsMegaEnemy ? enemyObject.MegaHealthOffset : 0) ) ) < 1 || _enemyState == EnemyState.Attacking)
            {
                if (!_slider.gameObject.activeSelf)
                    _slider.gameObject.SetActive(true);
            }
            else
            {
                if (_slider.gameObject.activeSelf)
                    _slider.gameObject.SetActive(false); 
            }
            
            foreach (var collider in Physics.OverlapSphere(transform.position, enemyObject.DetectionRadius + (IsMegaEnemy ? enemyObject.MegaDetectionOffset : 0)))
            {
                var enemy = collider.GetComponent<EnemyController>();
                var player = collider.GetComponent<PlayerController>();

                if (player)
                {
                    _enemyState = EnemyState.Attacking;
                    _player = player;
                }
                else if (enemy)
                {

                }
            }

            if (_enemyState == EnemyState.Attacking && _isAlive)
            {
                var distance = Vector3.Distance(_player.transform.position, transform.position);
                _agent.isStopped = false;
                if (distance > enemyObject.StoppingDistance + (IsMegaEnemy ? enemyObject.MegaStoppingOffset : 0))
                {
                    _agent.SetDestination(_player.transform.position);
                    TriggerAbilityOnCondition(_player.transform.position, EnemyObject.EnemyAbilityType.OnChase);
                }
                else
                {
                    _agent.isStopped = true;

                    var eul = transform.eulerAngles;
                    transform.LookAt(_player.transform.position);
                    transform.eulerAngles = Vector3.Slerp(eul, transform.eulerAngles, Time.deltaTime * 5f);

                    TriggerAbilityOnCondition(_player.transform.position, EnemyObject.EnemyAbilityType.OnStop);
                }

                if (distance > (enemyObject.DetectionRadius + (IsMegaEnemy ? enemyObject.MegaDetectionOffset : 0)) * 1.5)
                    _enemyState = EnemyState.Idle;
            }

        }

        if (_enemyState == EnemyState.Stunned)
        {
            _agent.isStopped = true;
            _slider.gameObject.SetActive(false);
        }

        if (_animator)
        {
            _animator.SetFloat("Velocity", Velocity.magnitude);
        }
    }


    public void OnDeath()
    {
        _enemyState = EnemyState.Stunned;
        _isAlive = false;
        _matchController.DeregisterEnemy(this);

        if (!IsMegaEnemy)
        {
            
            var deathParticles = Instantiate(
                Resources.Load<GameObject>("Prefabs/Particles/DeathParticle00"), transform.position, quaternion.identity);

            var collectible = Instantiate(
                Resources.Load<GameObject>("Prefabs/Collectibles/ItemProjectile"),
                transform.position + new Vector3(0, 1, 0), transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            FindFirstObjectByType<GameUIController>().StartCatchProcess(this);
        }



    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, enemyObject.DetectionRadius + (IsMegaEnemy ? enemyObject.MegaDetectionOffset : 0));
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, (enemyObject.DetectionRadius+(IsMegaEnemy ? enemyObject.MegaDetectionOffset : 0))*1.5f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyObject.StoppingDistance +(IsMegaEnemy ? enemyObject.MegaStoppingOffset : 0));

        if (_enemyState == EnemyState.Attacking)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position,_player.transform.position);
            
        }

        if (_agent)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _agent.destination);
        }
    }
}
