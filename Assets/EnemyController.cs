using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using DefaultNamespace;
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
    public ActorBehavior target;

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
                ? DataController.saveData.NextMap.BigEnemies.GetRandom()
                : DataController.saveData.NextMap.SmallEnemies.GetRandom();
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

        _health = enemyObject.GetMaxHealth(IsMegaEnemy);
        
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

                    
            _slider.value = Mathf.Lerp(_slider.value, (_health / (enemyObject.GetMaxHealth(IsMegaEnemy))), Time.deltaTime * 10f);

            if ((_health / (enemyObject.GetMaxHealth(IsMegaEnemy) ) ) < 1 || _enemyState == EnemyState.Attacking)
            {
                if (!_slider.gameObject.activeSelf)
                    _slider.gameObject.SetActive(true);
            }
            else
            {
                if (_slider.gameObject.activeSelf)
                    _slider.gameObject.SetActive(false); 
            }
            
            foreach (var collider in Physics.OverlapSphere(transform.position, enemyObject.GetDetectionRadius(IsMegaEnemy), LayerMask.GetMask("Player")))
            {
                var enemy = collider.GetComponent<ActorBehavior>();

                if (enemy && enemy.Team == ActorTeam.Player && enemy.IsAlive)
                {
                    var ally = enemy.As<AllyController>();
                    if (ally && !ally._isAlive)
                        continue;
                    
                    _enemyState = EnemyState.Attacking;
                    target = enemy;
                }
            }
            

            if (_enemyState == EnemyState.Attacking && _isAlive)
            {
                var distance = Vector3.Distance(target.transform.position, transform.position);
                _agent.isStopped = false;

                _abilityController._targetActor = target;
                
                if (distance > enemyObject.GetStoppingDistance(IsMegaEnemy))
                {
                    _agent.SetDestination(target.transform.position);
                    TriggerAbilityOnCondition(target.transform.position, EnemyObject.EnemyAbilityType.OnChase);
                }
                else
                {
                    _agent.isStopped = true;

                    var eul = transform.eulerAngles;
                    transform.LookAt(target.transform.position);
                    transform.eulerAngles = Vector3.Slerp(eul, transform.eulerAngles, Time.deltaTime * 5f);

                    TriggerAbilityOnCondition(target.transform.position, EnemyObject.EnemyAbilityType.OnStop);
                }

                if (distance > (enemyObject.GetDetectionRadius(IsMegaEnemy)))
                    _enemyState = EnemyState.Idle;
            }
            else
            {
                _abilityController._targetActor = null;
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
                Resources.Load<GameObject>($"Prefabs/Collectibles/{enemyObject.DropCollectible.GetRandom().ToString()}"),
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

        Gizmos.DrawIcon(transform.position+new Vector3(0,Height/2,0), $"Enemy/{enemyObject.Icon.name}.png");
        if (_enemyState == EnemyState.Attacking)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position,target.transform.position);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyObject.GetStoppingDistance(IsMegaEnemy));
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, (enemyObject.GetDetectionRadius(IsMegaEnemy)));
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, enemyObject.GetDetectionRadius(IsMegaEnemy));

            
        }

        if (_agent)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _agent.destination);
        }
    }
}
