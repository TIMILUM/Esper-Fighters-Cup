using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//간단한 오브젝트

[RequireComponent(typeof(Rigidbody))]
public class SimpleObject : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rig;
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
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
