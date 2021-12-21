using ExitGames.Client.Photon;
using Photon.Realtime;

namespace EsperFightersCup
{
    public static class CustomPropertyExtensions
    {
        /// <summary>
        /// 단일 키와 값으로 CustomProperty를 저장합니다.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetCustomProperty(this Player player, string key, object value)
        {
            var changeProp = new Hashtable { [key] = value };
            return player.SetCustomProperties(changeProp);
        }

        /// <summary>
        /// CAS (Check And Swap) 기능을 사용하여 CustomProperty를 저장합니다.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetCustomPropertyBySafe(this Player player, string key, object value)
        {
            var changeProp = new Hashtable { [key] = value };
            var oldProp = new Hashtable { [key] = player.CustomProperties[key] };
            return player.SetCustomProperties(changeProp, oldProp);
        }

        /// <summary>
        /// 단일 키와 값으로 CustomProperty를 저장합니다.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetCustomProperty(this Room room, string key, object value)
        {
            var changeProp = new Hashtable { [key] = value };
            return room.SetCustomProperties(changeProp);
        }

        /// <summary>
        /// CAS (Check And Swap) 기능을 사용하여 CustomProperty를 저장합니다.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetCustomPropertyBySafe(this Room room, string key, object value)
        {
            var changeProp = new Hashtable { [key] = value };
            var oldProp = new Hashtable { [key] = room.CustomProperties[key] };
            return room.SetCustomProperties(changeProp, oldProp);
        }
    }
}
