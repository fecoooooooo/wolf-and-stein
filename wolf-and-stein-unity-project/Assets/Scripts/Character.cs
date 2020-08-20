using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourSingleton<Character>
{
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int HP { get; private set; }
    public int Ammo { get; private set; }
    public int Keys { get; private set; }
    public int Notes { get; private set; }
    public bool HasMachinGun { get; private set; }
    public bool HasChainGun { get; private set; }

    new Rigidbody rigidbody;

    float TurnSpeed = 3f;
    const float MoveSpeed = 5f;
    float moveVector = 0;
    float turnVector = 0;

    public WeaponType CurrentWeapon { get; private set; } = WeaponType.Pistol;
    
    public EventHandler ShouldUpdateUI;
    public EventHandler WeaponChanged;
    public EventHandler ShootWeapon;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveVector = Input.GetAxisRaw("Vertical");
        turnVector = Input.GetAxisRaw("Horizontal");


        if (Input.GetKeyDown(KeyCode.Alpha0))
            EquipWeapon(WeaponType.Paw);
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipWeapon(WeaponType.Pistol);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            EquipWeapon(WeaponType.MachineGun);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            EquipWeapon(WeaponType.ChainGun);
        else if (Input.GetKeyDown(KeyCode.Space))
            Shoot();
    }

	private void Shoot()
	{

        ShootWeapon?.Invoke(this, null);
	}

	private void EquipWeapon(WeaponType weapon)
	{
        CurrentWeapon = weapon;

        WeaponChanged?.Invoke(this, null);
	}

	internal void SetValues(int score, int lives, int hP, int ammo, int keys, int notes, bool hasMachinGun, bool hasChainGun)
	{
        this.Score = score;
        this.Lives = lives;
        this.HP = hP;
        this.Ammo = ammo;
        this.Keys = keys;
        this.Notes = notes;
        this.HasMachinGun = hasMachinGun;
        this.HasChainGun = hasChainGun;

        ShouldUpdateUI?.Invoke(this, null);
        WeaponChanged?.Invoke(this, null);
    }

	private void FixedUpdate()
    {
        rigidbody.velocity = transform.forward * moveVector * MoveSpeed;
        rigidbody.angularVelocity = transform.up * turnVector * TurnSpeed;
    }

    public void AddKey(int amountToadd)
    {
        Keys += amountToadd;
        ShouldUpdateUI?.Invoke(this, null);
    }

    internal void AddNote(int amountToadd)
    {
        Notes += amountToadd;
        ShouldUpdateUI?.Invoke(this, null);
    }

    internal void AddAmmo(int amountToadd)
    {
        Ammo += amountToadd;
        ShouldUpdateUI?.Invoke(this, null);
    }  
    
    internal void AddScore(int amountToadd)
    {
        Score += amountToadd;
        ShouldUpdateUI?.Invoke(this, null);
    }  
    
    internal void AddHP(int amountToadd)
    {
        HP += amountToadd;
        ShouldUpdateUI?.Invoke(this, null);
    }
}

public enum WeaponType
{
    Paw,
    Pistol,
    MachineGun,
    ChainGun
}
