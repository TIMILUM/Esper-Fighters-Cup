using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 간단하게 오브젝트를 만들었습니다.
/// </summary>
public class EnvironmentStaticObject : AStaticObject
{

    [SerializeField]
    private Vector3 _minRandomScale;
    [SerializeField]
    private Vector3 _maxRandomScale;
    protected bool _isFloating = false;

    [SerializeField, Tooltip("직접 넣어주세여")]
    private Collider _Collider;

    private Vector3 _direction;



    private void RaiseObject()
    {

    }

    private void DropObject()
    {

    }

    private void ThrowObject()
    {

    }


    protected override void Start()
    {
        base.Start();
        transform.localScale = RandomScale(_minRandomScale, _maxRandomScale);
    }


    protected Vector3 RandomScale(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        base.OnHit(from, to, appendBuff);
    }




}
