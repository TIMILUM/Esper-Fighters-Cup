using System.Collections;
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

    [SerializeField] private List<ChracterHpUIStruct> ChracterUIStruct;

    private void Update()
    {
        foreach (var data in ChracterUIStruct)
        {
            data.Text.text = data.Name + " : " + data.Chracter.CharacterHP.ToString();
        }
    }
}
