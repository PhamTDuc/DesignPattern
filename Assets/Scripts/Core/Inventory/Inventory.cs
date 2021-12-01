using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Guinea.Core.Inventory
{
    public class Inventory : MonoBehaviour
    {
        // [TextArea(4, 20)] [SerializeField] string m_json;
        Dictionary<ItemType, int> m_inventoryItems;

        string m_entityJson; // * Entity hierarchy for switch between EntityBuilder and Gameplay and vice versa
        public Dictionary<ItemType, int> Items => m_inventoryItems;
        public event Action<ItemType, int> OnUpdateItem;
        public event Action<Dictionary<ItemType, int>> OnLoadItems;
        private bool m_isUpdated = false;

        public string EntityJson => m_entityJson;
        public void SaveToEntityJson(string json)
        {
            m_entityJson = json;
            Commons.Logger.Log($"Inventory::SaveToEntityJson(): {json}");
        }

        public void OnIsUpdated()
        {
            if (m_isUpdated)
            {
                m_isUpdated = false;
                OnLoadItems?.Invoke(m_inventoryItems);
            }
        }

        public void LoadFromJson(string json)
        {
            m_inventoryItems = DataHandler.JsonHandler.Deserialize<Dictionary<ItemType, int>>(json);
            m_isUpdated = true;
            Commons.Logger.Log($"Inventory::LoadFromJson(): {json}");
        }

        public string SaveToJson()
        {
            return DataHandler.JsonHandler.SerializeObject(m_inventoryItems);
        }

        public bool Use(ItemType itemType)
        {
            if (m_inventoryItems.TryGetValue(itemType, out int value) && value > 0)
            {
                m_inventoryItems[itemType] = --value;
                OnUpdateItem(itemType, value);
                return true;
            }
            else
            {
                Debug.LogWarning($"Item {itemType} is empty or not available in Inventory!");
                return false;
            }
        }

        public void Add(ItemType itemType, int count = 1)
        {
            if (m_inventoryItems.TryGetValue(itemType, out int value))
            {
                value += count;
                m_inventoryItems[itemType] = value;
                OnUpdateItem(itemType, value);
            }
            else
            {
                m_inventoryItems.Add(itemType, count);
                OnUpdateItem(itemType, count);
            }
        }


    }
}