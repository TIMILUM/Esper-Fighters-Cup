using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonRigidbodyView))]
public class FragmentStaticObject : EnvironmentStaticObject
{
    [SerializeField]
    private float _destoryTime;
    private float _currentTime;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        base.OnHit(from, to, appendBuff);
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > _destoryTime)
        {
            if (_buffController.GetBuff(BuffObject.Type.Raise) == null)
                PhotonNetwork.Destroy(gameObject);
        }
    }
}
