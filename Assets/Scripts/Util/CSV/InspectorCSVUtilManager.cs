using System.Collections.Generic;
using UnityEngine;

public class InspectorCSVUtilManager : MonoBehaviour
{
    /**
     * @Todo 기획자의 CSV 파싱 의도 및 범위를 파악하기 위해 Dictionary 키값이 string 형식으로 되어있습니다. CSV 사용 범위가 파악되면 해당 부분의 수정이 필요합니다.
     */
    private static readonly Dictionary<string, CSVData> _csvList = new Dictionary<string, CSVData>();

    [SerializeField]
    private string _csvPath = "CSV";

    private void Awake()
    {
        if (_csvList.Count > 0)
        {
            return;
        }

        var csvResources = Resources.LoadAll<TextAsset>(_csvPath);
        foreach (var csv in csvResources)
        {
            var data = CSVUtil.Parse(csv.text);
            _csvList.Add(csv.name, data);
        }
    }

    /// <summary>
    /// CSV 데이터를 불러옵니다. (절대 Awake()함수 단계에서 사용하지 마세요!)
    /// </summary>
    /// <param name="csvName">CSV 파일 이름</param>
    /// <returns>CSV 데이터</returns>
    public static CSVData GetData(string csvName)
    {
        return !_csvList.TryGetValue(csvName, out var result) ? null : result;
    }
}
