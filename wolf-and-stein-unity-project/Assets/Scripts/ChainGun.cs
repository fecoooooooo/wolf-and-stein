using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainGun : Pickable
{
    public override void OnPickUp()
    {
        Character.instance.AddWeapon(WeaponType.ChainGun);
    }

}
