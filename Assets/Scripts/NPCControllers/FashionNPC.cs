using System.Collections;
using DefaultNamespace;
using External;
using UnityEngine;

public class FashionNPC : NPCController
{
    private GameObject FashionRoom;
    private Vector3 _pos = Vector3.zero;
    
    IEnumerator MeetingSequence()
    {
        yield return DialogueController.Instance.ShowDialogue("Uh.. hello..?", "???");
        yield return DialogueController.Instance.ShowDialogue("Oh wait, new recruit! I'm Gwin.", "Gwin");
        yield return DialogueController.Instance.ShowDialogue("Maddie won't let me design on her.. But this is great for me!", "Gwin");
        yield return DialogueController.Instance.ShowDialogue("Anyways.. I'll need some clothing scraps to design.", "Gwin");

        _homeController.AlignUISection(1);

        // add ui highlights

        yield return DialogueController.Instance.ShowDialogue("I'll design hats, shirts, pants, and shoes for you! Just bring back some clothing scraps.", "Gwin");
        

        DataController.saveData.Flags.Add("MetFashi");
    }

    
    public override void OnInteract()
    {
        base.OnInteract();
        
        
        IEnumerator interact()
        {
            if (DataController.saveData.Flags.Contains("MetFashi"))
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
            
            if (!DataController.saveData.Flags.Contains("MetFashi"))
                yield return MeetingSequence();

        }

        StartCoroutine(interact());

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