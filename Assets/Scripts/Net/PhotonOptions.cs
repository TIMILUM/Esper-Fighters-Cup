using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup
{
    public static class PhotonOptions
    {
        public const byte MaxPlayers = 2;

        public static readonly TypedLobby RandomMatchLobby = new TypedLobby("RandomMatch", LobbyType.Default);
        public static readonly TypedLobby CustomMatchLobby = new TypedLobby("CustomMatch", LobbyType.Default);

        public static readonly RoomOptions DefaultRoomOption = new RoomOptions
        {
            MaxPlayers = MaxPlayers,
            PublishUserId = true,
            CustomRoomProperties = DefaultCustomRoomProperties,
            BroadcastPropsChangeToAll = true,
            CleanupCacheOnLeave = true
        };
        public static readonly Hashtable DefaultCustomRoomProperties = new Hashtable
        {
            [CustomPropertyKeys.GameStarted] = false,
            [CustomPropertyKeys.GameRound] = 0,
            [CustomPropertyKeys.GameState] = 0,
            [CustomPropertyKeys.GameRoundWinner] = 0,
            [CustomPropertyKeys.GameWinner] = null,
            [CustomPropertyKeys.GameLooser] = null
        };

        public static readonly Hashtable DefaultCustomPlayerProperties = new Hashtable
        {
            [CustomPropertyKeys.PlayerCharacterType] = null,
            [CustomPropertyKeys.PlayerGameRematch] = false,
            [CustomPropertyKeys.PlayerWinPoint] = 0,
            [CustomPropertyKeys.PlayerPhotonView] = null
        };
    }
}
