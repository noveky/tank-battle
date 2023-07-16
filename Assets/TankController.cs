using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankController : MonoBehaviour
{
	[System.NonSerialized] public Tank tank = null;

	public int startDirection = 0;
	[SerializeField] bool botControlEnabled = true; // 如果tank.isMine == false，则决定是否开启自动控制
	[System.NonSerialized] public int direction = 0;
	[System.NonSerialized] public bool moving = false;
	Stack<int> directionStack = new Stack<int>();
	string[] movButtons = { "Up", "Right", "Down", "Left" };

	[SerializeField] float botPressDesireFactor = 1f; // bot推进（拆墙、前往目标点）意愿系数

	// Bot
	float targetImportance = 0f; // 综合了目标点本身优先级和距离两个因素的新优先级
	bool grassAhead = false;
	bool wallAhead = false;
	bool playerTankAhead = false;
	bool botTankAhead = false;
	bool baseAhead = false;
	float distToObj = float.PositiveInfinity;
	//float distToTarget = float.PositiveInfinity;
	Wall facingWall = null;
	Tank facingTank = null;

	float simElapsedTime = 0f;

	private void Awake()
	{
		tank = GetComponent<Tank>();
	}

	private void Start()
	{
		direction = startDirection;

		if (!tank.isMine) BotStart();
	}

	void Inputs()
	{
		// Moving
		int lastDirection = direction;

		for (int i = 0; i < movButtons.Length; ++i)
		{
			if (Input.GetButtonDown(movButtons[i] + tank.playerNum)) directionStack.Push(i);
			if (directionStack.Count != 0 && Input.GetButtonUp(movButtons[i] + tank.playerNum) && directionStack.Peek() == i) directionStack.Pop();
		}
		while (directionStack.Count != 0 && !Input.GetButton(movButtons[directionStack.Peek()] + tank.playerNum))
		{
			directionStack.Pop();
		}

		moving = directionStack.Count != 0;
		if (moving) direction = directionStack.Peek();

		tank.OnTurn(lastDirection, direction);

		// Fire input
		if (Input.GetButton("Fire" + tank.playerNum))
		{
			tank.Fire();
		}
	}

	void BotStart()
	{
		if (!botControlEnabled) return;

		moving = Random.Range(0f, 1f) < 0.5f;
	}

	float BotGetTargetImportance(int direction)
	{
		// 检测前方目标点
		float targetImportance = 0f;
		//distToTarget = float.PositiveInfinity;
		//RaycastHit[] targetHits = Physics.RaycastAll(tank.muzzleTrans.position, tank.muzzleTrans.forward, float.PositiveInfinity, LayerMask.GetMask("BotTarget"));
		RaycastHit[] targetHits = Physics.RaycastAll(tank.transform.position, Quaternion.Euler(0f, 90f * direction, 0f) * Vector3.forward, float.PositiveInfinity, LayerMask.GetMask("BotTarget"));
		//if (Physics.Raycast(tank.muzzleTrans.position, tank.muzzleTrans.forward, out hitInfo, float.PositiveInfinity, LayerMask.GetMask("BotTarget")))
		foreach (RaycastHit hitInfo0 in targetHits)
		{
			BotTarget target = hitInfo0.transform.GetComponent<BotTarget>();
			if (target != null && hitInfo0.distance <= target.range)
			{
				float newImportance = target.importance * Mathf.Clamp01(0.5f + 0.5f * (hitInfo0.distance / target.range));//Mathf.Sin(Mathf.PI * (hitInfo0.distance / target.range)));
				targetImportance = Mathf.Max(targetImportance, newImportance);

				/*
				if (target.importance > targetImportance)
				{
					// 取最高优先级的目标点
					targetImportance = target.importance;
					distToTarget = hitInfo0.distance;
				}
				else if (target.importance == targetImportance)
				{
					// 取最远的同级目标点
					distToTarget = Mathf.Max(distToTarget, hitInfo0.distance);
				}
				*/
			}
		}
		return targetImportance;
	}

	void BotControl()
	{
		if (!botControlEnabled) return;

		//RaycastHit hitInfo = new RaycastHit();

		targetImportance = BotGetTargetImportance(direction);

		// 检测前方物体
		distToObj = float.PositiveInfinity;
		grassAhead = false;
		wallAhead = false;
		playerTankAhead = false;
		botTankAhead = false;
		baseAhead = false;
		facingWall = null;
		facingTank = null;
		if (Physics.Raycast(tank.muzzleTrans.position, tank.muzzleTrans.forward, out RaycastHit hitInfo, float.PositiveInfinity, LayerMask.GetMask("Wall", "Tank", "Grass")))
		{
			distToObj = hitInfo.distance;
			if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Grass"))
			{
				grassAhead = true;
			}
			else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				facingWall = hitInfo.transform.GetComponent<Wall>();
				if (facingWall != null && facingWall.destroyable)
				{
					if (facingWall.isBase) baseAhead = true;
					else wallAhead = true;
				}
			}
			else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Tank"))
			{
				facingTank = hitInfo.transform.GetComponent<Tank>();
				if (facingTank != null)
				{
					if (facingTank.isMine) playerTankAhead = true;
					else botTankAhead = true;
				}
			}
		}


		// 随机改变方向
		if (Random.Range(0f, 1f) < (baseAhead ? 0f : (playerTankAhead ? 0.0004f : (wallAhead ? 0.001f / (1f + facingWall.botBreakingDesireBonus / (1f + distToObj * 0.1f)) : (botTankAhead ? 0.006f : 0.002f)))) / (1f + targetImportance * 2f * (0.5f + botPressDesireFactor * 0.5f)/* / (1f + distToTarget * 0.1f) */))
		{
			int lastDirection = direction;

			direction = Random.Range(0, 4);

			tank.OnTurn(lastDirection, direction);
		}

		// 随机改变移动状态
		/*if ((baseAhead || playerTankAhead) && dist < 2f)
		{
			moving = false;
		}
		else */
		if (Random.Range(0f, 1f) < 0.005f)
		{
			moving = Random.Range(0f, 1f) < 0.8f * (1f + targetImportance * 0.04f * (0.5f + botPressDesireFactor * 0.5f));// / (1f + distToTarget * 0.1f));
		}

		// 遇岩浆则视为障碍物
		if (!tank.InLava && tank.InFrontOfLava)
		{
			//moving = false;
			OnBotObstacle();
		}

		// 如果前方是草丛或者可破坏的基地、墙或玩家坦克，则随机射击
		if (tank.fireTimer == 0f)
		{
			if (grassAhead && Random.Range(0f, 1f) < 0.001f || wallAhead && Random.Range(0f, 1f) < 0.005f * botPressDesireFactor * (1f + facingWall.botBreakingDesireBonus / (1f + distToObj * 0.1f)) * (1f + targetImportance * 0.6f/* / (1f + distToTarget * 0.1f) */) || playerTankAhead && Random.Range(0f, 1f) < 0.016f || baseAhead && Random.Range(0f, 1f) < 0.016f)
			{
				tank.Fire();
			}
		}
	}

	public void OnBotTakeDamage()
	{
		if (!botControlEnabled) return;

		//TODO
	}

	public void BotForceMove(int direction)
	{
		int lastDirection = this.direction;
		this.direction = direction;
		tank.OnTurn(lastDirection, direction);
		moving = true;
	}

	public void OnBotObstacle() // 当bot坦克撞到障碍物时
	{
		if (!botControlEnabled) return;

		// 催促前方的坦克
		/*if (botTankAhead)
		{
			facingTank.controller.BotForceMove(direction);
		}*/

		float maxTargetImpt = 0; // 四个方向中最高的目标重要程度
		int maxImptDirection = 0; // 目标重要程度最高的方向
		for (int i = 0; i < 4; ++i)
		{
			float targetImpt = BotGetTargetImportance(i);
			if (targetImpt > maxTargetImpt)
			{
				maxTargetImpt = targetImpt;
				maxImptDirection = i;
			}
		}

		// 以较大概率随机转弯，若前方有目标点则概率减小，若前方是射击口（即炮弹可以过但坦克不能过）则概率减小
		/*if (Vector3.Distance(transform.position, new Vector3(-4f, 1f, -4f)) <= 1f)
		{
			Debug.Log($"(({baseAhead} || {playerTankAhead} ? 0f : 1f / (1f + ({targetImportance} - {maxTargetImpt}) * 0.4f * (0.5f + {botPressDesireFactor} * 0.5f)) / ({facingWall} == null ? 1f : 1f + {facingWall.botBreakingDesireBonus} * 0.2f)) / (1f + 2f * Mathf.Clamp01({distToObj} * 0.5f)))" + ((baseAhead || playerTankAhead ? 0f : 1f / (1f + (targetImportance - maxTargetImpt) * 0.4f * (0.5f + botPressDesireFactor * 0.5f)) / (facingWall == null ? 1f : 1f + facingWall.botBreakingDesireBonus * 0.2f)) / (1f + 2f * Mathf.Clamp01(distToObj * 0.5f))).ToString());
		}*/
		/*if (Random.Range(0f, 1f) < (baseAhead || playerTankAhead ? 0f : 1f / (1f + (targetImportance - maxTargetImpt) * 0.4f * (0.5f + botPressDesireFactor * 0.5f)) / (facingWall == null ? 1f : 1f + facingWall.botBreakingDesireBonus * 0.2f)) / (1f + 2f * Mathf.Clamp01(distToObj * 0.5f)))
		{*/
		if (baseAhead || playerTankAhead ||
			Random.Range(0f, 1f) < 0.3f // 初始停止移动（不转弯）概率
			* Mathf.Clamp(1f + 0.5f * targetImportance * (0.5f + botPressDesireFactor * 0.5f), 0f, 5f) // 前方目标点越重要，越可能停止
			* Mathf.Clamp(1f + 0.2f * (targetImportance - maxTargetImpt), 0.2f, 1f) // 最优方向相对于前方越重要，越可能转弯
		)
		{
			moving = false;
		}
		else
		{

			int lastDirection = direction;

			if (Random.Range(0f, 1f) < Mathf.Clamp(0.4f * maxTargetImpt * (0.5f + botPressDesireFactor * 0.5f), 0f, 0.8f))
			{
				direction = maxImptDirection;
			}
			else
			{
				if (Random.Range(0, 2) == 1) direction = (direction + 1) % 4;
				else direction = (direction + 3) % 4;
			}

			tank.OnTurn(lastDirection, direction);
		}
		/*else
		{
			moving = false;
		}*/
	}

	private void Update()
	{
		if (LevelManager.Instance.paused) return;

		if (tank.isMine) Inputs();
		else
		{
			while (simElapsedTime >= Simulation.timestep)
			{
				BotControl();

				simElapsedTime -= Simulation.timestep;
			}
			simElapsedTime += Time.deltaTime;
		}
	}
}
