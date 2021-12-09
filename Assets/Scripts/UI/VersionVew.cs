using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup
{
    [RequireComponent(typeof(Text))]
    public class VersionVew : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Text>().text = PhotonNetwork.AppVersion;
        }
    }
}
