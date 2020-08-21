using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Pickable
{
    public override void OnPickUp()
    {
        Character.instance.AddWeapon(WeaponType.MachineGun);
    }

}
