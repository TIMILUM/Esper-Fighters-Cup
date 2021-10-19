using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhotonAnimatorView))]
public class ACharacter : Actor
{
    public enum Type
    {
        None,
        Telekinesis
    }

    [SerializeField]
    [Tooltip("캐릭터 타입을 설정합니다. 해당 값은 캐릭터 생성 시 사용됩니다.")]
    private Type _characterType = Type.None;

    public Type CharacterType => _characterType;


    //캐릭터 애니메이터
    [SerializeField, Tooltip("직접 컴포넌트를 넣어주세요.")]
    private Animator _animator = null;
    public Animator CharacterAnimator => _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
}
