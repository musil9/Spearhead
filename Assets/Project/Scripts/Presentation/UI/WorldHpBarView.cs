using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WorldHpBarView : MonoBehaviour
{
    [SerializeField] private GameObject m_root;
    [SerializeField] private Image m_fillImage;

    private Coroutine m_changeCoroutine;

    public void Initialize(int _currentHp, int _maxHp)
    {
        SetVisible(true);
        SetImmediate(_currentHp, _maxHp);
    }

    public void SetVisible(bool _isVisible)
    {
        if (m_root == null)
            return;

        m_root.SetActive(_isVisible);
    }

    public void SetImmediate(int _currentHp, int _maxHp)
    {
        StopChangeCoroutine();

        if (m_fillImage == null)
            return;

        m_fillImage.fillAmount = GetNormalizedHp(_currentHp, _maxHp);
    }

    public void PlayChange(int _previousHp, int _currentHp, int _maxHp, float _duration)
    {
        StopChangeCoroutine();

        m_changeCoroutine = StartCoroutine(PlayChangeRoutine(_previousHp, _currentHp, _maxHp, _duration));
    }

    private IEnumerator PlayChangeRoutine(int _previousHp, int _currentHp, int _maxHp, float _duration)
    {
        if (m_fillImage == null)
        {
            m_changeCoroutine = null;
            yield break;
        }

        var startAmount = GetNormalizedHp(_previousHp, _maxHp);

        var targetAmount = GetNormalizedHp(_currentHp, _maxHp);

        var duration = Mathf.Max(0.01f, _duration);

        var elapsedTime = 0f;

        m_fillImage.fillAmount = startAmount;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            var normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            var easedTime = 1f - Mathf.Pow(1f - normalizedTime, 3f);

            m_fillImage.fillAmount = Mathf.Lerp(startAmount, targetAmount, easedTime);

            yield return null;
        }

        m_fillImage.fillAmount = targetAmount;
        m_changeCoroutine = null;
    }

    private void StopChangeCoroutine()
    {
        if (m_changeCoroutine == null)
            return;

        StopCoroutine(m_changeCoroutine);
        m_changeCoroutine = null;
    }

    private static float GetNormalizedHp(
        int _currentHp,
        int _maxHp)
    {
        int safeMaxHp =
            Mathf.Max(1, _maxHp);

        return Mathf.Clamp01(
            (float)_currentHp / safeMaxHp);
    }
}
