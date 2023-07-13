using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealthBar : MonoBehaviour
{
	[SerializeField] Tank tank = null;

	[SerializeField] RectTransform rectTrans = null;
	[SerializeField] Slider slider = null;

	[SerializeField] bool isMine = false;

	private void Awake()
	{
		gameObject.SetActive(!(isMine ^ tank.isMine));
		OnGUI();
		transform.GetChild(0).gameObject.SetActive(false);
	}

	private void OnGUI()
	{
		//if (LevelManager.Instance.paused) return;

		bool visible = true;
		if (!isMine)
		{
			Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(tank.transform.position + Vector3.up * 0.3f)), out RaycastHit hit, float.PositiveInfinity, LayerMask.GetMask("Wall", "Grass", "Tank"));
			visible = hit.transform.gameObject.layer == LayerMask.NameToLayer("Tank");
		}
		transform.GetChild(0).gameObject.SetActive(visible);

		rectTrans.position = Camera.main.WorldToScreenPoint(tank.transform.position) + Vector3.up * 60;
		slider.value = tank.hp / tank.initHp;
	}
}
