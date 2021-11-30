using UnityEngine;

public abstract class ControllerObject<T> : ObjectBase where T : ControllerBase
{
    public bool IsRegistered { get; private set; }

    /// <summary>
    /// 컨트롤러 오브젝트의 주인 액터입니다.
    /// </summary>
    protected Actor Author { get; private set; }
    protected T Controller { get; private set; }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // Debug.Log(Author.photonView);
        if (Author.photonView.IsMine)
        {
            Controller.PlayerHitEnterEvent -= OnPlayerHitEnter;
        }
    }

    public void Register(T controller)
    {
        Controller = controller;
        Author = Controller.ControllerManager.GetActor();

        if (Author.photonView.IsMine)
        {
            Controller.PlayerHitEnterEvent += OnPlayerHitEnter;
        }

        IsRegistered = true;
        OnRegistered();
    }

    /// <summary>
    /// 컨트롤러 오브젝트의 Register가 호출된 후에 실행됩니다.
    /// </summary>
    protected virtual void OnRegistered()
    {
    }

    /// <summary>
    /// 플레이어가 아무 오브젝트와 충돌이 처음 일어날 때만 무조건 실행되는 함수입니다.
    /// </summary>
    /// <param name="other">충돌이 일어난 상대 오브젝트(게임 오브젝트면 무조건 다 받아옵니다.)</param>
    public abstract void OnPlayerHitEnter(GameObject other);
}
