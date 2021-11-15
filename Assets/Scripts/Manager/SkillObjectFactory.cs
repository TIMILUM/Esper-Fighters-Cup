using System.Collections.Generic;
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
    private GameObject _uiPrefabs;


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

    public GameObject CreateSkillUI(string objectname, Vector3 pos)
    {
        GameObject clone = null;
        if (objectname == "DropUI")
            clone = PhotonNetwork.Instantiate($"Prefab/UI/InGame/{_uiPrefabs.name}", pos, Quaternion.identity);
        return clone;
    }



}
