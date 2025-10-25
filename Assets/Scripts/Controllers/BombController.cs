using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using ScriptableObj;
using UnityEngine;

namespace Controllers
{
    public class BombController : MonoBehaviour
    {
        public float Lifetime;
        public float Damage;
        public float Speed;
        public float BlastRadius;
        public ActorBehavior.ActorTeam Target;
        
        public GameObject ObjectToSpawnOnDestroy;
        private List<ParticleSystem> _particleSystems;
         
        public void Derive(AbilityObject a, AbilityObject.AbilityUpgrade upgrade, EnemyController enemy)
        {
            Lifetime = a.Lifetime + (upgrade?.LifetimeChange ?? 0);
            Damage = (a.Damage + (upgrade?.DamageChange ?? 0)) * (enemy  && enemy.IsMegaEnemy ? enemy.enemyObject.MegaDamageMultiplier : 1);
            Speed = a.Speed + (upgrade?.SpeedChange ?? 0);
            BlastRadius = a.BlastRadius + (upgrade?.BlastChange ?? 0);
            yMax = a.LaunchY + (upgrade?.LaunchYChange ?? 0);
            Target = a.Target;
        } 
        
        public bool CanBounce = true;
        public Vector3 LaunchPosition;
        
        private float _age;
        private Rigidbody _rigidbody;
        
        // ballistics
        public float yMax;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            var g = 9.8f * Speed;
            var y_0 = transform.position.y - LaunchPosition.y;

            Vector3 displacementXZ = LaunchPosition - transform.position;
            displacementXZ.SetY(0);

            // Implement equations derived from kinematic analysis
            Vector3 velocityY = Vector3.up*Mathf.Sqrt(2*g*(yMax - y_0));
            Vector3 velocityXZ = displacementXZ/(Mathf.Sqrt(2*(yMax - y_0)/g) + Mathf.Sqrt(2*yMax/g));
        
            Vector3 velocity = velocityXZ + velocityY;
            
            _rigidbody.linearVelocity = velocity;
            
            if (ObjectToSpawnOnDestroy)
                ObjectToSpawnOnDestroy.SetActive(false);

            _particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
        }

        private void Update()
        {
            _age += Time.deltaTime;
            
            if (_age > Lifetime)
                OnExplode();

        }

        public void OnExplode()
        {
            foreach (var collider in Physics.OverlapSphere(transform.position, BlastRadius))
            {
                var enemy = collider.GetComponent<ActorBehavior>();
                
                if (enemy && enemy.Team == Target)
                    enemy.ChangeHealth(-Damage);
            }
            
            
            if (ObjectToSpawnOnDestroy)
            {
                ObjectToSpawnOnDestroy.SetActive(true);
                ObjectToSpawnOnDestroy.transform.SetParent(null);
                ObjectToSpawnOnDestroy.transform.localScale = Vector3.one;
            }
            foreach (var system in _particleSystems)
            {
                system.transform.SetParent(null);
            }
            
            Destroy(gameObject);
        }
        
        private void OnCollisionEnter(Collision other)
        {
                
            OnExplode();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position,BlastRadius);
            Gizmos.DrawWireSphere(LaunchPosition,1.0f);
        }
    }
}