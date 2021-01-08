using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 

/*
public struct ParticleState {
	public float mass; 
	public Vector3 currPos; 
	public Vector3 nextPos; 
	public Vector3 prevPos; 
	public Vector3 currVel; 
	public Vector3 nextVel; 
	public Vector3 prevVel; 
	public Vector3 midpVel; 
	public Vector3 pMidVel; 
	public Vector3 currAcc; 
}
*/ 

public class CollD: MonoBehaviour
{
	public float G = 9.81f; 
	public float nu = 0.00001516f; // dyn viscosity of air
	public float drag = 1.0f; 

	private string systemName = "colld"; 
	public int N = 2; 
	private float DT = 0.01f; 

	public GameObject[] Body = new GameObject[2]; 
	public float[] Mass = new float[2]; 
	public Vector3[] initialPositions = new Vector3[2]; 
	public Vector3[] initialVelocities = new Vector3[2]; 
	public ParticleState[] particle = new ParticleState[2]; 

	string getWd()
	{
		return Application.dataPath + "/Data/"; 
	}

	void InitializeData()
	{
		string dirPath = getWd(); 
		string stateDataPath = getWd() + "simdata.csv"; 
		string metaDataPath = getWd() + "metadata.txt"; 
		string timeDataPath = getWd() + "timedata.csv"; 
		StreamWriter stateData = new StreamWriter(stateDataPath); 
		StreamWriter metaData = new StreamWriter(metaDataPath); 
		StreamWriter timeData = new StreamWriter(timeDataPath); 

		metaData.WriteLine(
			systemName + "\n" + 
			DT + "\n" + 
			N); 

		for (int i = 0; i < N; i++)
		{
			metaData.WriteLine(particle[i].mass + "\n"); 
		}

		stateData.Flush(); 
		stateData.Close(); 
		metaData.Flush(); 
		metaData.Close(); 
		timeData.Flush(); 
		timeData.Close(); 
	}

	void WriteData(ParticleState[] particle, int t)
	{
		string stateDataPath = getWd() + "simdata.csv";  
		StreamWriter stateData = new StreamWriter(
				stateDataPath, append: true); 

		for (int i = 0; i < N; i++)
		{
			stateData.WriteLine(
				particle[i].currPos.x + "," +
				particle[i].currPos.y + "," +
				particle[i].currPos.z + "\n" +
				particle[i].currVel.x + "," +
				particle[i].currVel.y + "," +
				particle[i].currVel.z); 
		}

		stateData.Flush(); 
		stateData.Close(); 

		string timeDataPath = getWd() + "timedata.csv"; 
		StreamWriter timeData = new StreamWriter(
				timeDataPath, append: true); 

		float time = t * DT; 
		timeData.WriteLine(time); 

		timeData.Flush(); 
		timeData.Close(); 

	}

	void Start()
	{
		for (int i = 0; i < N; i++)
		{
			particle[i].mass = Mass[i]; 
			particle[i].currPos = initialPositions[i]; 
			particle[i].currVel = initialVelocities[i]; 
		}

		for (int i = 0; i < N; i++)
		{
			Body[i].transform.position = particle[i].currPos; 
		}

		InitializeData(); 
	}

	int t = 0; 
	public int vt = 0; 

	void FixedUpdate()
	{
		// render particles
		for (int i = 0; i < N; i++)
		{
			Body[i].transform.position = particle[i].currPos; 
		}

		// write current state
		WriteData(particle, t); 

//		// apply field acceleration
//		for (int i = 0; i < N; i++)
//		{
//			particle[i].currAcc.y = -G; 
//		}

		// verlet integration
		if (vt == 0)
		{
			for (int i = 0; i < N; i++)
			{
				particle[i].midpVel = particle[i].currVel +
					0.5f * particle[i].currAcc * DT;
			}	

			for (int i = 0; i < N; i++)
			{
				particle[i].nextPos = particle[i].currPos +
					0.5f * particle[i].midpVel * DT;
			}
		}

		else
		{
			for (int i = 0; i < N; i++)
			{
				particle[i].midpVel = particle[i].pMidVel +
					particle[i].currAcc * DT; 
			}

			for (int i = 0; i < N; i++)
			{
				particle[i].nextPos = particle[i].currPos +
					particle[i].midpVel * DT; 
			}
		}

		// compute "actual" next velocity
		for (int i = 0; i < N; i++)
		{
			particle[i].nextVel = particle[i].midpVel + 
				0.05f * particle[i].currAcc * DT; 
		}

//		// velocity damping
//		for (int i = 0; i < N; i++)
//		{
//			particle[i].currVel *= drag; 
//			particle[i].midpVel *= drag; 
//		}

		// update arrays for next frame
		for (int i = 0; i < N; i++)
		{
			particle[i].prevPos = particle[i].currPos; 
			particle[i].currPos = particle[i].nextPos; 
			particle[i].prevVel = particle[i].currVel; 
			particle[i].currVel = particle[i].nextVel; 
			particle[i].pMidVel = particle[i].midpVel; 
		}

		t++; 
		vt++; 
	}
}
