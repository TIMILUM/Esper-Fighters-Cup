using System;
using UnityEngine;

public class StunObject : BuffObject
{
    private ACharacter _character;

    private void Reset()
    {
        _name = "Stun";
        _buffStruct.Type = Type.Stun;
    }

    protected override void OnRegistered()
    {
        base.OnRegistered();
        _character = Author as ACharacter;

        if (!(_character is null))
        {
            _character.CharacterAnimator.SetTrigger("Hit");
        }
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
    {
        throw new NotImplementedException();
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }
}
