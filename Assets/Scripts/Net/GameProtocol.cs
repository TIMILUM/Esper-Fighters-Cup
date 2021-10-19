namespace EsperFightersCup.Net
{
    public static class GameProtocol
    {
        public const byte GameMatchEvent = 0x01;
        public const byte GamePlayerEvent = 0x02;
        public const byte GameBuffGenerateEvent = 0x03;
        public const byte GameBuffReleaseEvent = 0x04;
        public const byte GameParticleEvent = 0x05;
        public const byte GameFragmentAreaEvent = 0x06;

        /*
        public enum GamePlayer
        {
            Join,
            Ready,
            Leave
        }

        public enum GameSkill
        {
            OnReadyToUse,
            OnFrontDelay,
            OnUse,
            OnEndDelay,
            OnCanceled,
            OnRelease
        }
        */
    }
}
