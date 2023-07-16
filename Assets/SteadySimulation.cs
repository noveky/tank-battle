using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SteadySimulation
{
	public const float timestep = 0.004f;

	public class SpringAnim
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

		public void SimTimestep()
		{
			v -= k * r * SteadySimulation.timestep;
			r += v * SteadySimulation.timestep;
			v *= d;
		}

		public Vector3 GetNewR()
		{
			SimTimestep();
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

	public class TimestepActions
	{
		public Action[] actions;

		float elapsedTime = 0f;

		public TimestepActions(params Action[] actions)
		{
			this.actions = actions;
		}
		
		public void Update()
		{
			while (elapsedTime >= SteadySimulation.timestep)
			{
				foreach (var action in actions) action();
				elapsedTime -= SteadySimulation.timestep;
			}
			elapsedTime += Time.deltaTime;
		}
	}

	public static void Target(ref float x, float target, float speed)
	{
		if (x == target) return;
		float clampFactor = 0.6f;
		x += Mathf.Clamp((target - x) * speed, -speed * clampFactor, speed * clampFactor);
	}

}