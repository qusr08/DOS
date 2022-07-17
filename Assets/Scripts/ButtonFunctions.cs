using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour, IPointerEnterHandler {
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip interactSound;

	public void OnPointerEnter (PointerEventData eventData) {
		audioSource.PlayOneShot(interactSound, 0.05f);
	}
}
