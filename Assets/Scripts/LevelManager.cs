using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	[SerializeField] private Text levelNameText;
	[SerializeField] private GameManager gameManager;
	[SerializeField] private PlayerController die;
	[Space]
	[SerializeField] public GameObject LevelGameObject;
	[SerializeField] private Vector3 diceStartingRotation;

	private void OnValidate ( ) {
		gameManager = FindObjectOfType<GameManager>( );
		die = FindObjectOfType<PlayerController>( );
	}

	private void Awake ( ) {
		LevelGameObject.SetActive(false);
	}

	private void Start ( ) {
		OnValidate( );
	}

	public void StartLevel ( ) {
		gameManager.CurrentLevel.LevelGameObject.SetActive(false);
		LevelGameObject.SetActive(true);

		gameManager.CurrentLevel = this;
		gameManager.SetGameState(GameManager.GameState.LEVEL);

		// Make sure the die starts in the correct position
		die.transform.position = Vector3.zero;
		die.transform.eulerAngles = diceStartingRotation;
		die.UpdateDie( );

		// Make sure all tiles start in the correct position
		foreach (MapTile mapTile in LevelGameObject.transform.GetComponentsInChildren<MapTile>( )) {
			mapTile.ResetTile( );
		}
	}

	public void OnPointerEnter (PointerEventData eventData) {
		levelNameText.text = LevelGameObject.name;
	}

	public void OnPointerExit (PointerEventData eventData) {
		levelNameText.text = "";
	}
}
