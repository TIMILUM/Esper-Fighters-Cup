using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeHitWallState : SawBladeFSMBase
{
    protected override void Initialize()
    {
        State = SawBladeFSMSystem.StateEnum.HitWall;
    }

    public override void StartState()
    {
        
    }

    public override void EndState()
    {
        
    }
}
