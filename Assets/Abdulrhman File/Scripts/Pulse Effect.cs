using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    [Tooltip("Duration for half a pulse cycle (scale up or scale down).")]
    public float pulseDuration = 1f;
    
    [Tooltip("Maximum scale factor during the pulse.")]
    public float pulseScale = 1.2f;
    
    private Vector3 originalScale;
    
    private void Awake()
    {
        originalScale = transform.localScale;
    }
    
    private void OnEnable()
    {
        StartCoroutine(Pulse());
    }
    
    private System.Collections.IEnumerator Pulse()
    {
        while (true)
        {
            // Scale up
            float timer = 0f;
            while (timer < pulseDuration)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(1f, pulseScale, timer / pulseDuration);
                transform.localScale = originalScale * scale;
                yield return null;
            }
            // Scale down
            timer = 0f;
            while (timer < pulseDuration)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(pulseScale, 1f, timer / pulseDuration);
                transform.localScale = originalScale * scale;
                yield return null;
            }
        }
    }
}
