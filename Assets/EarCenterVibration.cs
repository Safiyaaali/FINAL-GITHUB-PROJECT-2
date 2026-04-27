using UnityEngine;

public class EarCenterVibration : MonoBehaviour
{
    public SoundModel soundModel; // Reference to sound data (microphone input)

    [Header("Movement")]
    public float vibrationSpeed = 45f;     // Speed of vibration (sine wave)
    public float maxForwardMove = 0.15f;   // Maximum forward/backward movement
    public float smoothSpeed = 8f;         // Smoothing speed for movement

    [Header("Tilt")]
    public float maxTilt = 6f;             // Maximum tilt/rotation

    [Header("Color")]
    public Renderer targetRenderer;        // Renderer that changes color
    public Color calmColor = new Color(0.8f, 0.6f, 0.6f); // Color at low sound
    public Color stressColor = new Color(1f, 0.2f, 0.2f); // Color at high sound

    private Vector3 startLocalPosition;    // Initial position (used as reference)
    private Quaternion startLocalRotation; // Initial rotation
    private float displayedStrength = 0f;  // Smoothed sound strength

    void Start()
    {
        // Store starting position and rotation
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;

        // If no renderer is assigned, try to get it automatically
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    void Update()
    {
        // Stop if no sound model is assigned
        if (soundModel == null)
            return;

        // Get sound strength and clamp between 0 and 1
        float targetStrength = Mathf.Clamp01(soundModel.smoothedLoudness * soundModel.micSensitivity);

        // Smooth the value to avoid sudden jumps
        displayedStrength = Mathf.Lerp(displayedStrength, targetStrength, Time.deltaTime * smoothSpeed);

        // Create a sine wave for vibration
        float wave = Mathf.Sin(Time.time * vibrationSpeed);

        // Forward/backward movement based on sound
        float forwardOffset = wave * displayedStrength * maxForwardMove;

        // Tilt (rotation) based on sound
        float tiltAmount = wave * displayedStrength * maxTilt;

        // Apply position change (Z-axis movement)
        transform.localPosition = startLocalPosition + new Vector3(0f, 0f, forwardOffset);

        // Apply rotation (slight tilt)
        transform.localRotation = startLocalRotation * Quaternion.Euler(tiltAmount * 0.4f, 0f, tiltAmount);

        // Change color based on sound intensity
        if (targetRenderer != null)
        {
            targetRenderer.material.color = Color.Lerp(calmColor, stressColor, displayedStrength);
        }
    }
}
