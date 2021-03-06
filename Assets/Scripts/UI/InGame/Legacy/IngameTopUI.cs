using EsperFightersCup;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class IngameTopUI : MonoBehaviourPunCallbacks
{
    [Header("Round")]
    [SerializeField]
    private Sprite[] _roundList;
    [SerializeField]
    private Image _roundCount;

    [Header("Player Infos")]
    [SerializeField]
    private IngamePlayerUI[] _playerUIList;

    private void Start()
    {
        SetRoundCount(1);
    }

    private void Update()
    {
        var players = InGamePlayerManager.Instance != null ? InGamePlayerManager.Instance.GamePlayers : null;
        if (players is null)
        {
            return;
        }

        foreach (var aplayer in players.Values)
        {
            if (aplayer == null)
            {
                continue;
            }

            // TODO: 플레이어가 직접 UI에 등록할 수 있도록 변경해야 함
            var index = InGamePlayerManager.FindPlayerIndex(aplayer.photonView.Controller);
            var ui = _playerUIList[index].transform;

            // BUG: 얘네 자식 오브젝트 순서 바뀌면 못알아봄
            var hpbar = ui.GetChild(1).GetComponent<Image>();
            hpbar.fillAmount = aplayer.HP / 100f;

            var nickname = ui.GetChild(2).GetComponent<Text>();
            nickname.text = aplayer.photonView.Controller.NickName ?? "unknown";
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameRound, out var value))
        {
            var round = (int)value;
            if (round > _roundList.Length)
            {
                SetRoundCount(_roundList.Length);
            }
            else
            {
                SetRoundCount(round);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.TryGetValue(CustomPropertyKeys.PlayerWinPoint, out var value))
        {
            var winPoint = (int)value;
            var ui = _playerUIList[targetPlayer.ActorNumber - 1].transform.Find("VictoryCount");
            for (int i = 0; i < ui.childCount; i++)
            {
                ui.GetChild(i).gameObject.SetActive(i < winPoint);
            }
        }
    }

    public void SetRoundCount(int round)
    {
        _roundCount.sprite = _roundList[round - 1];
    }

    public void SetPlayer(APlayer player)
    {
        foreach (var playerUI in _playerUIList)
        {
            if (playerUI.Player != null)
            {
                continue;
            }

            playerUI.Player = player;
            return;
        }
    }
}
