using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class FaffCentripetalCircularMotion : MonoBehaviour
{
	float DT = 0.01f; 

	public GameObject Spinner; 
	public GameObject Ball; 
	public GameObject Pointer; 
	public GameObject Spring; 
	public LineRenderer SupportString; 

	public float Mass; 
	public float AngularVelocity; 
	public float SpringConstant; 
	public float PointerRadius; 

	public Slider velSlider; 
	public Text velSliderText; 
	public Text angVelValue; 

	ParticleManifold ball; 
	ParticleManifold spring; 
	RotationalManifold spinner; 

	public Vector3 ToDegrees(Vector3 v)
	{
		return v * 180 / Mathf.PI; 
	}

	void Start()
	{
		ball.mass = Mass; 
		ball.position.curr = Ball.transform.position; 
		spinner.angularVelocity.curr.y = AngularVelocity; 
		spinner.angularVelocity.prev.y = AngularVelocity; 
		spring.position.curr = Spring.transform.position; 

		velSlider.minValue = 0.0f; 
		velSlider.maxValue = 9.0f; 
	}

	int t = 0; 
	float springBaseLength = 0.2f; 

	void FixedUpdate()
	{
		// spinner.angularVelocity.curr.y = AngularVelocity; 
		spinner.angularVelocity.curr.y = velSlider.value; 
		Vector3 RotationIncrement = spinner.angularVelocity.curr * DT; 

		/* compute forces */ 
		
		float springExtension = 
			ball.position.curr.z - springBaseLength; 
		float springForce = -SpringConstant * springExtension; 
		float centrifugalForce = 
			ball.mass *
			ball.position.curr.z *
			Mathf.Pow(spinner.angularVelocity.curr.y, 2); 
		float totalForce = springForce + centrifugalForce; 

		ball.acceleration.curr.z = totalForce / ball.mass; 

		/* verlet integration */ 

		if (t == 0)
		{
			ball.velocity.next = 
				ball.velocity.curr + 0.5f *
				ball.acceleration.curr * DT; 

			ball.position.next = 
				ball.position.curr + 0.5f *
				ball.velocity.next * DT; 
		}

		else 
		{
			ball.velocity.next = 
				ball.velocity.prev + 
				ball.acceleration.curr * DT; 

			ball.position.next = 
				ball.position.curr +
				ball.velocity.next * DT; 
		}

		// velocity damping
		ball.velocity.next *= 0.98f; 

		/* collision resolution */ 

		/* update */ 

		ball.position.prev = ball.position.curr; 
		ball.position.curr = ball.position.next; 
		ball.velocity.prev = ball.velocity.curr; 
		ball.velocity.curr = ball.velocity.next; 
		spinner.angularVelocity.prev = spinner.angularVelocity.curr; 

		Spinner.transform.Rotate(
			ToDegrees(RotationIncrement), Space.World); 

		Ball.transform.Translate(
			ball.position.curr - ball.position.prev, 
			Space.Self); 

		Spring.transform.Translate(
			-(ball.position.curr - ball.position.prev),
			Space.Self); 

		/* spring extension multiplier */ 

		float springExtensionMultiplier = 
			(springBaseLength + springExtension) / 
			springBaseLength; 

		Vector3 springScale = Spring.transform.localScale; 
		springScale.z = 0.035f * springExtensionMultiplier; 
		Spring.transform.localScale = springScale; 

		/* support string update */ 

		SupportString.SetPosition(1, ball.position.curr); 

		/* ui */ 

		float currAngVel = spinner.angularVelocity.curr.y; 
		string velText = currAngVel.ToString(); 
		velSliderText.text = "Angular Velocity = "; 
		angVelValue.text = velText; 


		t++; 
	}
}
