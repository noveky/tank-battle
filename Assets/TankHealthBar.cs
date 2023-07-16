using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealthBar : MonoBehaviour
{
	[SerializeField] Tank tank = null;

	[SerializeField] RectTransform rectTrans = null;
	[SerializeField] Slider outerSlider = null;
	[SerializeField] Slider innerSlider = null;

	[SerializeField] bool isMine = false;

	float HpRatio => tank.hp / tank.initHp;
	float visualRatio = 1f;

	SteadySimulation.TimestepActions timestepActions;

	private void Awake()
	{
		gameObject.SetActive(!(isMine ^ tank.isMine));
		OnGUI();
		transform.GetChild(0).gameObject.SetActive(false);

		timestepActions = new SteadySimulation.TimestepActions(
			() => SteadySimulation.Target(ref visualRatio, HpRatio, 0.05f)
		);
	}

	private void OnGUI()
	{
		//if (LevelManager.Instance.paused) return;

		bool visible = true;
		if (!isMine)
		{
			if (tank.hp >= tank.initHp)
			{
				visible = false;
			}
			else
			{
				Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(tank.transform.position + Vector3.up * 0.3f)), out RaycastHit hit, float.PositiveInfinity, LayerMask.GetMask("Wall", "Grass", "Tank"));
				visible = hit.transform.gameObject.layer == LayerMask.NameToLayer("Tank");
			}
		}
		transform.GetChild(0).gameObject.SetActive(visible);

		rectTrans.position = Camera.main.WorldToScreenPoint(tank.transform.position) + Vector3.up * 60;

		outerSlider.value = Mathf.Clamp01(visualRatio);
		innerSlider.value = Mathf.Clamp01(visualRatio == 0f ? 1f : HpRatio / visualRatio);
	}

	private void Update()
	{
		timestepActions.Update();
	}
}
