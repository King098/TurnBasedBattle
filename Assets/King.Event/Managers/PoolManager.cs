using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using King.TurnBasedCombat;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AUIFramework
{
    public enum PoolState
    {
        None,
        Initing,
        Working,
        Clear
    }
    public class Pool
    {
        public string Name;
        public PoolState State;
        public List<GameObject> UsedData;
        public List<GameObject> UnusedData;
        public string TemplatePath;
        public GameObject TemplateData;

        /// <summary>
        /// 当T对象放会池子的时候会额外执行的逻辑
        /// </summary>
        public Action<GameObject> PutAction;
        /// <summary>
        /// 清理池子的时候要额外执行的动作
        /// </summary>
        public Action<GameObject> ClearAction;

        protected AsyncOperationHandle loadHandle;

        public async Task Init()
        {
            if(State == PoolState.None || State == PoolState.Clear)
            {
                var handle = await ResourcesManager.LoadAsync<GameObject>(TemplatePath);
                if(handle.Status == AsyncOperationStatus.Succeeded)
                {
                    ResourcesManager.ReleaseHandle(loadHandle);
                    loadHandle = handle;
                    this.TemplateData = handle.Result as GameObject;
                    this.State = PoolState.Working;
                    BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"对象池 {this.Name} 开始正常工作了");
                }
            }
        }

        public GameObject Get()
        {
            if(State != PoolState.Working)
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"{this.Name} 对象池没有正常工作，不能使用Get方法");
                return null;
            }
            if(UnusedData.Count > 0)
            {
                var result = UnusedData[0];
                UnusedData.RemoveAt(0);
                UsedData.Add(result);
                return result;
            }
            else
            {
                if(TemplateData == null)
                {
                    BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"{Name} 对象池无法生成新的对象");
                    return null;
                }
                var result = GameObject.Instantiate(TemplateData);
                UsedData.Add(result);
                return result;
            }
        }

        public void Put(GameObject data)
        {
            if(State != PoolState.Working)
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"{this.Name} 对象池没有正常工作，不能使用Put方法");
                return;
            }
            if(UsedData.Contains(data))
            {
                UsedData.Remove(data);
            }
            if(!UnusedData.Contains(data))
            {
                UnusedData.Add(data);
            }
            PutAction?.Invoke(data);
        }

        public void Clear()
        {
            for(int i = 0;i<UnusedData.Count;i++)
            {
                ClearAction(UnusedData[i]);
            }
            for(int i = 0;i<UsedData.Count;i++)
            {
                ClearAction(UsedData[i]);
            }
            UsedData.Clear();
            UnusedData.Clear();
            if(TemplateData != null)
            {
                TemplateData = null;
            }
            ResourcesManager.ReleaseHandle(loadHandle);
            State = PoolState.Clear;
        }
    }
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance = null;
        public static PoolManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if(_instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(this);
            this.hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// 所有对象池的缓存
        /// </summary>
        private Dictionary<string,Pool> poolDic = new Dictionary<string, Pool>();

        /// <summary>
        /// 根据item的路径获取这个对象池的名字
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetPoolNameByPath(string path)
        {
            foreach(var item in poolDic.Values)
            {
                if(item.TemplatePath == path)
                {
                    return item.Name;
                }
            }
            return "";
        }

        /// <summary>
        /// 根据item的路径获取对象池对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Pool GetPoolByPath(string path)
        {
            foreach(var item in poolDic.Values)
            {
                if(item.TemplatePath == path)
                {
                    return item;
                }
            }
            return null;
        }

        public virtual async Task RegisterPool(string poolName,string path,Action<GameObject> putAction,Action<GameObject> clearAction)
        {
            if(poolDic.ContainsKey(poolName))
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"{poolName} 对象池已经注册过了，不能重复注册");
                return;
            }
            Pool pool = new Pool();
            pool.Name = poolName;
            pool.State = PoolState.None;
            pool.PutAction = putAction;
            pool.ClearAction = clearAction;
            pool.UsedData = new List<GameObject>();
            pool.UnusedData = new List<GameObject>();
            pool.TemplatePath = path;
            poolDic.Add(poolName,pool);
            BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"注册对象池 {poolName}");
            //初始化对象池
            await this.InitPool(poolName);
        }

        public async Task<GameObject> GetPoolObj(string poolName)
        {
            if(!poolDic.ContainsKey(poolName))
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"未注册的对象池 {poolName} GetPoolObj失败,尝试自动初始化后");
                await this.InitPool(poolName);
            }
            GameObject result = poolDic[poolName].Get();
            result.transform.SetParent(this.transform,false);
            return result;
        }

        public void PutToPool(string poolName,GameObject data)
        {
            if(data == null)
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"不能向对象池 {poolName} 放入空数据对象");
                return;
            }
            if(!poolDic.ContainsKey(poolName))
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"不能向对象池 {poolName} 放入空数据对象");
                return;
            }
            poolDic[poolName].Put(data);
        }

        public async Task InitPool(string poolName)
        {
            if(!poolDic.ContainsKey(poolName))
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"未注册的对象池 {poolName} InitPool失败");
                return;
            }
            await poolDic[poolName].Init();
        }

        public void ClearPool(string poolName)
        {
            if(!poolDic.ContainsKey(poolName))
            {
                BattleController.Instance.DebugLog(King.TurnBasedCombat.LogType.INFO,$"未注册的对象池 {poolName} ClearPool失败");
                return;
            }
            poolDic[poolName].Clear();
        }

        public void ClearAllPool()
        {
            foreach(var pool in poolDic.Values)
            {
                pool.Clear();
            }
        }
    }
}