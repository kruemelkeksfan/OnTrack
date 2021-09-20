using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
	[Tooltip("Determines Power of the Engine for driving forward/backward.")]
	[SerializeField] private float accelerationForce = 1.0f;
	[Tooltip("Determines Power of the Brakes for Deceleration.")]
	[SerializeField] private float brakeForce = 1.0f;
	[Tooltip("Determines Turn Speed.")]
	[SerializeField] private float steerTorque = 1.0f;
	[Tooltip("Determines how direct the Vehicle drives. Value between 0 and 1.0. Low Values lead to more drifting, high Values to directer Handling.")]
	[SerializeField] private float driftFactor = 0.2f;
	private new Transform transform = null;
	private new Rigidbody2D rigidbody = null;
	private Vector2 movement = Vector2.zero;

	private void Start()
	{
		driftFactor = Mathf.Clamp(driftFactor, 0.0f, 1.0f);

		transform = gameObject.GetComponent<Transform>();       // Buffer transform, because Component.transform calls GetComponent() under the hood
		rigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		// Only process one input per frame, since the state flowchart does not allow for multiple inputs at once
		// Also intending to brag a bit with how many methods I know to compare floats >.< Hope they are correctly used
		if(movement != Vector2.zero)
		{
			// ACCELERATE
			// Dot() to determine if the car is accelerating with (accelerating) or against its current velocity (braking)
			if(!Mathf.Approximately(movement.y, 0.0f) && Vector3.Dot(rigidbody.velocity, movement) > float.Epsilon)
			{
				rigidbody.AddForce(transform.up * accelerationForce * movement.y, ForceMode2D.Impulse);
			}
			// TURN
			// TODO: Could also calculate if the wheels have enough traction/are moving into the right direction for steering
			else if(!Mathf.Approximately(movement.x, 0.0f) && rigidbody.velocity != Vector2.zero)					// Unity Vectors use Approximately() implicitly
			{
				rigidbody.AddTorque(rigidbody.velocity.magnitude * -steerTorque * movement.x, ForceMode2D.Impulse);
			}
			// BRAKE
			else
			{
				rigidbody.AddForce(transform.up * brakeForce * movement.y, ForceMode2D.Impulse);
			}
		}

		// Stop Drift
		Vector2 targetVelocity = transform.up * Vector2.Dot(rigidbody.velocity, (Vector2)transform.up);				// Project current velocity on the vehicles forward direction
		rigidbody.velocity += (targetVelocity - rigidbody.velocity) * driftFactor;
	}

	public void Move(InputAction.CallbackContext context)
	{
		movement = context.ReadValue<Vector2>();
	}
}
