using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotTarget : MonoBehaviour
{
	public float importance = 0f; // 越高越会吸引bot来这里

	private void Awake()
	{
		//transform.GetChild(0).gameObject.SetActive(false);
	}
}
