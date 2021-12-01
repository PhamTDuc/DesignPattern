using UnityEngine;
using Zenject;

namespace Guinea.Core.Components
{
    public class AddOperator : IOperator
    {
        private static readonly Option[] options = { Option.UNDO };
        public override Option[] Options => options;
        private static SharedInteraction s_sharedInteraction;
        private static Inventory.Inventory s_inventory;
        private static Inventory.InventoryLoader s_inventoryLoader;

        [OpProperty]
        Inventory.ItemType itemType;
        [OpProperty]
        Vector3 position;
        // [OpProperty]
        Quaternion rotation;
        Transform m_instantiated;

        [Inject]
        void Initialize(SharedInteraction sharedInteraction, Inventory.Inventory inventory, Inventory.InventoryLoader inventoryLoader)
        {
            Commons.Logger.Assert(s_sharedInteraction == null, "s_sharedInteraction is already initialized!");
            Commons.Logger.Assert(s_inventory == null, "s_inventory is already initialized!");
            Commons.Logger.Assert(s_inventoryLoader == null, "s_inventoryLoader is already initialized!");
            s_sharedInteraction = sharedInteraction;
            s_inventory = inventory;
            s_inventoryLoader = inventoryLoader;
        }

        // * CleanUp when and preventing Assertion Error when switching Scene
        void OnDestroy()
        {
            s_sharedInteraction = null;
            s_inventory = null;
            s_inventoryLoader = null;
        }

        public override Result Execute(Context context)
        {
            Transform instance = s_inventoryLoader.Items[itemType].obj.transform;
            rotation = instance.rotation;
            m_instantiated = Instantiate(instance, position, rotation, s_sharedInteraction.ComponentsContainer);
            s_inventory.Use(itemType);
            context.Select(m_instantiated);
            OperatorManager.Execute<MoveOperator>(Exec.INVOKE);
            return IOperator.Result.FINISHED;
        }

        public override Result Cancel(Context context)
        {
            Commons.Logger.Assert(m_instantiated != null, "m_instantiated Could not be null");
            Destroy(m_instantiated.gameObject);
            s_inventory.Add(itemType);
            return IOperator.Result.CANCELLED;
        }

        public void Redo()
        {

        }
    }
}