using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace EsperFightersCup
{
    public class CameraViewStateInputer : MonoBehaviour
    {

        private string SceneNameChecker;
        private int ViewStateInputed = 1;   //1 = 60, 2 = 45.

        public void ViewStateInput_60()
        {
            ViewStateInputed = 1;
            Debug.Log("PlayerInput-ViewState: [1]");
        }

        public  void ViewStateInput_45()
        {
            ViewStateInputed = 2;
            Debug.Log("PlayerInput-ViewState: [2]");
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void Update()
        {
            SceneNameChecker = SceneManager.GetActiveScene().name;

            /*
            if (Input.GetKeyDown(KeyCode.K))
            {
                SceneManager.LoadScene("GameScene");
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (ViewStateInputed == 1) { ViewStateInputed = 2; }
                else { ViewStateInputed = 1; }
                Debug.Log("ViewStateInput : " + ViewStateInputed);
            }
            */

            if (SceneNameChecker == "GameScene")
            {
                GameObject CameraObject = GameObject.Find("Main Camera");
                CameraObject.GetComponent<CameraMovement>().CameraViewStateFirstInput(ViewStateInputed);
                Destroy(this.gameObject);
            }
        }
    }
}
