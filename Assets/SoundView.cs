using UnityEngine;

public class SoundView : MonoBehaviour
{
    public SoundModel model;
    public GameObject spherePrefab;

    public float cellSize = 1f;
    public float heightScale = 5f;

    private GameObject[,] spheres;

    void Start()
    {
        if (model == null)
        {
            Debug.LogError("SoundView mangler Model reference.");
            return;
        }

        if (spherePrefab == null)
        {
            Debug.LogError("SoundView mangler Sphere Prefab.");
            return;
        }

        int width = model.width;
        int depth = model.depth;

        spheres = new GameObject[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float spacing = 0.5f;

                float posX = (x - width / 2f) * spacing;
                float posZ = z * spacing;

                Vector3 pos = new Vector3(posX, 0, posZ);

                GameObject sphere = Instantiate(spherePrefab, pos, Quaternion.identity, transform);
                spheres[x, z] = sphere;
            }
        }
    }

    void Update()
    {
        if (model == null || model.noiseGrid == null || spheres == null)
            return;

        for (int x = 0; x < model.width; x++)
        {
            for (int z = 0; z < model.depth; z++)
            {
                float baseValue = model.noiseGrid[x, z];

                float rowFactor = Mathf.Lerp(0.6f, 1.2f, (float)z / (model.depth - 1));

                float distance = Vector2.Distance(
                    new Vector2(x, z),
                    new Vector2(model.width / 2f, model.depth / 2f)
                );

                float wave = Mathf.Sin(Time.time * 5f - distance * 0.5f);

                float value = Mathf.Clamp01(baseValue * rowFactor * (0.5f + wave * 0.5f));

                GameObject sphere = spheres[x, z];
                if (sphere == null)
                    continue;

                Vector3 pos = sphere.transform.localPosition;
                pos.y = value * heightScale;
                sphere.transform.localPosition = pos;

                Renderer renderer = sphere.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = GetColor(value);
                }

                HairCellVibration vibration = sphere.GetComponent<HairCellVibration>();
                if (vibration != null)
                {
                    vibration.SetStrength(value);
                }
            }
        }
    }

    Color GetColor(float value)
    {
        if (value < 0.4f)
        {
            return Color.Lerp(new Color(0.4f, 0.8f, 1f), new Color(0.5f, 1f, 0.5f), value / 0.4f);
        }
        else if (value < 0.75f)
        {
            return Color.Lerp(new Color(0.5f, 1f, 0.5f), new Color(1f, 0.65f, 0.2f), (value - 0.4f) / 0.35f);
        }
        else
        {
            return Color.Lerp(new Color(1f, 0.65f, 0.2f), new Color(0.8f, 0.1f, 0.1f), (value - 0.75f) / 0.25f);
        }
    }
}