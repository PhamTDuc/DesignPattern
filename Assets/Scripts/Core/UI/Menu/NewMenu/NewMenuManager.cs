using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Zenject;

namespace Guinea.Core.UI
{
    public class NewMenuManager : MonoBehaviour
    {
        Dictionary<Type, NewMenuBase> m_registeredMenus;
        [Tooltip("Parent for all menus prefab instantiated")]
        [SerializeField] Transform m_container;

        Stack<NewMenuBase> m_menuStack = new Stack<NewMenuBase>();
        NewMenuBase.Factory m_menuFactory;

        [Inject]
        void Initialize(NewMenuBase.Factory menuFactory)
        {
            m_menuFactory = menuFactory;
        }

        #region Unity Callbacks
        void OnEnable()
        {
            // * On Android the back button is sent as ESC
            InputManager.AddBackKeyEvent(OnBackKeyEvent);
        }
        void OnDisable()
        {
            InputManager.RemoveBackKeyEvent(OnBackKeyEvent);
        }
        #endregion

        public void Register(NewMenuBase menu)
        {
            Commons.Logger.Assert(menu != null, $"You are registering a null instance of NewMenuBase to NewMenuManager");
            m_registeredMenus.Add(menu.GetType(), menu);
        }


        public void UnRegister(NewMenuBase menu)
        {
            m_registeredMenus.Remove(menu.GetType());
        }

        public void Open<T>() where T : NewMenuBase
        {
            if (m_menuStack.Any(menu => menu.GetType() == typeof(T)))
            {
                Commons.Logger.LogWarning($"NewMenuBase {typeof(T)} already opened");
                return;
            }

            if (m_registeredMenus.TryGetValue(typeof(T), out NewMenuBase menu))
            {
                // * Menu already existed in the container
                NewMenuBase instance = Array.Find(m_container.GetComponentsInChildren<NewMenuBase>(true), m => menu.GetType() == typeof(T));
                if (instance == null)
                {
                    instance = m_menuFactory.Create(menu, m_container);
                }
                instance.gameObject.SetActive(true);
                instance.OnOpenMenu();

                if (m_menuStack.Count > 0)
                {
                    if (instance.DisableUnderneath)
                    {
                        DisableUnderneathMenus();
                    }
                }

                Canvas topCanvas = instance.GetComponent<Canvas>();
                Canvas underneathCanvas = m_menuStack.Peek().GetComponent<Canvas>();
                if (topCanvas != null && underneathCanvas != null)
                {
                    topCanvas.sortingOrder = underneathCanvas.sortingOrder + 1;
                }
            }
        }

        // * Only close menu in the top of the stack
        public void CloseMenu<T>()
        {
            NewMenuBase menu = m_menuStack.Pop();
            menu.OnCloseMenu();

            if (menu.DestroyWhenClose)
            {
                Destroy(menu.gameObject);
            }
            else
            {
                menu.gameObject.SetActive(false);
            }

            foreach (NewMenuBase m in m_menuStack)
            {
                m.gameObject.SetActive(true);
                if (m.DisableUnderneath) break;
            }
        }

        private void DisableUnderneathMenus()
        {
            foreach (NewMenuBase menu in m_menuStack)
            {
                menu.gameObject.SetActive(false);
                if (menu.DisableUnderneath) break; // * Some menus already disabled before hand
            }
        }
        private void OnBackKeyEvent(InputAction.CallbackContext context) => m_menuStack.Peek().OnBackKeyEvent();
    }
}