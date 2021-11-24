using UnityEngine;

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

    [SerializeField] private Animator _animator;

    public Type CharacterType => _characterType;
    public Animator Animator => _animator;
}
