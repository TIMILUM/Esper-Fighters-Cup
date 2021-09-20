using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerObject : ObjectBase
{
    protected ControllerBase _controller = null;

    public virtual void Register(ControllerBase controller)
    {
        _controller = controller;
    }

    /**
     * 종속된 컨트롤러를 탬플릿에 캐스팅 된 형식으로 안전하고 빠르게 얻어옵니다.
     */
    protected T ControllerCast<T>() where T : ControllerBase
    {
        return (T)_controller;
    }

    /**
     * 플레이어가 아무 오브젝트와 충돌이 처음 일어날 때만 무조건 실행되는 함수입니다.
     */
    public abstract void OnPlayerHitEnter(GameObject other);
}
