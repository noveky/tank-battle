using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotTarget : MonoBehaviour
{
	public float importance = 0f; // Խ��Խ������bot������
	public float range = 10f; // ���䷶Χ

	private void Awake()
	{
		//transform.GetChild(0).gameObject.SetActive(false);
	}
}
