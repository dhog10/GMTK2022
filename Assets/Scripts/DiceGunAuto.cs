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
        if (Object.FindObjectsOfType<Damagable>().Where(d => d.Team == Team.Enemy).Count() == 0)
        {
            return false;
        }

        return true;
    }
}
