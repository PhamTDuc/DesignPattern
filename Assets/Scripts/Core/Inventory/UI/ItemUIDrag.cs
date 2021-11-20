using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

namespace Guinea.Core.Inventory
{
    public class ItemUIDrag : MonoBehaviour
    {
        [SerializeField] RectTransform m_rectTransform;
        [SerializeField] Image m_itemIcon;

        Canvas m_canvas;
        void Awake()
        {
            m_canvas = GetComponentInParent<Canvas>();
            gameObject.SetActive(false);
        }

        public void OnBeginDrag(Sprite sprite, Transform trans)
        {
            transform.position = trans.position;
            m_itemIcon.sprite = sprite;
            gameObject.SetActive(true);
            Commons.Logger.Log("OnBeginDrag()");
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
        }

        public void OnEndDrag()
        {
            gameObject.SetActive(false);
            Commons.Logger.Log("OnEndDrag()");
        }
    }
}