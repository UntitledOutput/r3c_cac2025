
public TutorialTarget : MonoBehaviour {

    private TutorialController _tutorialController;
    public Collider Collider;

    public bool Active = false;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _tutorialController = FindAnyObjectByType<TutorialController>();
        Collider = gameObject.GetOrAddComponent<SphereCollider>();
        Collider.radius = 1;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Collider.enabled = Active;
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (Active) {
            _tutorialController.targetHit = true;
            Active = false;
        }
    }

}