
using System;
using MyBox;
using UnityEngine;
using UnityEngine.Animations;

public class TutorialTarget : MonoBehaviour {

    private TutorialController _tutorialController;
    public Collider Collider;

    public bool Active = false;
    public bool HideSprite = true;
    public Type Limit;

    private SpriteRenderer _sprite;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _tutorialController = FindAnyObjectByType<TutorialController>();
        Collider = gameObject.GetOrAddComponent<SphereCollider>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        
        var source = new ConstraintSource();
        source.sourceTransform = Camera.main.transform;
        source.weight = 1;
        _sprite.GetComponent<LookAtConstraint>().SetSource(0,source);
        _sprite.GetComponent<LookAtConstraint>().constraintActive = true;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Collider.enabled = Active;
        _sprite.enabled = !HideSprite;
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (Limit != null)
        {
            if (other.gameObject.GetComponent(Limit))
            {
                if (Active)
                {
                    _tutorialController._targetHit = true;
                    Active = false;
                }
            }
        }
        else
        {
            if (Active)
            {
                _tutorialController._targetHit = true;
                Active = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,1f);
    }
}