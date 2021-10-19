using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVUtil
{
    public static string SplitRegex = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    public static string LineSplitRegex = @"\r\n|\n\r|\n|\r";
    public static char[] TrimChars = { '\"' };
    
    public static CSVData Parse(string csv)
    {
        var csvStringArray = Regex.Split(csv, LineSplitRegex);
        if (csvStringArray.Length <= 1)
        {
            return null;
        }
        
        var result = new CSVData();
        var rawData = new List<List<string>>();

        // 해더는 0부터 내용은 1부터
        foreach (var csvLine in csvStringArray)
        {
            if (csvLine.Length <= 0)
            {
                continue;
            }

            var line = Regex.Split(csvLine, SplitRegex).ToList();
            rawData.Add(line);
        }

        var rawSize = rawData[0].Count;
        for (var i = 0; i < rawSize; ++i)
        {
            var nameString = rawData[0][i];
            var columnList = new List<string>();
            for (var j = 1; j < rawData.Count; j++)
            {
                columnList.Add(rawData[j][i]);
            }
            result.Set(nameString, columnList);
        }

        return result;
    }

    /// <summary>
    /// [InspectorCSVUtilManager의 함수를 경유함] CSV 데이터를 불러옵니다. (절대 Awake()함수 단계에서 사용하지 마세요!)
    /// </summary>
    /// <param name="csvName">CSV 파일 이름</param>
    /// <returns>CSV 데이터</returns>
    public static CSVData GetData(string csvName)
    {
        return InspectorCSVUtilManager.GetData(csvName);
    }
}
