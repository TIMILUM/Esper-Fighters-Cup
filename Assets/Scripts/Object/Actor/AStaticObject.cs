using UnityEngine;

[RequireComponent(typeof(ObjectHitSystem))]
public class AStaticObject : Actor
{
    [SerializeField]
    private float _fgravity = 30.0f;
    [SerializeField]
    private BoxCollider _boxcollider;
    [FMODUnity.EventRef]
    [SerializeField] private string _collideSound;

    private Vector3 _colliderSize;

    public ObjectHitSystem HitSystem { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        HitSystem = GetComponent<ObjectHitSystem>();
        HitSystem.OnHit += PlayHitSound;
        
        _colliderSize = _boxcollider.bounds.extents;
        _boxcollider.enabled = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (BuffController.ActiveBuffs.Exists(BuffObject.Type.Falling))
        {
            if (Rigidbody.position.y > _colliderSize.y + 0.1f)
            {
                Rigidbody.position -= new Vector3(0.0f, _fgravity, 0.0f) * Time.deltaTime;
            }
            else if (photonView.IsMine)
            {
                BuffController.ReleaseBuffsByType(BuffObject.Type.Falling);
                transform.position = new Vector3(transform.position.x, _colliderSize.y, transform.position.z);
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

    private void PlayHitSound(HitInfo info)
    {
        Debug.Log("PlayHitSound", gameObject);

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