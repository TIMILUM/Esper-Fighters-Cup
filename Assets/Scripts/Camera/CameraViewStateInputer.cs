using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EsperFightersCup
{
    public class CameraViewStateInputer : MonoBehaviour
    {

        CameraMovement CameraComponent;  //카메라 무브먼트 스크립트 컴포넌트에 접근하기 위한 변수
        private int ViewStateInputed = 1;         //캐릭터 선택 씬에서 이 변수에 뷰 스테이트를 입력받음.  


        void Start()
        {
            CameraComponent = GetComponent<CameraMovement>();
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (ViewStateInputed == 1) { ViewStateInputed = 2; }
                else { ViewStateInputed = 1; }
                Debug.Log("뷰스테이트 설정 : " + ViewStateInputed);
            }

            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                CameraComponent.cameraViewState = ViewStateInputed;
            }

        }
    }
}
