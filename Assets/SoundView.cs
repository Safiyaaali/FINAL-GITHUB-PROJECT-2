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
        int width = model.width;
        int depth = model.depth;

        spheres = new GameObject[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 pos = new Vector3(
                    x - model.width / 2,
                    0,
                    z - model.depth / 2
                );

                GameObject sphere = Instantiate(spherePrefab, pos, Quaternion.identity, transform);
                spheres[x, z] = sphere;
            }
        }
    }

    void Update()
    {
        for (int x = 0; x < model.width; x++)
        {
            for (int z = 0; z < model.depth; z++)
            {
                float value = model.noiseGrid[x, z];
                GameObject sphere = spheres[x, z];

                Vector3 pos = sphere.transform.localPosition;
                pos.y = value * heightScale;
                sphere.transform.localPosition = pos;

                Renderer renderer = sphere.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = GetColor(value);
                }
            }
        }
    }

    Color GetColor(float value)
    {
        if (value < 0.5f)
        {
            return Color.Lerp(Color.blue, Color.yellow, value * 2f);
        }
        else
        {
            return Color.Lerp(Color.yellow, Color.red, (value - 0.5f) * 2f);
        }
    }
}