using System;
using System.Collections.Generic;
using System.Linq;
using EsperFightersCup.Net;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

using EventCode = EsperFightersCup.Net.EventCode;

public class BuffController : ControllerBase
{
    // dictionary로 해야할 필요가 있을까요?
    private readonly Dictionary<BuffObject.Type, List<BuffObject>> _buffObjects =
        new Dictionary<BuffObject.Type, List<BuffObject>>();

    // Todo: 반드시 Static Util Class 형식으로 빼놓도록 수정해야한다!
    private readonly Dictionary<BuffObject.Type, BuffObject> _buffPrefabLists =
        new Dictionary<BuffObject.Type, BuffObject>();

    private RaiseEventOptions _generateBuffEventOption;

    private void Awake()
    {
        var prefabs = Resources.LoadAll<BuffObject>("Prefab/Buff");
        foreach (var buffObject in prefabs)
        {
            _buffPrefabLists.Add(buffObject.BuffType, buffObject);
        }

        _generateBuffEventOption = new RaiseEventOptions
        {
            TargetActors = new int[] { photonView.Controller.ActorNumber }
        };
    }

    private void Reset()
    {
        // 컨트롤러 타입 지정을 위해 Reset 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.BuffController);
    }

    public List<BuffObject> GetBuff(BuffObject.Type buffType)
    {
        if (!_buffObjects.TryGetValue(buffType, out var result))
        {
            return null;
        }

        return result.Count == 0 ? null : result;
    }

    public void GenerateBuff(BuffObject.BuffStruct buffStruct)
    {
        if (photonView is null)
        {
            Debug.LogError("PhotonView가 없습니다.");
            return;
        }

        var id = Guid.NewGuid().ToString();
        var packet = buffStruct.ToBuffEvent(photonView.ViewID, id);

        // 로컬에서 메소드 호출 -> 해당 포톤뷰의 액터에서 이벤트 전달
        EventSender.Broadcast(in packet, SendOptions.SendUnreliable, _generateBuffEventOption);
        Debug.Log($"Send buff generate event - {id}");
    }

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

        // 버프 해제는 로컬에서만 진행
        buff.gameObject.SetActive(false);

        foreach (var buffs in _buffObjects)
        {
            var idx = buffs.Value.FindIndex(buff => buff.BuffId == buff.BuffId);
            if (idx != -1)
            {
                var targetBuff = buffs.Value[idx];
                buffs.Value.RemoveAt(idx);
                Destroy(targetBuff.gameObject);

                Debug.Log($"{photonView.gameObject.name}: {buff.BuffId} Buff released");
                return;
            }
        }
    }

    protected override void OnGameEventReceived(GameEventArguments args)
    {
        if (photonView is null)
        {
            return;
        }

        switch (args.Code)
        {
            case EventCode.BuffGenerate:
                HandleBuffGenerate(args);
                break;

            case EventCode.BuffRelease:
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
        Debug.Log("HandleBuffGenerate");

        var buffType = (BuffObject.Type)data.Type;
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

    }
}
