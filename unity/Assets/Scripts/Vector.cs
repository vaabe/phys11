using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector : MonoBehaviour
{
	public Vector3 ToDegrees(Vector3 v)
	{
		return v * 180 / Mathf.PI; 
	}
}
