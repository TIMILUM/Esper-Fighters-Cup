using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup
{
    public static class GameRoom
    {
        public const byte MaxPlayers = 2;

        public static readonly RoomOptions DefaultRoomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayers,
            PublishUserId = true,
            CustomRoomProperties = DefaultRoomCustomProperties
        };

        public static readonly Hashtable DefaultRoomCustomProperties = new Hashtable
        {
            [CustomPropertyKeys.GameStarted] = false,
            [CustomPropertyKeys.GameRound] = 0,
            [CustomPropertyKeys.GameState] = 0
        };
    }
}
