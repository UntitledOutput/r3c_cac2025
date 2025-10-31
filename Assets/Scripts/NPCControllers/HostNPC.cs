using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DefaultNamespace;
using External;
using UnityEngine;

public class HostNPC : NPCController
{

    IEnumerator MeetingSequence()
    {
        yield return DialogueController.Instance.ShowDialogue("{name}! It's you!", "???");
        yield return DialogueController.Instance.ShowDialogue("It's Bruce, by the way.", "Bruce");
        yield return DialogueController.Instance.ShowDialogue("Welcome to GreenShift! We're a recycling agency, and we use the trash for better tools, clothing, and more.", "Bruce");
        yield return DialogueController.Instance.ShowDialogue("The main thing you'll be doing is waste collection runs!", "Bruce");
        yield return DialogueController.Instance.ShowDialogue("You'll be working in the streets, defeating trash enemies, and collecting the trash they drop.", "Bruce");
        yield return DialogueController.Instance.ShowDialogue("Also, i've come up with an idea.. Supposedly there's a super enemy at the end of every collection run..", "Bruce");
        yield return DialogueController.Instance.ShowDialogue("So i've had our lead scientist develop a way to capture these enemies, and turn them into robots to help you!", "Bruce");
        yield return DialogueController.Instance.ShowDialogue("How about this, you go into a collection run and report back. Good luck!", "Bruce");
        
        _homeController.AlignUISection(0);

        DataController.saveData.Flags.Add("MetHost");
        DataController.saveData.Save();

    }
    
    public override void OnInteract()
    {
        base.OnInteract();

        IEnumerator interact()
        {
            if (DataController.saveData.Flags.Contains("MetHost"))
                _homeController.AlignUISection(0);
            
            _cameraController.FocusOnPlayer = false;
                
            _cameraController.FocusPoint = transform;
            _cameraController.PositionOffset = (transform.forward * (Size*9f)) + (Vector3.up*(Height/1.0f)) ;
            _cameraController.FocusOffset = (Vector3.up*Height/1.0f) + (transform.right * (Size*2f));
            _cameraController.TargetFieldOfView = 45;
            
            _IsMenuOpen = true;
            InteractionCanvas.gameObject.SetActiveFast(false);
            
            if (!DataController.saveData.Flags.Contains("MetHost"))
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
    }
}