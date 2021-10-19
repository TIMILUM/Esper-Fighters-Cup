using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SkillObjectFactory : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _stonePrefab;
    [SerializeField]
    private GameObject _fragmentStaticObjectPrefab;




    public GameObject CreateSkillObject(string objectname, Vector3 pos)
    {

        GameObject clone = null;
        if (objectname == "Stone")
            clone = PhotonNetwork.Instantiate("Prefabs/Environment/" + _stonePrefab.name, pos, Quaternion.identity);
        if (objectname == "Fragment")
            clone = PhotonNetwork.Instantiate("Prefabs/Environment/" + _fragmentStaticObjectPrefab.name, pos, Quaternion.identity);


        return clone;
    }



}