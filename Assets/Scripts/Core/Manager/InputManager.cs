using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine;

namespace Guinea.Core
{
    public static class InputManager
    {
        public static GameplayInputAction s_inputAction = new GameplayInputAction();
        public static GameplayInputAction Map => s_inputAction;
        private static HashSet<InputActionMap> s_enabledInputActionMap = new HashSet<InputActionMap>();
        private static List<Action<InputAction.CallbackContext>> m_backKeyCallbackList = new List<Action<InputAction.CallbackContext>>();

        public static void Init()
        {
            // s_inputAction.Enable();
            s_inputAction.Disable();
            s_inputAction.UI.Enable();
            s_inputAction.UI.Escape.performed += OnBackKeyEvent;
        }

        public static void AddBackKeyEvent(Action<InputAction.CallbackContext> action)
        {
            if (!m_backKeyCallbackList.Contains(action))
            {
                m_backKeyCallbackList.Add(action);
                Commons.Logger.Log($"AddBackKeyEvent(): {m_backKeyCallbackList.Count} Action: {action}");
            }
        }

        public static void RemoveBackKeyEvent(Action<InputAction.CallbackContext> action)
        {
            if (m_backKeyCallbackList.Remove(action))
            {
                Commons.Logger.Log($"RemoveBackKeyEvent() SUCCESS: {m_backKeyCallbackList.Count} Action: {m_backKeyCallbackList.LastOrDefault()}");
            }
            else
            {
                Commons.Logger.Log($"RemoveBackKeyEvent() FAILED. {action} not exists");
            }
        }

        private static void OnBackKeyEvent(InputAction.CallbackContext context)
        {
            m_backKeyCallbackList.LastOrDefault()?.Invoke(context);
        }

        public static void SetActionMap(InputActionMap actionMap, bool enabled)
        {
            if (enabled)
            {
                s_enabledInputActionMap.Add(actionMap);
                actionMap.Enable();
            }
            else
            {
                s_enabledInputActionMap.Remove(actionMap);
                actionMap.Disable();
            }
        }

        public static void SetEnableExceptUI(bool enabled)
        {
            foreach (InputActionMap actionMap in s_enabledInputActionMap)
            {
                if (enabled)
                {
                    actionMap.Enable();
                }
                else
                {
                    actionMap.Disable();
                }
            }
        }

        public static void SetEnableUI(bool enabled)
        {
            if (enabled)
            {
                s_inputAction.UI.Enable();
            }
            else
            {
                s_inputAction.UI.Disable();
            }
        }

        public static void SetCursorVisible(bool visible)
        {
            Cursor.visible = visible;
        }

        public static string SaveBindingOverrideToString()
        {
            return s_inputAction.SaveBindingOverridesAsJson();
        }

        public static void LoadBindingOverridesFromJson(string json, bool removeExisting = true)
        {
            s_inputAction.LoadBindingOverridesFromJson(json, removeExisting);
        }
    }
}