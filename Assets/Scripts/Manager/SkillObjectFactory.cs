using Photon.Pun;
using UnityEngine;

public class SkillObjectFactory : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _stonePrefab;
    [SerializeField] private GameObject _fragmentStaticObjectPrefab;
    [SerializeField] private GameObject _dropStaticObjectPrefab;
    [SerializeField] private GameObject _windLoadingObjectPrefab;
    [SerializeField] private GameObject _skillRockStaticObjectPrefab;

    public GameObject CreateSkillObject(string objectname, Vector3 pos)
    {
        GameObject clone = null;
        if (objectname == "Stone")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{ _stonePrefab.name}", pos, Quaternion.identity);
        }
        else if (objectname == "Fragment")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_fragmentStaticObjectPrefab.name}", pos, Quaternion.identity);
        }
        else if (objectname == "DropObject")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_dropStaticObjectPrefab.name}", pos, Quaternion.identity);
        }
        else if (objectname == "WindLoadingObject")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_windLoadingObjectPrefab.name}", pos, Quaternion.identity);
        }
        else if (objectname == "SkillRockObj")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_skillRockStaticObjectPrefab.name}", pos, Quaternion.identity);
        }

        return clone;
    }

    public GameObject CreateSkillObject(string objectname, Vector3 pos, Quaternion rot)
    {

        GameObject clone = null;
        if (objectname == "Stone")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{ _stonePrefab.name}", pos, rot);
        }
        else if (objectname == "Fragment")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_fragmentStaticObjectPrefab.name}", pos, rot);
        }
        else if (objectname == "DropObject")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_dropStaticObjectPrefab.name}", pos, rot);
        }
        else if (objectname == "SkillRockObj")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_skillRockStaticObjectPrefab.name}", pos, rot);
        }
        else if (objectname == "WindLoadingObject")
        {
            clone = PhotonNetwork.Instantiate($"Prefab/StaticObject/{_windLoadingObjectPrefab.name}", pos, rot);
        }

        return clone;
    }

    /// <summary>
    /// ??????????????? ID ?????? ????????? ??????????????? ???????????? ???????????????. (CSV??? ID?????? ?????? ??????????????? ???????????? ????????? ?????????????????????.)
    /// ????????? ???????????? ????????? ?????? ???????????? ????????? ?????? ?????? ?????? ??????????????? ?????????.
    /// </summary>
    /// <param name="objectId">???????????? ID</param>
    /// <param name="pos">?????? ?????????</param>
    /// <returns>????????? ?????? ????????????</returns>
    public GameObject CreateSkillObject(int objectId, Vector3 pos, Vector3 rotation)
    {
        // ???????????? ?????? ?????? return null
        if (!IngameObjectIDParser.TryGetPrefabPackage(objectId, out var prefabPackage))
        {
            return null;
        }
        var clone = PhotonNetwork.Instantiate(prefabPackage.PrefabPath, pos, Quaternion.Euler(rotation));
        return clone;
    }

    public void DestroyObject(GameObject obj)
    {
        if (!obj)
        {
            return;
        }

        PhotonNetwork.Destroy(obj);
    }
}
