using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GamePreferences", menuName = "ScriptableObjects/GamePreferences", order = 1)]
public class GamePreferencesSE : ScriptableObject
{
	public float DoorAutoCloseTime = 3f;

	public GameObject Wall1;
	public GameObject Wall2;
	public GameObject Wall1Block;
	public GameObject Wall2Block;
	public GameObject Poster1;
	public GameObject Poster2;
	public GameObject WoodColumn;
	public GameObject StoneColumn;
	public GameObject Door;
	public GameObject DoorFrame;
	public GameObject Lamp;
	public GameObject Food;
	public GameObject Ammo;
	public GameObject Key;
	public GameObject Note;
	public GameObject Treasure;
	public GameObject MachineGun;
	public GameObject ChainGun;
}
