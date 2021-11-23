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
    ///     ??삵닏??븍뱜??ID 揶쏅???揶쎛筌왖????삵닏??븍뱜????밴쉐??롫뮉 ??λ땾??낅빍?? (CSV??ID揶쏅??????퉸 ??삵닏??븍뱜???븍뜄???븍┛ ???????밴쉐??뤿???щ빍??)
    ///     ??륁젟???袁⑹뒄??롢늺 ??λ땾??筌띾슢諭?????癒?쓺 ?얠눘堉김퉪??袁⑹뒄 ??곸뵠 獄쏅뗀以???륁젟??뤿????몃빍??
    /// </summary>
    /// <param name="objectId">??삵닏??븍뱜 ID</param>
    /// <param name="pos">??밴쉐 ?????/param>
    /// <returns>??밴쉐??野껊슣????삵닏??븍뱜</returns>
    public GameObject CreateSkillObject(int objectId, Vector3 pos)
    {
        // ??대선??? 筌륁궢釉?野껋럩??return null
        if (!IngameObjectIDParser.TryGetPrefabPackage(objectId, out var prefabPackage))
        {
            return null;
            Debug.Log(prefabPackage);
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
