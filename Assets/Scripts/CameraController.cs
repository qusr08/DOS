using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	[SerializeField] private Map map;
	[SerializeField] private PlayerController die;
	[Space]
	[SerializeField] private float distance;
	[SerializeField] [Range(0, 1)] private float smoothness;

	private Vector3 toPosition = Vector3.zero;
	private Vector3 velocity;

	private void OnValidate ( ) {
		map = FindObjectOfType<Map>( );
		die = FindObjectOfType<PlayerController>( );

		if (die == null || map == null) {
			return;
		}

		UpdatePosition( );
		transform.position = toPosition;
	}

	private void Start ( ) {
		OnValidate( );
	}

	private void Update ( ) {
		transform.position = Vector3.SmoothDamp(transform.position, toPosition, ref velocity, smoothness);

		UpdatePosition( );
	}

	private void UpdatePosition ( ) {
		// toPosition = (map.AverageMapPosition + die.transform.position) / 2f;
		toPosition = die.transform.position;

		float w = distance * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
		toPosition.x += w / Mathf.Sqrt(2);
		toPosition.y += distance * Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad);
		toPosition.z += -w / Mathf.Sqrt(2);
	}
}
