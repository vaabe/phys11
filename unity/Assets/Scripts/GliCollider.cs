using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliCollider: MonoBehaviour
{
	void OnTriggerEnter(Collider coll)
	{
		Debug.Log("fuk! collision");

		ParticleState[] particle = new ParticleState[1]; 

		particle = GameObject.Find("DynManifold").
			GetComponent<GliColls>().
			particle; 

		int N = GameObject.Find("DynManifold").
			GetComponent<GliColls>().N; 	

		// Debug.Log(particle[1].currVel.z); 

		for (int i = 0; i < N; i++)
		{
			// particle[i].currPos.y = 0; 
			particle[i].currVel.z *= -1; 
		}

		// Debug.Log(particle[1].currVel.z); 

		GameObject.Find("DynManifold").
			GetComponent<GliColls>().
			particle = particle; 

		GameObject.Find("DynManifold").
			GetComponent<GliColls>().
			vt = 0; 
	}
}
