using UnityEngine;
using System.Collections; // Add this line to use IEnumerator and coroutines

public class RainDroplet : MonoBehaviour
{
    public Transform warriorTransform; // Reference to the warrior's position
    public float proximityThreshold = 1.0f; // Proximity distance for checking interaction
    public bool isGood = true; // Is the raindrop good or bad
    public float lifetime = 5.0f; // Time before the raindrop is destroyed

    private float fallSpeed = 5.0f; // How fast the raindrop falls

    void Start()
    {
        // Start the countdown for destroying the raindrop after the specified lifetime
        StartCoroutine(DestroyRaindropAfterTime(lifetime));
    }

    void Update()
    {
        // Move the raindrop downwards
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    IEnumerator DestroyRaindropAfterTime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime); // Wait for the lifetime before destroying
        Destroy(gameObject); // Destroy the raindrop after the specified lifetime
    }

    public bool IsInProximity()
    {
        // Check if the raindrop is within the proximity threshold of the warrior
        if (warriorTransform != null)
        {
            float distance = Vector3.Distance(transform.position, warriorTransform.position);
            return distance <= proximityThreshold;
        }
        return false;
    }
}
