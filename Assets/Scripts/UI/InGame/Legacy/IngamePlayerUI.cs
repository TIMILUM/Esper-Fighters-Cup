using UnityEngine;
using UnityEngine.UI;

public class IngamePlayerUI : MonoBehaviour
{
    [SerializeField]
    private APlayer _player;
    public APlayer Player
    {
        get => _player;
        set => _player = value;
    }

    [SerializeField]
    private Image _hp;

    [SerializeField]
    private Text _nickName;

    private void Update()
    {
        if (_player == null)
        {
            return;
        }

        _hp.fillAmount = _player.HP / 100.0f;
    }
}
