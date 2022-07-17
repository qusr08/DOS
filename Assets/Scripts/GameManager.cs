using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public enum GameState {
		MAIN_MENU, LEVEL_SELECT, CREDITS, LEVEL, LEVEL_INTERRUPT
	}

	[SerializeField] private GameObject mainMenu;
	[SerializeField] private GameObject levelSelect;
	[SerializeField] private GameObject credits;
	[SerializeField] private GameObject levelPause;
	[SerializeField] private GameObject levelComplete;
	[Space]
	[SerializeField] private GameState _state;

	public GameState State {
		get {
			return _state;
		}

		set {
			_state = value;

			mainMenu.SetActive(false);
			levelSelect.SetActive(false);
			credits.SetActive(false);
			levelPause.SetActive(false);
			levelComplete.SetActive(false);

			switch (_state) {
				case GameState.MAIN_MENU:
					break;
				case GameState.LEVEL_SELECT:
					break;
				case GameState.CREDITS:
					break;
				case GameState.LEVEL:
					break;
				case GameState.LEVEL_INTERRUPT:
					break;
			}
		}
	}

	public void Quit ( ) {
		Application.Quit( );
	}
}
