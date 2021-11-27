public class MoveSpeedObject : BuffObject
{
    public float AddedSpeed { get; private set; }

    public override Type BuffType => Type.MoveSpeed;

    public override void OnBuffGenerated()
    {
        AddedSpeed = Info.ValueFloat[0] / 10.0f;
    }
}
