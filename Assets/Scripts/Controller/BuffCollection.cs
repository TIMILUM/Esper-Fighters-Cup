using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IReadonlyBuffCollection : IReadOnlyCollection<KeyValuePair<BuffObject.Type, IReadOnlyList<BuffObject>>>
{
    /// <summary>
    /// 버프 콜렉션의 타입 목록
    /// </summary>
    IEnumerable<BuffObject.Type> BuffTypes { get; }

    /// <summary>
    /// 타입별로 필터링되어 있는 버프 목록
    /// </summary>
    IEnumerable<IReadOnlyList<BuffObject>> BuffObjectsByType { get; }

    /// <summary>
    /// 타입과 일치하는 버프 목록을 가져옵니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IReadOnlyList<BuffObject> this[BuffObject.Type type] { get; }

    /// <summary>
    /// 타입과 일치하는 버프가 있는지 확인합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool Exists(BuffObject.Type type);

    /// <summary>
    /// 해당 버프가 있는지 확인합니다.
    /// </summary>
    /// <param name="buff"></param>
    /// <returns></returns>
    bool Exist(BuffObject buff);

    /// <summary>
    /// 아이디와 일치하는 버프가 있는지 확인합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool Exist(string id);
}

public interface IBuffCollection
{
    /// <summary>
    /// 버프 콜렉션에 버프 오브젝트를 추가합니다.
    /// </summary>
    /// <param name="buff"></param>
    void Add(BuffObject buff);

    /// <summary>
    /// 해당 버프를 목록에서 제거합니다.
    /// </summary>
    /// <param name="buff"></param>
    /// <returns>목록에서 제거된 버프 오브젝트</returns>
    BuffObject Remove(BuffObject buff);

    /// <summary>
    /// 아이디와 일치하는 버프를 목록에서 제거합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>목록에서 제거된 버프 오브젝트</returns>
    BuffObject Remove(string id);

    /// <summary>
    /// 타입과 일치하는 버프 오브젝트들을 목록에서 제거합니다.
    /// </summary>
    /// <param name="type">제거할 버프들의 타입</param>
    IReadOnlyList<BuffObject> Clear(BuffObject.Type type);

    /// <summary>
    /// 모든 버프를 버프 콜렉션에서 제거합니다.
    /// </summary>
    /// <returns>제거된 버프 목록</returns>
    IReadOnlyList<BuffObject> Clear();
}

/// <summary>
/// 버프 타입에 맞는 리스트가 반드시 하나 존재하는 버프 콜렉션입니다.
/// </summary>
public class BuffCollection : IBuffCollection, IReadonlyBuffCollection
{
    private readonly Dictionary<BuffObject.Type, List<BuffObject>> _activeBuffs = new Dictionary<BuffObject.Type, List<BuffObject>>();

    public BuffCollection()
    {
        foreach (BuffObject.Type buffType in Enum.GetValues(typeof(BuffObject.Type)))
        {
            _activeBuffs.Add(buffType, new List<BuffObject>());
        }
    }

    public IReadOnlyList<BuffObject> this[BuffObject.Type type] => _activeBuffs[type];
    public IEnumerable<BuffObject.Type> BuffTypes => _activeBuffs.Keys;
    public IEnumerable<IReadOnlyList<BuffObject>> BuffObjectsByType => _activeBuffs.Values;
    public int Count => _activeBuffs.Count;

    public void Add(BuffObject buff)
    {
        if (!buff)
        {
            return;
        }

        _activeBuffs[buff.BuffType].Add(buff);
    }

    public BuffObject Remove(BuffObject buff)
    {
        if (!buff)
        {
            return null;
        }
        return Remove(buff.BuffId);
    }

    public BuffObject Remove(string id)
    {
        foreach (var buffs in _activeBuffs.Values)
        {
            var idx = buffs.FindIndex(buff => buff.BuffId == id);
            if (idx < 0)
            {
                continue;
            }
            var buff = buffs[idx];
            buffs.RemoveAt(idx);
            return buff;
        }
        return null;
    }

    public IReadOnlyList<BuffObject> Clear(BuffObject.Type type)
    {
        var removedBuffs = _activeBuffs[type].ToList();
        _activeBuffs[type].Clear();

        return removedBuffs;
    }

    public IReadOnlyList<BuffObject> Clear()
    {
        var removedBuffs = new List<BuffObject>();

        foreach (var buffsByType in _activeBuffs.Values)
        {
            removedBuffs.AddRange(buffsByType);
            buffsByType.Clear();
        }

        return removedBuffs;
    }

    public bool Exists(BuffObject.Type type)
    {
        return this[type].Count > 0;
    }

    public bool Exist(BuffObject buff)
    {
        return buff ? Exist(buff.BuffId) : false;
    }

    public bool Exist(string id)
    {
        foreach (var buffs in _activeBuffs.Values)
        {
            if (buffs.FindIndex(buff => buff.BuffId == id) > -1)
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerator<KeyValuePair<BuffObject.Type, IReadOnlyList<BuffObject>>> GetEnumerator()
    {
        return _activeBuffs
            .Select(x => new KeyValuePair<BuffObject.Type, IReadOnlyList<BuffObject>>(x.Key, x.Value))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
