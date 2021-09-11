using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffObject : ControllerObject
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
    protected Type _buffType = Type.None;
    public Type BuffType => _buffType;

    [SerializeField]
    protected float _duration = 1.0f;
    public float Duration
    {
        get => _duration;
        set => _duration = value;
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
        if (_duration <= 0) return;
        if(_elapsedMilliseconds > _duration * 1000) ControllerCast<BuffController>().ReleaseBuff(this);
    }
}
