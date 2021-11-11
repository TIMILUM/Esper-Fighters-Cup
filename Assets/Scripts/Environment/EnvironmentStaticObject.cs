using UnityEngine;
/// <summary>
/// 간단하게 오브젝트를 만들었습니다.
/// </summary>
public class EnvironmentStaticObject : AStaticObject
{

    protected bool _isFloating = false;


    protected override void Start()
    {
        base.Start();
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        base.OnHit(from, to, appendBuff);
    }








}
