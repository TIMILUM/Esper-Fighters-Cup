using System.Collections.Generic;
using EsperFightersCup;
using EsperFightersCup.Net;
using ExitGames.Client.Photon;
using UnityEngine;

public class ParticleManager : PunEventSingleton<ParticleManager>
{
    /// <summary>
    /// 각각의 파티클 종류 정보
    /// </summary>
    [System.Serializable]
    private class ParticleInfo
    {
        [SerializeField] private string _particleName;
        [SerializeField] private int _maxNum = 30;
        [Tooltip("파티클 라이프 타임입니다. (단위는 밀리세컨드 입니다.)")]
        [SerializeField] private int _lifeTime = 3000;

        [Header("[캐릭터 팔레트 옵션]")]

        [Tooltip("옵션을 체크하지 않으면 Default Particle이, 체크하면 팔레트 컬러 인덱스에 맞는 Palette Particle이 사용됩니다.")]
        [SerializeField] private bool _usePalette;
        [SerializeField] private GameObject _defaultParticle;
        [SerializeField] private GameObject _paletteParticle1;
        [SerializeField] private GameObject _paletteParticle2;

        public string Name => _particleName;
        public int MaxNum => _maxNum;
        public int LifeTime => _lifeTime;
        public bool UsePalette => _usePalette;
        public GameObject DefaultParticle => _defaultParticle;
        public GameObject PaletteParticle1 => _paletteParticle1;
        public GameObject PaletteParticle2 => _paletteParticle2;
    }

    /// <summary>
    /// 파티클에 필요한 변수
    /// </summary>
    private class Particle
    {
        private readonly Queue<Particle> _parent;



        public string Name { get; set; }
        public GameObject Object { get; set; }
        public float LifeTime { get; set; }
        public float StartTime { get; set; }



        public Particle(GameObject particleFrefab, float lifeTime, Queue<Particle> parent)
        {
            _parent = parent;
            Object = particleFrefab;
            LifeTime = lifeTime;
        }

        public void StartParticle(string particleName)
        {
            StartTime = Time.time;
            Name = particleName;
        }

        public void ReturnToPool()
        {
            Object.SetActive(false);
            _parent.Enqueue(this);
        }
    }

    /// <summary>
    /// 각각의 파티클 종류 정보
    /// </summary>
    [SerializeField]
    private List<ParticleInfo> _particleInfo = new List<ParticleInfo>();

    /// <summary>
    /// 파티클 Pool
    /// </summary>
    private readonly Dictionary<string, Queue<Particle>> _particleList = new Dictionary<string, Queue<Particle>>();

    /// <summary>
    /// 지금 사용중인 파티클
    /// </summary>
    private readonly List<Particle> _activeParticle = new List<Particle>();

    /// <summary>
    /// 삭제할 파티클 큐
    /// </summary>
    private readonly Queue<Particle> _removeParticle = new Queue<Particle>();

    protected override void Awake()
    {
        base.Awake();

        foreach (var info in _particleInfo)
        {
            if (!_particleList.ContainsKey(info.Name))
            {
                if (info.UsePalette)
                {
                    if (!_particleList.ContainsKey(info.Name + "0"))
                    {
                        // name1, name2
                        _particleList.Add($"{info.Name}0", new Queue<Particle>());
                        _particleList.Add($"{info.Name}1", new Queue<Particle>());
                    }
                    else
                    {
                        Debug.LogError(info.Name);
                    }
                }
                else
                {
                    _particleList.Add(info.Name, new Queue<Particle>());
                }
            }

            for (int i = 0; i < info.MaxNum; i++)
            {

                if (info.UsePalette)
                {
                    var clone1 = Instantiate(info.PaletteParticle1, transform);
                    clone1.SetActive(false);
                    var palette1Queue = _particleList[$"{info.Name}0"];
                    palette1Queue.Enqueue(new Particle(clone1, info.LifeTime, palette1Queue));

                    var clone2 = Instantiate(info.PaletteParticle2, transform);
                    clone2.SetActive(false);
                    var palette2Queue = _particleList[$"{info.Name}1"];
                    palette2Queue.Enqueue(new Particle(clone2, info.LifeTime, palette2Queue));
                }

                else
                {
                    var clone = Instantiate(info.DefaultParticle, transform);
                    clone.SetActive(false);
                    var queue = _particleList[info.Name];
                    queue.Enqueue(new Particle(clone, info.LifeTime, queue));
                }



            }
        }
    }

    private void Update()
    {
        // foreach에 List.remove함수 불러오면 에러 메시지 발생해서 큐를 만들어 삭제를 했습니다.
        foreach (var item in _activeParticle)
        {
            float currentTime = (Time.time - item.StartTime) * 1000;

            if (item.LifeTime == 0)
            {
                if (!item.Object.activeSelf)
                {
                    if (item.Object.transform.parent != transform)
                    {
                        item.Object.transform.SetParent(transform);
                    }

                    item.ReturnToPool();
                    _removeParticle.Enqueue(item);
                }
                continue;
            }


            if (currentTime >= item.LifeTime)
            {
                if (item.Object.transform.parent != transform)
                {
                    item.Object.transform.SetParent(transform);
                }

                item.ReturnToPool();
                _removeParticle.Enqueue(item);
            }
        }

        while (_removeParticle.Count != 0)
        {
            _activeParticle.Remove(_removeParticle.Dequeue());
        }
    }

    /// <summary>
    /// 모든 클라이언트에서 파티클을 실행합니다.<para/>
    /// 팔레트 스왑 이펙트인 경우 이벤트를 보낸 플레이어(로컬 플레이어)의 팔레트 컬러를 사용합니다.
    /// </summary>
    /// <param name="particleName"> 파티클 이름</param>
    /// <param name="pos"> 파티클 시작 위치</param>
    /// <param name="angle">파티클 앵글 </param>
    public void PullParticleSync(string particleName, Vector3 pos, Quaternion angle)
    {

        //이펙트가 -90.0f가 기준이기 때문에 -90.0f를 빼주었습니다.
        angle.eulerAngles = new Vector3(angle.eulerAngles.x - 90.0f, angle.eulerAngles.y, angle.eulerAngles.z);


        var args = new GameParticlePlayArguments(particleName, pos, angle.eulerAngles);
        EventSender.Broadcast(in args, SendOptions.SendReliable);
    }

    /// <summary>
    /// 모든 클라이언트에서 플레이어의 특정 트랜스폼의 자식으로 파티클을 실행합니다.<para/>
    /// 팔레트 스왑 이펙트인 경우 이벤트를 보낸 플레이어(로컬 플레이어)의 팔레트 컬러를 사용합니다.
    /// </summary>
    /// <param name="particleName">파티클 이름</param>
    /// <param name="attachIndex">파티클을 붙일 <see cref="APlayer.EffectTrans"/>의 인덱스 번호</param>
    public void PullParticleAttachedSync(string particleName, int attachIndex)
    {
        var args = new GameParticlePlayAttachedArguments(particleName, attachIndex);
        EventSender.Broadcast(in args, SendOptions.SendReliable);
    }

    /// <summary>
    /// 로컬 클라이언트에서 파티클을 실행합니다.<para/>
    /// 팔레트 스왑 이펙트인 경우 로컬 플레이어의 팔레트 컬러를 사용합니다.
    /// </summary>
    /// <param name="particleName"> 파티클 이름</param>
    /// <param name="pos"> 파티클 시작 위치</param>
    /// <param name="angle">파티클 앵글 </param>
    public void PullParticleLocal(string particleName, Vector3 pos, Quaternion angle)
    {
        var localPlayer = InGamePlayerManager.Instance.LocalPlayer;
        PlayParticle(localPlayer, particleName, pos, angle);
    }

    /// <summary>
    /// 로컬 클라이언트에서 로컬 플레이어의 특정 트랜스폼의 자식으로 파티클을 실행합니다.<para/>
    /// 팔레트 스왑 이펙트인 경우 로컬 플레이어의 팔레트 컬러를 사용합니다.
    /// </summary>
    /// <param name="particleName">파티클 이름</param>
    /// <param name="attachIndex">파티클을 붙일 <see cref="APlayer.EffectTrans"/>의 인덱스 번호</param>
    public void PullParticleAttachedLocal(string particleName, int attachIndex)
    {
        var localPlayer = InGamePlayerManager.Instance.LocalPlayer;
        PlayParticleAttached(localPlayer, particleName, attachIndex);
    }

    /// <summary>
    /// 로컬 클라이언트에서 대상 플레이어의 특정 트랜스폼의 자식으로 파티클을 실행합니다.<para/>
    /// 팔레트 스왑 이펙트인 경우 대상 플레이어의 팔레트 컬러를 사용합니다.
    /// </summary>
    /// <param name="particleName">파티클 이름</param>
    /// <param name="attachIndex">파티클을 붙일 <see cref="APlayer.EffectTrans"/>의 인덱스 번호</param>
    /// /// <param name="targetPlayer">파티클을 실행할 대상 플레이어</param>
    public void PullParticleAttachedLocal(string particleName, int attachIndex, APlayer targetPlayer)
    {
        PlayParticleAttached(targetPlayer, particleName, attachIndex);
    }

    protected override void OnGameEventReceived(GameEventArguments args)
    {
        switch (args.Code)
        {
            case EventCode.PlayParticle:
                PlayParticle(args);
                break;

            case EventCode.PlayParticleAttached:
                PlayParticleAttached(args);
                break;

            default:
                break;
        }
    }

    private void PlayParticle(GameEventArguments args)
    {
        var data = (GameParticlePlayArguments)args.EventData;

        var player = InGamePlayerManager.Instance.GamePlayers[args.Sender];
        if (player == null)
        {
            Debug.LogWarning("타겟 플레이어를 찾지 못했습니다.");
            return;
        }

        PlayParticle(player, data.Name, data.Position, Quaternion.Euler(data.Angle));
    }

    private void PlayParticle(APlayer player, string particleName, Vector3 pos, Quaternion angle)
    {
        var info = _particleInfo.Find(x => x.Name == particleName);
        if (info == null)
        {
            Debug.LogWarning($"{particleName}와 동일한 파티클 이름이 없습니다.");
            return;
        }

        string name = particleName;
        if (info.UsePalette)
        {
            name += player.PaletteIndex.ToString();
        }

        if (!_particleList.TryGetValue(name, out var particleQueue))
        {
            Debug.LogWarning($"{name}와 동일한 파티클 큐가 없습니다.");
            return;
        }

        if (particleQueue.Count == 0)
        {
            Debug.LogWarning("파티클이 없습니다.");
            return;
        }

        var clon = particleQueue.Dequeue();
        clon.Object.SetActive(true);

        clon.Object.transform.SetPositionAndRotation(pos, angle);
        clon.StartParticle(particleName);
        _activeParticle.Add(clon);
    }

    private void PlayParticleAttached(GameEventArguments args)
    {
        var data = (GameParticlePlayAttachedArguments)args.EventData;

        var player = InGamePlayerManager.Instance.GamePlayers[args.Sender];
        if (player == null)
        {
            Debug.LogWarning("타겟 플레이어를 찾지 못했습니다.");
            return;
        }

        PlayParticleAttached(player, data.Name, data.AttachIndex);
    }

    private void PlayParticleAttached(APlayer player, string particleName, int attachIndex)
    {
        var info = _particleInfo.Find(x => x.Name == particleName);
        if (info == null)
        {
            Debug.LogWarning("동일한 파티클 이름이 없습니다.");
            return;
        }

        string name = particleName;
        if (info.UsePalette)
        {
            name += player.PaletteIndex.ToString();
        }

        if (!_particleList.TryGetValue(name, out var particleQueue))
        {
            Debug.LogWarning("동일한 파티클 이름이 없습니다.");
            return;
        }

        if (particleQueue.Count == 0)
        {
            Debug.LogWarning("파티클이 없습니다.");
            return;
        }

        if (attachIndex >= player.EffectTrans.Count)
        {
            Debug.LogWarning($"대상 플레이어의 EffectTrans를 가져오지 못했습니다");
            return;
        }
        var trans = player.EffectTrans[attachIndex];

        var clon = particleQueue.Dequeue();
        clon.Object.SetActive(true);
        clon.Object.transform.SetParent(trans);
        clon.Object.transform.SetPositionAndRotation(trans.position, trans.rotation);
        clon.StartParticle(particleName);
        _activeParticle.Add(clon);
    }
}
