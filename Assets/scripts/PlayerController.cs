using UnityEngine;
using TMPro;
using Unity.Cinemachine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform warriorTransform; // Warrior's Transform
    public float proximityThreshold = 1.0f; // Proximity for interaction
    public int score = 0; // Player's score
    public TextMeshProUGUI scoreText; // Reference to the TextMeshProUGUI component

    public CinemachineImpulseSource impulseSource; // Cinemachine Impulse Source
    public GameObject dropletHitEffect; // Effect when the droplet hits the camera
    public Transform cameraTransform; // Reference to the camera transform
    public float hitSpeed = 2.0f; // Base speed of the droplet moving toward the camera

    public Transform hitPositionTransform; // Empty transform to define the hit position
    public float hitRadius = 1.0f; // Radius around the hit position where interaction is allowed

    public float hitDelay = 0.0f; // Adjustable delay before the hit occurs

    void Update()
    {
        // Check for spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(HandleSpacebarPressWithDelay());
        }
    }

    IEnumerator HandleSpacebarPressWithDelay()
    {
        // Wait for the specified delay
        if (hitDelay > 0)
        {
            yield return new WaitForSeconds(hitDelay);
        }

        // Proceed to handle the raindrop interaction
        HandleSpacebarPress();
    }

    void HandleSpacebarPress()
    {
        // Find all raindrops in the scene
        RainDroplet[] raindrops = FindObjectsOfType<RainDroplet>();
        bool foundRaindrop = false;

        foreach (RainDroplet raindrop in raindrops)
        {
            if (IsInHitZone(raindrop.transform.position))
            {
                foundRaindrop = true;

                if (raindrop.isGood)
                {
                    // Good raindrop interaction
                    score += 1;
                    Debug.Log("Good Raindrop! Score: " + score);
                }
                else
                {
                    // Bad raindrop interaction
                    score -= 3;
                    Debug.Log("Bad Raindrop! Score: " + score);
                }

                // Move the raindrop toward the camera and handle hit
                StartCoroutine(MoveDropletToCamera(raindrop));

                break;
            }
        }

        if (!foundRaindrop)
        {
            // Penalize for pressing space with no raindrop nearby
            score -= 1;
            Debug.Log("Missed! Score: " + score);
        }

        // Update the score text in the UI
        UpdateScoreText();
    }

    IEnumerator MoveDropletToCamera(RainDroplet raindrop)
    {
        Rigidbody rb = raindrop.GetComponent<Rigidbody>();
        Transform dropletTransform = raindrop.transform;
        Vector3 startPosition = dropletTransform.position;
        Vector3 targetPosition = cameraTransform.position;

        float progress = 0f;
        float adjustedSpeed = hitSpeed / (rb != null ? rb.mass : 1.0f); // Adjust speed based on mass

        while (progress < 1f)
        {
            progress += Time.deltaTime * adjustedSpeed;
            dropletTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        // Trigger Cinemachine Impulse for camera shake
        if (impulseSource != null)
        {
            float impulseStrength = rb != null ? Mathf.Clamp(rb.mass, 1.0f, 10.0f) : 1.0f; // Scale impulse between 1 and 10
            impulseSource.GenerateImpulse(impulseStrength);
        }

        // Create a hit effect at the camera
        if (dropletHitEffect != null)
        {
            Instantiate(dropletHitEffect, cameraTransform.position, Quaternion.identity);
        }

        // Destroy the droplet
        Destroy(raindrop.gameObject);
    }

    // Check if a position is within the hit zone
    bool IsInHitZone(Vector3 position)
    {
        if (hitPositionTransform == null)
        {
            Debug.LogError("Hit Position Transform is not assigned in the PlayerController!");
            return false;
        }

        return Vector3.Distance(position, hitPositionTransform.position) <= hitRadius;
    }

    // Update the score display
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // Display the score
        }
    }

    // Draw the hit zone in the Unity Editor
    void OnDrawGizmos()
    {
        if (hitPositionTransform != null)
        {
            Gizmos.color = Color.red; // Set gizmo color to red
            Gizmos.DrawWireSphere(hitPositionTransform.position, hitRadius); // Draw a red wire sphere for the hit zone
        }
    }
}
