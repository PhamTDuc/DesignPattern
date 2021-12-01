using UnityEngine;

public interface IDestructible
{
    int MaxHealth { get; }
    int CurrentHealth { get; }
    void Heal(int value);
    void GetDamage(int damage, RaycastHit hit);
}