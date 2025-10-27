using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObj;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Controllers
{
    public class AllyController : ActorBehavior
    {
        protected PlayerController _player;
        protected AbilityController _abilityController;

        protected CapsuleCollider _capsuleCollider;
        
        public bool _isAlive { get; protected set; }
        public bool _isTransformFinal;
        
        public enum AllyState
        {
            Idle,
            Attacking,
        }
        public AllyState State;
        
        [Serializable]
        public class AllyAbilityInstance
        {
            public AbilityObject Ability { get; private set; }
            [SerializeField] private float cooldownTimer = 0;
            [SerializeField] private int abilityIndex = -1;
            private AbilityController _controller;

            public float GetCooldownTimer() => cooldownTimer;

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
                    cooldownTimer = Ability.ReloadTime;
                    //animator.SetTrigger();
                }

                return worked;
            }
        
            public void Update()
            {
                cooldownTimer -= Time.deltaTime;
                if (abilityIndex == -1)
                    abilityIndex = _controller.Abilities.Select((instance => instance.data)).ToList().IndexOf(Ability);
            }
        
            public AllyAbilityInstance(AbilityObject ability, AbilityController controller)
            {
                Ability = ability;
                this._controller = controller;
            }
        }
        
        
        [SerializeField] private List<AllyAbilityInstance> abilities = new List<AllyAbilityInstance>();

        public AllyAbilityInstance GetAbility(int index) => abilities[index];

        public AllyObject.AllyInstance ally;
        
        private Slider _slider;
        
        protected virtual void Start()
        {
            Team = ActorTeam.Player;
            _player = FindAnyObjectByType<PlayerController>();
            _abilityController = GetComponent<AbilityController>();
            
            _abilityController.SetupAbilities(ally.ally.abilities, null);
            foreach (var abilityObject in ally.ally.abilities)
            {
                abilities.Add(new AllyAbilityInstance(abilityObject, _abilityController));
            }

            var slider = Instantiate(Resources.Load<GameObject>("Prefabs/HealthSlider"), transform);
            slider.transform.localPosition = new Vector3(0, Height*1.25f);
            
            _slider = slider.GetComponentInChildren<Slider>();

            var source = new ConstraintSource();
            source.sourceTransform = Camera.main.transform;
            source.weight = 1;
            _slider.GetComponentInParent<LookAtConstraint>().SetSource(0,source);
            _slider.GetComponentInParent<LookAtConstraint>().constraintActive = true;
            _slider.GetComponentInParent<Canvas>().worldCamera = Camera.main;

            _capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            //_capsuleCollider.isTrigger = true;
            _capsuleCollider.includeLayers = LayerMask.GetMask("Enemy", "Default");
            _capsuleCollider.excludeLayers = ~_capsuleCollider.includeLayers;
            
            _health = ally.ally.Health;
        }

        protected virtual void Update()
        {
            if (_agent)
            {
                _capsuleCollider.radius = _agent.radius;
                _capsuleCollider.height = _agent.height;
            }
            else
            {
                _capsuleCollider.radius = 1f;
                _capsuleCollider.height = 1f;
            }

            _health = Mathf.Clamp(_health, 0, ally.ally.Health);
            
            _slider.value = Mathf.Lerp(_slider.value, (_health / ally.ally.Health), Time.deltaTime * 10f);
            
            if ( ((_health / ally.ally.Health) < 1 || State == AllyState.Attacking) && Vector3.Distance(_slider.transform.position, Camera.main.transform.position) > 2f && ally.ally.ShowHealthBar)
            {
                if (!_slider.gameObject.activeSelf)
                    _slider.gameObject.SetActive(true);
            }
            else
            {
                if (_slider.gameObject.activeSelf)
                    _slider.gameObject.SetActive(false); 
            }

            
            _isTransformFinal = true;
            if (_health <= 0 && _isAlive)
            {
                OnDeath();
                _animator.SetBool("Dead", true);

                return;
            } else if (!_isAlive)
            {
                _health += Time.deltaTime / ally.ally.TimeToRecover;
                if (_health >= 1f)
                {
                    _isAlive = true;
                }
                
                return;
            }
            
            if (_animator)
            {
                _animator.SetFloat("Velocity", Velocity.magnitude);
                _animator.SetBool("Dead", false);
            }
            
            foreach (var allyAbilityInstance in abilities)
            {
                allyAbilityInstance.Update();
            }
        }

        protected bool TryAbility(int abilityIndex, Vector3 point)
        {
            return abilities[abilityIndex].Trigger(point, _animator);
        }

        public virtual void OnDeath()
        {
            _isAlive = false;
        }

        public override void ChangeHealth(float diff, ActorBehavior source)
        {
            base.ChangeHealth(diff, source);
        }

        public virtual void OnPlayerShot(int selectedAbility, Vector3 shootPoint)
        {
            
        }

        public virtual void OnPlayerChangeHealth(float diff, ActorBehavior source)
        {

        }
    }
    
}