using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SpringAnim
{
	public Vector3 v = Vector3.zero;
	public Vector3 r = Vector3.zero;

	public float k = 0f;
	public float d = 0f;

	public SpringAnim(float k, float d)
	{
		this.k = k;
		this.d = d;
	}

	public void Update()
	{
		v -= k * r * Simulation.timestep;//Time.deltaTime;
		r += v * Simulation.timestep;//Time.deltaTime;
		Simulation.AutoDamp(ref v, d);
	}

	public Vector3 GetNewR()
	{
		Update();
		return r;
	}

	public void AddImpulse(Vector3 impulse)
	{
		v += impulse;
	}

	public void AddImpulse(float x, float y, float z)
	{
		AddImpulse(new Vector3(x, y, z));
	}
}

class Simulation
{
	public const float timestep = 0.004f;

	static float GetDeltaTime() => Time.deltaTime;

	public static float GetDampMultiplier(float _factor, int _stFPS = 0)
	{
		if (_stFPS == 0) return _factor;
		return Mathf.Pow(_factor, GetDeltaTime() * _stFPS);
	}

	public static void AutoDamp(ref Vector3 _vector3, float _factor, int _stFPS = 0)
	{
		_vector3 *= GetDampMultiplier(_factor, _stFPS);
	}

	public static void AutoDamp(ref float _float, float _factor, int _stFPS = 0)
	{
		_float *= GetDampMultiplier(_factor, _stFPS);
	}

	public static void AutoTarget(ref float _float, float _target, float _factor, int _stFPS = 0)
	{
		if (_float == _target) return;
		float clampFactor = (_stFPS == 0 ? 1f : GetDeltaTime() * _stFPS) * 0.6f;
		_float += Mathf.Clamp((_target - _float) * (1f - GetDampMultiplier(1f - _factor, _stFPS)), -_factor * clampFactor, _factor * clampFactor);
	}

}