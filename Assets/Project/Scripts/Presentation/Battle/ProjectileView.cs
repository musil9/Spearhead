using System.Collections;
using UnityEngine;

public class ProjectileView : MonoBehaviour
{
    public IEnumerator PlayRoutine(Vector3 _startPosition, Vector3 _targetPosition, float _duration)
    {
        transform.position = _startPosition;

        var direction = _targetPosition - _startPosition;

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }

        if (_duration <= 0f)
        {
            transform.position = _targetPosition;
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;

            var normalizedTime = Mathf.Clamp01(elapsedTime / _duration);

            transform.position = Vector3.Lerp(_startPosition, _targetPosition, normalizedTime);

            yield return null;
        }

        transform.position = _targetPosition;
    }
}
