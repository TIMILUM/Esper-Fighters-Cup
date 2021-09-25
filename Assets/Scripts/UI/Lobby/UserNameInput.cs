using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class UserNameInput : MonoBehaviour
{
    private const string NamePrefKey = "username";

    private InputField _nameInput;
    private bool _canSubmit;

    private void Start()
    {
        _nameInput = GetComponent<InputField>();

        if (PlayerPrefs.HasKey(NamePrefKey))
        {
            var name = PlayerPrefs.GetString(NamePrefKey);
            _nameInput.text = name;
            PhotonNetwork.NickName = name;
        }
    }

    public void SetUserName(string name)
    {
        var check = true;

        if (string.IsNullOrWhiteSpace(name))
        {
            check = false;
        }
        else if (name.Length > 15)
        {
            check = false;
        }

        _canSubmit = check;
    }

    public void SaveUserName(string name)
    {
        if (!_canSubmit)
        {
            return;
        }

        PhotonNetwork.NickName = name;
        PlayerPrefs.SetString(NamePrefKey, name);
        print(name);
    }
}
