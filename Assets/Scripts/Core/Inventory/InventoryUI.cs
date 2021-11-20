using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Guinea.Core.Components;
using Guinea.Core.UI;

namespace Guinea.Core.Inventory
{
    public class InventoryUI : MenuBase, IDropHandler
    {
        [SerializeField] Transform m_itemsContainer;
        Dictionary<ItemType, ItemUI> m_itemUIs;

        ItemUI.Factory m_factory;
        ManipulationManager m_manipulationManager;
        InventoryLoader m_inventoryLoader;
        Inventory m_inventory;

        AddComponentCommand.Factory m_addComponentCommandFactory;

        [Inject]
        void Initialize(ItemUI.Factory factory, AddComponentCommand.Factory addComponentCommandFactory, ManipulationManager manipulationManager, InventoryLoader inventoryLoader, Inventory inventory)
        {
            m_factory = factory;
            m_addComponentCommandFactory = addComponentCommandFactory;
            m_manipulationManager = manipulationManager;
            m_inventoryLoader = inventoryLoader;
            m_inventory = inventory;
            Commons.Logger.Log("InventoryUI::Initialize()");
        }

        void OnEnable()
        {
            m_inventory.OnLoadItems += GenerateInventoryUI;
            m_inventory.OnUpdateItem += UpdateUI;
        }

        void OnDisable()
        {
            m_inventory.OnLoadItems -= GenerateInventoryUI;
            m_inventory.OnUpdateItem -= UpdateUI;
        }


        // // * Will be call in Inventory::GenerateInventoryUI()
        private async void GenerateInventoryUI(Dictionary<ItemType, int> inventoryItems)
        {
            await m_inventoryLoader.Task;
            Commons.Logger.Log("InventoryUI::GenerateInventoryUI()");
            m_itemUIs = new Dictionary<ItemType, ItemUI>();
            foreach (KeyValuePair<ItemType, int> item in inventoryItems)
            {
                Sprite sprite = m_inventoryLoader.Items[item.Key].sprite;
                ItemUI slot = m_factory.Create(item.Key, m_itemsContainer, sprite, item.Value);
                m_itemUIs.Add(item.Key, slot);
            }
        }

        // * For existing item
        private void UpdateUI(ItemType type, int count)
        {
            if (m_itemUIs.TryGetValue(type, out ItemUI itemUI))
            {
                itemUI.UpdateUI(count);
            }
            else
            {
                Sprite sprite = m_inventoryLoader.Items[type].sprite;
                ItemUI slot = m_factory.Create(type, m_itemsContainer, sprite, count);
                m_itemUIs.Add(type, slot);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            Commons.Logger.Log("InventoryUI::On Drop()");
            RectTransform panel = transform as RectTransform;
            if (!RectTransformUtility.RectangleContainsScreenPoint(panel, Input.mousePosition))
            {
                Transform trans;
                Vector3 pos;
                Quaternion rot;
                if (eventData.pointerDrag != null)
                {
                    ItemUI item = eventData.pointerDrag.GetComponent<ItemUI>();
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Transform instance = m_inventoryLoader.Items[item.Type].obj.transform;
                    rot = instance.rotation;

                    if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                    {
                        pos = hit.point;
                        trans = Instantiate(instance, pos, instance.rotation);
                    }
                    else
                    {
                        Vector3 mousePos = Input.mousePosition;
                        mousePos.z = 8f;
                        pos = Camera.main.ScreenToWorldPoint(mousePos);
                        trans = Instantiate(instance, pos, instance.rotation);
                    }
                    m_inventory.Use(item.Type);
                    AddComponentCommand command = m_addComponentCommandFactory.Create(item.Type, trans);

                    m_manipulationManager.Select(trans);
                    m_manipulationManager.SwitchToMoveOperator();
                    m_manipulationManager.InvokeCurrentOperator();
                    Commons.Logger.Log("Drop Item  !!!");
                }
            }
        }

        public override void OnBackKeyEvent()
        {
            // throw new System.NotImplementedException();
        }

        public override void OnOpenMenu()
        {
            // throw new System.NotImplementedException();
        }

        public override void OnCloseMenu()
        {
            // throw new System.NotImplementedException();
        }
    }
}