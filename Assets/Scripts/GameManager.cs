using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public enum GameState {
		MAIN_MENU, LEVEL_SELECT, CREDITS, LEVEL, LEVEL_PAUSE, LEVEL_COMPLETE, LEVEL_RESTART, HELP
	}

	[SerializeField] private GameObject mainMenu;
	[SerializeField] private GameObject levelSelect;
	[SerializeField] private GameObject credits;
	[SerializeField] private GameObject help;
	[SerializeField] private GameObject level;
	[SerializeField] private GameObject levelPause;
	[SerializeField] private GameObject levelComplete;
	[Space]
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private float fadeSpeed;
	[SerializeField] private CameraController cameraController;
	[Space]
	[SerializeField] public GameState State;
	[SerializeField] public LevelManager CurrentLevel;

	private void Start ( ) {
		canvasGroup.alpha = 0;

		SetGameState((int) GameState.MAIN_MENU);
	}

	public void SetGameState (GameState state) {
		StartCoroutine(IFadeToGameState(state));
	}

	public void SetGameState (int stateIndex) {
		SetGameState((GameState) stateIndex);
	}

	public void Quit ( ) {
		Application.Quit( );
	}

	private IEnumerator IFadeToGameState (GameState state) {
		if (State == GameState.LEVEL_COMPLETE || (State == GameState.LEVEL_PAUSE && state != GameState.LEVEL && state != GameState.LEVEL_RESTART)) {
			cameraController.FlipCamera(false);
		}

		State = state;

		// Fade all ui out
		while (canvasGroup.alpha > 0) {
			canvasGroup.alpha -= Mathf.Min(fadeSpeed * Time.deltaTime, canvasGroup.alpha);

			yield return new WaitForEndOfFrame( );
		}

		// Deactivate all menus
		mainMenu.SetActive(false);
		levelSelect.SetActive(false);
		credits.SetActive(false);
		help.SetActive(false);
		level.SetActive(false);
		levelPause.SetActive(false);
		levelComplete.SetActive(false);

		// Enable the specified menu based on the game state
		switch (State) {
			case GameState.MAIN_MENU:
				mainMenu.SetActive(true);
				break;
			case GameState.LEVEL_SELECT:
				levelSelect.SetActive(true);
				break;
			case GameState.CREDITS:
				credits.SetActive(true);
				break;
			case GameState.HELP:
				help.SetActive(true);
				break;
			case GameState.LEVEL:
				level.SetActive(true);
				break;
			case GameState.LEVEL_PAUSE:
				levelPause.SetActive(true);
				break;
			case GameState.LEVEL_COMPLETE:
				levelComplete.SetActive(true);
				break;
		}

		// Fade all ui back in
		while (canvasGroup.alpha < 1) {
			canvasGroup.alpha += Mathf.Min(fadeSpeed * Time.deltaTime, 1f - canvasGroup.alpha);

			yield return new WaitForEndOfFrame( );
		}
	}
}
