using UnityEngine;
using Zenject;

namespace Guinea.Core.UI
{
    public class TabMenu : MenuBase
    {
        [SerializeField] TabButton m_btnPrefab;
        [SerializeField] Transform m_btnContainer;
        [SerializeField] Transform m_tabContainer;
        Tab[] m_tabs;
        int m_current = 0;
        TabButton.Factory m_tabBtnFactory;
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
            Tab tab = m_tabContainer.GetChild(m_current).GetComponent<Tab>();
            tab.gameObject.SetActive(false);
            tab = m_tabContainer.GetChild(index).GetComponent<Tab>();
            tab.gameObject.SetActive(true);
            m_current = index;
        }

        private void CreateTabButtonsAndDisableTabs()
        {
            for (int i = 0; i < m_tabs.Length; i++)
            {
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

        public override void OnBackKeyEvent()
        {
            // throw new System.NotImplementedException();
            Commons.Logger.Log("Call OnBackKeyEvent()");
            m_menuManager.CloseTopMenu();
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