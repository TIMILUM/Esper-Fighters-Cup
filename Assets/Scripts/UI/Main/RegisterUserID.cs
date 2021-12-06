using System.Text.RegularExpressions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    [RequireComponent(typeof(GoToScene))]
    public class RegisterUserID : MonoBehaviour
    {
        [SerializeField] private InputField _idInput;

        // private const string NamePrefKey = "username";

        private void Start()
        {
            Debug.Assert(_idInput, gameObject);
            /*
            if (PlayerPrefs.HasKey(NamePrefKey))
            {
                var name = PlayerPrefs.GetString(NamePrefKey);
                _idInput.text = name;
            }
            */
        }

        public void Register()
        {
            var id = _idInput.text;

            // 영어랑 숫자만, 최대 10글자
            if (string.IsNullOrWhiteSpace(id) || id.Length > 10 || !Regex.IsMatch(id, "^[0-9a-zA-Z]*$"))
            {
                var popup = PopupManager.Instance.CreateNewBasicPopup();
                popup.Open("<color=red>아이디가 잘못되었습니다.</color>", "아이디는 영어와 숫자를 혼합하여 최대 10글자 제한입니다.");
                return;
            }

            PhotonNetwork.NickName = id;
            // PlayerPrefs.SetString(NamePrefKey, id);

            print($"ID: {PhotonNetwork.NickName}");
            GetComponent<GoToScene>().LoadScene("LobbyScene");
        }
    }
}
