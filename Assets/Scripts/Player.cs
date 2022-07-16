using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	[SerializeField] [Min(0.001f)] private float movementSpeed;

	private Coroutine move = null;

	public bool IsMoving {
		get {
			return (move != null);
		}
	}

	private void Start ( ) {

	}

	private IEnumerator IMove (Vector2 direction) {
		Vector3 point = transform.position + ((new Vector3(direction.x, 0, direction.y) + Vector3.down) / 2f);
		Vector3 axis = new Vector3(direction.y, 0, (direction.y == 0 ? -1 : 1) * direction.x);
		float degreesToMove = 90;

		while (degreesToMove > 0) {
			transform.RotateAround(point, axis, movementSpeed);
			degreesToMove -= movementSpeed;

			yield return new WaitForEndOfFrame( );
		}

		//transform.RotateAround((Vector3.forward + Vector3.down) / 2f, Vector3.right, 90); // up
		//transform.RotateAround((Vector3.right + Vector3.down) / 2f, Vector3.forward, -90); // right
		//transform.RotateAround((Vector3.back + Vector3.down) / 2f, Vector3.left, 90); // down
		//transform.RotateAround((Vector3.left + Vector3.down) / 2f, Vector3.back, -90); // left

		// Rotate the transform around a point
		//transform.RotateAround(transform.position + ((point + Vector3.down) / 2f), axis, 90);

		move = null;
	}

	private void OnMove (InputValue inputValue) {
		// Get the current input values
		Vector2 movement = inputValue.Get<Vector2>( );

		// If the input is on a diagonal or not straight on, then ignore it
		if (movement.magnitude > 1) {
			return;
		}

		// If the player is not already moving, move in the direction on the input
		if (!IsMoving) {
			move = StartCoroutine(IMove(movement));
		}
	}
}
