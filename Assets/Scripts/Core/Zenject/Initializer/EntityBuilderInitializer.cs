using UnityEngine;
using Zenject;

namespace Guinea.Core
{
    public class EntityBuilderInitializer : MonoBehaviour
    {
        Inventory.Inventory m_inventory;
        [Inject]
        void Initialize(Inventory.Inventory inventory)
        {
            m_inventory = inventory;
        }

        void Start()
        {
            m_inventory.OnIsUpdated();
        }
    }
}