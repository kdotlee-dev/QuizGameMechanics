using UnityEngine;
using System;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static IEnumerator MoveUI(RectTransform rect, Vector2 targetPos, float duration, Action onComplete = null)
    {
        Vector2 startPos = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rect.anchoredPosition = targetPos;

        onComplete?.Invoke();
    }
}
