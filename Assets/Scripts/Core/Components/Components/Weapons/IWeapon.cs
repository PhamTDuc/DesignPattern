using UnityEngine;

namespace Guinea.Core.Components
{
    public interface IWeapon
    {
        void Shoot(bool pressed);
        void Reload();
        void AimTo(Vector3 target);
    }
}