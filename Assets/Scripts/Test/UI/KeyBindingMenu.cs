using UnityEngine;
using Guinea.Core;
using Guinea.Core.UI;

namespace Guinea.Test.UI
{
    public class KeyBindingMenu : MenuBase
    {

        KeyBinding[] m_keyBindings;

        static string s_bindingsKeyName = "SaveBindings";

        void Awake()
        {
            m_keyBindings = GetComponentsInChildren<KeyBinding>();
        }

        public override void OnBackKeyEvent()
        {
            m_menuManager.CloseTopMenu();
        }

        public override void OnCloseMenu() { }

        public override void OnOpenMenu() { }

        public void OnSaveToJson()
        {
            Debug.Log("Save KeyBinding to used later");
            string json = InputManager.SaveBindingOverrideToString();
            PlayerPrefs.SetString(s_bindingsKeyName, json);
        }

        public void OnLoadFromJson()
        {
            Debug.Log("Load from Json to used ");
            string json = PlayerPrefs.GetString(s_bindingsKeyName);
            if (!string.IsNullOrEmpty(json))
            {
                InputManager.LoadBindingOverridesFromJson(json);
            }
        }

        public void OnResetAll()
        {
            foreach (KeyBinding keyBinding in m_keyBindings)
            {
                keyBinding.Reset();
            }
        }
    }
}