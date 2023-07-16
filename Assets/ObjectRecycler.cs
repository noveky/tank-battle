using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRecycler : MonoBehaviour
{
	[SerializeField] float lifetime = 1f;

	float life = 0f;

	private void Awake()
	{
		life = lifetime;
	}

	private void Update()
	{
		if (LevelManager.Instance.paused) return;

		life -= Time.deltaTime;
		if (life <= 0f)
		{
			Destroy(gameObject);
		}
	}
}
