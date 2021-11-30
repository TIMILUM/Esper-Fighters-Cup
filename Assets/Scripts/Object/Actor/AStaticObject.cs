using UnityEngine;

[RequireComponent(typeof(ObjectHitSystem))]
public class AStaticObject : Actor
{
    // Start is called before the first frame update

    [SerializeField]
    private float _fgravity = 30.0f;
    [SerializeField]
    private BoxCollider _boxcollider;
    [FMODUnity.EventRef]
    [SerializeField] private string _collideSound;

    public ObjectHitSystem HitSystem { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        HitSystem = GetComponent<ObjectHitSystem>();
        HitSystem.OnHit += PlayHitSound;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!photonView.IsMine)
        {
            return;
        }

        if (BuffController.ActiveBuffs.Exists(BuffObject.Type.Falling))
        {
            if (Rigidbody.position.y > _boxcollider.bounds.extents.y + 0.03f)
            {
                Rigidbody.position -= new Vector3(0.0f, _fgravity, 0.0f) * Time.deltaTime;
            }
            else
            {
                BuffController.ReleaseBuffsByType(BuffObject.Type.Falling);
            }
            return;
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

    private void PlayHitSound(HitInfo info)
    {
        if (string.IsNullOrWhiteSpace(_collideSound))
        {
            return;
        }
        var instance = FMODUnity.RuntimeManager.CreateInstance(_collideSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, gameObject.transform, Rigidbody);
        instance.setParameterByName("DestroyCheck", info.IsDestroy ? 1f : 0f);
        instance.start();
        instance.release();
    }
}
