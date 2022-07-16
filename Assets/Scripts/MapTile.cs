using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {
	private enum MapTileType {
		NONE, FLIP, ROTATE_90_LEFT, ROTATE_90_RIGHT, ROTATE_180, MOVE_LEFT, MOVE_RIGHT, MOVE_UP, MOVE_DOWN, EXCLUSIVE
	}

	[SerializeField] private Vector3 moveDieOnStep;
	[SerializeField] private Vector3 rotateDieOnStep;
	[SerializeField] [Range(0, 6)] public int ExclusiveFace;
	[Space]
	[SerializeField] private float elevationAmount;

	private Coroutine effect;

	public bool IsMovingDie {
		get {
			return (effect != null);
		}
	}

	private void OnValidate ( ) {
		// TODO: update the model of the tile based on the serialized values in Unity
	}

	public void EffectDie (Transform die) {
		if (!IsMovingDie) {
			effect = StartCoroutine(IEffectDie(die));
		}
	}

	private IEnumerator IEffectDie (Transform die) {
		// TODO: make die float above the tile while rotating ???
		// TODO: rotate and move die by amount specified

		effect = null;

		yield return null;
	}
}
