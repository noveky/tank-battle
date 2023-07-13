using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardPanel : MonoBehaviour
{
	[SerializeField] Text scoreboardText = null;

	public void ShowScoreboard()
	{
		scoreboardText.text = GameManager.instance.GetScoreboard();
	}

	public void ClearScoreboard()
	{
		GameManager.instance.ClearScoreboard();
		ShowScoreboard();
	}
}
