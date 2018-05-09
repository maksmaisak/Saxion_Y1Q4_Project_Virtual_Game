using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticleSystem : MonoBehaviour {

	// Use this for initialization
	void Start () {

        float duration = GetComponentsInChildren<ParticleSystem>()
            .Max(ps => ps.main.duration);

        Destroy(gameObject, duration);
	}
}
