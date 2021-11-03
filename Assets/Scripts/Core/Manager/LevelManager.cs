using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Guinea.Core
{
    public class LevelManager: MonoBehaviour
    {
        [SerializeField]GameObject m_loadingScreen;
        [SerializeField]GameObject m_bar;
        
        List<SceneIndex> m_loadedScenes = new List<SceneIndex>();
        List<AsyncOperation> m_asyncOperations = new List<AsyncOperation>();
        float m_totalProgress;

        public void LoadLevel(IEnumerable<SceneIndex> sceneIndexes)
        {
            // m_loadingScreen.gameObject.SetActive(true);
            foreach(SceneIndex sceneIndex in m_loadedScenes)
            {
                m_asyncOperations.Add(SceneManager.UnloadSceneAsync((int)sceneIndex));
            }

            foreach(SceneIndex sceneIndex in sceneIndexes)
            {
                m_asyncOperations.Add(SceneManager.LoadSceneAsync((int)sceneIndex,LoadSceneMode.Additive));
            }
        }

        public IEnumerator GetSceneLoadProgress()
        {
            foreach(AsyncOperation asyncOperation in m_asyncOperations)
            {
                while(!asyncOperation.isDone)
                {
                    m_totalProgress = 0f;
                    foreach(AsyncOperation operation in m_asyncOperations)
                    {
                        m_totalProgress += operation.progress / 0.9f;
                    }

                    m_totalProgress /= m_asyncOperations.Count;
                    yield return null;
                }
            }

            // m_loadingScreen.gameObject.SetActive(false);
        }
    }

}