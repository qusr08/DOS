using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public enum GameState {
		MAIN_MENU, LEVEL_SELECT, CREDITS, LEVEL, LEVEL_PAUSE, LEVEL_COMPLETE, LEVEL_RESTART
	}

	[SerializeField] private GameObject mainMenu;
	[SerializeField] private GameObject levelSelect;
	[SerializeField] private GameObject credits;
	[SerializeField] private GameObject level;
	[SerializeField] private GameObject levelPause;
	[SerializeField] private GameObject levelComplete;
	[Space]
	[SerializeField] public GameState State;
	[SerializeField] public LevelManager CurrentLevel;

	private void Start ( ) {
		SetGameState((int) GameState.MAIN_MENU);
	}

	public void SetGameState (GameState state) {
		State = state;

		mainMenu.SetActive(false);
		levelSelect.SetActive(false);
		credits.SetActive(false);
		level.SetActive(false);
		levelPause.SetActive(false);
		levelComplete.SetActive(false);

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
	}

	public void SetGameState (int stateIndex) {
		SetGameState((GameState) stateIndex);
	}

	public void Quit ( ) {
		Application.Quit( );
	}
}
