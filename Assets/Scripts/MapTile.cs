using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {
	public enum MapTileType {
		NONE, ROTATE_90_LEFT, ROTATE_90_RIGHT, ROTATE_180, FLIP, MOVE_LEFT, MOVE_RIGHT, MOVE_UP, MOVE_DOWN
	}

	[SerializeField] public MapTileType Type;
	[SerializeField] [Range(0, 6)] public int ExclusiveFace;
	[Space]
	[SerializeField] private float elevationAmount;
	[SerializeField] private float movementSpeed;

	private Coroutine effect = null;

	public bool IsMovingDie {
		get {
			return (effect != null);
		}
	}

	private void OnValidate ( ) {
		// TODO: update the model of the tile based on the serialized values in Unity
	}

	public void EffectDie (Die die) {
		if (!IsMovingDie) {
			effect = StartCoroutine(IEffectDie(die));
		}
	}

	private IEnumerator IEffectDie (Die die) {
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
			case MapTileType.MOVE_LEFT:
				toPosition += Vector3.left;
				break;
			case MapTileType.MOVE_RIGHT:
				toPosition += Vector3.right;
				break;
			case MapTileType.MOVE_UP:
				toPosition += Vector3.forward;
				break;
			case MapTileType.MOVE_DOWN:
				toPosition += Vector3.back;
				break;
		}

		// Check to see if the dice can move by sliding to the specified spot
		bool canMove = die.IsValidMapTile(toPosition - die.transform.position, roll: false);

		// If the tile specifies that the die should move or rotate, then lerp between the vectors
		if ((canMove && fromPosition != toPosition) || fromRotation != toRotation) {
			float time = 0;
			while (time < 1) {
				time += movementSpeed * Time.deltaTime;

				die.transform.position = Vector3.Lerp(fromPosition, toPosition, time);
				die.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, time);

				yield return new WaitForEndOfFrame( );
			}

			// Set positions at the end to make sure that the position/rotation of the die doesnt get messed up
			die.transform.position = toPosition;
			die.transform.rotation = toRotation;
		}

		effect = null;
	}
}
