using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GamePreferences", menuName = "ScriptableObjects/GamePreferences", order = 1)]
public class GamePreferencesSE : ScriptableObject
{
	public float DoorAutoCloseTime = 3f;
	public GameObject Wall1;
	public GameObject Wall2;
	public GameObject Door;
	public GameObject DoorFrame;
}
