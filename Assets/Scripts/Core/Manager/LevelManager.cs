using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Guinea.Core.UI;

namespace Guinea.Core
{
    public class LevelManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] GameObject m_loadingScreen;
        [SerializeField] ProgressBar m_bar;
        #region AsyncManagers
        [Header("IAsyncManagers")]
        Inventory.InventoryLoader m_inventoryLoader;
        #endregion

        List<SceneIndex> m_loadedScenes = new List<SceneIndex>();
        List<AsyncOperation> m_asyncOperations = new List<AsyncOperation>();
        List<IAsyncManager> m_asyncManagers = new List<IAsyncManager>();
        private static WaitForSeconds s_delay = new WaitForSeconds(4f);
        float m_loadAsyncManagerProgress = 0f;
        float m_loadLevelProgress = 0f;

        static readonly SceneIndex[] s_startupScenes = { SceneIndex.ENTITY_BUILDER };

        [Inject]
        void Initialize(Inventory.InventoryLoader inventoryLoader)
        {
            m_inventoryLoader = inventoryLoader;
            Commons.Logger.Log("LevelManager::InstallBindings()");
        }

        public void Awake()
        {
            #region AsyncManagers
            m_asyncManagers.Add(m_inventoryLoader);
            #endregion

            StartCoroutine(Main());
        }

        public void LoadLevel(IEnumerable<SceneIndex> sceneIndexes, bool loadingScreen = false, bool removeLoadedScene = false)
        {
            if (removeLoadedScene)
            {
                foreach (SceneIndex sceneIndex in m_loadedScenes)
                {
                    m_asyncOperations.Add(SceneManager.UnloadSceneAsync((int)sceneIndex));
                    m_loadedScenes.Remove(sceneIndex);
                }

            }

            foreach (SceneIndex sceneIndex in sceneIndexes)
            {
                m_asyncOperations.Add(SceneManager.LoadSceneAsync((int)sceneIndex, LoadSceneMode.Additive));
                m_loadedScenes.Add(sceneIndex);
            }

            if (loadingScreen)
            {
                StartCoroutine(GetSceneLoadProgress());
            }
        }

        private IEnumerator GetSceneLoadProgress()
        {
            float totalProgress;
            m_loadingScreen.SetActive(true);
            foreach (AsyncOperation asyncOperation in m_asyncOperations)
            {
                while (!asyncOperation.isDone)
                {
                    totalProgress = 0f;
                    foreach (AsyncOperation operation in m_asyncOperations)
                    {
                        totalProgress += operation.progress / 0.9f;
                    }

                    totalProgress /= m_asyncOperations.Count;
                    m_loadLevelProgress = totalProgress;
                    m_bar.ChangeValue((m_loadAsyncManagerProgress + m_loadLevelProgress) / 2f);
                    yield return null;
                }
            }
            m_loadingScreen.SetActive(false);
        }

        private IEnumerator LoadAsyncManagersCoroutine()
        {
            m_loadingScreen.SetActive(true);
            while (m_asyncManagers.Find(asyncManager => asyncManager.Status == AsyncOperationStatus.None) != null)
            {
                float totalProgress = 0f;
                foreach (IAsyncManager asyncManager in m_asyncManagers)
                {
                    Commons.Logger.Assert(asyncManager.Status != AsyncOperationStatus.Failed, $"Load AsyncManager {asyncManager.DebugName} FAILED");
                    totalProgress += asyncManager.PercentComplete;
                    yield return s_delay;
                }
                totalProgress /= m_asyncOperations.Count;
                m_loadAsyncManagerProgress = totalProgress;
                m_bar.ChangeValue((m_loadAsyncManagerProgress + m_loadLevelProgress) / 2f);
                yield return null;
            }
            m_loadingScreen.SetActive(false);
        }

        private IEnumerator Main()
        {
            foreach (IAsyncManager asyncManager in m_asyncManagers)
            {
                asyncManager.Init();
            }

            yield return LoadAsyncManagersCoroutine();

            // * Only call LoadLevel() from SceneIndex.TITLE_SCENE, not from other scene
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.TITLE_SCENE)
            {
                LoadLevel(s_startupScenes, true);
            }
        }
    }

}