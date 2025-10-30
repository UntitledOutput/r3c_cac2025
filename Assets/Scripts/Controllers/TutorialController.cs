
public class TutorialController : MonoBehaviour {

    private PlayerController _playerController;
    private CameraController _cameraController;
    private TutorialTarget Target;

    private Transform botTarget;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _cameraController = FindAnyObjectByType<CameraController>();
        
        Target = new GameObject().AddComponent<TutorialTarget>();

        botTarget = Instantiate(Resources.Load<GameObject>("Prefabs/OnboardBot"));
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        
    }

    public bool _targetHit = true;

    public IEnumerator TutorialSequence() {

        _playerController.BlockMovement = true;

        // player select

        // 

        yield return DialogueController.Instance.ShowDialogue("Welcome to GreenShift. We specialize in recycling management, and it seems that you're interested in our services!", "???");
        yield return DialogueController.Instance.ShowDialogue("Your application to work here was accepted, though we do have an onboarding process.", "???");
        yield return DialogueController.Instance.ShowDialogue("You applied for the position of Waste Collector, so we will be training you to be the best waste collector in the area.", "???");

        // move camera out
        _cameraController.FocusOnPlayer = false;
            
        _cameraController.FocusPoint = botTarget;
        _cameraController.PositionOffset = (botTarget.forward* 2f) + (botTarget.up * 1.0f)
        _cameraController.FocusOffset = Vector3.zero;
        _cameraController.TargetFieldOfView = 45;

        yield return DialogueController.Instance.ShowDialogue("Welcome to the GreenShift Training Facility. You will be learning everything necessary to carry out collection runs.", "???");

        // move camera to player

        _cameraController.FocusOnPlayer = true;
        _cameraController.TargetFieldOfView = 75;

        yield return DialogueController.Instance.ShowDialogue("For your first task: use WASD to move towards the target", "???");

        // walk to target

        _playerController.BlockMovement = false;

        var task = new TaskController.Task{name="Walk to Target", description="Walk to the Target, and you get your next task"};

        TaskController.Instance.AddTask(task);

        while (Vector3.Distance(_playerController.transform.position, TutorialTarget.position)) {
            yield return null;
        }

        // completed task

        task.Complete();

        AbilityObject startingAbility = null;

        yield return DialogueController.Instance.ShowDialogue("Great job. Next task, I'll provide you an ability.", "???");
        yield return DialogueController.Instance.ShowDialogue("Abilities allow you to interact with the environment around you.", "???");
        yield return DialogueController.Instance.ShowDialogue($"Here's my favorite: {startingAbility.DisplayName}", "???");

        yield return DialogueController.Instance.ShowDialogue("Hit the targets by tapping on them, and I'll give you your next task.", "???");

        // hit the 3 targets
        task = new TaskController.Task{name="Hit the Three Targets", description="Hit the three targets and you get your next task."};


        while (!targetHit) {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        TutorialTarget.transform.DoMove(new Vector3(3,0,0), 1.0f).SetEase(Ease.InOutQuad);

        while (!targetHit) {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        TutorialTarget.transform.DoMove(new Vector3(-3,0,-3), 1.0f).SetEase(Ease.InOutQuad);


        while (!targetHit) {
            yield return null;
        }

        // completed task

        task.Complete();

        yield return new WaitForSeconds(0.5f);     

        yield return DialogueController.Instance.ShowDialogue("You're doing good. Now to move on to a more difficult part.", "???");



        yield return new WaitForSeconds(0.5f);

        yield return DialogueController.Instance.ShowDialogue("I'm going to give you 2 more abilities, and you must defeat the enemy placed in front of you.", "???"); 

        task = new TaskController.Task{name="Defeat the Enemy", description="Defeat the enemy, and you get your next task."};

        while (!targetHit) {
            yield return null;
        }

        // completed task
        task.Complete();

        yield return DialogueController.Instance.ShowDialogue("Great job. You have passed the employee onboarding module.", "???"); 
        yield return DialogueController.Instance.ShowDialogue("Incoming transmission....", "???"); 

        var renderers = botTarget.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            foreach (Material material in renderer.materials) {
                if (material.name == "Screen00") {
                    material.SetBool("_")
                }
            }
        }

        yield return DialogueController.Instance.ShowDialogue("Hey! {name}! You did it!", "???"); 
        yield return DialogueController.Instance.ShowDialogue("You're a GreenShift employee now! I can't wait to start working with you.", "???"); 
        yield return DialogueController.Instance.ShowDialogue("By the way, the name's Bruce. You can come inside now.", "Bruce"); 

        // open door

    }
}