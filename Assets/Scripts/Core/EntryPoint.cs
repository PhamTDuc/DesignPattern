using UnityEngine;
using Zenject;

namespace Guinea.Core
{
    public sealed class EntryPoint: MonoBehaviour
    {
        private LevelManager m_levelManager;
        private static readonly SceneIndex[] m_sceneIndexes = {SceneIndex.TITLE}; 
        
        [Inject]
        void Initialize(LevelManager levelManager)
        {
            m_levelManager = levelManager;
        }

        void Awake()
        {
            m_levelManager.LoadLevel(m_sceneIndexes);
        }
    }
}