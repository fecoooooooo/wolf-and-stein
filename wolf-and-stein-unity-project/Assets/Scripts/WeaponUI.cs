using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    Animator animator;
    float Speed = 0;
	float currentLoopStart;

    void Start()
    {
        animator = GetComponent<Animator>();
        Character.instance.WeaponChanged += OnWeaponChanged;
        Character.instance.ShootWeapon += OnShootWeapon;
    }

	public void OnReachLoopStart()
	{
		currentLoopStart = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
	}

	public void OnReachLoopEnd()
	{
		if (Character.instance.Shooting)
			animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, currentLoopStart);
	}


    public void OnReachAnimEnd()
	{
		if (Character.instance.Shooting)
			return;

        Speed = 0;
        animator.SetFloat("Speed", Speed);
    }


    private void OnWeaponChanged(object sender, EventArgs e)
	{
		switch (Character.instance.CurrentWeapon)
		{
			case WeaponType.Paw:
				animator.SetTrigger("Paw");
				break;
			case WeaponType.Pistol:
				animator.SetTrigger("Pistol");
				break;
			case WeaponType.MachineGun:
				animator.SetTrigger("MachineGun");
				break;
			case WeaponType.ChainGun:
				animator.SetTrigger("ChainGun");
				break;
			default:
				throw new Exception("No Such weapontype exists");
		}
	}

    private void OnShootWeapon(object sender, EventArgs e)
	{
		Speed = 1;
		animator.SetFloat("Speed", Speed);
	}

}
