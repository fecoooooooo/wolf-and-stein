using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviourSingleton<Character>
{
    public const int MAX_HP = 100;
    public const int MAX_AMMO = 99;
    public const float MAX_SHOOT_ANGLE = 30;

    float TURN_SPEED = 3f;
    const float FORWARD_SPEED = 5f;
    const float SIDE_SPEED = 5f;
    
    const float PAW_RANGE = 1.3f;

    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int HP { get; private set; }
    public int Ammo { get; private set; }
    public int Keys { get; private set; }
    public int Notes { get; private set; }
    public bool HasMachinGun { get; private set; }
    public bool HasChainGun { get; private set; }
    public bool Shooting { get; private set; }

    new Rigidbody rigidbody;

    
    float forwardImput = 0;
    float turnInput = 0;
    float sideInput = 0;

    public HPState HPState
	{
		get
		{
            if (90 <= HP)
                return HPState.Healty;
            else if (75 <= HP && HP <= 89)
                return HPState.Hurt1;
            else if (60 <= HP && HP <= 74)
                return HPState.Hurt2;
            else if (45 <= HP && HP <= 59)
                return HPState.Hurt3;
            else if (30 <= HP && HP <= 44)
                return HPState.Hurt4;
            else if (15 <= HP && HP <= 29)
                return HPState.Hurt5;
            else if (1 <= HP && HP <= 14)
                return HPState.Hurt6;
            else
                return HPState.Dead;
        }
	}


    public WeaponType CurrentWeapon { get; private set; } = WeaponType.Pistol;
    public float Damage 
    {
        get => WEAPON_DAMAGES[(int)CurrentWeapon];
    }
	public bool HasAutomaticWeapon { get => CurrentWeapon == WeaponType.MachineGun || CurrentWeapon == WeaponType.ChainGun; }

	readonly float[] WEAPON_DAMAGES = { 30f, 70f, 25f, 35f };
	public EventHandler ShouldUpdateUI;
	public EventHandler HPChanged;
    public EventHandler WeaponChanged;
    public EventHandler ShootWeapon;

    private void Start()
    {
        HP = MAX_HP;
        rigidbody = GetComponent<Rigidbody>();
        WeaponUI.instance.WeaponFired += OnWeaponFired;
    }

    void Update()
    {
        forwardImput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
        
        sideInput = 0;
        if (Input.GetKey(KeyCode.Q))
            sideInput = -1;
        else if (Input.GetKey(KeyCode.E))
            sideInput = 1;

        if (Input.GetKeyDown(KeyCode.Alpha8))
            TakeDamage(7);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipWeapon(WeaponType.Paw);
        else if (Input.GetKeyDown(KeyCode.Alpha2) && Ammo > 0)
            EquipWeapon(WeaponType.Pistol);
        else if (Input.GetKeyDown(KeyCode.Alpha3) && Ammo > 0/*&& HasMachinGun*/)
            EquipWeapon(WeaponType.MachineGun);
        else if (Input.GetKeyDown(KeyCode.Alpha4) && Ammo > 0/*&& HasChainGun*/)
            EquipWeapon(WeaponType.ChainGun);
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Shooting = true;
            Shoot();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
            Shooting = false;
    }

	private void Shoot()
	{
        ShootWeapon?.Invoke(this, null);
	}

    void OnWeaponFired(object sender, EventArgs e)
	{
        DamageEnemies();
    }

    private void DamageEnemies()
	{
        foreach (Enemy en in Map.instance.GetEnemies())
        {
            Vector3 charToEnemy = (en.transform.position - transform.position).normalized;
            var currentAngle = Vector3.Angle(transform.forward, charToEnemy);

            if (MAX_SHOOT_ANGLE < currentAngle)
                continue;

            int layerMask = 1 << LayerMask.NameToLayer("Blockers") | 1 << LayerMask.NameToLayer("Wall");


            bool enemyHit = false;
            foreach(Vector3 targetPos in en.GetTargetPositions())
			{
                /*var a = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                a.transform.position = targetPos;
                a.transform.localScale = new Vector3(.01f, .01f, .01f);*/

                if (CurrentWeapon == WeaponType.Paw && PAW_RANGE < Vector3.Distance(transform.position, targetPos))
                    continue;

                if (false == Physics.Linecast(transform.position, targetPos, layerMask, QueryTriggerInteraction.Ignore))
				{
                    enemyHit = true;
                    break;
				}
			}

            if(enemyHit)
                en.TakeDamage(Damage);
        }
    }

	private void EquipWeapon(WeaponType weapon)
	{
        CurrentWeapon = weapon;

        WeaponChanged?.Invoke(this, null);
	}

    public void TakeDamage(int damage)
	{
        HP -= damage;
        HP = HP < 0 ? 0 : HP;

        HPChanged?.Invoke(this, null);
        ShouldUpdateUI?.Invoke(this, null);
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
        HPChanged?.Invoke(this, null);
    }

	private void FixedUpdate()
    {
        Vector3 forwardAmount = transform.forward * forwardImput * FORWARD_SPEED;
        Vector3 sideAmount = transform.right * sideInput * SIDE_SPEED;

        rigidbody.velocity = forwardAmount + sideAmount;

        rigidbody.angularVelocity = transform.up * turnInput * TURN_SPEED;
    }

    public void AddKey(int amountToAdd)
    {
        Keys += amountToAdd;
        ShouldUpdateUI?.Invoke(this, null);
    }

    internal void AddNote(int amountToAdd)
    {
        Notes += amountToAdd;
        ShouldUpdateUI?.Invoke(this, null);
    }

    internal void AddAmmo(int amountToAdd)
    {
        Ammo += amountToAdd;
        Ammo = Ammo > MAX_AMMO ? MAX_AMMO : Ammo;
        ShouldUpdateUI?.Invoke(this, null);
    }  
    
    internal void AddScore(int amountToAdd)
    {
        Score += amountToAdd;
        ShouldUpdateUI?.Invoke(this, null);
    }  
    
    internal void AddHP(int amountToAdd)
    {
        HP += amountToAdd;
        HP = HP > MAX_HP ? MAX_HP : HP;
        ShouldUpdateUI?.Invoke(this, null);
    }

    internal void AddWeapon(WeaponType weaponType)
    {
        bool hadWeapon = true;

		switch (weaponType)
		{
			case WeaponType.MachineGun:
                if (!HasMachinGun)
                    hadWeapon = false;

                HasMachinGun = true;
				break;
			case WeaponType.ChainGun:
                if (!HasChainGun)
                    hadWeapon = false;

                HasChainGun = true;
				break;
			default:
				break;
		}

        if (!hadWeapon)
            EquipWeapon(weaponType);
    }
}

public enum WeaponType
{
    Paw,
    Pistol,
    MachineGun,
    ChainGun
}

public enum HPState
{
    Healty,
    Hurt1,
    Hurt2,
    Hurt3,
    Hurt4,
    Hurt5,
    Hurt6,
    Dead
}
