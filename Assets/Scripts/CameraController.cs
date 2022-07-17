using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	[SerializeField] private MapManager map;
	[SerializeField] private PlayerController die;
	[SerializeField] private GameManager gameManager;
	[Space]
	[SerializeField] private float distance;
	[SerializeField] [Range(0, 1)] private float smoothness;
	[Space]
	[SerializeField] private float horizontalMenuOffset;
	[SerializeField] private float verticalMenuOffset;

	private Vector3 toPosition = Vector3.zero;
	private Vector3 velocity;

	private void OnValidate ( ) {
		map = FindObjectOfType<MapManager>( );
		die = FindObjectOfType<PlayerController>( );
		gameManager = FindObjectOfType<GameManager>( );

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
		// Update the position that the camera should look at
		// toPosition = (map.AverageMapPosition + die.transform.position) / 2f;
		toPosition = die.transform.position;

		// Move the camera to different menus
		switch (gameManager.State) {
			case GameManager.GameState.MAIN_MENU:
			case GameManager.GameState.LEVEL_SELECT:
			case GameManager.GameState.CREDITS:
				toPosition.x += horizontalMenuOffset;
				toPosition.y += verticalMenuOffset;
				toPosition.z += horizontalMenuOffset;

				break;
			case GameManager.GameState.LEVEL_INTERRUPT:
				toPosition.x += horizontalMenuOffset;
				toPosition.z += horizontalMenuOffset;

				break;
		}

		float w = distance * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
		toPosition.x += w / Mathf.Sqrt(2);
		toPosition.y += distance * Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad);
		toPosition.z += -w / Mathf.Sqrt(2);
	}
}
