using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoomConnector : MonoBehaviourPunCallbacks
{
    public const byte MaxPlayersInRoom = 2;
    // public const string PropIsPrivateKey = "is_private";
    public const string PropPasswordKey = "password";

    public UnityEvent OnRoomListUpdated { get; private set; } = new UnityEvent();
    public UnityEvent OnRoomJoined { get; private set; } = new UnityEvent();

    public Dictionary<string, RoomInfo> CurrentRooms { get; private set; } = new Dictionary<string, RoomInfo>();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 마스터 서버와 연결합니다.
    /// </summary>
    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// 지정한 이름의 방에 입장합니다.
    /// </summary>
    /// <param name="roomName">방 이름</param>
    /// <param name="password">패스워드가 존재할 경우 패스워드를 사용하여 입장합니다.</param>
    /// <returns></returns>
    public bool JoinRoom(string roomName, string password = null)
    {
        if (!CurrentRooms.TryGetValue(roomName, out var cachedRoomInfo))
        {
            return false;
        }

        var roomPasswdHash = (string)cachedRoomInfo.CustomProperties[PropPasswordKey];

        // 룸의 패스워드가 empty일 경우 공개방임
        if (!string.IsNullOrEmpty(roomPasswdHash))
        {
            // password check
            if (password == null || password.ToSHA256Hash() != roomPasswdHash)
            {
                return false;
            }
        }

        return PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// 방을 새로 생성합니다.
    /// </summary>
    /// <param name="roomName">방 이름</param>
    /// <param name="password">패스워드. 만약 공개방을 생성하고 싶은 경우 null을 넘깁니다.</param>
    public void CreateRoom(string roomName, string password = null)
    {
        if (string.IsNullOrWhiteSpace(roomName))
        {
            throw new ArgumentException("방 이름은 비어 있을 수 없습니다.", nameof(roomName));
        }

        var roomOption = new RoomOptions
        {
            CustomRoomPropertiesForLobby = new string[] { PropPasswordKey },
            CustomRoomProperties = new Hashtable { { PropPasswordKey, password ?? string.Empty } }
        };

        PhotonNetwork.CreateRoom(roomName, roomOption);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버에 연결됨");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 입장");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                CurrentRooms.Remove(room.Name);
            }
            else
            {
                CurrentRooms[room.Name] = room;

            }
        }

        OnRoomListUpdated?.Invoke();
    }


    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("IngameScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("서버와 연결 종료됨");

        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            return;
        }

        SceneManager.LoadScene("LobbyScene");
    }
}
