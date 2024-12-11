using UnityEngine;

public class SimplePlayerAnimator : MonoBehaviour
{
    public float NormalWalkSpeed = 1.7f;
    public float NormalSprintSpeed = 5;
    public float MaxSprintScale = 1.4f;
    public float JumpAnimationScale = 0.65f;
    public float HitResetDelay = 0.2f;

    private Animator animator;
    private Vector3 previousPosition;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing");
            enabled = false;
        }
        previousPosition = transform.position;
    }

    private void Update()
    {
        HandleSpaceInput();
        UpdateMovementAnimation();
    }

    private void HandleSpaceInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("hit", true);
            Invoke(nameof(ResetHit), HitResetDelay);
        }
    }

    private void ResetHit()
    {
        animator.SetBool("hit", false);
    }

    private void UpdateMovementAnimation()
    {
        Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        bool isWalking = speed > NormalWalkSpeed * 0.5f && speed <= NormalWalkSpeed;
        bool isRunning = speed > NormalWalkSpeed;

        animator.SetFloat("Speed", speed);
        animator.SetBool("Walking", isWalking);
        animator.SetBool("Running", isRunning);
        animator.SetFloat("MotionScale", Mathf.Clamp(speed / NormalSprintSpeed, 0, MaxSprintScale));
    }
}