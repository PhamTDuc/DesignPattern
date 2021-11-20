using UnityEngine;
using Guinea.Core;
using Guinea.Core.UI;

namespace Guinea.Test.UI
{
    public class TestMainMenu : MenuBase
    {
        void Start()
        {
            Commons.Logger.Log("TestMainMenu.Start()");
            m_menuManager.CloseCurrentMenus();
        }

        public void OnNewGame()
        {
            Commons.Logger.Log("New Game");
        }

        public void OnSetting()
        {
            m_menuManager.OpenMenu(MenuType.MAIN_SETTING);
        }

        public void OnKeyBinding()
        {
            m_menuManager.OpenMenu(MenuType.MAIN_KEYBINDING);
        }

        public void OnStats()
        {
            m_menuManager.OpenMenu(MenuType.MAIN_STATS);
        }

        public void OnTabMenu()
        {
            m_menuManager.OpenMenu(MenuType.MAIN_TAB_MENU);
        }

        public void OnQuit()
        {
#if UNITY_EDITOR
            // * Application.Quit() does not work in the editor so
            // * UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        public override void OnBackKeyEvent()
        {
            if (m_menuManager.IsVisible)
            {
                m_menuManager.CloseCurrentMenus();
                InputManager.SetEnableExceptUI(true); // * Disable other Maps when Menus is ON
            }
            else
            {
                m_menuManager.OpenCurrentMenus();
                InputManager.SetEnableExceptUI(false); // * Enable other Maps when Menus is OFF
            }
        }

        public override void OnOpenMenu() { }

        public override void OnCloseMenu() { }
    }
}