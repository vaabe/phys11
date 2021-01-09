using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct State {
	public Vector3 prev; 
	public Vector3 curr; 
	public Vector3 next; 
}

public struct LinearManifold {
	public float mass; 
	public State position; 
	public State velocity; 
	public State acceleration;
}

public struct RotationalManifold {
	public float inertia; 
	public State angle; 
	public State angularVelocity; 
	public State angularAcceleration; 
}

public struct State1 {
	public float prev; 
	public float curr; 
	public float next; 
}

public struct RotationalManifold1 {
	public float inertia; 
	public State1 angle; 
	public State1 angularVelocity; 
	public State1 angularAcceleration; 
}
