using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class DropUI : MonoBehaviour
    {
        private GameObject _object;
        [SerializeField]
        GameObject _perSceondUI;

        private Vector3 _objectStartPos;
        private float _startDistance;

        public void InitDropUI(GameObject obj)
        {
            _object = obj;
            _objectStartPos = obj.transform.position;
            _startDistance = Vector3.Distance(_objectStartPos, transform.position);
        }

        public void Update()
        {


            var perSceond = Vector3.Distance(_objectStartPos, _object.transform.position) / _startDistance;
            _perSceondUI.transform.localScale = new Vector3(perSceond, perSceond, perSceond);

            StartCoroutine(ObjFallingBuffCheck());
            if (perSceond > 0.9f)
            {
                StopCoroutine(ObjFallingBuffCheck());
                Destroy(gameObject);
            }

        }

        private IEnumerator ObjFallingBuffCheck()
        {
            yield return new WaitForSeconds(0.03f);

            while (true)
            {
                if (_object.GetComponent<Actor>().BuffController.GetBuff(BuffObject.Type.Falling) == null)
                {
                    break;
                }
                yield return null;
            }
            Destroy(gameObject);
        }




    }
}
