using UnityEngine;

public class HairCellVibration : MonoBehaviour
{
    public float vibrationSpeed = 40f;
    public float maxTilt = 20f;
    public float currentStrength = 0f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float wave = Mathf.Sin(Time.time * vibrationSpeed) * currentStrength * maxTilt;
        transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, wave);
    }

    public void SetStrength(float value)
    {
        currentStrength = value;
    }
}