using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;
using UnityEngine.AI;


public class ActorBehavior : MonoBehaviour
    {

        protected MatchController _matchController;
        protected CapsuleCollider _capsuleCollider;
        protected NavMeshAgent _agent;
        protected Animator _animator;
        
        [SerializeField]
        protected float _health = 1;

        public bool IsAlive => _health > 0;

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
        public Vector3 Velocity
        {
            get
            {
                if (_agent && _agent.enabled)
                    return _agent.velocity;



                return transform.position - _cachedPosition;
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
            _camera = Camera.main;
            _matchController = FindAnyObjectByType<MatchController>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }

        private Vector3 _cachedPosition;
        private Camera _camera;
        private List<Renderer> _renderers = new List<Renderer>();

        protected virtual void LateUpdate()
        {
            if (_renderers.Count <= 0)
                _renderers = gameObject.GetComponentsInChildren<Renderer>().ToList();
            
            _cachedPosition = transform.position;

            var distanceFromCamera = Vector3.Distance(transform.position, _camera.transform.position);
            if (distanceFromCamera > 40f)
            {
                foreach (var renderer in _renderers)
                {
                    renderer.enabled = false;
                }

                _animator.enabled = false;
            }
            else
            {
                foreach (var renderer in _renderers)
                {
                    renderer.enabled = true;
                }

                _animator.enabled = true;
            }
        }
    }