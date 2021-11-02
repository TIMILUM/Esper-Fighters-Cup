using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedObject : BuffObject
{

    private float _addedSpeed = 0;
    public float AddedSpeed => _addedSpeed;
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

    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.MoveSpeed;
        
    }
    
    public override void SetBuffStruct(BuffStruct buffStruct)
    {
        base.SetBuffStruct(buffStruct);
        _addedSpeed = buffStruct.ValueFloat[0] / 10.0f;
    }

    protected override void OnHit(ObjectBase @from, ObjectBase to, BuffStruct[] appendBuff)
    {
        throw new System.NotImplementedException();
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
        throw new System.NotImplementedException();
    }
}
