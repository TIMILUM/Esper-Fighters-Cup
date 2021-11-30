using UnityEngine;

public class AStaticObject : Actor
{
    // Start is called before the first frame update

    [SerializeField]
    private float _fgravity = 30.0f;
    [SerializeField]
    private BoxCollider _boxcollider;
    private Vector3 _colliderSize;
    protected override void Start()
    {
        _colliderSize = _boxcollider.bounds.extents;
        _boxcollider.enabled = false;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (BuffController.ActiveBuffs.Exists(BuffObject.Type.Falling))
        {
            if (transform.position.y > _colliderSize.y + 0.03f)
            {
                transform.position -= new Vector3(0.0f, _fgravity, 0.0f) * Time.deltaTime;
            }
            else
            {
                BuffController.ReleaseBuffsByType(BuffObject.Type.Falling);
                transform.position = new Vector3(transform.position.x, _colliderSize.y, transform.position.z);
            }
        }

        if (BuffController.ActiveBuffs.Exists(BuffObject.Type.KnockBack) || BuffController.ActiveBuffs.Exists(BuffObject.Type.Falling))
        {
            if (GetComponent<Rigidbody>().isKinematic)
            {
                GetComponent<Rigidbody>().isKinematic = false;
            }
            return;
        }

        if (!GetComponent<Rigidbody>().isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
