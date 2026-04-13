using UnityEngine;

public class EarCenterVibration : MonoBehaviour
{
    public SoundModel soundModel;

    [Header("Movement")]
    public float vibrationSpeed = 45f;
    public float maxForwardMove = 0.15f;
    public float smoothSpeed = 8f;

    [Header("Tilt")]
    public float maxTilt = 6f;

    [Header("Color")]
    public Renderer targetRenderer;
    public Color calmColor = new Color(0.8f, 0.6f, 0.6f);
    public Color stressColor = new Color(1f, 0.2f, 0.2f);

    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;
    private float displayedStrength = 0f;

    void Start()
    {
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;

        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    void Update()
    {
        if (soundModel == null)
            return;

        float targetStrength = Mathf.Clamp01(soundModel.smoothedLoudness * soundModel.micSensitivity);
        displayedStrength = Mathf.Lerp(displayedStrength, targetStrength, Time.deltaTime * smoothSpeed);

        float wave = Mathf.Sin(Time.time * vibrationSpeed);

        float forwardOffset = wave * displayedStrength * maxForwardMove;
        float tiltAmount = wave * displayedStrength * maxTilt;

        transform.localPosition = startLocalPosition + new Vector3(0f, 0f, forwardOffset);
        transform.localRotation = startLocalRotation * Quaternion.Euler(tiltAmount * 0.4f, 0f, tiltAmount);

        if (targetRenderer != null)
        {
            targetRenderer.material.color = Color.Lerp(calmColor, stressColor, displayedStrength);
        }
    }
}