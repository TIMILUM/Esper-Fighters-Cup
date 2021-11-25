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

    protected virtual void Update()
    {
        if (BuffController.GetBuff(BuffObject.Type.Falling) != null)
        {
            if (transform.position.y > _boxcollider.bounds.extents.y + 1.0f)
            {
                transform.position -= new Vector3(0.0f, _fgravity, 0.0f) * Time.deltaTime;
            }
            else
                BuffController.ReleaseBuff(BuffObject.Type.Falling);
        }

        if (BuffController.GetBuff(BuffObject.Type.KnockBack) != null || BuffController.GetBuff(BuffObject.Type.Falling) != null)
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
