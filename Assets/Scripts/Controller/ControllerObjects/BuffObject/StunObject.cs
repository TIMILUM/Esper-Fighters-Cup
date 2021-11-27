using System;
using UnityEngine;

public class StunObject : BuffObject
{
    private ACharacter _character;

    protected override void Reset()
    {
        base.Reset();

        _name = "Stun";
        _buffStruct.Type = Type.Stun;
    }

    protected override void OnRegistered()
    {
        base.OnRegistered();
        _character = Author as ACharacter;

        if (_character && _character.Animator)
        {
            var data = transform.transform.position;
            data.y = 0.01f;
            _character.Animator.SetTrigger("Hit");
            ParticleManager.Instance.PullParticle("Hit", data, Quaternion.identity);
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
