using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class IngameMaterialChangeEvent : MonoBehaviour
{
    [SerializeField]
    private bool _isChangeWhenStart;

    [SerializeField]
    private float _defaultDurationSeconds = 0.5f;

    [Header("Changed Material")]
    [SerializeField]
    private Material _mineMaterial;

    [SerializeField]
    private Material _enemyMaterial;

    private Actor _actor;

    private Coroutine _changingCoroutine;
    private bool _isStopCoroutine;
    private Material _material;

    private Material _originalMaterial;
    private Renderer _renderer;

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        _renderer = GetComponentInChildren<Renderer>();
        _originalMaterial = GetComponent<Renderer>().sharedMaterial;

        if (_actor == null)
        {
            return;
        }

        _material = _actor.PaletteIndex == 0 ? _mineMaterial : _enemyMaterial;
    }

    private void Reset()
    {
        _originalMaterial = GetComponent<Renderer>().sharedMaterial;
    }

    private void Start()
    {
        if (_isChangeWhenStart)
        {
            Change();
        }
    }

    public bool Change()
    {
        return Change(_defaultDurationSeconds);
    }

    public bool Change(float seconds)
    {
        if (_changingCoroutine != null)
        {
            return false;
        }

        _changingCoroutine = StartCoroutine(ChangeCoroutine(seconds <= 0 ? _defaultDurationSeconds : seconds));
        return true;
    }

    public void Restore()
    {
        _renderer.material = _originalMaterial;
        _isStopCoroutine = true;
    }

    private IEnumerator ChangeCoroutine(float seconds)
    {
        _isStopCoroutine = false;

        _renderer.material = _material;

        for (var time = 0.0f; time <= seconds; time += Time.deltaTime)
        {
            if (_isStopCoroutine)
            {
                yield break;
            }

            yield return null;
        }

        Restore();
        _changingCoroutine = null;
    }
}
