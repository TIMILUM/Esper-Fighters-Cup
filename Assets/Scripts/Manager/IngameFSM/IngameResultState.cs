using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EsperFightersCup
{
    public class IngameResultState : InGameFSMStateBase
    {
        protected override void Initialize()
        {
            State = IngameFSMSystem.State.Result;
        }

        public override void StartState()
        {
            base.StartState();
            SceneManager.LoadScene("ResultScene");
        }

        public override void EndState()
        {
            base.EndState();
        }
    }
}
