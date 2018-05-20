using UnityEngine;
using System.Collections;

#pragma warning disable 0649

/// Attach this to a GameObject to provide an alternative sound for when that object is hit with a grapple hook.
public class CustomGrappleHookHitSound : MonoBehaviour
{
    [SerializeField] AudioClip _audioClip;
    public AudioClip audioClip { get { return _audioClip; } }
}
