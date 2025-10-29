using External;
using UnityEngine;

public class ScientistNPC : NPCController
{
    public override void OnInteract()
    {
        base.OnInteract();
        _homeController.AlignUISection(2);
        
        _cameraController.FocusOnPlayer = false;
            
        _cameraController.FocusPoint = transform;
        _cameraController.PositionOffset = (transform.forward * (Size*9f)) + (Vector3.up*(Height/2.0f)) ;
        _cameraController.FocusOffset = (Vector3.up*Height/2.0f) + (transform.right * (Size*2f));
        _cameraController.TargetFieldOfView = 45;

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
    }
}