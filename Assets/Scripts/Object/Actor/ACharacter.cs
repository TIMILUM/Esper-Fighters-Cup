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

    [Tooltip("캐릭터 타입을 설정합니다. 해당 값은 캐릭터 생성 시 사용됩니다.")]
    [SerializeField] private Type _characterType = Type.None;

    public Type CharacterType => _characterType;
    public AnimatorSync Animator { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Animator = GetComponent<AnimatorSync>();
    }
}
