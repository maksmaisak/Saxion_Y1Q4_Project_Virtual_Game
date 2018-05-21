using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

//TODO maybe have an enum for different group types. Have one "change" method to take in a member of that enum. Find particle systems at startup by looking at their assigned type as a member of that enum?

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject wanderParticleGroup;
    [SerializeField] private GameObject toPlayerParticleGroup;
    [SerializeField] private GameObject shootingParticleGroup;
    [SerializeField] private GameObject grappledParticleGroup;
    [SerializeField] private GameObject fallingToDeathParticleGroup;

    private List<GameObject> particleGroupsList;

    void Awake()
    {
        particleGroupsList = new List<GameObject>()
        {wanderParticleGroup,toPlayerParticleGroup,shootingParticleGroup,grappledParticleGroup,fallingToDeathParticleGroup};
    }

    public void SwitchWandering() {ChangeParticleGroup(wanderParticleGroup);}
    public void SwitchActive()    {ChangeParticleGroup(toPlayerParticleGroup);}
    public void SwitchGrappled()  {ChangeParticleGroup(grappledParticleGroup);}
    public void SwitchToFalling() {ChangeParticleGroup(fallingToDeathParticleGroup);}

    public void SetShootingEffectsActive(bool isActive = true) 
    {
        if (isActive)
        {
            Play(shootingParticleGroup);
        }
        else 
        {
            Stop(shootingParticleGroup);
        }
    }

    public void DisableAllParticleGroups()
    {
        particleGroupsList.ForEach(Stop);
    }

    public void DetachParticleSystemsFromParent()
    {
        foreach (ParticleSystem system in GetComponentsInChildren<ParticleSystem>())
        {
            system.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
        }

        if (gameObject.GetComponent<AutoDestroyParticleSystem>() == null)
        {
            gameObject.AddComponent<AutoDestroyParticleSystem>();
        }

        transform.SetParent(null);
    }

    private void ChangeParticleGroup(GameObject newParticleGroup, bool disableOthers = true)
    {
        if (disableOthers)
        {
            foreach (GameObject particleGroup in particleGroupsList)
            {
                if (particleGroup == newParticleGroup) continue;
                Stop(particleGroup);
            }
        }

        Play(newParticleGroup);
    }

    private void Play(GameObject particleGroup)
    {
        particleGroup.SetActive(true);
        foreach (var ps in particleGroup.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
    }

    private void Stop(GameObject particleGroup)
    {
        foreach (var ps in particleGroup.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }
    }
}
