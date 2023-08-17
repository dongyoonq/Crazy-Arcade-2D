using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePanel : MonoBehaviour
{
    public RectTransform parentPanel;
    public float moveDistance = 200f; // 이동 거리
    public float moveDuration = 0.7f; // 이동 애니메이션 지속시간

    private bool isMoved = false;
    private Vector2 originalPosition;

    private IEnumerator Start()
    {
        originalPosition = parentPanel.anchoredPosition;
        yield return null;
    }

    public void TogglePanel()
    {
        StopAllCoroutines();
        StartCoroutine(MovePanelCoroutine());
    }

    private IEnumerator MovePanelCoroutine()
    {
        isMoved = !isMoved;
        Vector2 targetPosition = isMoved ? originalPosition + new Vector2(moveDistance, 0) : originalPosition;

        Vector2 startPosition = parentPanel.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            parentPanel.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parentPanel.anchoredPosition = targetPosition;
    }
}