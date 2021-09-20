using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("이 오브젝트와 충돌한 오브젝트에게 부여되는 버프목록입니다.")]
    protected List<BuffObject.BuffStruct> _buffOnCollision = new List<BuffObject.BuffStruct>();

    /**
     * 인게임 오브젝트(스킬, 버프, 플레이어, 환경 오브젝트 등)과 충돌이 일어나면 호출되는 함수입니다.
     * 플레이어에 종속된 이벤트가 아닌 인게임 오브젝트에서 인게임 오브젝트로 발생하는 이벤트입니다.
     * 만약 플레이어에 종속된 충돌 이벤트를 원하신다면 다른 함수를 찾아보세요.
     */
    protected abstract void OnHit(ObjectBase @from, ObjectBase to, BuffObject.BuffStruct[] appendBuff);

    /**
     * 인게임 오브젝트(스킬, 버프, 플레이어, 환경 오브젝트 등)과 충돌을 일으킬 함수입니다.
     * 해당 함수를 실행하면 관련된 오브젝트에 _buffOnCollision과 함께 OnHit 함수로 호출됩니다.
     */
    public virtual void SetHit(ObjectBase to)
    {
        to.OnHit(this, to, _buffOnCollision.ToArray());
    }

    /**
     * 이 오브젝트와 충돌한 오브젝트에게 부여될 버프를 추가합니다.
     */
    protected void AddBuffOnCollision(BuffObject.BuffStruct buff)
    {
        _buffOnCollision.Add(buff);
    }
}
