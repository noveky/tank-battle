using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	[SerializeField] GameObject tankPrefab = null;
	[SerializeField] bool randStartDirection = false;
	[SerializeField] int fixedStartDirection = -1; // -1表示默认使用tankPrefab的startDirection
	[SerializeField] float immortalTime = 3f;
	[SerializeField] bool respawnWhenDead = false;
	[SerializeField] float spawnTime = 0f;
	[SerializeField] float respawnTime = 0f;
	[SerializeField] int playerNum = 1;
	[SerializeField] ParticleSystem spawnParticle = null;

	GameObject tankObj = null;
	float spawnTimer = 0f;
	bool spawned = false;

	const float spawnParticlePlayTime = 1f;

	private void Awake()
	{
		tankPrefab.SetActive(false);
	}

	void Spawn()
	{
		tankObj = Instantiate(tankPrefab, tankPrefab.transform.position, tankPrefab.transform.rotation);
		tankObj.SetActive(true);

		TankController tankController = tankObj.GetComponent<TankController>();
		tankController.tank.playerNum = playerNum;
		tankController.tank.immortalTime = immortalTime;
		if (randStartDirection)
		{
			tankController.startDirection = Random.Range(0, 4);
		}
		else if (fixedStartDirection != -1)
		{
			tankController.startDirection = fixedStartDirection;
		}

		// 双人合作模式，敌方坦克血量加倍
		if (GameManager.Instance.gameMode == GameManager.GameMode.COOP_MODE)
		{
			if (tankController.tank.isMine)
			{
				tankController.tank.initHp *= 1.5f;
				tankController.tank.hp *= 1.5f;
				tankController.tank.fireInterval *= 0.75f;
			}
			else
			{
				tankController.tank.initHp *= 2f;
				tankController.tank.hp *= 2f;
				tankController.tank.fireInterval *= 1f;
			}
		}
	}

	private void Start()
	{
		if (playerNum == 0) // Enemy
		{
			spawnTimer = spawnTime + spawnParticlePlayTime + LevelManager.Instance.preparationTime;
		}
		else // Player
		{
			spawnTimer = spawnTime;
		}
		//StartCoroutine(DelaySpawn());
	}

	IEnumerator DelaySpawn()
	{
		yield return new WaitForSeconds(spawnTime);
		Spawn();
		spawned = true;
	}

	private void Update()
	{
		if (LevelManager.Instance.paused) return;

		if (spawnTimer >= 0f)
		{
			if (spawnTimer - Time.deltaTime < spawnParticlePlayTime)
			{
				if (spawnParticle != null) spawnParticle.Play();
			}
			spawnTimer -= Time.deltaTime;
		}
		if (spawnTimer < 0f && !spawned)
		{
			Spawn();
			if (spawnParticle != null) spawnParticle.Stop();
			spawned = true;
		}

		if (respawnWhenDead && spawned)
		{
			if (tankObj == null)
			{
				spawned = false;
				spawnTimer = respawnTime;
			}
		}
	}
}
