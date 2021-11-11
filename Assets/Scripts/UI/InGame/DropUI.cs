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
        private GameObject _dropUISceondUI;
        [SerializeField]
        private GameObject _perSceondUI;



        [SerializeField]
        private GameObject _enemyPerdropUISceondUI;
        [SerializeField]
        private GameObject _enemyPerSceondUI;

        private GameObject _currentUI;

        private Vector3 _objectStartPos;
        private float _startDistance;

        private Vector3 _currentScale;


        public void Start()
        {
            if (photonView.IsMine)
            {
                _dropUISceondUI.SetActive(true);
                _currentUI = _perSceondUI;
                _currentUI.transform.localScale = Vector3.zero;

            }
            else
            {
                _enemyPerdropUISceondUI.SetActive(true);
                _currentUI = _enemyPerSceondUI;
                _currentUI.transform.localScale = Vector3.zero;

            }
        }
        public void InitDropUI(GameObject obj)
        {

            _object = obj;
            _objectStartPos = obj.transform.position;

            StartCoroutine(ObjFallingBuffCheck());

        }



        public void Update()
        {



            if (photonView.IsMine)
            {
                if (_startDistance == 0)
                {
                    return;
                }
                if (_object == null)
                {
                    PhotonNetwork.Destroy(gameObject);
                    return;
                }


                var perSceond = Vector3.Distance(_objectStartPos, _object.transform.position) / _startDistance;


                if (perSceond > 0.9f)
                {
                    StopCoroutine(ObjFallingBuffCheck());
                    PhotonNetwork.Destroy(gameObject);
                }


                photonView.RPC(nameof(DropPreSeondRPC), RpcTarget.AllBuffered, perSceond);
            }

        }

        private IEnumerator ObjFallingBuffCheck()
        {

            yield return new WaitForSeconds(0.3f);
            _currentUI.transform.localScale = Vector3.zero;
            _startDistance = Vector3.Distance(_objectStartPos, transform.position);
            while (true)
            {

                if (_object.GetComponent<Actor>().BuffController.GetBuff(BuffObject.Type.Falling) == null)
                {
                    break;
                }
                yield return null;
            }
            PhotonNetwork.Destroy(gameObject);

        }


        [PunRPC]
        public void DropPreSeondRPC(float Persceond)
        {
            _currentUI.transform.localScale = new Vector3(Persceond, Persceond, Persceond);

        }



    }
}
