using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
	public string levelName = string.Empty;
	public string nextLevel = string.Empty;

	[SerializeField] AudioClip battleClip = null;

	[System.NonSerialized] public int killCount = 0, deathCount = 0, score = 0;

	[SerializeField] GameManager.GameMode defaultGameMode = GameManager.GameMode.UNDEFINED;//bool isVersusLevel = false; // ��ս��ͼ
	[SerializeField] SpawnPoint mySpawnPointA = null, mySpawnPointB = null; // �����ؿ�������������
	public Base playerBase = null; // �����ؿ�����һ���
	public Base baseA = null, baseB = null; // ˫�˶�ս����������

	[SerializeField] Transform enemySpawnPointsTrans = null;
	/*[System.NonSerialized]*/ public int tanksToKill = 0;

	[SerializeField] PausePanel pausePanel = null;
	[SerializeField] LevelStartPanel startPanel = null;
	[SerializeField] LevelEndPanel completePanel = null;
	[SerializeField] LevelEndPanel defeatPanel = null;
	[SerializeField] LevelEndPanel playerAPanel = null, playerBPanel = null; // ˫�˶�ս��������ҷֱ�Ļ�ʤ����

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

		// �ؿ�����
		switch (GameManager.instance.gameMode)
		{
			case GameManager.GameMode.DUALBATTLE_MODE:
				{

					if (baseA.destroyed)
					{
						// ���Bʤ��
						playerBPanel.Show();
					}
					else
					{
						// ���Aʤ��
						playerAPanel.Show();
					}
					break;
				}
			case GameManager.GameMode.SINGLEPLAYER_MODE:
			case GameManager.GameMode.COOP_MODE:
				{
					// ͳ�Ʒ���
					GameManager.instance.score += score;

					// ��ʾ�ؿ��������
					string data = killCount + "\n" + deathCount + "\n" + score + "\n" + GameManager.instance.score;
					if (playerBase.destroyed)
					{
						// ��һ��ر����٣�ͨ��ʧ��
						defeatPanel.Show();
						defeatPanel.dataText.text = data;
					}
					else
					{
						// �ؿ����
						completePanel.Show();
						completePanel.dataText.text = data;
					}
					break;
				}
		}
		yield return new WaitForSeconds(2f); // �ӳ�һЩʱ��
		SetPaused(true); // ��ͣ
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
