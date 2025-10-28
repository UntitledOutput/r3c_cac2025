using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class ActorBehavior : MonoBehaviour
    {

        protected MatchController _matchController;
        protected CapsuleCollider _capsuleCollider;
        protected NavMeshAgent _agent;
        protected Animator _animator;
        public Transform Model { get; private set; }
        
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

        public virtual void ChangeHealth(float diff, ActorBehavior source)
        {
            _health += diff;
            _health = Mathf.Clamp(_health, 0, 1);
        }

        private bool lerping = false;
        IEnumerator LerpToPos(Vector3 p, float length)
        {
            lerping = true;

            float timeElapsed = 0;
            float moveDuration = length;

            var startPosition = transform.position;
            
            do
            {
                // Add time since last frame to the time elapsed
                timeElapsed += Time.deltaTime;

                float normalizedTime = timeElapsed / moveDuration;
                //normalizedTime = Easing.EaseInOutQuint(normalizedTime);

                // Interpolate position and rotation
                transform.position = Vector3.Lerp(startPosition, p,normalizedTime);

                var pos = transform.position;
                if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                }

                _agent.isStopped = true;
                
                // Wait for one frame
                yield return null;
            } while (timeElapsed < moveDuration);
            
            lerping = false;
        }
        public void LerpToPosition(Vector3 position, float time)
        {
            if (!lerping)
            {
                StartCoroutine(LerpToPos(position, time));
            }
        }
        
        private void Awake()
        {
            _camera = Camera.main;
            _matchController = FindAnyObjectByType<MatchController>();
            
            if (SceneManager.GetActiveScene().buildIndex != 1)
                _matchController = null;
            
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            Model = transform.Find("Model");
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
            if (distanceFromCamera > 40f || distanceFromCamera < 3f)
            {
                foreach (var renderer in _renderers)
                {
                    if (renderer) 
                        renderer.enabled = false;
                }

                if (_animator)
                    _animator.enabled = false;
            }
            else
            {
                foreach (var renderer in _renderers)
                {
                    if (renderer)
                        renderer.enabled = true;
                }

                if (_animator)
                    _animator.enabled = true;
            }

            _renderers.RemoveAll(x => x == null);
        }

        public void RecaptureRenderers()
        {
            _renderers.Clear();
        }
    }