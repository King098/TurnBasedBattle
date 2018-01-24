using UnityEngine;
using System;

namespace King.Event
{
    ///<summary>
    ///消息发送体
    ///</summary>
    public class Notification
    {
        ///<summary>
        ///消息发送者
        ///</summary>
        public GameObject sender;
        ///<summary>
        ///消息内容
        ///</summary>
        public EventArgs args;

        ///<summary>
        ///空构造函数
        ///</summary>
        public Notification() { }

        ///<summary>
        ///指定发送者和发送内容的构造函数
        ///</summary>
        public Notification(GameObject sender, EventArgs args)
        {
            this.sender = sender;
            this.args = args;
        }

        ///<summary>
        ///只指定发送内容的构造函数
        ///</summary>
        public Notification(EventArgs args)
        {
            this.sender = null;
            this.args = args;
        }
    }
}