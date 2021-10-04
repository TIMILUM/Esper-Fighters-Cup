using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentStaticObject : EnvironmentStaticObject
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
