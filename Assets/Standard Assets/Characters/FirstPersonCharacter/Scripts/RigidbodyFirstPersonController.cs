using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        private enum State
        {
            Grounded,
            Airborne,
            OnWall
        }

        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed  = 8.0f;  // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed   = 4.0f;  // Speed when walking sideways
            public float RunMultiplier = 2.0f;  // Speed when sprinting
            public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public float WallJumpUpwardsModifier = 1f;
            public float WallJumpSidewaysModifier = 1f;
            public float WallJumpAwayFromWallModifier = 1f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;
#if !MOBILE_INPUT
            private bool m_Running;
#endif
            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input == Vector2.zero) return;
                if (input.x > 0 || input.x < 0)
                {
                    //strafe
                    CurrentTargetSpeed = StrafeSpeed;
                }
                if (input.y < 0)
                {
                    //backwards
                    CurrentTargetSpeed = BackwardSpeed;
                }
                if (input.y > 0)
                {
                    //forwards
                    //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                    CurrentTargetSpeed = ForwardSpeed;
                }
#if !MOBILE_INPUT
                if (Input.GetKey(RunKey))
                {
                    CurrentTargetSpeed *= RunMultiplier;
                    m_Running = true;
                }
                else
                {
                    m_Running = false;
                }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }

        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character

            public float wallCheckDistance = 0.01f;
            public float stickToWallHelperDistance = 0.5f;
            public float stickToWallHelperForce = 1f;

            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl;
            [Tooltip("When in the air, should it change its velocity to move where the camera is looking?")]
            public bool alwaysForwardWhenAirborne;
            [Tooltip("When in the air, input will be scaled by this number. Set it to a smaller value to have less contol when in the air.")]
            public float airControlMultiplier = 1f;
            [Tooltip("Set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }

        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();
        public LayerMask groundAndWallDetectionLayerMask = Physics.DefaultRaycastLayers;

        [Range(0f, 90f)] public float awayFromWallLeanAngle = 10f;
        public float awayFromWallLeanAngleChangePerSecond = 180f;

        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_SurfaceContactNormal;
        private bool m_Jump, m_Jumping;
        private float m_defaultDrag;

        private State m_State;
        private State m_PreviousState;

        public Vector3 velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool isGrounded
        {
            get { return m_State == State.Grounded; }
        }

        public bool isAirborne
        {
            get { return m_State == State.Airborne; }
        }

        public bool isOnWall
        {
            get { return m_State == State.OnWall; }
        }

        public bool isJumping
        {
            get { return m_Jumping; }
        }

        public bool isRunning
        {
            get
            {
#if !MOBILE_INPUT
                return movementSettings.Running;
#else
	            return false;
#endif
            }
        }

        void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform);

            m_defaultDrag = m_RigidBody.drag;
        }

        void Update()
        {
            RotateView();

            if (!m_Jump && CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                m_Jump = true;
            }
        }

        void FixedUpdate()
        {
            StateCheck();

            Vector2 input = GetInput();
            movementSettings.UpdateDesiredTargetSpeed(input);

            if (IsNonZero(input))
            {
                // Always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;

                if (m_State == State.Grounded)
                {
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, m_SurfaceContactNormal).normalized;
                    desiredMove *= movementSettings.CurrentTargetSpeed;

                    // TODO this speed limiting thing should be in other states as well (to some degree)
                    float targetSpeed = movementSettings.CurrentTargetSpeed;
                    if (m_RigidBody.velocity.sqrMagnitude < targetSpeed * targetSpeed)
                    {
                        m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                    }
                }
                else if (m_State == State.OnWall)
                {
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, Vector3.up).normalized;
                    desiredMove *= movementSettings.CurrentTargetSpeed;

                    m_RigidBody.AddForce(-Physics.gravity, ForceMode.Acceleration);
                    m_RigidBody.AddForce(desiredMove, ForceMode.Impulse);
                }
                else if (m_State == State.Airborne && advancedSettings.airControl)
                {
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, Vector3.up).normalized;
                    desiredMove *= movementSettings.ForwardSpeed * advancedSettings.airControlMultiplier;

                    m_RigidBody.AddForce(desiredMove, ForceMode.Impulse);
                }
            }

            if (m_State == State.Grounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = m_defaultDrag;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && IsZero(input) && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else if (m_State == State.OnWall)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    Vector3 awayFromWall = m_SurfaceContactNormal;
                    Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, Vector3.up);
                    //desiredMove -= Vector3.Project(desiredMove, awayFromWall);

                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);

                    Vector3 force = (
                        Vector3.up * movementSettings.WallJumpUpwardsModifier +
                        desiredMove.normalized * movementSettings.WallJumpSidewaysModifier +
                        m_SurfaceContactNormal * movementSettings.WallJumpAwayFromWallModifier
                    ) * movementSettings.JumpForce;

                    m_RigidBody.drag = m_defaultDrag;
                    m_RigidBody.AddForce(force, ForceMode.Impulse);
                    m_Jumping = true;
                }
                else
                {
                    StickToWallHelper(); // TODO also use it when detaching without jumping.
                }
            }
            else if (m_State == State.Airborne)
            {
                m_RigidBody.drag = m_defaultDrag;

                if (!m_Jumping)
                {
                    if (m_PreviousState == State.Grounded)
                    {
                        StickToGroundHelper();
                    }
                }
            }

            m_Jump = false;
        }

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_SurfaceContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }

        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.stickToGroundHelperDistance, groundAndWallDetectionLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }

        private void StickToWallHelper()
        {
            RaycastHit hitInfo;
            if (RaycastWalls(advancedSettings.stickToWallHelperDistance, out hitInfo))
            {
                Vector3 awayFromWall = hitInfo.normal;
                m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, awayFromWall);
                m_RigidBody.AddForce(-awayFromWall * advancedSettings.stickToWallHelperForce, ForceMode.Impulse);
            }
        }

        private Vector2 GetInput()
        {
            return new Vector2(
                CrossPlatformInputManager.GetAxis("Horizontal"),
                CrossPlatformInputManager.GetAxis("Vertical")
            );
        }

        private void RotateView()
        {
            // avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            Quaternion leanOffset;
            if (m_State == State.OnWall)
            {
                Vector3 wallNormal = m_SurfaceContactNormal;
                Vector3 wallTangent = Vector3.Cross(wallNormal, Vector3.up);

                leanOffset = Quaternion.Euler(
                    awayFromWallLeanAngle * Vector3.Dot(cam.transform.forward, wallNormal), 
                    0f, 
                    -awayFromWallLeanAngle * Vector3.Dot(cam.transform.forward, wallTangent)
                );
                Debug.Log(leanOffset.eulerAngles);
            }
            else
            {
                leanOffset = Quaternion.identity;
            }
            mouseLook.cameraRotationOffset = Quaternion.RotateTowards(
                mouseLook.cameraRotationOffset, 
                leanOffset, 
                awayFromWallLeanAngleChangePerSecond * Time.deltaTime
            );

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;
            mouseLook.LookRotation(transform, cam.transform);
            if (m_State != State.Airborne || advancedSettings.alwaysForwardWhenAirborne)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
        }

        private void StateCheck()
        {
            m_PreviousState = m_State;

            GroundCheck();
            if (m_State == State.Airborne)
            {
                WallCheck();
            }

            if (m_PreviousState == State.Airborne && m_State != State.Airborne && m_Jumping)
            {
                m_Jumping = false;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, groundAndWallDetectionLayerMask, QueryTriggerInteraction.Ignore))
            {
                m_State = State.Grounded;
                m_SurfaceContactNormal = hitInfo.normal;
            }
            else
            {
                m_State = State.Airborne;
                m_SurfaceContactNormal = Vector3.up;
            }
        }

        private void WallCheck()
        {
            RaycastHit hitInfo;
            if (RaycastWalls(advancedSettings.wallCheckDistance, out hitInfo))
            {
                Debug.Log("Went to State.OnWall because of " + hitInfo.collider.gameObject);
                m_State = State.OnWall;
                m_SurfaceContactNormal = hitInfo.normal;
            }
            else
            {
                m_State = State.Airborne;
                m_SurfaceContactNormal = Vector3.up;
            }
        }

        private bool RaycastWalls(float maxDistance, out RaycastHit hitInfo)
        {
            Vector3 position = transform.position;

            Transform cameraTransform = cam.transform;
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            if (CastAgainstWall(position, right, maxDistance, out hitInfo)) return true;
            if (CastAgainstWall(position, -right, maxDistance, out hitInfo)) return true;
            if (CastAgainstWall(position, forward, maxDistance, out hitInfo)) return true;
            if (CastAgainstWall(position, -forward, maxDistance, out hitInfo)) return true;

            return false;
        }

        private bool CastAgainstWall(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hitInfo)
        {
            return Physics.SphereCast(
                new Ray(position, direction),
                radius: m_Capsule.radius * (1.0f - advancedSettings.shellOffset),
                hitInfo: out hitInfo,
                maxDistance: maxDistance,
                layerMask: groundAndWallDetectionLayerMask,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore
            );
        }

        private static bool IsNonZero(Vector2 vector)
        {
            return Mathf.Abs(vector.x) > float.Epsilon || Mathf.Abs(vector.y) > float.Epsilon;
        }

        private static bool IsZero(Vector2 vector)
        {
            return Mathf.Abs(vector.x) <= float.Epsilon && Mathf.Abs(vector.y) <= float.Epsilon;
        }
    }
}
