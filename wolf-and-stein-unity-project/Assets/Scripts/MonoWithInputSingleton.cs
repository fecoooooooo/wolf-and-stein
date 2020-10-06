using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoWithInputSingleton<T> : MonoWithInput where T : MonoWithInputSingleton<T>
{
	public static T instance;

	protected void Awake()
	{
		if (instance != null)
		{
			Debug.LogError($"Multiple instances of singleton class: {typeof(T)}\n");
			Debug.Break();
		}
		instance = (T)this;

		OnAwake();
	}

	protected virtual void OnAwake()
	{ }

	protected void OnDestroy()
	{
		instance = null;
	}
}