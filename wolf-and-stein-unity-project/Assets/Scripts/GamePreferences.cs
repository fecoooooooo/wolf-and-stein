using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GamePreferences:MonoBehaviour
{
	public GamePreferencesSE gamePreferences;
	static GamePreferencesSE instance;
	public static GamePreferencesSE Instance { get => instance; }

	private void OnValidate()
	{
		instance = gamePreferences;
	}
}
