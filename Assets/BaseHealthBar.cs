using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealthBar : MonoBehaviour
{
	[SerializeField] Base @base = null;

	[SerializeField] RectTransform rectTrans = null;
	[SerializeField] Slider slider = null;

	private void Awake()
	{
		OnGUI();
		transform.GetChild(0).gameObject.SetActive(true);
	}

	private void OnGUI()
	{
		//if (LevelManager.Instance.paused) return;

		if (@base.hp == 0f) transform.GetChild(0).gameObject.SetActive(false);

		rectTrans.position = Camera.main.WorldToScreenPoint(@base.transform.position) + Vector3.up * 100;
		slider.value = @base.hp / @base.initHp;
	}
}
