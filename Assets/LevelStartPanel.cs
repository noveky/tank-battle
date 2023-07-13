using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartPanel : MonoBehaviour
{
	public Text[] texts = { };

	private void Start()
	{
		foreach (Text text in texts)
		{
			text.text = LevelManager.instance.levelName;
		}
		StartCoroutine(Hide());
	}

	IEnumerator Hide()
	{
		yield return new WaitForSeconds(2f);

		gameObject.SetActive(false);
	}
}
