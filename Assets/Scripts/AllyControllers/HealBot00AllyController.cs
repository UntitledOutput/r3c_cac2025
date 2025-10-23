using Controllers;
using MyBox;
using ScriptableObj;
using UnityEngine;


public class HealBot00AllyController : AllyController
{
    private EnemyController _targetEnemy = null;

    private static bool _isLeftTaken = false;
    private static bool _isRightTaken = false;
    private static bool _isBackTaken = false;
    
    protected override void Start()
    {
        base.Start();
    }
    
    protected override void Update()
    {
        base.Update();
        var minDistanceFromRoot = 0f;
        var distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_targetEnemy == null && State == AllyState.Attacking)
        {
            State = AllyState.Idle;
        }
        
        
        var root = Vector3.zero;
        var lookRoot = Vector3.zero;

        if (State == AllyState.Idle)
        {
            root = _player.transform.position + (Vector3.up * _player.Height);
            minDistanceFromRoot = _player.Size + 1.0f;
            lookRoot = _player.transform.position + (Vector3.up * (_player.Height/2));
        }
        else
        {
            root = _targetEnemy.transform.position + (Vector3.up * _targetEnemy.Height);
            minDistanceFromRoot = _targetEnemy.Size + 2.0f;
            lookRoot = _targetEnemy.transform.position + (Vector3.up * (_targetEnemy.Height/2));
        }

        var playerRight = root + (Vector3.right*minDistanceFromRoot);
        var playerLeft = root + (-Vector3.right*minDistanceFromRoot);
        var playerBack = root + (-Vector3.forward*minDistanceFromRoot);

        Vector3 nearestPlayerPos = Vector3.positiveInfinity;
        {
            var dist = Vector3.Distance(transform.position, playerLeft);
            if (dist < Vector3.Distance(transform.position, nearestPlayerPos) && !_isLeftTaken)
                nearestPlayerPos = playerLeft;
        }
        {
            var dist = Vector3.Distance(transform.position, playerRight);
            if (dist < Vector3.Distance(transform.position, nearestPlayerPos) && !_isRightTaken)
                nearestPlayerPos = playerRight;
        }
        {
            var dist = Vector3.Distance(transform.position, playerBack);
            if (dist < Vector3.Distance(transform.position, nearestPlayerPos) && !_isBackTaken)
                nearestPlayerPos = playerBack;
        }

        if (nearestPlayerPos == playerLeft)
            _isLeftTaken = true;
        else if (nearestPlayerPos == playerBack)
            _isBackTaken = true;
        else if (nearestPlayerPos == playerRight)
            _isRightTaken = true;
        

        var dir = (nearestPlayerPos - transform.position).normalized;

        if (dir.magnitude > 0.5f && Vector3.Distance(transform.position, nearestPlayerPos) > 0.1f)
        {
            transform.position += dir*(Time.deltaTime*GetSpeed());
        }
        


        {
            var eul = transform.eulerAngles;
            transform.LookAt(lookRoot);
            
            transform.eulerAngles = Vector3.Slerp(eul, transform.eulerAngles, Time.deltaTime * 5f);
        }

        if (State == AllyState.Attacking)
        {
            if (distanceFromPlayer > GetMaxDistanceFromPlayer())
            {
                _targetEnemy = null;
            }

            if (_targetEnemy == null || !_targetEnemy.IsAlive)
            {
                State = AllyState.Idle;
            }
            else
            {
                TryAbility(0,_targetEnemy.transform.position);
            }
        } else if (State == AllyState.Idle)
        {
            if (distanceFromPlayer <= (minDistanceFromRoot*2.0f))
            {
                if (_player.Health < 1)
                {
                    TryAbility(1,_player.transform.position);
                }
            }
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        _isLeftTaken = false;
        _isBackTaken = false;
        _isRightTaken = false;
    }


    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void OnPlayerShot(int selectedAbility, Vector3 shootPoint)
    {
        base.OnPlayerShot(selectedAbility, shootPoint);

        var ability = _player.AbilityController.Abilities[selectedAbility];

        if (State == AllyState.Idle)
        {
            // when shooting at another enemy, it will go and target that enemy
            if (ability.data.Type == AbilityObject.AbilityType.Shooter)
            {
                foreach (var collider in Physics.OverlapSphere(shootPoint, GetDetectionRadius()))
                {
                    var enemy = collider.GetComponent<EnemyController>();
                    var player = collider.GetComponent<PlayerController>();
                
                    if (enemy && enemy.IsAlive)
                    {
                        State = AllyState.Attacking;
                        _targetEnemy = enemy;
                    }
                }
            }
        }
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