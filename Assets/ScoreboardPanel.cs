using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardPanel : MonoBehaviour
{
	[SerializeField] Text scoreboardText = null;

	public void ShowScoreboard()
	{
		scoreboardText.text = GameManager.Instance.GetScoreboard();
	}

	public void ClearScoreboard()
	{
		GameManager.Instance.ClearScoreboard();
		ShowScoreboard();
	}
}
