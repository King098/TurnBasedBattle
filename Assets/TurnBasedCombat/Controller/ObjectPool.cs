using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace King.Tools
{
    /// <summary>
    /// 对象池设计
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        private static ObjectPool m_self = null;
        public static ObjectPool Instance
        {
            get
            {
                return m_self;
            }
        }
        void Awake()
        {
            m_self = this;
        }

        #region GameObject对象池逻辑
        private Dictionary<string, List<GameObject>> GameObjectPools = new Dictionary<string, List<GameObject>>();
        //清空所有对象池
        public void ClearGameObjectPools()
        {
            GameObjectPools.Clear();
        }

        //清空一个对象池
        public void ClearGameObjectPool(string name)
        {
            if (GameObjectPools.ContainsKey(name))
            {
                GameObjectPools[name].Clear();
            }
        }

        /// <summary>
        /// 初始化一个对象池
        /// </summary>
        /// <param name="name">对象池的名字</param>
        /// <param name="prefab">对象池中的对象的本体</param>
        /// <param name="number">对象池初始化的数量</param>
        public void InitGameObjectPool(string name, GameObject prefab, int number)
        {
            //如果当前不存在这个对象池,则初始化一个对象池出来
            if (!GameObjectPools.ContainsKey(name))
            {
                //将输入的数量强制变成有效值
                int count = Mathf.Clamp(number, 1, int.MaxValue);

                List<GameObject> list = new List<GameObject>();
                //开始根据数量创建对象
                for (int i = 0; i < count; i++)
                {
                    GameObject obj = Instantiate<GameObject>(prefab);
                    obj.transform.SetParent(this.transform, false);
                    obj.SetActive(false);
                    list.Add(obj);
                }
                //将创建的数据保存起来
                GameObjectPools.Add(name, list);
            }
            else
            {
                Debug.LogError(name + " ObjectPool Is Exist!");
            }
        }

        /// <summary>
        /// 是否在对象池中还有可以使用的对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasUserableGameObjectInPool(string name, out GameObject obj)
        {
            if (GameObjectPools.ContainsKey(name))
            {
                for (int i = 0; i < GameObjectPools[name].Count; i++)
                {
                    if (!GameObjectPools[name][i].activeInHierarchy)
                    {
                        obj = GameObjectPools[name][i];
                        return true;
                    }
                }
            }
            obj = null;
            return false;
        }

        /// <summary>
        /// 从对象池中获取一个对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns></returns>
        public GameObject GetGameObjectFromPool(string name)
        {
            if (GameObjectPools.ContainsKey(name))
            {
                GameObject result = null;
                if (HasUserableGameObjectInPool(name, out result))
                {
                    result.SetActive(true);
                    return result;
                }
                else
                {
                    GameObject obj = Instantiate<GameObject>(GameObjectPools[name][0]);
                    obj.transform.SetParent(this.transform, false);
                    obj.SetActive(false);
                    GameObjectPools[name].Add(obj);
                    return obj;
                }
            }
            else
            {
                Debug.LogError(name + " ObjectPool Is Not Exist! Can't Get GameObject!");
                return null;
            }
        }
        #endregion

        #region Component对象池逻辑
        private Dictionary<string, List<Component>> ComponentPools = new Dictionary<string, List<Component>>();

        //清空所有组件对象池
        public void ClearComponentPools()
        {
            ComponentPools.Clear();
        }

        //清空组件对象池数据
        public void ClearComponentPool(string name)
        {
            if (ComponentPools.ContainsKey(name))
            {
                ComponentPools[name].Clear();
            }
        }

        /// <summary>
        /// 初始化一个组件对象池
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="name">对象池名字</param>
        /// <param name="prefab">对象池源对象</param>
        /// <param name="number">初始化对象数量</param>
        public void InitComponentPools<T>(string name, GameObject prefab, int number) where T : Component
        {
            if (!ComponentPools.ContainsKey(name))
            {
                //强制让数据有效化
                int count = Mathf.Clamp(number, 1, int.MaxValue);
                //开始创建对象
                List<Component> list = new List<Component>();
                for (int i = 0; i < count; i++)
                {
                    T obj = Instantiate<GameObject>(prefab).GetComponent<T>();
                    obj.transform.SetParent(this.transform, false);
                    obj.gameObject.SetActive(false);
                    list.Add(obj);
                }
                //保存数据
                ComponentPools.Add(name, list);
            }
            else
            {
                Debug.LogError(name + " ComponenetPool Is Exist!");
            }
        }

        /// <summary>
        /// 当前是否有可用的组件对象
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="name">对象池名字</param>
        /// <param name="obj">可以用的组件对象</param>
        /// <returns></returns>
        bool HasUserableComponentInPool<T>(string name, out T obj) where T : Component
        {
            if (ComponentPools.ContainsKey(name))
            {
                for (int i = 0; i < ComponentPools[name].Count; i++)
                {
                    if (!ComponentPools[name][i].gameObject.activeInHierarchy)
                    {
                        obj = ComponentPools[name][i] as T;
                        return true;
                    }
                }
            }
            obj = null;
            return false;
        }

        /// <summary>
        /// 从组件对象池中获取一个组件对象
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="name">对象池名字</param>
        /// <returns>返回组件</returns>
        public T GetComponentFromPool<T>(string name) where T : Component
        {
            if (ComponentPools.ContainsKey(name))
            {
                T t = null;
                if (HasUserableComponentInPool<T>(name, out t))
                {
                    t.gameObject.SetActive(true);
                    return t;
                }
                else
                {
                    T obj = Instantiate<GameObject>(ComponentPools[name][0].gameObject).GetComponent<T>();
                    obj.transform.SetParent(this.transform, false);
                    obj.gameObject.SetActive(false);
                    ComponentPools[name].Add(obj);
                    return t;
                }
            }
            else
            {
                Debug.LogError(name + " ComponenetPool Is Not Exist! Can't Get Component!");
                return null;
            }
        }
        #endregion
    }
}