/*
 This script simulates a single hair cell reacting to sound by using a sine wave to control tilt and movement, 
with smoothing and random variation to create a more natural and organic behavior.
 */

using UnityEngine;

public class HairCellVibration : MonoBehaviour
{
    public float vibrationSpeed = 35f;      // Speed of vibration (sine wave frequency)
    public float maxTilt = 18f;             // Maximum tilt angle (side-to-side)
    public float maxHeightOffset = 0.15f;   // Maximum vertical movement (up/down)
    public float smoothSpeed = 8f;          // Smoothing speed for movement

    [Range(0.8f, 1.2f)]
    public float sensitivityMultiplier = 1f; // Small variation in sensitivity per hair

    [Range(0f, 2f)]
    public float phaseOffset = 0f; // Offset in wave timing (makes movement less uniform)

    private Quaternion startRotation; // Initial rotation of the object
    private Vector3 startPosition;    // Initial position of the object

    private float currentStrength = 0f;   // Raw input strength
    private float displayedStrength = 0f; // Smoothed strength

    void Start()
    {
        // Store initial transform values
        startRotation = transform.localRotation;
        startPosition = transform.localPosition;

        // Add random variation if not manually set
        if (phaseOffset == 0f)
        {
            phaseOffset = Random.Range(0f, 2f);
        }

        if (sensitivityMultiplier == 1f)
        {
            sensitivityMultiplier = Random.Range(0.85f, 1.15f);
        }
    }

    void Update()
    {
        // Smooth the strength to avoid jittery movement
        displayedStrength = Mathf.Lerp(displayedStrength, currentStrength, Time.deltaTime * smoothSpeed);

        // Apply individual sensitivity variation
        float adjustedStrength = displayedStrength * sensitivityMultiplier;

        // Generate sine wave with individual phase offset
        float wave = Mathf.Sin(Time.time * vibrationSpeed + phaseOffset);

        // Side-to-side tilt (Z axis)
        float tiltZ = wave * adjustedStrength * maxTilt;

        // Slight forward/backward tilt (X axis)
        float tiltX = wave * adjustedStrength * (maxTilt * 0.35f);

        // Small vertical movement (always positive using Abs)
        float yOffset = Mathf.Abs(wave) * adjustedStrength * maxHeightOffset;

        // Apply rotation based on calculated tilt
        transform.localRotation = startRotation * Quaternion.Euler(tiltX, 0f, tiltZ);

        // Apply vertical movement
        transform.localPosition = startPosition + new Vector3(0f, yOffset, 0f);
    }

    public void SetStrength(float value)
    {
        // Clamp input value between 0 and 1
        currentStrength = Mathf.Clamp01(value);
    }
}

