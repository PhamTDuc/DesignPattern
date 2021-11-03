using UnityEngine;

public interface IDestructible
{
    void Heal(int value);
    void GetDamage(int damage, RaycastHit hit);
}