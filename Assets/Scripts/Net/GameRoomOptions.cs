using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup
{
    public static class GameRoomOptions
    {
        public const byte MaxPlayers = 2;

        public static readonly RoomOptions DefaultRoomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayers,
            PublishUserId = true,
            CustomRoomProperties = DefaultRoomCustomProperties,
            BroadcastPropsChangeToAll = true
        };

        public static readonly Hashtable DefaultRoomCustomProperties = new Hashtable
        {
            [CustomPropertyKeys.GameStarted] = false,
            [CustomPropertyKeys.GameRound] = 0,
            [CustomPropertyKeys.GameState] = 0,
            [CustomPropertyKeys.GameRoundWinner] = 0,
            [CustomPropertyKeys.GameWinner] = null,
            [CustomPropertyKeys.GameLooser] = null
        };

        public static readonly Hashtable DefaultPlayerCustomProperties = new Hashtable
        {
            [CustomPropertyKeys.PlayerCharacterType] = null,
            [CustomPropertyKeys.PlayerGameRematch] = false,
            [CustomPropertyKeys.PlayerWinPoint] = 0,
            [CustomPropertyKeys.PlayerPhotonView] = null
        };
    }
}
