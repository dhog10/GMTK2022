using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceGunAuto : DiceGun
{
    public override bool AutoFire
        => true;

    protected override bool FireInputPressed()
    {
        return true;
    }
}
