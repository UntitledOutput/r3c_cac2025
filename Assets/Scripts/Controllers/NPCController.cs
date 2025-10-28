using Controllers;
using External;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem = Utils.InputSystem;

public class NPCController : ActorBehavior
{
    private bool isPlayerInDistance = false;

    public float DetectionRange = 4f;

    public Canvas InteractionCanvas;

    public bool _IsMenuOpen = false;

    protected HomeController _homeController;
    protected CameraController _cameraController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _homeController = FindAnyObjectByType<HomeController>();
        _cameraController = FindAnyObjectByType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerInDistance = false;
        var results = Physics.OverlapSphere(transform.position, DetectionRange, LayerMask.GetMask("Player"));
        foreach (var result in results)
        {
            var player = result.GetComponent<PlayerController>();
            if (player)
            {
                isPlayerInDistance = true;
            }
        }

        if (!_IsMenuOpen)
        {
            InteractionCanvas.gameObject.SetActiveFast(isPlayerInDistance);

            if (InputSystem.Interact && isPlayerInDistance)
            {
                OnInteract();
            }
        }
        else
        {
            if (InputSystem.Close)
            {
                CloseMenu();
            }
        }
    }

    public virtual void OnInteract()
    {


    }

    public virtual void CloseMenu()
    {

    }
    
    
}