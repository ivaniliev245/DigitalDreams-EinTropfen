using UnityEngine;
using System.Collections;

public class RainManager : MonoBehaviour
{
    public GameObject goodRaindropPrefab; // Good Raindrop prefab
    public GameObject badRaindropPrefab; // Bad Raindrop prefab
    public Transform warriorTransform;   // Reference to the warrior
    public float rainSpawnRate = 2.0f;   // Spawn interval in seconds
    public float proximityThreshold = 1.0f; // Proximity distance for checking drops

    // Adjustable bounds for the raindrop spawn area
    public Vector2 spawnAreaX = new Vector2(-5f, 5f); // Min and Max X bounds
    public Vector2 spawnAreaZ = new Vector2(-5f, 5f); // Min and Max Z bounds
    public float spawnHeight = 10f; // Fixed Y spawn above the plane

    void Start()
    {
        StartCoroutine(SpawnRaindrops());
    }

    IEnumerator SpawnRaindrops()
    {
        while (true)
        {
            SpawnRaindrop();
            yield return new WaitForSeconds(rainSpawnRate); // Wait for the spawn rate to elapse
        }
    }

    void SpawnRaindrop()
    {
        // Randomly decide to spawn a Good or Bad raindrop
        GameObject raindropPrefab = Random.value < 0.7f ? goodRaindropPrefab : badRaindropPrefab; // 70% chance for Good

        if (raindropPrefab != null)
        {
            // Randomize spawn position within the defined 3D bounds
            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnAreaX.x, spawnAreaX.y), // Random X position
                spawnHeight, // Fixed Y spawn height
                Random.Range(spawnAreaZ.x, spawnAreaZ.y) // Random Z position
            );

            // Instantiate the raindrop prefab at the calculated spawn position
            GameObject raindrop = Instantiate(raindropPrefab, spawnPosition, Quaternion.identity);
            Rigidbody rb = raindrop.GetComponent<Rigidbody>();

            // Ensure the raindrop prefab has a Rigidbody
            if (rb == null)
            {
                Debug.LogError("Raindrop prefab is missing a Rigidbody component!");
            }

            RainDroplet raindropScript = raindrop.GetComponent<RainDroplet>();

            // Assign properties to the raindrop script if present
            if (raindropScript != null)
            {
                raindropScript.warriorTransform = warriorTransform;
                raindropScript.proximityThreshold = proximityThreshold;
            }
        }
        else
        {
            Debug.LogError("Raindrop prefab is not assigned.");
        }
    }

    void OnDrawGizmos()
    {
        // Draw spawn area bounds
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(
            new Vector3((spawnAreaX.x + spawnAreaX.y) / 2, spawnHeight, (spawnAreaZ.x + spawnAreaZ.y) / 2),
            new Vector3(spawnAreaX.y - spawnAreaX.x, 0.1f, spawnAreaZ.y - spawnAreaZ.x)
        );

        // Draw proximity threshold sphere around the warrior
        if (warriorTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(warriorTransform.position, proximityThreshold);
        }
    }
}
