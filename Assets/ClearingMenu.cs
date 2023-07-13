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
		SceneManager.LoadScene(LevelManager.instance.nextLevel);
	}

	public void SaveAndBack()
	{
		GameManager.instance.SaveGame(LevelManager.instance.nextLevel);
		BackHome();
	}

	public void BackHome()
	{
		SceneManager.LoadScene(GameManager.mainMenuScene);
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void SubmitScore(InputField nameInput)
	{
		name = nameInput.text;
		if (name.Trim() == string.Empty)
		{
			name = "Player";
		}
		GameManager.instance.SubmitScore(name);
		BackHome();
	}
}
