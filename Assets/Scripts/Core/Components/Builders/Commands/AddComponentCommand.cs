using UnityEngine;
using Guinea.Core.Inventory;
using Zenject;

namespace Guinea.Core.Components
{
    public class AddComponentCommand : ICommand
    {
        ItemType m_itemType;
        Vector3 m_pos;
        Quaternion m_rot;
        Transform m_createdObj;
        InventoryLoader m_inventoryLoader;
        Inventory.Inventory m_inventory;


        public AddComponentCommand(ItemType itemType, Transform createdObj, InventoryLoader inventoryLoader, Inventory.Inventory inventory)
        {
            m_itemType = itemType;
            m_createdObj = createdObj;
            m_pos = createdObj.position;
            m_rot = createdObj.rotation;
            m_inventoryLoader = inventoryLoader;
            m_inventory = inventory;
            Commons.Logger.Assert(m_inventoryLoader != null, "InventoryLoader couldn't be null");
            Commons.Logger.Assert(m_inventory != null, "Inventory couldn't be null");
        }

        public void Execute()
        {
            Transform instance = m_inventoryLoader.Items[m_itemType].obj.transform;
            GameObject.Instantiate(instance, m_pos, m_rot);
            m_inventory.Use(m_itemType);
        }

        public void Undo()
        {
            GameObject.Destroy(m_createdObj);
            m_inventory.Add(m_itemType);
        }

        public class Factory : PlaceholderFactory<ItemType, Transform, AddComponentCommand> { }
    }
}