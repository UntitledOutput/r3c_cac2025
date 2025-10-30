
public TutorialEnemy : EnemyController {

    private TutorialController _tutorialController

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public override void Start()
    {
        base.Start();
        _tutorialController = FindAnyObjectByType<TutorialController>();
        StartCoroutine(OpeningSequence())
    }

    IEnumerator OpeningSequence() {
        var renderers = GetComponentsInChildren<Renderer>();
        
        float v = 0;

        while (v < 1) {
            foreach (var renderer in renderers)
            {
                foreach (var rendererMaterial in renderer.materials)
                {
                    rendererMaterial.SetFloat("_FadeInTime", v);
                }
            }

            v += Time.deltaTime * 0.5f;
        }
    }

    public override void OnDeath() {
        base.OnDeath();

        _tutorialController.targetHit = true;
    }

}