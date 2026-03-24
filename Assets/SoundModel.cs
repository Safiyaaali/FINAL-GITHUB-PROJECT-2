using UnityEngine;

public class SoundModel : MonoBehaviour
{
    [Header("Grid")]
    public int width = 20;
    public int depth = 30;
    public float[,] noiseGrid;

    [Header("Microphone")]
    public string selectedMic = "";
    public int sampleWindow = 128;
    public int frequency = 44100;

    [Header("Sound Settings")]
    public float micSensitivity = 50f;

    public float currentLoudness;

    private AudioClip micClip;
    private string micName;

    void Awake()
    {
        noiseGrid = new float[width, depth];
    }

    void Start()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("Ingen mikrofon fundet.");
            return;
        }

        micName = string.IsNullOrEmpty(selectedMic) ? Microphone.devices[0] : selectedMic;
        micClip = Microphone.Start(micName, true, 10, frequency);

        Debug.Log("Mikrofon startet: " + micName);
    }

    void Update()
    {
        currentLoudness = GetLoudness();

        float value = Mathf.Clamp01(currentLoudness * micSensitivity);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                noiseGrid[x, z] = value;
            }
        }
    }

    float GetLoudness()
    {
        if (micClip == null) return 0f;

        int micPosition = Microphone.GetPosition(micName) - sampleWindow;
        if (micPosition < 0) return 0f;

        float[] samples = new float[sampleWindow];
        micClip.GetData(samples, micPosition);

        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += samples[i] * samples[i];
        }

        return Mathf.Sqrt(sum / sampleWindow);
    }

    void OnDisable()
    {
        if (!string.IsNullOrEmpty(micName) && Microphone.IsRecording(micName))
        {
            Microphone.End(micName);
        }
    }
}