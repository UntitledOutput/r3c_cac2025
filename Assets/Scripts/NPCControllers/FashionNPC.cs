using External;
using UnityEngine;

public class FashionNPC : NPCController
{
    private GameObject FashionRoom;
    private Vector3 _pos = Vector3.zero;
    
    public override void OnInteract()
    {
        base.OnInteract();
        _homeController.AlignUISection(1);

        if (!FashionRoom)
        {
            FashionRoom = GameObject.Find("FashionRoom");
        }

        var playerPoint = FashionRoom.transform.Find("PlayerSpawn");
        
        _cameraController.FocusOnPlayer = false;
            
        _cameraController.FocusPoint = playerPoint;
        _cameraController.PositionOffset = new Vector3(0, 1.5f, 6.5f);
        _cameraController.FocusOffset = new Vector3(1, _cameraController.PositionOffset.y, 1.0f);
        _cameraController.TargetFieldOfView = 45;

        _pos = FindAnyObjectByType<PlayerController>().transform.position;
        FindAnyObjectByType<PlayerController>().transform.position = playerPoint.position;
        FindAnyObjectByType<PlayerController>().transform.forward = -FashionRoom.transform.forward;

        _IsMenuOpen = true;
        InteractionCanvas.gameObject.SetActiveFast(false);
    }

    public override void CloseMenu()
    {
        base.CloseMenu();
        _homeController.AlignUISection(-1);
        _IsMenuOpen = false;

        _cameraController.FocusOnPlayer = true;
        _cameraController.TargetFieldOfView = 60;


        FindAnyObjectByType<PlayerController>().transform.position = _pos;
    }
}