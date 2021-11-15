using System.Collections.Generic;
using EsperFightersCup.Net;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// 파편 지형을 만들어 주는 클래스 입니다.
/// </summary>
public class FragmentAreaManager : PunEventCallbacks
{
    [SerializeField]
    private GameObject _fragmentPrefab;
    [SerializeField]
    private float _shardsOfDebris;

    private List<FragmentAreaInfo> _currentfragmentList = new List<FragmentAreaInfo>();
    private List<FragmentAreaInfo> _fragmentList = new List<FragmentAreaInfo>();

    private Queue<FragmentAreaInfo> _currentFragmentremoveQue = new Queue<FragmentAreaInfo>();
    private Queue<FragmentAreaInfo> _fragmentremoveQue = new Queue<FragmentAreaInfo>();

    private class FragmentAreaInfo
    {
        [SerializeField] private GameObject _fragment;
        [SerializeField] private float _range;

        public GameObject Fragment => _fragment;
        public float Range => _range;

        public FragmentAreaInfo(GameObject fragment, float range)
        {
            _fragment = fragment;
            _range = range;
        }
    }

    /// <summary>
    /// 파편지대를 추가합니다.
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public GameObject AddFragmentList(Transform trans, float range, int actorViewID)
    {
        var clone = PhotonNetwork.Instantiate("Prefab/EnvironmentObject/" + _fragmentPrefab.name, trans.position, trans.rotation);
        clone.GetComponent<FragmentArea>().NotFloatObject(actorViewID);
        clone.transform.localScale = trans.localScale;
        _currentfragmentList.Add(new FragmentAreaInfo(clone, range));
        return clone;
    }

    public GameObject AddFragmentList(Vector3 trans, float range, int ActorViewID)
    {
        var clone = Instantiate(_fragmentPrefab, trans, Quaternion.identity);
        clone.GetComponent<FragmentArea>().NotFloatObject(ActorViewID);
        clone.transform.localScale = new Vector3(range, 1.0f, range);
        _currentfragmentList.Add(new FragmentAreaInfo(clone, range));
        return clone;
    }

    public bool CreateFragmentCheck(Vector3 pos)
    {
        foreach (var item in _currentfragmentList)
        {
            var fragmentPos = item.Fragment.transform.position;
            if (Vector3.Distance(pos, fragmentPos) < item.Range)
            {
                return false;
            }
        }
        return true;
    }

    public void AllDestory()
    {
        foreach (var item in _currentfragmentList)
        {
            _currentFragmentremoveQue.Enqueue(item);
        }
    }

    private void Update()
    {
        while (_currentFragmentremoveQue.Count != 0)
        {
            var removeobj = _currentFragmentremoveQue.Dequeue();
            PhotonNetwork.Destroy(removeobj.Fragment);
            _currentfragmentList.Remove(removeobj);
        }
        while (_fragmentremoveQue.Count != 0)
        {
            var removeobj = _fragmentremoveQue.Dequeue();
            PhotonNetwork.Destroy(removeobj.Fragment);
            _fragmentList.Remove(removeobj);
        }
    }

    /// <summary>
    /// 주변에 있는 파편지대 판별하여 파편뭉치 생성
    /// </summary>
    public void SetFragmentAreaActive(Vector3 pos, float range, int ActorViewID)
    {
        // PacketSender.Broadcast(new GameFragmentAreaGenEvent(ActorViewID, pos, range), SendOptions.SendUnreliable);
        // 아래 코드 HandleFragmentAreaEvent()로 옮기기
    }

    /// <summary>
    /// 주변에 뭐가 있는지 확인
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="range"></param>
    public void EventStart()
    {
        foreach (var item in _currentfragmentList)
        {
            item.Fragment.GetComponent<FragmentArea>().StartEvent();
        }
    }

    /// <summary>
    /// 던지기 함수 입니다.
    /// </summary>
    /// <param name="buffstruct"></param>
    /// <param name="target"></param>
    public void ThrowObject(BuffObject.BuffStruct buffstruct, Vector3 target)
    {
        _fragmentList.AddRange(_currentfragmentList);
        _currentfragmentList.Clear();
    }

    /// <summary>
    ///
    /// </summary>
    public void CancelFragment()
    {
        foreach (var fragmentAare in _currentfragmentList)
        {
            fragmentAare.Fragment.GetComponent<FragmentArea>().CancelEvent();
        }
    }

    public int FragmentCount()
    {
        return _currentfragmentList.Count;
    }

    public void CurrentFragmentAreaClear()
    {

        _fragmentList.AddRange(_currentfragmentList);
        _currentfragmentList.Clear();
    }

    protected override void OnGameEventReceived(GameEventArguments args)
    {
        if (args.Code != EventCode.FragmentAreaGen)
        {
            return;
        }

        var data = (GameFragmentAreaGenEvent)args.EventData;

        // SetFragmentAreaActive코드 실행
        var createfragmentPosList = new Queue<Vector3>();

        foreach (var currentFragment in _currentfragmentList)
        {
            bool isCheck = false;
            var currentFragmentPos = currentFragment.Fragment.transform.position;

            if (Vector3.Distance(currentFragmentPos, data.Position) > data.Range)
            {
                _currentFragmentremoveQue.Enqueue(currentFragment);
                continue;
            }

            foreach (var fragment in _fragmentList)
            {
                if (!fragment.Fragment.GetComponent<FragmentArea>().GetActive())
                {
                    _fragmentremoveQue.Enqueue(fragment);
                    continue;
                }

                var fragmentPos = fragment.Fragment.transform.position;
                var halfPos = (fragmentPos - currentFragmentPos) * 0.5f;
                if (Vector3.Distance(currentFragmentPos, fragmentPos) < currentFragment.Range * 2)
                {
                    var createPos = currentFragment.Fragment.transform.position + halfPos;
                    createfragmentPosList.Enqueue(createPos);
                    isCheck = true;
                }
            }

            if (isCheck)
            {
                _currentFragmentremoveQue.Enqueue(currentFragment);
            }
            else
            {
                currentFragment.Fragment.GetComponent<FragmentArea>().FragmentAreaActive();
            }
        }

        while (_currentFragmentremoveQue.Count != 0)
        {
            var removeobj = _currentFragmentremoveQue.Dequeue();
            PhotonNetwork.Destroy(removeobj.Fragment);
            _currentfragmentList.Remove(removeobj);
        }

        while (createfragmentPosList.Count != 0)
        {
            AddFragmentList(createfragmentPosList.Dequeue(), _shardsOfDebris, data.FragmentAuthorViewID).GetComponent<FragmentArea>().FragmentActive();
        }
    }
}
