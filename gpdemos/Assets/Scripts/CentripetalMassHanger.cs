using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CentripetalMassHanger : MonoBehaviour
{
	public GameObject Ball; 
	public GameObject Pointer; 
	// public GameObject Spring; 
	public GameObject Hanger; 
	public LineRenderer SupportWire; 
	public LineRenderer HangerPulleyWire; 
	public LineRenderer BallPulleyWire; 

	public float HangerMass; 
	public float BallMass; 
	public float PointerRadius; 
	public float SpringConstant; 
	public float SpringBaseLength; 
	public float G = 9.81f; 

	// private float timeStep = 0.01f; 

//	LinearManifold hanger; 
//	LinearManifold ball; 
	LinearManifold spring; 

	public Slider hangerMassSlider; 
	public Slider ballMassSlider; 
	public Slider springConstantSlider; 
	public Slider pointerRadiusSlider; 

	public Text hangerMassText; 
	public Text ballMassText; 
	public Text springConstantText; 
	public Text pointerRadiusText; 
	public Text ballRadiusText; 

	Vector3 ballInitialPosition, hangerInitialPosition; 

	float hangerScale; 

	public LineRenderer HelSpring; 
	const int SPRING_POINTS = 1000; 
	const int SPRING_REVS = 10; 
	public Vector3 helSpringPivot = new Vector3(0.0f, 0.5f, 0.0f); 

	Vector3[] helSpringPoints = new Vector3[SPRING_POINTS]; 

	public void GetHelSpringPoints(Vector3 v2)
	{
		float helSpringLength = v2.z; 
		float spiralLength = helSpringLength / SPRING_REVS; 
		float c = spiralLength / (2 * Mathf.PI); 
		float r = 0.02f; 
		float[] t = new float[SPRING_POINTS]; 

		// Debug.Log(spiralLength); 

		for (int i = 0; i < SPRING_POINTS; i++)
		{
			t[i] = (10 * 2 * Mathf.PI) / SPRING_POINTS * i; 
		}

		for (int i = 0; i < SPRING_POINTS; i++)
		{
			helSpringPoints[i].x = r * Mathf.Cos(t[i]); 
			helSpringPoints[i].y = r * Mathf.Sin(t[i]) + 0.5f; 
			helSpringPoints[i].z = c * t[i]; 
		}
	}

	void Start()
	{
//		ball.mass = BallMass; 
//		ball.position.curr = Ball.transform.position; 
//
//		hanger.mass = HangerMass; 
//		hanger.position.curr = Hanger.transform.position; 

		// spring.position.curr = Spring.transform.position; 
		ballInitialPosition = Ball.transform.position; 
		hangerInitialPosition = Hanger.transform.position; 

		GetHelSpringPoints(Ball.transform.position); 
		HelSpring.SetPositions(helSpringPoints); 

		hangerScale = Hanger.transform.localScale.y; 
		// Debug.Log(hangerScale); 
	}

	// int time = 0; 

	void FixedUpdate()
	{
		/* get user modded values */ 

		HangerMass = hangerMassSlider.value; 
		BallMass = ballMassSlider.value; 
		SpringConstant = springConstantSlider.value; 

		Vector3 PointerNewPosition = Pointer.transform.position; 
		PointerNewPosition.z = pointerRadiusSlider.value; 
		Vector3 PointerPositionChange = 
			PointerNewPosition - Pointer.transform.position; 
		Pointer.transform.Translate(PointerPositionChange); 

		/* compute forces */ 

		float hangerGravityForce = HangerMass * G; 
		float springExtension = hangerGravityForce / SpringConstant; 

//		float springExtension = 
//			ball.position.curr.z - SpringBaseLength; 
//		float springForce = -SpringConstant * springExtension; 
//		float totalForce = springForce + hangerGravityForce; 
//
//		float acceleration = totalForce / (HangerMass + BallMass); 
//
//		ball.acceleration.curr.z = acceleration; 
//		ball.acceleration.curr.y = -acceleration; 

		/* update */ 

		Vector3 ballNewPosition = ballInitialPosition; 
	        Vector3 hangerNewPosition = hangerInitialPosition; 

		ballNewPosition.z += springExtension; 
		hangerNewPosition.y -= springExtension; 

		Ball.transform.position = ballNewPosition; 
		Hanger.transform.position = hangerNewPosition; 

		Vector3 distancePulleyBall = new Vector3(0.0f, 0.0f, 0.05f); 
//		Spring.transform.position = 
//			Ball.transform.position - distancePulleyBall; 


		GetHelSpringPoints(Ball.transform.position); 
		HelSpring.SetPositions(helSpringPoints); 

		SupportWire.SetPosition(1, Ball.transform.position); 
		BallPulleyWire.SetPosition(0, Ball.transform.position); 
		HangerPulleyWire.SetPosition(0, Hanger.transform.position); 

		float hangerScaleFactor = HangerMass / 1.0f; 
		Vector3 newScale = Hanger.transform.localScale; 
		newScale.y = hangerScaleFactor * hangerScale; 
		Hanger.transform.localScale = newScale; 


		/* UI text */ 

		hangerMassText.text = 
			"Hanger Mass = " +
			HangerMass +
			" kg"; 
		ballMassText.text = 
			"Ball Mass = " + 
			BallMass +
			" kg"; 
		pointerRadiusText.text = 
			"Pointer Radius = " +
			Pointer.transform.position.z.ToString() +
			" m"; 
		springConstantText.text = 
			"Spring Constant = " +
			SpringConstant.ToString() +
			" N/m"; 

		ballRadiusText.text = 
			"Current Radius of Ball = " +
			Ball.transform.position.z +
			" m"; 


	}

}
