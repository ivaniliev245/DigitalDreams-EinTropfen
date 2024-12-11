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
    public float hitSpeed = 2.0f; // Speed of the droplet moving toward the camera

    void Update()
    {
        // Check for spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpacebarPress();
        }
    }

    void HandleSpacebarPress()
    {
        // Find all raindrops in the scene
        RainDroplet[] raindrops = FindObjectsOfType<RainDroplet>();
        bool foundRaindrop = false;

        foreach (RainDroplet raindrop in raindrops)
        {
            if (raindrop.IsInProximity())
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
        Transform dropletTransform = raindrop.transform;
        Vector3 startPosition = dropletTransform.position;
        Vector3 targetPosition = cameraTransform.position;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * hitSpeed;
            dropletTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        // Trigger Cinemachine Impulse for camera shake
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        // Create a hit effect at the camera
        if (dropletHitEffect != null)
        {
            Instantiate(dropletHitEffect, cameraTransform.position, Quaternion.identity);
        }

        // Destroy the droplet
        Destroy(raindrop.gameObject);
    }

    // Update the score display
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // Display the score
        }
    }
}
