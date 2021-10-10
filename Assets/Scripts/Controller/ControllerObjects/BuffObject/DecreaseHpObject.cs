using UnityEngine;

public class DecreaseHpObject : BuffObject
{

    private ACharacter _character = null;
    private float _damage = 0;

    protected override void Start()
    {
        base.Start();
        _character = Controller.ControllerManager.GetActor().GetComponent<ACharacter>();
    }

    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.DecreaseHp;
    }

    protected override void Update()
    {
        base.Update();
        if (!IsRegistered || !Author.photonView.IsMine)
        {
            return;
        }

        if (_character == null)
        {
            Debug.Log("HP does not found!");
            ControllerCast<BuffController>().ReleaseBuff(BuffId);
            return;
        }

        _character.CharacterHP -= _damage;
        Debug.Log("ChracterName : " + _character.transform.name + "  Chracter HP : " + _character.CharacterHP);
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
