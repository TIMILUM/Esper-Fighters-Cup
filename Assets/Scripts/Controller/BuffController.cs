using System;
using System.Collections.Generic;
using System.Linq;
using EsperFightersCup.Net;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class BuffController : ControllerBase
{
    // dictionary로 해야할 필요가 있을까요?
    private readonly Dictionary<BuffObject.Type, List<BuffObject>> _buffObjects =
        new Dictionary<BuffObject.Type, List<BuffObject>>();

    // Todo: 반드시 Static Util Class 형식으로 빼놓도록 수정해야한다!
    private readonly Dictionary<BuffObject.Type, BuffObject> _buffPrefabLists =
        new Dictionary<BuffObject.Type, BuffObject>();

    private void Awake()
    {
        var prefabs = Resources.LoadAll<BuffObject>("Prefabs/BuffPrefabs");
        foreach (var buffObject in prefabs)
        {
            _buffPrefabLists.Add(buffObject.BuffType, buffObject);
        }
    }

    private void Reset()
    {
        // 컨트롤러 타입 지정을 위해 Reset 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.BuffController);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public List<BuffObject> GetBuff(BuffObject.Type buffType)
    {
        if (!_buffObjects.TryGetValue(buffType, out var result))
        {
            return null;
        }

        return result.Count == 0 ? null : result;
    }

    [Obsolete("호환되지 않는 메소드입니다.", true)]
    public void GenerateBuff(BuffObject.Type buffType)
    {
    }

    public void GenerateBuff(BuffObject.BuffStruct buffStruct)
    {
        if (photonView is null)
        {
            Debug.LogError("PhotonView가 없습니다.");
            return;
        }

        var id = $"{buffStruct.Type}{PhotonNetwork.ServerTimestamp}";
        var packet = new GameBuffGenerateEvent(photonView.ViewID, id, buffStruct);
        PacketSender.Broadcast(in packet, SendOptions.SendUnreliable);
        Debug.Log("Generate buff", gameObject);
    }

    /*
    [Obsolete("아이디를 통해 해제하는 방식을 사용해주세요.", true)]
    public void ReleaseBuff(BuffObject buffObject)
    {
        var type = buffObject.BuffType;
        if (!_buffObjects.ContainsKey(type))
        {
            return;
        }

        _buffObjects[type].Remove(buffObject);
        Destroy(buffObject.gameObject);
    }
    */

    /// <summary>
    /// 버프 타입과 일치하는 모든 버프를 해제합니다.
    /// </summary>
    /// <param name="buffType"></param>
    public void ReleaseBuff(BuffObject.Type buffType)
    {
        if (!_buffObjects.TryGetValue(buffType, out var buffList))
        {
            return;
        }

        foreach (var buffObject in buffList.ToList())
        {
            ReleaseBuff(buffObject);
        }
    }

    /// <summary>
    /// 버프를 해제합니다.
    /// </summary>
    /// <param name="id">버프오브젝트 아이디</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void ReleaseBuff(BuffObject buff)
    {
        if (photonView is null)
        {
            Debug.LogError("PhotonView가 없습니다.");
            return;
        }

        if (!buff)
        {
            throw new ArgumentNullException(nameof(buff));
        }

        Debug.Log($"Send buff release event - {buff.BuffId}");
        var packet = new GameBuffReleaseEvent(photonView.ViewID, buff.BuffId);

        buff.gameObject.SetActive(false);
        PacketSender.Broadcast(in packet, SendOptions.SendUnreliable);
    }

    protected override void OnGameEventReceived(GameEventArguments args)
    {
        base.OnGameEventReceived(args);
        if (photonView is null)
        {
            return;
        }

        switch (args.Code)
        {
            case GameProtocol.BuffGenerate:
                HandleBuffGenerate(args);
                break;

            case GameProtocol.BuffRelease:
                HandleBuffRelease(args);
                break;
        }
    }

    private void HandleBuffGenerate(GameEventArguments args)
    {
        var data = (GameBuffGenerateEvent)args.EventData;
        if (data.TargetViewID != photonView.ViewID) // 같은 ViewID에서 보낸 것인지 체크
        {
            return;
        }

        var buffType = data.Type;
        if (!_buffPrefabLists.ContainsKey(buffType))
        {
            return;
        }

        if (!_buffObjects.ContainsKey(buffType))
        {
            _buffObjects.Add(buffType, new List<BuffObject>());
        }

        var buffObjectList = _buffObjects[buffType];
        if (!data.AllowDuplicates && buffObjectList.Count > 0)
        {
            ReleaseBuff(buffType);
        }

        var prefab = _buffPrefabLists[buffType];
        var buffObject = Instantiate(prefab, transform);
        buffObject.name = data.BuffId;
        buffObject.BuffId = data.BuffId;
        buffObject.SetBuffStruct((BuffObject.BuffStruct)data);
        buffObject.Register(this);

        buffObjectList.Add(buffObject);

        Debug.Log($"{photonView.gameObject.name}: {data.BuffId} Buff generated via RaiseEvent");
    }

    private void HandleBuffRelease(GameEventArguments args)
    {
        var data = (GameBuffReleaseEvent)args.EventData;
        if (data.TargetViewID != photonView.ViewID) // 같은 ViewID에서 보낸 것인지 체크
        {
            return;
        }

        foreach (var buffs in _buffObjects)
        {
            var idx = buffs.Value.FindIndex(buff => buff.BuffId == data.BuffId);
            if (idx != -1)
            {
                var targetBuff = buffs.Value[idx];
                buffs.Value.RemoveAt(idx);
                Destroy(targetBuff.gameObject);

                Debug.Log($"{photonView.gameObject.name}: {data.BuffId} Buff released via RaiseEvent");
                return;
            }
        }
    }
}
