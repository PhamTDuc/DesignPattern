namespace Guinea.Core.UI
{
    public class Menu : MenuBase
    {
        public override void OnBackKeyEvent()
        {
            m_menuManager.CloseTopMenu();
        }

        public override void OnCloseMenu()
        { }

        public override void OnOpenMenu()
        { }
    }
}