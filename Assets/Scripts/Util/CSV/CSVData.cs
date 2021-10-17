using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CSVData
{
    public enum Type
    {
        NONE, INT, FLOAT, BOOL, STRING
    }
    
    private Dictionary<string, List<object>> _data = new Dictionary<string, List<object>>();

    /// <summary>
    /// CSV 데이터를 가져옵니다.
    /// 캐스팅 시 float, bool, string만 허용됩니다.
    /// </summary>
    /// <param name="key">가져올 CSV 데이터 이름입니다.</param>
    /// <param name="result">성공적으로 가져온 CSV 데이터 리스트입니다.</param>
    /// <typeparam name="T">캐스팅 시 float, bool, string만 허용됩니다.</typeparam>
    /// <returns>성공적으로 가져왔는지의 유무입니다.</returns>
    public bool Get<T>(string key, out List<T> result)
    {
        if (!_data.TryGetValue(key, out var dataValue))
        {
            result = default;
            return false;
        }

        result = dataValue.Cast<T>().ToList();
        return result != null;
    }

    public void Set(string key, List<string> value)
    {
        if (value.Count <= 0)
        {
            return;
        }

        var type = GetType(value[0]);
        _data.Add(key, value.Select(x => ConvertCast(x, type)).ToList());
    }

    private static object ConvertCast(string value, CSVData.Type type)
    {
        return type switch
        {
            Type.INT => int.Parse(value),
            Type.FLOAT => float.Parse(value),
            Type.BOOL => value == "true",
            Type.STRING => value,
            _ => null
        };
    }

    private Type GetType(string value)
    {
        if (float.TryParse(value, out var resultFloat))
        {
            return Type.FLOAT;
        }
        
        if (value == "true" || value == "false")
        {
            return Type.BOOL;
        }
        
        return Type.STRING;
    }
}
