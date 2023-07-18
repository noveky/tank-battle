using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearingMenu : MonoBehaviour
{
	/*[SerializeField] bool gameCompleteMenu = false;

	private void Start()
	{
		if (gameCompleteMenu) GameManager.Instance.OnExitGamePlay();
	}*/

	public void NextLevel()
	{
		SceneManager.LoadScene(LevelManager.Instance.nextLevelSceneName);
	}

	public void SaveAndBack()
	{
		if (LevelManager.Instance.LevelComplete()) // 关卡通过了
		{
			GameManager.Instance.SaveGame(LevelManager.Instance.nextLevelSceneName);
		}
		else if (LevelManager.Instance.LevelEnd()) // 关卡结束但失败了，把分扣回去
		{
			GameManager.Instance.score -= LevelManager.Instance.score;
		}
		else // 关卡未结束
		{
			GameManager.Instance.SaveGame(SceneManager.GetActiveScene().name);
		}
		BackHome();
	}

	public void BackHome()
	{
		SceneManager.LoadScene(GameManager.mainMenuScene);
	}

	public void Restart()
	{
		LevelManager.Instance.RestartLevel();
	}

	public void SubmitScore(InputField nameInput)
	{
		name = nameInput.text;
		if (name.Trim() == string.Empty)
		{
			name = "Player";
		}
		GameManager.Instance.SubmitScore(name);
		BackHome();
	}
}
