using System;
using ScriptableObj;
using UnityEngine;

namespace Controllers
{
    public class BulletController : MonoBehaviour
    {
        public float Speed;
        public float Lifetime;
        public float Damage;

        public float LifetimeLossOnHit;
        
        public bool CanBounce = true;

        public ActorBehavior.ActorTeam Target;
        
        private float _age;

        public GameObject ObjectToSpawnOnDestroy;

        public void Derive(AbilityObject a, AbilityObject.AbilityUpgrade upgrade, EnemyController enemy)
        {
            Speed = a.Speed + (upgrade?.SpeedChange ?? 0);
            Lifetime = a.Lifetime + (upgrade?.LifetimeChange ?? 0);
            Damage = (a.Damage + (upgrade?.DamageChange ?? 0)) * (enemy  && enemy.IsMegaEnemy ? enemy.enemyObject.MegaDamageMultiplier : 1);;
            LifetimeLossOnHit = a.LifetimeLossOnHit + (upgrade?.LifetimeChange ?? 0); 
            Target = a.Target;
        }

        private void Start()
        {
            if (ObjectToSpawnOnDestroy)
                ObjectToSpawnOnDestroy.SetActive(false);
        }

        private void Update()
        {
            _age += Time.deltaTime;

            if (_age > Lifetime)
            {
                if (ObjectToSpawnOnDestroy)
                {
                    ObjectToSpawnOnDestroy.SetActive(true);
                    ObjectToSpawnOnDestroy.transform.SetParent(null);
                    ObjectToSpawnOnDestroy.transform.localScale = Vector3.one;
                }

                Destroy(gameObject);
            }

            transform.position += transform.forward * (Time.deltaTime * Speed);
        }
        private void OnCollisionEnter(Collision other)
        {
            if (CanBounce)
            {
                Vector3 incomingVelocity = transform.forward; // Get the object's current velocity
                Vector3 surfaceNormal = other.contacts[0].normal; // Get the normal from the collision
                Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity.normalized, surfaceNormal); // Calculate the new direction

                transform.forward = reflectedVelocity;
            }

            if (other.gameObject.layer == (Target == ActorBehavior.ActorTeam.Enemy ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player")))
            {
                var actor = other.gameObject.GetComponent<ActorBehavior>();
                if (actor)
                    actor.ChangeHealth(-Damage);
            }

            _age += (Lifetime * LifetimeLossOnHit);
        }
    }
}