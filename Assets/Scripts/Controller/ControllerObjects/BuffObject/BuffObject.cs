using System;
using System.Collections;
using EsperFightersCup.Net;
using Photon.Pun;
using UnityEngine;

public abstract class BuffObject : ControllerObject<BuffController>
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

    private Coroutine _elapsedTimeout;

    /// <summary>
    /// 해당 버프의 아이디입니다.
    /// </summary>
    public string BuffId { get; set; }

    /// <summary>
    /// 버프 생성 후 지금까지 진행된 밀리초입니다. (밀리초 단위)
    /// </summary>
    [Obsolete("제대로 작동하지 않습니다.", true)]
    public int ElapsedTime { get; private set; }

    /// <summary>
    /// 해당 버프가 생성된 시간입니다.
    /// </summary>
    public int StartTime { get; private set; }

    /// <summary>
    /// 해당 버프의 타입입니다.
    /// </summary>
    public abstract Type BuffType { get; }

    public BuffStruct Info { get; private set; }

    protected sealed override void OnRegistered(Action continueFunc)
    {
        // TODO: 나중에 continueFunc에서 ActiveBuff 제거하는 코드 넣어야됨
        StartTime = PhotonNetwork.ServerTimestamp;
        gameObject.SetActive(true);

        OnBuffGenerated();
        // IsOnlyOnce일 경우 다음 프레임 기다리고 ReleaseBuff를 또 부를 필요 없이 바로 OnReleased 호출해서 삭제
        if (Author.photonView.IsMine && !Author.photonView.Controller.IsInactive)
        {
            _elapsedTimeout = StartCoroutine(CheckBuffRelease());
        }
    }

    protected sealed override void OnReleased()
    {
        if (Author.photonView.IsMine && _elapsedTimeout != null)
        {
            StopCoroutine(_elapsedTimeout);
            _elapsedTimeout = null;
        }
        OnBuffReleased();
        Destroy(gameObject);
    }

    /// <summary>
    /// BuffStruct를 통해 해당 버프의 세부 정보를 설정해주는 함수입니다.
    /// </summary>
    /// <param name="info">버프 관련 데이터를 담는 임시 구조체입니다.</param>
    public void SetBuffStruct(BuffStruct info)
    {
        Info = info;
    }

    private IEnumerator CheckBuffRelease()
    {
        yield return new WaitForEndOfFrame();

        if (Info.IsOnlyOnce)
        {
            Controller.ReleaseBuff(this);
            yield break;
        }

        if (Info.Duration == 0f)
        {
            yield break;
        }

        yield return new WaitForSeconds(Info.Duration);
        Controller.ReleaseBuff(this);
    }

    /// <summary>
    /// 버프가 생성될 때 호출됩니다.<para/>
    /// 버프컨트롤러의 주인이 본인이 아니더라도 호출됩니다.
    /// </summary>
    public virtual void OnBuffGenerated()
    {
    }

    /// <summary>
    /// 버프가 해제될 때 호출됩니다.<para/>
    /// 버프컨트롤러의 주인이 본인이 아니더라도 호출됩니다.
    /// </summary>
    public virtual void OnBuffReleased()
    {
    }

    [Serializable]
    public class BuffStruct
    {
        [SerializeField] private Type _type;
        [SerializeField] private float _duration;
        [SerializeField] private float[] _valueFloat = new float[0];
        [SerializeField] private Vector3[] _valueVector3 = new Vector3[0];
        [SerializeField] private bool _allowDuplicates;
        [SerializeField] private float _damage;
        [SerializeField] private bool _isOnlyOnce;

        public Type Type { get => _type; set => _type = value; }
        /// <summary>
        /// Duration은 무조건 초 단위로 넘겨주셔야 됩니다. (1000ms -> 0.1s)
        /// </summary>
        public float Duration { get => _duration; set => _duration = value; }
        /// <summary>
        /// 시간 정보를 가지는 값들은 초 단위로 변환 후 넘겨주세요.
        /// </summary>
        public float[] ValueFloat { get => _valueFloat; set => _valueFloat = value; }
        public Vector3[] ValueVector3 { get => _valueVector3; set => _valueVector3 = value; }
        public bool AllowDuplicates { get => _allowDuplicates; set => _allowDuplicates = value; }
        public float Damage { get => _damage; set => _damage = value; }
        /// 해당 버프 한번만 적용 되는지 판별하는 변수
        public bool IsOnlyOnce { get => _isOnlyOnce; set => _isOnlyOnce = value; }

        public override string ToString()
        {
            return string.Join(",",
                Type,
                Duration,
                string.Join(",", ValueFloat),
                string.Join(",", ValueVector3),
                AllowDuplicates,
                Damage,
                IsOnlyOnce);
        }

        public BuffGenerateArguments ToBuffArguments(string id)
        {
            return new BuffGenerateArguments(
                (int)_type,
                id,
                _duration,
                ValueFloat,
                ValueVector3,
                AllowDuplicates,
                Damage,
                IsOnlyOnce);
        }

        public static explicit operator BuffStruct(in BuffGenerateArguments args)
        {
            return new BuffStruct
            {
                Type = (Type)args.Type,
                Duration = args.Duration,
                ValueFloat = args.ValueFloat,
                ValueVector3 = args.ValueVector3,
                AllowDuplicates = args.AllowDuplicates,
                Damage = args.Damage,
                IsOnlyOnce = args.IsOnlyOnce
            };
        }
    }
}
