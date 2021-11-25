using UnityEngine;

public class DecreaseHpObject : BuffObject
{
    private ACharacter _character = null;
    private float _damage = 0;

    protected override void Reset()
    {
        base.Reset();

        _name = "";
        _buffStruct.Type = Type.DecreaseHp;
    }

    protected override void OnRegistered()
    {
        base.OnRegistered();
        _character = Author.GetComponent<ACharacter>();
    }

    protected override void Update()
    {
        base.Update();
        if (!IsRegistered || !Author.photonView.IsMine)
        {
            return;
        }

        if (_character is null)
        {
            Debug.Log("HP does not found!");
            Controller.ReleaseBuff(this);
            return;
        }

        _character.HP -= (int)_damage; // BUG: 대미지가 float보다는 int로 바뀌는게 좋음
        Debug.Log("ChracterName : " + _character.transform.name + "  Chracter HP : " + _character.HP);
    }

    public override void SetBuffStruct(BuffStruct buffStruct)
    {
        base.SetBuffStruct(buffStruct);
        _damage = buffStruct.Damage;
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
    {
    }
}
