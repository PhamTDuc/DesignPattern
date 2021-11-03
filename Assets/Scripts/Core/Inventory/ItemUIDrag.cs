using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

using Guinea.Core.Components;

namespace Guinea.Core.Inventory
{
    public class ItemUIDrag : MonoBehaviour
    {
        [SerializeField] Canvas m_canvas;
        [SerializeField] RectTransform m_rectTransform;
        [SerializeField] Image m_itemIcon;

        public void OnBeginDrag(Sprite sprite, Transform trans)
        {
            transform.position = trans.position;
            m_itemIcon.sprite = sprite;
            gameObject.SetActive(true);
            Debug.Log("OnBeginDrag()");
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
        }

        public void OnEndDrag()
        {
            gameObject.SetActive(false);
            Debug.Log("OnEndDrag()");
        }
    }
}