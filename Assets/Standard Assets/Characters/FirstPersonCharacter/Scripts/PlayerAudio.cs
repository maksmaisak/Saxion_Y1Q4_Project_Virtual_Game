using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [Space]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;
    [Space]
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] [Range(0f, 2f)] float footstepVolumeScale = 1f;

    void Start()
    {
        Assert.IsNotNull(audioSource);
        Assert.IsNotNull(jumpSound);
        Assert.IsNotNull(landSound);

        Assert.IsNotNull(footstepSounds);
        Assert.IsTrue(footstepSounds.Length > 0);
    }

    public void PlayJump()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlayLand()
    {
        audioSource.PlayOneShot(landSound);
    }

    public void PlayFootstep()
    {
        if (audioSource.isPlaying) return;
        audioSource.PlayOneShot(PickFootstepSound(), footstepVolumeScale);
    }

    private AudioClip PickFootstepSound()
    {
        if (footstepSounds.Length == 0) return footstepSounds[0];

        int i = Random.Range(1, footstepSounds.Length);

        // Move the picked sound to index 0 so it's not picked next time
        AudioClip clip = footstepSounds[i];
        footstepSounds[i] = footstepSounds[0];
        footstepSounds[0] = clip;

        return clip;
    }
}