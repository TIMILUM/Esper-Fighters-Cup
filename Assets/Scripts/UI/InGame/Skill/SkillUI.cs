using EsperFightersCup.Net;
using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup.UI
{
    public abstract class SkillUI : MonoBehaviour
    {
        public string Name { get; private set; }
        public Vector2 Position => new Vector2(transform.position.x, transform.position.z);
        public float RotationY => transform.rotation.eulerAngles.y;
        public Vector2 Scale => new Vector2(transform.localScale.x, transform.localScale.z);
        public float Duration { get; private set; }
        public Actor Target { get; private set; }

        public void Init(GameUIPlayArguments args)
        {
            Name = args.Name;
            Duration = args.Duration;

            var defaultRot = transform.rotation.eulerAngles;

            transform.localPosition = new Vector3(args.Position.x, 0, args.Position.y);
            transform.localRotation = Quaternion.Euler(new Vector3(defaultRot.x, args.RotationY, defaultRot.z));
            transform.localScale = new Vector3(args.Scale.x, 1, args.Scale.y);

            ChangeTarget(args.AuthorViewID);

            if (Duration > 0f)
            {
                Destroy(gameObject, Duration);
            }
        }

        public void ChangeTarget(int viewID)
        {
            if (viewID > 0)
            {
                var pv = PhotonNetwork.GetPhotonView(viewID);
                if (pv != null && pv.TryGetComponent<Actor>(out var actor))
                {
                    Target = actor;
                }
            }
        }

        public void SetPosition(Vector3 position)
        {
            var uiPos = new Vector3(position.x, transform.position.y, position.z);
            transform.position = uiPos;
        }

        public void SetRotation(Vector3 rotation)
        {
            var rot = transform.rotation.eulerAngles;
            var uiRot = Quaternion.Euler(rot.x, rotation.y, rot.z);
            transform.rotation = uiRot;
        }

        public void SetPositionAndRotation(Vector3 position, Vector3 rotation)
        {
            var uiPos = new Vector3(position.x, transform.position.y, position.z);

            var rot = transform.rotation.eulerAngles;
            var uiRot = Quaternion.Euler(rot.x, rotation.y, rot.z);
            transform.SetPositionAndRotation(uiPos, uiRot);
        }

        protected virtual void LateUpdate()
        {
            if (!Target && gameObject)
            {
                Destroy(gameObject);
            }
        }
    }
}
