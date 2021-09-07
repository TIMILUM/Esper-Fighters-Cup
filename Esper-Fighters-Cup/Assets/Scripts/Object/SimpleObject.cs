using UnityEngine;


//간단한 오브젝트

[RequireComponent(typeof(Rigidbody))]
public class SimpleObject : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rig;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        //Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (rig.velocity.magnitude > 5.0f && collision.transform.tag == "Player")
            Destroy(gameObject);
    }

}
