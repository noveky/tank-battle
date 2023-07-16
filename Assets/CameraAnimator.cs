using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CameraAnimator : Singleton<CameraAnimator>
{
	Vector3 animPos = Vector3.zero;
	Vector3 animAng = Vector3.zero;

	SpringAnim hitAngAnim = new SpringAnim(4600f, 0.93f),
		hitPosAnim = new SpringAnim(2800f, 0.91f),
		fireAngAnim = new SpringAnim(1400f, 0.94f),
		firePosAnim = new SpringAnim(900f, 0.9f);

	void Animate()
	{
		animPos = animAng = Vector3.zero;

		animAng += hitAngAnim.GetNewR();
		animPos += hitPosAnim.GetNewR();
		animAng += fireAngAnim.GetNewR();
		animPos += firePosAnim.GetNewR();

		transform.localPosition = animPos;
		transform.localEulerAngles = animAng;
	}

	float simElapsedTime = 0f;

	private void Update()
	{
		if (LevelManager.Instance.paused) return;

		while (simElapsedTime >= Simulation.timestep)
		{
			Animate();

			simElapsedTime -= Simulation.timestep;
		}
		simElapsedTime += Time.deltaTime;
	}

	public void PlayHitAnim(float power)
	{
		hitAngAnim.AddImpulse(0f, 0f, (Random.Range(0, 2) == 1 ? 1 : -1) * power * 1f);
		hitPosAnim.AddImpulse(0f, 0f, power * -0.032f);
	}

	public void PlayFireAnim()
	{
		fireAngAnim.AddImpulse(0f, 0f, (Random.Range(0, 2) == 1 ? 1 : -1) * Random.Range(0.7f, 1f) * 8f);
		firePosAnim.AddImpulse(0f, 0f, -1f);
	}
}
