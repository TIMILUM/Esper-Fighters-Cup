using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjectFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject _stonePrefab;
    [SerializeField]
    private GameObject _FragmentStaticObjectPrefab;

    private List<GameObject> _objectList = new List<GameObject>();
    private List<GameObject> _compareList = new List<GameObject>();

    public GameObject CreateSkillObject(string objectname, Vector3 pos)
    {
        GameObject clone = null;
        if (objectname == "Stone")
            clone = Instantiate(_stonePrefab, pos, Quaternion.identity);
        if (objectname == "Fragment")
            clone = Instantiate(_FragmentStaticObjectPrefab, pos, Quaternion.identity);

        _objectList.Add(clone);
        return clone;
    }

    public List<GameObject> CompareSkillObject(Vector3 pos, float range)
    {
        _compareList.Clear();

        foreach (var item in _objectList)
        {
            if (Vector3.Distance(pos, item.transform.position) < range)
            {
                _compareList.Add(item);
            }
        }

        return _compareList;
    }

    public void RemoveSkillObject(GameObject remove)
    {
        if (!_objectList.Contains(remove))
        {
            _objectList.Remove(remove);
        }
    }
}
