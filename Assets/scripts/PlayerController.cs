using UnityEngine;
using TMPro;
using Unity.Cinemachine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform warriorTransform;
    public float proximityThreshold = 1.0f;
    public int score = 0;
    public TextMeshProUGUI scoreText;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(HandleSpacebarPressWithDelay());
        }
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

                // Trigger the hit sound from the assigned hitAudioSource
                if (hitAudioSource != null && !hitAudioSource.isPlaying)
                {
                    hitAudioSource.Play();
                }

                if (raindrop.isGood)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }

                StartCoroutine(MoveDropletToCamera(raindrop, previousScore));
                break;
            }
        }

        if (!foundRaindrop)
        {
            previousScore = score;
            score -= 1;

            // Trigger collision sound from the assigned collisionAudioSource
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

        // Trigger flying sound from the assigned flyingAudioSource
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

    void OnDrawGizmos()
    {
        if (hitPositionTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPositionTransform.position, hitRadius);
        }
    }
}
