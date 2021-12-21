namespace EsperFightersCup
{
    public class DropObject : AStaticObject
    {
        /*
        [FMODUnity.EventRef]
        [SerializeField] private string _hitGroundSound;

        private const string CreateSound = "event:/SFX/Elena_Skill/SFX_RandDrop";

        protected override void Start()
        {
            base.Start();
            AudioEmitter.Event = CreateSound;
            AudioEmitter.Play();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Floor"))
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(_hitGroundSound, gameObject);
                var pos = transform.position;
                pos.y = 0.03f;
                ParticleManager.Instance.PullParticleLocal("Break_Dust", pos, Quaternion.identity);
            }
        }

        public void DropAfterDuration(Vector2 size, int duration)
        {
            if (photonView.IsMine)
            {
                DropAfterDurationAsync(size, duration).Forget();
            }
        }

        private async UniTaskVoid DropAfterDurationAsync(Vector2 size, int duration)
        {
            // var ui = GameUIManager.Instance.PlayLocal()
            AudioEmitter.SetParameter("Step", 1f);
            await UniTask.Yield();
        }
        */
    }
}
