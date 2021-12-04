using EsperFightersCup.Net;
using EsperFightersCup.UI.Popup;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace EsperFightersCup.UI.Match
{
    public class RandomMatch : PunEventCallbacks
    {
        [SerializeField] private Text _matchingText;
        [SerializeField] private BasicPopup _popup;

        //FMOD 사운드 이미터를 가진 오브젝트 3종을 인스펙터에서 입력받습니다. (생성 시 사운드 재생. 제거 시 종료 처리)
        [SerializeField] private GameObject _matchingSound;
        [SerializeField] private GameObject _failSound;
        [SerializeField] private GameObject _successSound;

        private Coroutine _matchFaildTimer;

        private void Start()
        {
            _matchingText.text = "매칭 중...";
            PhotonNetwork.JoinLobby(PhotonOptions.RandomMatchLobby);

            CoroutineTimer.SetTimerOnce(StartRandomMatch, 1f);

            _matchingSound.SetActive(true); //매칭 시작시 오브젝트 액티브시켜 사운드 재생
        }

        private void StartRandomMatch()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PrintRandomMatchFaildMessage("서버와 연결되어 있지 않습니다", "TitleScene");
                Destroy(_matchingSound);    //매칭 종료에 따라 오브젝트 제거하여 매칭 사운드 제거
                _failSound.SetActive(true); //실패 사운드 오브젝트 액티브하여 사운드 재생
                return;
            }

            _matchFaildTimer = CoroutineTimer.SetTimerOnce(OnMatchFailed, 20f);
            GameMatchSystem.Instance.OnMatched += OnMatched;

            var result = PhotonNetwork.JoinRandomOrCreateRoom(
                roomOptions: PhotonOptions.DefaultRoomOption, typedLobby: PhotonOptions.RandomMatchLobby);

            if (!result)
            {
                PrintRandomMatchFaildMessage("랜덤 매칭에 실패했습니다", "LobbyScene");
                Destroy(_matchingSound);    //매칭 종료에 따라 오브젝트 제거하여 매칭 사운드 제거
                _failSound.SetActive(true); //실패 사운드 오브젝트 액티브하여 사운드 재생
                return;
            }
        }

        private void OnMatched()
        {
            _matchingText.text = "유저를 찾았습니다!";
            CoroutineTimer.Stop(ref _matchFaildTimer);
            Destroy(_matchingSound);    //매칭 종료에 따라 오브젝트 제거하여 매칭 사운드 제거
            _successSound.SetActive(true);  //성공 사운드 오브젝트 액티브하여 사운드 재생
        }

        private void OnMatchFailed()
        {
            PrintRandomMatchFaildMessage("유저를 찾지 못했습니다", "LobbyScene");
            Destroy(_matchingSound);    //매칭 종료에 따라 오브젝트 제거하여 매칭 사운드 제거
            _failSound.SetActive(true); //실패 사운드 오브젝트 액티브하여 사운드 재생
        }

        private void PrintRandomMatchFaildMessage(string cause, string nextScene)
        {
            _matchingText.text = cause;

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            CoroutineTimer.SetTimerOnce(() => SceneManager.LoadScene(nextScene), 2f);
        }
    }
}
