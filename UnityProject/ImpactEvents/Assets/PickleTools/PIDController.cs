using UnityEngine;
using System.Collections;

public class PIDController  {

	[SerializeField]
	float p;
	/// <summary>
	/// Increase proportional to make it move faster!
	/// </summary>
	/// <value>The p.</value>
	public float P {
		get { return p; }
		set { p = value; }
	}
	[SerializeField]
	float i;
	/// <summary>
	/// Increase integral to make more relentless!
	/// </summary>
	/// <value>The i.</value>
	public float I {
		get { return i; }
		set { i = value; }
	}
	[SerializeField]
	float d;
	/// <summary>
	/// Increase derivative to make smoother.
	/// </summary>
	/// <value>The d.</value>
	public float D {
		get { return d; }
		set { d = value; }
	}
	float previousError;
	float integralError;
	bool fresh;
	[SerializeField]
	bool angular = false;
	public bool Angular {
		get { return angular; }
		set { angular = value; }
	}

	public PIDController(float proportion = -10.0f, float integral = -2.0f, float difference = -1.0f){
		p = proportion;
		i = integral;
		d = difference;
	}

	public void Reset(){
		previousError = integralError = 0;
		fresh = true;
	}

	public float UpdateAt(float deltaTime, float current, float target){
		if(deltaTime <= 0){
			return 0;
		}
		float error = current - target;
		if(angular){
			error = error < -Mathf.PI ? error + 2 * Mathf.PI : error;
			error = error > Mathf.PI ? error - 2 * Mathf.PI : error;
		}
		integralError = (fresh) ? error : integralError;
		previousError = (fresh) ? error : previousError;
		integralError = ( 1.0f - deltaTime ) * integralError + deltaTime * error;
		float derivativeError = (error - previousError) / deltaTime;
		previousError = error;
		fresh = false;
		//Debug.LogWarning(string.Format("{0} * {1} + {2} * {3} + {4} * {5}",
		                               //p, error, i, integralError, d, derivativeError));
		return p * error + i * integralError + d * derivativeError; 
	}
}
