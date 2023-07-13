using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAnimator : MonoBehaviour
{
	[System.NonSerialized] public Tank tank = null;

	public Transform animTrans = null;

	Vector3 animPos = Vector3.zero;
	Vector3 animAng = Vector3.zero;

	Vector3 turnAnimAng = Vector3.zero;
	SpringAnim firePosAnim = new SpringAnim(2400f, 0.88f),
		fireAngAnim = new SpringAnim(300f, 0.85f);

	private void Awake()
	{
		tank = GetComponent<Tank>();
	}

	float simElapsedTime = 0f;

	private void LateUpdate()
	{
		if (LevelManager.instance.paused) return;

		while (simElapsedTime >= Simulation.timestep)
		{
			Animate();

			simElapsedTime -= Simulation.timestep;
		}
		simElapsedTime += Time.deltaTime;
	}

	void Animate()
	{
		animPos = animAng =  Vector3.zero;

		animPos += firePosAnim.GetNewR();
		animAng += fireAngAnim.GetNewR();

		Simulation.AutoDamp(ref turnAnimAng, 0.92f);
		animAng += turnAnimAng;

		animTrans.localPosition = animPos;
		animTrans.localEulerAngles = animAng;
	}

	public void PlayFireAnim()
	{
		firePosAnim.AddImpulse(0f, 0f, -5f);
		fireAngAnim.AddImpulse(-240f, 0f, 0f);
	}

	public void PlayTurnAnim(int fromDirection, int toDirection)
	{
		int turn1 = (toDirection - fromDirection + 4) % 4;
		int turn2 = (toDirection - fromDirection - 4) % 4;
		float angle = (Mathf.Abs(turn1) < Mathf.Abs(turn2) ? turn1 : turn2) * 90f;
		turnAnimAng -= new Vector3(0f, angle, 0f);
	}
}
