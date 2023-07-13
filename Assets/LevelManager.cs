using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
	public string levelName = string.Empty;
	public string nextLevel = string.Empty;

	[SerializeField] AudioClip battleClip = null;

	[System.NonSerialized] public int killCount = 0, deathCount = 0, score = 0;

	[SerializeField] GameManager.GameMode defaultGameMode = GameManager.GameMode.UNDEFINED;//bool isVersusLevel = false; // 对战地图
	[SerializeField] SpawnPoint mySpawnPointA = null, mySpawnPointB = null; // 单机关卡的两个出生点
	public Base playerBase = null; // 单机关卡的玩家基地
	public Base baseA = null, baseB = null; // 双人对战的两个基地

	[SerializeField] Transform enemySpawnPointsTrans = null;
	/*[System.NonSerialized]*/ public int tanksToKill = 0;

	[SerializeField] PausePanel pausePanel = null;
	[SerializeField] LevelStartPanel startPanel = null;
	[SerializeField] LevelEndPanel completePanel = null;
	[SerializeField] LevelEndPanel defeatPanel = null;
	[SerializeField] LevelEndPanel playerAPanel = null, playerBPanel = null; // 双人对战的两个玩家分别的获胜界面

	[System.NonSerialized] public bool paused = false;

	private void Awake()
	{
		//if (defaultGameMode == GameManager.GameMode.UNDEFINED)
		if (isGeneratingInstance)
		{
			enabled = false;
			SetPaused(true);
			return;
		}

		if (GameManager.instance.gameMode == GameManager.GameMode.UNDEFINED) GameManager.instance.gameMode = defaultGameMode;

		if (GameManager.instance.gameMode == GameManager.GameMode.COOP_MODE) mySpawnPointB.gameObject.SetActive(true);
		if (GameManager.instance.gameMode == GameManager.GameMode.DUALBATTLE_MODE) pausePanel.saveAndBackButton.interactable = false;
		if (GameManager.instance.gameMode != GameManager.GameMode.DUALBATTLE_MODE) pausePanel.restartButton.interactable = false;

		for (int i = 0; i < enemySpawnPointsTrans.childCount; ++i)
		{
			if (enemySpawnPointsTrans.GetChild(i).gameObject.activeSelf) ++tanksToKill;
		}
		//tanksToKill = enemySpawnPointsTrans.childCount;

		startPanel.gameObject.SetActive(true);
	}

	private void Start()
	{
		SetPaused(false);
		GameManager.instance.OnEnterGamePlay();
		GameManager.instance.ChangeBattleClip(battleClip);

		StartCoroutine(LevelHandler());
	}

	public void SetPaused(bool paused)
	{
		this.paused = paused;
		Time.timeScale = (paused ? 0f : 1f);
	}

	bool LevelEnd()
	{
		if (GameManager.instance.gameMode == GameManager.GameMode.DUALBATTLE_MODE) return baseA.destroyed || baseB.destroyed;
		else return playerBase.destroyed || tanksToKill == 0;
	}

	IEnumerator LevelHandler()
	{
		yield return new WaitUntil(LevelEnd);

		// 关卡结束
		switch (GameManager.instance.gameMode)
		{
			case GameManager.GameMode.DUALBATTLE_MODE:
				{

					if (baseA.destroyed)
					{
						// 玩家B胜利
						playerBPanel.Show();
					}
					else
					{
						// 玩家A胜利
						playerAPanel.Show();
					}
					break;
				}
			case GameManager.GameMode.SINGLEPLAYER_MODE:
			case GameManager.GameMode.COOP_MODE:
				{
					// 统计分数
					GameManager.instance.score += score;

					// 显示关卡结算界面
					string data = killCount + "\n" + deathCount + "\n" + score + "\n" + GameManager.instance.score;
					if (playerBase.destroyed)
					{
						// 玩家基地被销毁，通关失败
						defeatPanel.Show();
						defeatPanel.dataText.text = data;
					}
					else
					{
						// 关卡完成
						completePanel.Show();
						completePanel.dataText.text = data;
					}
					break;
				}
		}
		yield return new WaitForSeconds(2f); // 延迟一些时间
		SetPaused(true); // 暂停
	}

	private void Update()
	{
		if (LevelEnd())
		{
			pausePanel.gameObject.SetActive(false);
		}
		else
		{
			if (Input.GetButtonDown("Cancel"))
			{
				SetPaused(!paused);
			}

			pausePanel.gameObject.SetActive(paused);
		}
	}
}
