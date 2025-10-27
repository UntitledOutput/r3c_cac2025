using Controllers;
using MyBox;
using ScriptableObj;
using UnityEngine;


public class ShieldBot00AllyController : AllyController
{
    private EnemyController _targetEnemy = null;
    private float stoppingDistance;
    private Collider[] _cachedEnemyColliders;
    
    protected override void Start()
    {
        base.Start();
        stoppingDistance = _agent.stoppingDistance;
        _cachedEnemyColliders = new Collider[10];
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (!_isAlive)
            return;

        _agent.stoppingDistance = stoppingDistance;
        var targetPosition = _player.transform.position;
        if (_targetEnemy)
        {
            targetPosition = _player.transform.position +
                             ((_targetEnemy.transform.position - _player.transform.position).normalized * (Vector3.Distance(transform.position, _targetEnemy.transform.position)/3f));

            _agent.stoppingDistance = 0;
        }
        
        var distFromTarget = Vector3.Distance(transform.position, targetPosition);
        var distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
        _agent.isStopped = false;
        _agent.SetDestination(targetPosition);

        if (_targetEnemy)
        {
            
            targetPosition = _targetEnemy.transform.position;
        }
        else
        {
            if (distanceFromPlayer < 6.5f)
            {
                _agent.SetDestination(_player.transform.position - (_player.transform.forward*6.5f));
            }
        }
        
        {
            var eul = transform.eulerAngles;
            transform.LookAt(targetPosition.SetY(transform.position.y));

            var q = Quaternion.Euler(eul);
            
            transform.rotation = Quaternion.Slerp(q, transform.rotation, Time.deltaTime * 5f);
        }

        if (State == AllyState.Attacking)
        {
            if (distanceFromPlayer > GetMaxDistanceFromPlayer() || distFromTarget > GetMaxDistanceFromPlayer())
            {
                _targetEnemy = null;
            }

            if (_targetEnemy == null || !_targetEnemy.IsAlive || _targetEnemy.target != _player)
            {
                State = AllyState.Idle;
            }
            
            if (_targetEnemy)
            {
                if (distFromTarget < _agent.stoppingDistance+.5f)
                {
                    if (TryAbility(0, _targetEnemy.transform.position))
                    {
                        _animator.SetTrigger("Attack");
                    }

                    if (Vector3.Distance(transform.position, _targetEnemy.transform.position) < 5f)
                    {
                        if (TryAbility(1, _targetEnemy.transform.position))
                        {
                            _animator.SetTrigger("Attack");
                        }
                    }
                }
                else
                {
                    State = AllyState.Attacking;
                }

                int count = Physics.OverlapSphereNonAlloc(
                    transform.position, 2f, _cachedEnemyColliders, LayerMask.GetMask("Enemy"));

                if (count >= 3)
                {
                    //Debug.Log(count);
                    if (TryAbility(2, _targetEnemy.transform.position))
                    {
                        _animator.SetTrigger("Attack");
                    }
                }
            }
        }
    }

    public void PushEnemy(Vector3 point)
    {
        var enemiesAtPoint = Physics.OverlapSphere(point, 0.1f, LayerMask.GetMask("Enemy"));

        foreach (var collider1 in enemiesAtPoint)
        {
            var enemy = collider1.GetComponent<EnemyController>();

            if (enemy && enemy.IsAlive)
            {
                enemy.LerpToPosition((-enemy.transform.forward*6f) + enemy.transform.position, 0.25f);
                enemy.ChangeHealth(-0.125f,this);
            }

            break;
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

    }


    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void OnPlayerShot(int selectedAbility, Vector3 shootPoint)
    {
        base.OnPlayerShot(selectedAbility, shootPoint);

        var ability = _player.AbilityController.Abilities[selectedAbility];

  
    }

    public override void OnPlayerChangeHealth(float diff, ActorBehavior source)
    {
        base.OnPlayerChangeHealth(diff, source);

        if (diff < 0 && source)
        {
            var enemy = source.GetComponent<EnemyController>();
                
            if (enemy && enemy.IsAlive)
            {
                State = AllyState.Attacking;
                _targetEnemy = enemy;

            }
        }
    }

    public override void ChangeHealth(float diff, ActorBehavior source)
    {
        base.ChangeHealth(diff, source);
        OnPlayerChangeHealth(diff, source);
    }

    // getting stats
    public float GetDetectionRadius()
    {
        var rad = 2.0f;

        return rad;
    }

    public float GetMaxDistanceFromPlayer()
    {
        var dist = 15.0f;

        return dist;
    }

    public float GetSpeed()
    {
        var spd = 3.5f;

        return spd;
    }
}