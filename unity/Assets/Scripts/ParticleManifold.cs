using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public struct StateVector {
	public Vector3 curr; 
	public Vector3 prev; 
	public Vector3 next; 
}

public struct ParticleManifold {
	public float mass; 
	public StateVector position; 
	public StateVector velocity; 
	public StateVector midVelocity; 
	public StateVector acceleration; 
}

/*
public struct RotationalManifold {
	public float inertia; 
	public StateVector angularVelocity; 
	public StateVector angularAcceleration; 
}
*/ 
