using System.Collections;
using System.Collections.Generic;
using EsperFightersCup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IngameDelayCursorObject : MonoBehaviour
{
    private static UnityAction<bool, float> _setActiveCursor = null;

    [SerializeField]
    private float _scaledSize = 1.0f;

    private Coroutine _cursorCoroutine;
    [SerializeField]
    private Image _cursorImage;
    private Canvas _canvas;
    private float _elapsedTime = 0.0f;

    private void Reset()
    {
        _cursorImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
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
        _elapsedTime = 0.0f;

        for (; _elapsedTime < activeTime; _elapsedTime += Time.deltaTime)
        {
            var rectTransform = transform as RectTransform;
            rectTransform.anchoredPosition = GetMousePosition();
            _cursorImage.fillAmount = Mathf.Lerp(1, 0, _elapsedTime / activeTime);
            yield return null;
        }

        _cursorCoroutine = null;
        ActiveCursor(false);
    }

    private Vector2 GetMousePosition()
    {
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        var eventPosition = Input.mousePosition;
        int displayIndex = _canvas.targetDisplay;
        
        // Multiple display support only when not the main display. For display 0 the reported
        // resolution is always the desktops resolution since its part of the display API,
        // so we use the standard none multiple display method. (case 741751)
        float w = Screen.width;
        float h = Screen.height;
        if (displayIndex > 0 && displayIndex < Display.displays.Length)
        {
            w = Display.displays[displayIndex].systemWidth;
            h = Display.displays[displayIndex].systemHeight;
        }

        w /= 2;
        h /= 2;
        return new Vector2(eventPosition.x - w, eventPosition.y - h);
    }
}
