using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Unity.Cinemachine.Samples
{
    public abstract class SimplePlayerControllerBase : MonoBehaviour, Unity.Cinemachine.IInputAxisOwner
    {
        public float Speed = 1f;
        public float SprintSpeed = 4;
        public float JumpSpeed = 4;
        public float SprintJumpSpeed = 6;

        public Action PreUpdate;
        public Action<Vector3, float> PostUpdate;
        public Action StartJump;
        public Action EndJump;

        public InputAxis MoveX = InputAxis.DefaultMomentary;
        public InputAxis MoveZ = InputAxis.DefaultMomentary;
        public InputAxis Jump = InputAxis.DefaultMomentary;
        public InputAxis Sprint = InputAxis.DefaultMomentary;

        protected Vector2 m_LastRawInput = Vector2.zero; // Changed to protected

        public UnityEvent Landed = new();

        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref MoveX, Name = "Move X" });
            axes.Add(new() { DrivenAxis = () => ref MoveZ, Name = "Move Z" });
            axes.Add(new() { DrivenAxis = () => ref Jump, Name = "Jump" });
            axes.Add(new() { DrivenAxis = () => ref Sprint, Name = "Sprint" });
        }

        public virtual void SetStrafeMode(bool b) { }
        public abstract bool IsMoving { get; }
    }

    public class SimplePlayerController : SimplePlayerControllerBase
    {
        public float Damping = 0.5f;
        public bool Strafe = false;

        public enum ForwardModes { Camera, Player, World };
        public enum UpModes { Player, World };

        public ForwardModes InputForward = ForwardModes.Camera;
        public UpModes UpMode = UpModes.World;
        public Camera CameraOverride;
        public LayerMask GroundLayers = 1;
        public float Gravity = 10;

        float m_TimeLastGrounded = 0;
        Vector3 m_CurrentVelocityXZ;
        Vector3 m_LastInput;
        float m_CurrentVelocityY;
        bool m_IsSprinting;
        bool m_IsJumping;
        CharacterController m_Controller;

        public override void SetStrafeMode(bool b) => Strafe = b;
        public override bool IsMoving => m_LastInput.sqrMagnitude > 0.01f;
        public bool IsSprinting => m_IsSprinting;
        public bool IsJumping => m_IsJumping;
        public Camera Camera => CameraOverride == null ? Camera.main : CameraOverride;
        public bool IsGrounded() => GetDistanceFromGround(transform.position, UpDirection, 10) < 0.01f;

        const float kDelayBeforeInferringJump = 0.3f;
        void Start() => TryGetComponent(out m_Controller);

        private void OnEnable()
        {
            m_CurrentVelocityY = 0;
            m_IsSprinting = false;
            m_IsJumping = false;
            m_TimeLastGrounded = Time.time;
        }
//------------------------------------begin--------update-------------------------
  void Update()
        {
            PreUpdate?.Invoke();
            bool justLanded = ProcessJump();

            // Input handling
            Vector3 rawInput = new Vector3(MoveX.Value, 0, MoveZ.Value);
            rawInput = Vector3.ClampMagnitude(rawInput, 1f); // Ensure input vector magnitude is within range

            // Transform input based on the chosen reference frame
            Quaternion inputFrame = GetInputFrame(Vector3.Dot(rawInput, m_LastRawInput) < 0.8f);
            m_LastRawInput = new Vector2(rawInput.x, rawInput.z);
            Vector3 adjustedInput = inputFrame * rawInput;

            if (!m_IsJumping)
            {
                m_IsSprinting = Sprint.Value > 0.5f;
                Vector3 desiredVelocity = adjustedInput * (m_IsSprinting ? SprintSpeed : Speed);

                float damping = justLanded ? 0f : Damping;
                if (Vector3.Angle(m_CurrentVelocityXZ, desiredVelocity) < 100f)
                    m_CurrentVelocityXZ = Vector3.Slerp(m_CurrentVelocityXZ, desiredVelocity, Damper.Damp(1f, damping, Time.deltaTime));
                else
                    m_CurrentVelocityXZ += Damper.Damp(desiredVelocity - m_CurrentVelocityXZ, damping, Time.deltaTime);
            }

            ApplyMotion();

            // // Rotation logic for Strafe and Non-Strafe modes
            // if (!Strafe && m_CurrentVelocityXZ.sqrMagnitude > 0.001f)
            // {
            //     // Rotate character to face movement direction
            //     Quaternion targetRotation = Quaternion.LookRotation(m_CurrentVelocityXZ.normalized, UpDirection);
            //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Damper.Damp(1f, Damping, Time.deltaTime));
            // }
            // else if (Strafe)
            // {
            //     // Maintain fixed rotation for strafing
            //     Quaternion targetRotation = Quaternion.LookRotation(inputFrame * Vector3.forward, UpDirection);
            //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Damper.Damp(1f, Damping, Time.deltaTime));
            // }

            // // Notify post-update actions
            // PostUpdate?.Invoke(Quaternion.Inverse(transform.rotation) * m_CurrentVelocityXZ, m_IsSprinting ? JumpSpeed / SprintJumpSpeed : 1f);
                        if (!Strafe && m_CurrentVelocityXZ.sqrMagnitude > 0.001f)
            {
                // Flip the rotation direction
                Vector3 rotationDirection = -m_CurrentVelocityXZ.normalized; // Invert direction
                var targetRotation = Quaternion.LookRotation(rotationDirection, UpDirection);

                // Smoothly interpolate to the target rotation
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    Damper.Damp(1f, Damping, Time.deltaTime));
            }

       

       
       
       
       
        }

// ------------------------END----OF------UPDATE-----------------------
        Vector3 UpDirection => UpMode == UpModes.World ? Vector3.up : transform.up;
        Quaternion GetInputFrame(bool inputDirectionChanged)
        {
            var frame = Quaternion.identity;
            switch (InputForward)
            {
                case ForwardModes.Camera: frame = Camera.transform.rotation; break;
                case ForwardModes.Player: return transform.rotation;
            }
            var playerUp = transform.up;
            var up = frame * Vector3.up;
            var axis = Vector3.Cross(up, playerUp);
            if (axis.sqrMagnitude < 0.001f)
                return frame;
            var angle = UnityVectorExtensions.SignedAngle(up, playerUp, axis);
            return Quaternion.AngleAxis(angle, axis) * frame;
        }

        void ApplyMotion()
        {
            Vector3 motion = m_CurrentVelocityXZ;
            motion.y = m_CurrentVelocityY;
            if (m_Controller != null)
                m_Controller.Move(motion * Time.deltaTime);
            else
                transform.position += motion * Time.deltaTime;
        }

         bool ProcessJump()
        {
            bool justLanded = false;
            var now = Time.time;
            bool grounded = IsGrounded();

            m_CurrentVelocityY -= Gravity * Time.deltaTime;

            if (!m_IsJumping)
            {
                // Process jump command
                if (grounded && Jump.Value > 0.01f)
                {
                    m_IsJumping = true;
                    m_CurrentVelocityY = m_IsSprinting ? SprintJumpSpeed : JumpSpeed;
                }
                // If we are falling, assume the jump pose
                if (!grounded && now - m_TimeLastGrounded > kDelayBeforeInferringJump)
                    m_IsJumping = true;
 
                if (m_IsJumping)
                {
                    StartJump?.Invoke();
                    grounded = false;
                }
            }

            if (grounded)
            {
                m_TimeLastGrounded = Time.time;
                m_CurrentVelocityY = 0;

                // If we were jumping, complete the jump
                if (m_IsJumping)
                {
                    EndJump?.Invoke();
                    m_IsJumping = false;
                    justLanded = true;
                    Landed.Invoke();
                }
            }
            return justLanded;
        }

        float GetDistanceFromGround(Vector3 pos, Vector3 down, float maxDistance)
        {
            if (m_Controller != null && m_Controller.isGrounded)
                return 0;

            if (Physics.Raycast(pos, down, out var hitInfo, maxDistance, GroundLayers))
                return hitInfo.distance;

            return maxDistance;
        }
    }
}
