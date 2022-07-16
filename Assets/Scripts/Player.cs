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

		// Slowly rotate the die around a point to have it move
		while (degreesToMove > 0) {
			transform.RotateAround(point, axis, movementSpeed);
			degreesToMove -= movementSpeed;

			yield return new WaitForEndOfFrame( );
		}

		move = null;
	}

	private bool IsValidWorldTile (Vector2 direction) {
		// Check to see if there is a world tile at the position dictated by the direction
		if (Physics.Raycast(transform.position + new Vector3(direction.x, 0, direction.y), Vector3.down, out RaycastHit raycastHit)) {
			// TODO: check to see if the world tile type allows for the die to go onto it
			
			return true;
		}

		return false;
	}

	private void OnMove (InputValue inputValue) {
		// Get the current input values
		Vector2 movement = inputValue.Get<Vector2>( );

		// If the input is on a diagonal or not straight on, then ignore it
		if (movement.magnitude > 1) {
			return;
		}

		// If the player is not already moving, move in the direction on the input
		if (!IsMoving && IsValidWorldTile(movement)) {
			move = StartCoroutine(IMove(movement));
		}
	}
}
