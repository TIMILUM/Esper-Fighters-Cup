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
            Type.BOOL => value == "true" || value == "TRUE",
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
        
        if (value == "true" || value == "TRUE")
        {
            return Type.BOOL;
        }
        
        return Type.STRING;
    }
}
