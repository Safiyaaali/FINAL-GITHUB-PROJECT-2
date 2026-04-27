/* Summery
 This script simulates a three-segment hair that bends like a wave based on sound input, 
with delayed motion between segments and smooth color transitions representing sound intensity.
*/

using UnityEngine;

public class HairThreeSegmentVibration : MonoBehaviour
{
    [Header("Color Settings")]
    public bool enableColor = true; // Enables/disables color visualization

    [Header("Colors")]
    public Renderer[] segmentRenderers; // Renderers for each segment
    public Color idleColor = new Color(0.35f, 0.2f, 0.1f);   // Default brown color
    public Color lowColor = new Color(0.2f, 0.9f, 0.2f);     // Low intensity (green)
    public Color midColor = new Color(1f, 0.9f, 0.2f);       // Medium intensity (yellow)
    public Color highColor = new Color(1f, 0.2f, 0.15f);     // High intensity (red)

    [Header("Color Flow")]
    public float colorSmoothSpeed = 3f; // Speed of color transition
    public float idleThreshold = 0.05f; // Threshold for idle state

    [Header("Movement")]
    public float vibrationSpeed = 7f; // Speed of vibration wave
    public float smoothSpeed = 6f;    // Smoothing for strength changes

    [Header("Angles")]
    public float maxAngleBottom = 10f; // Max bend for bottom segment
    public float maxAngleMiddle = 18f; // Max bend for middle segment
    public float maxAngleTop = 30f;    // Max bend for top segment

    [Header("Wave Delay")]
    public float segmentDelay = 0.25f; // Delay between segments (wave effect)

    [Header("Direction")]
    public bool bendToNegativeSide = false; // If true, bend in opposite direction

    [Header("Variation")]
    public float phaseOffset = 0f;        // Random phase offset for variation
    public float strengthMultiplier = 1f; // Random strength variation

    private Transform pivot1; // Bottom segment pivot
    private Transform pivot2; // Middle segment pivot
    private Transform pivot3; // Top segment pivot

    private float targetStrength;   // Target strength from input
    private float currentStrength;  // Smoothed strength
    private float incomingValue;    // Raw incoming value

    private Quaternion pivot1StartRot; // Initial rotation for pivot1
    private Quaternion pivot2StartRot; // Initial rotation for pivot2
    private Quaternion pivot3StartRot; // Initial rotation for pivot3

    private Color currentColor; // Current color applied

    void Awake()
    {
        // Find child pivots in hierarchy
        pivot1 = transform.Find("Pivot1");

        if (pivot1 != null)
            pivot2 = pivot1.Find("Pivot2");

        if (pivot2 != null)
            pivot3 = pivot2.Find("Pivot3");
    }

    void Start()
    {
        // Ensure all pivots exist
        if (pivot1 == null || pivot2 == null || pivot3 == null)
        {
            Debug.LogError("Could not find Pivot1, Pivot2 or Pivot3 on " + gameObject.name);
            enabled = false;
            return;
        }

        // Store initial rotations
        pivot1StartRot = pivot1.localRotation;
        pivot2StartRot = pivot2.localRotation;
        pivot3StartRot = pivot3.localRotation;

        // Add random variation if not set manually
        if (phaseOffset == 0f)
            phaseOffset = Random.Range(0f, 10f);

        if (strengthMultiplier == 1f)
            strengthMultiplier = Random.Range(0.9f, 1.1f);

        // Set initial color
        currentColor = idleColor;
        ApplyColor(currentColor);
    }

    void Update()
    {
        // Smoothly update strength
        currentStrength = Mathf.Lerp(currentStrength, targetStrength, Time.deltaTime * smoothSpeed);
        float strength = currentStrength * strengthMultiplier;

        // Time-based wave calculation
        float t = Time.time * vibrationSpeed + phaseOffset;

        // Create wave per segment with delay using Abs(Sin), so the value stays positive (0 to 1) instead of oscillating between -1 and 1
        float wave1 = Mathf.Abs(Mathf.Sin(t));
        float wave2 = Mathf.Abs(Mathf.Sin(t - segmentDelay));
        float wave3 = Mathf.Abs(Mathf.Sin(t - segmentDelay * 2f));

        // Calculate bending angles
        float angle1 = wave1 * strength * maxAngleBottom;
        float angle2 = wave2 * strength * maxAngleMiddle;
        float angle3 = wave3 * strength * maxAngleTop;

        // Optional direction flip
        if (bendToNegativeSide)
        {
            angle1 = -angle1;
            angle2 = -angle2;
            angle3 = -angle3;
        }

        // Apply rotations to each segment
        pivot1.localRotation = pivot1StartRot * Quaternion.Euler(0f, 0f, angle1);
        pivot2.localRotation = pivot2StartRot * Quaternion.Euler(0f, 0f, angle2);
        pivot3.localRotation = pivot3StartRot * Quaternion.Euler(0f, 0f, angle3);

        // Update color based on input
        UpdateColor();
    }

    public void SetStrength(float value)
    {
        // Clamp input value and set as target strength
        incomingValue = Mathf.Clamp01(value);
        targetStrength = incomingValue;
    }

    public void SetVibration(float value)
    {
        // Alias method (same as SetStrength)
        SetStrength(value);
    }

    void UpdateColor()
    {
        // Skip if no renderers assigned
        if (segmentRenderers == null || segmentRenderers.Length == 0)
            return;

        // If color is disabled → fade back to idle
        if (!enableColor)
        {
            currentColor = Color.Lerp(currentColor, idleColor, Time.deltaTime * colorSmoothSpeed);
            ApplyColor(currentColor);
            return;
        }

        float v = incomingValue;
        Color targetColor;

        // Determine color based on intensity
        if (v < idleThreshold)
        {
            targetColor = idleColor;
        }
        else if (v < 0.5f)
        {
            float t = Mathf.InverseLerp(idleThreshold, 0.5f, v);
            targetColor = Color.Lerp(lowColor, midColor, t);
        }
        else
        {
            float t = Mathf.InverseLerp(0.5f, 1f, v);
            targetColor = Color.Lerp(midColor, highColor, t);
        }

        // Smooth transition to target color
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorSmoothSpeed);

        // Apply color to all segments
        ApplyColor(currentColor);
    }

    void ApplyColor(Color colorToApply)
    {
        foreach (Renderer rend in segmentRenderers)
        {
            if (rend == null)
                continue;

            // Support both URP (_BaseColor) and Standard shader (_Color)
            if (rend.material.HasProperty("_BaseColor"))
                rend.material.SetColor("_BaseColor", colorToApply);

            if (rend.material.HasProperty("_Color"))
                rend.material.color = colorToApply;
        }
    }
}
