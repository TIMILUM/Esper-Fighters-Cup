using UnityEngine;
using UnityEngine.SceneManagement;

namespace EsperFightersCup.UI
{
    public class GoToScene : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
