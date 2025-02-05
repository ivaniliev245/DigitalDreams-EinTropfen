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

    public GameObject decalPrefab; // Prefab for the decal object
    public float decalLifetime = 3.0f; // Time before the decal is destroyed

    public AudioClip collisionSound; // Sound to play on first collision
    public AudioClip destructionSound; // Sound to play on destruction

    public float mass = 1.0f; // Default mass for the droplet
    public float drag = 0.0f; // Default drag for the droplet

    private Rigidbody rb; // Reference to the Rigidbody component
    private AudioSource audioSource; // AudioSource for playing sounds
    private bool hasCollided = false; // Tracks if the droplet has already collided

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the raindrop!");
        }
        else
        {
            rb.mass = mass;
            rb.linearDamping = drag;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.Log("AudioSource component is missing from the raindrop!");
        }

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
        yield return new WaitForSeconds(lifetime);

        if (destructionVFX != null)
        {
            GameObject vfxInstance = Instantiate(destructionVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, vfxLifetime);
        }

        SpawnAndPlaySound(destructionSound);
        SpawnDecal();

        Destroy(gameObject);
    }

    void SpawnDecal()
    {
        if (decalPrefab != null)
        {
            GameObject decalInstance = Instantiate(decalPrefab, transform.position, Quaternion.identity);
            Destroy(decalInstance, decalLifetime);
        }
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
            GameObject soundObject = new GameObject("SoundEffect");
            AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();
            tempAudioSource.clip = clip;
            tempAudioSource.Play();
            Destroy(soundObject, clip.length);
        }
    }

    public bool IsInProximity()
    {
        if (warriorTransform != null)
        {
            float distance = Vector3.Distance(transform.position, warriorTransform.position);
            return distance <= proximityThreshold;
        }
        return false;
    }
}
