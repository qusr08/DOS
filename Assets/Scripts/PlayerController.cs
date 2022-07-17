using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	[SerializeField] private GameManager gameManager;
	[Space]
	[SerializeField] [Min(0.001f)] public float RollSpeed;

	private Coroutine move = null;
	private Vector3 movement = Vector3.zero;
	private readonly Vector3[ ] faces = new Vector3[6];

	public bool IsMoving {
		get {
			return (move != null);
		}
	}

	private void OnValidate ( ) {
		gameManager = FindObjectOfType<GameManager>( );
	}

	private void Start ( ) {
		OnValidate( );

		UpdateDie( );
	}

	private void Update ( ) {
		// If the player is not already moving, check to see if they are trying to move
		if (!IsMoving) {
			// If the input is on a diagonal or not straight on, then ignore it
			if (movement.magnitude <= 1 && movement.magnitude > 0) {
				// Check to make sure the direction the player is trying to move in is valid
				if (IsValidMapTile(movement, out MapTile fromMapTile, out MapTile toMapTile)) {
					move = StartCoroutine(IMove(movement, fromMapTile, toMapTile));
				}
			}
		}

		// If the two face is facing upwards the player wins the level
		if (faces[1] == Vector3.up && gameManager.State == GameManager.GameState.LEVEL) {
			gameManager.SetGameState(GameManager.GameState.LEVEL_COMPLETE);
		}
	}

	private IEnumerator IMove (Vector3 direction, MapTile fromMapTile, MapTile toMapTile) {
		// TODO: Might have to edit the point if corners of die are "sanded" off
		Vector3 point = transform.position + ((direction + Vector3.down) / 2f);
		Vector3 rotationAxis = new Vector3(direction.z, 0, (direction.z == 0 ? -1 : 1) * direction.x);
		float toRotateDegrees = 90;

		// Slowly rotate the die around a point to have it move
		while (toRotateDegrees > 0) {
			float time = RollSpeed * Time.deltaTime;

			transform.RotateAround(point, rotationAxis, Mathf.Min(time, toRotateDegrees));

			toRotateDegrees -= time;

			yield return new WaitForEndOfFrame( );
		}

		// Round values so there are no decimal places and update all face directions
		UpdateDie( );

		if (fromMapTile.Type == MapTile.MapTileType.CRACKED) {
			fromMapTile.DestroyTile( );
		}

		// The tile that the die has just moved onto might have a special movement/rotation
		toMapTile.EffectDie(this);
		// Wait for the tile to finish moving the die
		while (toMapTile.IsEffectingDie) {
			yield return new WaitForEndOfFrame( );
		}

		move = null;
	}

	public void UpdateDie ( ) {
		// Snap transform position and rotation so there are no decimals
		transform.position = Vector3Int.RoundToInt(transform.position);
		transform.eulerAngles = Vector3Int.RoundToInt(transform.eulerAngles);

		// Update all faces based on the transform rotation
		// The index is 1 less than the face value, so a 0 index is actually the 1 side of the die
		faces[0] = transform.up;
		faces[1] = transform.forward;
		faces[2] = transform.right * -1;
		faces[3] = transform.right;
		faces[4] = transform.forward * -1;
		faces[5] = transform.up * -1;

		// Debug.DrawRay(transform.position, faces[0], Color.red, 1);
		// Debug.DrawRay(transform.position, faces[1], Color.blue, 1);
	}

	private int GetNextBottomFace (Vector3 direction, bool roll) {
		Vector3 point = transform.position + ((direction + Vector3.down) / 2f);
		Vector3 axis = new Vector3(direction.z, 0, (direction.z == 0 ? -1 : 1) * direction.x);

		// Temporarily rotate the die in the specified direction to check the future face values
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

		// Undo the temporary roll
		if (roll) {
			transform.RotateAround(point, axis, -90);
			UpdateDie( );
		}

		return face;
	}

	public bool IsValidMapTile (Vector3 direction, out MapTile fromMaptile, out MapTile toMapTile, bool roll = true) {
		fromMaptile = null;
		if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit fromHit)) {
			fromMaptile = fromHit.transform.GetComponent<MapTile>( );
		}

		// Check to see if there is a world tile at the position dictated by the direction
		toMapTile = null;
		if (Physics.Raycast(transform.position + direction, Vector3.down, out RaycastHit toHit)) {
			toMapTile = toHit.transform.GetComponent<MapTile>( );

			// If there is no map tile the die cannot move in the direction specified
			if (toMapTile == null) {
				return false;
			}

			// If the tile is being destroyed do not count it as an actual tile
			if (toMapTile.IsDestroyed) {
				return false;
			}

			if (toMapTile.ExclusiveFace == 0 || GetNextBottomFace(direction, roll) == toMapTile.ExclusiveFace) {
				return true;
			}
		}

		return false;
	}

	public bool IsValidMapTile (Vector3 direction, bool roll = true) {
		return IsValidMapTile(direction, out MapTile _, out MapTile _, roll);
	}

	private void OnMove (InputValue inputValue) {
		if (gameManager.State != GameManager.GameState.LEVEL) {
			return;
		}

		Vector2 input = inputValue.Get<Vector2>( );

		movement = new Vector3(input.x, 0, input.y);
	}
}
