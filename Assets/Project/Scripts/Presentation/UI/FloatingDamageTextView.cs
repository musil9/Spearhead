using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingDamageTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text;

    [Header("Animation")]
    [SerializeField] private float m_duration = 0.65f;
    [SerializeField] private float m_riseDistance = 0.5f;

    public IEnumerator PlayRoutine(int _damage, Vector3 _worldPosition)
    {
        transform.position = _worldPosition;

        if (m_text == null)
            yield break;

        m_text.text = $"-{_damage}";

        Color originalColor = m_text.color;
        originalColor.a = 1f;

        m_text.color = originalColor;

        var startPosition = _worldPosition;

        var targetPosition = _worldPosition + Vector3.up * m_riseDistance;

        var duration = Mathf.Max(0.01f, m_duration);

        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            var normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            transform.position = Vector3.Lerp(startPosition, targetPosition, normalizedTime);

            var currentColor = originalColor;

            currentColor.a = 1f - Mathf.Clamp01((normalizedTime - 0.4f) / 0.6f);

            m_text.color = currentColor;

            yield return null;
        }

        transform.position = targetPosition;

        var finalColor = originalColor;
        finalColor.a = 0f;

        m_text.color = finalColor;
    }
}
