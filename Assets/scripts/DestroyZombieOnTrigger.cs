using UnityEngine;

public class DestroyZombieOnTrigger : MonoBehaviour
{
    public float destroyDelay = 1f; // Delay before destroying the zombie

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("zombie"))
        {
            Destroy(other.gameObject, destroyDelay);
        }
    }
}
