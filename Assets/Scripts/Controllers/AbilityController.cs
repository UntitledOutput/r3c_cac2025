using System;
using System.Collections.Generic;
using DefaultNamespace;
using MyBox;
using ScriptableObj;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    public class AbilityController : MonoBehaviour
    {
        private Camera _camera;
        private ParticleSystem _particleSystem;
        private ActorBehavior _actor;

        public ActorBehavior _targetActor;
        private EnemyController _baseEnemy;
        
        [Serializable]
        public class AbilityInstance
        {
            public AbilityObject data;
            public AbilityObject.AbilityUpgrade upgrade;
            public int ammo;
            public float reload;
        }

        public List<AbilityInstance> Abilities = new List<AbilityInstance>();

        public int ChosenAbility = -1;
        private AbilityInstance _ability
        {
            get
            {
                if (ChosenAbility < 0)
                    return null;

                return Abilities[ChosenAbility];

            }
        }

        public void SetupAbilities(List<AbilityObject> abilityObjects, List<AbilityObject.AbilityUpgrade> upgrades)
        {
            Abilities.Clear();
            for (var i = 0; i < abilityObjects.Count; i++)
            {
                var abilityObject = abilityObjects[i];
                
                var ability = new AbilityInstance();
                if (abilityObject)
                {
                    ability.data = abilityObject;
                    if (upgrades != null)
                    {
                        ability.upgrade = upgrades[i];
                        ability.ammo +=  (ability.upgrade?.AmmoChange ?? 0);
                    }

                    ability.ammo += abilityObject.AmmoCount;
                    ability.reload = 1;
                }
                
                Abilities.Add(ability);
            }
        }
        
        private void Start()
        {
            _camera = Camera.main;

            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _actor = GetComponent<ActorBehavior>();

            _baseEnemy = GetComponent<EnemyController>();
        }

        private void Update()
        {

            foreach (var abilityInstance in Abilities)
            {
                if (!abilityInstance.data)
                    continue;
                if ((abilityInstance.ammo <= 0))
                {
                    abilityInstance.reload += Time.deltaTime / (abilityInstance.data.ReloadTime +  (abilityInstance.upgrade?.ReloadChange) ?? 0);
                    if (abilityInstance.reload >= 1)
                    {
                        abilityInstance.ammo += abilityInstance.data.AmmoCount +  (abilityInstance.upgrade?.AmmoChange ?? 0);
                        abilityInstance.reload = 0;
                    }
                }
            }
        }

        public Vector3 GetDirection()
        {
            var pos = GetShootPoint() - transform.position;

            return pos;

            return transform.forward;
        }

        public Vector3 GetShootPoint()
        {
            Vector2 point = Vector2.zero;
            if (Input.mousePresent)
            {
                point = Input.mousePosition;
            }
            else
            {
                point = Input.GetTouch(0).position;
            }
            
            Ray ray = Camera.main.ScreenPointToRay(point);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask:LayerMask.GetMask("Map")))
            {
                return hit.point;
            }
            return Vector3.zero;
        }

        private Collider[] _results = new Collider[10];

        public bool TryShoot(Vector3 shootPoint)
        {
            if (_ability.ammo <= 0)
                return false;
            _ability.ammo -= 1;
            if (_ability.data.Type != AbilityObject.AbilityType.Effect && _ability.data.Type != AbilityObject.AbilityType.Melee && _ability.data.Type != AbilityObject.AbilityType.Trigger)
            {
                if (_ability.data.Type == AbilityObject.AbilityType.Bomb)
                {
                    // check if it fails or not
                    var g = 9.8f * (_ability.data.Speed +  ((_ability.upgrade?.SpeedChange) ?? 0));
                    var yMax = _ability.data.LaunchY +  ((_ability.upgrade?.LaunchYChange ?? 0));
                    var y_0 = transform.position.y - shootPoint.y;

                    Vector3 displacementXZ = shootPoint - transform.position;
                    displacementXZ.SetY(0);

                    float sqrtA = Mathf.Sqrt(2 * (yMax - y_0) / g);
                    float sqrtB = Mathf.Sqrt(2 * yMax / g);
                    float sqrtY = Mathf.Sqrt(2 * g * (yMax - y_0));

                    if (float.IsNaN(sqrtA) || float.IsNaN(sqrtB) || float.IsNaN(sqrtY) || (sqrtA + sqrtB) == 0)
                    {
                        // results in NaN, it'll fail
                        return false;
                    }
                }
                
                var bullet = Instantiate(_ability.data.Prefab);
                bullet.layer = _actor.Team == ActorBehavior.ActorTeam.Enemy ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player");
                bullet.transform.position = transform.position + (transform.up * ( _actor.Height/2));
                if (_ability.data.Type == AbilityObject.AbilityType.Bomb)
                    bullet.transform.position += transform.up;
                else
                {
                    bullet.transform.position += (transform.forward * (_actor.Size * 2));

                    if (_actor.Team == ActorBehavior.ActorTeam.Enemy)
                    {
                        bullet.transform.position = bullet.transform.position.SetY(shootPoint.y + (_targetActor.Height/2));
                    }
                    SoundManager.Instance.PlaySound(_ability.data.Sound);
                }

                bullet.transform.eulerAngles = transform.eulerAngles;
                if (_ability.data.Type == AbilityObject.AbilityType.Projectile)
                    bullet.GetComponent<BulletController>().Derive(_ability.data, _ability.upgrade, _actor);
                else
                {

                    
                    bullet.GetComponent<BombController>().Derive(_ability.data,_ability.upgrade, _actor );
                    bullet.GetComponent<BombController>().LaunchPosition = shootPoint;
                }

            }
            else if (_ability.data.Type == AbilityObject.AbilityType.Effect)
            {
                
                var effect = Instantiate(_ability.data.Prefab);
                effect.transform.position = transform.position;

                effect.GetComponent<AbilityEffectController>().ability = _ability.data;
                effect.GetComponent<AbilityEffectController>().actor = _actor;
                SoundManager.Instance.PlaySound(_ability.data.Sound);
            } else if (_ability.data.Type == AbilityObject.AbilityType.Melee)
            {
                // speed poses as a distance value
                var distance = _ability.data.Distance + ((_ability.upgrade?.DistanceChange) ?? 0);

                
                var size = Physics.OverlapSphereNonAlloc(transform.position, distance, _results, _actor.Team == ActorBehavior.ActorTeam.Enemy
                    ? LayerMask.GetMask("Player")
                    : LayerMask.GetMask("Enemy"));
                
                // Debug.Log(results[0]);
                // Debug.Log(                  _actor.Team == ActorBehavior.ActorTeam.Enemy
                //     ? "Player"
                //     : "Enemy");
                // Debug.Log(distance);
                // Debug.Log(_ability.data.Speed);

                for (int i = 0; i < Mathf.Min(size,_ability.data.PossibleTargetCount + (_ability.upgrade?.SpeedChange ?? 0)); i++)
                {

                    ActorBehavior actor = _results[i].GetComponent<ActorBehavior>();

                    if (actor && actor.Team != _actor.Team)
                    {
                        actor.ChangeHealth(-(_ability.data.Damage + ((_ability.upgrade?.DamageChange) ?? 0)),actor);
                    }
                }
                SoundManager.Instance.PlaySound(_ability.data.Sound);
            } else if (_ability.data.Type == AbilityObject.AbilityType.Trigger)
            {
                _actor.SendMessage(_ability.data.Trigger,shootPoint, SendMessageOptions.RequireReceiver);
            }

            return true;

        }
    }
}