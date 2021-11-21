using UnityEngine;
using Zenject;

namespace Guinea.Core.Components
{
    public class AddOperator : IOperator
    {
        private static readonly Option[] options = { Option.UNDO };
        public override Option[] Options => options;
        private static Inventory.Inventory s_inventory;
        private static Inventory.InventoryLoader s_inventoryLoader;

        [OpProperty]
        Inventory.ItemType itemType;
        [OpProperty]
        Vector3 position;
        // [OpProperty]
        Quaternion rotation;
        Transform m_instantiated;

        public static void Initialize(Inventory.Inventory inventory, Inventory.InventoryLoader inventoryLoader)
        {
            s_inventory = inventory;
            s_inventoryLoader = inventoryLoader;
        }

        public override Result Execute(Context context)
        {
            Transform instance = s_inventoryLoader.Items[itemType].obj.transform;
            rotation = instance.rotation;
            m_instantiated = Instantiate(instance, position, rotation);
            Debug.Log($"Execute: {m_instantiated}");
            s_inventory.Use(itemType);
            context.Select(m_instantiated);
            OperatorManager.Execute<MoveOperator>(Exec.INVOKE);
            return IOperator.Result.FINISHED;
        }

        public override Result Cancel(Context context)
        {
            Commons.Logger.Assert(m_instantiated != null, "m_instantiated Could not be null");
            Debug.Log($"Cancel: {m_instantiated}");
            Destroy(m_instantiated.gameObject);
            s_inventory.Add(itemType);
            return IOperator.Result.CANCELLED;
        }
    }
}