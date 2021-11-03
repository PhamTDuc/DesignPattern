using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Guinea.Core.Components;

namespace Guinea.Core.Inventory
{
    public class InventoryUI : MonoBehaviour, IDropHandler
    {
        [SerializeField] Transform m_itemsContainer;
        [SerializeField] InventoryLoader m_inventoryLoader;

        ItemUI.Factory m_factory;
        ManipulationManager m_manipulationManager;



        [Inject]
        void Initialize(ItemUI.Factory factory, ManipulationManager manipulationManager)
        {
            m_factory = factory;
            m_manipulationManager = manipulationManager;
        }

        void Awake()
        {
            m_inventoryLoader.Init(); // TODO: Calling this in loading managers instead of a specific MonoBehaviour
        }

        void Start()
        {
            GenerateItems();
        }

        async void GenerateItems()
        {
            await m_inventoryLoader.Task;
            ItemType[] items = { ItemType.FRAME_00, ItemType.WHEEL_00, ItemType.WEAPON_00}; // WARN: For testing purpose only
            foreach (ItemType item in items)
            {
                Sprite sprite = m_inventoryLoader.Items[item].sprite;
                ItemUI slot = m_factory.Create(item, m_itemsContainer, sprite);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("InventoryUI:: On Drop ()");
            RectTransform panel = transform as RectTransform;
            if (!RectTransformUtility.RectangleContainsScreenPoint(panel, Input.mousePosition))
            {
                Transform trans;
                if (eventData.pointerDrag != null)
                {
                    ItemUI item = eventData.pointerDrag.GetComponent<ItemUI>();
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                    {
                        trans = Instantiate(m_inventoryLoader.Items[item.Type].obj, hit.point, Quaternion.identity).transform;
                        Debug.Log("Drop Item  !!!");
                    }
                    else
                    {
                        Vector3 mousePos = Input.mousePosition;
                        mousePos.z = 8f;
                        Vector3 itemPos = Camera.main.ScreenToWorldPoint(mousePos);
                        trans = Instantiate(m_inventoryLoader.Items[item.Type].obj, itemPos, Quaternion.identity).transform;
                    }
                    m_manipulationManager.SelectObject(trans);
                    m_manipulationManager.Invoke();
                }
            }
        }
    }
}