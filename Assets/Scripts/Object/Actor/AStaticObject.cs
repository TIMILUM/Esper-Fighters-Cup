using EsperFightersCup;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(ObjectHitSystem))]
[RequireComponent(typeof(Collider))]
public class AStaticObject : Actor
{
    [SerializeField]
    private float _fgravity = 30.0f;

    public Vector3 ColliderSize { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        ColliderSize = GetComponent<Collider>().bounds.extents;
    }

    protected override void Start()
    {
        base.Start();

        var state = IngameFSMSystem.Instance[IngameFSMSystem.State.RoundEnd] as IngameRoundEndState;
        state.OnRoundEnd += HandleRoundEnd;
    }

    private void HandleRoundEnd(int round)
    {
        if (photonView == null || !photonView.IsMine)
        {
            return;
        }
        if (gameObject)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (BuffController.ActiveBuffs.Exists(BuffObject.Type.Falling))
        {
            if (Rigidbody.position.y > ColliderSize.y + 0.1f)
            {
                Rigidbody.position -= new Vector3(0.0f, _fgravity, 0.0f) * Time.deltaTime;
            }
            else if (photonView.IsMine)
            {
                BuffController.ReleaseBuffsByType(BuffObject.Type.Falling);
                transform.position = new Vector3(transform.position.x, ColliderSize.y + 0.1f, transform.position.z);
            }
        }

        if (BuffController.ActiveBuffs.Exists(BuffObject.Type.KnockBack) || BuffController.ActiveBuffs.Exists(BuffObject.Type.Falling))
        {
            if (Rigidbody.isKinematic)
            {
                Rigidbody.isKinematic = false;
            }
            return;
        }

        if (!Rigidbody.isKinematic)
        {
            Rigidbody.isKinematic = true;
        }
    }
}
