using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [HideInInspector] public static CameraShaker Instance;

    [SerializeField] private AnimationCurve curve;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 startPos = transform.localPosition;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float strength = curve.Evaluate(elapsedTime / duration);

            transform.localPosition = startPos + (magnitude * strength * (Vector3)Random.insideUnitCircle);

            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0);
        }

        transform.localPosition = startPos;
    }
}