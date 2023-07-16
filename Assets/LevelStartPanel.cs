using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartPanel : MonoBehaviour
{
	public Text[] levelNameTexts = { };

	private void LateUpdate()
	{
		if (LevelManager.Instance.PreparationCountdown < 0)
		{
			gameObject.SetActive(false);
		}

		string textStr;
		if (LevelManager.Instance.ElapsedTime < 2)
		{
			textStr = LevelManager.Instance.levelName;
		}
		else if (LevelManager.Instance.PreparationCountdown == 0)
		{
			textStr = "GO!";
		}
		else
		{
			textStr = LevelManager.Instance.PreparationCountdown.ToString();
		}

		foreach (Text text in levelNameTexts)
		{
			text.text = textStr;
		}
	}
}
