using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : ControllerBase
{
    private APlayer _player = null;

    // Todo: 반드시 Static Util Class 형식으로 빼놓도록 수정해야한다!
    private readonly Dictionary<BuffObject.Type, BuffObject> _buffPrefabLists =
        new Dictionary<BuffObject.Type, BuffObject>();

    private readonly Dictionary<BuffObject.Type, List<BuffObject>> _buffObjects =
        new Dictionary<BuffObject.Type, List<BuffObject>>();

    private void Reset()
    {
        // 컨트롤러 타입 지정을 위해 Reset 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.BuffController);
    }

    private void Awake()
    {
        var prefabs = Resources.LoadAll<BuffObject>("Prefabs/BuffPrefabs");
        foreach (var buffObject in prefabs)
        {
            _buffPrefabLists.Add(buffObject.BuffType, buffObject);
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _player = _controllerManager.GetActor() as APlayer;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
        foreach (var buffPair in _buffObjects)
        {
            var buffList = buffPair.Value;
            for (var i = buffList.Count - 1; i >= 0; i--)
            {
                buffList[i].OnPlayerHitEnter(other);
            }
        }
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
    
    public BuffObject GenerateBuff(BuffObject.BuffStruct buffStruct)
    {
        var buffType = buffStruct.Type;
        if (!_buffPrefabLists.ContainsKey(buffType)) return null;
        if (!_buffObjects.ContainsKey(buffType)) _buffObjects.Add(buffType, new List<BuffObject>());
        var buffObjectList = _buffObjects[buffType];
        if (!buffStruct.AllowDuplicates && buffObjectList.Count > 0) return null;

        var prefab = _buffPrefabLists[buffType];
        var buffObject = Instantiate(prefab, transform);
        buffObject.Register(this);
        buffObject.SetBuffStruct(buffStruct);
        
        buffObjectList.Add(buffObject);
        return buffObject;
    }
}
