using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
	[SerializeField] private float accelerationForce = 1.0f;
	[SerializeField] private float brakeForce = 1.0f;
	[SerializeField] private float steerTorque = 1.0f;
	private new Transform transform = null;
	private new Rigidbody rigidbody = null;
	private Vector2 movement = Vector2.zero;

	private void Start()
	{
		transform = gameObject.GetComponent<Transform>();	// Buffer transform, because Component.transform calls GetComponent() under the hood
		rigidbody = gameObject.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		// Only process one input per frame, since the state flowchart does not allow for multiple inputs at once
		// Also intending to brag a bit with how many methods I know to compare floats >.< Hope they are correctly used
		// ACCELERATE
		// Dot() to determine if the car is breaking or accelerating backwards
		// The double check of (movement.y < -float.Epsilon) is not very beautiful, but I have to either double evaluate a condition or write the if-body twice
		if(movement.y > float.Epsilon || (movement.y < -float.Epsilon && Vector3.Dot(rigidbody.velocity, new Vector3(movement.x, 0.0f, movement.y)) > float.Epsilon))
		{
			rigidbody.AddForce(transform.forward * accelerationForce * movement.y * Time.deltaTime, ForceMode.Impulse);
		}
		// BRAKE
		else if(movement.y < -float.Epsilon)
		{
			rigidbody.AddForce(transform.forward * brakeForce * movement.y * Time.deltaTime, ForceMode.Impulse);
		}
		// TURN
		else if(!Mathf.Approximately(movement.x, 0.0f) && rigidbody.velocity != Vector3.zero)								// Unity Vectors use Approximately() implicitly
		{
			rigidbody.AddTorque(transform.up * steerTorque * movement.x * Time.deltaTime, ForceMode.Impulse);
		}
	}

	public void Move(InputAction.CallbackContext context)
	{
		movement = context.ReadValue<Vector2>();
	}
}
