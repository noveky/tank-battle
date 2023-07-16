using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
	Wall wall = null;
	Transform boxTrans = null;
	Transform contentTrans = null;

	public bool destroyed { get => wall == null; }

	public float hp
	{
		get
		{
			if (wall == null) return 0f;
			return wall.hp;
		}
	}

	public float initHp
	{
		get
		{
			if (wall == null) return 1f;
			return wall.initHp;
		}
	}

	private void Awake()
	{
		boxTrans = transform.GetChild(0);
		wall = boxTrans.GetComponent<Wall>();
		contentTrans = transform.GetChild(1).GetChild(0);
	}

	private void Update()
	{
		if (LevelManager.Instance.paused) return;

		float y = wall.hp / wall.initHp;
		contentTrans.localPosition = new Vector3(0f, y * 0.5f - 0.5f, 0f);
		contentTrans.localScale = (y <= 0f ? Vector3.zero : new Vector3(1f, y, 1f));
	}
}
