using UnityEngine;

namespace EsperFightersCup.Test
{
    public class EventInvokeTest : MonoBehaviour
    {
        public void PrintInvokeMessage(string message)
        {
            Debug.Log($"EventInvokeTest: {message}");
        }
    }
}
