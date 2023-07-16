using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
	private void Start()
	{
		GameManager.Instance.OnExitGamePlay();
	}

	public void GameMode(int mode)
	{
		switch (mode)
		{
			case 0:
				{
					GameManager.Instance.gameMode = GameManager.GameMode.SINGLEPLAYER_MODE;
					break;
				}
			case 1:
				{
					GameManager.Instance.gameMode = GameManager.GameMode.COOP_MODE;
					break;
				}
			case 2:
				{
					GameManager.Instance.gameMode = GameManager.GameMode.DUALBATTLE_MODE;
					break;
				}
		}
		}

	public void NewGame()
	{
		// 重置分数
		GameManager.Instance.score = 0;

		// 加载关卡
		switch (GameManager.Instance.gameMode)
		{
			case GameManager.GameMode.SINGLEPLAYER_MODE:
			case GameManager.GameMode.COOP_MODE:
				{
					LoadLevel(GameManager.firstLevel);
					break;
				}
			case GameManager.GameMode.DUALBATTLE_MODE:
				{
					LoadLevel(GameManager.dualLevel);
					break;
				}
		}
	}

	public void LoadGame()
	{
		string levelName = GameManager.Instance.GetSavedLevelName();
		if (levelName.Trim() != string.Empty)
		{
			// 读取完毕进入存档时，删除该存档
			GameManager.Instance.DeleteArchive();

			// 加载关卡
			LoadLevel(levelName);
		}
	}

	public void LoadLevel(string levelName)
	{
		SceneManager.LoadScene(levelName);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
