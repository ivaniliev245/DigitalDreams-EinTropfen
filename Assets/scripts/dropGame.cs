using UnityEngine;
using System.Collections;

public class DropGame : MonoBehaviour
{
    public GameObject dropPrefab;
    public AudioClip[] dropSounds;
    public AudioClip harmonicSound;
    public AudioClip disharmonicSound;
    public float spawnInterval = 1f;
    public float inputWindow = 0.2f;
    public int requiredHitsForHarmony = 5;

    private AudioSource audioSource;
    private int consecutiveHits = 0;
    private float lastDropTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SpawnDrops());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckPlayerInput();
        }
    }

    IEnumerator SpawnDrops()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnDrop();
        }
    }

void SpawnDrop()
{
    GameObject drop = Instantiate(dropPrefab, new Vector3(Random.Range(-5f, 5f), 5f, 0), Quaternion.identity);

    if (dropSounds.Length > 0)
    {
        AudioClip dropSound = dropSounds[Random.Range(0, dropSounds.Length)];
        audioSource.PlayOneShot(dropSound);
    }
    else
    {
        Debug.LogWarning("No drop sounds assigned in the dropSounds array.");
    }

    lastDropTime = Time.time;
    StartCoroutine(AnimateDrop(drop));
}


    IEnumerator AnimateDrop(GameObject drop)
    {
        float startY = drop.transform.position.y;
        float endY = -5f;
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float newY = Mathf.Lerp(startY, endY, t);
            drop.transform.position = new Vector3(drop.transform.position.x, newY, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        CreateRipple(drop.transform.position);
        Destroy(drop);
    }

    void CreateRipple(Vector3 position)
    {
        // Hier würde man einen visuellen Effekt für die Welle erstellen
        Debug.Log("Ripple created at " + position);
    }

    void CheckPlayerInput()
    {
        float timeSinceLastDrop = Time.time - lastDropTime;

        if (timeSinceLastDrop <= inputWindow)
        {
            consecutiveHits++;
            audioSource.PlayOneShot(harmonicSound);
            BrightenScreen();
            if (consecutiveHits >= requiredHitsForHarmony)
            {
                EnterHarmonyState();
            }
        }
        else
        {
            consecutiveHits = 0;
            audioSource.PlayOneShot(disharmonicSound);
            DarkenScreen();
        }
    }

    void BrightenScreen()
    {
        // Hier würde man den Bildschirm aufhellen
        Debug.Log("Screen brightened");
    }

    void DarkenScreen()
    {
        // Hier würde man den Bildschirm verdunkeln
        Debug.Log("Screen darkened");
    }

    void EnterHarmonyState()
    {
        // Hier würde man den harmonischen Endzustand einleiten
        Debug.Log("Entering harmony state");
        StopAllCoroutines();
        // Weitere Logik für den Endzustand...
    }
}