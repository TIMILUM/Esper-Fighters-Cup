namespace EsperFightersCup.Net
{
    /// <summary>
    /// RaiseEvent로 받은 데이터를 제공합니다.
    /// </summary>
    public readonly struct GameEventArguments
    {
        /// <summary>
        /// 게임이벤트 코드
        /// </summary>
        public byte Code { get; }

        /// <summary>
        /// 게임이벤트를 보낸 ActorNumber
        /// </summary>
        public int Sender { get; }

        /// <summary>
        /// 게임이벤트 데이터
        /// </summary>
        public IGameEvent EventData { get; }

        public GameEventArguments(byte code, int sender, IGameEvent eventData)
        {
            Code = code;
            Sender = sender;
            EventData = eventData;
        }
    }
}
