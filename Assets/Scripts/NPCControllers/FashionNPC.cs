using External;
using UnityEngine;

public class FashionNPC : NPCController
{
    public override void OnInteract()
    {
        base.OnInteract();
        _homeController.AlignUISection(1);
        
        _cameraController.FocusOnPlayer = false;
            
        _cameraController.FocusPoint = transform;
        _cameraController.PositionOffset = (transform.forward * (Size*9f)) + (Vector3.up*(Height/2.5f)) ;
        _cameraController.FocusOffset = (Vector3.up*Height/4.5f) + (transform.right * (Size*2f));

        _IsMenuOpen = true;
        InteractionCanvas.gameObject.SetActiveFast(false);
    }

    public override void CloseMenu()
    {
        base.CloseMenu();
        _homeController.AlignUISection(-1);
        _IsMenuOpen = false;

        _cameraController.FocusOnPlayer = true;
    }
}