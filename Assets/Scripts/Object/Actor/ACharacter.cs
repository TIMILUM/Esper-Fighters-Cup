using EsperFightersCup.Net;
using UnityEngine;

[RequireComponent(typeof(AnimatorSync))]
public class ACharacter : Actor
{
    public enum Type
    {
        None,
        Telekinesis,
        Plank
    }

    [SerializeField]
    [Tooltip("캐릭터 타입을 설정합니다. 해당 값은 캐릭터 생성 시 사용됩니다.")]
    private Type _characterType = Type.None;

    public Type CharacterType => _characterType;

    public Animator Animator { get; private set; }
    public AnimatorSync AnimatorSync { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AnimatorSync = GetComponent<AnimatorSync>();
        Animator = AnimatorSync.Animator;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
}
