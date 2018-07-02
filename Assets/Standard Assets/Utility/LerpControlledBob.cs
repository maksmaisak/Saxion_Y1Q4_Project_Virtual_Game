using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    [Serializable]
    public class LerpControlledBob
    {
        public float bobDuration = 0.15f;
        public float bobAmount = 0.1f;

        public float offset { get; private set; }

        public IEnumerator DoBobCycle()
        {
            // make the camera move down slightly
            float t = 0f;
            while (t < bobDuration)
            {
                offset = Mathf.Lerp(0f, bobAmount, t / bobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            // make it move back to neutral
            t = 0f;
            while (t < bobDuration)
            {
                offset = Mathf.Lerp(bobAmount, 0f, t / bobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            offset = 0f;
        }
    }
}
