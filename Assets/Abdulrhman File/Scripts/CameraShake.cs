using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("Duration of the camera shake effect.")]
    public float shakeDuration = 0.5f;
    [Tooltip("Magnitude of the shake. Increase to shake more.")]
    public float shakeMagnitude = 0.1f;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// Call this method to trigger the camera shake.
    /// </summary>
    public void TriggerShake()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 randomPoint = originalPosition + Random.insideUnitSphere * shakeMagnitude;
            // Keep the original Z position to avoid moving the camera forward/backwards.
            transform.localPosition = new Vector3(randomPoint.x, randomPoint.y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
