using UnityEngine;
using System.Collections;

public class RainDroplet : MonoBehaviour
{
    public Transform warriorTransform; // Reference to the warrior's position
    public float proximityThreshold = 1.0f; // Proximity distance for checking interaction
    public bool isGood = true; // Is the raindrop good or bad
    public float lifetime = 5.0f; // Time before the raindrop is destroyed

    public GameObject destructionVFX; // Prefab for the destruction VFX
    public float vfxLifetime = 2.0f; // Time before the VFX is destroyed

    private Rigidbody rb; // Reference to the Rigidbody component

    void Start()
    {
        // Cache the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the raindrop!");
        }

        // Start the countdown for destroying the raindrop after the specified lifetime
        StartCoroutine(DestroyRaindropAfterTime(lifetime));
    }

    IEnumerator DestroyRaindropAfterTime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime); // Wait for the lifetime before destroying

        // Spawn the destruction VFX
        if (destructionVFX != null)
        {
            GameObject vfxInstance = Instantiate(destructionVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, vfxLifetime); // Destroy the VFX after its lifetime
        }

        Destroy(gameObject); // Destroy the raindrop
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
