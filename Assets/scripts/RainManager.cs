using UnityEngine;
using System.Collections;

public class RainManager : MonoBehaviour
{
    public GameObject goodRaindropPrefab; // Good Raindrop prefab
    public GameObject badRaindropPrefab; // Bad Raindrop prefab
    public Transform warriorTransform;   // Reference to the warrior
    public float rainSpawnRate = 2.0f;   // Spawn interval in seconds
    public float proximityThreshold = 1.0f; // Proximity distance for checking drops

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
            // Spawn position within a defined 2D plane (X, Y only, ignore Z)
            Vector3 spawnPosition = new Vector3(
                Random.Range(-5f, 5f), // Random X position
                10f, // Fixed Y spawn above the screen (adjust as needed)
                0f // Set Z to 0 to keep it in the 2D plane
            );

            GameObject raindrop = Instantiate(raindropPrefab, spawnPosition, Quaternion.identity);
            RainDroplet raindropScript = raindrop.GetComponent<RainDroplet>();

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

    // This method will draw the proximity threshold radius in the Scene view using Gizmos.
    void OnDrawGizmos()
    {
        // Check if the warriorTransform is set before drawing the Gizmo
        if (warriorTransform != null)
        {
            // Draw a wireframe sphere with the radius of the proximity threshold
            Gizmos.color = Color.green; // You can change the color to your preference
            Gizmos.DrawWireSphere(warriorTransform.position, proximityThreshold);
        }
    }
}
