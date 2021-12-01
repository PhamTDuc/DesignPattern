using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Guinea.Core.Inventory
{
    // * Persistent Data Loader
    public class InventoryLoader : MonoBehaviour, IAsyncManager
    {
        [TextArea(4, 20)] [SerializeField] string m_json;
        Dictionary<ItemType, ItemAsset> m_itemsSpriteDict;
        List<ItemAssetHandle> m_handles;
        List<Task> m_tasks = new List<Task>();
        Task m_completedTask;

        private static string debugName = "Inventory Loader";
        private static WaitForSeconds s_delay = new WaitForSeconds(1f);

        public string DebugName => debugName;

        public Exception OperationException { get; private set; }
        public float PercentComplete { get; private set; }
        public AsyncOperationStatus Status { get; private set; } = AsyncOperationStatus.None;
        public Task Task
        {
            get
            {
                if (m_completedTask == null) Initialize();
                return m_completedTask;
            }
        }
        public Dictionary<ItemType, ItemAsset> Items => m_itemsSpriteDict;

        public void Initialize()
        {
            if (m_completedTask != null) return;
            List<ItemInfo> itemsInfo = DataHandler.JsonHandler.Deserialize<List<ItemInfo>>(m_json);
            m_handles = new List<ItemAssetHandle>();
            m_itemsSpriteDict = new Dictionary<ItemType, ItemAsset>();
            m_completedTask = new Task(() => { });

            foreach (ItemInfo itemInfo in itemsInfo)
            {
                AsyncOperationHandle<Sprite> spriteHandle = Addressables.LoadAssetAsync<Sprite>(itemInfo.spriteAddress);
                AsyncOperationHandle<GameObject> objHandle = Addressables.LoadAssetAsync<GameObject>(itemInfo.objectAddress);
                Commons.Logger.Log($"ItemInfo: {itemInfo.objectAddress}");
                m_handles.Add(new ItemAssetHandle(spriteHandle, objHandle));
                m_tasks.Add(spriteHandle.Task);
                m_tasks.Add(objHandle.Task);
            }
            StartCoroutine(LoadFromListCoroutine(itemsInfo));
        }

        private IEnumerator LoadFromListCoroutine(List<ItemInfo> itemsInfo)
        {
            ItemAssetHandle current_handle;
            while ((current_handle = m_handles.Find(handle => !handle.IsDone)) != default(ItemAssetHandle))
            {
                // * Break when having any Exception
                OperationException = current_handle.OperationException;
                if (OperationException != null)
                {
                    Status = AsyncOperationStatus.Failed;
                    yield break;
                }

                float percentageComplete = 0f;
                foreach (ItemAssetHandle handle in m_handles)
                {
                    percentageComplete += handle.PercentComplete;
                }
                PercentComplete = percentageComplete / m_handles.Count;
                yield return s_delay;
            }

            for (int i = 0; i < itemsInfo.Count; i++)
            {
                if (Enum.TryParse(itemsInfo[i].type, out ItemType itemType))
                {
                    m_itemsSpriteDict.Add(itemType, new ItemAsset(m_handles[i].spriteHandle.Result, m_handles[i].objHandle.Result));
                }
                else
                {
                    Debug.LogError($"Fail to Parse {itemsInfo[i].type} to enum type<{typeof(ItemType)}>");
                }

            }

            PercentComplete = 100f;
            Status = AsyncOperationStatus.Succeeded;
            m_completedTask.Start();
        }
        ~InventoryLoader()
        {
            foreach (ItemAssetHandle handle in m_handles)
            {
                handle.Release();
            }
        }

        public struct ItemAsset
        {
            public Sprite sprite;
            public GameObject obj;
            public ItemAsset(Sprite sprite, GameObject obj)
            {
                this.sprite = sprite;
                this.obj = obj;
            }
        }

        private struct ItemAssetHandle : IEquatable<ItemAssetHandle>
        {
            public AsyncOperationHandle<Sprite> spriteHandle;
            public AsyncOperationHandle<GameObject> objHandle;
            public float PercentComplete => (spriteHandle.PercentComplete + objHandle.PercentComplete) / 2.0f;
            public bool IsDone => spriteHandle.IsDone && objHandle.IsDone;
            public ItemAssetHandle(AsyncOperationHandle<Sprite> spriteHandle, AsyncOperationHandle<GameObject> objHandle)
            {
                this.spriteHandle = spriteHandle;
                this.objHandle = objHandle;
            }

            public void Release()
            {
                Addressables.Release(spriteHandle);
                Addressables.Release(objHandle);
            }

            public bool Equals(ItemAssetHandle other)
            {
                return this.spriteHandle.Equals(other.spriteHandle) && this.objHandle.Equals(other.objHandle);
            }

            public override bool Equals(object o)
            {
                if (o is ItemAssetHandle)
                {
                    return Equals((ItemAssetHandle)o);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this.spriteHandle.GetHashCode() + this.objHandle.GetHashCode();
            }

            public static bool operator !=(ItemAssetHandle left, ItemAssetHandle right)
            {
                return !left.Equals(right);
            }
            public static bool operator ==(ItemAssetHandle left, ItemAssetHandle right)
            {
                return left.Equals(right);
            }

            public Exception OperationException => spriteHandle.OperationException != null ? spriteHandle.OperationException : objHandle.OperationException;
        }
    }
}