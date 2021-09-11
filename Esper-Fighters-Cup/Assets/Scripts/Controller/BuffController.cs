using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : ControllerBase
{
    private APlayer _player = null;

    private readonly Dictionary<BuffObject.Type, BuffObject> _buffPrefabLists =
        new Dictionary<BuffObject.Type, BuffObject>();

    private readonly Dictionary<BuffObject.Type, List<BuffObject>> _buffObjects =
        new Dictionary<BuffObject.Type, List<BuffObject>>();

    private void Reset()
    {
        // 컨트롤러 타입 지정을 위해 Awake 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.BuffController);
    }

    private void Awake()
    {
        var prefabs = Resources.LoadAll<BuffObject>("Prefabs/SkillPrefabs");
        foreach (var buffObject in prefabs)
        {
            _buffPrefabLists.Add(buffObject.BuffType, buffObject);
        }
    }

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        _player = _controllerManager.GetActor() as APlayer;
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
    }

    public List<BuffObject> GetBuff(BuffObject.Type buffType)
    {
        if (!_buffObjects.ContainsKey(buffType)) return null;
        var result = _buffObjects[buffType];
        return result.Count == 0 ? null : result;
    }

    public void ReleaseBuff(BuffObject buffObject)
    {
        var type = buffObject.BuffType;
        if (!_buffObjects.ContainsKey(type)) return;
        _buffObjects[type].Remove(buffObject);
        Destroy(buffObject.gameObject);
    }

    public BuffObject GenerateBuff(BuffObject.Type buffType)
    {
        if (!_buffPrefabLists.ContainsKey(buffType)) return null;
        if (!_buffObjects.ContainsKey(buffType)) _buffObjects.Add(buffType, new List<BuffObject>());

        var prefab = _buffPrefabLists[buffType];
        var buffObject = Instantiate(prefab, transform);
        buffObject.Register(this);
        
        _buffObjects[buffType].Add(buffObject);
        return buffObject;
    }
}
