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
	public State angularVelocity; 
	public State angularAcceleration; 
}
