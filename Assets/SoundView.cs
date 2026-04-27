using UnityEngine;

public class SoundView : MonoBehaviour
{
    [Header("References")]
    public SoundModel model;          // Reference to the sound data (contains noiseGrid, width, depth)
    public GameObject hairPrefab;     // Prefab for each hair object

    [Header("Layout")]
    public float spacing = 0.5f;      // Distance between each hair in the grid
    public float groundY = 0f;        // Y position of the ground
    public float sinkIntoGround = 0.15f; // How much the hair is pushed into the ground

    [Header("Wave Flow")]
    public float waveSpeed = 2.5f;    // Speed of the wave animation over time
    public float waveSpacing = 0.45f; // Distance between wave peaks along the Z axis
    public float waveMin = 0.35f;     // Minimum wave influence
    public float waveMax = 1f;        // Maximum wave influence

    [Header("Depth Response")]
    public float frontRowMultiplier = 0.7f;  // Front hairs react less
    public float backRowMultiplier = 1.25f;  // Back hairs react more

    private GameObject[,] hairs; // 2D array storing all instantiated hairs

    void Start()
    {
        // Check if model is assigned
        if (model == null)
        {
            Debug.LogError("SoundView is missing Model reference.");
            return;
        }

        // Check if prefab is assigned
        if (hairPrefab == null)
        {
            Debug.LogError("SoundView is missing Hair Prefab reference.");
            return;
        }

        int width = model.width;   // Number of hairs in X direction
        int depth = model.depth;   // Number of hairs in Z direction

        hairs = new GameObject[width, depth]; // Initialize array

        // Loop through grid and create hairs
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // Calculate position in X (centered grid)
                float posX = (x - width / 2f) * spacing;

                // Calculate position in Z (depth direction)
                float posZ = z * spacing;

                // Final position with slight offset into ground
                Vector3 pos = new Vector3(posX, groundY - sinkIntoGround, posZ);

                // Instantiate hair prefab and parent it to this object
                GameObject hair = Instantiate(hairPrefab, pos, Quaternion.identity, transform);

                // Store reference in array
                hairs[x, z] = hair;
            }
        }
    }

    void Update()
    {
        // Safety check to avoid errors
        if (model == null || model.noiseGrid == null || hairs == null)
            return;

        int width = model.width;
        int depth = model.depth;

        // Loop through all hairs
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // Base sound value from noise grid (microphone input)
                float baseValue = model.noiseGrid[x, z];

                // Depth-based scaling (back rows react more than front rows)
                float rowFactor = Mathf.Lerp(frontRowMultiplier, backRowMultiplier, (float)z / (depth - 1));

                // Create a sine wave over time and depth
                float wave = Mathf.Sin(Time.time * waveSpeed + z * waveSpacing);

                // Convert wave from [-1,1] to [0,1]
                float wave01 = (wave + 1f) * 0.5f;

                // Map wave into desired range
                float waveFactor = Mathf.Lerp(waveMin, waveMax, wave01);

                // Slight variation across X axis to avoid stiffness
                float sideVariation = Mathf.Lerp(0.92f, 1.08f, (float)x / Mathf.Max(1, width - 1));

                // Final strength value (clamped between 0 and 1)
                float value = Mathf.Clamp01(baseValue * rowFactor * waveFactor * sideVariation);

                GameObject hair = hairs[x, z];
                if (hair == null)
                    continue;

                // Get vibration script from hair
                HairThreeSegmentVibration vibration = hair.GetComponent<HairThreeSegmentVibration>();

                // Apply calculated strength to hair movement
                if (vibration != null)
                {
                    vibration.SetStrength(value);
                }
            }
        }
    }
}