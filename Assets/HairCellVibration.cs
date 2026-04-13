using UnityEngine;

public class HairCellVibration : MonoBehaviour
{
    public float vibrationSpeed = 35f;
    public float maxTilt = 18f;
    public float maxHeightOffset = 0.15f;
    public float smoothSpeed = 8f;

    [Range(0.8f, 1.2f)]
    public float sensitivityMultiplier = 1f;

    [Range(0f, 2f)]
    public float phaseOffset = 0f;

    private Quaternion startRotation;
    private Vector3 startPosition;

    private float currentStrength = 0f;
    private float displayedStrength = 0f;

    void Start()
    {
        startRotation = transform.localRotation;
        startPosition = transform.localPosition;

        // Hvis ikke sat manuelt, får hver celle en lille unik variation
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
        // Gør bevægelsen mere glidende
        displayedStrength = Mathf.Lerp(displayedStrength, currentStrength, Time.deltaTime * smoothSpeed);

        float adjustedStrength = displayedStrength * sensitivityMultiplier;

        // Sinusbølge med individuel fase
        float wave = Mathf.Sin(Time.time * vibrationSpeed + phaseOffset);

        // Side-til-side tilt
        float tiltZ = wave * adjustedStrength * maxTilt;

        // En lille frem/tilbage bevægelse
        float tiltX = wave * adjustedStrength * (maxTilt * 0.35f);

        // Lidt op/ned, men meget mindre end tilt
        float yOffset = Mathf.Abs(wave) * adjustedStrength * maxHeightOffset;

        transform.localRotation = startRotation * Quaternion.Euler(tiltX, 0f, tiltZ);
        transform.localPosition = startPosition + new Vector3(0f, yOffset, 0f);
    }

    public void SetStrength(float value)
    {
        currentStrength = Mathf.Clamp01(value);
    }
}