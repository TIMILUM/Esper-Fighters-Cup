using System;
using System.Diagnostics.CodeAnalysis;
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
        IncreaseHp
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

    protected override void Start()
    {
        base.Start();
        StartTime = PhotonNetwork.ServerTimestamp;
    }

    protected virtual void Update()
    {
        if (!Author.photonView.IsMine)
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
            ControllerCast<BuffController>().ReleaseBuff(BuffId);
        }

        if (_buffStruct.isOnlyonce)
        {
            ControllerCast<BuffController>().ReleaseBuff(BuffId);
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

    /**
     * @todo BuffStruct 리팩토링 필요
     * @body get/set auto 프로퍼티로 변경을 하거나, 인스펙터 상에 보여야 한다면 SerializeField 적용 등 리팩토링이 필요합니다.
     */
    [Serializable]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class BuffStruct
    {
        public Type Type;
        public float Duration;
        public float[] ValueFloat;
        public Vector3[] ValueVector3;
        public bool AllowDuplicates = true;
        public float Damage;
        /// 해당 버프 한번만 적용 되는지 판별하는 변수
        public bool isOnlyonce = false;

    }
}
