using UnityEngine;
using Guinea.Core.UI;

namespace Guinea.Test.UI
{
    public class TestMainMenu : MenuBase
    {
        public void OnNewGame()
        {
            Debug.Log("New Game");
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

        public void OnQuit()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        public override void OnBackKeyEvent() { }

        public override void OnOpenMenu() { }

        public override void OnCloseMenu() { }
    }
}