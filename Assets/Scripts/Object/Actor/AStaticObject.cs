using UnityEngine;

public class AStaticObject : Actor
{
    // Start is called before the first frame update

    [SerializeField]
    private float _fgravity = 30.0f;
    [SerializeField]
    private BoxCollider _boxcollider;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (BuffController.ActiveBuffs.Exists(BuffObject.Type.Falling))
        {
            if (transform.position.y > _boxcollider.bounds.extents.y + 1.0f)
            {
                transform.position -= new Vector3(0.0f, _fgravity, 0.0f) * Time.deltaTime;
            }
            else
            {
                BuffController.ReleaseBuffsByType(BuffObject.Type.Falling);
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
