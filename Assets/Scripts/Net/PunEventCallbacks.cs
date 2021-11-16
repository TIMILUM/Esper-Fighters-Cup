using Photon.Pun;

namespace EsperFightersCup.Net
{
    /// <summary>
    /// <see cref="GameEventListener"/>로부터 이벤트를 받는 콜백메소드가 들어있습니다.
    /// </summary>
    public class PunEventCallbacks : MonoBehaviourPunCallbacks
    {
        public override void OnEnable()
        {
            base.OnEnable();
            GameEventListener.Instance.GameEventReceived += OnGameEventReceived;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (GameEventListener.Instance != null)
            {
                GameEventListener.Instance.GameEventReceived -= OnGameEventReceived;
            }
        }

        /// <summary>
        /// <see cref="GameEventListener"/>에서 이벤트를 받으면 해당 콜백 메소드를 통해 처리합니다.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnGameEventReceived(GameEventArguments args)
        {
        }
    }
}
