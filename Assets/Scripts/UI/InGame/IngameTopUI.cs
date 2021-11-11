using System.Collections;
using System.Collections.Generic;
using EsperFightersCup;
using UnityEngine;
using UnityEngine.UI;

public class IngameTopUI : MonoBehaviour
{
    [Header("Round")]
    [SerializeField]
    private Sprite[] _roundList;
    [SerializeField]
    private Image _roundCount;

    [Header("Player Infos")]
    [SerializeField]
    private IngamePlayerUI[] _playerUIList;
    
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
    
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
