using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class DestroyAfterSecond : MonoBehaviour
    {

        [SerializeField] private float delayTime;

        void Start()
        {
            Invoke("DeleteThisOBJ", delayTime);
        }

        void DeleteThisOBJ()
        {
            Destroy(this.gameObject);
        }

    }
}
