using UnityEngine;

public class WorldFiller : MonoBehaviour
{
    [Header("Tree Settings")]
    public GameObject[] treePrefabs;

    [Header("Grass Settings")]
    public GameObject[] grassPrefabs;
    [Range(0f, 1f)] public float grassChance = 0.3f;

    [Header("World Settings")]
    public float spacing = 5f;
    public float outerRadius = 50f;     // Full radius of the filled world
    public float innerClearRadius = 35f; // Keep this radius clear for building

    void Start()
    {
        for (float x = -outerRadius; x <= outerRadius; x += spacing)
        {
            for (float z = -outerRadius; z <= outerRadius; z += spacing)
            {
                Vector2 posXZ = new Vector2(x, z);
                float dist = posXZ.magnitude;

                // 🧱 Skip if inside the build zone
                if (dist <= innerClearRadius || dist > outerRadius)
                    continue;

                Vector3 pos = new Vector3(x, 0f, z);

                // 🌲 Tree spawn
                if (treePrefabs.Length > 0)
                {
                    GameObject tree = Instantiate(
                        treePrefabs[Random.Range(0, treePrefabs.Length)],
                        pos,
                        Quaternion.Euler(0, Random.Range(0f, 360f), 0),
                        transform
                    );

                    float scale = Random.Range(0.9f, 1.3f);
                    tree.transform.localScale = new Vector3(1.5f, 3.0f, 1.5f);
                }

                // 🌿 Grass spawn (chance-based)
                if (grassPrefabs.Length > 0 && Random.value < grassChance)
                {
                    Vector3 grassPos = pos + new Vector3(
                        Random.Range(-spacing / 2f, spacing / 2f),
                        0f,
                        Random.Range(-spacing / 2f, spacing / 2f)
                    );

                    GameObject grass = Instantiate(
                        grassPrefabs[Random.Range(0, grassPrefabs.Length)],
                        grassPos,
                        Quaternion.Euler(0, Random.Range(0f, 360f), 0),
                        transform
                    );

                    float grassScale = Random.Range(0.8f, 1.2f);
                    grass.transform.localScale = Vector3.one * grassScale;
                }
            }
        }
    }
}
