using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    [SerializeField] private GameObject wanderParticleGroup;
    [SerializeField] private GameObject toPlayerParticleGroup;
    [SerializeField] private GameObject shootingParticleGroup;
    [SerializeField] private GameObject grappledParticleGroup;
    [SerializeField] private GameObject fallingToDeathParticleGroup;

    private List<GameObject> particleGroupsList;

    // Use this for initialization
    void Start()
    {

        particleGroupsList = new List<GameObject>()
        {wanderParticleGroup,toPlayerParticleGroup,shootingParticleGroup,grappledParticleGroup,fallingToDeathParticleGroup};
    }

    public void ChangeParticleGroup(GameObject newParticleGroup)
    {
        if (particleGroupsList == null) return;
        DisableAllParticleGroups();
        newParticleGroup.SetActive(true);
    }

    public void DisableAllParticleGroups()
    {
        if (particleGroupsList == null) return;
        foreach (GameObject particleGroup in particleGroupsList)
        {
            particleGroup.SetActive(false);
        }
    }

    public GameObject GetToPlayerParticalGroup()
    {
        return toPlayerParticleGroup;
    }
}
