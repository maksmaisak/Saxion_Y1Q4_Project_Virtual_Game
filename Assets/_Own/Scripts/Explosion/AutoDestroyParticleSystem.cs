using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticleSystem : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        float duration = Mathf.Max(GetParticlesDuration(), GetAudioDuration());
        Destroy(gameObject, duration);
    }

    private float GetParticlesDuration()
    {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        if (particleSystems.Length == 0) return 0f;
        return particleSystems.Max(ps => GetParticleSystemDuration(ps));
    }

    private float GetAudioDuration()
    {
        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
        if (audioSources.Length == 0) return 0f;
        return audioSources.Max(source => source.clip.length);
    }

    private float GetParticleSystemDuration(ParticleSystem particleSystem)
    {
        float emissionDuration = particleSystem.main.duration;

        // Does not account for other MinMaxCurve modes, but should do for now.
        float particleLifetime = particleSystem.main.startLifetime.constant;

        return emissionDuration + particleLifetime;
    }
}
