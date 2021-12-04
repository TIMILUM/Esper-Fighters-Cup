using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup.UI.InGame.Skill
{
    public class PlankSlidingUI : SkillUI
    {

        

        private void Update()
        {
            if (Target.photonView.IsMine)
            {
                var TargetPos = Target.transform.position;
                var TargetForword = Target.transform.forward * 2.0f;


                transform.SetPositionAndRotation
                    (TargetPos + TargetForword, Target.transform.rotation);

                if (!Target.BuffController.ActiveBuffs.Exists(BuffObject.Type.Sliding))
                {
                    Destroy(gameObject);
                }
            }


        }
    }
}
