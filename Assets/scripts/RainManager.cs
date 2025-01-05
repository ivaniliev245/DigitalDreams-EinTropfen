using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainManager : MonoBehaviour
{
    public List<GameObject> goodRaindropPrefabs; // List of good raindrop prefabs
    public List<GameObject> badRaindropPrefabs;  // List of bad raindrop prefabs
    public Transform warriorTransform;          // Reference to the warrior
    public float rainSpawnRate = 2.0f;          // Spawn interval in seconds
    public float proximityThreshold = 1.0f;     // Proximity distance for checking drops

    // Adjustable bounds for the raindrop spawn area
    public Vector2 spawnAreaX = new Vector2(-5f, 5f); // Min and Max X bounds
    public Vector2 spawnAreaZ = new Vector2(-5f, 5f); // Min and Max Z bounds
    public float spawnHeight = 10f; // Fixed Y spawn above the plane

    // Adjustable scale randomness
    public Vector2 scaleRange = new Vector2(0.5f, 1.5f); // Min and Max scale

    void Start()
    {
        if (goodRaindropPrefabs == null || goodRaindropPrefabs.Count == 0)
            Debug.LogError("No good raindrop prefabs assigned.");
        if (badRaindropPrefabs == null || badRaindropPrefabs.Count == 0)
            Debug.LogError("No bad raindrop prefabs assigned.");

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
        GameObject raindropPrefab = null;

        // Randomly decide to spawn a Good or Bad raindrop
        if (Random.value < 0.7f && goodRaindropPrefabs.Count > 0) // 70% chance for Good
        {
            int index = Random.Range(0, goodRaindropPrefabs.Count);
            raindropPrefab = goodRaindropPrefabs[index];
        }
        else if (badRaindropPrefabs.Count > 0) // 30% chance for Bad
        {
            int index = Random.Range(0, badRaindropPrefabs.Count);
            raindropPrefab = badRaindropPrefabs[index];
        }

        if (raindropPrefab != null)
        {
            // Randomize spawn position within the defined 3D bounds
            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnAreaX.x, spawnAreaX.y), // Random X position
                spawnHeight, // Fixed Y spawn height
                Random.Range(spawnAreaZ.x, spawnAreaZ.y) // Random Z position
            );

            // Randomize rotation
            Quaternion spawnRotation = Quaternion.Euler(
                Random.Range(0f, 360f), // Random X rotation
                Random.Range(0f, 360f), // Random Y rotation
                Random.Range(0f, 360f)  // Random Z rotation
            );

            // Instantiate the raindrop prefab with the randomized position and rotation
            GameObject raindrop = Instantiate(raindropPrefab, spawnPosition, spawnRotation);

            // Randomize scale
            float randomScale = Random.Range(scaleRange.x, scaleRange.y);
            raindrop.transform.localScale = Vector3.one * randomScale;

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
            Debug.LogError("Failed to select a raindrop prefab.");
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
