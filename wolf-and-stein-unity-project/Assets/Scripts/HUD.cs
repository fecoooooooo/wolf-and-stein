using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviourSingleton<HUD>
{
    TextMeshProUGUI LevelLabel;
    TextMeshProUGUI ScoreLabel;
    TextMeshProUGUI LivesLabel;
    TextMeshProUGUI HealthLabel;
    TextMeshProUGUI AmmoLabel;
    TextMeshProUGUI KeysLabel;
    TextMeshProUGUI NotesLabel;
    
    Image WeaponImg;
    
    Sprite pawWeaponImg;
    Sprite pistolWeaponImg;
    Sprite machineGunWeaponImg;
    Sprite chainGunWeaponImg;

    void Start()
    {
        LevelLabel  = transform.Find("LevelLabel").GetComponent<TextMeshProUGUI>();
        ScoreLabel  = transform.Find("ScoreLabel").GetComponent<TextMeshProUGUI>();
        LivesLabel  = transform.Find("LivesLabel").GetComponent<TextMeshProUGUI>();
        HealthLabel = transform.Find("HealthLabel").GetComponent<TextMeshProUGUI>();
        AmmoLabel   = transform.Find("AmmoLabel").GetComponent<TextMeshProUGUI>();
        KeysLabel   = transform.Find("KeysLabel").GetComponent<TextMeshProUGUI>();
        NotesLabel  = transform.Find("NotesLabel").GetComponent<TextMeshProUGUI>();
        
        WeaponImg  = transform.Find("Weapon").GetComponent<Image>();

        pawWeaponImg = Resources.Load<Sprite>("inventory_paw");
        pistolWeaponImg = Resources.Load<Sprite>("inventory_pistol");
        machineGunWeaponImg = Resources.Load<Sprite>("inventory_rifle");
        chainGunWeaponImg = Resources.Load<Sprite>("inventory_chaingun");
        
        Character.instance.ShouldUpdateUI += OnUpdateUI;
        Character.instance.WeaponChanged += OnWeaponChanged;
    }

    private void OnUpdateUI(object sender, EventArgs e)
	{
        LevelLabel.text = Map.instance.CurrentLevel.ToString();
        ScoreLabel.text = Character.instance.Score.ToString();
        LivesLabel.text = Character.instance.Lives.ToString();
        HealthLabel.text = Character.instance.HP.ToString() + "%";
        AmmoLabel.text = Character.instance.Ammo.ToString();
        KeysLabel.text = Character.instance.Keys.ToString();
        NotesLabel.text = Collectibles.instance.FoundEntriesCount.ToString();
	}

    private void OnWeaponChanged(object sender, EventArgs e)
    {
        switch (Character.instance.CurrentWeapon)
        {
            case WeaponType.Paw:
                WeaponImg.sprite = pawWeaponImg;
                break;
            case WeaponType.Pistol:
                WeaponImg.sprite = pistolWeaponImg;
                break;
            case WeaponType.MachineGun:
                WeaponImg.sprite = machineGunWeaponImg;
                break;
            case WeaponType.ChainGun:
                WeaponImg.sprite = chainGunWeaponImg;
                break;
            default:
                throw new Exception("No Such weapontype exists");
        }
    }
}
