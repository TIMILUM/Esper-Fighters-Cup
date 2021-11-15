using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChracterHpUIStruct
{
    public Text Text;
    public ACharacter Chracter;
    public string Name;
}

public class SimpleChracterHPUI : MonoBehaviour
{

    [SerializeField] private List<ChracterHpUIStruct> _characterUIStruct;

    private void Update()
    {
        foreach (var data in _characterUIStruct)
        {
            data.Text.text = data.Name + " : " + data.Chracter.HP.ToString();
        }
    }
}
