using UnityEngine;

public class MovieSphereSpawner : MonoBehaviour
{
    public Vector3 forestCenter = new Vector3(165, 40, 128);
    public Vector3 desertCenter = new Vector3(720, 25, 128);
    public Vector3 oceanCenter = new Vector3(720, 25, 1000);
    public Vector3 mountainCenter = new Vector3(180, 40, 49);
    public Vector3 cliffCenter = new Vector3(320, 215, 612);

    public GameObject movieSpherePrefab;
    public int gridX = 50;
    public int gridZ = 50;
    public float spacing = 60f;
    public float influenceRadius = 300f;
    public float heightOffsetAboveTerrain = 2.5f;
    public LayerMask terrainLayer;
    
    void Start()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                float posX = x * spacing + Random.Range(-35f, 135f);
                float posZ = z * spacing + Random.Range(-35f, 35f);
                Vector3 basePos = new Vector3(posX, 1000f, posZ);

                if (Physics.Raycast(basePos, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayer))
                {
                    float terrainY = hit.point.y;

                    float dForest = Vector3.Distance(hit.point, forestCenter);
                    float dDesert = Vector3.Distance(hit.point, desertCenter);
                    float dOcean = Vector3.Distance(hit.point, oceanCenter);
                    float dMountain = Vector3.Distance(hit.point, mountainCenter);
                    float dCliff = Vector3.Distance(hit.point, cliffCenter);

                    float minDist = Mathf.Min(dForest, dDesert, dOcean, dMountain, dCliff);
                    Vector3 nearestCenter = forestCenter;

                    if (minDist == dDesert) nearestCenter = desertCenter;
                    else if (minDist == dOcean) nearestCenter = oceanCenter;
                    else if (minDist == dMountain) nearestCenter = mountainCenter;
                    else if (minDist == dCliff) nearestCenter = cliffCenter;

                    float height = Mathf.Lerp(terrainY, nearestCenter.y, Mathf.Clamp01(1f - minDist / influenceRadius));
                    Vector3 spawnPos = new Vector3(posX, height + heightOffsetAboveTerrain, posZ);

                    Instantiate(movieSpherePrefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }
}