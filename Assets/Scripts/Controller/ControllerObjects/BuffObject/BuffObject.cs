using System;
using Photon.Pun;
using UnityEngine;

public abstract class BuffObject : ControllerObject
{
    /// <summary>
    /// 버프 오브젝트의 모든 타입이 작성된 enum입니다.
    /// </summary>
    public enum Type
    {
        None,
        Stun,
        KnockBack,
        Slow,
        DecreaseHp,
        IncreaseHp,
        Raise,
        Falling,
        Sliding,
        Grab,
        MoveSpeed
    }

    [SerializeField]
    protected string _name = "None";

    [SerializeField]
    protected BuffStruct _buffStruct;

    /// <summary>
    /// 해당 버프의 아이디입니다.
    /// </summary>
    public string BuffId { get; set; }

    /// <summary>
    /// 버프 생성 후 지금까지 진행된 밀리초입니다. (밀리초 단위)
    /// </summary>
    public double ElapsedMilliseconds { get; private set; }

    /// <summary>
    /// 해당 버프가 생성된 시간입니다.
    /// </summary>
    public int StartTime { get; private set; }

    /// <summary>
    /// 해당 버프의 타입입니다.
    /// </summary>
    public Type BuffType => _buffStruct.Type;

    /// <summary>
    /// 해당 버프가 지속되는 시간입니다. (초 단위)
    /// </summary>
    public float Duration
    {
        get => _buffStruct.Duration;
        set => _buffStruct.Duration = value;
    }

    protected virtual void Start()
    {
        StartTime = PhotonNetwork.ServerTimestamp;
    }

    protected virtual void Update()
    {
        /**
         * @todo Update 용 abstract 메소드 만들기
         * @body abstract로 Update 메소드를 만들어서 자식 클래스에서는 아래 조건문 생략
         */
        if (!IsRegistered || !Author.photonView.IsMine)
        {
            return;
        }

        var now = PhotonNetwork.ServerTimestamp;
        ElapsedMilliseconds = now - StartTime;

        if (_buffStruct.Duration <= 0)
        {
            return;
        }

        if (ElapsedMilliseconds > _buffStruct.Duration * 1000)
        {
            // BUG: 특정 상황에서 연속으로 메소드를 실행함
            // 아마 ReleaseBuff를 보내고 나서 다시 이벤트를 받기까지 시간 간격이 있는데,
            // 그 동안 이 오브젝트가 해제되지 못해서 생기는 버그인듯?
            ControllerCast<BuffController>().ReleaseBuff(this);
        }

        if (_buffStruct.IsOnlyOnce)
        {
            ControllerCast<BuffController>().ReleaseBuff(this);
        }
    }

    /// <summary>
    /// BuffStruct를 통해 해당 버프의 세부 정보를 설정해주는 함수입니다.
    /// </summary>
    /// <param name="buffStruct">버프 관련 데이터를 담는 임시 구조체입니다.</param>
    public virtual void SetBuffStruct(BuffStruct buffStruct)
    {
        _buffStruct = buffStruct;
    }

    [Serializable]
    public class BuffStruct
    {
        [SerializeField] private Type _type;
        [SerializeField] private float _duration;
        [SerializeField] private float[] _valueFloat;
        [SerializeField] private Vector3[] _valueVector3;
        [SerializeField] private bool _allowDuplicates;
        [SerializeField] private float _damage;
        [SerializeField] private bool _isOnlyOnce;


        public Type Type { get => _type; set => _type = value; }
        public float Duration { get => _duration; set => _duration = value; }
        public float[] ValueFloat { get => _valueFloat; set => _valueFloat = value; }
        public Vector3[] ValueVector3 { get => _valueVector3; set => _valueVector3 = value; }
        public bool AllowDuplicates { get => _allowDuplicates; set => _allowDuplicates = value; }
        public float Damage { get => _damage; set => _damage = value; }
        /// 해당 버프 한번만 적용 되는지 판별하는 변수
        public bool IsOnlyOnce { get => _isOnlyOnce; set => _isOnlyOnce = value; }
    }


}
