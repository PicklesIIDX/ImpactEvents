using UnityEngine;
using System.Collections;

public class MoveOnController : MonoBehaviour {

	[SerializeField]
	protected float xSpeed = 0.0f;
	public float XSpeed {
		get { return xSpeed; }
	}
	[SerializeField]
	protected float ySpeed = 0.0f;
	public float YSpeed {
		get { return ySpeed; }
	}

	public virtual void Initialize(float newXSpeed, float newYSpeed) {
		xSpeed = newXSpeed;
		ySpeed = newYSpeed;
	}

	void Update() {
		transform.position = GetNextPosition(Time.deltaTime, transform.position);
	}

	public Vector3 GetNextPosition(float deltaTime, Vector3 currentPosition) {
		currentPosition.x += xSpeed * deltaTime;
		currentPosition.y += ySpeed * deltaTime;
		return currentPosition;
	}
}
