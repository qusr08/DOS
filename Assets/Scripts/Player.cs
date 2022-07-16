using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	[SerializeField] [Min(0.001f)] private float movementSpeed;

	private Coroutine move = null;
	private Vector3 movement = Vector3.zero;
	private Vector3[ ] faces;

	public bool IsMoving {
		get {
			return (move != null);
		}
	}

	private void Awake ( ) {
		faces = new Vector3[6];
	}

	private void Start ( ) {
		UpdateDie( );
	}

	private void Update ( ) {
		// If the player is not already moving, check to see if they are trying to move
		if (!IsMoving) {
			// If the input is on a diagonal or not straight on, then ignore it
			if (movement.magnitude <= 1 && movement.magnitude > 0) {
				// Check to make sure the direction the player is trying to move in is valid
				if (IsValidMapTile(movement, out MapTile mapTile)) {
					move = StartCoroutine(IMove(movement, mapTile));
				}
			}
		}

		// If the two face is facing upwards the player wins the level
		if (faces[1] == Vector3.up) {
			// TODO: trigger a win in the game
		}
	}

	private IEnumerator IMove (Vector3 direction, MapTile mapTile) {
		// TODO: Might have to edit the point if corners of die are "sanded" off
		Vector3 point = transform.position + ((direction + Vector3.down) / 2f);
		Vector3 axis = new Vector3(direction.z, 0, (direction.z == 0 ? -1 : 1) * direction.x);
		float degreesToMove = 90;

		// Slowly rotate the die around a point to have it move
		while (degreesToMove > 0) {
			float time = movementSpeed * Time.deltaTime;
			transform.RotateAround(point, axis, Mathf.Min(time, degreesToMove));
			degreesToMove -= time;

			yield return new WaitForEndOfFrame( );
		}

		// The tile that the die has just moved onto might have a special movement/rotation
		mapTile.EffectDie(transform);
		// Wait for the tile to finish moving the die
		while (mapTile.IsMovingDie) {
			yield return new WaitForEndOfFrame( );
		}

		// Round values so there are no decimal places and update all face directions
		UpdateDie( );

		move = null;
	}

	private void UpdateDie ( ) {
		// Snap transform position and rotation so there are no decimals
		transform.position = Vector3Int.RoundToInt(transform.position);
		transform.eulerAngles = Vector3Int.RoundToInt(transform.eulerAngles);

		// Update all faces based on the transform rotation
		faces[0] = transform.up;
		faces[1] = transform.forward;
		faces[2] = transform.right * -1;
		faces[3] = transform.right;
		faces[4] = transform.forward * -1;
		faces[5] = transform.up * -1;
	}

	private int GetNextFace (Vector3 direction, bool roll = true) {
		Vector3 point = transform.position + ((direction + Vector3.down) / 2f);
		Vector3 axis = new Vector3(direction.z, 0, (direction.z == 0 ? -1 : 1) * direction.x);

		// Temporarily rotate the die in the specified direction to check the next face values
		if (roll) {
			transform.RotateAround(point, axis, 90);
			UpdateDie( );
		}

		// See which face index is facing down
		int face = -1;
		for (int i = 0; i < faces.Length; i++) {
			if (faces[i] == Vector3.down) {
				face = (i + 1);
			}
		}

		// Undo the temporary roll so the cube doesnt move
		if (roll) {
			transform.RotateAround(point, axis, -90);
			UpdateDie( );
		}

		return face;
	}

	private bool IsValidMapTile (Vector3 direction, out MapTile mapTile) {
		mapTile = null;

		// Check to see if there is a world tile at the position dictated by the direction
		if (Physics.Raycast(transform.position + direction, Vector3.down, out RaycastHit raycastHit)) {
			mapTile = raycastHit.transform.GetComponent<MapTile>( );

			// If there is no map tile the die cannot move in the direction specified
			if (mapTile == null) {
				return false;
			}

			// TODO: If the map tile that the die is trying to move to has a specific face, make sure the face that will go onto that tile is the same
			if (mapTile.ExclusiveFace == 0 || GetNextFace(direction) == mapTile.ExclusiveFace) {
				return true;
			}
		}

		return false;
	}

	private void OnMove (InputValue inputValue) {
		Vector2 input = inputValue.Get<Vector2>( );

		movement = new Vector3(input.x, 0, input.y);
	}
}
