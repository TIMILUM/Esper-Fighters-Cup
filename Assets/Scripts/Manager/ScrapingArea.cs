using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 파편 지형을 만들어 주는 클래스 입니다.
/// </summary>
public class ScrapingArea
{
    private class ScrapingAreaInfo
    {
        public GameObject _scraping;
        public float _range;

        public ScrapingAreaInfo(GameObject scraping, float range)
        {
            _scraping = scraping;
            _range = range;
        }
    }


    private List<ScrapingAreaInfo> _scrapingList = new List<ScrapingAreaInfo>();

    public void AddScrapingList(GameObject scraping, float range)
    {
        _scrapingList.Add(new ScrapingAreaInfo(scraping, range));
    }

    public bool ScrapingCampare(Vector3 pos)
    {
        foreach (var item in _scrapingList)
        {
            var ScrapingPos = item._scraping.transform.position;
            if (Vector3.Distance(pos, ScrapingPos) < item._range)
            {
                return false;
            }
        }
        return true;
    }

    public void AllSetActive(bool isActive)
    {
        foreach (var item in _scrapingList)
        {
            item._scraping.SetActive(isActive);
        }
    }

    public int ScrapingCount()
    {
        return _scrapingList.Count;
    }
}
