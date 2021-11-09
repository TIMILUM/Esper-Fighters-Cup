using System.Collections;
using EsperFightersCup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IngameDelayCursorObject : MonoBehaviour
{
    private static UnityAction<bool, float> _setActiveCursor = null;

    [SerializeField]
    private float _scaledSize = 1.0f;

    private Coroutine _cursorCoroutine;
    private Image _cursorImage;
    private float _elapsedTime;

    private void Reset()
    {
        _cursorImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _setActiveCursor += ActiveCursor;
        _cursorImage.enabled = false;
        transform.localScale *= _scaledSize;
    }

    private void OnDestroy()
    {
        _setActiveCursor = null;
    }

    /// <summary>
    ///     딜레이 커서를 활성화 합니다.
    /// </summary>
    /// <param name="isActive">딜레이 커서 활성화 여부</param>
    /// <param name="activeTime">딜레이 커서 활성화 시간</param>
    public static void SetActiveCursor(bool isActive, float activeTime = 0)
    {
        if (activeTime <= 0)
        {
            _setActiveCursor?.Invoke(false, 0);
        }
        else
        {
            _setActiveCursor?.Invoke(isActive, activeTime);
        }
    }

    private void ActiveCursor(bool isActive, float activeTime = 0)
    {
        GameCursorUtil.SetVisible(!isActive);
        _cursorImage.enabled = isActive;
        if (!isActive)
        {
            return;
        }

        if (_cursorCoroutine != null)
        {
            StopCoroutine(_cursorCoroutine);
            _cursorCoroutine = null;
        }

        _cursorCoroutine = StartCoroutine(CursorAnimation(activeTime));
    }

    private IEnumerator CursorAnimation(float activeTime)
    {
        ((RectTransform)transform).anchoredPosition = Input.mousePosition;

        for (; _elapsedTime > activeTime; _elapsedTime += Time.deltaTime)
        {
            _cursorImage.fillAmount = Mathf.Lerp(1, 0, _elapsedTime / activeTime);
            yield return null;
        }

        _cursorCoroutine = null;
        ActiveCursor(false);
    }
}
