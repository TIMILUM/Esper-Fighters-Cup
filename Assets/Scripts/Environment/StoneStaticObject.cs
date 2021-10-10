public class StoneStaticObject : EnvironmentStaticObject
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        base.OnHit(from, to, appendBuff);
    }
}
