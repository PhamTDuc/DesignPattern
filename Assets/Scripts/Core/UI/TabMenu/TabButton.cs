using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Zenject;

namespace Guinea.Core.UI
{
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] TextMeshProUGUI m_label;
        protected TabMenu m_tabMenu;

        [Inject]
        public void Initialize(TabMenu tabMenu)
        {
            m_tabMenu = tabMenu;
        }

        public void SetLabel(string label) => m_label.SetText(label);

        public void OnPointerClick(PointerEventData eventData)
        {
            m_tabMenu.OnTabButtonClick(transform.GetSiblingIndex());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_tabMenu.OnTabButtonEnter(transform.GetSiblingIndex());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_tabMenu.OnTabButtonExit(transform.GetSiblingIndex());
        }

        public class Factory : PlaceholderFactory<TabButton, Transform, string,TabButton> { }
    }
}