using UnityEngine;
using TMPro;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform warriorTransform;
    public float proximityThreshold = 1.0f;
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText; // Added for displaying the timer

    public CinemachineImpulseSource impulseSource;
    public GameObject dropletHitEffect;
    public Transform cameraTransform;
    public float hitSpeed = 2.0f;

    public Transform hitPositionTransform;
    public float hitRadius = 1.0f;

    public float hitDelay = 0.0f;

    public AudioSource hitAudioSource;
    public AudioSource collisionAudioSource;
    public AudioSource flyingAudioSource;

    public float timerDuration = 60.0f; // Adjustable timer in seconds
    public string timeoutSceneName;     // Assignable scene for timer end
    public string scoreTargetSceneName; // Assignable scene for reaching 30 points

    private float timer;
    private HashSet<GameObject> trackedBadObjects = new HashSet<GameObject>();

    void Start()
    {
        timer = timerDuration;
        UpdateTimerText(); // Initialize timer display
    }

    void Update()
    {
        timer -= Time.deltaTime;
        UpdateTimerText(); // Update timer display

        if (timer <= 0)
        {
            SceneManager.LoadScene(timeoutSceneName);
        }

        if (score >= 30)
        {
            SceneManager.LoadScene(scoreTargetSceneName);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(HandleSpacebarPressWithDelay());
        }

        DetectBadObjects();
    }

    void DetectBadObjects()
    {
        GameObject[] badObjects = GameObject.FindGameObjectsWithTag("bad");
        foreach (GameObject badObject in badObjects)
        {
            if (!trackedBadObjects.Contains(badObject))
            {
                trackedBadObjects.Add(badObject);
                score -= 1;
                UpdateScoreText();
            }
        }

        trackedBadObjects.RemoveWhere(obj => obj == null);
    }

    IEnumerator HandleSpacebarPressWithDelay()
    {
        if (hitDelay > 0)
        {
            yield return new WaitForSeconds(hitDelay);
        }
        HandleSpacebarPress();
    }

    void HandleSpacebarPress()
    {
        RainDroplet[] raindrops = FindObjectsOfType<RainDroplet>();
        bool foundRaindrop = false;
        int previousScore = score;

        foreach (RainDroplet raindrop in raindrops)
        {
            if (IsInHitZone(raindrop.transform.position))
            {
                foundRaindrop = true;

                if (hitAudioSource != null && !hitAudioSource.isPlaying)
                {
                    hitAudioSource.Play();
                }

                if (raindrop.isGood)
                {
                    score += 5;
                }

                StartCoroutine(MoveDropletToCamera(raindrop, previousScore));
                break;
            }
        }

        if (!foundRaindrop)
        {
            previousScore = score;
            score -= 1;

            if (previousScore != score && collisionAudioSource != null && !collisionAudioSource.isPlaying)
            {
                collisionAudioSource.Play();
            }
        }

        UpdateScoreText();
    }

    IEnumerator MoveDropletToCamera(RainDroplet raindrop, int previousScore)
    {
        Rigidbody rb = raindrop.GetComponent<Rigidbody>();
        Transform dropletTransform = raindrop.transform;
        Vector3 startPosition = dropletTransform.position;
        Vector3 targetPosition = cameraTransform.position;

        float progress = 0f;
        float adjustedSpeed = hitSpeed / (rb != null ? rb.mass : 1.0f);

        if (flyingAudioSource != null && !flyingAudioSource.isPlaying)
        {
            flyingAudioSource.Play();
        }

        while (progress < 1f)
        {
            progress += Time.deltaTime * adjustedSpeed;
            dropletTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        if (flyingAudioSource != null && flyingAudioSource.isPlaying)
        {
            flyingAudioSource.Stop();
        }

        if (previousScore != score && collisionAudioSource != null && !collisionAudioSource.isPlaying)
        {
            collisionAudioSource.Play();
        }

        if (impulseSource != null)
        {
            float impulseStrength = rb != null ? Mathf.Clamp(rb.mass, 1.0f, 10.0f) : 1.0f;
            impulseSource.GenerateImpulse(impulseStrength);
        }

        if (dropletHitEffect != null)
        {
            Instantiate(dropletHitEffect, cameraTransform.position, Quaternion.identity);
        }

        Destroy(raindrop.gameObject);
    }

    bool IsInHitZone(Vector3 position)
    {
        if (hitPositionTransform == null)
        {
            Debug.LogError("Hit Position Transform is not assigned in the PlayerController!");
            return false;
        }
        return Vector3.Distance(position, hitPositionTransform.position) <= hitRadius;
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.CeilToInt(timer) + "s";
        }
    }

    void OnDrawGizmos()
    {
        if (hitPositionTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPositionTransform.position, hitRadius);
        }
    }
}
