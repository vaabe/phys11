using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HumanArmGravity : MonoBehaviour
{
	public GameObject ArmManifold; 
	public GameObject Weight; 

	public float BallMass; 
	public float AppliedForce; 
	public LineRenderer WeightSupportWire; 
	public float SpringConstant = 1000.0f; 

	private float ArmRadius = 0.5f; 
	private float G = 9.81f; 

	private float timeStep = 0.1f; 

	RotationalManifold1 arm; 

	public Slider appliedForceSlider; 
	public Slider ballMassSlider; 

	public Text AppliedForceText; 
	public Text BallMassText; 
	public Text ArmAngleText; 
	public Text AppliedTorqueText; 
	public Text GravityTorqueText; 

	public Vector3 VecToDegrees(Vector3 v)
	{
		return v * (180 / Mathf.PI); 
	}

	public Vector3 VecToRad(Vector3 v)
	{
		return v * (Mathf.PI / 180); 
	}

	public float FloatToRad(float f)
	{
		return f * (Mathf.PI / 180); 
	}

	Vector3 weightInitialPosition; 

	void Start()
	{
		arm.angle.curr = 
			ArmManifold.transform.rotation.eulerAngles.z; 
		// Debug.Log(arm.angle.curr); 

		arm.inertia = BallMass * Mathf.Pow(ArmRadius, 2); 

		// Debug.Log(ArmManifold.transform.rotation.eulerAngles.z); 

		weightInitialPosition = Weight.transform.position; 

	}

	int time = 0; 

	void FixedUpdate()
	{
		/* get user modded values */ 

		AppliedForce = appliedForceSlider.value; 
		BallMass = ballMassSlider.value; 

		/* compute torques */ 

		float appliedTorque = ArmRadius * AppliedForce; 
		float gravityForce = BallMass * G; 
		float gravityTorque = ArmRadius * gravityForce * 
			Mathf.Sin(FloatToRad(arm.angle.curr)); 
		gravityTorque *= -1; 

		float totalTorque = appliedTorque + gravityTorque; 

		arm.angularAcceleration.curr = totalTorque / arm.inertia; 

		// Debug.Log(arm.angularAcceleration.curr); 

		/* verlet integration */ 

		if (time == 0)
		{
			arm.angularVelocity.next = 
				arm.angularVelocity.curr + 0.5f *
				arm.angularAcceleration.curr * timeStep; 

			arm.angle.next = 
				arm.angle.curr + 0.5f *
				arm.angularVelocity.next * timeStep; 
		}

		else
		{
			arm.angularVelocity.next = 
				arm.angularVelocity.curr + 
				arm.angularAcceleration.curr * timeStep; 

			arm.angle.next = 
				arm.angle.curr + 
				arm.angularVelocity.next * timeStep; 
		}

		/* velocity damping */ 

		arm.angularVelocity.next *= 0.99f; 

		/* collisions */ 

		if (arm.angle.next > 150.0f)
		{
			arm.angle.next = 150.0f; 
			arm.angularVelocity.next *= -0.7f; 
		}

		/* update */ 
		arm.angle.prev = arm.angle.curr; 
		arm.angle.curr = arm.angle.next; 
		arm.angularVelocity.prev = arm.angularVelocity.curr; 
		arm.angularVelocity.curr = arm.angularVelocity.next; 

//		Quaternion target = Quaternion.Euler(0, 0, 
//				arm.angle.curr - arm.angle.prev); 

		float rotationIncrementZ = arm.angle.curr - arm.angle.prev; 
		Vector3 rotationIncrement = new Vector3(
				0.0f, 0.0f, rotationIncrementZ); 

		ArmManifold.transform.Rotate(rotationIncrement); 

//		ArmManifold.transform.rotation = 
//			Quaternion.Slerp(
//				ArmManifold.transform.rotation, 
//				target, 
//				time * 

//		ArmManifold.transform.eulerAngles = 
//			new Vector3(0.0f, 0.0f, arm.angle.curr); 
//			// arm.angle.curr; 


		float springExtension = AppliedForce / SpringConstant * 0.1f;  
		Vector3 weightNewPosition = weightInitialPosition; 
		weightNewPosition.y -= springExtension; 
		Weight.transform.position = weightNewPosition; 

		WeightSupportWire.SetPosition(1, Weight.transform.position); 

		/* UI text */ 

		AppliedForceText.text = 
			"Applied Force = " +
			AppliedForce + 
			" N"; 
		BallMassText.text = 
			"Ball Mass = " +
			BallMass +
			" kg"; 
		ArmAngleText.text = 
			"Arm Angle = \n" +
			arm.angle.prev +
			" degrees"; 
		AppliedTorqueText.text = 
			"Applied Torque = \n" +
			appliedTorque +
			" Nm"; 
		GravityTorqueText.text = 
			"Gravity Torque = \n" +
			gravityTorque +
			" Nm"; 

		time++; 
	}
}
