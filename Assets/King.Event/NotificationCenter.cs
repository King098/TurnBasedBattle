using UnityEngine;
using System;
using System.Collections.Generic;

namespace King.Event
{
    ///<summary>
    ///消息管理中心
    ///</summary>
    public class NotificationCenter
    {
        private static NotificationCenter m_self = null;
        public static NotificationCenter Instance
        {
            get
            {
                if (m_self == null)
                {
                    m_self = new NotificationCenter();
                }
                return m_self;
            }
        }

        public delegate void NotificationDelegate(Notification notific);

        ///<summary>
        ///消息监听存储
        ///</summary>
        private Dictionary<uint, NotificationDelegate> eventListeners = new Dictionary<uint, NotificationDelegate>();

		///<summary>
        ///添加一个监听消息(没有这个消息ID则创建新的，如果有则直接添加)
        ///</summary>
		public void AddListener(uint id,NotificationDelegate listener)
		{
			if(!eventListeners.ContainsKey(id))
			{
				NotificationDelegate noti = null;
				eventListeners[id] = noti;
			}
			eventListeners[id] += listener;
		}

		///<summary>
        ///删除一个监听消息
        ///</summary>
		public void RemoveListener(uint id,NotificationDelegate listener)
		{
			if(!eventListeners.ContainsKey(id))
			{
				return;
			}
			eventListeners[id] -= listener;
			if(eventListeners[id] == null)
			{
				eventListeners.Remove(id);
			}
		}

		///<summary>
        ///删除所有对应ID的监听消息
        ///</summary>
		public void RemoveListeners(uint id)
		{
			if(!eventListeners.ContainsKey(id))
			{
				return;
			}
			eventListeners.Remove(id);
		}

		///<summary>
        ///分发消息，自定义传入参数
        ///</summary>
		public void DispatchEvent(uint id,Notification notific)
		{
			if(!eventListeners.ContainsKey(id))
			{
				return;
			}
			eventListeners[id](notific);
		}

		///<summary>
        ///分发消息，需要知道发送者和消息内容
        ///</summary>
		public void DispatchEvent(uint id,GameObject sender,EventArgs args)
		{
			if(!eventListeners.ContainsKey(id))
			{
				return;
			}
			eventListeners[id](new Notification(sender,args));
		}

		///<summary>
        ///分发消息，不需要指定发送者，只需要消息内容
        ///</summary>
		public void DispatchEvent(uint id,EventArgs args)
		{
			if(!eventListeners.ContainsKey(id))
			{
				return;
			}
			eventListeners[id](new Notification(args));
		}

		///<summary>
        ///分发消息，不需要指定任何内容
        ///</summary>
		public void DispatchEvent(uint id)
		{
			if(!eventListeners.ContainsKey(id))
			{
				return;
			}
			eventListeners[id](new Notification());
		}
    }
}