using System;
using Controllers;
using MyBox;
using ScriptableObj;
using UnityEngine;
using Utils;

public class PlayerController : ActorBehavior
{
    private Camera camera;
    public Rigidbody rigidbody;
    private AbilityController _abilityController;
    private float _timeSinceAttack = 0;
    
    
    private bool isGrounded;
    private float timeSinceGrounded = 0f;
    private Vector3 _shootPoint;

    public float MoveSpeed = 2.5f;
    public bool BlockMovement = false;

    public float Health => _health;
    private bool _IsDead = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        Team = ActorTeam.Player;
    }

    public void OnDeath()
    {
        _IsDead = true;
        BlockMovement = true;
        FindAnyObjectByType<GameUIController>().Fail();
    }

    // Update is called once per frame
    void Update()
    {
        if (_abilityController == null)
            _abilityController = GetComponent<AbilityController>();

        if (_IsDead)
            return;
        if (_health <= 0.01f && !_IsDead)
            OnDeath();

        var attacking = InputSystem.Attack || InputSystem.SubAttack || InputSystem.SideAttack_OneTime;
        
        var jump = false;
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

        moveDirection *= MoveSpeed;
        
        if (jump && isGrounded)
        {
            rigidbody.AddForce(transform.up * 2.5f, ForceMode.Impulse);
        }
        else if (!isGrounded)
        {
            timeSinceGrounded += Time.fixedDeltaTime;
        }

        if (!BlockMovement)
        {
            rigidbody.linearVelocity = new Vector3(moveDirection.x, rigidbody.linearVelocity.y, moveDirection.z);

            if ((InputSystem.Attack_OneTime || InputSystem.SubAttack_OneTime || InputSystem.SideAttack_OneTime) && _abilityController)
            {
                if (InputSystem.Attack_OneTime)
                    _abilityController.ChosenAbility = 0;
                if (InputSystem.SubAttack_OneTime)
                    _abilityController.ChosenAbility = 1;
                if (InputSystem.SideAttack_OneTime)
                    _abilityController.ChosenAbility = 2;
                
                _shootPoint = _abilityController.GetShootPoint();
                _abilityController.TryShoot(_shootPoint);
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
    
    private Vector3 CalculateMoveDirection(Vector2 movement)
    {
        var forward = camera.transform.forward.SetY(0).normalized;
        var right = camera.transform.right.SetY(0).normalized;
        var moveDirection = new Vector3(movement.x, 0, movement.y).normalized;

        return camera.transform.TransformDirection(moveDirection).SetY(0);
    }
    
    /*private void OnCollisionEnter(Collision other)
    {
        currentSurface = other.gameObject.GetComponent<SurfaceController>();
        if (currentSurface)
        {
            isGrounded = true;
            StartCoroutine(HandleGrounding(timeSinceGrounded, lastSurfacePosition));
            timeSinceGrounded = 0f;
            lastSurfacePosition = other.GetContact(0).point;
            surfaceNormal = other.GetContact(0).normal.normalized;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        currentSurface = other.gameObject.GetComponent<SurfaceController>();
        if (!isGrounded)
        {
            if (currentSurface)
            {
                isGrounded = true;
                StartCoroutine(HandleGrounding(timeSinceGrounded, lastSurfacePosition));
                timeSinceGrounded = 0f;
            }
        }
        else
        {
            surfaceNormal = other.GetContact(0).normal.normalized;
        }

        if (currentSurface)
        {
            lastSurfacePosition = other.GetContact(0).point;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (currentSurface && currentSurface.gameObject == other.gameObject)
        {
            currentSurface = null;
            isGrounded = false;
            //surfaceNormal = Vector3.up;
        }
    }*/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_shootPoint,0.5f);
        
        if (rigidbody)
            Gizmos.DrawLine(rigidbody.position, rigidbody.position+(rigidbody.linearVelocity*0.5f));
    }
}
