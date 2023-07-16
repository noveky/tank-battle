using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Wall : MonoBehaviour
{
	public GameObject hitParticlePrefab = null;
	[SerializeField] GameObject destroyParticlePrefab = null;

	public float initHp = 80f;
	public bool destroyable = true;
	[System.NonSerialized] public float hp = 0f;
	public bool isBase = false;
	public float botBreakingDesireBonus = 0f; // bot��ݻٸ�ǽ�����Ը�ӳ�
	public bool fixRotation = false; // �����ڷ�ʱδͳһ�ĳ���

	private void Awake()
	{
		hp = initHp;
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) FixRotation();
#endif
	}

	private void FixRotation()
	{
		if (fixRotation)
		{
			transform.rotation = Quaternion.identity;
		}
	}

	public bool ReceiveDamage(float damage) // ������ݻ٣��ͷ���true�����򷵻�false
	{
		if (!destroyable) return false;

		hp -= damage;
		if (hp <= 0f)
		{
			hp = 0f;

			if (destroyParticlePrefab != null)
			{
				Instantiate(destroyParticlePrefab, transform.position, transform.rotation);
			}

			Destroy(gameObject);

			return true;
		}
		return false;
	}
}
