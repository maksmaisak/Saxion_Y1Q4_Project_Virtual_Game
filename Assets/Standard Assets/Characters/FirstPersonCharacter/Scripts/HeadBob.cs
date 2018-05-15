using System;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class HeadBob : MonoBehaviour
    {
        public new Camera camera;
        public CurveControlledBob motionBob = new CurveControlledBob();
        public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();
        public RigidbodyFirstPersonController rigidbodyFirstPersonController;
        public float StrideInterval;
        [Range(0f, 1f)] public float RunningStrideLengthen;

        private bool m_PreviouslyAirborne;
        private Vector3 m_OriginalCameraPosition;

        void Start()
        {
            motionBob.Setup(camera, StrideInterval);
            m_OriginalCameraPosition = camera.transform.localPosition;
        }

        void Update()
        {
            Vector3 newCameraPosition;
            if (rigidbodyFirstPersonController.velocity.magnitude > 0f && !rigidbodyFirstPersonController.isAirborne)
            {
                camera.transform.localPosition = motionBob.DoHeadBob(rigidbodyFirstPersonController.velocity.magnitude * (rigidbodyFirstPersonController.isRunning ? RunningStrideLengthen : 1f));
                newCameraPosition = camera.transform.localPosition;
                newCameraPosition.y = camera.transform.localPosition.y - jumpAndLandingBob.Offset();
            }
            else
            {
                newCameraPosition = camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - jumpAndLandingBob.Offset();
            }
            camera.transform.localPosition = newCameraPosition;

            if (m_PreviouslyAirborne && !rigidbodyFirstPersonController.isAirborne)
            {
                StartCoroutine(jumpAndLandingBob.DoBobCycle());
            }

            m_PreviouslyAirborne = rigidbodyFirstPersonController.isAirborne;
        }
    }
}
