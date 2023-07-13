using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndPanel : MonoBehaviour
{
	public Text dataText = null;
	public Button defaultButton = null;
	public GameObject buttonsParentObj = null;

	private void Awake()
	{
		buttonsParentObj.SetActive(false);
	}

	private void Update()
	{
		if (!LevelManager.instance.paused) return;
		buttonsParentObj.SetActive(true);
		if (defaultButton != null && Input.GetButtonDown("Submit")) defaultButton.onClick.Invoke();
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}
}
