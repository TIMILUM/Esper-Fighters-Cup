using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviourPunCallbacks
{

    protected abstract void OnHit(ObjectBase from, ObjectBase to);

    public void SetHit(ObjectBase to)
    {
        to.OnHit(this, to);
    }
    
    
}
