using System;
using UnityEngine;
using System.Collections;

public class MirrorRotator : MonoBehaviour
{
    // Rotation amount per click (in degrees)
    public float rotationAngle = 90f;

    // Rotation speed in degrees per second (adjustable in the Inspector)
    public float rotationSpeed = 90f;

    // Flag to ensure one rotation at a time
    private bool isRotating = false;
    
    // transform of the mirror sprite 
    private Transform MirrorTransform;

    private void Start()
    {
        MirrorTransform = transform.GetChild(1);
    }

    // Called when the object is clicked/tapped
    private void OnMouseDown()
    {
        if (!isRotating)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.mirror);
            StartCoroutine(RotateMirror());
        }
    }

    // Coroutine to smoothly rotate the mirror
    private IEnumerator RotateMirror()
    {
        isRotating = true;

        // Get the current Z rotation and calculate the target rotation
        float startAngle = MirrorTransform.eulerAngles.z;
        float targetAngle = startAngle + rotationAngle;
        targetAngle = NormalizeAngle(targetAngle);

        // Rotate until the angle difference is minimal
        while (Mathf.Abs(Mathf.DeltaAngle(MirrorTransform.eulerAngles.z, targetAngle)) > 0.1f)
        {
            float step = rotationSpeed * Time.deltaTime;
            float newAngle = Mathf.MoveTowardsAngle(MirrorTransform.eulerAngles.z, targetAngle, step);
            MirrorTransform.eulerAngles = new Vector3(MirrorTransform.eulerAngles.x, MirrorTransform.eulerAngles.y, newAngle);
            yield return null;
        }

        // Snap to the target angle and finish
        MirrorTransform.eulerAngles = new Vector3(MirrorTransform.eulerAngles.x, MirrorTransform.eulerAngles.y, targetAngle);
        isRotating = false;
    }

    // Helper function to normalize angles between 0 and 360 degrees
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0)
            angle += 360f;
        return angle;
    }
}
