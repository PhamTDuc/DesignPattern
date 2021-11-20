using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Guinea.Core.UI
{
    public class NewMenuBase : MonoBehaviour
    {
        [SerializeField] NewMenuBase[] m_children;
        [SerializeField] bool m_destroyWhenClose;
        [SerializeField] bool m_disableUnderneath;
        public bool DestroyWhenClose { get => m_destroyWhenClose; }
        public bool DisableUnderneath { get => m_disableUnderneath; }
        NewMenuManager m_menuManager;

        [Inject]
        void Initialize(NewMenuManager menuManager)
        {
            m_menuManager = menuManager;
        }

        void OnEnable()
        {
            foreach (NewMenuBase menu in m_children)
            {
                m_menuManager.Register(menu);
            }
        }

        void OnDisable()
        {
            foreach (NewMenuBase menu in m_children)
            {
                m_menuManager.UnRegister(menu);
            }
        }

        public virtual void OnBackKeyEvent() { }

        public virtual void OnOpenMenu() { }
        public virtual void OnCloseMenu() { }

        public class Factory : PlaceholderFactory<NewMenuBase, Transform, NewMenuBase> { }
    }
}