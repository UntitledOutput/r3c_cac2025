public class FollowAbilityEffectController : AbilityEffectController
{
    protected override void Update()
    {
        base.Update();
        transform.position = actor.transform.position;
        transform.eulerAngles = actor.transform.eulerAngles;
    }
}