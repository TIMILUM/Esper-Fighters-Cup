public class SawBladeHitWallState : SawBladeFSMBase
{
    protected override void Initialize()
    {
        State = SawBladeFSMSystem.StateEnum.HitWall;
    }

    public override void StartState()
    {
        _sawBladeObject.SawBladeAnimator.SetBool("isActive", false);
    }

    public override void EndState()
    {
    }
}
