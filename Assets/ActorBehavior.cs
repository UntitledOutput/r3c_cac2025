using System;
using Controllers;
using UnityEngine;
using UnityEngine.AI;


public class ActorBehavior : MonoBehaviour
    {

        protected MatchController _matchController;
        private CapsuleCollider _capsuleCollider;
        protected NavMeshAgent _agent;
        
        [SerializeField]
        protected float _health = 1;

        public float Height
        {
            get
            {
                if (_agent)
                {
                    return _agent.height;
                }
                
                if (_capsuleCollider)
                {
                    return _capsuleCollider.height;
                }

                return 1;
            }
        }

        public float Size
        {
            get
            {
                if (_agent)
                {
                    return _agent.radius;
                }
                
                if (_capsuleCollider)
                {
                    return _capsuleCollider.radius;
                }

                return 0.5f;
            }
        }
        
        public enum ActorTeam
        {
            Player,
            Enemy
        }

        public ActorTeam Team;

        public void ChangeHealth(float diff)
        {
            _health += diff;
            _health = Mathf.Clamp(_health, 0, 1);
        }
        
        private void Awake()
        {
            _matchController = FindAnyObjectByType<MatchController>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _agent = GetComponent<NavMeshAgent>();
        }
    }