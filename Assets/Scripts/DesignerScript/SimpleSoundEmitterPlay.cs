using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{

    [RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
    public class SimpleSoundEmitterPlay : MonoBehaviour
    {
        private FMODUnity.StudioEventEmitter _emitter;

        void Start()
        {
            _emitter = GetComponent<FMODUnity.StudioEventEmitter>();
            _emitter.Play();
        }
    }
}
