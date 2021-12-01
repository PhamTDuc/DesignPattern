using UnityEngine;
using Guinea.Core.Components;
using Zenject;

namespace Guinea.Test.UI
{
    public class TestButtons : MonoBehaviour
    {
        Core.LevelManager m_levelManager;
        [Inject]
        void Initialize(Core.LevelManager levelManager)
        {
            m_levelManager = levelManager;
        }

        public void Undo()
        {
            OperatorManager.Undo();
        }

        public void Redo()
        {
            OperatorManager.Redo();
        }

        public void GoToTestScene()
        {
            m_levelManager.LoadLevel(new[] { Core.SceneIndex.LEVEL_01 }, true, true);
        }
    }
}