using UnityEngine;

public class RoomList : MonoBehaviour
{
    [SerializeField] private GameObject _contents;
    [SerializeField] private GameObject _roomEnterButtonPrefab;

    private RoomConnector _roomConnector;

    private void Start()
    {
        Debug.Assert(_contents);
        Debug.Assert(_roomEnterButtonPrefab);
        _roomConnector = FindObjectOfType<RoomConnector>();

        ClearRooms();
    }

    private void OnEnable()
    {
        if (!(_roomConnector is null))
        {
            _roomConnector.OnRoomListUpdated.AddListener(OnRoomListUpdated);
        }
    }

    private void OnDisable()
    {
        if (!(_roomConnector is null))
        {
            _roomConnector.OnRoomListUpdated.RemoveListener(OnRoomListUpdated);
        }
    }

    private void OnRoomListUpdated()
    {
        ClearRooms();
        var rooms = _roomConnector.CurrentRooms;

        foreach (var room in rooms)
        {
            var roomButton = Instantiate(_roomEnterButtonPrefab, _contents.transform);
            // 룸 정보 작성
        }
    }

    private void ClearRooms()
    {
        var childCount = _contents.transform.childCount;

        for (var i = childCount - 1; i >= 0; i--)
        {
            var child = _contents.transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }
}
