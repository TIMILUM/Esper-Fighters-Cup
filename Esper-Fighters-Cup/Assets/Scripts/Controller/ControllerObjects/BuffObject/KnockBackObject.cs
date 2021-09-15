using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackObject : BuffObject
{
    [SerializeField, Tooltip("넉백 방향 입니다.")]
    private Vector3 _normalizedDirection = Vector3.zero;
    public Vector3 NormalizedDirection
    {
        set => _normalizedDirection = value;
    }

    [SerializeField, Tooltip("최종 넉백 거리는 (스피드 * 지속시간) 입니다.")]
    private float _speed = 1;
    public float Speed
    {
        set => _speed = value;
    }
    
    [SerializeField]
    private AnimationCurve _lerpCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private Actor _actor = null;
    
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        _normalizedDirection = _normalizedDirection.normalized;
        _actor = _controller.ControllerManager.GetActor();
        _startPosition = _actor.transform.localPosition;
        _endPosition = _startPosition + (_normalizedDirection * _speed * _buffStruct.Duration);
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
        if (photonView != null && !photonView.IsMine) return;
        
        var t = _lerpCurve.Evaluate((float)((_elapsedMilliseconds / 1000) / _buffStruct.Duration));
        _actor.transform.position = Vector3.Lerp(_startPosition, _endPosition, t);
    }

    public override void SetBuffStruct(BuffStruct buffStruct)
    {
        base.SetBuffStruct(buffStruct);
        _normalizedDirection = buffStruct.ValueVector3[0];
        _speed = buffStruct.ValueFloat[0];
    }

    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.KnockBack;
    }

    protected override void OnHit(ObjectBase @from, ObjectBase to, BuffStruct[] appendBuff)
    {
    }
}