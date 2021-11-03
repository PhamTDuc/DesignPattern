using UnityEngine;

namespace Guinea.Core.Components
{
    [CreateAssetMenu(fileName = "Ammunition", menuName = "Components/Ammunition")]
    public class Ammunition : ScriptableObject
    {
        public int maxDamage;
        public float maxRange;

        public int maxAmmo;
        public float fireRate;
        public bool allowHolding;
        public float reloadTime;
    }
}