using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealthBar : MonoBehaviour
{
	[SerializeField] Base @base = null;

	[SerializeField] RectTransform rectTrans = null;
	[SerializeField] Slider outerSlider = null;
	[SerializeField] Slider innerSlider = null;

	float HpRatio => @base.hp / @base.initHp;
	float visualRatio = 1f;

	SteadySimulation.TimestepActions timestepActions;

	private void Awake()
	{
		OnGUI();
		transform.GetChild(0).gameObject.SetActive(false);

		timestepActions = new SteadySimulation.TimestepActions(
			() => SteadySimulation.Target(ref visualRatio, HpRatio, 0.05f)
		);
	}

	private void OnGUI()
	{
		//if (LevelManager.Instance.paused) return;

		transform.GetChild(0).gameObject.SetActive(@base.hp > 0f && @base.hp < @base.initHp);

		rectTrans.position = Camera.main.WorldToScreenPoint(@base.transform.position) + Vector3.up * 100;

		outerSlider.value = Mathf.Clamp01(visualRatio);
		innerSlider.value = Mathf.Clamp01(visualRatio == 0f ? 1f : HpRatio / visualRatio);
	}

	private void Update()
	{
		timestepActions.Update();
	}
}
