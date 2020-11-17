using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : Pickable
{
    public override void OnPickUp()
    {
        Collectibles.instance.AddCollectible();
    }

}
