using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup.Net
{
    public class AnimatorSync : PunEventCallbacks, IPunObservable
    {
        public enum ParameterType
        {
            Float = 1,
            Int = 3,
            Bool = 4,
            Trigger = 9,
        }

        [Serializable]
        public class AnimatorSyncParameter
        {
            [SerializeField] private string _name;
            [SerializeField] private ParameterType _type;

            public string Name => _name;
            public ParameterType Type => _type;
        }

        [SerializeField] private Animator _animator;
        [SerializeField] private List<AnimatorSyncParameter> _parameters;

        private Actor _actor;
        private List<string> _syncTriggers;

        public Animator Animator => _animator;

        private void Awake()
        {
            _actor = GetComponent<Actor>();

            _syncTriggers = _parameters
                .Where(parameter => parameter.Type == ParameterType.Trigger)
                .Select(parameter => parameter.Name)
                .ToList();
        }

        public void SetFloat(string name, float value)
        {
            if (CheckAnimatorIsNotNull())
            {
                _animator.SetFloat(name, value);
            }
        }

        public void SetInt(string name, int value)
        {
            if (CheckAnimatorIsNotNull())
            {
                _animator.SetInteger(name, value);
            }
        }

        public void SetBool(string name, bool value)
        {
            if (CheckAnimatorIsNotNull())
            {
                _animator.SetBool(name, value);
            }
        }

        public void SetTrigger(string name)
        {
            _animator.SetTrigger(name);
            if (_syncTriggers.Contains(name))
            {
                var packet = new GameAnimatorTriggerSyncEvent(_actor.photonView.ViewID, name);
                EventSender.Broadcast(in packet, SendOptions.SendUnreliable, EventSendOptions.SendOthers);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (_animator == null || !photonView)
            {
                return;
            }

            if (stream.IsWriting)
            {
                foreach (var parameter in _parameters)
                {
                    switch (parameter.Type)
                    {
                        case ParameterType.Float:
                            stream.SendNext(_animator.GetFloat(parameter.Name));
                            break;
                        case ParameterType.Int:
                            stream.SendNext(_animator.GetInteger(parameter.Name));
                            break;
                        case ParameterType.Bool:
                            stream.SendNext(_animator.GetBool(parameter.Name));
                            break;
                        case ParameterType.Trigger:
                        default:
                            break;
                    }
                }
            }
            else
            {
                foreach (var parameter in _parameters)
                {
                    switch (parameter.Type)
                    {
                        case ParameterType.Float:
                            _animator.SetFloat(parameter.Name, (float)stream.ReceiveNext());
                            break;
                        case ParameterType.Int:
                            _animator.SetInteger(parameter.Name, (int)stream.ReceiveNext());
                            break;
                        case ParameterType.Bool:
                            _animator.SetBool(parameter.Name, (bool)stream.ReceiveNext());
                            break;
                        case ParameterType.Trigger:
                        default:
                            break;
                    }
                }
            }
        }

        protected override void OnGameEventReceived(GameEventArguments args)
        {
            if (args.Code != EventCode.AnimatorTriggerSync || !_animator || !_actor)
            {
                return;
            }

            var data = (GameAnimatorTriggerSyncEvent)args.EventData;
            if (data.ActorViewID == _actor.photonView.ViewID)
            {
                _animator.SetTrigger(data.Name);
            }
        }

        private bool CheckAnimatorIsNotNull()
        {
            if (_animator == null)
            {
                Debug.LogError("Animator가 지정되어 있지 않습니다!", gameObject);
                return false;
            }
            return true;
        }
    }
}
