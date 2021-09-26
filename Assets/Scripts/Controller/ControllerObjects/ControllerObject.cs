using UnityEngine;

public abstract class ControllerObject : ObjectBase
{
    protected ControllerBase _controller;

    public virtual void Register(ControllerBase controller)
    {
        _controller = controller;
    }

    /// <summary>
    ///     종속된 컨트롤러를 탬플릿에 캐스팅 된 형식으로 안전하고 빠르게 얻어옵니다.
    /// </summary>
    /// <typeparam name="T">형변환 시킬 컨트롤러의 클래스 탬플릿명을 작성합니다</typeparam>
    protected T ControllerCast<T>() where T : ControllerBase
    {
        return (T)_controller;
    }

    /// <summary>
    ///     플레이어가 아무 오브젝트와 충돌이 처음 일어날 때만 무조건 실행되는 함수입니다.
    /// </summary>
    /// <param name="other">충돌이 일어난 상대 오브젝트(게임 오브젝트면 무조건 다 받아옵니다.)</param>
    public abstract void OnPlayerHitEnter(GameObject other);
}