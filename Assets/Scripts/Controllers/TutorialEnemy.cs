
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : EnemyController
{

    private TutorialController _tutorialController;

    public bool IsActive = false;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public override void Start()
    {
        base.Start();
        _tutorialController = FindAnyObjectByType<TutorialController>();
        StartCoroutine(OpeningSequence());
    }

    public override void Update()
    {
        if (IsActive)
        {
            base.Update();
        }
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

            yield return null;
        }
    }

    public override void OnDeath() {
        _tutorialController._targetHit = true;
                    
        var deathParticles = Instantiate(
            Resources.Load<GameObject>("Prefabs/Particles/DeathParticle00"), transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }

}