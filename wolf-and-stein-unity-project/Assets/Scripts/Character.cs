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
    
    public EventHandler ShouldUpdateUI;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveVector = Input.GetAxisRaw("Vertical");
        turnVector = Input.GetAxisRaw("Horizontal");
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
