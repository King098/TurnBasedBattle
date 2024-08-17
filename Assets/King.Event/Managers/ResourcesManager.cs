


using System;
using System.Threading.Tasks;
using King.TurnBasedCombat;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace AUIFramework
{
    /// <summary>
    /// addressable资源管理器
    /// </summary>
    public static class ResourcesManager
    {
        public static void LoadScene(string sceneNameOrPath, Action<SceneInstance> action)
        {
            AsyncOperationHandle<SceneInstance> sceneLoadHandle = Addressables.LoadSceneAsync(sceneNameOrPath);
            sceneLoadHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (action != null)
                    {
                        action.Invoke(handle.Result);
                    }
                }
            };
        }

        /// <summary>
        /// 使用await/async方式加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetNameOrPath"></param>
        /// <returns></returns>
        public async static Task<AsyncOperationHandle> LoadAsync<T>(string assetNameOrPath)
        {
            BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"ResourcesManager 异步加载资源 {assetNameOrPath}");
            try
            {
                AsyncOperationHandle<T> loadHandle = Addressables.LoadAssetAsync<T>(assetNameOrPath);
                await loadHandle.Task;
                if (loadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    return loadHandle;
                }
                else
                {
                    BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"ResourcesManager 异步加载资源 {assetNameOrPath}");
                    return loadHandle;
                }
            }
            catch
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"ResourcesManager 异步加载资源 {assetNameOrPath}");
                return default(AsyncOperationHandle);
            }
        }

        public static AsyncOperationHandle LoadAsync<T>(string assetNameOrPath, Action<AsyncOperationHandle,T> action,Action<AsyncOperationStatus> failed = null)
        {
            BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"ResourcesManager 异步加载资源 {assetNameOrPath}");
            AsyncOperationHandle<T> loadHandle = Addressables.LoadAssetAsync<T>(assetNameOrPath);
            loadHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (action != null)
                    {
                        action.Invoke(handle,handle.Result);
                    }
                }
                else
                {
                    failed?.Invoke(handle.Status);
                    BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"ResourcesManager 异步加载资源 {assetNameOrPath}");
                }
            };
            return loadHandle;
        }

        public static void ReleaseHandle(AsyncOperationHandle handle)
        {
            if(handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

#if !UNITY_WEBGL
        //同步加载资源接口，不在WEBGL平台生效
        public static T LoadSync<T>(string assetNameOrPath)
        {
            AsyncOperationHandle<T> loadHandle = Addressables.LoadAssetAsync<T>(assetNameOrPath);
            loadHandle.WaitForCompletion();
            T result = default;
            if(loadHandle.IsDone)
            {
                result = loadHandle.Result;
            }
            return result;
        }
#endif
    }
}