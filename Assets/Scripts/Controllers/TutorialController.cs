
public class TutorialController : MonoBehaviour {

    private PlayerController _playerController;
    private CameraController _cameraController;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _cameraController = FindAnyObjectByType<CameraController>();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        
    }

    public IEnumerator TutorialSequence() {

        _playerController.BlockMovement = true;

        // player select

        // 

        yield return DialogueController.Instance.ShowDialogue("Welcome to GreenShift. We specialize in recycling management, and it seems that you're interested in our services!", "???");
        yield return DialogueController.Instance.ShowDialogue("Your application to work here was accepted, though we do have an onboarding process.", "???");
        yield return DialogueController.Instance.ShowDialogue("You applied for the position of Waste Collector, so we will be training you to be the best waste collector in the area.", "???");

        // move camera out

        Transform botTarget = null;

        _cameraController.FocusOnPlayer = false;
            
        _cameraController.FocusPoint = botTarget;
        _cameraController.PositionOffset = (botTarget.forward* 2f) + (botTarget.up * 1.0f)
        _cameraController.FocusOffset = Vector3.zero;
        _cameraController.TargetFieldOfView = 45;

        yield return DialogueController.Instance.ShowDialogue("Welcome to the GreenShift Training Facility. You will be learning everything necessary to carry out collection runs.", "???");

        // move camera to player

        _cameraController.FocusOnPlayer = true;
        _cameraController.TargetFieldOfView = 75;

        yield return DialogueController.Instance.ShowDialogue("For your first task: use WASD to move.", "???");

        _playerController.BlockMovement = false;

    }
}