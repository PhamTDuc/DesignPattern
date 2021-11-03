using UnityEngine;
using Zenject;

namespace Guinea.Core.UI
{
    public class TabMenu : MonoBehaviour
    {
        [SerializeField] TabButton m_btnPrefab;
        [SerializeField] Transform m_btnContainer;
        [SerializeField] Transform m_tabContainer;
        Tab[] m_tabs;
        int current = 0;
        TabButton.Factory m_tabBtnFactory;
        // DiContainer m_container;

        // public void Initialize(DiContainer container)
        // {
        //     m_container = container;
        // }
        [Inject]
        public void Initialize(TabButton.Factory tabBtnFactory)
        {
            m_tabBtnFactory = tabBtnFactory;
        }

        void Awake()
        {
            m_tabs = m_tabContainer.GetComponentsInChildren<Tab>(true);
            CreateTabButtonsAndDisableTabs();
        }

        public void OnTabButtonClick(int index)
        {
            Tab tab = m_tabContainer.GetChild(index).GetComponent<Tab>();
            SwitchTab(index);
        }

        public void OnTabButtonEnter(int index)
        {
            Tab tab = m_tabContainer.GetChild(index).GetComponent<Tab>();
        }

        public void OnTabButtonExit(int index)
        {
            Tab tab = m_tabContainer.GetChild(index).GetComponent<Tab>();
        }

        public void SetTabStatus(int index, bool enabled)
        {
            TabButton tabBtn = m_btnContainer.GetChild(index).GetComponent<TabButton>();
            Tab tab = m_tabContainer.GetChild(index).GetComponent<Tab>();
            tabBtn.gameObject.SetActive(enabled);
            tab.gameObject.SetActive(enabled);
        }

        private void SwitchTab(int index)
        {
            Tab tab = m_tabContainer.GetChild(current).GetComponent<Tab>();
            tab.gameObject.SetActive(false);
            tab = m_tabContainer.GetChild(index).GetComponent<Tab>();
            tab.gameObject.SetActive(true);
            current = index;
        }

        private void CreateTabButtonsAndDisableTabs()
        {
            for (int i = 0; i < m_tabs.Length; i++)
            {
                // TabButton tabBtn = m_container.InstantiatePrefabForComponent<TabButton>(m_btnPrefab, m_btnContainer);
                TabButton tabBtn = m_tabBtnFactory.Create(m_btnPrefab, m_btnContainer, m_tabs[i].name);

                // * Activate the first tab, disable other tabs
                if (i == 0)
                {
                    m_tabs[i].gameObject.SetActive(true);
                }
                else
                {
                    m_tabs[i].gameObject.SetActive(false);
                }
            }
        }
    }
}