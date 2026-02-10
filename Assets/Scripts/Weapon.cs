using UnityEngine;

[System.Serializable]
public class Weapon
{
    public float damage = 1f;
    public float cooldown = 1f;

    public void Apply(float newDamage, float newCooldown)
    {
        damage = newDamage;
        cooldown = newCooldown;
        Debug.Log($"[Weapon] Applied: damage={damage}, cooldown={cooldown}");
    }
}
