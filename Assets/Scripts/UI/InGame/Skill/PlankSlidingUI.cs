using UnityEngine;

namespace EsperFightersCup.UI
{
    public class PlankSlidingUI : MonoBehaviour
    {
        private void Update()
        {
            if (!transform.parent.GetComponent<Actor>().BuffController.ActiveBuffs.Exists(BuffObject.Type.Sliding))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
