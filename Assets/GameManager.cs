using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : Singleton<GameManager>
{
	//public const int deathPenalty = 200; // 死亡时的扣分数目

	public enum GameMode { UNDEFINED, SINGLEPLAYER_MODE, COOP_MODE, DUALBATTLE_MODE };

	public static string mainMenuScene = "MainMenu";
	public static string firstLevel = "Level1";
	public static string dualLevel = "1v1Map1";

	string scoreboardPathName = string.Empty;
	string loadLevelPathPrefix = string.Empty;

	[System.NonSerialized] public GameMode gameMode = GameManager.GameMode.UNDEFINED; // 0: 单人闯关，1：双人闯关，2：双人对战

	[System.NonSerialized] public int score = 0;

	[SerializeField] AudioSource mainMenuAudio = null;
	[SerializeField] AudioSource battleAudio = null;

	public void OnEnterGamePlay()
	{
		if (mainMenuAudio != null && mainMenuAudio.isPlaying) mainMenuAudio.Stop();
	}

	public void OnExitGamePlay()
	{
		if (battleAudio != null)
		{
			if (battleAudio.isPlaying) battleAudio.Stop();
			battleAudio.clip = null;
		}
		if (mainMenuAudio != null) mainMenuAudio.Play();
	}

	public void ChangeBattleClip(AudioClip battleClip)
	{
		//if (battleClip == null) return;

		if (battleAudio == null) return;

		if (battleClip == battleAudio.clip) return;

		if (battleAudio.isPlaying) battleAudio.Stop();
		battleAudio.clip = battleClip;
		if (battleClip != null) battleAudio.Play();
	}

	private void Awake()
	{
		//if (gameMode == GameMode.UNDEFINED) DontDestroyOnLoad(gameObject);
		//if (!isNewbornInstance) DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(gameObject);
		if (!isGeneratingInstance)
		{
			_instance = this;
			gameObject.name = "(scene singleton) GameManager";
			GameManager[] instances = FindObjectsOfType<GameManager>();
			foreach(GameManager ins in instances)
			{
				if (ins == this) continue;
				Destroy(ins.gameObject);
			}
		}

		scoreboardPathName = Application.streamingAssetsPath + "/scoreboard.txt";
		loadLevelPathPrefix = Application.streamingAssetsPath + "/savedlevel_";
	}

	private void Update()
	{
		// Cursor
		if (LevelManager.hasInstance)
		{
			if (LevelManager.instance.paused)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void SubmitScore(string name)
	{
		string str = string.Empty;
		if (File.Exists(scoreboardPathName)) str = File.ReadAllText(scoreboardPathName);
		//str += name + " " + score + "\n";
		string mode = string.Empty;
		switch (gameMode)
		{
			case GameMode.SINGLEPLAYER_MODE:
				{
					mode += "Singleplayer";
					break;
				}
			case GameMode.COOP_MODE:
				{
					mode = "Co-op";
					break;
				}
		}
		str += "Mode: " + mode + ", Name: " + name + ", Score: " + score + "\n";
		File.WriteAllText(scoreboardPathName, str);
	}

	string GetLoadLevelPathName()
	{
		string pathName = loadLevelPathPrefix; // 读档路径
		switch (gameMode)
		{
			case GameMode.SINGLEPLAYER_MODE:
				{
					pathName += "singleplayer.txt";
					break;
				}
			case GameMode.COOP_MODE:
				{
					pathName += "co-op.txt";
					break;
				}
		}
		return pathName;
	}

	public void SaveGame(string levelName)
	{
		string pathName = GetLoadLevelPathName();
		string str = levelName + "\n" + score;
		File.WriteAllText(pathName, str);
	}

	public string GetSavedLevelName()
	{
		string pathName = GetLoadLevelPathName();
		if (File.Exists(pathName))
		{
			string[] strSplit = File.ReadAllText(pathName).Split('\n');
			if (strSplit.Length >= 1)
			{
				string levelName = strSplit[0];
				int score = 0;
				if (strSplit.Length >= 2)
				{
					score = int.Parse(strSplit[1]);
				}
				this.score = score;
				return levelName;
			}
		}
		return string.Empty;
	}

	public void DeleteArchive()
	{
		string pathName = GetLoadLevelPathName();
		File.Delete(pathName);
	}

	public string GetScoreboard()
	{
		if (!File.Exists(scoreboardPathName)) return string.Empty;
		return File.ReadAllText(scoreboardPathName);
	}

	public void ClearScoreboard()
	{
		if (File.Exists(scoreboardPathName)) File.Delete(scoreboardPathName);
	}
}
