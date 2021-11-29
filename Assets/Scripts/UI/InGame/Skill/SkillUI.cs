using EsperFightersCup.Net;
using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup.UI.InGame.Skill
{
    public abstract class SkillUI : MonoBehaviour
    {
        public string Name { get; private set; }
        public Vector2 Position { get; private set; }
        public float RotationY { get; private set; }
        public Vector2 Scale { get; private set; }
        public float Duration { get; private set; }
        public int ViewID { get; private set; }
        public Actor Target { get; private set; }

        public void Init(GameUIPlayArguments args)
        {
            Name = args.Name;
            Position = args.Position;
            RotationY = args.RotationY;
            Scale = args.Scale;
            Duration = args.Duration;
            ViewID = args.ViewID;

            var defaultRot = transform.rotation.eulerAngles;

            transform.localPosition = new Vector3(Position.x, 0, Position.y);
            transform.localRotation = Quaternion.Euler(new Vector3(defaultRot.x, RotationY, defaultRot.z));
            transform.localScale = new Vector3(Scale.x, 1, Scale.y);

            if (ViewID > 0)
            {
                var pv = PhotonNetwork.GetPhotonView(ViewID);
                if (pv != null && pv.TryGetComponent<Actor>(out var actor))
                {
                    Target = actor;
                }
            }

            if (Duration > 0f)
            {
                Destroy(gameObject, Duration);
            }
        }
    }
}
