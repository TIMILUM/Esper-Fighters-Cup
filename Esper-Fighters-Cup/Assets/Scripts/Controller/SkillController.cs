using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : ControllerBase
{
    private APlayer _player = null;
    private void Reset()
    {
        // 컨트롤러 타입 지정을 위해 Reset 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.SkillController);
    }
    
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        _player = _controllerManager.GetActor() as APlayer;
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
    }
}
