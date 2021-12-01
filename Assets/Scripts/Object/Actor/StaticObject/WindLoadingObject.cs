using System.Collections;
using Photon.Pun;
using UnityEngine;

public class WindLoadingObject : AStaticObject
{
    [SerializeField]
    private ColliderChecker _colliderChecker;

    private BuffObject.BuffStruct[] _appendBuffStack;

    private Coroutine _generateKnockBackCoroutine;

    protected override void Start()
    {
        base.Start();
        _colliderChecker.OnCollision += SetHit;
    }

    protected override void Update()
    {
        base.Update();
        if (_generateKnockBackCoroutine == null)
        {
            _generateKnockBackCoroutine = StartCoroutine(GenerateKnockBackObject());
        }
    }

    public void SetBuffStack(BuffObject.BuffStruct[] appendBuff)
    {
        _appendBuffStack = appendBuff;
    }

    private IEnumerator GenerateKnockBackObject()
    {
        foreach (var buffStruct in _appendBuffStack)
        {
            BuffController.GenerateBuff(buffStruct);
        }

        yield return new WaitUntil(() => BuffController.ActiveBuffs[BuffObject.Type.KnockBack].Count > 0);
        yield return new WaitUntil(() => BuffController.ActiveBuffs[BuffObject.Type.KnockBack].Count <= 0);

        PhotonNetwork.Destroy(gameObject);
    }
}
