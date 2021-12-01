using UnityEngine;
using Zenject;

namespace Guinea.Core.Test
{
    public class TestEntityBuilderInitializer : MonoBehaviour
    {

#if UNITY_EDITOR
        Inventory.Inventory m_inventory;
        [Inject]
        void Initialize(Inventory.Inventory inventory)
        {
            m_inventory = inventory;
        }

        void Awake()
        {
            string json_test = @"{
            ""WEAPON_00"": 6,
            ""WHEEL_00"": 4,
            ""FRAME_00"": 2
            }";
            m_inventory.LoadFromJson(json_test);
        }
#endif
    }
}