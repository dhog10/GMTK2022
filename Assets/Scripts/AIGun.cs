using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGun : DiceGun
{
    public Enemy Enemy { get; set; }

    protected override bool FireInputPressed()
    {
        return this.Enemy?.IsInputFiring() ?? false;
    }
}
