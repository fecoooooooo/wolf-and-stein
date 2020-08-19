using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameHandler : MonoBehaviourSingleton<SaveGameHandler>
{
	private void Start()
	{
        LoadData();
	}

	public void LoadData()
	{
        Map.instance.SetLevel(PlayerPrefs.GetInt("Level", 1));
        int Score = PlayerPrefs.GetInt("Score", 0);
        int Lives = PlayerPrefs.GetInt("Lives", 3);
        int HP = PlayerPrefs.GetInt("HP", 100);
        int Ammo = PlayerPrefs.GetInt("Ammo", 10);
        int Keys = PlayerPrefs.GetInt("Keys", 0);
        int Notes = PlayerPrefs.GetInt("Notes", 0);
        bool HasMachinGun = PlayerPrefs.GetInt("HasMachinGun", 0) == 0 ? false : true;
        bool HasChainGun = PlayerPrefs.GetInt("HasChainGun", 0) == 0 ? false : true;

        Character.instance.SetValues(Score, Lives, HP, Ammo, Keys, Notes, HasMachinGun, HasChainGun);
	}

    public void Save()
    {
        PlayerPrefs.SetInt("Level", Map.instance.CurrentLevel);
        PlayerPrefs.SetInt("Score", Character.instance.Score);
        PlayerPrefs.SetInt("Lives", Character.instance.Lives);
        PlayerPrefs.SetInt("HP", Character.instance.HP);
        PlayerPrefs.SetInt("Ammo", Character.instance.Ammo);
        PlayerPrefs.SetInt("Keys", Character.instance.Keys);
        PlayerPrefs.SetInt("Notes", Character.instance.Notes);
        PlayerPrefs.SetInt("HasMachinGun", Character.instance.HasMachinGun ? 1 : 0);
        PlayerPrefs.SetInt("HasChainGun", Character.instance.HasChainGun ? 1 : 0);
        PlayerPrefs.Save();
    }
}
