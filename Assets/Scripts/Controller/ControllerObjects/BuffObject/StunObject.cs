using System;
using UnityEngine;

public class StunObject : BuffObject
{
    private void Reset()
    {
        _name = "Stun";
        _buffStruct.Type = Type.Stun;
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
    {
        throw new NotImplementedException();
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }
}
