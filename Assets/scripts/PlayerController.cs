using UnityEngine;
using TMPro; // Add this for TextMeshPro

public class PlayerController : MonoBehaviour
{
    public Transform warriorTransform; // Warrior's Transform
    public float proximityThreshold = 1.0f; // Proximity for interaction
    public int score = 0; // Player's score
    public TextMeshProUGUI scoreText; // Reference to the TextMeshProUGUI component

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
        // Find all raindrops (now RainDroplets) in the scene
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

                // Destroy the raindrop after interaction
                Destroy(raindrop.gameObject);
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

    // Update the score display
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // Display the score
        }
    }
}
