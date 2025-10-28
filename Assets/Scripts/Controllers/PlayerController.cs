using System;
using System.Collections.Generic;
using Controllers;
using External;
using MyBox;
using ScriptableObj;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class PlayerController : ActorBehavior
{
    private static readonly int Velocity1 = Animator.StringToHash("Velocity");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private Camera camera;
    public Rigidbody rigidbody;
    private AbilityController _abilityController;
    private float _timeSinceAttack = 0;
    private List<AllyController> _allies = new List<AllyController>();
    private Animator _animator;
    
    private Vector3 _shootPoint;

    public float MoveSpeed = 2.5f;
    public bool BlockMovement = false;

    public float Health => _health;
    private bool _IsDead = false;
    
    public  AbilityController AbilityController
    {
        get { return _abilityController; }
    }

    public List<AllyController> Allies
    {
        get => _allies;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        Team = ActorTeam.Player;
        
        // loading allies
        if (_matchController )
        {
            foreach (var matchControllerPassedAlly in _matchController.PassedAllies)
            {
                if (matchControllerPassedAlly != null && matchControllerPassedAlly.ally)
                {
                    var allyInstance = Instantiate(
                        matchControllerPassedAlly.ally.Prefab, transform.position + (Vector3.up * 3f),
                        Quaternion.identity);

                    allyInstance.GetComponent<AllyController>().ally = matchControllerPassedAlly;
                    _allies.Add(allyInstance.GetComponent<AllyController>());
                }
            }
        }

        while (_allies.Count < 3)
        {
            _allies.Add(null);
        }
    }

    public void OnDeath()
    {
        _IsDead = true;
        BlockMovement = true;
        foreach (var allyController in _allies)
        {
            if (allyController)
                allyController.OnDeath();
        }
        
        FindAnyObjectByType<GameUIController>().Fail();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_abilityController == null)
            _abilityController = GetComponent<AbilityController>();

        if (_IsDead)
        {
            return;
        }

        if (_health <= 0.01f && !_IsDead)
            OnDeath();

        var attacking = InputSystem.Attack || InputSystem.SubAttack || InputSystem.SideAttack_OneTime;
        if (!_matchController)
            attacking = false;
        
        var moveDirection = CalculateMoveDirection(InputSystem.Move);
        if ((moveDirection.magnitude > 0 && _timeSinceAttack > 1f) || attacking )
        {
            var fwd = moveDirection;
            if ((_abilityController && attacking))
            {
                fwd = _abilityController.GetDirection().SetY(0);
            }

            //fwd = Vector3.Project(fwd, transform.up);

            if (!BlockMovement)
            {
                transform.forward = Vector3.Slerp(transform.forward, fwd, 0.25f);
            }
        }

        var lerpSpeed = Time.deltaTime * 10f;
        _animator.SetBool("Shooting", attacking || _timeSinceAttack <= 1f );
        _animator.LerpFloat(Velocity1,InputSystem.Move.magnitude,lerpSpeed);
        _animator.LerpFloat(MoveX,InputSystem.Move.x,lerpSpeed);
        _animator.LerpFloat(MoveY,InputSystem.Move.y,lerpSpeed);

        moveDirection *= MoveSpeed;

        if (!BlockMovement)
        {
            rigidbody.linearVelocity = new Vector3(moveDirection.x, rigidbody.linearVelocity.y, moveDirection.z);

            if ((InputSystem.Attack_OneTime || InputSystem.SubAttack_OneTime || InputSystem.SideAttack_OneTime) && _abilityController && _matchController)
            {
                if (InputSystem.Attack_OneTime)
                    _abilityController.ChosenAbility = 0;
                if (InputSystem.SubAttack_OneTime)
                    _abilityController.ChosenAbility = 1;
                if (InputSystem.SideAttack_OneTime)
                    _abilityController.ChosenAbility = 2;
                
                _shootPoint = _abilityController.GetShootPoint();
                if (_abilityController.TryShoot(_shootPoint))
                {
                    foreach (var allyController in _allies)
                    {
                        if (allyController)
                            allyController.OnPlayerShot(_abilityController.ChosenAbility,_shootPoint);
                    }
                }
            }
        }

        if (attacking)
        {
            _timeSinceAttack = 0;
        }
        else
        {
            _timeSinceAttack += Time.deltaTime;
        }

        if (transform.position.y <= -25)
        {
            transform.position = transform.position.SetY(2);
            rigidbody.linearVelocity = Vector3.zero;
            
        }
        

        
    }

    public override void ChangeHealth(float diff, ActorBehavior source)
    {
        base.ChangeHealth(diff, source);
        foreach (var allyController in Allies)
        {
            if (allyController)
                allyController.OnPlayerChangeHealth(diff, source);
        }
    }

    private Vector3 CalculateMoveDirection(Vector2 movement)
    {
        var moveDirection = new Vector3(movement.x, 0, movement.y).normalized;

        return camera.transform.TransformDirection(moveDirection).SetY(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_shootPoint,0.5f);
        
        if (rigidbody)
            Gizmos.DrawLine(rigidbody.position, rigidbody.position+(rigidbody.linearVelocity*0.5f));
    }
}
