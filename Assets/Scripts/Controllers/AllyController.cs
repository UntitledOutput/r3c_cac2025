using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObj;
using UnityEngine;

namespace Controllers
{
    public class AllyController : ActorBehavior
    {
        protected PlayerController _player;
        protected AbilityController _abilityController;
        
        protected bool _isAlive;
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
        }

        protected virtual void Update()
        {
            _isTransformFinal = true;
            if (_health <= 0 && _isAlive)
            {
                OnDeath();

                return;
            }
            
            if (_animator)
            {
                _animator.SetFloat("Velocity", Velocity.magnitude);
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

        public virtual void OnPlayerShot(int selectedAbility, Vector3 shootPoint)
        {
            
        }
    }
    
}