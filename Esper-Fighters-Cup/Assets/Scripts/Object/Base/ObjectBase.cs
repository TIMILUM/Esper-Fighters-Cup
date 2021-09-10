using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ObjectBase : MonoBehaviourPunCallbacks
{
    
    protected virtual void OnHit(ObjectBase from, ObjectBase to)
    {
        
    }

    public void SetHit(ObjectBase to)
    {
        to.OnHit(this, to);
    }
}
