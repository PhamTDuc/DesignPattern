using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Guinea.Core.Inventory
{
    public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] TextMeshProUGUI m_count;
        ItemUIDrag m_itemUIDrag;

        ItemType m_itemType;
        public ItemType Type => m_itemType;

        [Inject]
        void Initialize(ItemUIDrag draggable)
        {
            m_itemUIDrag = draggable;
        }

        public void Init(ItemType itemType, int count)
        {
            m_itemType = itemType;
            UpdateUI(count);
        }

        public void UpdateUI(int count)
        {
            m_count.text = count.ToString();

            if (count == 0)
            {
                m_itemUIDrag.OnEndDrag();
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Image image = transform.GetChild(0).GetComponent<Image>();
            m_itemUIDrag.OnBeginDrag(image.sprite, transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_itemUIDrag.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_itemUIDrag.OnEndDrag();
        }

        public class Factory : PlaceholderFactory<ItemType, UnityEngine.Transform, Sprite, int, ItemUI> { }
    }
}