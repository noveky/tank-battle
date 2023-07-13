using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndPanel : MonoBehaviour
{
	public Text scoreText = null;

	private void Awake()
	{
		scoreText.text = GameManager.instance.score.ToString();
	}
}
