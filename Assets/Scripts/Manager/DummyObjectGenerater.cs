using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup
{
    public class DummyObjectGenerater : MonoBehaviour
    {
        [SerializeField] private GameObject[] _dummyObjects;
        [SerializeField] private Transform[] _spawnPositions;

        public void RandomSpawnDummy()
        {
            if (!PhotonNetwork.OfflineMode)
            {
                return;
            }

            foreach (var spawn in _spawnPositions)
            {
                var randomDummy = _dummyObjects[Random.Range(0, _dummyObjects.Length)];
                var dummy = PhotonNetwork.Instantiate($"Prefab/StaticObject/{randomDummy.name}",
                    spawn.position, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));

                var staticObject = dummy.GetComponent<AStaticObject>();
                dummy.transform.position = new Vector3(dummy.transform.position.x, staticObject.ColliderSize.y + 0.1f, dummy.transform.position.z);
            }
        }
    }
}
