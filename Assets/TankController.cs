using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankController : MonoBehaviour
{
	[System.NonSerialized] public Tank tank = null;

	public int startDirection = 0;
	[SerializeField] bool botControlEnabled = true; // ���tank.isMine == false��������Ƿ����Զ�����
	[System.NonSerialized] public int direction = 0;
	[System.NonSerialized] public bool moving = false;
	Stack<int> directionStack = new Stack<int>();
	string[] movButtons = { "Up", "Right", "Down", "Left" };

	[SerializeField] float botPressDesireFactor = 1f; // bot�ƽ�����ǽ��ǰ��Ŀ��㣩��Ըϵ��

	// Bot
	float targetImportance = 0f; // �ۺ���Ŀ��㱾�����ȼ��;����������ص������ȼ�
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
		// ���ǰ��Ŀ���
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
					// ȡ������ȼ���Ŀ���
					targetImportance = target.importance;
					distToTarget = hitInfo0.distance;
				}
				else if (target.importance == targetImportance)
				{
					// ȡ��Զ��ͬ��Ŀ���
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

		// ���ǰ������
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


		// ����ı䷽��
		if (Random.Range(0f, 1f) < (baseAhead ? 0f : (playerTankAhead ? 0.0004f : (wallAhead ? 0.001f / (1f + facingWall.botBreakingDesireBonus / (1f + distToObj * 0.1f)) : (botTankAhead ? 0.006f : 0.002f)))) / (1f + targetImportance * 2f * (0.5f + botPressDesireFactor * 0.5f)/* / (1f + distToTarget * 0.1f) */))
		{
			int lastDirection = direction;

			direction = Random.Range(0, 4);

			tank.OnTurn(lastDirection, direction);
		}

		// ����ı��ƶ�״̬
		/*if ((baseAhead || playerTankAhead) && dist < 2f)
		{
			moving = false;
		}
		else */
		if (Random.Range(0f, 1f) < 0.005f)
		{
			moving = Random.Range(0f, 1f) < 0.8f * (1f + targetImportance * 0.04f * (0.5f + botPressDesireFactor * 0.5f));// / (1f + distToTarget * 0.1f));
		}

		// ���ҽ�����Ϊ�ϰ���
		if (!tank.InLava && tank.InFrontOfLava)
		{
			//moving = false;
			OnBotObstacle();
		}

		// ���ǰ���ǲݴԻ��߿��ƻ��Ļ��ء�ǽ�����̹�ˣ���������
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

	public void OnBotObstacle() // ��bot̹��ײ���ϰ���ʱ
	{
		if (!botControlEnabled) return;

		// �ߴ�ǰ����̹��
		/*if (botTankAhead)
		{
			facingTank.controller.BotForceMove(direction);
		}*/

		float maxTargetImpt = 0; // �ĸ���������ߵ�Ŀ����Ҫ�̶�
		int maxImptDirection = 0; // Ŀ����Ҫ�̶���ߵķ���
		for (int i = 0; i < 4; ++i)
		{
			float targetImpt = BotGetTargetImportance(i);
			if (targetImpt > maxTargetImpt)
			{
				maxTargetImpt = targetImpt;
				maxImptDirection = i;
			}
		}

		// �Խϴ�������ת�䣬��ǰ����Ŀ�������ʼ�С����ǰ��������ڣ����ڵ����Թ���̹�˲��ܹ�������ʼ�С
		/*if (Vector3.Distance(transform.position, new Vector3(-4f, 1f, -4f)) <= 1f)
		{
			Debug.Log($"(({baseAhead} || {playerTankAhead} ? 0f : 1f / (1f + ({targetImportance} - {maxTargetImpt}) * 0.4f * (0.5f + {botPressDesireFactor} * 0.5f)) / ({facingWall} == null ? 1f : 1f + {facingWall.botBreakingDesireBonus} * 0.2f)) / (1f + 2f * Mathf.Clamp01({distToObj} * 0.5f)))" + ((baseAhead || playerTankAhead ? 0f : 1f / (1f + (targetImportance - maxTargetImpt) * 0.4f * (0.5f + botPressDesireFactor * 0.5f)) / (facingWall == null ? 1f : 1f + facingWall.botBreakingDesireBonus * 0.2f)) / (1f + 2f * Mathf.Clamp01(distToObj * 0.5f))).ToString());
		}*/
		/*if (Random.Range(0f, 1f) < (baseAhead || playerTankAhead ? 0f : 1f / (1f + (targetImportance - maxTargetImpt) * 0.4f * (0.5f + botPressDesireFactor * 0.5f)) / (facingWall == null ? 1f : 1f + facingWall.botBreakingDesireBonus * 0.2f)) / (1f + 2f * Mathf.Clamp01(distToObj * 0.5f)))
		{*/
		if (baseAhead || playerTankAhead ||
			Random.Range(0f, 1f) < 0.3f // ��ʼֹͣ�ƶ�����ת�䣩����
			* Mathf.Clamp(1f + 0.5f * targetImportance * (0.5f + botPressDesireFactor * 0.5f), 0f, 5f) // ǰ��Ŀ���Խ��Ҫ��Խ����ֹͣ
			* Mathf.Clamp(1f + 0.2f * (targetImportance - maxTargetImpt), 0.2f, 1f) // ���ŷ��������ǰ��Խ��Ҫ��Խ����ת��
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
