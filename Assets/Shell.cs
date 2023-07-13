using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
	[System.NonSerialized] public Tank tank = null;

	[System.NonSerialized] public float speed = 0f;
	[System.NonSerialized] public float power = 0f;
	[System.NonSerialized] public float decelIndex = 0f; // ����Ч��ָ��
	const float castRadius = 0.02f; // �ϰ�����뾶
	const float wallCastWidth = 0.49f; // ��ǽ���
	const float shellClashCastRadius = 0.12f; // �ڵ���ײ���뾶

	//Vector3 v0 = Vector3.zero;

	//Rigidbody rb = null;

	private void Awake()
	{
		//rb = GetComponent<Rigidbody>();
	}
	private void Start()
	{
		//v0 = transform.forward * speed;
	}

	private void FixedUpdate()
	{
		//rb.velocity = v0;
	}

	float lastDist = 0f; // ��¼��һ֡���ƶ����룬Ŀ����ÿ���жϵ�ʱ�����һ֡���߶������غϣ���ֹ��������ƶ���Ŀ��
	private void Update()
	{
		if (LevelManager.instance.paused) return;

		float dist = speed * Time.deltaTime;
		bool dontDestroy = false;
		if (Physics.SphereCast(transform.position - transform.forward * lastDist, castRadius * 4f, transform.forward, out RaycastHit hitInfo, dist + lastDist, LayerMask.GetMask("Wall", "Tank")))
		{
			float animPower = power * 0.8f;

			if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
			{
				Vector3 castBoxHalfExtents = transform.TransformVector(new Vector3(wallCastWidth / 2f, 0f, 0f));
				castBoxHalfExtents.x = Mathf.Abs(castBoxHalfExtents.x);
				castBoxHalfExtents.z = Mathf.Abs(castBoxHalfExtents.z);

				// ����һ��List�������б����е�ǽ
				List<RaycastHit> hits = new List<RaycastHit>();

				Wall hitWall0 = hitInfo.transform.GetComponent<Wall>();
				// �ǻ������÷�Χ��ǽ�ж���������Χ��⣬���Ƿ�Ϊ����Ķ�
				if (hitWall0.isBase || Physics.BoxCast(hitInfo.point + transform.forward * 0.01f, castBoxHalfExtents, -transform.forward, Quaternion.identity, 0.02f, LayerMask.GetMask("Wall")))
				{
					// ֻ����ֱ�߻��е�ǽ
					hits.Add(hitInfo);
				}
				else
				{
					// ����Χ�ж�
					RaycastHit[] hitArray = Physics.BoxCastAll(hitInfo.point - transform.forward * 0.01f, castBoxHalfExtents, transform.forward, Quaternion.identity, 0.02f, LayerMask.GetMask("Wall"));
					hits.AddRange(hitArray);
				}

				foreach (RaycastHit hit in hits)
				{
					Wall hitWall = hit.transform.GetComponent<Wall>();
					if (hitWall != null && !(hitWall.isBase && hitWall.transform != hitInfo.transform))
					{
						if (hitWall.hitParticlePrefab != null)
						{
							Instantiate(hitWall.hitParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
						}

						bool destroyed = hitWall.ReceiveDamage(power);
						if (destroyed) animPower += 40f + hitWall.initHp * 0.2f;
					}
				}
			}
			else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Tank"))
			{
				Tank hitTank = hitInfo.transform.GetComponent<Tank>();
				bool dealDamage = (tank.isMine ^ hitTank.isMine) || (GameManager.instance.gameMode != GameManager.GameMode.COOP_MODE && tank.isMine && hitTank.isMine && tank.playerNum != hitTank.playerNum);
				if (dealDamage)
				{
					if (hitTank != null)
					{
						if (hitTank.hitParticlePrefab != null)
						{
							Instantiate(hitTank.hitParticlePrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
						}

						hitTank.GetDecelerationEffect(decelIndex);
						bool destroyed = hitTank.ReceiveDamage(power, tank);
						if (destroyed) animPower += 150f + hitTank.initHp * 0.3f;
					}
				}
				else
				{
					animPower = 0f;
					dontDestroy = true;
				}
			}

			CameraAnimator.instance.PlayHitAnim(animPower);

			if (!dontDestroy) Destroy(gameObject);
		}
		else if (Physics.SphereCast(transform.position, shellClashCastRadius, transform.forward, out RaycastHit hitInfo0, dist * 2.1f, LayerMask.GetMask("Shell")))
		{
			// �ڵ���ײ����
			CameraAnimator.instance.PlayHitAnim(power * 2f);

			Destroy(hitInfo0.transform.gameObject);
			Destroy(gameObject);
		}
		/*else
		{
			if (Physics.SphereCast(transform.position, castRadius, transform.forward, out RaycastHit hitInfo1, dist * 4f, LayerMask.GetMask("Shell")))
			{
				CameraAnimator.Instance.PlayHitAnim(power * 2f);

				Destroy(hitInfo1.transform.gameObject);
				Destroy(gameObject);
			}
		}*/

		transform.Translate(Vector3.forward * dist);
		lastDist = dist;
	}

	/*private void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision.gameObject);
		float animPower = power * 0.8f;
		if (collision.gameObject.layer == LayerMask.NameToLayer("Shell"))
		{
			animPower *= 2f;

			Destroy(collision.gameObject);
			Destroy(gameObject);
		}
		else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			Wall hitWall = collision.transform.GetComponent<Wall>();
			if (hitWall != null)
			{
				if (hitWall.hitParticlePrefab != null)
				{
					Instantiate(hitWall.hitParticlePrefab, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
				}

				bool destroyed = hitWall.ReceiveDamage(power);
				if (destroyed) animPower += 40f + hitWall.initHp * 0.2f;
			}
		}
		else if (collision.gameObject.layer == LayerMask.NameToLayer("Tank"))
		{
			Tank hitTank = collision.transform.GetComponent<Tank>();
			if (hitTank != null)
			{
				if (hitTank.isMine) return;
				if (hitTank.hitParticlePrefab != null)
				{
					Instantiate(hitTank.hitParticlePrefab, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
				}

				bool destroyed = hitTank.ReceiveDamage(power);
				if (destroyed) animPower += 150f + hitTank.initHp * 0.3f;
			}
		}

		CameraAnimator.Instance.PlayHitAnim(animPower);

		Destroy(gameObject);
	}*/
}
