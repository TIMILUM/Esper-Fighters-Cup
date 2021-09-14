using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviourPunCallbacks
{
    [SerializeField]
    protected List<BuffObject.BuffStruct> _buffOnCollision = new List<BuffObject.BuffStruct>();

    protected abstract void OnHit(ObjectBase @from, ObjectBase to, BuffObject.BuffStruct[] appendBuff);

    public virtual void SetHit(ObjectBase to)
    {
        to.OnHit(this, to, _buffOnCollision.ToArray());
    }

    protected void AddBuffOnCollision(BuffObject.BuffStruct buff)
    {
        _buffOnCollision.Add(buff);
    }
}
