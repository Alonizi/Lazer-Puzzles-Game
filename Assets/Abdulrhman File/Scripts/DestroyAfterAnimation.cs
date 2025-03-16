using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    // Time in seconds to wait before destroying the GameObject.
    [SerializeField] private float destroyDelay = 1f;

    private void Start()
    {
        // Destroy the GameObject after the specified delay.
        Destroy(gameObject, destroyDelay);
    }
}
