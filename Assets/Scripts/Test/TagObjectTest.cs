using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace EsperFightersCup
{
    public class TagObjectTest : MonoBehaviourPunCallbacks
    {
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            var aplayer = targetPlayer.TagObject as APlayer;
            Debug.Log(aplayer != null ? $"APlayer is not null, {aplayer.gameObject.name}" : "APlayer is null");
        }
    }
}
