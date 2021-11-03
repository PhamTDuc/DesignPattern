using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Guinea.Core.Inventory
{
    public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        ItemUIDrag m_itemUIDrag;

        ItemType m_itemType;
        public ItemType Type => m_itemType;

        [Inject]
        void Initialize(ItemUIDrag draggable)
        {
            m_itemUIDrag = draggable;
        }

        public void SetItemType(ItemType itemType)
        {
            m_itemType = itemType;
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

        public class Factory : PlaceholderFactory<ItemType, UnityEngine.Transform, Sprite, ItemUI> { }
    }
}