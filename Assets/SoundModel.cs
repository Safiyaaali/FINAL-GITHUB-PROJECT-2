/* Summery
 This script captures microphone input, 
calculates loudness using RMS, 
smooths the signal, 
and distributes the value across a 2D grid used to control visual elements.
*/

using UnityEngine; // Gives access to Unity features (mikrofon, matematik osv.)

public class SoundModel : MonoBehaviour // Script that can be attached to a GameObject (script der kan sættes på et objekt)
{
    [Header("Grid")]
    public int width = 20;   // Width of grid (antal kolonner)
    public int depth = 30;   // Depth of grid (antal rækker)
    public float[,] noiseGrid; // 2D array storing sound values (2D array med lydværdier)

    [Header("Microphone")]
    public string selectedMic = ""; // Specific microphone (valgt mikrofon)
    public int sampleWindow = 128;  // Number of samples used (antal samples)
    public int frequency = 44100;   // Sample rate (lydfrekvens)

    [Header("Sound Settings")]
    public float micSensitivity = 50f; // Amplifies microphone input (forstærker lyd)

    public float currentLoudness;   // Raw loudness (rå lydstyrke)
    public float smoothedLoudness;  // Smoothed loudness (glattet lyd)
    public float smoothingSpeed = 8f; // Speed of smoothing (hastighed for udjævning)

    private AudioClip micClip; // Audio clip from microphone (lyd fra mic)
    private string micName;    // Name of active microphone (navn på mic)

    void Awake()
    {
        // Create the grid array (opretter grid)
        noiseGrid = new float[width, depth];
    }

    void Start()
    {
        // Check if any microphone is available (tjekker om der er en mikrofon)
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("Ingen mikrofon fundet."); // No microphone found
            return;
        }

        // Select microphone (vælger mikrofon)
        micName = string.IsNullOrEmpty(selectedMic) ? Microphone.devices[0] : selectedMic;

        // Start recording from microphone (starter optagelse)
        micClip = Microphone.Start(micName, true, 10, frequency);

        Debug.Log("Mikrofon startet: " + micName);
    }

    void Update()
    {
        // Ensure grid exists (sikrer array findes)
        if (noiseGrid == null)
        {
            noiseGrid = new float[width, depth];
        }

        // Get current loudness from microphone (henter lydstyrke)
        currentLoudness = GetLoudness();

        // Smooth the loudness to avoid jumps (udglatter lyd)
        smoothedLoudness = Mathf.Lerp(smoothedLoudness, currentLoudness, Time.deltaTime * smoothingSpeed);

        // Scale and clamp value between 0 and 1 (begrænser værdien)
        float value = Mathf.Clamp01(smoothedLoudness * micSensitivity);

        // Fill the entire grid with the same value (fylder hele grid med samme lydværdi)
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
        // Return 0 if no microphone data (hvis ingen mic data)
        if (micClip == null) return 0f;
        if (string.IsNullOrEmpty(micName)) return 0f;

        // Get current position in recording buffer (finder position i lydbuffer)
        int micPosition = Microphone.GetPosition(micName) - sampleWindow;

        // If too early, return 0 (hvis for tidligt)
        if (micPosition < 0) return 0f;

        float[] samples = new float[sampleWindow];

        // Get audio samples (henter lyddata)
        micClip.GetData(samples, micPosition);

        float sum = 0f;

        // Calculate RMS (root mean square) loudness (beregner lydstyrke)
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += samples[i] * samples[i];
        }

        // Return average loudness (returnerer gennemsnit)
        return Mathf.Sqrt(sum / sampleWindow);
    }

    void OnDisable()
    {
        // Stop microphone when object is disabled (stopper mic)
        if (!string.IsNullOrEmpty(micName) && Microphone.IsRecording(micName))
        {
            Microphone.End(micName);
        }
    }
}