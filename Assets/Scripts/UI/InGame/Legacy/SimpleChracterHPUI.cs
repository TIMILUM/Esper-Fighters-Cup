using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChracterHpUIStruct
{
    [SerializeField] private Text _text;
    [SerializeField] private APlayer _player;
    [SerializeField] private string _name;

    public Text Text => _text;
    public APlayer Player => _player;
    public string Name => _name;
}

public class SimpleChracterHPUI : MonoBehaviour
{

    [SerializeField] private List<ChracterHpUIStruct> _characterUIStruct;

    private void Update()
    {
        foreach (var data in _characterUIStruct)
        {
            data.Text.text = data.Name + " : " + data.Player.HP.ToString();
        }
    }
}
