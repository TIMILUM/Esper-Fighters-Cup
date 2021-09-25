using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunObject : BuffObject
{
    
    
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
    }

    private void Reset()
    {
        _name = "Stun";
        _buffStruct.Type = Type.Stun;
    }

    protected override void OnHit(ObjectBase @from, ObjectBase to, BuffStruct[] appendBuff)
    {
        throw new NotImplementedException();
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
        
    }
}
