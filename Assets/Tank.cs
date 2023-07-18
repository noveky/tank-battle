using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
	public enum Item { NONE, HEALTH };

	public bool isMine = false;
	public int playerNum = 1;

	public GameObject hitParticlePrefab = null;
	[SerializeField] GameObject destroyParticlePrefab = null;

	public Transform muzzleTrans = null;
	[SerializeField] GameObject shellPrefabPlayer = null;
	[SerializeField] GameObject shellPrefabEnemy = null;
	[SerializeField] ParticleSystem[] firingParticles = { };
	[SerializeField] ParticleSystem[] dustParticles = { };
	[SerializeField] ParticleSystem immortalParticle = null; // �޵�ʱ������Ч��
	[SerializeField] ParticleSystem healthParticle = null; // ��ü�Ѫ���ߵ�����Ч��
	[SerializeField] ParticleSystem[] onFireParticles = { }; // �Ż�ʱ������Ч��
	[SerializeField] BotTarget botTarget = null;

	[SerializeField] AudioSource fireAudio = null;
	[SerializeField] AudioClip fireClip = null;

	[SerializeField] float initSpeed = 0f; // ��ʼ����
	float maxSpeed = 0f; // ��ǰ�������
	float speed = 0f; // ��ǰ����
	float decelEffect = 0f; // ����Ч��
	public bool OnIce { get; private set; } = false; // �Ƿ��ڻ���
	public bool InLava { get; private set; } = false; // �Ƿ��ڲ��ҽ�
	public bool InFrontOfLava { get; private set; } = false; // �Ƿ���ǰ���ҽ�
	List<Collider> iceColliders = new List<Collider>(); // �洢��ǰ�н����ı��津������ײ�壬���ǿ�ʱ˵���ڱ�����
	List<Collider> lavaColliders = new List<Collider>();

	[SerializeField] float shellSpeed = 0f;
	[SerializeField] float shellPower = 0f;
	[SerializeField] float shellDecelIndex = 0f; // �ڵ��ļ���Ч��ָ��

	[System.NonSerialized] public float immortalTime = 0f;

	public float initHp = 100f;
	[System.NonSerialized] public float hp = 0f;

	public float fireInterval = 0f;
	[System.NonSerialized] public float fireTimer = 0f;

	[System.NonSerialized] public bool isImmortal = true;
	float immortalTimer = 0f;

	[System.NonSerialized] public TankController controller = null;
	[System.NonSerialized] public TankAnimator animator = null;

	public Item item = Item.NONE; // ������ߣ�-1��ʾ�޵���
	public int points = 0; // ��ɱ��õķ���/�����ɱ�۳��ķ���

	private void Awake()
	{
		hp = initHp;

		controller = GetComponent<TankController>();
		animator = GetComponent<TankAnimator>();

		if (isMine) botTarget.gameObject.SetActive(true);

		//if (!isMine) enabled = controller.enabled = animator.enabled = false;
	}

	private void Start()
	{
		if (immortalTime > 0f)
		{
			// �����޵�ʱ��
			isImmortal = true;
			immortalTimer = immortalTime;
			immortalParticle.Play();
		}
		else
		{
			isImmortal = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ice"))
		{
			if (!iceColliders.Contains(other)) iceColliders.Add(other);
			OnIce = true;
		}
		else if (other.gameObject.layer == LayerMask.NameToLayer("Lava"))
		{
			if (!lavaColliders.Contains(other)) lavaColliders.Add(other);
			InLava = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ice"))
		{
			if (iceColliders.Contains(other)) iceColliders.Remove(other);
			if (iceColliders.Count == 0) OnIce = false;
		}
		else if (other.gameObject.layer == LayerMask.NameToLayer("Lava"))
		{
			if (lavaColliders.Contains(other)) lavaColliders.Remove(other);
			if (lavaColliders.Count == 0) InLava = false;
		}
	}

	public void Fire()
	{
		if (fireTimer > 0f) return;

		GameObject shellObj = Instantiate(isMine ? shellPrefabPlayer : shellPrefabEnemy, muzzleTrans.position, muzzleTrans.rotation);
		Shell shell = shellObj.GetComponent<Shell>();
		shell.tank = this;
		shell.speed = shellSpeed;
		shell.power = shellPower;
		shell.decelIndex = shellDecelIndex;

		foreach (ParticleSystem firingParticle in firingParticles)
		{
			firingParticle.Play();
		}

		fireAudio.PlayOneShot(fireClip);

		animator.PlayFireAnim();
		if (isMine) CameraAnimator.Instance.PlayFireAnim();

		fireTimer = fireInterval;
	}

	public void GetDecelerationEffect(float decelIndex)
	{
		decelEffect += Mathf.Pow(1.5f, decelIndex) - 1f;
	}

	public bool ReceiveDamage(float damage, Tank sourceTank = null) // �����ɱ�����ͷ���true�����򷵻�false
	{
		if (isImmortal || hp <= 0f) return false;

		controller.OnBotTakeDamage();

		hp -= damage;
		if (hp <= 0f)
		{
			hp = 0f;

			if (destroyParticlePrefab != null)
			{
				Instantiate(destroyParticlePrefab, transform.position, transform.rotation);
			}

			// �������
			if (sourceTank != null)
			{
				sourceTank.GetItem(item);
			}

			// ͳ��
			if (isMine)
			{
				++LevelManager.Instance.deathCount;
				LevelManager.Instance.ChangeScore(-GameManager.deathPenalty);
			}
			else
			{
				if (sourceTank != null && sourceTank.isMine) ++LevelManager.Instance.killCount;
				--LevelManager.Instance.tanksToKill;
				LevelManager.Instance.ChangeScore(points);
			}

			Destroy(gameObject);

			return true;
		}
		return false;
	}

	public void GetItem(Item item)
	{
		switch(item)
		{
			case Item.HEALTH: // ��Ѫ
				{
					hp = Mathf.Clamp(hp + initHp * 0.7f, 0f, initHp);
					healthParticle.Play();
					break;
				}
		}
	}

	public void OnTurn(int fromDirection, int toDirection) // ת���¼�
	{
		if (fromDirection == toDirection) return;
		
		//speed = 0f;

		transform.localEulerAngles = new Vector3(0f, controller.direction * 90f, 0f);
		//transform.position += transform.forward * 0.001f; // �ƶ�������룬��ֹ��ǽ�������¿�ǽ����

		animator.PlayTurnAnim(fromDirection, toDirection);
	}

	void Sim()
	{
		// �޵�ʱ���ʱ
		if (immortalTimer >= 0f)
		{
			immortalTimer -= Simulation.timestep;//Time.deltaTime;
		}
		if (immortalTimer < 0f && isImmortal)
		{
			immortalParticle.Stop();
			isImmortal = false;
		}

		// �ҽ�
		if (InLava)
		{
			if (!onFireParticles[0].isPlaying)
			{
				foreach (var onFireParticle in onFireParticles)
				{
					onFireParticle.Play();
				}
			}
			ReceiveDamage(120f * Simulation.timestep);
		}
		else
		{
			if (onFireParticles[0].isPlaying)
			{
				foreach (var onFireParticle in onFireParticles)
				{
					onFireParticle.Stop();
				}
			}
		}

		// �����ʱ
		fireTimer -= Simulation.timestep;//Time.deltaTime;
		if (fireTimer < 0f) fireTimer = 0f;

		// ��������OnTurn�����ڴ���
		//transform.localEulerAngles = new Vector3(0f, controller.direction * 90f, 0f);

		// �ƶ�
		Simulation.AutoDamp(ref decelEffect, 0.97f);
		maxSpeed = initSpeed * Mathf.Pow(0.3f, decelEffect) * (OnIce ? 1.4f : (InLava ? 0.3f : 1f));
		if (controller.moving)
		{
			Simulation.AutoTarget(ref speed, maxSpeed, (OnIce ? 0.6f : 0.3f));
		}
		else
		{
			Simulation.AutoTarget(ref speed, 0f, (OnIce ? 0.1f : 0.4f));
		}
		const float castRadius = 0.3f;
		const float castOffset = 0.1f;
		Vector3 castBoxHalfExtents = transform.TransformVector(new Vector3(castRadius, 0f, 0f));
		castBoxHalfExtents.x = Mathf.Abs(castBoxHalfExtents.x);
		castBoxHalfExtents.z = Mathf.Abs(castBoxHalfExtents.z);
		InFrontOfLava = Physics.BoxCast(transform.position - transform.forward * castOffset, castBoxHalfExtents * (0.7f / castRadius), transform.forward, Quaternion.identity, 0.8f + castRadius + castOffset, LayerMask.GetMask("Lava"), QueryTriggerInteraction.Collide);
		//if (controller.moving)
		{
			float dist = speed * Simulation.timestep;//Time.deltaTime;
			/*RaycastHit[] hitArray = Physics.BoxCastAll(transform.position, new Vector3(castRadius, 0f, castRadius), transform.forward, Quaternion.identity, dist, LayerMask.GetMask("Wall", "Tank", "River"));
			List<RaycastHit> hits = new List<RaycastHit>(hitArray);
			hits.Sort((x, y) => x.distance.CompareTo(y.distance));
			//if (Physics.BoxCast(transform.position - transform.forward * castRadius, new Vector3(castRadius, castRadius, castRadius), transform.forward, out RaycastHit hitInfo, Quaternion.identity, dist + castRadius, LayerMask.GetMask("Wall", "Tank", "River")))
			foreach (RaycastHit hitInfo in hits)
			{
				if (hitInfo.transform.GetComponent<Tank>() == this) continue;
				dist = hitInfo.distance * 0;
				if (!isMine) controller.OnBotObstacle();
				break;
			}*/
			//Debug.DrawRay(transform.position - transform.forward * castOffset, transform.forward * (dist + castRadius + castOffset), Color.red, 0.1f, false);
			//if (isMine) print(transform.TransformVector(new Vector3(castRadius, 0.1f, 0.001f)));
			if (Physics.BoxCast(transform.position - transform.forward * castOffset, castBoxHalfExtents, transform.forward, out RaycastHit hitInfo, Quaternion.identity, dist + castRadius + castOffset, LayerMask.GetMask("Wall", "Tank", "River")))
			{
				dist = hitInfo.distance * 0f - 0.001f; // ����һ����룬��ֹ��ǽ
				if (!isMine) controller.OnBotObstacle();// else print(hitInfo.transform);
				speed = 0f;
			}
			transform.Translate(Vector3.forward * dist);
		}
		
		/*
		// ��β����
		foreach (ParticleSystem dustParticle in dustParticles)
		{
			if (controller.moving && !dustParticle.isEmitting) dustParticle.Play();
			if (!controller.moving) dustParticle.Stop();
		}
		*/
	}

	float simElapsedTime = 0f;

	private void Update()
	{
		if (LevelManager.Instance.paused) return;

		while (simElapsedTime >= Simulation.timestep)
		{
			Sim();

			simElapsedTime -= Simulation.timestep;
		}
		simElapsedTime += Time.deltaTime;
	}
}
