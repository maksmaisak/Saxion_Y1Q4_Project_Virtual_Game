using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

public class DynamicMusicPlayer : MonoBehaviour
{

    [SerializeField] float volumeMaxDelta = float.PositiveInfinity;
    [SerializeField] float combatTimeBound;

    private float targetVolumeNormal;
    private float targetVolumeCombat;

    [SerializeField] private AudioSource normalAudioSource;
    [SerializeField] private AudioSource combatAudioSource;

    void Start()
    {

        Assert.IsNotNull(normalAudioSource);
        Assert.IsNotNull(combatAudioSource);

        normalAudioSource.Play();
        normalAudioSource.loop = true;

        combatAudioSource.Play();
        combatAudioSource.loop = true;
    }

    void Update()
    {
        IntensityChecker checker = IntensityChecker.Instance;


        if(checker.timeSinceLastCombat < combatTimeBound)
        {
            targetVolumeCombat = 0.5f;
            targetVolumeNormal = 0;
        }
        else
        {   
            targetVolumeCombat = 0;
            targetVolumeNormal = 0.22f;
        }

        AdjustChannelVolumes();
    }

    private void AdjustChannelVolumes()
    {
        normalAudioSource.volume = Mathf.MoveTowards(
            normalAudioSource.volume,
            targetVolumeNormal,
            volumeMaxDelta * Time.deltaTime
        );

        combatAudioSource.volume = Mathf.MoveTowards(
            combatAudioSource.volume,
            targetVolumeCombat,
            volumeMaxDelta * Time.deltaTime
        );
    }
}
