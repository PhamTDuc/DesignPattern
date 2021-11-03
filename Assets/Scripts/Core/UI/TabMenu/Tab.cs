using System;
using UnityEngine;
using Zenject;

namespace Guinea.Core.UI
{
    public class Tab : MonoBehaviour, ITabEvent
    {
        protected TabMenu m_tabMenu;

        public event Action OnTabEnable;
        public event Action OnTabDisable;

        [Inject]
        public void Initialize(TabMenu tabMenu)
        {
            m_tabMenu = tabMenu;
        }

        void OnEnable()
        {
            OnTabEnable?.Invoke();
        }

        void OnDisable()
        {
            OnTabDisable?.Invoke();
        }
    }
}