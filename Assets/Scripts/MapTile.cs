using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {
	public enum MapTileType {
		NONE, ROTATE_90_LEFT, ROTATE_90_RIGHT, ROTATE_180, FLIP,
		MOVE_NEG_X, MOVE_POS_X, MOVE_POS_Z, MOVE_NEG_Z, CRACKED
	}

	[SerializeField] public MapTileType Type;
	[SerializeField] [Range(0, 6)] public int ExclusiveFace;
	[Space]
	[SerializeField] private float elevationAmount;
	[SerializeField] private float destroyElevationAmount;
	[SerializeField] private float turnSpeed;
	[SerializeField] private float moveSpeed;

	private Coroutine effect = null;

	public bool IsMovingDie {
		get {
			return (effect != null);
		}
	}

	private void OnValidate ( ) {
		// TODO: update the model of the tile based on the serialized values in Unity
	}

	public void EffectDie (PlayerController die) {
		if (!IsMovingDie) {
			effect = StartCoroutine(IEffectDie(die));
		}
	}

	private IEnumerator IEffectDie (PlayerController die) {
		// TODO: make die float above the tile while rotating ???

		Vector3 fromPosition = die.transform.position;
		Vector3 toPosition = fromPosition;
		Quaternion fromRotation = die.transform.rotation;
		Quaternion toRotation = fromRotation;

		// Get the from and to positions to move the die by
		switch (Type) {
			case MapTileType.ROTATE_90_LEFT:
				toRotation = Quaternion.AngleAxis(90, Vector3.up) * fromRotation;
				break;
			case MapTileType.ROTATE_90_RIGHT:
				toRotation = Quaternion.AngleAxis(-90, Vector3.up) * fromRotation;
				break;
			case MapTileType.ROTATE_180:
				toRotation = Quaternion.AngleAxis(180, Vector3.up) * fromRotation;
				break;
			case MapTileType.FLIP:
				toRotation = Quaternion.AngleAxis(180, Vector3.right) * fromRotation;
				break;
			case MapTileType.MOVE_NEG_X:
				toPosition += Vector3.left;
				break;
			case MapTileType.MOVE_POS_X:
				toPosition += Vector3.right;
				break;
			case MapTileType.MOVE_POS_Z:
				toPosition += Vector3.forward;
				break;
			case MapTileType.MOVE_NEG_Z:
				toPosition += Vector3.back;
				break;
		}

		// Check to see if the dice can move by sliding to the specified spot
		bool canMove = die.IsValidMapTile(toPosition - die.transform.position, roll: false);

		// If the tile specifies that the die should move or rotate, then lerp between the vectors
		if ((canMove && fromPosition != toPosition) || fromRotation != toRotation) {
			float rotateTime = 0;
			float moveTime = 0;

			// while (!CloseEnough(die.transform.position, toPosition) || !CloseEnough(die.transform.eulerAngles, toRotation.eulerAngles)) {
			while (rotateTime < 1 || moveTime < 1) {
				rotateTime += turnSpeed * Time.deltaTime;
				moveTime += moveSpeed * Time.deltaTime;

				die.transform.SetPositionAndRotation(
					Vector3.Slerp(fromPosition, toPosition, moveTime),
					Quaternion.Slerp(fromRotation, toRotation, rotateTime)
				);

				yield return new WaitForEndOfFrame( );
			}

			// Set positions at the end to make sure that the position/rotation of the die doesnt get messed up
			die.transform.SetPositionAndRotation(toPosition, toRotation);
		}

		effect = null;
	}

	private IEnumerator IDestroyTile ( ) {
		Vector3 fromPosition = transform.position;
		Vector3 toPosition = transform.position + (Vector3.down * destroyElevationAmount);

		float time = 0;
		while (time < 1) {
			time += moveSpeed * Time.deltaTime;

			transform.position = Vector3.Slerp(fromPosition, toPosition, time);

			yield return new WaitForEndOfFrame( );
		}

		transform.position = toPosition;

		Destroy(gameObject);
	}

	public void DestroyTile ( ) {
		StartCoroutine(IDestroyTile( ));
	}

	private bool CloseEnough (Vector3 vector1, Vector3 vector2) {
		return ((vector1 - vector2).magnitude <= 0.005f);
	}
}
