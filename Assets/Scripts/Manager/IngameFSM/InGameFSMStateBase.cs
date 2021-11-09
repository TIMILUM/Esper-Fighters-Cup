using UnityEngine;

public abstract class InGameFSMStateBase : InspectorFSMBase<IngameFSMSystem.State, IngameFSMSystem>
{
    [SerializeField, Tooltip("해당 상태일 때 활성화 하고자 하는 오브젝트를 담는 리스트입니다.(해당 상태에서 나가면 비활성화)")]
    private GameObject[] _activeGameObjects;

    [SerializeField, Tooltip("해당 상태일 때 비활성화 하고자 하는 오브젝트를 담는 리스트입니다.(해당 상태에서 나가면 활성화)")]
    private GameObject[] _inactiveGameObjects;

    public override void StartState()
    {
        ActiveObjects(_activeGameObjects, true);
        ActiveObjects(_inactiveGameObjects, false);
    }

    public override void EndState()
    {
        ActiveObjects(_activeGameObjects, false);
        ActiveObjects(_inactiveGameObjects, true);
    }

    protected override void ChangeState(IngameFSMSystem.State state)
    {
        FsmSystem.ChangeState(state);
    }

    private void ActiveObjects(GameObject[] objects, bool isActive = true)
    {
        foreach (var gameObj in objects)
        {
            gameObj.SetActive(isActive);
        }
    }
}
