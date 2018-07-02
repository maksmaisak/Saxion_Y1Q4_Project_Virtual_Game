using System;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class HeadBob : MonoBehaviour
    {
        public Transform bobTransform;
        public CurveControlledBob motionBob = new CurveControlledBob();
        public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();
        public RigidbodyFirstPersonController rigidbodyFirstPersonController;
        public float strideInterval = 4;
        [Range(0f, 1f)] public float runningStrideLengthen = 0.722f;

        private bool wasPreviouslyAirborne;
        private Vector3 originalCameraLocalPosition;
        private Vector3 motionBobOffset;

        void Start()
        {
            bobTransform = bobTransform ? bobTransform : transform;
            
            motionBob.Setup(bobTransform, strideInterval);
            originalCameraLocalPosition = bobTransform.localPosition;
        }

        void Update()
        {
            float currentSpeed = rigidbodyFirstPersonController.velocity.magnitude;
            bool isAirborne = rigidbodyFirstPersonController.isAirborne;
                        
            if (currentSpeed > 0f && !isAirborne)
            {
                float modifier = rigidbodyFirstPersonController.isRunning ? runningStrideLengthen : 1f;
                motionBobOffset = motionBob.DoHeadBob(currentSpeed * modifier);
            }

            Vector3 jumpAndLandingOffset = Vector3.down * jumpAndLandingBob.offset;
            bobTransform.localPosition = originalCameraLocalPosition + motionBobOffset + jumpAndLandingOffset;

            if (wasPreviouslyAirborne && !isAirborne)
            {
                StartCoroutine(jumpAndLandingBob.DoBobCycle());
            }

            wasPreviouslyAirborne = isAirborne;
        }
    }
}
