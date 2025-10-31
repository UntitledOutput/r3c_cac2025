
using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using DefaultNamespace;
using DG.Tweening;
using External;
using MyBox;
using ScriptableObj;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour {

    private PlayerController _playerController;
    private CameraController _cameraController;
    private TutorialTarget Target;

    public Transform botTarget;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _cameraController = FindAnyObjectByType<CameraController>();
        
        Target = new GameObject().AddComponent<TutorialTarget>();
        Instantiate(Resources.Load<GameObject>("Prefabs/TutorialTarget"), Target.transform);

    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        LoadingScreenController.LoadingScreen.CloseLoadingScreen();
    }

    public void StartSequence()
    {
        StartCoroutine(TutorialSequence());
    }

    private void Update()
    {
        botTarget.LookAt(_playerController.transform.position.SetY(botTarget.transform.position.y));
    }

    private void LateUpdate()
    {
        _targetHit = false;
    }

    public bool _targetHit = false;
    public TutorialEnemy EnemyPrefab;

    public IEnumerator TutorialSequence()
    {
        Target.HideSprite = true;
        
        _cameraController.FocusOnPlayer = false;

        _cameraController.FocusPoint = botTarget;
        _cameraController.PositionOffset = (botTarget.forward * 3.5f);
        _cameraController.FocusOffset = Vector3.zero;
        _cameraController.TargetFieldOfView = 45;
        _playerController.GetComponentInChildren<ClothingController>().ChangeClothing(DataController.saveData.BuildListOfClothing());
        _playerController.BlockMovement = true;
        FindAnyObjectByType<TriggerPlus>().enabled = false;

        // player select

        // 

        yield return DialogueController.Instance.ShowDialogue(
            "Welcome to GreenShift. We specialize in recycling management, and it seems that you're interested in our services!",
            "???");

        yield return DialogueController.Instance.ShowDialogue(
            "Your application to work here was accepted, though we do have an onboarding process.", "???");

        yield return DialogueController.Instance.ShowDialogue(
            "You applied for the position of Waste Collector, so we will be training you to be the best waste collector in the area.",
            "???");

        // move camera out


        yield return DialogueController.Instance.ShowDialogue(
            "This is the GreenShift Training Facility. You will be learning everything necessary to carry out collection runs.",
            "???");

        // move camera to player

        _cameraController.FocusOnPlayer = true;
        _cameraController.TargetFieldOfView = 75;

        yield return DialogueController.Instance.ShowDialogue(
            "For your first task: use WASD to move towards the target", "???");

        // walk to target
        
        Target.HideSprite = false;

        Target.transform.position = new Vector3(0, 0, 5);
        Target.Limit = typeof(PlayerController);
        Target.Active = true;
        _playerController.BlockMovement = false;

        var task = new TaskController.Task
            { name = "Walk to Target", description = "Walk to the Target, and you get your next task" };

        TaskController.Instance.AddNewTask(task);

        while (Vector3.Distance(_playerController.transform.position, Target.transform.position) > 1f)
        {
            yield return null;
        }

        // completed task

        task.Complete();

        _cameraController.FocusOnPlayer = false;

        _cameraController.FocusPoint = botTarget;
        _cameraController.PositionOffset = (botTarget.forward * 3.5f);
        _cameraController.FocusOffset = Vector3.zero;
        _cameraController.TargetFieldOfView = 45;

        AbilityObject startingAbility =
            Resources.Load<AbilityObject>("Settings/Abilities/Projectiles/PlayerProjectile00");

        yield return DialogueController.Instance.ShowDialogue(
            "Great job. Next task, I'll provide you an ability.", "???");

        yield return DialogueController.Instance.ShowDialogue(
            "Abilities allow you to interact with the environment around you.", "???");

        yield return DialogueController.Instance.ShowDialogue(
            $"Take this, it's called {startingAbility.DisplayName}.", "???");

        Target.Limit = typeof(BulletController);
        _playerController.AbilityController.SetAbility(0, startingAbility, null);

        yield return DialogueController.Instance.ShowDialogue(
            "Hit the targets by tapping/clicking on them, and I'll give you your next task.", "???");

        _cameraController.FocusOnPlayer = true;
        _cameraController.TargetFieldOfView = 75;

        // hit the 3 targets
        task = new TaskController.Task
            { name = "Hit the Three Targets", description = "Hit the three targets and you get your next task." };

        Target.transform.DOMove(Target.transform.position + new Vector3(-3, 1, 5), 1.0f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(1.5f);

        Target.Active = true;
        _targetHit = false;
        while (!_targetHit)
        {
            yield return null;
        }

        Target.Active = true;
        yield return new WaitForSeconds(0.5f);

        Target.transform.DOMove(Target.transform.position + new Vector3(6, 0, 0), 1.0f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(1.5f);

        Target.Active = true;
        _targetHit = false;
        while (!_targetHit)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        Target.transform.DOMove(Target.transform.position + new Vector3(-3, 0, 3), 1.0f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(1.5f);

        Target.Active = true;
        _targetHit = false;
        while (!_targetHit)
        {
            yield return null;
        }

        // completed task

        task.Complete();
        Target.HideSprite = true;

        yield return new WaitForSeconds(0.5f);

        _cameraController.FocusPoint = botTarget;
        _cameraController.PositionOffset = (botTarget.forward * 3.5f);
        _cameraController.FocusOffset = Vector3.zero;
        _cameraController.TargetFieldOfView = 45;

        yield return DialogueController.Instance.ShowDialogue(
            "You're doing good. Now to move on to a more difficult part.", "???");

        yield return DialogueController.Instance.ShowDialogue(
            "I'm going to give you 2 more abilities, and you must defeat the enemy placed in front of you.", "???");

        _playerController.AbilityController.SetAbility(
            1, Resources.Load<AbilityObject>("Settings/Abilities/Bombs/PlayerBomb00"), null);

        _playerController.AbilityController.SetAbility(
            2, Resources.Load<AbilityObject>("Settings/Abilities/Effects/PlayerHeal00"), null);

        var enemy = Instantiate(EnemyPrefab.gameObject).GetComponent<TutorialEnemy>();
        enemy.transform.position = Target.transform.position;
        enemy.transform.forward = -Vector3.forward;

        if (_playerController.IsMobile)
        {
            yield return DialogueController.Instance.ShowDialogue(
                "To switch abilities, tap the ability on the right side of the screen.", "???");
        }
        else
        {
            yield return DialogueController.Instance.ShowDialogue(
                "To switch abilities, press Q for the second ability, and E for the third ability.", "???");

            yield return DialogueController.Instance.ShowDialogue(
                "You can also right click on the mouse for the second ability.", "???");
        }

        _cameraController.FocusOnPlayer = true;
        _cameraController.TargetFieldOfView = 75;

        task = new TaskController.Task
            { name = "Defeat the Enemy", description = "Defeat the enemy, and you get your next task." };

        enemy.IsActive = true;

        while (!_targetHit)
        {
            yield return null;
        }

        // completed task
        task.Complete();

        yield return DialogueController.Instance.ShowDialogue(
            "Great job. You have passed the employee onboarding module.", "???");

        yield return DialogueController.Instance.ShowDialogue("Incoming transmission....", "???");

        yield return DialogueController.Instance.ShowDialogue("Hey! You did it!", "???");
        yield return DialogueController.Instance.ShowDialogue(
            "You're a GreenShift employee now! I can't wait to start working with you.", "???");

        yield return DialogueController.Instance.ShowDialogue(
            "By the way, the name's Bruce. You can come inside now.", "Bruce");

        _cameraController.FocusPoint = botTarget;
        _cameraController.PositionOffset = (botTarget.forward * 3.5f);
        _cameraController.FocusOffset = Vector3.zero;
        _cameraController.TargetFieldOfView = 45;
        
        DataController.saveData.Save();
        
        yield return new WaitForSeconds(0.5f);
        
        GoHomeScene();

    }

    public void SelectCharacter(int index)
    {
        List<Color> skinColors = new List<Color>()
        {
            BaseUtils.ColorFromHex("FFD7AD"),
            BaseUtils.ColorFromHex("D7A193"),
            BaseUtils.ColorFromHex("744B40"),
            BaseUtils.ColorFromHex("412923"),
            BaseUtils.ColorFromHex("ECAB77"),
            BaseUtils.ColorFromHex("FBC59B"),
        };
        
        List<Color> hairColors = new List<Color>()
        {
            BaseUtils.ColorFromHex("0D0805D0"),
            BaseUtils.ColorFromHex("CFAB63"),
            BaseUtils.ColorFromHex("0D0805D0"),
            BaseUtils.ColorFromHex("0D0805D0"),
            BaseUtils.ColorFromHex("0D0805D0"),
            BaseUtils.ColorFromHex("523522"),
        };

        DataController.saveData.SkinColor = skinColors[index];
        DataController.saveData.HairColor = hairColors[index];
    }

    public List<ClothingObject> HairObjs;
    
    public void SelectHair(int index)
    {
        DataController.saveData.HairObject = HairObjs[index];
    }

    public void GoHomeScene()
    {
        LoadingScreenController.LoadingScreen.OpenLoadingScreen((() =>
        {
            SceneManager.LoadScene("HomeScene");
        }));
    }
}