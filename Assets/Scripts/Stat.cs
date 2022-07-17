using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    private string _id;

    [SerializeField]
    private string _name;

    [SerializeField]
    private float _default;

    public string Name
        => _name;

    public float Default
        => _default;

    public float Value { get; set; }

    public static void ProcessStats()
    {
        var stats = CharacterControl.Instance.Stats;

        var hp = stats.FirstOrDefault(s => s._id.Equals("hp")).Value;
        CharacterControl.Instance.Damagable.MaxHealth = hp;
        CharacterControl.Instance.Damagable.SetHealth(hp);

        var speed = stats.FirstOrDefault(s => s._id.Equals("speed")).Value;
        CharacterControl.Instance.SetMaxSpeed(CharacterControl.Instance.OriginalMaxSpeed * speed * 0.1f);

        var attackDamage = stats.FirstOrDefault(s => s._id.Equals("attack damage")).Value;
        foreach (var gun in CharacterControl.Instance.Guns)
        {
            gun.Damage = attackDamage * 0.1f * gun.OriginalDamage;
        }

        var rpm = stats.FirstOrDefault(s => s._id.Equals("attack speed")).Value;
        foreach (var gun in CharacterControl.Instance.Guns)
        {
            gun.RPM = rpm * 0.1f * gun.OriginalRPM;
        }

        var pd = stats.FirstOrDefault(s => s._id.Equals("pd")).Value;
        if (pd > 0)
        {
            var pdGun = CharacterControl.Instance.Guns.Where(g => g is DiceGunAuto).Cast<DiceGunAuto>().FirstOrDefault();
            if (pdGun == null)
            {
                pdGun = GameObject.Instantiate(CharacterControl.Instance.PDGunPrefab, CharacterControl.Instance.GunsObject.transform).GetComponent<DiceGunAuto>();
                CharacterControl.Instance.Guns.Add(pdGun);
            }

            pdGun.Damage = 1f;
            pdGun.RPM = 240f * pd * 0.1f;
        }
        else
        {
            var toRemove = new List<DiceGun>();
            foreach (var gun in CharacterControl.Instance.Guns.Where(g => g is DiceGunAuto).Cast<DiceGunAuto>())
            {
                toRemove.Add(gun);
            }

            foreach (var gun in toRemove)
            {
                CharacterControl.Instance.Guns.Remove(gun);
                GameObject.Destroy(gun);
            }
        }
    }
}
