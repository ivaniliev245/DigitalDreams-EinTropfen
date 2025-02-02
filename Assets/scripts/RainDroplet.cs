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

    public AudioClip collisionSound; // Sound to play on first collision
    public AudioClip destructionSound; // Sound to play on destruction

    public float mass = 1.0f; // Default mass for the droplet
    public float drag = 0.0f; // Default drag for the droplet

    private Rigidbody rb; // Reference to the Rigidbody component
    private AudioSource audioSource; // AudioSource for playing sounds
    private bool hasCollided = false; // Tracks if the droplet has already collided

    void Start()
    {
        // Cache the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the raindrop!");
        }
        else
        {
            // Set Rigidbody properties
            rb.mass = mass;
            rb.linearDamping = drag;
        }

        // Cache the AudioSource component (should already be attached to the GameObject)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
           Debug.Log("AudioSource component is missing from the raindrop!");

        }

        // Start the countdown for destroying the raindrop after the specified lifetime
        StartCoroutine(DestroyRaindropAfterTime(lifetime));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasCollided) // Ensure the sound plays only on the first collision
        {
            hasCollided = true;
            PlaySound(collisionSound);
        }
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

        // Play destruction sound
        SpawnAndPlaySound(destructionSound);

        Destroy(gameObject); // Destroy the raindrop
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void SpawnAndPlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            // Create a new GameObject for the sound
            GameObject soundObject = new GameObject("SoundEffect");
            AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();
            tempAudioSource.clip = clip;
            tempAudioSource.Play();

            // Destroy the sound object after the clip has finished playing
            Destroy(soundObject, clip.length);
        }
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
