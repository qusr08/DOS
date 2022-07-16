using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
	public Vector3 AverageMapPosition;

	private void Start ( ) {
		Vector3 positionSum = Vector3.zero;
		foreach (Transform child in transform) {
			positionSum += child.position;
		}

		AverageMapPosition = positionSum / transform.childCount;
	}
}
