using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public abstract class BuffObject : ControllerObject
{
    /// <summary>
    ///     버프 오브젝트의 모든 타입이 작성된 enum입니다.
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
    ///     버프 생성 후 지금까지 진행된 밀리초입니다. (밀리초 단위)
    /// </summary>
    public double ElapsedMilliseconds { get; private set; }

    /// <summary>
    ///     해당 버프가 생성된 시간입니다.
    /// </summary>
    public DateTime StartTime { get; } = DateTime.Now;

    /// <summary>
    ///     해당 버프의 타입입니다.
    /// </summary>
    public Type BuffType => _buffStruct.Type;

    /// <summary>
    ///     해당 버프가 지속되는 시간입니다. (초 단위)
    /// </summary>
    public float Duration
    {
        get => _buffStruct.Duration;
        set => _buffStruct.Duration = value;
    }

    /// <summary>
    /// 버프의 시전자 오브젝트입니다.
    /// </summary>
    protected APlayer Author { get; private set; }

    // Start is called before the first frame update
    protected void Start()
    {
        if (_controller.ControllerManager.GetActor() is APlayer player)
        {
            Author = player;
        }
    }

    /// <summary>
    /// 해당 버프가 자신의 것인지를 확인하고 설정된 값에 따라 버프를 해제하는 작업을 합니다.
    /// </summary>
    protected void Update()
    {
        if (!Author.photonView.IsMine)
        {
            return;
        }

        ElapsedMilliseconds = (DateTime.Now - StartTime).TotalMilliseconds;
        if (_buffStruct.Duration <= 0)
        {
            return;
        }

        if (ElapsedMilliseconds > _buffStruct.Duration * 1000)
        {
            ControllerCast<BuffController>().ReleaseBuff(this);
        }

        if (_buffStruct.isOnlyonce)
        {
            ControllerCast<BuffController>().ReleaseBuff(this);
        }
    }

    /// <summary>
    ///     BuffStruct를 통해 해당 버프의 세부 정보를 설정해주는 함수입니다.
    /// </summary>
    /// <param name="buffStruct">버프 관련 데이터를 담는 임시 구조체입니다.</param>
    public virtual void SetBuffStruct(BuffStruct buffStruct)
    {
        _buffStruct = buffStruct;
    }

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
