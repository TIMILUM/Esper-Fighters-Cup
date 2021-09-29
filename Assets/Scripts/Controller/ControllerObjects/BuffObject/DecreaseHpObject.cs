using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseHpObject : BuffObject
{

    private ACharacter _character = null;
    private float _damage = 0;

    private new void Start()
    {
        _character = _controller.ControllerManager.GetActor().GetComponent<ACharacter>();
    }

    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.DecreaseHp;
    }

    private new void Update()
    {
        base.Update();
        if (_character == null)
        {
            Debug.Log("HP does not found!");
            ControllerCast<BuffController>().ReleaseBuff(this);
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
