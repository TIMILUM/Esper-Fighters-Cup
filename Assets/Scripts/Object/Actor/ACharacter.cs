using UnityEngine;
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

    // Start is called before the first frame update
    [SerializeField]
    private float _characterHp;
    public float CharacterHP
    {
        get => _characterHp;
        set => _characterHp = value;
    }

    protected override void Start()
    {
        base.Start();
    }
}
