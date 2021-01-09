using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CentripetalCircularMotion : MonoBehaviour
{
	Vector Vec; 

	public GameObject Spinner; 
	public GameObject Ball; 
	public GameObject Pointer; 
	// public GameObject Spring; 
	public LineRenderer SupportWire; 

	public float AngularVelocity; 
	public float Mass; 
	public float PointerRadius; 
	public float SpringConstant; 
	public float SpringBaseLength; 

	private float timeStep = 0.01f;  

	LinearManifold ball; 
	LinearManifold spring; 
	RotationalManifold spinner; 

	public Slider angularVelocitySlider; 
	public Slider massSlider; 
	public Slider pointerRadiusSlider; 
	public Slider springConstantSlider; 

	public Text angularVelocityText; 
	public Text massText; 
	public Text pointerRadiusText; 
	public Text springConstantText; 
	public Text ballRadiusText; 

	public Vector3 ToDegrees(Vector3 v)
	{
		return v * 180 / Mathf.PI; 
	}
	
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
		Vec = FindObjectOfType<Vector>(); 

		ball.mass = Mass; 
		ball.position.curr = Ball.transform.position; 

		spinner.angularVelocity.curr.y = AngularVelocity; 
		spinner.angularVelocity.prev.y = AngularVelocity; 

		// spring.position.curr = Spring.transform.position; 

		GetHelSpringPoints(ball.position.curr); 
		HelSpring.SetPositions(helSpringPoints); 

	}

	int time = 0; 

	void FixedUpdate()
	{
		/* get user modded values */ 

		// get angular velocity 
		spinner.angularVelocity.curr.y = angularVelocitySlider.value; 

		// get mass of ball
		Mass = massSlider.value; 
		ball.mass = Mass; 

		// get pointer radius
		Vector3 PointerNewPosition = Pointer.transform.position; 
		PointerNewPosition.z = pointerRadiusSlider.value; 
		Vector3 PointerPositionChange = 
			PointerNewPosition - Pointer.transform.position; 
		Pointer.transform.Translate(PointerPositionChange); 

		// get spring constant
		SpringConstant = springConstantSlider.value; 

		/* compute forces */ 

		float springExtension = 
			ball.position.curr.z - SpringBaseLength; 
		float springForce = -SpringConstant * springExtension; 
		float centrifugalForce = 
			ball.mass *
			ball.position.curr.z *
			Mathf.Pow(spinner.angularVelocity.curr.y, 2); 
		float totalForce = springForce + centrifugalForce; 

		ball.acceleration.curr.z = totalForce / ball.mass; 

		/* verlet integration */ 

		if (time == 0)
		{
			ball.velocity.next = 
				ball.velocity.curr + 0.5f *
				ball.acceleration.curr * timeStep; 

			ball.position.next = 
				ball.position.curr + 0.5f *
				ball.velocity.next * timeStep; 
		}

		else 
		{
			ball.velocity.next = 
				ball.velocity.prev +
				ball.acceleration.curr * timeStep; 

			ball.position.next = 
				ball.position.curr +
				ball.velocity.next * timeStep; 
		}

		/* velocity damping */ 

		ball.velocity.next *= 0.90f; 

		/* collision resolution */ 

		if (ball.position.next.z < 0.2f)
		{
			ball.position.next.z = 0.2f; 
			ball.velocity.next.z *= -0.3f; 
		}

		/* update */ 

		ball.position.prev = ball.position.curr; 
		ball.position.curr = ball.position.next; 
		ball.velocity.prev = ball.velocity.curr; 
		ball.velocity.curr = ball.velocity.next; 
		spinner.angularVelocity.prev = spinner.angularVelocity.curr; 
		
		// update spinner
		Vector3 SpinnerRotationIncrement = 
			spinner.angularVelocity.curr * timeStep; 
		Spinner.transform.Rotate(
			ToDegrees(SpinnerRotationIncrement), 
			Space.World); 

		// update ball
		Ball.transform.Translate(
			ball.position.curr - ball.position.prev, 
			Space.Self); 

		// update spring
//		Spring.transform.Translate(
//			-(ball.position.curr - ball.position.prev), 
//			Space.Self); 
//		float springExtensionMultiplier = 
//			(SpringBaseLength + springExtension) / 
//			SpringBaseLength; 
//		Vector3 springScale = Spring.transform.localScale; 
//		springScale.z = 0.035f * springExtensionMultiplier; 
//		Spring.transform.localScale = springScale; 

		GetHelSpringPoints(ball.position.curr); 
		HelSpring.SetPositions(helSpringPoints); 

		// update support wire
		SupportWire.SetPosition(1, ball.position.curr); 

		/* UI text */ 

		angularVelocityText.text = 
			"Angular Velocity = " +
			spinner.angularVelocity.curr.y +
			" rad/s"; 
		massText.text = 
			"Ball Mass = " + 
			ball.mass.ToString() +
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
			ball.position.curr.z +
			" m"; 

		time++; 
	}
}
