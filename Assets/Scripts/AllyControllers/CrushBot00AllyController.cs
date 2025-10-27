using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using External;
using MyBox;
using NUnit.Framework;
using ScriptableObj;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class CrushBot00AllyController : AllyController
{
    private EnemyController _targetEnemy = null;
    private float _timeSameTarget = 0;
    
    protected override void Start()
    {
        base.Start();
        
        _legConstraints.Add(new LegConstraint(transform.RecursiveFind("LegLBackConstraint").gameObject, transform));
        _legConstraints.Add(new LegConstraint(transform.RecursiveFind("LegRBackConstraint").gameObject, transform));
        _legConstraints.Add(new LegConstraint(transform.RecursiveFind("LegLFrontConstraint").gameObject, transform));
        _legConstraints.Add(new LegConstraint(transform.RecursiveFind("LegRFrontConstraint").gameObject, transform));

        StartCoroutine(LegUpdateCoroutine());
    }

    protected override void Update()
    {
        base.Update();
        
        ProceduralLegs();
        
        if (!_isAlive)
            return;

        if (_targetEnemy == null)
        {
            _timeSameTarget = 0;
            State = AllyState.Idle;
            _IsAttacking = false;
            var enemies = Physics.OverlapSphere(transform.position, GetDetectionRadius(), LayerMask.GetMask("Enemy"));

            foreach (var enemy in enemies)
            {
                var enemyCtrl = enemy.GetComponent<EnemyController>();
                if (enemyCtrl)
                {
                    _targetEnemy = enemyCtrl;
                    State = AllyState.Attacking;
                    _timeSameTarget = 0;
                    break;
                }
            }
        }

        var targetPosition = _player.transform.position;
        if (_targetEnemy)
        {
            targetPosition = _targetEnemy.transform.position;
            _timeSameTarget += Time.deltaTime;
        }
        
        var distFromTarget = Vector3.Distance(transform.position, targetPosition);
        var distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distFromTarget > 2.5f)
        {
            _agent.SetDestination(targetPosition);
        } else if (distFromTarget <= 1.0f)
        {
            _agent.isStopped = false;
        }
        
        if (!_IsAttacking) {
            var eul = transform.eulerAngles;
            transform.LookAt(targetPosition.SetY(transform.position.y));
            
            transform.eulerAngles = Vector3.Slerp(eul, transform.eulerAngles, Time.deltaTime * 5f);
        }

        if (_IsAttacking)
        {
            _agent.isStopped = false;
        }
        if (State == AllyState.Attacking && !_IsAttacking)
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
                if (!_IsAttacking)
                {
                    if (_timeSameTarget > 3)
                    {
                        if (GetAbility(0).GetCooldownTimer() <= 0.01f)
                        {
                            StartCoroutine(MeleeAttack());
                        }
                        else if (GetAbility(1).GetCooldownTimer() <= 0.01f)
                        {
                            StartCoroutine(JumpAttack());
                        }
                    }
                }
            }
        }
    }

    private bool _IsAttacking = false;
    IEnumerator MeleeAttack()
    {
        if (GetAbility(0).GetCooldownTimer() <= 0.01f)
        {
            _agent.radius = 0.1f;
            _IsAttacking = true;

            var dir = transform.position - _targetEnemy.transform.position;
            dir = dir.normalized;
            
            float timeElapsed = 0;
            float moveDuration = 2.0f;
            
            do
            {
                if (!_targetEnemy)
                {
                    _IsAttacking = false;
                    _agent.radius = 0.75f;
                    yield break;
                }
                
                dir = transform.position - _targetEnemy.transform.position;
                dir = dir.normalized;
                
                timeElapsed += Time.deltaTime;
                var pos = transform.position + (dir * (Time.deltaTime * 5));
                if (NavMesh.SamplePosition(pos, out NavMeshHit hit, GetDetectionRadius(), NavMesh.AllAreas))
                {
                    pos = hit.position;
                }

                transform.position = pos;
                
                var eul = transform.eulerAngles;
                transform.LookAt(_targetEnemy.transform.position.SetY(transform.position.y));
            
                transform.eulerAngles = Vector3.Slerp(eul, transform.eulerAngles, Time.deltaTime * 5f);

                yield return null;
            } while (timeElapsed < moveDuration);
            
            _animator.SetTrigger("MeleeAttack");


            yield return new WaitForSeconds(0.125f);
            
            if (!_targetEnemy)
            {
                _IsAttacking = false;
                _agent.radius = 0.75f;
                yield break;
            }

            var startPosition = transform.position;
            var endPosition = _targetEnemy.transform.position;
            
            timeElapsed = 0;
            moveDuration = 0.25f;

            
            do
            {
                if (!_targetEnemy)
                {
                    _IsAttacking = false;
                    _agent.radius = 0.75f;
                    yield break;
                }
                
                // Add time since last frame to the time elapsed
                timeElapsed += Time.deltaTime;

                float normalizedTime = timeElapsed / moveDuration;
                //normalizedTime = Easing.EaseInOutQuint(normalizedTime);

                // Interpolate position and rotation
                transform.position = Vector3.Lerp(startPosition, endPosition,normalizedTime);

                // Wait for one frame
                yield return null;
            } while (timeElapsed < moveDuration);
            
            TryAbility(0,_targetEnemy.transform.position);
            
            _IsAttacking = false;
            _agent.radius = 0.75f;

        }
    }

    IEnumerator JumpAttack()
    {
        if (GetAbility(1).GetCooldownTimer() <= 0.01f)
        {
            _agent.radius = 0.1f;
            _IsAttacking = true;

            var dir = transform.position - _targetEnemy.transform.position;
            dir = dir.normalized;
            
            _animator.SetTrigger("JumpAttack");

            var startPosition = transform.position;
            var endPosition = _targetEnemy.transform.position;
            
            float timeElapsed = 0;
            float moveDuration = 0.75f;

            
            do
            {
                if (!_targetEnemy)
                {
                    _IsAttacking = false;
                    _agent.radius = 0.75f;
                    yield break;
                }
                // Add time since last frame to the time elapsed
                timeElapsed += Time.deltaTime;

                float normalizedTime = timeElapsed / moveDuration;
                //normalizedTime = Easing.EaseInOutQuint(normalizedTime);

                // Interpolate position and rotation
                transform.position = Vector3.Lerp(startPosition, endPosition,normalizedTime);

                // Wait for one frame
                yield return null;
            } while (timeElapsed < moveDuration);
            
            TryAbility(1,_targetEnemy.transform.position);
            
            _IsAttacking = false;
            _agent.radius = 0.75f;

        }
    }

    [Serializable]
    private class LegConstraint
    {
        private Vector3 originalOffset;

        public float YSnap;
        public Vector3 currentOffset;
        
        public Vector3 currentPosition;
        
        public GameObject constraintObject;
        private CrushBot00AllyController parentTransform;
        public bool Moving = false;

        public LegConstraint(GameObject g, Transform transform)
        {
            constraintObject = g;
            parentTransform = transform.GetComponent<CrushBot00AllyController>();

            originalOffset = g.transform.position - transform.position;
            currentPosition = originalOffset + parentTransform.transform.position;
            currentOffset = originalOffset;
        }

        public Vector3 GetTargetPosition()
        {
            var p = parentTransform.transform.position + (parentTransform.transform.TransformDirection(currentOffset));
            p.y = YSnap;

            return p;
        }


        IEnumerator MoveToTarget()
        {
            Moving = true;

            var startPosition = currentPosition;

            var endPosition = GetTargetPosition();
            var angle = Random.Range(0, 360f) * Mathf.Deg2Rad;

            endPosition += new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle))*0.1f;
            

            float timeElapsed = 0;
            float moveDuration = 1 / 8f;
            
            do
            {
                // Add time since last frame to the time elapsed
                timeElapsed += Time.deltaTime;

                float normalizedTime = timeElapsed / moveDuration;
                //normalizedTime = Easing.EaseInOutQuint(normalizedTime);

                // Interpolate position and rotation
                currentPosition = Vector3.Lerp(startPosition, endPosition,normalizedTime);

                // Wait for one frame
                yield return null;
            } while (timeElapsed < moveDuration);

            Moving = false;
        }
        
        public void TryMove()
        {
            
            currentOffset = originalOffset;
            YSnap = parentTransform.transform.position.y + originalOffset.y;

            if (Physics.Raycast(
                    GetTargetPosition() + (Vector3.up * 10), Vector3.down, out var hit,
                    Mathf.Infinity,LayerMask.GetMask("Map")))
            {
                YSnap = hit.point.y;
            }

            if (!Moving)
            {
                var distance = Vector3.Distance(GetTargetPosition(), currentPosition);
                if (distance > 0.5f * parentTransform.transform.localScale.magnitude)
                {
                    parentTransform.RunCoroutine(MoveToTarget());
                }
            }

            constraintObject.transform.position = currentPosition;
        }
    }
    
    private List<LegConstraint> _legConstraints = new List<LegConstraint>();

    public void RunCoroutine(IEnumerator c)
    {
        StartCoroutine(c);
    }
    
    [SerializeField]
    private float _lastLegUpdateDiffrence;
    
    // Only allow diagonal leg pairs to step together
    IEnumerator LegUpdateCoroutine()
    {
        yield break;
        var frontLeftLegStepper = _legConstraints[2];
        var backLeftLegStepper = _legConstraints[0];

        var frontRightLegStepper = _legConstraints[3];
        var backRightLegStepper = _legConstraints[1];
        
        // Run continuously
        while (true)
        {
            _lastLegUpdateDiffrence = 0;
            // Try moving one diagonal pair of legs
            do
            {
                frontLeftLegStepper.TryMove();
                backRightLegStepper.TryMove();
                // Wait a frame
                yield return null;
                _lastLegUpdateDiffrence = 0;
      
                // Stay in this loop while either leg is moving.
                // If only one leg in the pair is moving, the calls to TryMove() will let
                // the other leg move if it wants to.
            } while (backRightLegStepper.Moving || frontLeftLegStepper.Moving);

            // Do the same thing for the other diagonal pair
            do
            {
                frontRightLegStepper.TryMove();
                backLeftLegStepper.TryMove();
                yield return null;
                _lastLegUpdateDiffrence = 0;
            } while (backLeftLegStepper.Moving || frontRightLegStepper.Moving);
        }
    }
    
    private void ProceduralLegs()
    {
        // if (_lastLegUpdateDiffrence > 1.0f )
        // {
        //     _lastLegUpdateDiffrence = 0;
        //     StartCoroutine(LegUpdateCoroutine());
        // }

        foreach (var legConstraint in _legConstraints)
        {
            legConstraint.TryMove();
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        _lastLegUpdateDiffrence += Time.deltaTime;
    }


    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void OnPlayerShot(int selectedAbility, Vector3 shootPoint)
    {
        base.OnPlayerShot(selectedAbility, shootPoint);
    }
    
    // getting stats
    public float GetDetectionRadius()
    {
        var rad = 5.0f;

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
    
    // debug
    private void OnDrawGizmos()
    {
        foreach (var legConstraint in _legConstraints)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(legConstraint.currentPosition, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(legConstraint.GetTargetPosition(), 0.1f);
            Gizmos.DrawLine(legConstraint.currentPosition, legConstraint.GetTargetPosition());
        }
    }
}