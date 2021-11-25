using Photon.Pun;
using UnityEngine;

public class SkillObjectFactory : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _stonePrefab;
    [SerializeField]
    private GameObject _fragmentStaticObjectPrefab;
    [SerializeField]
    private GameObject _dropStaticObjectPrefab;

    [SerializeField]
    private GameObject _DropuiPrefabs;
    [SerializeField]
    private GameObject _reverseuiPrefabs;



    public GameObject CreateSkillObject(string objectname, Vector3 pos)
    {

        GameObject clone = null;
        if (objectname == "Stone")
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{ _stonePrefab.name}", pos, Quaternion.identity);
        if (objectname == "Fragment")
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_fragmentStaticObjectPrefab.name}", pos, Quaternion.identity);
        if (objectname == "DropObject")
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_dropStaticObjectPrefab.name}", pos, Quaternion.identity);

        return clone;
    }

    /// <summary>
    ///     오브젝트의 ID 값을 가지고 오브젝트를 생성하는 함수입니다. (CSV의 ID값을 통해 오브젝트를 불러오기 때문에 생성되었습니다.)
    ///     수정이 필요하면 함수를 만든 사람에게 물어볼 필요 없이 바로 수정하셔도 됩니다.
    /// </summary>
    /// <param name="objectId">오브젝트 ID</param>
    /// <param name="pos">생성 포지션</param>
    /// <returns>생성된 게임 오브젝트</returns>
    public GameObject CreateSkillObject(int objectId, Vector3 pos)
    {
        // 얻어오지 못한 경우 return null
        if (!IngameObjectIDParser.TryGetPrefabPackage(objectId, out var prefabPackage))
        {
            return null;
        }
        var clone = PhotonNetwork.Instantiate(prefabPackage.PrefabPath, pos, Quaternion.identity);
        Debug.Log(prefabPackage);
        return clone;
    }

    public GameObject CreateSkillUI(string objectname, Vector3 pos)
    {
        GameObject clone = null;
        if (objectname == "DropUI")
            clone = PhotonNetwork.Instantiate($"Prefab/UI/InGame/{_DropuiPrefabs.name}", pos, Quaternion.identity);
        if (objectname == "ThrowUI")
            clone = PhotonNetwork.Instantiate($"Prefab/UI/InGame/{_reverseuiPrefabs.name}", pos, Quaternion.identity);
        return clone;
    }

    public void DestroyObj(GameObject obj)
    {
        PhotonNetwork.Destroy(obj);
    }





}
