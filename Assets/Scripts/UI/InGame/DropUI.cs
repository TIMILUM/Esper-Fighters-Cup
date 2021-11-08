using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
namespace EsperFightersCup
{
    public class DropUI : MonoBehaviourPunCallbacks
    {
        private GameObject _object;
        [SerializeField]
        GameObject _perSceondUI;

        private Vector3 _objectStartPos;
        private float _startDistance;

        private Vector3 _currentScale;

        public void InitDropUI(GameObject obj)
        {
            StartCoroutine(ObjFallingBuffCheck());

            _object = obj;
            _objectStartPos = obj.transform.position;
        }



        public void Update()
        {



            if (photonView.IsMine)
            {
                if (_startDistance == 0)
                {
                    return;
                }

                var perSceond = Vector3.Distance(_objectStartPos, _object.transform.position) / _startDistance;


                if (perSceond > 0.9f)
                {
                    StopCoroutine(ObjFallingBuffCheck());
                    PhotonNetwork.Destroy(gameObject);
                }


                photonView.RPC("DropPreSeondRPC", RpcTarget.AllBuffered, perSceond);
            }

        }

        private IEnumerator ObjFallingBuffCheck()
        {
            _perSceondUI.transform.localScale = Vector3.zero;
            yield return new WaitForSeconds(0.3f);
            _startDistance = Vector3.Distance(_objectStartPos, transform.position);
            while (true)
            {

                if (_object.GetComponent<Actor>().BuffController.GetBuff(BuffObject.Type.Falling) == null)
                {
                    break;
                }
                yield return null;
            }
        }


        [PunRPC]
        public void DropPreSeondRPC(float Persceond)
        {
            _perSceondUI.transform.localScale = new Vector3(Persceond, Persceond, Persceond);

        }



    }
}
