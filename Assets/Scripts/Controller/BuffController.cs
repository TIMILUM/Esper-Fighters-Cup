using System;
using System.Collections.Generic;
using System.Linq;
using EsperFightersCup.Net;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BuffController : ControllerBase, IOnEventCallback
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
        var packet = new GameBuffGeneratePacket(photonView.ViewID, id, buffStruct);
        PacketSender.Broadcast(in packet, SendOptions.SendReliable);
    }

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
            ReleaseBuff(buffObject.BuffId);
        }
    }

    /// <summary>
    /// 버프를 해제합니다.
    /// </summary>
    /// <param name="id">버프오브젝트 아이디</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void ReleaseBuff(string id)
    {
        if (photonView is null)
        {
            Debug.LogError("PhotonView가 없습니다.");
            return;
        }

        Debug.Log($"Send buff release event - {id}");
        var packet = new GameBuffReleasePacket(photonView.ViewID, id);
        PacketSender.Broadcast(in packet, SendOptions.SendReliable);
    }

    /// <summary>
    /// 버프 이벤트를 받아서 핸들링합니다.
    /// </summary>
    /// <param name="photonEvent"></param>
    public void OnEvent(EventData photonEvent)
    {
        if (photonView is null)
        {
            return;
        }

        switch (photonEvent.Code)
        {
            case GameProtocol.GameBuffGenerateEvent:
                HandleBuffGenerate(photonEvent);
                break;

            case GameProtocol.GameBuffReleaseEvent:
                HandleBuffRelease(photonEvent);
                break;
        }
    }

    private void HandleBuffGenerate(EventData received)
    {
        // 본인이 버프 생성 이벤트를 생성했는가
        // var player = PhotonNetwork.CurrentRoom.GetPlayer(sender);
        // Debug.Log(player?.NickName ?? "null");
        // var isMine = sender == photonView.ControllerActorNr;
        var packet = PacketSerializer.Deserialize<GameBuffGeneratePacket>((byte[])received.CustomData);

        // 이벤트는 이 스크립트가 붙은 모든 곳에서 실행되는데 처리는 ViewID가 같은 오브젝트만 처리
        if (packet.ViewID != photonView.ViewID)
        {
            return;
        }

        var buffType = packet.Type;
        if (!_buffPrefabLists.ContainsKey(buffType))
        {
            return;
        }

        if (!_buffObjects.ContainsKey(buffType))
        {
            _buffObjects.Add(buffType, new List<BuffObject>());
        }

        var buffObjectList = _buffObjects[buffType];
        if (!packet.AllowDuplicates && buffObjectList.Count > 0)
        {
            ReleaseBuff(buffType);
        }

        var prefab = _buffPrefabLists[buffType];
        var buffObject = Instantiate(prefab, transform);
        buffObject.name = packet.BuffId;
        buffObject.BuffId = packet.BuffId;
        buffObject.SetBuffStruct((BuffObject.BuffStruct)packet);
        buffObject.Register(this);

        buffObjectList.Add(buffObject);

        Debug.Log($"{photonView.gameObject.name}: {packet.BuffId} Buff generated via RaiseEvent");
    }

    /*
    private void HandleBuffGenerate(BuffObject.Type buffType)
    {
        if (!_buffPrefabLists.ContainsKey(buffType))
        {
            return;
        }

        if (!_buffObjects.ContainsKey(buffType))
        {
            _buffObjects.Add(buffType, new List<BuffObject>());
        }

        var prefab = _buffPrefabLists[buffType];
        var buffObject = Instantiate(prefab, transform);
        buffObject.Register(this);

        _buffObjects[buffType].Add(buffObject);
    }
    */

    private void HandleBuffRelease(EventData received)
    {
        var packet = PacketSerializer.Deserialize<GameBuffReleasePacket>((byte[])received.CustomData);
        if (packet.ViewID != photonView.ViewID)
        {
            return;
        }

        foreach (var buffs in _buffObjects)
        {
            var idx = buffs.Value.FindIndex(buff => buff.BuffId == packet.BuffId);
            if (idx != -1)
            {
                var targetBuff = buffs.Value[idx];
                buffs.Value.RemoveAt(idx);
                Destroy(targetBuff.gameObject);

                Debug.Log($"{photonView.gameObject.name}: {packet.BuffId} Buff released via RaiseEvent");
                return;
            }
        }
    }
}
