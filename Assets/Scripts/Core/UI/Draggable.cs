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
            Commons.Logger.Log("OnBegin Drag");
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
            Commons.Logger.Log(" Dragging");
        }

        public void OnEndDrag(PointerEventData eventData)
        {

            Commons.Logger.Log("OnEnd Drag");
        }
    }
}