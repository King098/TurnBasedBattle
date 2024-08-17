using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AUIFramework
{
    public class TimerNode
    {
        public Action<TimerNode,object[]> callback;
        public float duration;//重复触发间隔
        public float delay;//定时器启动延时
        public int repeat;//定时器重复次数
        public float passedTime;//当前定时器经过的时长
        public float timerScale;//定时器倍速
        public object[] param;//自定义回调参数
        public bool isRemoved;//是否待删除
        public string timerId;//定时器id
        public bool isPaused;//是否暂停了
        public string tag;//定时器标识组
    }

    public class TimerManager : MonoBehaviour
    {
        private static TimerManager _instance = null;
        public static TimerManager Instance
        {
            get
            {
                return _instance;
            }
        }

        internal List<TimerNode> timerList = new List<TimerNode>();
        internal List<TimerNode> newTimerList = new List<TimerNode>();
        // internal List<string> removedTimerIds = new List<string>();
        // internal List<string> removedTimerTags = new List<string>();
        internal List<TimerNode> removedTimerList = new List<TimerNode>();
        internal Dictionary<string,float> timerScaleTagList = new Dictionary<string, float>();
        internal Dictionary<string,float> timerScaleIdList = new Dictionary<string, float>();

        private static int timerIdNum = 0;

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
            DontDestroyOnLoad(this.gameObject);

            timerIdNum = 0;
        }


        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            timerList.Clear();
            newTimerList.Clear();
            // removedTimerIds.Clear();
            removedTimerList.Clear();
            // removedTimerTags.Clear();
            timerScaleIdList.Clear();
            timerScaleTagList.Clear();
        }

        public string AddTimer(Action<TimerNode,object[]> callback,float delay = 0,int repeat = 1,float duration = 0f,float timerScale = 1f,string tag = "",params object[] param)
        {
            TimerNode timer = new TimerNode();
            timer.callback = callback;
            timer.duration = duration;
            timer.delay = delay;
            timer.repeat = repeat;
            timer.isPaused = false;
            timer.timerId = $"timer_{timerIdNum}";
            timer.passedTime = 0f;
            timer.isRemoved = false;
            timer.tag = tag;
            timer.timerScale = timerScale;
            timer.param = param;
            timerIdNum++;

            this.newTimerList.Add(timer);
            return timer.timerId;
        }

        public void RemoveTimer(TimerNode timer)
        {
            timer.isRemoved = true;
        }

        public void RemoveTimerById(string timerId)
        {
            // this.removedTimerIds.Add(timerId);
            TimerNode timer = this.timerList.Find(t=>t.timerId == timerId);
            if(timer != null)
            {
                timer.isRemoved = true;
            }
        }

        public void RemoveTimerByTag(string tag)
        {
            // this.removedTimerTags.Add(tag);
            List<TimerNode> timers = this.timerList.FindAll(t=>t.tag == tag);
            foreach(var t in timers)
            {
                t.isRemoved = true;
            }
        }

        public void SetTimerScaleByTag(string tag,float timerScale)
        {
            this.timerScaleTagList.Add(tag,timerScale);
        }

        public void SetTimerScaleById(string timerId,float timerScale)
        {
            this.timerScaleIdList.Add(tag,timerScale);
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate()
        {
            while(this.newTimerList.Count > 0)
            {
                var timer = this.newTimerList[0];
                this.newTimerList.RemoveAt(0);

                // if(!timer.isRemoved && this.removedTimerIds.IndexOf(timer.timerId) >= 0)
                // {
                //     timer.isRemoved = true;
                // }
                // if(!timer.isRemoved && this.removedTimerTags.IndexOf(timer.timerId) >= 0)
                // {
                //     timer.isRemoved = true;
                // }
                if(this.timerScaleIdList.ContainsKey(timer.timerId))
                {
                    timer.timerScale = this.timerScaleIdList[timer.timerId];
                }
                if(this.timerScaleTagList.ContainsKey(timer.tag))
                {
                    timer.timerScale = this.timerScaleTagList[timer.tag];
                }
                this.timerList.Add(timer);
            }
            for(int i = 0;i<this.timerList.Count;i++)
            {
                var timer = this.timerList[i];
                // if(!timer.isRemoved && this.removedTimerIds.IndexOf(timer.timerId) >= 0)
                // {
                //     timer.isRemoved = true;
                // }
                // if(!timer.isRemoved && this.removedTimerTags.IndexOf(timer.timerId) >= 0)
                // {
                //     timer.isRemoved = true;
                // }
                if(timer.isRemoved)
                {
                    this.removedTimerList.Add(timer);
                    continue;
                }

                if(timer.isPaused)
                {
                    continue;
                }
                if(this.timerScaleIdList.ContainsKey(timer.timerId))
                {
                    timer.timerScale = this.timerScaleIdList[timer.timerId];
                }
                if(this.timerScaleTagList.ContainsKey(timer.tag))
                {
                    timer.timerScale = this.timerScaleTagList[timer.tag];
                }
                var add = Time.fixedDeltaTime * timer.timerScale;
                timer.passedTime += add;
                //重复间隔
                if(timer.passedTime >= timer.delay + timer.duration)
                {
                    timer.callback?.Invoke(timer,timer.param);
                    timer.repeat--;
                    timer.passedTime -= (timer.delay + timer.duration);
                    timer.delay = 0;

                    if(timer.repeat == 0)
                    {
                        timer.isRemoved = true;
                        this.removedTimerList.Add(timer);
                    }
                    else if(timer.repeat < 0)
                    {
                        timer.repeat = -1;
                    }
                }
            }
            // this.removedTimerIds.Clear();
            // this.removedTimerTags.Clear();
            this.timerScaleIdList.Clear();
            this.timerScaleTagList.Clear();
            //删除timer
            for(int i = 0;i<removedTimerList.Count;i++)
            {
                this.timerList.Remove(removedTimerList[i]);
            }
            this.removedTimerList.Clear();
        }
    }
}