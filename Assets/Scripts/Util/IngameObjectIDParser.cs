using System.Collections.Generic;
using UnityEngine;

public class IngameObjectIDParser : MonoBehaviour
{

    static private Dictionary<int, IngameObjectPrefabPackage> s_ingameObjectPrefabs = new Dictionary<int, IngameObjectPrefabPackage>();

    [SerializeField, Tooltip("ID로 찾을 수 있는 모든 오브젝트의 경로를 작성해주시면 됩니다.")]
    private string[] _objectResourcePathList;

    private void Awake()
    {
        if (s_ingameObjectPrefabs.Count > 0)
        {
            return;
        }

        foreach (var path in _objectResourcePathList)
        {
            var resources = Resources.LoadAll<Actor>(path);
            foreach (var resource in resources)
            {
                if (s_ingameObjectPrefabs.ContainsKey(resource.ID))
                {
                    Debug.LogWarning("[IngameObjectIDParser] 중복되는 ID 값을 가진 오브젝트가 있습니다. (" + resource.ID + ")\n대상 경로 : " +
                                   path + "/" + resource.name);
                    continue;
                }

                s_ingameObjectPrefabs.Add(resource.ID, new IngameObjectPrefabPackage()
                {
                    PrefabActor = resource,
                    PrefabPath = path + "/" + resource.name
                });
            }
        }
    }

    /// <summary>
    ///     ID값을 통해 프리팹 패키지를 불러옵니다.
    ///     프리팹 패키지(IngameObjectPrefabPackage) 안에는 프리팹(PrefabActor)과 프리팹 경로(PrefabPath)가 있습니다.
    /// </summary>
    /// <param name="id">찾을 오브젝트 ID</param>
    /// <param name="result">찾은 오브젝트 프리팹 패키지</param>
    /// <returns>찾았는지의 여부</returns>
    public static bool TryGetPrefabPackage(int id, out IngameObjectPrefabPackage result)
    {
        return s_ingameObjectPrefabs.TryGetValue(id, out result);
    }

    public class IngameObjectPrefabPackage
    {
        public Actor PrefabActor { get; set; }
        public string PrefabPath { get; set; }
    }
}
