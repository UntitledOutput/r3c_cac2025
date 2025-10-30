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
        yield return DialogueController.Instance.ShowDialogue("Oh, hey! You must be the new recruit!", "???");
        yield return DialogueController.Instance.ShowDialogue("I'm Maddie. I'm the recycling specialist here at GreenShift. You give me the trash you collect, and I turn it into gold! \\sWell.. not actual gold..\\", "Maddie");
        yield return DialogueController.Instance.ShowDialogue("Here. let me show you!", "Maddie");
        
        _homeController.AlignUISection(2);

        yield return DialogueController.Instance.ShowDialogue("Here is the recycling menu! You can recycle anything you collect on your recycle runs, from trash, to enemies you capture.", "Maddie");
        yield return DialogueController.Instance.ShowDialogue("Don't you think thats a little harmful, Maddie?", "Gwin");
        yield return DialogueController.Instance.ShowDialogue("Gwin.. We've been over this time and time again.. The enemies are like jellyfish. They don't even know they exist.", "Maddie");
        yield return DialogueController.Instance.ShowDialogue("Okay..", "Gwin");
        
        if (DataController.saveData.Flags.Contains("MetFashi")) 
            yield return DialogueController.Instance.ShowDialogue("I'm assuming you've already met Gwin.. Our fashion specialist.", "Maddie");
        else
            yield return DialogueController.Instance.ShowDialogue("Gwin over there, our fashion specialist is a little crazy sometimes.", "Maddie");
        
        // add ui highlights
        
        yield return DialogueController.Instance.ShowDialogue("Anyways, that was a brief overview of what I do here! Come back at the end of every run to see what I could do for you!", "Maddie");

        DataController.saveData.Flags.Add("MetScien");
    }

    
    public override void OnInteract()
    {
        base.OnInteract();
        
        
        IEnumerator interact()
        {
            if (DataController.saveData.Flags.Contains("MetScien"))
                _homeController.AlignUISection(2);
            
            _cameraController.FocusOnPlayer = false;
                
            _cameraController.FocusPoint = transform;
            _cameraController.PositionOffset = (transform.forward * (Size*9f)) + (Vector3.up*(Height/2.0f)) ;
            _cameraController.FocusOffset = (Vector3.up*Height/2.0f) + (transform.right * (Size*2f));
            _cameraController.TargetFieldOfView = 45;
            
            _IsMenuOpen = true;
            InteractionCanvas.gameObject.SetActiveFast(false);
            
            if (!DataController.saveData.Flags.Contains("MetScien"))
                yield return MeetingSequence();

        }

        StartCoroutine(interact());
        
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