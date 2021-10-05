using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 스킬에서 만들어내는 오브젝트를 관리하기 위해서 
/// 싱글톤으로 만들었습니다.
/// </summary>
public class InGameSkillManager : MonoBehaviour
{
    private static InGameSkillManager s_Instance;

    [SerializeField]
    private GameObject _scrapingFrefab;
    [SerializeField]
    private SkillObjectFactory _skillObjectfactory;

    private ScrapingArea _scrapingArea = new ScrapingArea();

    public static InGameSkillManager Instance
    {
        get
        {
            if (s_Instance == null)
                return null;

            return s_Instance;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;


    }

    public void AddScraping(Transform transform, float range)
    {
        var clone = Instantiate(_scrapingFrefab, transform.position, transform.rotation);
        clone.transform.localScale = transform.localScale;
        _scrapingArea.AddScrapingList(clone, range);
    }
    public bool ScrapingCampare(Vector3 pos)
    {
        return _scrapingArea.ScrapingCampare(pos);
    }
    public int ScrapingCount()
    {
        return _scrapingArea.ScrapingCount();
    }
    public void ScrapingAllSetActive(bool isActive = true)
    {
        _scrapingArea.AllSetActive(isActive);
    }

    public GameObject CreateSkillObject(string objectname, Vector3 pos)
    {
        return _skillObjectfactory.CreateSkillObject(objectname, pos);
    }
    public List<GameObject> CompareSkillObject(Vector3 pos, float range)
    {
        return _skillObjectfactory.CompareSkillObject(pos, range);
    }
    public void RemoveSkillObject(GameObject removeObject)
    {
        _skillObjectfactory.RemoveSkillObject(removeObject);
    }

}
