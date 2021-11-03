using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Guinea.Core.UI
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Canvas m_canvas;
        private RectTransform m_rectTransform;

        void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBegin Drag");
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
            Debug.Log(" Dragging");
        }

        public void OnEndDrag(PointerEventData eventData)
        {

            Debug.Log("OnEnd Drag");
        }
    }
}