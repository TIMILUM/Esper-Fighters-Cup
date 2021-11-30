using System;
using System.Collections.Generic;
using EsperFightersCup.Net;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public sealed class BuffController : ControllerBase
{
    private readonly BuffCollection _activeBuffs = new BuffCollection();
    // 캐싱용 딕셔너리
    private readonly Dictionary<BuffObject.Type, BuffObject> _buffTable = new Dictionary<BuffObject.Type, BuffObject>();
    private readonly object _buffReleaseLock = new object();

    // TODO: 값을 올바르게 리턴하는지 체크
    public IReadonlyBuffCollection ActiveBuffs => _activeBuffs;

    protected override void Reset()
    {
        base.Reset();

        // 컨트롤러 타입 지정을 위해 Reset 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.BuffController);
    }

    protected override void Start()
    {
        base.Start();

        var prefabs = Resources.LoadAll<BuffObject>("Prefab/Buff");
        foreach (var buffObject in prefabs)
        {
            _buffTable.Add(buffObject.BuffType, buffObject);
        }
    }

    /// <summary>
    /// 버프를 생성합니다. RPC를 통해 모든 플레이어들에게 동기화됩니다.
    /// </summary>
    /// <param name="buffStruct">생성할 버프 정보</param>
    public void GenerateBuff(BuffObject.BuffStruct buffStruct)
    {
        var id = Guid.NewGuid().ToString("N");
        var args = buffStruct.ToBuffArguments(id);

        photonView.RPC(nameof(GenerateBuffRPC), RpcTarget.All, args);
    }

    /// <summary>
    /// 버프 타입과 일치하는 모든 버프를 해제합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    /// <param name="buffType">해제할 버프 타입</param>
    public void ReleaseBuffsByType(BuffObject.Type buffType)
    {
        photonView.RPC(nameof(ReleaseBuffsByTypeRPC), RpcTarget.All, (int)buffType);
    }

    /// <summary>
    /// 버프 오브젝트를 해제합니다. 버프 오브젝트의 ID를 기반으로 RPC를 통해 동기화됩니다.
    /// </summary>
    /// <param name="buff">해제할 버프 오브젝트</param>
    public void ReleaseBuff(BuffObject buff)
    {
        ReleaseBuff(buff.BuffId);
    }

    /// <summary>
    /// 아이디와 일치하는 버프를 해제합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    /// <param name="id">버프오브젝트 아이디</param>
    public void ReleaseBuff(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("해제할 버프의 ID가 비어 있어 버프를 해제하지 못했습니다.");
            return;
        }

        photonView.RPC(nameof(ReleaseBuffRPC), RpcTarget.All, id);
    }

    [PunRPC]
    private void GenerateBuffRPC(BuffGenerateArguments args)
    {
        var buffType = (BuffObject.Type)args.Type;
        if (!_buffTable.ContainsKey(buffType))
        {
            return;
        }

        var buffs = _activeBuffs[buffType];
        if (photonView.IsMine && !args.AllowDuplicates && buffs.Count > 0)
        {
            ReleaseBuffsByType(buffType);
        }

        var prefab = _buffTable[buffType];
        var buff = Instantiate(prefab, transform);
        buff.name = args.BuffId;
        buff.BuffId = args.BuffId;
        buff.SetBuffStruct((BuffObject.BuffStruct)args);

        _activeBuffs.Add(buff);
        Debug.Log($"Buff generate [{ControllerManager.Author.name}] [{buff.BuffType}] [{buff.BuffId}]", gameObject);
        buff.Register(this, null);
    }

    [PunRPC]
    private void ReleaseBuffsByTypeRPC(int buffType)
    {
        lock (_buffReleaseLock)
        {
            foreach (var targetBuff in _activeBuffs[(BuffObject.Type)buffType])
            {
                Debug.Log($"Buff release [{ControllerManager.Author.name}] [{targetBuff.BuffType}] [{targetBuff.BuffId}]", gameObject);
                targetBuff.Release();
            }
            _activeBuffs.Clear((BuffObject.Type)buffType);
        }
    }

    [PunRPC]
    private void ReleaseBuffRPC(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        lock (_buffReleaseLock)
        {
            var targetBuff = _activeBuffs.Remove(id);
            if (targetBuff is null)
            {
                Debug.LogWarning($"ID와 일치하는 버프를 찾지 못했습니다. ({id})");
                return;
            }

            Debug.Log($"Buff release [{ControllerManager.Author.name}] [{targetBuff.BuffType}] [{targetBuff.BuffId}]", gameObject);
            targetBuff.Release();
        }
    }
}
