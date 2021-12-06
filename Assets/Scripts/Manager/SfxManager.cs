using System.Collections.Generic;
using EsperFightersCup.Net;
using ExitGames.Client.Photon;
using UnityEngine;

namespace EsperFightersCup
{
    public class SfxManager : PunEventSingleton<SfxManager>
    {
        [System.Serializable]
        private class Sfx
        {
            [SerializeField]
            private string _name;
            [SerializeField, FMODUnity.EventRef]
            private string _path;

            public string Name => _name;
            public string Path => _path;
        }

        [SerializeField]
        private Sfx[] _sfxList;

        private Dictionary<string, Sfx> _chache;

        protected override void Awake()
        {
            base.Awake();
            _chache = new Dictionary<string, Sfx>();
            foreach (var sfx in _sfxList)
            {
                if (_chache.ContainsKey(sfx.Name))
                {
                    continue;
                }
                _chache.Add(sfx.Name, sfx);
            }
        }

        public void PlaySFXSync(string name, Vector3 pos, string initParam = null, float paramValue = 0f)
        {
            var args = new GameSoundPlayArguments(name, pos, initParam ?? string.Empty, paramValue);
            EventSender.Broadcast(in args, SendOptions.SendReliable);
        }

        protected override void OnGameEventReceived(GameEventArguments args)
        {
            if (args.Code != EventCode.PlaySound)
            {
                return;
            }

            var data = (GameSoundPlayArguments)args.EventData;

            if (!_chache.TryGetValue(data.Name, out var sfx))
            {
                Debug.LogWarning($"{data.Name}과 일치하는 SFX를 찾지 못했습니다.");
                return;
            }

            var instance = FMODUnity.RuntimeManager.CreateInstance(sfx.Path);
            if (!string.IsNullOrWhiteSpace(data.InitParameter))
            {
                instance.setParameterByName(data.InitParameter, data.ParameterValue);
            }
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(data.Position));
            instance.start();
            instance.release();
        }
    }
}
