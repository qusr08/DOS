using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
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
		die = FindObjectOfType<PlayerController>( );
		gameManager = FindObjectOfType<GameManager>( );

		if (die == null) {
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

	private void LateUpdate ( ) {
		if (gameManager.State == GameManager.GameState.LEVEL_RESTART && (transform.position - toPosition).magnitude < 2f) {
			gameManager.CurrentLevel.StartLevel( );
		}
	}

	private void UpdatePosition ( ) {
		// Update the position that the camera should look at
		toPosition = die.transform.position;

		// Move the camera to different menus
		switch (gameManager.State) {
			case GameManager.GameState.MAIN_MENU:
			case GameManager.GameState.LEVEL_SELECT:
			case GameManager.GameState.CREDITS:
				toPosition = new Vector3(horizontalMenuOffset, verticalMenuOffset, horizontalMenuOffset);

				break;
			case GameManager.GameState.LEVEL_PAUSE:
			case GameManager.GameState.LEVEL_COMPLETE:
				toPosition.x += horizontalMenuOffset;
				toPosition.z += horizontalMenuOffset;

				break;
			case GameManager.GameState.LEVEL_RESTART:
				toPosition = new Vector3(0, verticalMenuOffset / 2f, 0);

				break;
		}

		float w = distance * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad);
		toPosition.x += w / Mathf.Sqrt(2);
		toPosition.y += distance * Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad);
		toPosition.z += -w / Mathf.Sqrt(2);
	}
}
