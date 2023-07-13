using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
	public GameObject hitParticlePrefab = null;
	[SerializeField] GameObject destroyParticlePrefab = null;

	public float initHp = 80f;
	public bool destroyable = true;
	[System.NonSerialized] public float hp = 0f;
	public bool isBase = false;
	public float botBreakingDesireBonus = 0f; // bot想摧毁该墙体的意愿加成

	private void Awake()
	{
		hp = initHp;
	}

	public bool ReceiveDamage(float damage) // 如果被摧毁，就返回true，否则返回false
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
