using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffObject : ControllerObject
{
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

    public Type BuffType => _buffStruct.Type;

    public float Duration
    {
        get => _buffStruct.Duration;
        set => _buffStruct.Duration = value;
    }

    protected DateTime _startTime = DateTime.Now;
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
