using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
	private void Start()
	{
		GameManager.instance.OnExitGamePlay();
	}

	public void GameMode(int mode)
	{
		switch (mode)
		{
			case 0:
				{
					GameManager.instance.gameMode = GameManager.GameMode.SINGLEPLAYER_MODE;
					break;
				}
			case 1:
				{
					GameManager.instance.gameMode = GameManager.GameMode.COOP_MODE;
					break;
				}
			case 2:
				{
					GameManager.instance.gameMode = GameManager.GameMode.DUALBATTLE_MODE;
					break;
				}
		}
		}

	public void NewGame()
	{
		// ���÷���
		GameManager.instance.score = 0;

		// ���عؿ�
		switch (GameManager.instance.gameMode)
		{
			case GameManager.GameMode.SINGLEPLAYER_MODE:
			case GameManager.GameMode.COOP_MODE:
				{
					SceneManager.LoadScene(GameManager.firstLevel);
					break;
				}
			case GameManager.GameMode.DUALBATTLE_MODE:
				{
					SceneManager.LoadScene(GameManager.dualLevel);
					break;
				}
		}
	}

	public void LoadGame()
	{
		string levelName = GameManager.instance.GetSavedLevelName();
		if (levelName.Trim() != string.Empty)
		{
			// ��ȡ��Ͻ���浵ʱ��ɾ���ô浵
			GameManager.instance.DeleteArchive();

			// ���عؿ�
			SceneManager.LoadScene(levelName);
		}
	}

	public void Quit()
	{
		Application.Quit();
	}
}
