using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffObject : ControllerObject
{
    /**
     * 버프 오브젝트의 모든 타입이 작성된 enum입니다.
     */
    public enum Type
    {
        None,
        Stun,
        KnockBack,
        Slow,
    }
    [SerializeField]
    protected string _name = "None";

    [SerializeField]
    protected BuffStruct _buffStruct;

    /**
     * 해당 버프의 타입입니다.
     */
    public Type BuffType => _buffStruct.Type;

    /**
     * 해당 버프가 지속되는 시간입니다. (초 단위)
     */
    public float Duration
    {
        get => _buffStruct.Duration;
        set => _buffStruct.Duration = value;
    }

    /**
     * 해당 버프가 생성된 시간입니다.
     */
    protected DateTime _startTime = DateTime.Now;
    /**
     * 버프 생성 후 지금까지 진행된 밀리초입니다. (밀리초 단위)
     */
    protected double _elapsedMilliseconds;
    
    // Start is called before the first frame update
    protected void Start()
    {
    }

    // Update is called once per frame
    protected void Update()
    {
        _elapsedMilliseconds = (DateTime.Now - _startTime).TotalMilliseconds;
        if (_buffStruct.Duration <= 0) return;
        if(_elapsedMilliseconds > _buffStruct.Duration * 1000) ControllerCast<BuffController>().ReleaseBuff(this);
    }

    /**
     * BuffStruct를 통해 해당 버프의 세부 정보를 설정해주는 함수입니다.
     */
    public virtual void SetBuffStruct(BuffStruct buffStruct)
    {
        _buffStruct = buffStruct;
    }

    [Serializable]
    public class BuffStruct
    {
        public Type Type;
        public float Duration;
        public float[] ValueFloat;
        public Vector3[] ValueVector3;
        public bool AllowDuplicates = true;
    }
}
