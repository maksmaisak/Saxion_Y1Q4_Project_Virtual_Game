using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

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
        shootingParticleGroup.SetActive(isActive);
    }

    public void DisableAllParticleGroups()
    {
        foreach (GameObject particleGroup in particleGroupsList)
        {
            particleGroup.SetActive(false);
        }
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
                particleGroup.SetActive(false);
            }
        }

        newParticleGroup.SetActive(true);
    }
}
