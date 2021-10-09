using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 파편 지형을 만들어 주는 클래스 입니다.
/// </summary>
public class FragmentAreaManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _fregmentFrefab;
    [SerializeField]
    private float _shardsOfDebris;

    private List<FragmentAreaInfo> _currentfragmentList = new List<FragmentAreaInfo>();
    private List<FragmentAreaInfo> _fragmentList = new List<FragmentAreaInfo>();

    private Queue<FragmentAreaInfo> _currentFragmentremoveQue = new Queue<FragmentAreaInfo>();
    private Queue<FragmentAreaInfo> _fragmentremoveQue = new Queue<FragmentAreaInfo>();



    private class FragmentAreaInfo
    {

        public GameObject _fragment;
        public float _range;

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
    public GameObject AddFragmentList(Transform trans, float range)
    {
        var clone = Instantiate(_fregmentFrefab, trans.position, trans.rotation);
        clone.transform.localScale = trans.localScale;
        _currentfragmentList.Add(new FragmentAreaInfo(clone, range));
        return clone;
    }

    public GameObject AddFragmentList(Vector3 trans, float range)
    {
        var clone = Instantiate(_fregmentFrefab, trans, Quaternion.identity);
        clone.transform.localScale = new Vector3(range, 1.0f, range);
        _currentfragmentList.Add(new FragmentAreaInfo(clone, range));
        return clone;
    }



    public bool CreateFragmentCheck(Vector3 Pos)
    {
        foreach (var item in _currentfragmentList)
        {
            var FragmentPos = item._fragment.transform.position;
            if (Vector3.Distance(Pos, FragmentPos) < item._range)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 파편지대 삭제
    /// </summary>
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
            Destroy(removeobj._fragment);
            _currentfragmentList.Remove(removeobj);
        }
        while (_fragmentremoveQue.Count != 0)
        {
            var removeobj = _fragmentremoveQue.Dequeue();
            Destroy(removeobj._fragment);
            _fragmentList.Remove(removeobj);
        }
    }
    /// <summary>
    /// 주변에 이미 파편지대가 있는지 확인후 맞는 이벤트를 적용하는함수입니다.
    /// </summary>
    public void SetFragmentAreaActive(Vector3 pos, float range)
    {
        var CreatefragmentPosList = new Queue<Vector3>();

        foreach (var currentFragment in _currentfragmentList)
        {

            bool _isCheck = false;
            var currentFragmentPos = currentFragment._fragment.transform.position;

            if (Vector3.Distance(currentFragmentPos, pos) > range)
            {
                _currentFragmentremoveQue.Enqueue(currentFragment);
                continue;
            }
            foreach (var Fragment in _fragmentList)
            {
                if (!Fragment._fragment.GetComponent<FragmentArea>().GetActive())
                {
                    _fragmentremoveQue.Enqueue(Fragment);
                    continue;
                }

                var FragmentPos = Fragment._fragment.transform.position;
                var HalfPos = (FragmentPos - currentFragmentPos) * 0.5f;
                if (Vector3.Distance(currentFragmentPos, FragmentPos) < currentFragment._range * 2)
                {
                    var CreatePos = currentFragment._fragment.transform.position + HalfPos;
                    CreatefragmentPosList.Enqueue(CreatePos);
                    _isCheck = true;
                }
            }


            if (!_isCheck)
                currentFragment._fragment.GetComponent<FragmentArea>().FragmentAreaActive();
            if (_isCheck)
                _currentFragmentremoveQue.Enqueue(currentFragment);

        }

        while (_currentFragmentremoveQue.Count != 0)
        {
            var removeobj = _currentFragmentremoveQue.Dequeue();
            Destroy(removeobj._fragment);
            _currentfragmentList.Remove(removeobj);
        }
        while (CreatefragmentPosList.Count != 0)
        {
            AddFragmentList(CreatefragmentPosList.Dequeue(), _shardsOfDebris).GetComponent<FragmentArea>().FragmentActive();
        }
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
            item._fragment.GetComponent<FragmentArea>().StartEvent();
        }
    }

    /// <summary>
    /// 던지기 함수 입니다.
    /// </summary>
    /// <param name="Buffstruct"></param>
    /// <param name="Target"></param>
    public void ThrowObject(BuffObject.BuffStruct Buffstruct, Vector3 Target)
    {
        foreach (var FragmentAare in _currentfragmentList)
        {
            FragmentAare._fragment.GetComponent<FragmentArea>().KnockBackObject(Buffstruct, Target);
        }
        //던지고 파편지대의 리스트로 옮깁니다.
        _fragmentList.AddRange(_currentfragmentList);
        _currentfragmentList.Clear();
    }

    /// <summary>
    ///
    /// </summary>
    public void CancelFragment()
    {
        foreach (var FragmentAare in _currentfragmentList)
        {
            FragmentAare._fragment.GetComponent<FragmentArea>().CancelEvent();
        }
    }

    public int FragmentCount()
    {
        return _currentfragmentList.Count;
    }

    public void FragmentDirection(Vector3 pos)
    {
        foreach (var FragmentAare in _currentfragmentList)
        {
            FragmentAare._fragment.GetComponent<FragmentArea>().DirectionLookAt(pos);
        }
    }
}
