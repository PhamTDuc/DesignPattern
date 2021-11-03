using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Guinea.Core.UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] MenuBase[] m_menus;

        [Tooltip("Parent for all menus prefab instantiated")]
        [SerializeField] Transform m_container;
        [SerializeField] bool m_isToggle;
        Stack<MenuBase> m_menuStack = new Stack<MenuBase>();
        MenuBase.Factory m_menuFactory;

        [Inject]
        public void Initialize(MenuBase.Factory menuFactory)
        {
            m_menuFactory = menuFactory;
        }

        #region Unity Callbacks
        void OnValidate()
        {
            foreach (MenuBase menu in m_menus)
            {
                if (menu == null)
                {
                    Debug.LogWarning("Some menus is missing from MenuManager");
                }
            }
        }

        void Awake()
        {
            // * Deactivate all menus in containers
            foreach (MenuBase m in m_container.GetComponentsInChildren<MenuBase>(true))
            {
                m.gameObject.SetActive(false);
            }

            OpenMenu(m_menus[0].Type);
            if (m_isToggle)
            {
                m_container.gameObject.SetActive(false); // * Not show ToggleMenu at first
            }
        }

        void OnEnable()
        {
            // * On Android the back button is sent as ESC
            InputManager.AddBackKeyEvent(OnBackKeyEvent);
            // InputManager.Map.UI.BackKey.performed += OnBackKeyEvent;
        }

        void OnDisable()
        {
            InputManager.RemoveBackKeyEvent(OnBackKeyEvent);
        }
        #endregion

        public void OpenMenu(MenuType menuType)
        {
            // * Menu already in Stack and Activate
            if (m_menuStack.Any(m => m.Type == menuType && m.gameObject.activeSelf))
            {
                return;
            }

            MenuBase menu = Array.Find(m_menus, m => m.Type == menuType);
            if (menu != null)
            {
                MenuBase instance = Array.Find(m_container.GetComponentsInChildren<MenuBase>(true), m => m.Type == menuType);
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
                        DisableUnderneath();
                    }

                    if (instance.DisableSameLayer)
                    {
                        if (instance.Layer == m_menuStack.Peek().Layer)
                        {
                            m_menuStack.Pop().gameObject.SetActive(false);

                        }
                    }

                    Canvas topCanvas = instance.GetComponent<Canvas>();
                    Canvas underneathCanvas = m_menuStack.Peek().GetComponent<Canvas>();
                    if (topCanvas != null && underneathCanvas != null)
                    {
                        topCanvas.sortingOrder = underneathCanvas.sortingOrder + 1;
                    }
                }

                m_menuStack.Push(instance);
            }
        }

        public void CloseMenu(MenuType menuType)
        {
            if (m_menuStack.Count == 0)
            {
                Debug.LogWarning($"Menu {menuType} can't be closed because menu stack is empty");
                return;
            }

            if (m_menuStack.Peek().Type != menuType)
            {
                Debug.LogWarning($"Menu {menuType} can't be closed because it is not on the top of menu stack");
            }
            CloseTopMenu();
        }

        public void CloseTopMenu()
        {
            MenuBase menu = m_menuStack.Pop();

            if (menu.DestroyWhenClose)
            {
                Destroy(menu.gameObject);
            }
            else
            {
                menu.gameObject.SetActive(false);
            }

            foreach (MenuBase m in m_menuStack)
            {
                m.gameObject.SetActive(true);
                if (m.DisableUnderneath) break;
            }
        }

        public void OnBackKeyEvent(InputAction.CallbackContext context)
        {
            if (m_menuStack.Count > 1)
            {
                m_menuStack.Peek().OnBackKeyEvent();
            }
            else
            {
                if (m_isToggle)
                {
                    m_container.gameObject.SetActive(!m_container.gameObject.activeSelf);
                    InputManager.SetEnableExceptUI(!m_container.gameObject.activeSelf);
                }
                else
                {
                    m_menuStack.Peek().OnBackKeyEvent();
                }
            }
        }

        void DisableUnderneath()
        {
            foreach (MenuBase m in m_menuStack)
            {
                m.gameObject.SetActive(false);
                if (m.DisableUnderneath) break;
            }
        }


        #region For used in UnityEvent in Inspector
        public void OpenMenuHelper(string menuType)
        {
            if (Enum.TryParse<MenuType>(menuType, true, out MenuType type))
            {
                OpenMenu(type);
            }
        }

        public void CloseMenuHelper(string menuType)
        {
            if (Enum.TryParse<MenuType>(menuType, true, out MenuType type))
            {
                CloseMenu(type);
            }
        }
        #endregion
    }
}