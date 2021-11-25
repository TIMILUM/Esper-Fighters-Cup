using UnityEngine;

public class MoveSpeedObject : BuffObject
{
    public float AddedSpeed { get; private set; }

    protected override void Reset()
    {
        base.Reset();

        _name = "";
        _buffStruct.Type = Type.MoveSpeed;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void SetBuffStruct(BuffStruct buffStruct)
    {
        base.SetBuffStruct(buffStruct);
        AddedSpeed = buffStruct.ValueFloat[0] / 10.0f;
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
    {
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }
}
